using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
        public const byte INVALID_ENGINE = 0xFF;
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
            public const char VPANEL_ID        = '0';
            public const char SIMPLE_GAUGE_ID  = '1';
            public const char SCAN_GAUGE_ID    = '2';
            public const char ODOMOTER_ID      = '3';
            public const char TRANSMISSION_ID  = '4';
            public const char MULTIBAR_ID      = '5';
            public const char HISTOGRAM_ID     = '6';
            public const char CLOCK_PANEL_ID   = '7';
            public const char IMG_PANEL_ID     = '8';
            public const char TEXT_PANEL_ID    = '9';
            public const char TANK_MINDER_ID   = 'A';
            public const char TIRE_PANEL_ID    = 'B';
            public const char MESSAGE_PANEL_ID = 'C';
            public const char RESERVED_ID      = 'D';
            public const char DIAG_ALARM_ID    = 'E';
            public const char RADIAL_GAUGE_ID  = 'F';
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
        /// <param name="byteArr"></param>
        /// <param name="byteString"></param>
        /// <param name="length"></param>
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

        //-----------------------------------------------------------------------------------------
        //Misc Helpers
        //-----------------------------------------------------------------------------------------
        public static void CreateTimer(Timer timer, ElapsedEventHandler callback, int interval)
        {
            timer = new Timer(interval);
            timer.Elapsed += callback;
            timer.AutoReset = true;
            timer.Enabled = true;
        }


    }

    //*************************************************************************************
    //PID definitions
    //*************************************************************************************
    public static class PIDs
    {
        public const byte retarderSwitch = 47;
        public const byte retarderOilPressur = 119;
        public const byte retarderOilTemp = 120;
        public const byte retarderOilStatus = 121;
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
    }
}
