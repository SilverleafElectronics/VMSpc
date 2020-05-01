using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.JsonFileManagers
{
    public class MeterContents : IJsonContents
    {
        public double
            Fuelmeter,
            Hourmeter,
            Odometer;
    }
    public class MeterReader : JsonFileReader<MeterContents>
    {
        public MeterReader(string filename) : base(filename)
        {

        }
        protected override MeterContents GetDefaultContents()
        {
            return new MeterContents()
            {
                Fuelmeter = 0.0,
                Hourmeter = 0.0,
                Odometer = 0.0
            };
        }
    }
}
