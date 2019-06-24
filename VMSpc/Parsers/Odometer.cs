using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using static VMSpc.XmlFileManagers.SettingsManager;
using static VMSpc.Parsers.PresenterWrapper;

namespace VMSpc.Parsers
{
    class Odometer
    {
        private double startFuel,
                       startHours,
                       startMiles,
                       startLiters,
                       startKilometers;

        private double currentFuel,
                       currentHours,
                       currentMiles,
                       currentLiters,
                       currentKilometers;

        public double currentTripFuel,
                      currentTripHours,
                      currentTripMiles,
                      currentTripLiters,
                      currentTripKilometers,
                      currentTripMPG,
                      currentTripLPK,
                      currentTripMPH,
                      currentTripKPH;
        
        private ushort odometerPID;
        private string fileName;

        public Odometer(string filename)
        {
            fileName = filename;
            OdometerManager manager = new OdometerManager(fileName);
            odometerPID = Settings.get_odometerPID();

            startFuel = manager.startFuel;
            startHours = manager.startHours;
            startMiles = manager.startMiles;
            startLiters = manager.startLiters;
            startKilometers = manager.startKilometers;

            currentFuel =
            currentHours =
            currentMiles =
            currentLiters =
            currentKilometers = 0;

            currentTripFuel =
            currentTripHours =
            currentTripMiles =
            currentTripLiters =
            currentTripKilometers = 
            currentTripMPG =
            currentTripLPK =
            currentTripMPH =
            currentTripKPH = 0;

        }

        //move back to OdometerDlg
        private string GenerateNewOdometer()
        {
            DateTime curDate = DateTime.Now;
            long i = (curDate.Year * 365) + (curDate.Month * 30) + curDate.Day;
            long j = (curDate.Hour * 3600) + (curDate.Minute * 60) +curDate.Second;
            return String.Format("Odo%d%d.trp.xml", i, j);
        }

        public void CalculateValues()
        {
            currentFuel = PresenterList[250].datum.value;
            currentHours = PresenterList[247].datum.value;
            currentMiles = PresenterList[odometerPID].datum.value;
            currentLiters = PresenterList[250].datum.valueMetric;
            currentKilometers = PresenterList[odometerPID].datum.valueMetric;

            currentTripFuel = currentFuel - startFuel;
            currentTripHours = currentHours - startHours;
            currentTripMiles = currentMiles - startMiles;
            currentTripLiters = currentLiters - startLiters;
            currentTripKilometers = currentKilometers - startKilometers;
        }


    }
}
