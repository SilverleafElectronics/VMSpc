using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.UI.CustomComponents;
using VMSpc.Common;
using VMSpc.DevHelpers;
using VMSpc.Exceptions;
using VMSpc.Loggers;

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
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }

        protected void SubscribeToEvent(ulong eventID)
        {
            EventBridge.Instance.SubscribeToEvent(this, eventID);
        }

        protected void SubscribeToEvent(ulong eventID, byte publisherInstance)
        {
            EventBridge.Instance.SubscribeToEvent(this, eventID, publisherInstance);
        }

        protected void UnsubscribeFromEvent(ulong eventID)
        {
            EventBridge.Instance.UnsubscribeFromEvent(this, eventID);
        }

        protected void UnsubscribeFromEvent(ulong eventID, byte publisherInstance)
        {
            EventBridge.Instance.UnsubscribeFromEvent(this, eventID, publisherInstance);
        }

        protected abstract void HandleNewData(VMSEventArgs e);
        public abstract void Draw();
        public abstract void Update();
        public abstract void Enable();
        public abstract void Disable();
    }
}
