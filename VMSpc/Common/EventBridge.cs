using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Common
{
    public sealed class EventBridge
    {
        private Dictionary<ulong, List<IEventConsumer>> eventRegistry;

        public static EventBridge Instance { get; private set; }

        static EventBridge() { }
        private EventBridge() 
        {
            eventRegistry = new Dictionary<ulong, List<IEventConsumer>>();
        }
        public static void Initialize()
        {
            Instance = new EventBridge();
        }

        /// <summary>
        /// Empties all event registries. This should only be called if the application is completely reset.
        /// </summary>
        public void Reset()
        {
            eventRegistry.Clear();
        }

        /// <summary>
        /// Empties all event registries with IGUIEventConsumers attached. This should be called when the GUI is reset
        /// to allow garbage collection of unused GUI elements. Otherwise, a reference to them will be held by the
        /// eventRegistry, allowing them to persist indefinitely.
        /// </summary>
        public void RemoveGUIRegistryItems()
        {
            foreach (var registry in eventRegistry)
            {
                registry.Value.RemoveAll(x => x is IGUIEventConsumer);
            }
        }

        ~EventBridge() { }

        public void SubscribeToEvent(IEventConsumer consumer, ulong eventID)
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
        public void SubscribeToEvent(IEventConsumer consumer, ulong eventID, byte publisherInstance)
        {
            SubscribeToEvent(consumer, GetInstancedEvent(eventID, publisherInstance));
        }

        public void UnsubscribeFromEvent(IEventConsumer consumer, ulong eventID)
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
        public void UnsubscribeFromEvent(IEventConsumer consumer, ulong eventID, byte publisherInstance)
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

        /// <summary>
        /// Publishes this event to all consumers subscribed to the exact EventID
        /// </summary>
        /// <param name="e"></param>
        public void PublishEvent(VMSEventArgs e)
        {
            if (eventRegistry.ContainsKey(e.eventID))
            {
                foreach (var consumer in eventRegistry[e.eventID])
                {
                    consumer.ConsumeEvent(e);
                }
            }
            var generic = e.GetGenericID();
            if (e.GetGenericID() != e.eventID && eventRegistry.ContainsKey(e.GetGenericID()))
            {
                foreach (var consumer in eventRegistry[e.GetGenericID()])
                {
                    consumer.ConsumeEvent(e);
                }
            }
        }

        public void AddEventPublisher(IEventPublisher publisher)
        {
            publisher.RaiseVMSEvent += HandleVMSEvent;
        }

        public void RemoveEventPublisher(IEventPublisher publisher)
        {
            if (publisher != null)
            {
                publisher.RaiseVMSEvent -= HandleVMSEvent;
            }
        }

        public static ulong GetInstancedEvent(ulong eventID, byte instance) => (eventID | ((ulong)instance << 16));
    }
}
