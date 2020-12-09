using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.JsonFileManagers;
using VMSpc.Enums.Parsing;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Parsers;
using VMSpc.DevHelpers;

namespace VMSpc
{
    /// <summary>
    /// <para>
    /// Catches values published by the CanMessageHandler and stores the values in the associated parameter. Publishes the parsed values
    /// for use by the GUI. If the value is not a recognized parameter, it will be stored for use by the PID Sniffer (only applicable
    /// to J1708; there's no way to determine the intended parameter ID of an unknown J1939 message segment).
    /// </para>
    /// <para>
    /// For interacting with Advanced Parsers: Advanced Parsers catch values submitted from here, either through fully or partially
    /// parsed data. Advanced Parsers parse this data, and if they are meant to submit a PID value, they route the parsed data
    /// back here for storage. Advanced parsers should publish this VMSParsedDataEvent using an InferredMessageSegment.
    /// </para>
    /// </summary>
    public class PIDValueStager : IEventConsumer, ISingleton
    {
        public class ParameterWrapper : IEventConsumer, IEventPublisher
        {
            public JParameter Parameter { get; set; }

            public ParameterWrapper(JParameter Parameter)
            {
                this.Parameter = Parameter;
                EventBridge.Instance.SubscribeToEvent(this, (EventIDs.PARSED_DATA_EVENT | Parameter.Pid));
                EventBridge.Instance.AddEventPublisher(this);
            }

            public event EventHandler<VMSEventArgs> RaiseVMSEvent;

            /// <summary>
            /// The ParameterWrapper consumes a VMSParsedDataEventArg. If the data is parsed, it will store the value and publish
            /// a PidValueEventArg. If the data is not parsed, it will publish a RawDataEventArg. This raw data will either be discarded
            /// or caught by a advanced parser. If the advanced parser manipulates standard PID data, it will republish the VMSParsedDataEventArg,
            /// which will then be caught again here.
            /// </summary>
            /// <param name="e"></param>
            public void ConsumeEvent(VMSEventArgs e)
            {
                var segment = (e as VMSParsedDataEventArgs).messageSegment;
                switch (segment.DataSource)
                {
                    case VMSDataSource.J1708:
                        if (segment.ParseStatus == ParseStatus.Parsed)
                        {
                            UpdateWithJ1708(segment);
                        }
                        break;
                    case VMSDataSource.J1939:
                        if (segment.ParseStatus == ParseStatus.Parsed)
                        {
                            UpdateWithJ1939(segment);
                        }
                        break;
                    case VMSDataSource.Inferred:
                        if (segment.ParseStatus == ParseStatus.Parsed)
                        {
                            UpdateWithInferred(segment);
                        }
                        break;
                }
            }

            public void UpdateWithJ1708(CanMessageSegment segment)
            {
                switch (ConfigManager.Settings.Contents.globalParseBehavior)
                {
                    case ParseBehavior.IGNORE_1708:
                        break;
                    case ParseBehavior.PRIORITIZE_1939:
                        if (Parameter.J1939Value.TimeReceived < DateTime.Now.AddMinutes(-1))
                        {
                            UpdateAndPublish(Parameter.J1708Value, segment);
                        }
                        break;
                    case ParseBehavior.IGNORE_1939:
                    case ParseBehavior.PARSE_ALL:
                    case ParseBehavior.PRIORITIZE_1708:
                        UpdateAndPublish(Parameter.J1708Value, segment);
                        break;
                }
            }

            public void UpdateWithJ1939(CanMessageSegment segment)
            {
                switch (ConfigManager.Settings.Contents.globalParseBehavior)
                {
                    case ParseBehavior.IGNORE_1708:
                    case ParseBehavior.PRIORITIZE_1939:
                    case ParseBehavior.PARSE_ALL:
                        UpdateAndPublish(Parameter.J1939Value, segment);
                        break;
                    case ParseBehavior.PRIORITIZE_1708: //Waits until the J1708 Value for this PID is over a minute old
                        if (Parameter.J1708Value.TimeReceived < DateTime.Now.AddMinutes(-1))
                        {
                            UpdateAndPublish(Parameter.J1939Value, segment);
                        }
                        break;
                    case ParseBehavior.IGNORE_1939:
                        break;
                }
            }

            public void UpdateWithInferred(CanMessageSegment segment)
            {
                UpdateAndPublish(Parameter.J1939Value, segment);
            }

            public void UpdateAndPublish(PidValue pidValue, CanMessageSegment segment)
            {
                if (pidValue == null)
                    pidValue = new PidValue();
                pidValue.RawValue = segment.RawValue;
                pidValue.StandardValue = segment.StandardValue * Parameter.Multiplier + Parameter.Offset;
                pidValue.MetricValue = segment.MetricValue * Parameter.Multiplier + Parameter.Offset;
                pidValue.TimeReceived = segment.TimeReceived;
                Parameter.LastValue = pidValue.StandardValue;
                Parameter.LastMetricValue = pidValue.MetricValue;
                Parameter.Seen = true;
                var e = new VMSPidValueEventArgs(EventIDs.PID_BASE | Parameter.Pid, Parameter.LastValue)
                {
                    segment = segment,
                };
                RaiseVMSEvent?.Invoke(this, e);
                //VMSConsole.PrintLine("Time to parse: " + (segment.TimeParsed - segment.TimeReceived));
            }
        }

        static PIDValueStager() { }
        public static PIDValueStager Instance;

        public static void Initialize()
        {
            Instance = new PIDValueStager();
        }

        private List<ParameterWrapper> ParameterWrappers;
        private List<ushort> RecognizedPIDs;

        private PIDValueStager()
        {
            AddParameters();
            //Subscribe to the generic PARSED_DATA_EVENT. This stager catches all PIDs to use unrecognized ones in the PIDSniffer
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PARSED_DATA_EVENT | 0xFFFF);
        }

        public void Reset()
        {
            foreach (var wrapper in ParameterWrappers)
            {
                EventBridge.Instance.UnsubscribeFromAllEvents(wrapper);
                EventBridge.Instance.RemoveEventPublisher(wrapper);
            }
            AddParameters();
        }

        public JParameter GetParameter(ushort pid)
        {
            foreach (var wrapper in ParameterWrappers)
            {
                if (wrapper.Parameter.Pid == pid)
                {
                    return wrapper.Parameter;
                }
            }
            return null;
        }

        private void AddParameters()
        {
            ParameterWrappers = new List<ParameterWrapper>();
            RecognizedPIDs = new List<ushort>();
            foreach (var param in ConfigManager.ParamData.Contents.Parameters)
            {
                ParameterWrappers.Add(new ParameterWrapper(param));
                RecognizedPIDs.Add(param.Pid);
            }
            RecognizedPIDs.Sort();
        }

        /// <summary>
        /// Parses all ParsedDataEvents, regardless of whether or not they are recognized. Because of this constraint, we keep the RecognizedPIDs List sorted
        /// so that we can perform a binary search at each event consumption.
        /// </summary>
        /// <param name="e"></param>
        public void ConsumeEvent(VMSEventArgs e)
        {
            var ev = (e as VMSParsedDataEventArgs);
            if (ev != null && ev.messageSegment != null)
            {
                var pid = ev.messageSegment.Pid;
                var index = RecognizedPIDs.BinarySearch(pid);
                if (index < -1)
                {
                    //List.BinarySearch() returns the bitwise complement of the next largest element's index
                    //in the list when the value isn't found. This means that we can take the complement again
                    //for the insertion to avoid sorting every time a new value is added.
                    AddUnrecognizedParameter(~index, pid);
                }
            }
        }

        private void AddUnrecognizedParameter(int index, ushort pid)
        {
            JParameter param = new JParameter()
            {
                Pid = pid,
                LowRed = 0,
                LowYellow = 0,
                HighRed = 0,
                HighYellow = 0,
                GaugeMin = 0,
                GaugeMax = 100,
                Abbreviation = "UNKNOWN",
                ParamName = $"Unrecognized Pid: {pid}",
                Multiplier = 1,
                Offset = 0,
                Unit = "UNKNOWN",
                MetricUnit = "UNKOWN",
                Format = "{0:0.#}",
            };
            RecognizedPIDs.Insert(index, pid);
            ParameterWrappers.Add(new ParameterWrapper(param));
            ConfigManager.ParamData.AddParam(param);
        }
    }
}
