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
        public double CurrentKilometers => CurrentMiles * 1.60934;
        public double CurrentFuelGallons
        {
            get => Meters.Fuelmeter;
            private set => Meters.Fuelmeter = value;
        }
        public double CurrentFuelLiters => CurrentFuelGallons * 3.78541;
        public double CurrentEngineHours
        {
            get => Meters.Hourmeter;
            private set => Meters.Hourmeter = value;
        }
        public double AverageSpeed => CurrentMiles / CurrentEngineHours;
        public double RollingMPG => CurrentMiles / CurrentFuelGallons;
        public double RollingLitersPer100k => RollingMPG * 235.214583;
        public double RecentMPG => CurrentMiles / CurrentFuelGallons;
        public double RecentLitersPer100k => RecentMPG * 235.214583;
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

            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0x0F005));   //Gear selected/attained


            EventBridge.Instance.AddEventPublisher(this);

            CurrentMiles = Meters.Odometer;
            CurrentFuelGallons = Meters.Fuelmeter;
            CurrentEngineHours = Meters.Hourmeter;
            PublishMeters();
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

        private void PublishEvent(ushort pid, double value)
        {
            PublishEvent(new VMSParsedDataEventArgs(pid, new InferredMessageSegment(pid, value)));
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
                    if (segment.StandardValue > CurrentFuelGallons && segment.StandardValue < 25000)
                    {
                        CurrentFuelGallons = segment.StandardValue;
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
            switch (segment.PGN)
            {
                case 0x0F005:
                    if (segment.RawData[5] < 250)
                        RangeSelected = Convert.ToChar(segment.RawData[5]).ToString();
                    if (segment.RawData[3] < 125)
                        RangeAttained = "R";
                    else if (segment.RawData[3] == 125)
                        RangeAttained = "N";
                    else if (segment.RawData[3] < 135)
                        RangeAttained = Convert.ToChar(segment.RawData[3] - 125).ToString() + "0";
                    else if (segment.RawData[3] < 161)
                        RangeAttained = Convert.ToChar(segment.RawData[3] - 135).ToString() + "a";
                    else if (segment.RawData[3] == 251)
                        RangeAttained = "P";
                    SetTransmissionData(segment);
                    break;
            }
        }

        private void SetTransmissionData(CanMessageSegment segment)
        {
            double transmissionMode = 0;
            switch (segment.RawData[4])
            {
                case 16:
                case 21:
                case 32:
                    transmissionMode = 1;
                    break;
                case 5:
                case 26:
                case 31:
                    transmissionMode = 2;
                    break;
            }
            if (transmissionMode != 0)
                RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs(EventIDs.PID_BASE | 13, transmissionMode));
        }

        /// <summary>
        /// Publishes Odometer, Fuelmeter, and Hourmeter
        /// </summary>
        private void PublishMeters()
        {
            PublishEvent(OdometerPid, CurrentMiles);
            PublishEvent(PIDWrapper.totalFuel, CurrentFuelGallons);
            PublishEvent(PIDWrapper.engineHours, CurrentEngineHours);
            if (
                !((double.IsNaN(CurrentMiles)) || (CurrentMiles < 0)) &&
                !((double.IsNaN(CurrentFuelGallons)) || (CurrentFuelGallons < 0)) &&
                !((double.IsNaN(CurrentEngineHours)) || (CurrentEngineHours < 0))
                )
            {
                PublishEvent(9, RollingMPG);
                PublishEvent(502, RecentMPG);
                PublishEvent(601, RollingLitersPer100k);
                PublishEvent(602, RecentLitersPer100k);
                PublishEvent(512, AverageSpeed);
            }
        }
    }
}
