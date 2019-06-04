using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Managers
{
    abstract class J1939Parser
    {
        public readonly int spn;
        public J1939Parser()
        {

        }

        public abstract void Parse();

        private void ConvertAndStore()
        {

        }

        private void ConvertAndStore(float scale, float offset)
        {

        }
    }
}
