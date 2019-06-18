using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;
using static VMSpc.Constants;
using static VMSpc.Parsers.PGNMapper;
using VMSpc.DevHelpers;

namespace VMSpc.Parsers
{
    public class J1939Parser
    {
        public readonly int spn;
        public J1939Parser()
        {

        }

        public void Parse(J1939Message canMessage)
        {
            if (PGNMap.ContainsKey(canMessage.pgn) && PGNMap[canMessage.pgn] != null)
            {
                foreach (TSPNDatum datum in PGNMap[canMessage.pgn])
                    datum.Parse(canMessage.address, canMessage.data);
            }
        }
    }
}
