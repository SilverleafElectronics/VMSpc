using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.JsonFileManagers;
using VMSpc.Parsers;
using VMSpc.Common;

namespace VMSpc.UI.Managers
{
    public class OdometerManager : ComponentManager
    {
        public OdometerReader odometerReader;
        private OdometerSettings odometerSettings;
        private double
            startDistance,
            startFuel,
            startHours,
            currentDistance,
            currentFuel,
            currentHours,
            currentDistanceByFuel,
            currentSpeed,
            lastDistance,
            lastFuel,
            lastHours,
            lastDistanceByFuel,
            lastSpeed;

        public OdometerManager(OdometerSettings odometerSettings)
            :base()
        {
            this.odometerSettings = odometerSettings;
            odometerReader = new OdometerReader(odometerSettings.fileName);

        }

        protected override void UpdateValues()
        {
            UpdateFuel();
            UpdateHours();
            UpdateDistance();
            UpdateEconomy();
            UpdateSpeed();
        }

        private void UpdateEconomy()
        {
            currentDistanceByFuel = (currentFuel < 0.1) ? 0.0 : currentDistance / currentFuel;
            if (odometerSettings.showInMetric && currentDistanceByFuel != 0)
            {
                currentDistanceByFuel = 100 / currentDistanceByFuel;   //converts km/liter to liter/1000km
            }
            if (lastDistanceByFuel != currentDistanceByFuel)
            {
                PublishEvent(EventIDs.CURRENT_MPG_EVENT, currentDistanceByFuel);
            }
            lastDistanceByFuel = currentDistanceByFuel;
        }

        private void UpdateDistance()
        {
            currentDistance = ChassisParameter.ChassisParam.odometer - startDistance;
            if (odometerSettings.showInMetric)
            {
                currentDistance *= 1.609344;
            }
            if (lastDistance != startDistance)
            {
                PublishEvent(EventIDs.DISTANCE_TRAVELLED_EVENT, currentDistance);
            }
            lastDistance = startDistance;
        }

        private void UpdateHours()
        {
            currentHours = ChassisParameter.ChassisParam.hourmeter - startHours;
            if (currentHours != lastHours)
            {
                PublishEvent(EventIDs.HOURS_EVENT, currentHours);
            }
            lastHours = currentHours;
        }

        private void UpdateFuel()
        {
            currentFuel = ChassisParameter.ChassisParam.fuelmeter - startFuel;
            if (odometerSettings.showInMetric)
            {
                currentFuel = (currentFuel * 3.78541);
            }
            if (currentFuel != lastFuel)
            {
                PublishEvent(EventIDs.FUEL_READING_EVENT, currentFuel);
            }
            lastFuel = currentFuel;
        }

        private void UpdateSpeed()
        {
            currentSpeed = (currentHours > 0) ? (currentDistance / currentHours) : 0;
            if (lastSpeed != currentSpeed)
            {
                PublishEvent(EventIDs.AVERAGE_SPEED_EVENT, currentSpeed);
            }
            lastSpeed = currentSpeed;
        }
    }
}
