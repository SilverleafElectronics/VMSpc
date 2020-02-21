using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers.TireParsers
{
    interface ITireParser
    {
        void LearnTire(byte position);
        void ClearTire(byte position);
    }
}
