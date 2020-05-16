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
            startGallons,
            startMiles,
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
            startGallons = tankMinderReader.Contents.StartGallons;
            startMiles = tankMinderReader.Contents.StartMiles;
            currentFuelmeter = startGallons;
            currentOdometer = startMiles;
        }

        protected override void UpdateValues()
        {
            currentFuelmeter = ChassisParameters.Instance.CurrentFuel;
            currentOdometer = ChassisParameters.Instance.CurrentMiles;
            UpdateFuel();
            UpdateRecentMPG();
            UpdateMilesToEmpty();
        }

        private void UpdateFuel()
        {
            currentFuel = tankSettings.tankSize - currentFuelmeter + startGallons;
            if (tankSettings.showInMetric)
            {
                currentFuel = (currentFuel * 3.78541);
            }
            PublishEvent(EventIDs.FUEL_READING_EVENT, currentFuel);
            lastFuel = currentFuel;
        }

        private void UpdateRecentMPG()
        {
            if (tankSettings.useRollingMPG || (currentFuelmeter < 1.0))
            {
                currentDistanceByFuel = ConfigManager.ParamData.GetParam((ushort)((tankSettings.showInMetric) ? 602 : 502)).LastValue;
            }
            else
            {
                currentDistanceByFuel = (currentOdometer - startMiles) / (currentFuelmeter - startGallons);
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
