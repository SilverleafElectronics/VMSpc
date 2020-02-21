using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Parsers.SpecialParsers.TireParsers;

namespace VMSpc.Parsers.TireParsers
{
    public abstract class TireParser : ITireParser
    {
        public static byte MAX_NUM_TIRES = 16;
        protected Tire[] Tires = new Tire[MAX_NUM_TIRES];
        public abstract void LearnTire(byte position);
        public abstract void ClearTire(byte position);
    }
}
