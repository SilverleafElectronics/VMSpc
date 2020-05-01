using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using VMSpc.Common;

namespace VMSpc.UI.Managers
{
    public abstract class ComponentManager
    {
        public byte managerInstance { get; set; }
        public static byte ManagerEventInstance = 1;
        private Timer UpdateTimer;

        public ComponentManager()
        {
            managerInstance = ManagerEventInstance;
            ManagerEventInstance++;
            UpdateTimer = Constants.CREATE_TIMER(UpdateValues, 1000);
        }

        ~ComponentManager()
        {
            Disable();
        }

        public void Disable()
        {
            UpdateTimer?.Stop();
        }

        protected abstract void UpdateValues();

        protected void PublishEvent(uint managerEvent, double value)
        {
            uint eventID = managerEvent;
            InstancedVMSDataEventArgs eventArgs = new InstancedVMSDataEventArgs(eventID, managerInstance, value);
            EventBridge.EventProcessor.PublishEvent(eventArgs);
        }
    }
}
