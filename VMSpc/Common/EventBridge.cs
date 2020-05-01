using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Common
{
    public sealed class EventBridge
    {
        private Dictionary<uint, List<IEventConsumer>> eventRegistry;
        static EventBridge() { }
        private EventBridge()
        {
            eventRegistry = new Dictionary<uint, List<IEventConsumer>>();
        }

        /// <summary>
        /// Empties all event registries. This should be called when items are removed from the GUI, so that they can be garbage collected
        /// </summary>
        public void Reset()
        {
            eventRegistry.Clear();
        }

        public void RemoveGUIRegistryItems()
        {
            foreach (var registry in eventRegistry)
            {
                registry.Value.RemoveAll(x => x is IGUIEventConsumer);
            }
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

        /// <summary>
        /// Subscribe to an instanced event
        /// </summary>
        public void SubscribeToEvent(IEventConsumer consumer, uint eventID, byte publisherInstance)
        {
            SubscribeToEvent(consumer, GetInstancedEvent(eventID, publisherInstance));
        }

        public void UnsubscribeFromEvent(IEventConsumer consumer, uint eventID)
        {
            //If the event isn't in the dictionary. This shouldn't ever happen
            if (eventRegistry.ContainsKey(eventID))
            {
                eventRegistry[eventID].Remove(consumer);
            }
        }

        /// <summary>
        /// Unsubscribes to an instanced event
        /// </summary>
        public void UnsubscribeFromEvent(IEventConsumer consumer, uint eventID, byte publisherInstance)
        {
            UnsubscribeFromEvent(consumer, GetInstancedEvent(eventID, publisherInstance));
        }

        public void UnsubscribeFromAllEvents(IEventConsumer consumer)
        {
            foreach (var registry in eventRegistry)
            {
                registry.Value.RemoveAll(x => x == consumer);
            }
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

        public static uint GetInstancedEvent(uint eventID, byte instance) => (eventID | (uint)(instance << 16));
    }
}
