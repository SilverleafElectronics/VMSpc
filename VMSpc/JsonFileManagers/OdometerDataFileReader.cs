using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.JsonFileManagers
{
    public class OdometerDataEntry
    {
        public DateTime EndDate { get; set; }
        public double Miles { get; set; }
        public double Fuel { get; set; }
        public double Time { get; set; }
        public double Speed { get; set; }
        public double MPG { get; set; }
    }

    public class OdometerDataContents : IJsonContents
    {
        public List<OdometerDataEntry> OdometerDataEntries;
    }
    public class OdometerDataFileReader : JsonFileReader<OdometerDataContents>
    {
        public OdometerDataFileReader(string filepath) : base(filepath)
        {

        }

        protected override OdometerDataContents GetDefaultContents()
        {
            return new OdometerDataContents()
            {
                OdometerDataEntries = new List<OdometerDataEntry>(),
            };

        }
    }
}
