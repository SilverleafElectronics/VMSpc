using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Constants;

namespace VMSpc.Parsers
{
    //*************************************************************************************
    //PID definitions
    //*************************************************************************************
    public static class PIDWrapper
    {

        public const byte retarderSwitch = 47;
        public const byte retarderOilPressure = 119;
        public const byte retarderOilTemp = 120;
        public const byte retarderStatus = 121;
        public const byte percentAcceleratorPosition = 91;
        public const byte voltage = 168;
        public const byte cruiseSpeed = 86;
        public const byte coolantTemp = 110;
        public const byte engineLoad = 92;
        public const byte fuelRate = 183;
        public const byte ambientTemp = 171;
        public const byte fuelTemp = 174;
        public const byte instantMPG = 184;
        public const byte airInletTemp = 172;
        public const byte intakeTemp = 105;
        public const byte oilPSI = 100;
        public const byte retarderPercent = 122;
        public const byte oilTemp = 175;
        public const byte roadSpeed = 84;
        public const byte cruiseSetStatus = 85;
        public const byte engineSpeed = 190;
        public const byte transmissionTemp = 177;
        public const byte transmissionSpeed = 191;
        public const byte turboBoost = 102;
        public const byte rangeSelected = 162;
        public const byte rangeAttained = 163;
        public const byte totalMilesCummins = 244;
        public const byte totalMiles = 245;
        public const byte engineHours = 247;
        public const byte totalFuel = 250;
        public const byte diagnosticsPID = 0xC2;
        public const byte multipartMessage = 0xC0;
        public const byte torquePercent = 92;
        public const byte rpms = 190;
    }
}
