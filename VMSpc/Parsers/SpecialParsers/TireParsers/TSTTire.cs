using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers.SpecialParsers.TireParsers
{
    public class TSTTire : Tire
    {
        public TSTTire(int index) : base(index) { }
        public override ulong Pressure { set { DisplayPressure = (double)value; } }
        public override ulong Temperature { set { DisplayTemperature = (double)value; } }
        private byte EnableStatus { get; set; }
        private byte LeakStatus { get; set; }
        private byte ElectricalStatus { get; set; }
        private byte PressureStatus { get; set; }
        private byte ExtendedPressure { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
