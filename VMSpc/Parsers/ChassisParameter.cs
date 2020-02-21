using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers
{
    public sealed class ChassisParameter
    {
        public byte cruiseStatus;
        public byte cruiseAdjust;
        public uint rawOdometer, rawFuelmeter, rawHourmeter;
        public double odometer,
                      fuelmeter,
                      hourmeter,
                      idleFuel,
                      idleHours,
                      recentSpeed,
                      recentFuelRate,
                      rollingSpeed,
                      rollingFuelRate,
                      milesToEmpty,
                      kmsToEmpty;
        public string vehicleID,
                      softwareID,
                      componentID,
                      rangeSelected,
                      rangeAttained,
                      cruiseStat;
        public byte mode;
        public byte error;
        public bool diagActiveFlag;

        static ChassisParameter() { }

        /// <summary>
        /// A singleton ChassisParameter object. Used for tracking chassis components
        /// </summary>
        public static ChassisParameter ChassisParam { get; set; } = new ChassisParameter();
        public ChassisParameter() { }

    }
}
