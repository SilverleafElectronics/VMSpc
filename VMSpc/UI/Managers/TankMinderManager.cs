using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Common;
using VMSpc.Parsers;
using System.Timers;
using VMSpc.AdvancedParsers;

namespace VMSpc.UI.Managers
{
    class TankMinderManager : ComponentManager
    {
        public TankMinderReader 
            tankMinderReader;
        private TankMinderSettings 
            tankSettings;
        private double
            startFuel,
            startDistance,
            currentFuelmeter,
            currentOdometer;
        private double
            currentDistanceByFuel,
            currentFuel,
            currentDistanceToEmpty,
            lastDistanceByFuel,
            lastFuel,
            lastDistanceToEmpty;
        public TankMinderManager(TankMinderSettings tankSettings)
            :base()
        {
            this.tankSettings = tankSettings;
            tankMinderReader = new TankMinderReader(tankSettings.fileName);
            startFuel = (!tankSettings.showInMetric) ? tankMinderReader.Contents.StartGallons : tankMinderReader.Contents.StartLiters;
            startDistance = (!tankSettings.showInMetric) ? tankMinderReader.Contents.StartMiles : tankMinderReader.Contents.StartKilometers;
            currentFuelmeter = startFuel;
            currentOdometer = startDistance;
            UpdateValues();
        }

        protected override void UpdateValues()
        {
            currentFuelmeter = (!tankSettings.showInMetric) ? ChassisParameters.Instance.CurrentFuelGallons : ChassisParameters.Instance.CurrentFuelLiters;
            currentOdometer = (!tankSettings.showInMetric) ? ChassisParameters.Instance.CurrentMiles : ChassisParameters.Instance.CurrentKilometers;
            UpdateFuel();
            UpdateRecentMPG();
            UpdateMilesToEmpty();
        }

        private void UpdateFuel()
        {
            currentFuel = tankSettings.tankSize - currentFuelmeter + startFuel;
            PublishEvent(EventIDs.FUEL_READING_EVENT, currentFuel);
            lastFuel = currentFuel;
        }

        private void UpdateRecentMPG()
        {
            if (tankSettings.useRollingMPG || (currentFuelmeter < 1.0))
            {
                currentDistanceByFuel = PIDValueStager.Instance.GetParameter((ushort)((tankSettings.showInMetric) ? 602 : 502)).LastValue;
            }
            else
            {
                if ((currentFuelmeter - startFuel) == 0)
                    currentDistanceByFuel = 0;
                else
                    currentDistanceByFuel = (currentOdometer - startDistance) / (currentFuelmeter - startFuel);
            }
            PublishEvent(EventIDs.CURRENT_MPG_EVENT, currentDistanceByFuel);
            lastDistanceByFuel = currentDistanceByFuel;
        }

        private void UpdateMilesToEmpty()
        {
            currentDistanceToEmpty = currentFuel * currentDistanceByFuel;
            PublishEvent(EventIDs.DISTANCE_REMAINING_EVENT, currentDistanceToEmpty);
            lastDistanceToEmpty = currentDistanceToEmpty;
        }
    }
}
