using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Enums.Parsing;
using VMSpc.Common;

namespace VMSpc.Parsers.SpecialParsers.TireParsers
{
    public abstract class Tire : IEventPublisher
    {
        public Tire(int Index)
        {
            this.Index = Index;
            EventBridge.EventProcessor.AddEventPublisher(this);
        }
        public int Index { get; set; }
        public ulong Id { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool Changed { get; set; }
        public TireStatus Status 
        { 
            get { return Status; } 
            set 
            { 
                if (value != Status)
                {
                    Status = value; Changed = true;
                }
            } 
        }

        public double DisplayPressure 
        {
            get { return DisplayPressure; } 
            set
            {
                if (value != DisplayPressure)
                {
                    DisplayPressure = value; Changed = true;
                }
            }
        }
        public double DisplayTemperature
        {
            get { return DisplayTemperature; }
            set
            {
                if (value != DisplayTemperature)
                {
                    DisplayTemperature = value; Changed = true;
                }
            }
        }

        public void ProcessChange()
        {
            if (Changed)
            {
                OnRaiseCustomEvent(new TireEventArgs(this));
                Changed = false;
            }
        }

        public abstract ulong Pressure { set; }
        public abstract ulong Temperature { set; }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;
        protected virtual void OnRaiseCustomEvent(TireEventArgs e)
        {
            RaiseVMSEvent?.Invoke(this, e);
        }
    }
}
