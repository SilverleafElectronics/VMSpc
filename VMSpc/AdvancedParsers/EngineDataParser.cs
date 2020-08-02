using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Parsers;

namespace VMSpc.AdvancedParsers
{
    public class EngineDataParser : AdvancedParser, IEventConsumer, IEventPublisher
    {
        public double CurrentRPMs { get; private set; }
        public double CurrentLoadPercent { get; private set; }
        public EngineDataParser()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | 190);
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | 92);
            EventBridge.Instance.AddEventPublisher(this);
            CurrentRPMs = double.NaN;
            CurrentLoadPercent = double.NaN;
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        public void ConsumeEvent(VMSEventArgs e)
        {
            var message = (e as VMSPidValueEventArgs);
            switch (message.segment.Pid)
            {
                case 190:
                    CurrentRPMs = message.segment.RawValue;
                    break;
                case 92:
                    CurrentLoadPercent = message.segment.StandardValue;
                    break;
            }
            if (!double.IsNaN(CurrentRPMs) && !double.IsNaN(CurrentLoadPercent))
            {
                var HP = EngineSpec.CalculateHorsepower(CurrentRPMs, CurrentLoadPercent);
                var Torque = EngineSpec.CalculateTorque(CurrentRPMs, CurrentLoadPercent);
                var torqueSegment = message.segment.DeepCopy();
                torqueSegment.Pid = 510;
                torqueSegment.StandardValue = Torque;
                var hpSegment = message.segment.DeepCopy();
                hpSegment.Pid = 509;
                hpSegment.StandardValue = HP;
                RaiseVMSEvent?.Invoke(this, new VMSParsedDataEventArgs(510, torqueSegment));
                RaiseVMSEvent?.Invoke(this, new VMSParsedDataEventArgs(509, hpSegment));
                /*
                RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs(
                    (EventIDs.PID_BASE | 510),
                    Torque
                    ));
                RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs(
                    (EventIDs.PID_BASE | 509),
                    HP
                    ));
                */
            }
        }
    }
}
