using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;

namespace VMSpc.AdvancedParsers
{
    public class EngineDataParser : AdvancedParser, IEventConsumer, IEventPublisher
    {
        public double CurrentRPMs { get; private set; } = double.NaN;
        public double CurrentLoadPercent { get; private set; } = double.NaN;
        public EngineDataParser()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | 190);
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | 92);
            EventBridge.Instance.AddEventPublisher(this);
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
                RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs(
                    (EventIDs.PID_BASE | 510),
                    Torque
                    ));
                RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs(
                    (EventIDs.PID_BASE | 509),
                    HP
                    ));
            }
        }
    }
}
