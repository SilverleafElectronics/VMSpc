using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;

namespace VMSpc.AdvancedParsers
{
    public class Acceleration : AdvancedParser, IEventConsumer, IEventPublisher
    {
        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        private double LastSpeed;
        private double NewSpeed;

        public Acceleration()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | 84);
            EventBridge.Instance.AddEventPublisher(this);
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            NewSpeed = (e as VMSPidValueEventArgs).segment.StandardValue;
            Calculate();
        }

        private void Calculate()
        {
            if (!double.IsNaN(LastSpeed))
            {
                double acceleration = (NewSpeed - LastSpeed);
                double metricAcceleration = (acceleration * 1.60934);
                RaiseVMSEvent?.Invoke(this, new VMSPidValueEventArgs((EventIDs.PID_BASE | 10), acceleration));
            }
            LastSpeed = NewSpeed;
        }
    }
}
