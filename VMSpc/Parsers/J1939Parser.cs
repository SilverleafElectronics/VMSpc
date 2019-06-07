using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;
using static VMSpc.PIDs;

namespace VMSpc.Parsers
{
    public class J1939Parser
    {
        public readonly int spn;
        public J1939Parser()
        {

        }

        public void Parse(CanMessage canMessage)
        {
            
        }

        private void ConvertAndStore()
        {

        }

        private void ConvertAndStore(float scale, float offset)
        {

        }

        public void SetValueSPN(uint pid, uint raw, float v_metric, float v_standard, byte src)
        {
            PIDStruct temp = PIDList[(byte)pid];
            if (src == Constants.J1708)
                temp.Prioritize1708 = true;
            if (temp.Prioritize1708 && (src != Constants.J1708))
                return;
        }
    }


    class TSPNDatum
    {
        public float value;
        public float value_metric;
        public float recipNum;
        public uint pgn;
        public virtual void Parse() { }
    }

    class TSPNPresenter
    {
        public TSPNDatum datum;
        string title;
        bool seen;
        bool seenOnJ1708;
        
        public TSPNPresenter(TSPNDatum spn, string title, int index)
        {
            seenOnJ1708 = false;
        }


    }
}
