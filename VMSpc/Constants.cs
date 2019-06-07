﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMSpc.DevHelpers;

//These are global constants. For global variables, see Globals.cs

namespace VMSpc
{
    public static class Constants
    {
        //-----------------------------------------------------------------------------------------
        //ERRORS
        //-----------------------------------------------------------------------------------------
        //string errors
        public const string STR_NODATA = "NODATA";
        public const string STR_USAGEERR = "USAGEERR";
        public const string STR_ERR = "ERR";

        //integer errors
        public const int INT_NODATA = int.MaxValue;
        public const int INT_USAGEERR = int.MaxValue - 1;
        public const int INT_ERR = -int.MaxValue;

        //double errors
        public const double DUB_NODATA = double.MaxValue;
        public const double DUB_USAGEERR = double.MaxValue - 1;
        public const double DUB_ERR = -double.MaxValue;

        //evaluators for errors. all return true if the value passed
        //does not equal [TYPE]_NODATA, [TYPE]_USAGEERR, or [TYPE]_ERR
        public static bool VALID_STRING(string eval)
        {
            if (eval == STR_NODATA || eval == STR_USAGEERR || eval == STR_ERR)
                return false;
            return true;
        }

        public static bool VALID_INT(int eval)
        {
            if (eval == INT_NODATA || eval == INT_USAGEERR || eval == INT_ERR)
                return false;
            return true;
        }

        public static bool VALID_DOUBLE(double eval)
        {
            if (eval == DUB_NODATA || eval == DUB_USAGEERR || eval == DUB_ERR)
                return false;
            return true;
        }

        //-----------------------------------------------------------------------------------------
        //ENGINES
        //-----------------------------------------------------------------------------------------
        #region ENGINE RELATED VALUES
        //Engine values, for use internally
        public const byte J1939 = 1;
        public const byte J1708 = 2;
        public const byte INVALID_CAN_MESSAGE = 0xFF;
        //Engine header values: Messages from the JIB open with these characters
        public const char J1939_HEADER = 'R';
        public const char J1939_STATUS_HEADER = 'I';
        public const char J1708_HEADER = 'J';
        #endregion //ENGINE TYES


        //-----------------------------------------------------------------------------------------
        //Panel IDs
        //-----------------------------------------------------------------------------------------

        public struct PanelIDs
        {
            public const char VPANEL_ID = '0';
            public const char SIMPLE_GAUGE_ID = '1';
            public const char SCAN_GAUGE_ID = '2';
            public const char ODOMOTER_ID = '3';
            public const char TRANSMISSION_ID = '4';
            public const char MULTIBAR_ID = '5';
            public const char HISTOGRAM_ID = '6';
            public const char CLOCK_PANEL_ID = '7';
            public const char IMG_PANEL_ID = '8';
            public const char TEXT_PANEL_ID = '9';
            public const char TANK_MINDER_ID = 'A';
            public const char TIRE_PANEL_ID = 'B';
            public const char MESSAGE_PANEL_ID = 'C';
            public const char RESERVED_ID = 'D';
            public const char DIAG_ALARM_ID = 'E';
            public const char RADIAL_GAUGE_ID = 'F';
        }

        //-----------------------------------------------------------------------------------------
        //UI Helpers
        //-----------------------------------------------------------------------------------------

        public const int LEFT = 1;
        public const int RIGHT = 0;
        public const int UP = 2;
        public const int DOWN = 3;
        public const int HORIZONTAL = 0;
        public const int VERTICAL = 1;


        //-----------------------------------------------------------------------------------------
        //RV-C Helpers
        //-----------------------------------------------------------------------------------------

        public static byte RVC_BYTE(uint val)
        {
            return ((byte)(val & 0x000000FF));
        }
        public static ushort RVC_WORD(uint val)
        {
            return ((ushort)(val & 0x0000FFFF));
        }
        public static uint RVC_LONG(uint val)
        {
            return val;
        }
        public static bool PGN_IS_PROPRIETARY(uint pgn)
        {
            return ((pgn & 0x1FF00) == 0xEF00);
        }
        public const uint RVC_MAXVAL = 0xFFFFFFFD;
        public const uint RVC_NODATA = 0xFFFFFFFF;

        /// <summary>
        /// Converts a string representation of bytes into an actual byte array. E.g.: "FAFB" becomes [0xFA, 0xFB]
        /// </summary>
        /// <returns>False if the length is not divisible by 2. True otherwise</returns>
        public static bool ByteStringToByteArray(ref byte[] byteArr, string byteString, int length)
        {
            int arrLength = length / 2;
            if (length % 2 != 0)
                return false;
            for (int i = 0; i < arrLength; i++)
                byteArr[i] = Convert.ToByte(byteString.Substring((i * 2), 2));
            return true;
        }

        /// <summary>
        /// Converts 2 characters from a string representation of a hexadecimal byte and converts them to a byte
        /// </summary>
        public static byte BinConvert(char c1, char c2)
        {
            byte r = 0;
            if (c1 <= '9')
                r += (byte)(0x10 * (c1 - '0'));
            else
                r += (byte)(0x10 * (10 + c1 - 'A'));
            if (c2 <= '9')
                r += (byte)((c2 - '0'));
            else
                r += (byte)((10 + c2 - 'A'));
            return r;
        }

        /// <summary>
        /// Converts a string representation of bytes into an actual byte array (or List). E.g.: "FAFB" becomes [0xFA, 0xFB]
        /// </summary>
        /// <returns>False if the length is not divisible by 2. True otherwise</returns>
        public static bool ByteStringToByteArray(ref List<byte> byteArr, string byteString, int length)
        {
            try
            {
                if (length % 2 != 0)
                    return false;
                for (int i = 0; i < (length - 2); i += 2)
                    byteArr.Add(BinConvert(byteString[i], byteString[i+1]));
                return true;
            }
            catch(Exception ex)
            {
                VMSConsole.PrintLine(ex.Message);
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------
        //Misc Helpers
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// Attaches the specified callback method and interval to a timer object. Returns the timer object
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Timer CreateTimer(ElapsedEventHandler callback, int interval)
        {
            Timer timer = new Timer(interval);
            timer.Elapsed += callback;
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

        /// <summary>
        /// Returns the string representation of the provided pid
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string PidToString(this byte pid)
        {
            switch (pid)
            {
                case PIDs.retarderSwitch: return "Retarder Switch";
                case PIDs.retarderOilPressure: return "Retarder Oil Pressure";
                case PIDs.retarderOilTemp: return "Retarder Oil Temperature";
                case PIDs.retarderStatus: return "Retarder Status";
                case PIDs.percentAcceleratorPosition: return "Accelerator Position";
                case PIDs.voltage: return "Voltage";
                case PIDs.cruiseSpeed: return "Cruise Speed";
                case PIDs.coolantTemp: return "Coolant Temperature";
                case PIDs.engineLoad: return "Engine Load";
                case PIDs.fuelRate: return "Fuel Rate";
                case PIDs.fuelTemp: return "Fuel Temperature";
                case PIDs.instantMPG: return "Instant MPG";
                case PIDs.airInletTemp: return "Air Inlet Temperature";
                case PIDs.intakeTemp: return "Intake Temperature";
                case PIDs.oilPSI: return "Oil PSI";
                case PIDs.retarderPercent: return "Retarder Percent";
                case PIDs.oilTemp: return "Oil Temperature";
                case PIDs.roadSpeed: return "Road Speed";
                case PIDs.cruiseSetStatus: return "Cruise Set Status";
                case PIDs.engineSpeed: return "Engine Speed";
                case PIDs.transmissionTemp: return "Transmission Temperature";
                case PIDs.transmissionSpeed: return "Transmission Speed";
                case PIDs.turboBoost: return "Turbo Boost";
                case PIDs.rangeSelected: return "Range Selected";
                case PIDs.rangeAttained: return "Range Attained";
                case PIDs.totalMilesCummins: return "Total Miles (Cummins)";
                case PIDs.totalMiles: return "Total Miles";
                case PIDs.engineHours: return "Engine Hours";
                case PIDs.totalFuel: return "Total Fuel";
                case PIDs.diagnosticsPID: return "Diagnostic PID";
                default: return "Unkown(" + pid + ")";
            }
        }

        //-----------------------------------------------------------------------------------------
        //Communications Types
        //-----------------------------------------------------------------------------------------
        public const int USB = 0;
        public const int SERIAL = 1;
        public const int WIFI = 2;
        public const int LOGPLAYER = 3;


    }

    //*************************************************************************************
    //PID definitions
    //*************************************************************************************
    public static class PIDs
    {

        public const byte SINGLE_BYTE = 0;
        public const byte DOUBLE_BYTE = 1;
        public const byte NON_STANDARD = 0xFF;

        public struct PIDStruct
        {
            public byte PID;
            public string FriendlyName;
            public int NumDataBytes;
            public double StandardOffset;
            public double StandardMultiplier;
            public double MetricOffset;
            public double MetricMultiplier;
            public byte ParserType;
            public bool Prioritize1708;
            public PIDStruct(byte pid, int numDataBytes, string friendlyName)
            {
                PID = pid;
                NumDataBytes = numDataBytes;
                FriendlyName = friendlyName;
                StandardOffset = 0;
                StandardMultiplier = 1;
                MetricOffset = 0;
                MetricMultiplier = 1;
                ParserType = NON_STANDARD;
                Prioritize1708 = false;
            }
        }

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

        public static Dictionary<byte, PIDStruct> PIDList = new Dictionary<byte, PIDStruct>();

        static PIDs()
        {
            //                             PID  NumDataBytes       FriendlyName   
            PIDList.Add(47  , new PIDStruct(47, 2, "Retarder Switch"));
            PIDList.Add(119 , new PIDStruct(119, 2, "Retarder Oil Pressure"));
            PIDList.Add(120 , new PIDStruct(120, 2, "Retarder Oil Temperature"));
            PIDList.Add(121 , new PIDStruct(121, 2, "Retarder Status"));
            PIDList.Add(91  , new PIDStruct(91, 2, "Accelerator Position"));
            PIDList.Add(168 , new PIDStruct(168, 3, "Voltage"));
            PIDList.Add(86  , new PIDStruct(86, 2, "Cruise Speed"));
            PIDList.Add(110 , new PIDStruct(110, 2, "Coolant Temperature"));
            PIDList.Add(92  , new PIDStruct(92, 2, "Engine Load"));
            PIDList.Add(183 , new PIDStruct(183, 3, "Fuel Rate"));
            PIDList.Add(174 , new PIDStruct(174, 3, "Fuel Temperature"));
            PIDList.Add(184 , new PIDStruct(184, 3, "Instantaneous MPG"));
            PIDList.Add(172 , new PIDStruct(172, 3, "Air Inlet Temperature"));
            PIDList.Add(105 , new PIDStruct(105, 2, "Intake Temperature"));
            PIDList.Add(100 , new PIDStruct(100, 2, "Oil PSI"));
            PIDList.Add(122 , new PIDStruct(122, 2, "Retarder Percent"));
            PIDList.Add(175 , new PIDStruct(175, 3, "Oil Temperature"));
            PIDList.Add(84  , new PIDStruct(84, 2, "Road Speed"));
            PIDList.Add(85  , new PIDStruct(85, 2, "Cruise Set Status"));
            PIDList.Add(190 , new PIDStruct(190, 3, "Engine Speed"));
            PIDList.Add(177 , new PIDStruct(177, 3, "Transmission Temperature"));
            PIDList.Add(191 , new PIDStruct(191, 3, "Transmission Speed"));
            PIDList.Add(102 , new PIDStruct(102, 2, "Turbo Boost"));
            PIDList.Add(162 , new PIDStruct(162, 3, "Range Selected"));
            PIDList.Add(163 , new PIDStruct(163, 3, "Range Attained"));
            PIDList.Add(244 , new PIDStruct(244, 6, "Total Miles"));
            PIDList.Add(245 , new PIDStruct(245, 6, "Total Miles"));
            PIDList.Add(247 , new PIDStruct(247, 6, "Engine Hours"));
            PIDList.Add(250 , new PIDStruct(250, 6, "Total Fuel"));
            PIDList.Add(0xC2, new PIDStruct(0xC2, 1000, "Diagnostic"));
            PIDList.Add(0xC0, new PIDStruct(0xC0, 1000, "Multipart"));
        }
    }

    public static class PGN_Constants
    {
    }
}
