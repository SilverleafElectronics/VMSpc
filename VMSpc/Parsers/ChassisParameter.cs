using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.Parsers
{
    public sealed class ChassisParameter : IEventConsumer
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
        public ChassisParameter() 
        {
            EventBridge.EventProcessor.SubscribeToEvent(this, EventIDs.PID_BASE | 247); //hourmeter
            EventBridge.EventProcessor.SubscribeToEvent(this, EventIDs.PID_BASE | 250); //fuelmeter
            EventBridge.EventProcessor.SubscribeToEvent(this, EventIDs.PID_BASE | ConfigManager.Settings.Contents.odometerPid);
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            var eventID = e.eventID & 0xFF;
            var ev = (e as VMSDataEventArgs);
            if (eventID == 247)
                hourmeter = ev.value;
            else if (eventID == 250)
                fuelmeter = ev.value;
            else if (eventID == ConfigManager.Settings.Contents.odometerPid)
                odometer = ev.value;
        }
    }
}
