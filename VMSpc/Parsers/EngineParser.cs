using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace VMSpc
{
    public class EngineParser
    {
        int cid;
        Func<double, double> parse;
        public EngineParser(int id)
        {
            cid = id;
            //default to J1939. This must be called at construction to avoid
            //a null reference exception at runtime
            changeEngineType(Constants.J1939);
        }

        public void changeEngineType(int engine)
        {
            if (engine == Constants.J1939)
                parse = J1939Parser;
        }

        public virtual double J1939Parser(double value) { return 0; }

        public virtual double J1708Parser(double value) { return 0; }

    }
}
