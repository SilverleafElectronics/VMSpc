using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.JsonFileManagers;
using VMSpc.Parsers;
using VMSpc.Common;
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

    public interface IGUIEventConsumer : IEventConsumer { }

    public abstract class VMSEventArgs : EventArgs
    {
        public uint eventID;
        public VMSEventArgs(uint eventID)
        {
            this.eventID = eventID;
        }
    }
    public abstract class InstancedVMSEventArgs : VMSEventArgs
    {
        public uint instance;
        public InstancedVMSEventArgs(uint eventID, byte instance)
            : base(EventBridge.GetInstancedEvent(eventID, instance))
        {
            this.instance = instance;
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

    public class AlarmEventArgs : VMSEventArgs
    {
        public AlarmSettings alarmSettings;
        public AlarmEventArgs(uint eventID, AlarmSettings alarmSettings)
            :base(eventID)
        {

        }
    }

    public class InstancedVMSDataEventArgs : InstancedVMSEventArgs
    {
        public double value;
        public InstancedVMSDataEventArgs(uint eventID, byte instance, double value)
            : base(eventID, instance)
        {
            this.value = value;
        }
    }

    public class DiagnosticEventArgs : VMSEventArgs
    {
        public DiagnosticRecord record;
        public DiagnosticEventArgs(DiagnosticRecord record)
            : base(EventIDs.DIAGNOSTIC_BASE)
        {
            this.record = record;
        }
    }

    public class TireEventArgs : VMSEventArgs
    {
        public Tire tire;
        public TireEventArgs(Tire tire)
            : base(EventIDs.TIRE_BASE | (uint)tire.Index)
        {
            this.tire = tire;
        }
    }
}
