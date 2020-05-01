using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.UI.CustomComponents;
using VMSpc.Common;

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
            EventBridge.EventProcessor.SubscribeToEvent(this, eventID);
        }

        protected void SubscribeToEvent(uint eventID, byte publisherInstance)
        {
            EventBridge.EventProcessor.SubscribeToEvent(this, eventID, publisherInstance);
        }

        protected void UnsubscribeFromEvent(uint eventID)
        {
            EventBridge.EventProcessor.UnsubscribeFromEvent(this, eventID);
        }

        protected void UnsubscribeFromEvent(uint eventID, byte publisherInstance)
        {
            EventBridge.EventProcessor.UnsubscribeFromEvent(this, eventID, publisherInstance);
        }

        protected abstract void HandleNewData(VMSEventArgs e);
        public abstract void Draw();
        public abstract void Update();
        public abstract void Enable();
        public abstract void Disable();
    }
}
