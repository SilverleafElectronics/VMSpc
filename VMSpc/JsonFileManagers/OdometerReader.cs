using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Parsers.PresenterWrapper;

namespace VMSpc.JsonFileManagers
{
    public class OdometerContents : IJsonContents
    {
        public double
            StartGallons,
            StartHours,
            StartMiles,
            StartLiters,
            StartKilometers;
    }
    public class OdometerReader : JsonFileReader<OdometerContents>
    {
        public OdometerReader(string filename)
            :base(filename)
        {
        }

        protected override OdometerContents GetDefaultContents()
        {
            var miles = GetStandardValueSPN(ConfigManager.Settings.Contents.odometerPid);
            var gallons = GetStandardValueSPN(250);
            var hours = GetStandardValueSPN(247);
            return new OdometerContents()
            {
                StartMiles = miles,
                StartGallons = gallons,
                StartHours = hours,
                StartKilometers = (miles * 1.60934),
                StartLiters = (gallons * 3.78541)
            };
        }

        public static string GetNewFilePath()
        {
            return "\\configuration\\odometers\\" + DateTime.Now.ToString("MMddyyyyHHmmss");
        }
    }
}
