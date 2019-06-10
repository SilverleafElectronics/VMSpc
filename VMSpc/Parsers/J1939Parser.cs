using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.Constants;
using static VMSpc.Parsers.PGNMapper;

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
            foreach (TSPNDatum datum in PGNMap[canMessage.pgn])
                datum.Parse(canMessage.address, canMessage.rawData);
        }

        public void SetValueSPN(uint pid, uint raw, float v_metric, float v_standard, byte src)
        {
            PID temp = PIDManager.PIDList[(byte)pid];
            if (src == J1708)
                temp.Prioritize1708 = true;
            if (temp.Prioritize1708 && (src != J1708))
                return;
        }
    }
}
