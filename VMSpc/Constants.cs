using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    //ENGINE RELATED MACROS
    //-----------------------------------------------------------------------------------------

        public const int J1939 = 0;
        public const int J1708 = 1;

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

    }
}
