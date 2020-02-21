using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Parsers;
using VMSpc.Parsers.SpecialParsers.TireParsers;

namespace VMSpc
{
    public interface IEventPublisher
    {
        event EventHandler<VMSEventArgs> RaiseVMSEvent;
    }
    public interface IEventConsumer
    {
        void ConsumeEvent(VMSEventArgs e);
    }
    public abstract class VMSEventArgs : EventArgs
    {
        public uint eventID;
        public VMSEventArgs(uint eventID)
        {
            this.eventID = eventID;
        }
    }
    public class VMSDataEventArgs : VMSEventArgs
    {
        public double value;
        public VMSDataEventArgs(uint eventID, double value)
            : base(eventID)
        {
            this.value = value;
        }
    }
    public class DiagnosticEventArgs : VMSEventArgs
    {
        public DiagnosticRecord record;
        public DiagnosticEventArgs(DiagnosticRecord record)
            : base(Constants.DIAGNOSTIC_BASE)
        {
            this.record = record;
        }
    }
    public class TireEventArgs : VMSEventArgs
    {
        public Tire tire;
        public TireEventArgs(Tire tire)
            : base(Constants.TIRE_BASE | (uint)tire.Index)
        {
            this.tire = tire;
        }
    }
    public sealed class EventBridge
    {
        private Dictionary<uint, List<IEventConsumer>> eventRegistry;
        static EventBridge() { }
        private EventBridge() 
        {
            eventRegistry = new Dictionary<uint, List<IEventConsumer>>();

        }
        ~EventBridge() { }

        public static EventBridge EventProcessor { get; } = new EventBridge();

        public void SubscribeToEvent(IEventConsumer consumer, uint eventID)
        {
            if (!eventRegistry.ContainsKey(eventID))
            {
                eventRegistry[eventID] = new List<IEventConsumer>();
            }
            eventRegistry[eventID].Add(consumer);
        }

        public void UnsubscribeFromEvent(IEventConsumer consumer, uint eventID)
        {
            //If the event isn't in the dictionary. This shouldn't ever happen
            if (!eventRegistry.ContainsKey(eventID))
            {
                return;
            }
            eventRegistry[eventID].Remove(consumer);
        }

        void HandleVMSEvent(object sender, VMSEventArgs e)
        {
            PublishEvent(e);
        }

        public void PublishEvent(VMSEventArgs e)
        {
            if (eventRegistry.ContainsKey(e.eventID))
            {
                foreach (var consumer in eventRegistry[e.eventID])
                {
                    consumer.ConsumeEvent(e);
                }
            }
        }

        public void AddEventPublisher(IEventPublisher publisher)
        {
            publisher.RaiseVMSEvent += HandleVMSEvent;
        }
    }
}
