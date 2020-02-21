using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMSpc.DevHelpers;
using VMSpc.Parsers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.CustomComponents;
using static VMSpc.Constants;
using static VMSpc.Parsers.PresenterWrapper;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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

        //object checking
        /// <summary> Shorthand method for checking whether or not an object is null </summary>
        public static bool NOT_NULL(object obj)
        {
            return obj != null;
        }

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
            public const char NO_ID = '~';
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

        public const int RESIZE_TOP = 0x0001;
        public const int RESIZE_BOTTOM = 0x0010;
        public const int RESIZE_RIGHT = 0x0100;
        public const int RESIZE_LEFT = 0x1000;
        public const int RESIZE_TOPLEFT = RESIZE_TOP + RESIZE_LEFT;
        public const int RESIZE_TOPRIGHT = RESIZE_TOP + RESIZE_RIGHT;
        public const int RESIZE_BOTTOMLEFT = RESIZE_BOTTOM + RESIZE_LEFT;
        public const int RESIZE_BOTTOMRIGHT = RESIZE_BOTTOM + RESIZE_RIGHT;

        public const int RESIZE_NONE = -1;

        public const int MOVEMENT_RESIZE = 1;
        public const int MOVEMENT_MOVE = 2;
        public const int MOVEMENT_NONE = 0;

        /// <summary>
        /// Method to map a sane text alignment enumeration (Left = 0, Center = 1, Right = 2) to the stupid order imposed by TextAlignment
        /// </summary>
        public static TextAlignment GET_TEXT_ALIGNMENT(int alignment)
        {
            switch (alignment)
            {
                case 0:
                    return TextAlignment.Left;
                case 1:
                    return TextAlignment.Center;
                case 2:
                    return TextAlignment.Right;
                default:
                    return TextAlignment.Left;
            }
        }

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

        public static bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            if (storage.GetType() != value.GetType())
                return false;
            storage = value;
            return true;
        }


        /// <summary>
        /// Converts a string representation of bytes into a List<byte> object. E.g.: "FAFB" becomes [0xFA, 0xFB]
        /// </summary>
        /// <returns>False if the length is not divisible by 2. True otherwise</returns>
        public static bool BYTE_STRING_TO_BYTE_ARRAY(ref List<byte> byteArr, string byteString, int length)
        {
            try
            {
                if (length % 2 != 0)
                    return false;
                for (int i = 0; i <= (length - 2); i += 2)
                    byteArr.Add(BinConvert(byteString[i], byteString[i + 1]));
                return true;
            }
            catch (Exception ex)
            {
                //VMSConsole.PrintLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Converts a string representation of bytes into an actual byte array. E.g.: "FAFB" becomes [0xFA, 0xFB]
        /// </summary>
        /// <returns>False if the length is not divisible by 2. True otherwise</returns>
        public static bool BYTE_STRING_TO_BYTE_ARRAY(byte[] byteArr, string byteString, int length)
        {
            int arrLength = length / 2;
            if (length % 2 != 0)
                return false;
            for (int i = 0; i < arrLength; i++)
                byteArr[i] = Convert.ToByte(byteString.Substring((i * 2), 2), 16);
            return true;
        }

        /// <summary>
        /// converts a string representation of a 
        /// </summary>
        /// <param name="hex_message"></param>
        /// <returns></returns>
        public static uint STRING_TO_UINT(string hex_message)
        {
            return 0;
        }

        //-----------------------------------------------------------------------------------------
        //Event-Handling helpers
        //-----------------------------------------------------------------------------------------
        public static uint PID_BASE = 0x00010000;
        public static uint DIAGNOSTIC_BASE = 0x00020000;
        public static uint TIRE_BASE = 0x00030000;

        //-----------------------------------------------------------------------------------------
        //Misc Helpers
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// Attaches the specified callback method and interval to a timer object. Returns the timer object
        /// </summary>
        /// <returns></returns>
        public static Timer CREATE_TIMER(Action callback, int millisecond_interval)
        {
            Timer timer = new Timer(millisecond_interval);
            timer.Elapsed += (object obj, ElapsedEventArgs e) => callback();
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

        /// <summary> returns the passed value if valid (!= DUB_NODATA). Otherwise, returns 0 </summary>
        public static double ZERO_IF_INVALID(double value)
        {
            if (value == DUB_NODATA)
                return 0;
            return value;
        }

        /// <summary>
        /// Returns a copy of the object parameter for copying by value
        /// </summary>
        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary> Prevents division by zero or subzero dividend. Also prevents division where either operator is NaN </summary>
        public static double SAFE_DIVIDE(double dividend, double divisor)
        {
            if (divisor <= 0 || Double.IsNaN(dividend) || Double.IsNaN(divisor))
                return 0;
            return dividend / divisor;
        }

        /// <summary> returns a new filename with the extension converted </summary>
        public static string CHANGE_FILE_TYPE(string fileName, string originalExtension, string newExtension)
        {
            return fileName.Replace(originalExtension, newExtension);
        }

        public static double DoubleArrayMax(double[] arr)
        {
            double max = 0;
            foreach (var item in arr)
            {
                if (item > max)
                    max = item;
            }
            return max;
        }

        public static double DoubleArrayMin(double[] arr)
        {
            double min = Double.MaxValue;
            foreach (var item in arr)
            {
                if (item < min)
                    min = item;
            }
            return min;
        }

        public static double DoubleArrayMaxNegative(double[] arr)
        {
            double val = -(DoubleArrayMin(arr));
            return -(DoubleArrayMin(arr));
        }

        public static void ArrayFill<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = value;
        }

        /// <summary> Shorthand for incrementing or decrementing an index for an array. Keeps the index in the bounds of the array (assuming a minimum index of 0 </summary>
        public static void SafeIndexAdd(ref int index, int addend, int arraySize, int min = 0)
        {
            index += addend;
            if (index < min) index = arraySize + index;
            index %= arraySize;
        }

        /// <summary> Counts (and returns) the number of true arguments passed. e.g., TruthCount(true, false, true) = 2 </summary>
        public static int TruthCount(params bool[] list)
        {
            int total = 0;
            for (int i = 0; i < list.Length; i++)
                total += (list[i] ? 1 : 0);
            return total;
        }

        public const byte SINGLE_BYTE = 0;
        public const byte DOUBLE_BYTE = 1;
        public const byte NON_STANDARD = 0xFF;

        /// <summary>
        /// Returns the string representation of the provided pid
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string PidToString(this byte pid)
        {
            switch (pid)
            {
                case PIDWrapper.retarderSwitch: return "Retarder Switch";
                case PIDWrapper.retarderOilPressure: return "Retarder Oil Pressure";
                case PIDWrapper.retarderOilTemp: return "Retarder Oil Temperature";
                case PIDWrapper.retarderStatus: return "Retarder Status";
                case PIDWrapper.percentAcceleratorPosition: return "Accelerator Position";
                case PIDWrapper.voltage: return "Voltage";
                case PIDWrapper.cruiseSpeed: return "Cruise Speed";
                case PIDWrapper.coolantTemp: return "Coolant Temperature";
                case PIDWrapper.engineLoad: return "Engine Load";
                case PIDWrapper.fuelRate: return "Fuel Rate";
                case PIDWrapper.fuelTemp: return "Fuel Temperature";
                case PIDWrapper.instantMPG: return "Instant MPG";
                case PIDWrapper.airInletTemp: return "Air Inlet Temperature";
                case PIDWrapper.intakeTemp: return "Intake Temperature";
                case PIDWrapper.oilPSI: return "Oil PSI";
                case PIDWrapper.retarderPercent: return "Retarder Percent";
                case PIDWrapper.oilTemp: return "Oil Temperature";
                case PIDWrapper.roadSpeed: return "Road Speed";
                case PIDWrapper.cruiseSetStatus: return "Cruise Set Status";
                case PIDWrapper.engineSpeed: return "Engine Speed";
                case PIDWrapper.transmissionTemp: return "Transmission Temperature";
                case PIDWrapper.transmissionSpeed: return "Transmission Speed";
                case PIDWrapper.turboBoost: return "Turbo Boost";
                case PIDWrapper.rangeSelected: return "Range Selected";
                case PIDWrapper.rangeAttained: return "Range Attained";
                case PIDWrapper.totalMilesCummins: return "Total Miles (Cummins)";
                case PIDWrapper.totalMiles: return "Total Miles";
                case PIDWrapper.engineHours: return "Engine Hours";
                case PIDWrapper.totalFuel: return "Total Fuel";
                case PIDWrapper.diagnosticsPID: return "Diagnostic PID";
                default: return "Unkown(" + pid + ")";
            }
        }

        //-----------------------------------------------------------------------------------------
        //Communications Types
        //-----------------------------------------------------------------------------------------
        public const int SERIAL = 0;
        public const int USB = 1;
        public const int WIFI = 2;
        public const int LOGPLAYER = 3;

        public const int PARSE_ALL = 0;
        public const int IGNORE_1939 = 1;
        public const int IGNORE_1708 = 2;
        public const int FAVOR_1939 = 3;
        public const int FAVOR_1708 = 4;

        public const byte LOGTYPE_RAWLOG = 0;
        public const byte LOGTYPE_PARSEREADY = 1;
        public const byte LOGTYPE_FULL = 2;

        //-----------------------------------------------------------------------------------------
        //TPMS Types
        //-----------------------------------------------------------------------------------------
        public const int TPMS_NONE = 0;
        public const int TPMS_TST = 1;
        public const int TPMS_PRESSURE_PRO = 2;

        public static object DynamicCast(object obj, string type)
        {
            object castedObject;
            castedObject = new Border();
            return castedObject;


        }

    } //End of Constants class
}
