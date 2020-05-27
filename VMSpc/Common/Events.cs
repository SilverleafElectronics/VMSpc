using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Enums.Parsing;
using VMSpc.JsonFileManagers;
using VMSpc.Parsers;
using VMSpc.Parsers.SpecialParsers.TireParsers;
using VMSpc.AdvancedParsers;

namespace VMSpc.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventPublisher
    {
        event EventHandler<VMSEventArgs> RaiseVMSEvent;
    }

    public static class EventPublisherExtensions
    {
        //public static void PublishEvent(this IEventPublisher publisher, VMSEventArgs e)
        //{
        //    //publisher.RaiseVMSEvent?.Invoke();
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IEventConsumer
    {
        void ConsumeEvent(VMSEventArgs e);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IGUIEventConsumer : IEventConsumer { }

    /// <summary>
    /// 
    /// </summary>
    public abstract class VMSEventArgs : EventArgs
    {
        public ulong eventID;
        public VMSEventArgs(ulong eventID)
        {
            this.eventID = eventID;
        }
        /// <summary>
        /// Returns a generic verison of the event. Override this when an event's eventID has optional multiplexing elements (such
        /// as instancing, Source Address, etc).
        /// </summary>
        /// <returns></returns>
        public virtual ulong GetGenericID()
        {
            return eventID;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VMSCommDataEventArgs : VMSEventArgs
    {
        public string message;
        public DateTime timeStamp;
        public VMSCommDataEventArgs(string message, DateTime timeStamp)
            : base(EventIDs.NEW_COMM_DATA_EVENT)
        {
            this.message = message;
            this.timeStamp = timeStamp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class VMSResponseDataEventArgs : VMSEventArgs
    {
        public string rawMessage;
        public DateTime receivedTime;
        public VMSResponseDataEventArgs(ulong eventID, string rawMessage, DateTime receivedTime)
            :base(eventID)
        {
            this.rawMessage = rawMessage;
            this.receivedTime = receivedTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VMSCommDataErrorEventArgs : VMSResponseDataEventArgs
    {
        public MessageError messageError;
        public Exception exception;
        public VMSCommDataErrorEventArgs(VMSCommDataEventArgs commEvent, MessageError messageError, Exception exception = null)
            : base(EventIDs.COMM_DATA_ERROR_EVENT, commEvent.message, commEvent.timeStamp)
        {
            this.messageError = messageError;
            this.exception = exception;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DiagnosticEventArgs : VMSEventArgs
    {
        public DiagnosticMessage message;
        public DiagnosticEventArgs()
            : base(EventIDs.DIAGNOSTIC_BASE)
        {
        }
        public DiagnosticEventArgs(DiagnosticMessage message)
            :base(EventIDs.DIAGNOSTIC_BASE)
        {
            this.message = message;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VMSParsedDataEventArgs : VMSEventArgs
    {
        public CanMessageSegment messageSegment;
        public VMSParsedDataEventArgs(ushort pid, CanMessageSegment messageSegment)
            :base(EventIDs.PARSED_DATA_EVENT | pid)
        {
            this.messageSegment = messageSegment;
        }
    }

    public abstract class VMSRawDataEventArgs : VMSEventArgs
    {
        public CanMessageSegment messageSegment;
        public VMSRawDataEventArgs(ulong eventID)
            : base(eventID)
        {

        }
    }

    public class VMSJ1708RawDataEventArgs : VMSRawDataEventArgs
    {
        new public J1708MessageSegment messageSegment
        {
            get => (base.messageSegment as J1708MessageSegment);
            set => base.messageSegment = value;
        }
        public VMSJ1708RawDataEventArgs(J1708MessageSegment messageSegment)
            : base(EventIDs.Get_J1708RawDataEvent(messageSegment.Pid, messageSegment.Mid))
        {
            this.messageSegment = messageSegment;
        }
        public override ulong GetGenericID()
        {
            return EventIDs.Get_J1708RawDataEvent(messageSegment.Pid);
        }
    }

    public class VMSJ1939RawDataEventArgs : VMSRawDataEventArgs
    {
        new public J1939MessageSegment messageSegment
        {
            get => (base.messageSegment as J1939MessageSegment);
            set => base.messageSegment = value;
        }
        public VMSJ1939RawDataEventArgs(J1939MessageSegment messageSegment)
            : base(EventIDs.Get_J1939RawDataEvent(messageSegment.PGN, messageSegment.SourceAddress))
        {
            this.messageSegment = messageSegment;
        }
        public override ulong GetGenericID()
        {
            return EventIDs.Get_J1939RawDataEvent(messageSegment.PGN);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class VMSPidValueEventArgs : VMSEventArgs
    {
        public double value;
        public CanMessageSegment segment;
        public DateTime timeReceived;
        public VMSPidValueEventArgs(ulong eventID, double value)
            : base(eventID)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AlarmEventArgs : VMSEventArgs
    {
        public AlarmSettings alarmSettings;
        public AlarmEventArgs(ulong eventID, AlarmSettings alarmSettings)
            : base(eventID)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TireEventArgs : VMSEventArgs
    {
        public Tire tire;
        public TireEventArgs(Tire tire)
            : base(EventIDs.TIRE_BASE | (ushort)tire.Index)
        {
            this.tire = tire;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class InstancedVMSEventArgs : VMSEventArgs
    {
        public InstancedVMSEventArgs(ulong eventID, byte instance)
            : base(EventBridge.GetInstancedEvent(eventID, instance))
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InstancedVMSDataEventArgs : InstancedVMSEventArgs
    {
        public double value;
        public InstancedVMSDataEventArgs(ulong eventID, byte instance, double value)
            : base(EventBridge.GetInstancedEvent(eventID, instance), instance)
        {
            this.value = value;
        }
    }
}
