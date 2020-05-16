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
    /// Catches values published by the CanMessageHandler and stores the values in the associated parameter. Publishes the parsed values
    /// for use by the GUI. If the value is not a recognized parameter, it will be stored for use by the PID Sniffer (only applicable
    /// to J1708; there's no way to determine the intended parameter ID of an unknown J1939 message segment).
    /// </summary>
    public class PIDValueStager
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
                        else
                        {
                            PublishUnparsedData(segment);
                        }
                        break;
                    case VMSDataSource.J1939:
                        if (segment.ParseStatus == ParseStatus.Parsed)
                        {
                            UpdateWithJ1939(segment);
                        }
                        else
                        {
                            PublishUnparsedData(segment);
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
                    case ParseBehavior.PRIORITIZE_1708:
                        if (Parameter.J1708Value.TimeReceived < DateTime.Now.AddMinutes(-1))
                        {
                            UpdateAndPublish(Parameter.J1939Value, segment);
                        }
                        break;
                    case ParseBehavior.IGNORE_1939:
                        break;
                }
            }

            public void UpdateAndPublish(PidValue pidValue, CanMessageSegment segment)
            {
                if (pidValue == null)
                    pidValue = new PidValue();
                pidValue.RawValue = segment.RawValue;
                pidValue.StandardValue = segment.StandardValue;
                pidValue.MetricValue = segment.MetricValue;
                pidValue.TimeReceived = segment.TimeReceived;
                Parameter.LastValue = pidValue.StandardValue;
                Parameter.LastMetricValue = pidValue.MetricValue;
                var e = new VMSPidValueEventArgs(EventIDs.PID_BASE | Parameter.Pid, Parameter.LastValue)
                {
                    segment = segment,
                };
                RaiseVMSEvent?.Invoke(this, e);
                //VMSConsole.PrintLine("Time to parse: " + (segment.TimeParsed - segment.TimeReceived));
            }

            public void PublishUnparsedData(CanMessageSegment segment)
            {
                VMSRawDataEventArgs e;
                switch (segment.DataSource)
                {
                    case VMSDataSource.J1708:
                        e = new VMSJ1708RawDataEventArgs(segment as J1708MessageSegment);
                        RaiseVMSEvent?.Invoke(this, e);
                        break;
                    case VMSDataSource.J1939:
                        e = new VMSJ1939RawDataEventArgs(segment as J1939MessageSegment);
                        RaiseVMSEvent?.Invoke(this, e);
                        break;
                }
            }
        }

        private List<ParameterWrapper> ParameterWrappers;

        public PIDValueStager()
        {
            ParameterWrappers = new List<ParameterWrapper>();
            foreach (var param in ConfigManager.ParamData.Contents.Parameters)
            {
                ParameterWrappers.Add(new ParameterWrapper(param));
            }
            //Add all parameters that don't exist in the Parameter List. This allows us to publish all values, regardless of parameter assignment
            for (ushort i = 0; i < 65530; i++)
            {
                if (ConfigManager.ParamData.GetParam(i) == null)
                {
                    ParameterWrappers.Add(
                        new ParameterWrapper(
                            new JParameter() { 
                                Pid = i
                            }
                            )
                        );
                }
            }
        }
    }
}
