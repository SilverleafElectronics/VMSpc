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
    public class J1939Parser : IDataBus
    {
        public readonly int spn;
        public J1939Parser()
        {

        }

        public void Parse(CanMessage canMessage)
        {
            J1939Message j1939Message = (canMessage as J1939Message);
            if (PGNMap.ContainsKey(j1939Message.pgn) && PGNMap[j1939Message.pgn] != null)
            {
                foreach (TSPNDatum datum in PGNMap[j1939Message.pgn])
                {
                    //if (Settings.)
                    datum.Parse(j1939Message.address, j1939Message.data);
                }
            }
        }

        public void SendMessage(byte[] message)
        {

        }
    }
}
