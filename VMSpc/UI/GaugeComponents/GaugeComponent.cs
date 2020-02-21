using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.CustomComponents;
using static VMSpc.EventBridge;

namespace VMSpc.UI.GaugeComponents
{
    public abstract class GaugeComponent : VMSCanvas, IGaugeComponent, IEventConsumer
    {
        public virtual void ConsumeEvent(VMSEventArgs e)
        {
            try
            {
                HandleNewData(e);
            }
            catch (Exception ex)
            {

            }
        }

        protected void SubscribeToEvent(uint eventID)
        {
            EventProcessor.SubscribeToEvent(this, eventID);
        }

        protected void UnsubscribeFromEvent(uint eventID)
        {
            EventProcessor.UnsubscribeFromEvent(this, eventID);
        }

        protected abstract void HandleNewData(VMSEventArgs e);
        public abstract void Draw();
        public abstract void Update();
        public abstract void Enable();
        public abstract void Disable();
    }
}
