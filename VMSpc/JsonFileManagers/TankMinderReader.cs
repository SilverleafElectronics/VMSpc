using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Parsers.PresenterWrapper;

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
            var miles = GetStandardValueSPN(ConfigManager.Settings.Contents.odometerPid);
            var gallons = GetStandardValueSPN(250);
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
