using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Parsers;

namespace VMSpc.AdvancedParsers
{
    public class Trackers
    {
        private MaxTracker MaxCoolant = new MaxTracker(503, 110, 2000, 12000, 999.9);
        private MaxTracker MaxTransmission = new MaxTracker(504, 4, 2000, 12000, 999.9);
        private MaxTracker MaxOil = new MaxTracker(505, 175, 2000, 12000, 999.9);
        private MaxTracker MaxManifoldTemp = new MaxTracker(506, 105, 2000, 12000, 999.9);
        private MaxTracker MaxRPMs = new MaxTracker(507, 190, 2000, 12000, 9999.9);
        private MaxTracker MaxSpeed = new MaxTracker(508, 84, 2000, 12000, 199.9);
    }

    public abstract class Tracker : AdvancedParser, IEventPublisher
    {
        protected double TrackedValue = double.NaN;
        protected double TrackedMetricValue = double.NaN;
        protected ushort TrackedValuePID;
        protected ushort PublishedPID;
        protected double Value;
        protected double MetricValue;

        public Tracker(ushort PublishedPID, ushort TrackedValuePID)
        {
            this.TrackedValuePID = TrackedValuePID;
            this.PublishedPID = PublishedPID;

            EventBridge.Instance.AddEventPublisher(this);
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        protected void PublishEvent(VMSEventArgs e)
        {
            RaiseVMSEvent?.Invoke(this, e);
        }

        protected void PublishValues()
        {
            var segment = new InferredMessageSegment()
            {
                Pid = PublishedPID,
                ParseStatus = Enums.Parsing.ParseStatus.Parsed,
                TimeParsed = DateTime.Now,
                TimeReceived = DateTime.Now,
                StandardValue = Value,
                MetricValue = Value,
            };
            var e = new VMSParsedDataEventArgs(PublishedPID, segment);
            PublishEvent(e);
        }
    }

    public class MaxTracker : Tracker, IEventConsumer
    {
        private double MaxValue;
        private TimeValuePair[] ValueArray;
        private ulong TimeSpan;
        private int BufferSize, Left, Right;
        uint counter;

        public MaxTracker(ushort PublishedPID, ushort TrackedValuePID, int BufferSize, ulong TimeSpan, double MaxValue = 999999.9)
            :base(PublishedPID, TrackedValuePID)
        {
            this.MaxValue = MaxValue;
            this.BufferSize = BufferSize;
            this.TimeSpan = TimeSpan;
            this.MaxValue = MaxValue;
            ValueArray = new TimeValuePair[BufferSize];
            counter = 0;

            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | TrackedValuePID);
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            var newValue = (e as VMSPidValueEventArgs).segment.StandardValue;
            var newMetricValue = (e as VMSPidValueEventArgs).segment.MetricValue;
            counter++;
            TimeValuePair recordStruct = new TimeValuePair(counter, newMetricValue, newValue);
            if (recordStruct.value > MaxValue)
                return;
            if (recordStruct.value >= ValueArray[Right].value)
            {
                ValueArray[Left] = recordStruct;
                Right = Left;
                Value = ValueArray[Right].value;
                MetricValue = ValueArray[Right].valueMetric;
                PublishValues();
                return;
            }
            bool roomForMore = (ValueArray[Left].value < recordStruct.value);
            while (ValueArray[Left].value < recordStruct.value)
                Constants.SafeIndexAdd(ref Left, 1, BufferSize);
            Constants.SafeIndexAdd(ref Left, -1, BufferSize);

            if (Left != Right)
                roomForMore = true;
            if (roomForMore)
                ValueArray[Left] = recordStruct;
            else
            {
                Constants.SafeIndexAdd(ref Left, 1, BufferSize);
                if (ValueArray[Left].time < (counter - TimeSpan))
                {
                    ValueArray[Left] = recordStruct;
                }
            }
            if (counter > TimeSpan)
                while (ValueArray[Right].time < (counter - TimeSpan))
                    Constants.SafeIndexAdd(ref Right, -1, BufferSize);
        }

        protected struct TimeValuePair
        {
            public ulong time;
            public double valueMetric;
            public double value;
            public TimeValuePair(ulong time, double valueMetric, double value) { this.time = time; this.valueMetric = valueMetric; this.value = value; }
        }
    }

    public class AverageTracker : Tracker, IEventConsumer
    {
        public ushort SecondTrackedValuePID;
        private double SecondTrackedValue = double.NaN;
        private double SecondTrackedMetricValue = double.NaN;
        public AverageTracker(ushort PublishedPID, ushort TrackedValuePID, ushort SecondTrackedValuePID)
            :base(PublishedPID, TrackedValuePID)
        {
            this.SecondTrackedValuePID = SecondTrackedValuePID;

            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | TrackedValuePID);
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | SecondTrackedValuePID);
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            var segment = (e as VMSParsedDataEventArgs).messageSegment;
            if (segment == null)
                return;
            if (segment.Pid == TrackedValuePID)
            {
                TrackedValue = segment.StandardValue;
                TrackedMetricValue = segment.MetricValue;
            }
            else if (segment.Pid == SecondTrackedValuePID)
            {
                SecondTrackedValue = segment.StandardValue;
                SecondTrackedMetricValue = segment.MetricValue;
            }

            if (!double.IsNaN(TrackedValue) && !double.IsNaN(SecondTrackedValue) && SecondTrackedValue != 0)
            {
                Value = (TrackedValue / SecondTrackedMetricValue);
                MetricValue = (TrackedMetricValue / SecondTrackedMetricValue);
                PublishValues();
            }
        }
    }
}
