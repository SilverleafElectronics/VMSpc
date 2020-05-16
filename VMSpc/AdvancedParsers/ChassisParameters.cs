using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Enums.Parsing;
using VMSpc.Parsers;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.AdvancedParsers
{
    public sealed class ChassisParameters : AdvancedParser, IEventConsumer, IEventPublisher, ISingleton
    {
        private readonly JsonFileManagers.SettingsContents Settings = ConfigManager.Settings.Contents;
        private readonly JsonFileManagers.MeterContents Meters = ConfigManager.Meters.Contents;
        public static ChassisParameters Instance { get; private set; }
        public ushort OdometerPid;
        public double CurrentMiles
        {
            get => Meters.Odometer;
            private set => Meters.Odometer = value;
        }
        public double CurrentFuel
        {
            get => Meters.Fuelmeter;
            private set => Meters.Fuelmeter = value;
        }
        public double CurrentEngineHours
        {
            get => Meters.Hourmeter;
            private set => Meters.Hourmeter = value;
        }
        public string RangeSelected { get; private set; } = "?";
        public string RangeAttained { get; private set; } = "?";
        static ChassisParameters() { }
        private ChassisParameters()
        {
            SetOdometerType(Settings.odometerType);
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.totalFuel));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.engineHours));

            //Subscribe to J1939 Odometer/Fuel/Hours events
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0x0FEE9));   //PGN_FUEL
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0x0FEC1));   //PGN_ODOMETER
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0x0FEE5));   //PGN_HOURMETER

            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.rangeSelected));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.rangeAttained));
            EventBridge.Instance.AddEventPublisher(this);

            CurrentMiles = Meters.Odometer;
            CurrentFuel = Meters.Fuelmeter;
            CurrentEngineHours = Meters.Hourmeter;
        }

        public static void Initialize()
        {
            Instance = new ChassisParameters();
        }

        public void SetOdometerType(OdometerType OdometerType)
        {
            EventBridge.Instance.UnsubscribeFromEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.totalMiles));
            EventBridge.Instance.UnsubscribeFromEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.totalMilesCummins));
            switch (OdometerType)
            {
                case OdometerType.Standard:
                    EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.totalMiles));
                    OdometerPid = PIDWrapper.totalMiles;
                    break;
                case OdometerType.Cummins:
                    EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(PIDWrapper.totalMilesCummins));
                    OdometerPid = PIDWrapper.totalMilesCummins;
                    break;
            }
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        public void ConsumeEvent(VMSEventArgs e)
        {
            var segment = (e as VMSRawDataEventArgs).messageSegment;
            switch (segment.ParseStatus)
            {
                case ParseStatus.NotParsed:
                    ParseUnparsedData(segment);
                    break;
                case ParseStatus.PartiallyParsed:
                    ParsePartiallyParsedData(segment);
                    break;
                default:
                    break;
            }
        }

        private void PublishEvent(VMSEventArgs e)
        {
            RaiseVMSEvent?.Invoke(this, e);
        }

        /// <summary>
        /// Parses partially parsed message segments. The databus shouldn't matter, since J1939
        /// assigns a PID on partially parsed datum
        /// </summary>
        /// <param name="segment"></param>
        private void ParsePartiallyParsedData(CanMessageSegment segment)
        {
            bool parsingCompleted = false;
            switch (segment.Pid)
            {
                case PIDWrapper.totalMiles:
                case PIDWrapper.totalMilesCummins:
                    if (segment.StandardValue > CurrentMiles && segment.StandardValue < 2000000)
                    {
                        CurrentMiles = segment.StandardValue;
                        parsingCompleted = true;
                    }
                    break;
                case PIDWrapper.totalFuel:
                    if (segment.StandardValue > CurrentFuel && segment.StandardValue < 25000)
                    {
                        CurrentFuel = segment.StandardValue;
                        parsingCompleted = true;
                    }
                    break;
                case PIDWrapper.engineHours:
                    if (segment.StandardValue > CurrentEngineHours && segment.StandardValue < 50000)
                    {
                        CurrentEngineHours = segment.StandardValue;
                        parsingCompleted = true;
                    }
                    break;
            }
            if (parsingCompleted)
            {
                PublishEvent(new VMSParsedDataEventArgs(segment.Pid, segment));
            }
            else
            {
                PublishMeters();
            }
        }

        private void ParseUnparsedData(CanMessageSegment segment)
        {
            switch (segment.DataSource)
            {
                case VMSDataSource.J1708:
                    ParseJ1708RawData(segment as J1708MessageSegment);
                    break;
                case VMSDataSource.J1939:
                    ParseJ1939RawData(segment as J1939MessageSegment);
                    break;
                default:
                    break;
            }
        }

        private void ParseJ1708RawData(J1708MessageSegment segment)
        {
            switch (segment.Pid)
            {
                case PIDWrapper.rangeSelected:
                    RangeSelected = Convert.ToChar(segment.RawData[1]).ToString();
                    break;
                case PIDWrapper.rangeAttained:
                    RangeAttained = Convert.ToChar(segment.RawData[1]).ToString();
                    break;
            }
        }

        private void ParseJ1939RawData(J1939MessageSegment segment)
        {

        }

        private void SetTransmissionData(CanMessageSegment segment)
        {
            double transmissionMode = 0;
            RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs(EventIDs.PID_BASE | 13, transmissionMode));
        }

        /// <summary>
        /// Publishes Odometer, Fuelmeter, and Hourmeter
        /// </summary>
        private void PublishMeters()
        {
            PublishEvent(new VMSPidValueEventArgs(EventIDs.PID_BASE | OdometerPid, CurrentMiles));
            PublishEvent(new VMSPidValueEventArgs(EventIDs.PID_BASE | PIDWrapper.totalFuel, CurrentFuel));
            PublishEvent(new VMSPidValueEventArgs(EventIDs.PID_BASE | PIDWrapper.engineHours, CurrentEngineHours));
        }
    }
}
