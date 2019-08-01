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
    public sealed class PIDWrapper
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

        public Dictionary<ushort, PID> PIDList = new Dictionary<ushort, PID>();

        static PIDWrapper() { }
        public static PIDWrapper PIDManager { get; set; } = new PIDWrapper();
        public PIDWrapper()
        {
        }

        public void Activate()
        {
            //                             PID  NumDataBytes       Title   
            PIDList.Add(47, new PID(47, 2, "Retarder Switch"));
            PIDList.Add(119, new PID(119, 2, "Retarder Oil Pressure"));
            PIDList.Add(120, new PID(120, 2, "Retarder Oil Temperature"));
            PIDList.Add(121, new PID(121, 2, "Retarder Status"));
            PIDList.Add(91, new PID(91, 2, "Accelerator Position"));
            PIDList.Add(168, new PID(168, 3, "Voltage"));
            PIDList.Add(86, new PID(86, 2, "Cruise Speed"));
            PIDList.Add(110, new PID(110, 2, "Coolant Temperature"));
            PIDList.Add(92, new PID(92, 2, "Engine Load"));
            PIDList.Add(183, new PID(183, 3, "Fuel Rate"));
            PIDList.Add(174, new PID(174, 3, "Fuel Temperature"));
            PIDList.Add(184, new PID(184, 3, "Instantaneous MPG"));
            PIDList.Add(172, new PID(172, 3, "Air Inlet Temperature"));
            PIDList.Add(105, new PID(105, 2, "Intake Temperature"));
            PIDList.Add(100, new PID(100, 2, "Oil PSI"));
            PIDList.Add(122, new PID(122, 2, "Retarder Percent"));
            PIDList.Add(175, new PID(175, 3, "Oil Temperature"));
            PIDList.Add(84, new PID(84, 2, "Road Speed"));
            PIDList.Add(85, new PID(85, 2, "Cruise Set Status"));
            PIDList.Add(190, new PID(190, 3, "Engine Speed"));
            PIDList.Add(177, new PID(177, 3, "Transmission Temperature"));
            PIDList.Add(191, new PID(191, 3, "Transmission Speed"));
            PIDList.Add(102, new PID(102, 2, "Turbo Boost"));
            PIDList.Add(162, new PID(162, 3, "Range Selected"));
            PIDList.Add(163, new PID(163, 3, "Range Attained"));
            PIDList.Add(244, new PID(244, 6, "Total Miles"));
            PIDList.Add(245, new PID(245, 6, "Total Miles"));
            PIDList.Add(247, new PID(247, 6, "Engine Hours"));
            PIDList.Add(250, new PID(250, 6, "Total Fuel"));
            PIDList.Add(0xC2, new PID(0xC2, 1000, "Diagnostic"));
            PIDList.Add(0xC0, new PID(0xC0, 1000, "Multipart"));
        }
    }

    public class PID
    {
        public byte pidNumber;
        public string Title;
        public int NumDataBytes;
        public double StandardOffset;
        public double StandardMultiplier;
        public double MetricOffset;
        public double MetricMultiplier;
        public byte ParserType;
        public bool Prioritize1708;

        //public double standardValue;
       // public double rawValue;
        //public double metricValue;
        public PID(byte pid, int numDataBytes, string title)
        {
            pidNumber = pid;
            NumDataBytes = numDataBytes;
            Title = title;
            StandardOffset = 0;
            StandardMultiplier = 1;
            MetricOffset = 0;
            MetricMultiplier = 1;
            ParserType = NON_STANDARD;
            Prioritize1708 = false;
            //standardValue = rawValue = metricValue = 0.0;
        }
    }
}
