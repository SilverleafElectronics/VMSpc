using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.AdvancedParsers;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.JsonFileManagers
{
    public class TankMinderContents : IJsonContents
    {
        public double
            StartGallons,
            StartMiles,
            StartLiters,
            StartKilometers;
    }
    public class TankMinderReader : JsonFileReader<TankMinderContents>
    {
        public TankMinderReader(string filename)
            : base(filename)
        { 
        }
        protected override TankMinderContents GetDefaultContents()
        {
            var miles = ChassisParameters.Instance.CurrentMiles;
            var gallons = ChassisParameters.Instance.CurrentFuelGallons;
            return new TankMinderContents()
            {
                StartMiles = miles,
                StartGallons = gallons,
                StartKilometers = (miles * 1.60934),
                StartLiters = (gallons * 3.78541)
            };
        }

        public static string GetNewFilePath()
        {
            return "\\configuration\\tankminders\\" + DateTime.Now.ToString("MMddyyyyHHmmss");
        }
    }
}
