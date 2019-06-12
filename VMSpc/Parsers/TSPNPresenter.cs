using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers
{
    class TSPNPresenter
    {
        public TSPNDatum datum;
        public string title;



        public TSPNPresenter(TSPNDatum spn, string title, int index)
        {
            
        }

        public bool Seen()
        {
            return false;
        }
        public bool SeenOnJ1708()
        {
            return false;
        }
    }

    
}
