using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers.SpecialParsers.TireParsers
{
    public class PProTire : Tire
    {
        public PProTire(int index) : base(index)
        {
            //TODO - initialize these properties. Keep in mind inferred properties
        }
        public override ulong Pressure 
        { 
            set 
            { 
                DisplayPressure = (0.6d * (double)value); 
            } 
        }
        public override ulong Temperature 
        { 
            set 
            { 
                DisplayTemperature = (2.5d * (double)value); 
            } 
        }
        public byte Strength { get; set; }
        public byte TransmissionCount { get; set; }
        public double DisplayTargetPressure { get; set; }
        public byte TargetPressure 
        { 
            set 
            {
                if (value != 0xFF)
                {
                    DisplayTargetPressure = (0.6d * (double)value);
                }
            } 
        }
        public byte AlarmStatus { get; set; }
        public byte Position { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
