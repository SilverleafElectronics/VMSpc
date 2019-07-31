using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.XmlFileManagers.SettingsManager;
using static VMSpc.Parsers.PresenterWrapper;
using VMSpc.Helpers;

namespace VMSpc.XmlFileManagers
{
    /// <summary>
    /// Tracks the state of the odometer in Odometer.xml. For specific Odo##.xml files (for use with odometer panels), see OdometerManager.cs
    /// </summary>
    public sealed class OdometerTracker : XmlFileManager
    {
        private Timer timer;
        static OdometerTracker() { }
        public static OdometerTracker Odometer { get; set; } = new OdometerTracker();

        public double Hours
        {
            get => Double.Parse(getNodeValueByTagName("Hourmeter"));
            set => SetNodeValueByTagName("Hourmeter", value.ToString());
        }

        public double Miles
        {
            get => Double.Parse(getNodeValueByTagName("Odometer"));
            set => SetNodeValueByTagName("Odometer", value.ToString());
        }

        public double Fuel
        {
            get => Double.Parse(getNodeValueByTagName("Fuelmeter"));
            set => SetNodeValueByTagName("Fuelmeter", value.ToString());
        }

        public OdometerTracker() : base("Odometer.xml")
        {
            timer = CREATE_TIMER(UpdateOdometer, 10000);
        }

        public double Liters => Fuel * 3.78541;
        public double Kilometers => Miles * 1.60934;

        /// <summary> Updates the values in Odometer.xml with the latest values. Only writes to the file if an update has been made </summary>
        private void UpdateOdometer(object source, ElapsedEventArgs e)
        {
            bool updated = false;
            if (SEEN(247) && Hours != ParamData.parameters[247].LastValue)
            {
                Hours = ParamData.parameters[247].LastValue;
                updated = true;
            }
            if (SEEN(Settings.odometerPID) && Miles != ParamData.parameters[Settings.odometerPID].LastValue)
            {
                Miles = ParamData.parameters[Settings.odometerPID].LastValue;
                updated = true;
            }
            if (SEEN(250) && Fuel != ParamData.parameters[250].LastValue)
            {
                Fuel = ParamData.parameters[250].LastValue;
                updated = true;
            }
            if (updated)
                SaveConfiguration();
        }

        public override void Activate()
        {
            PresenterList[247].datum.SetValue((uint)Hours, Hours, Hours);
            PresenterList[250].datum.SetValue((uint)Fuel, Fuel, Liters);
            PresenterList[Settings.odometerPID].datum.SetValue((uint)Miles, Miles, Kilometers);
        }
    }
}
