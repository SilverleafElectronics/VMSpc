using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Enums.Parsing;
using VMSpc.Common;

namespace VMSpc.Parsers
{
    public abstract class DiagnosticRecord
    {
        /// <summary>
        /// Initializes an empty DiagnosticRecord, using the current time for the time
        /// </summary>
        public DiagnosticRecord()
        {
            time = DateTime.Now;
            dataBusType = DataBusType.NONE;
            odometer = ChassisParameter.ChassisParam.odometer;
            coolant = SPNDefinitions.spn_coolantLevel.rawValue;
            rpms = SPNDefinitions.spn_rpms.rawValue;
            spn = 0xFFFFFFFF;
        }
        public uint spn;
        public DateTime time;
        public DataBusType dataBusType;
        public byte fmi;
        public double
            odometer,
            coolant,
            rpms;
    }
    public class J1939Record : DiagnosticRecord
    {
        public J1939Record() : base()
        { 
            mode = 0xFF;
        }
        public byte mode;
        public override string ToString()
        {
            string retString = "";
            return retString;
        }
    }
    public class  J1708Record : DiagnosticRecord
    {
        public J1708Record() : base()
        {
            ecode = mid = count = 0xFF;
        }
        public byte
            ecode,
            mid,
            count;
        public override string ToString()
        {
            string codeType = DiagnosticGuiHelper.CodeTypeAsString(mid);
            string ecodeType = ((this.ecode & 0x10) == 0x10) ? " SID" : " PID";
            string spn = string.Format(" {0, 5}", this.spn);
            string ecode = string.Format(" {0, 2}", this.ecode);
            string paramString = (ecodeType == "SID")
                ? Diagnostics.GetSIDString(this.mid, (ushort)this.spn) 
                : Diagnostics.GetPidString((ushort)this.spn);
            string mode = Diagnostics.GetModeString((byte)(this.ecode & 0x0F));
            string dateString;
            if (ChassisParameter.ChassisParam.diagActiveFlag)
                dateString = "** INACTIVE **";
            else
                dateString = time.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            return codeType + ecodeType + spn + ecode + paramString + mode + dateString;
        }
    }

    public static class DiagnosticGuiHelper
    {
        public static string GetDiagnosticString(DiagnosticRecord record)
        {
            return record.ToString();
        }

        public static string CodeTypeAsString(byte code)
        {
            switch (code)
            {
                case 0x80: return "ENG";
                case 0x82: return "TRN";
                case 0x88: return "AGS";
                default:   return string.Format("{0,3}", code);
            }
        }
    }

    public sealed class Diagnostics : IEventPublisher
    {
        static Diagnostics() { }
        public static Diagnostics diagnostics { get; set; } = new Diagnostics();
        public Diagnostics()
        {
            EventBridge.EventProcessor.AddEventPublisher(this);
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;
        private void OnRaiseCustomEvent(DiagnosticEventArgs e)
        {
            RaiseVMSEvent?.Invoke(this, e);
        }


        public static uint numDiagCodes = 0;
        public static DiagnosticRecord[] DiagnosticRecords = new DiagnosticRecord[20];

        public static string GetPidString(ushort pid)
        {
            switch (pid)
            {
                case 2: return "Transmitter System Status     ";
                case 3: return "Transmitter System Diagnostic ";
                case 5: return "Underrange Warning Condition  ";
                case 6: return "Overrange Warning Condition   ";
                case 13: return "Entry Assist Deployment       ";
                case 14: return "Entry Assist Motor Current    ";
                case 15: return "Fuel Supply Pump Inlet Prssr  ";
                case 16: return "Suction Side Fuel Fltr Prssr  ";
                case 17: return "Engine Oil Level Remote Resvr ";
                case 18: return "Extended Range Fuel Pressure  ";
                case 19: return "Ext. Range Engine Oil Pressure";
                case 20: return "Ext. Range Engine Coolant Prsr";
                case 21: return "Engine ECU Temperature        ";
                case 22: return "Ext. Eng. Crnkcase BlowBy Prss";
                case 23: return "Generator Oil Pressure        ";
                case 24: return "Generator Coolant Temperature ";
                case 25: return "Air Conditioner System #2     ";
                case 26: return "Estimated Percent Fan Speed   ";
                case 27: return "% Exhaust Gas Recirc Valve Pos";
                case 28: return "% Accelerator Position #3     ";
                case 29: return "% Accelerator Position #2     ";
                case 30: return "Crankcase Blow-by Pressure    ";
                case 31: return "Transmission Range Position   ";
                case 32: return "Transmission Splitter Position";
                case 33: return "Clutch Cylinder Position      ";
                case 34: return "Clutch Cylinder Actuator      ";
                case 35: return "Shift Finger Actuator #2      ";
                case 36: return "Clutch Plates Wear Condition  ";
                case 37: return "Transmission Tank Air Pressure";
                case 38: return "Second Fuel Level (Right Side)";
                case 39: return "Tire Pressure Check Interval  ";
                case 40: return "Engine Retarder Switches      ";
                case 41: return "Cruise Control Switches Status";
                case 42: return "Pressure Switch Status        ";
                case 43: return "Ignition Switch Status        ";
                case 44: return "Warning Indicator Lamps       ";
                case 45: return "Inlet Air Heater Status       ";
                case 46: return "Vehicle Wet Tank Pressure     ";
                case 47: return "Retarder Status               ";
                case 48: return "Ext. Range Barometric Pressure";
                case 49: return "ABS Control Status            ";
                case 50: return "A/C System Clutch Status #1   ";
                case 51: return "Throttle Position             ";
                case 52: return "Engine Intercooler Temperature";
                case 53: return "Transmission Synchro Clutch   ";
                case 54: return "Transmission Synchro Brake    ";
                case 55: return "Shift Finger Positional Status";
                case 56: return "Transmission Range Switch     ";
                case 57: return "Transmission Actuator #2      ";
                case 58: return "Shift Finger Actuator Status  ";
                case 59: return "Shift Finger Gear Position    ";
                case 60: return "Shift Finger Rail Position    ";
                case 61: return "Parking Brake Actuator Status ";
                case 62: return "Retarder Inhibit Status       ";
                case 63: return "Transmission Actuator #1      ";
                case 64: return "Direction Switch Status       ";
                case 65: return "Service Brake Switch Status   ";
                case 66: return "Vehicle Enabling Component    ";
                case 67: return "Shift Request Switch Status   ";
                case 68: return "Torque Limiting Factor        ";
                case 69: return "Two Speed Axle Switch Status  ";
                case 70: return "Parking Brake Switch Status   ";
                case 71: return "Idle Shutdown Timer Status    ";
                case 72: return "Blower Bypass Value Position  ";
                case 73: return "Lift Pump                     ";
                case 74: return "Maximum Road Speed Limit      ";
                case 75: return "Steering Axle Temperature     ";
                case 76: return "Axle Lift Air Pressure        ";
                case 77: return "Forward Rear Drive Axle Temp  ";
                case 78: return "Rear Rear-Drive Axle Temp     ";
                case 79: return "Road Surface Temperature      ";
                case 80: return "Washer Fluid Level            ";
                case 81: return "Particulate Trap Inlet Prssr  ";
                case 82: return "Air Start Pressure            ";
                case 83: return "Road Speed Limit Status       ";
                case 84: return "Road Speed                    ";
                case 85: return "Cruise Control Status         ";
                case 86: return "Cruise Control Set Speed      ";
                case 87: return "Cruise Control High-Set Limit ";
                case 88: return "Cruise Control Low-Set Limit  ";
                case 89: return "Power Takeoff Status          ";
                case 90: return "PTO Oil Temperature           ";
                case 91: return "Accelerator Pedal Position    ";
                case 92: return "Percent Engine Load           ";
                case 93: return "Output Torque                 ";
                case 94: return "Fuel Delivery Pressure        ";
                case 95: return "Fuel Filter Differential Prssr";
                case 96: return "Fuel Level                    ";
                case 97: return "Water in Fuel Indicator       ";
                case 98: return "Engine Oil Level              ";
                case 99: return "Engine Oil Filter Diff Prssr  ";
                case 100: return "Engine Oil Pressure           ";
                case 101: return "Crankcase Pressure            ";
                case 102: return "Boost Pressure                ";
                case 103: return "Turbo Speed                   ";
                case 104: return "Turbo Oil Pressure            ";
                case 105: return "Intake Manifold Temperature   ";
                case 106: return "Air Inlet Pressure            ";
                case 107: return "Air Filter Differential Prssr ";
                case 108: return "Barometric Pressure           ";
                case 109: return "Coolant Pressure              ";
                case 110: return "Engine Coolant Temperature    ";
                case 111: return "Coolant Level                 ";
                case 112: return "Coolant Filter Diff. Prssr    ";
                case 113: return "Governor Droop                ";
                case 114: return "Net Battery Current           ";
                case 115: return "Alternator Current            ";
                case 116: return "Brake Application Pressure    ";
                case 117: return "Brake Primary Pressure        ";
                case 118: return "Brake Secondary Pressure      ";
                case 119: return "Hydraulic Retarder Pressure   ";
                case 120: return "Hydraulic Retarder Oil Temp   ";
                case 121: return "Engine Retarder Status        ";
                case 122: return "Engine Retarder Percent       ";
                case 123: return "Clutch Pressure               ";
                case 124: return "Transmission Oil Level        ";
                case 125: return "Transmission Oil Level        ";
                case 126: return "Transmission Filter Diff. Prss";
                case 127: return "Transmission Oil Pressure     ";
                case 128: return "Component-specific request    ";
                case 129: return "Injector Metering Rail #2 Prss";
                case 130: return "Power Specific Fuel Economy   ";
                case 131: return "Exhaust Back Pressure         ";
                case 132: return "Mass Air Flow                 ";
                case 133: return "Average Fuel Rate             ";
                case 134: return "Wheel Speed Sensor Status     ";
                case 135: return "Fuel Delivery Pressure        ";
                case 136: return "Auxiliary Vacuum Pressure     ";
                case 137: return "Auxiliary Gage Pressure #1    ";
                case 138: return "Auxiliary Absolute Pressure   ";
                case 139: return "Tire Pressure Control System  ";
                case 140: return "Tire Pressure Control Solenoid";
                case 141: return "Tire Pressure Target          ";
                case 142: return "Drive Tire Pressure Target    ";
                case 143: return "Steer Tire Pressure Target    ";
                case 144: return "Tire Pressure                 ";
                case 145: return "Drive Channel Tire Pressure   ";
                case 146: return "Steer Channel Tire Pressure   ";
                case 147: return "Average Fuel Economy          ";
                case 148: return "Instantaneous Fuel Economy    ";
                case 149: return "Fuel Mass Flow Rate           ";
                case 150: return "PTO Engagement Control Status ";
                case 151: return "ATC Control Status            ";
                case 152: return "Number of ECU Resets          ";
                case 153: return "Crankcase Pressure            ";
                case 154: return "Auxiliary Input and Output #2 ";
                case 155: return "Auxiliary Input and Output #1 ";
                case 156: return "Injector Timing Rail Pressure ";
                case 157: return "Injector Metering Rail Press  ";
                case 158: return "Battery Potential             ";
                case 159: return "Gas Supply Pressure           ";
                case 160: return "Main Shaft Speed              ";
                case 161: return "Input Shaft Speed             ";
                case 162: return "Transmission Range Selected   ";
                case 163: return "Transmission Range Attained   ";
                case 164: return "Injection Control Pressure    ";
                case 165: return "Compass Bearing               ";
                case 166: return "Rated Engine Power            ";
                case 167: return "Alternator Potential          ";
                case 168: return "Battery Potential             ";
                case 169: return "Cargo Ambient Temperature     ";
                case 170: return "Cab Interior Temperature      ";
                case 171: return "Ambient Air Temperature       ";
                case 172: return "Air Inlet Temperature         ";
                case 173: return "Exhaust Gas Temperature       ";
                case 174: return "Fuel Temperature              ";
                case 175: return "Engine Oil Temperature        ";
                case 176: return "Turbo Oil Temperature         ";
                case 177: return "Transmission Oil Temperature  ";
                case 178: return "Front Axle Weight             ";
                case 179: return "Rear Axle Weight              ";
                case 180: return "Trailer Weight                ";
                case 181: return "Cargo Weight                  ";
                case 182: return "Trip Fuel                     ";
                case 183: return "Fuel Rate                     ";
                case 184: return "Instantaneous Fuel Economy    ";
                case 185: return "Average Fuel Economy          ";
                case 186: return "Power Takeoff Speed           ";
                case 187: return "Power Takeoff Set Speed       ";
                case 188: return "Idle Engine Speed             ";
                case 189: return "Rated Engine Speed            ";
                case 190: return "Engine Speed                  ";
                case 191: return "Transmission Output Shaft Spd ";
                case 192: return "Multisection Parameter        ";
                case 193: return "Transmitter System Diagnostic ";
                case 194: return "Transmitter System Diagnostic ";
                case 195: return "Diagnostic Data               ";
                case 196: return "Diagnostic Data               ";
                case 197: return "Connection Management         ";
                case 198: return "Connection Mode Data Transfer ";
                case 199: return "Traction Control Disable State";
                case 218: return "State Line Crossing           ";
                case 219: return "Current State and Country     ";
                case 220: return "Engine Torque History         ";
                case 221: return "Anti-theft Request            ";
                case 222: return "Anti-theft Status             ";
                case 223: return "Auxiliary A/D Counts          ";
                case 224: return "Immobilizer Security Code     ";
                case 228: return "Speed Sensor Calibration      ";
                case 229: return "Total Fuel Used               ";
                case 230: return "Total Idle Fuel Used          ";
                case 231: return "Trip Fuel                     ";
                case 232: return "DGPS Differential Correction  ";
                case 233: return "Unit Number                   ";
                case 234: return "Software Identification       ";
                case 235: return "Total Idle Hours              ";
                case 236: return "Total Idle Fuel Used          ";
                case 237: return "Vehicle Identification Number ";
                case 238: return "Velocity Vector               ";
                case 239: return "Vehicle Position              ";
                case 240: return "Change Reference Number       ";
                case 241: return "Tire Pressure                 ";
                case 242: return "Tire Temperature              ";
                case 243: return "Component Identification      ";
                case 244: return "Trip Distance                 ";
                case 245: return "Total Vehicle Distance        ";
                case 246: return "Total Vehicle Hours           ";
                case 247: return "Total Engine Hours            ";
                case 248: return "Total PTO Hours               ";
                case 249: return "Total Engine Revolutions      ";
                case 250: return "Total Fuel Used               ";
                case 251: return "Clock                         ";
                case 252: return "Date                          ";
                case 253: return "Elapsed Time                  ";
                case 256: return "Parameter Request             ";
                case 257: return "Cold Restart Request          ";
                case 258: return "Warm Restart Request          ";
                case 259: return "Component Restart             ";
                case 351: return "Turbo Inlet Temperature       ";
                case 354: return "Humidity Sensor               ";

                case 362: return "EGR Valve #2 Position         ";
                case 363: return "Hydraulic Retarder Control Prs";
                case 364: return "HVAC Unit Discharge Temp      ";
                case 365: return "Weighing System Status        ";
                case 366: return "Engine Oil Level              ";
                case 367: return "Lane Tracking System Status   ";
                case 368: return "Lane Departure Indication     ";
                case 369: return "Distance to Rear Object       ";
                case 370: return "Trailer Pnmtc Brake Control Pr";
                case 371: return "Trailer Pnmtc Supply Line Prsr";
                case 372: return "Remote Accelerator            ";
                case 373: return "Center Rear Drive Axle Temp   ";
                case 374: return "Alternator AC Voltage         ";
                case 375: return "Fuel Return Pressure          ";
                case 376: return "Fuel Pump Inlet Vacuum        ";
                case 377: return "Compression Unbalance         ";
                case 383: return "Vehicle Acceleration          ";
                case 404: return "Turbo Outlet Temperature      ";
                case 411: return "EGR Gas Differential Pressure ";
                case 412: return "EGR Exhaust Gas Temperature   ";
                case 419: return "Starter Circuit Resistance    ";
                case 420: return "Starter Current (Average)     ";
                case 421: return "Alternator Neg. Cable Voltage ";
                case 422: return "Auxiliary Current             ";
                case 423: return "Ext Net Battery Current       ";
                case 424: return "DC Voltage                    ";
                case 425: return "Auxiliary Frequency           ";
                case 426: return "Alternator Field Voltage      ";
                case 427: return "Battery Resistance Change     ";
                case 428: return "Battery Internal Resistance   ";
                case 429: return "Starter Current Peak          ";
                case 430: return "Starter Solenoid Voltage      ";
                case 431: return "Starter Negative Cable Voltage";
                case 432: return "Starter Motor Voltage         ";
                case 433: return "Fuel Shutoff Solenoid Voltage ";
                case 434: return "AC Voltage                    ";
                case 436: return "Trip Sudden Decelerations     ";
                case 439: return "Boost Pressure #1             ";
                case 440: return "Boost Pressure #2             ";
                case 441: return "Auxiliary Temperature #1      ";
                case 442: return "Auxiliary Temperature #2      ";
                case 443: return "Auxiliary Gage Pressure #2    ";
                case 444: return "Battery #2 Voltage            ";
                case 445: return "Cylinder Head Temperature B   ";
                case 446: return "Cylinder Head Temperature A   ";
                case 524: return "Transmission Select Gear      ";
                case 558: return "Accelerator Pedal             ";
                case 592: return "Shutdown Timer                ";
                case 593: return "Shutdown Timer                ";
                case 596: return "Cruise Enable Switch          ";
                case 597: return "Brake Switch                  ";
                case 598: return "Clutch Switch                 ";
                case 599: return "Cruise Set Switch             ";
                case 600: return "Cruise Coast Switch           ";
                case 601: return "Cruise Resume Switch          ";
                case 602: return "Cruise Accelerate Switch      ";
                case 603: return "Brake Pedal Switch #2         ";
                case 612: return "Mag Speed Signal              ";
                case 626: return "Ether Injector                ";
                case 627: return "Injector Power                ";
                case 629: return "ECM Internal                  ";
                case 630: return "Calibration Memory            ";
                case 631: return "Calibration Mod               ";
                case 632: return "Fuel Shtoff Valve             ";
                case 633: return "Injector Valve                ";
                case 635: return "Timing Actuator               ";
                case 637: return "Timing Sensor                 ";
                case 639: return "J1939 Data Link               ";
                case 640: return "Shutdown Command              ";
                case 641: return "VGT Driver Temperature        ";
                case 644: return "External Speed Input          ";
                case 647: return "Fan Control                   ";
                case 651: return "Injector Driver 1             ";
                case 652: return "Injector Driver 2             ";
                case 653: return "Injector Driver 3             ";
                case 654: return "Injector Driver 4             ";
                case 655: return "Injector Driver 5             ";
                case 656: return "Injector Driver 6             ";
                case 677: return "Starter Driver                ";
                case 678: return "ECU 8 Volt Supply             ";
                case 697: return "Aux PWM Driver                ";
                case 703: return "Aux IO Circuit 3              ";
                case 706: return "Aux IO Circuit 6              ";
                case 707: return "Aux IO Circuit 7              ";
                case 723: return "Camshaft Alignment            ";
                case 729: return "Intake Heater                 ";
                case 973: return "Retarder Select               ";
                case 974: return "Remote Accelerator Pedal      ";
                case 975: return "Fan Speed                     ";
                case 979: return "Remote PTO Switch             ";
                case 980: return "PTO Enable Switch             ";
                case 982: return "PTO Resume Switch             ";
                case 984: return "PTO Set Switch                ";
                case 986: return "Fan Speed Percent             ";
                case 1072: return "Brake Driver 1                ";
                case 1073: return "Brake Driver 2                ";
                case 1075: return "Fuel Lift Pump                ";
                case 1110: return "Engine Shutdown               ";
                case 1112: return "Brake Driver 3                ";
                case 1136: return "ECM Temperature               ";
                case 1172: return "Turbo Inlet Temperature       ";
                case 1209: return "Exhaust Pressure              ";
                case 1237: return "Shutdown Override             ";
                case 1244: return "Fuel Driver 2                 ";
                case 1245: return "Timing Driver 2               ";
                case 1267: return "Accessory Relay               ";
                case 1347: return "Fuel Pump 1 Pressure          ";
                case 1349: return "Injector 2 Pressure           ";
                case 1378: return "Oil Change Schedule           ";
                case 1388: return "Aux Pressure Sensor #2        ";
                case 1590: return "Adaptive Cruise Control       ";
                case 1633: return "Cruise Pause Switch           ";
                case 1845: return "Transmission Torque           ";
                case 2623: return "Accelerator Pedal             ";
                case 2629: return "Turbo Outlet Temperature      ";
                case 2789: return "Turbo Inlet Temperature       ";
                case 2791: return "EGR Valve Control             ";
                case 2797: return "Injector Codes                ";
                case 2813: return "Air Shutoff                   ";
                case 2900: return "Transmission Crank            ";
                case 2948: return "Intake Oil Pressure           ";
                case 2949: return "Intake Oil Pressure           ";
                case 2950: return "Intake Actuator #1            ";
                case 2951: return "Intake Actuator #2            ";
                case 2952: return "Intake Actuator #3            ";
                case 2953: return "Intake Actuator #4            ";
                case 2954: return "Intake Actuator #5            ";
                case 2955: return "Intake Actuator #6            ";
                case 3050: return "DPF Efficiency                ";
                case 3058: return "EGR System                    ";
                case 3064: return "No DPF Trap                   ";
                case 3241: return "DPF Exhaust Temp #1           ";
                case 3242: return "DPF Intake Temp #1            ";
                case 3245: return "DPF Exhaust Temp #3           ";
                case 3246: return "DPF Outlet Temp #1            ";
                case 3249: return "DPF Exhaust Temp #2           ";
                case 3251: return "DPF Filter Pressure           ";
                case 3276: return "DPF Intake Temp #2            ";
                case 3280: return "DPF Outlet Temp #2            ";
                case 3285: return "DPF Pressure #2               ";
                case 3363: return "SCR Tank Heater               ";
                case 3473: return "DPF #1 Ignition               ";
                case 3474: return "DPF #1 Combustion             ";
                case 3479: return "DPF #1 Fuel Pressure          ";
                case 3480: return "DPF Fuel Pressure             ";
                case 3481: return "DPF Fuel Rate                 ";
                case 3482: return "DPF Fuel Valve #1             ";
                case 3484: return "DPF #1 Ignition               ";
                case 3486: return "DPF #1 Air Pressure           ";
                case 3487: return "DPF #1 Air Pressure           ";
                case 3490: return "DPF #1 Purge Valve            ";
                case 3494: return "DPF #2 Fuel Pressure          ";
                case 3509: return "Sensor 1 Circuit              ";
                case 3510: return "Sensor 2 Circuit              ";
                case 3511: return "Sensor 3 Circuit              ";
                case 3512: return "Sensor 4 Circuit              ";
                case 3513: return "Sensor 5 Circuit              ";
                case 3530: return "DPF #1 Regen                  ";
                case 3532: return "SCR Tank Level Status         ";
                case 3555: return "Altitude                      ";
                case 3556: return "DPF Injector                  ";
                case 3597: return "ECU DC Output 1               ";
                case 3598: return "ECU DC Output 2               ";
                case 3659: return "Cylinder 1 Actuator #2        ";
                case 3660: return "Cylinder 2 Actuator #2        ";
                case 3661: return "Cylinder 3 Actuator #2        ";
                case 3662: return "Cylinder 4 Actuator #2        ";
                case 3663: return "Cylinder 5 Actuator #2        ";
                case 3664: return "Cylinder 6 Actuator #2        ";
                case 3695: return "Regen Inhibit Switch          ";
                case 3696: return "Regen Force Switch            ";
                case 3703: return "DPF Inhibited                 ";
                case 3711: return "DPF Inhibited                 ";
                case 3714: return "DPF Inhibited                 ";
                case 3719: return "DPF 1 Soot Load               ";
                case 3830: return "DPF 1 Pressure 2              ";
                case 3832: return "DPF 1 Air Flow 2              ";
                case 4097: return "DPF Fuel Drain                ";
                //case 520192: return "CGI Flow Rate                 ";
                //case 520193: return "CGI Gas Temp                  ";
                //case 520194: return "CGI Actuator                  ";
                //case 520196: return "CGI Pressure                  ";
                //case 520197: return "CGI Pressure                  ";
                default: return "UNKNOWN COMPONENT             ";
            }
        }

        public static string GetSIDString(byte source, ushort sid)
        {
            if (sid > 155)
            {
                switch (sid)
                {
                    case 216: return "Other ECUs Reporting Faults   ";
                    case 217: return "Anti-theft Start Inhibit      ";
                    case 218: return "ECM Main Relay                ";
                    case 219: return "Start Signal Indicator        ";
                    case 220: return "Tractor/Trailer Interface     ";
                    case 221: return "Internal Sensor Voltage Supply";
                    case 222: return "Protect Lamp                  ";
                    case 223: return "Ambient Light Sensor          ";
                    case 224: return "Audible Alarm                 ";
                    case 225: return "Green Lamp                    ";
                    case 226: return "Transmission Neutral Switch   ";
                    case 227: return "Auxiliary Analog Input #1     ";
                    case 228: return "High Side Refrigerant Pressure";
                    case 229: return "Kickdown Switch               ";
                    case 230: return "Idle Validation Switch        ";
                    case 231: return "SAE J1939 Data Link           ";
                    case 232: return "5 Volts DC Supply             ";
                    case 233: return "Controller #2                 ";
                    case 234: return "Parking Brake On Actuator     ";
                    case 235: return "Parking Brake Off Actuator    ";
                    case 236: return "Power Connect Device          ";
                    case 237: return "Start Enable Device           ";
                    case 238: return "Diagnostic Lamp— Red          ";
                    case 239: return "Diagnostic Light— Amber       ";
                    case 240: return "Program Memory                ";
                    case 242: return "Cruise Control Resume Switch  ";
                    case 243: return "Cruise Control Set Switch     ";
                    case 244: return "Cruise Control Enable Switch  ";
                    case 245: return "Clutch Pedal Switch #1        ";
                    case 246: return "Brake Pedal Switch #1         ";
                    case 247: return "Brake Pedal Switch #2         ";
                    case 248: return "Proprietary Data Link         ";
                    case 249: return "SAE J1922 Data Link           ";
                    case 250: return "SAE J1708 (J1587) Data Link   ";
                    case 251: return "Power Supply                  ";
                    case 252: return "Calibration Module            ";
                    case 253: return "Calibration Memory            ";
                    case 254: return "Controller #1                 ";
                    default: return "UNKNOWN COMPONENT             ";
                }
            }
            else if (source == 0x80)
            {
                switch (sid)
                {
                    case 1: return "Injector Cylinder #1          ";
                    case 2: return "Injector Cylinder #2          ";
                    case 3: return "Injector Cylinder #3          ";
                    case 4: return "Injector Cylinder #4          ";
                    case 5: return "Injector Cylinder #5          ";
                    case 6: return "Injector Cylinder #6          ";
                    case 7: return "Injector Cylinder #7          ";
                    case 8: return "Injector Cylinder #8          ";
                    case 9: return "Injector Cylinder #9          ";
                    case 10: return "Injector Cylinder #10         ";
                    case 11: return "Injector Cylinder #11         ";
                    case 12: return "Injector Cylinder #12         ";
                    case 13: return "Injector Cylinder #13         ";
                    case 14: return "Injector Cylinder #14         ";
                    case 15: return "Injector Cylinder #15         ";
                    case 16: return "Injector Cylinder #16         ";
                    case 17: return "Fuel Shutoff Valve            ";
                    case 18: return "Fuel Control Valve            ";
                    case 19: return "Throttle Bypass Valve         ";
                    case 20: return "Timing Actuator               ";
                    case 21: return "Engine Position Sensor        ";
                    case 22: return "Timing Sensor                 ";
                    case 23: return "Rack Actuator                 ";
                    case 24: return "Rack Position Sensor          ";
                    case 25: return "Ext. Engine Protection Input  ";
                    case 26: return "Aux. Output Device Driver #1  ";
                    case 27: return "Var. Geom. Turbo Actuator #1  ";
                    case 28: return "Var. Geom. Turbo Actuator #2  ";
                    case 29: return "External Fuel Command Input   ";
                    case 30: return "External Speed Command Input  ";
                    case 31: return "Tachometer Signal Output      ";
                    case 32: return "Turbo #1 Wastegate Drive      ";
                    case 33: return "Fan Clutch Output Dev. Driver ";
                    case 34: return "Exhaust Back Pressure Sensor  ";
                    case 35: return "Exhaust Prssr Regltr Solenoid ";
                    case 36: return "Glow Plug Lamp                ";
                    case 37: return "Elec. Drive Unit Power Relay  ";
                    case 38: return "Glow Plug Relay               ";
                    case 39: return "Engine Starter Motor Relay    ";
                    case 40: return "Auxiliary Output Dev Driver #2";
                    case 41: return "ECM 8 Volts DC Supply         ";
                    case 42: return "Injection Cntrl Prsr Regulator";
                    case 43: return "Autoshift High Gear Actuator  ";
                    case 44: return "Autoshift Low Gear Actuator   ";
                    case 45: return "Autoshift Neutral Actuator    ";
                    case 46: return "Autoshift Common Low Side     ";
                    case 47: return "Injector Cylinder #17         ";
                    case 48: return "Injector Cylinder #18         ";
                    case 49: return "Injector Cylinder #19         ";
                    case 50: return "Injector Cylinder #20         ";
                    case 51: return "Auxiliary Output Dev Driver #3";
                    case 52: return "Auxiliary Output Dev Driver #4";
                    case 53: return "Auxiliary Output Dev Driver #5";
                    case 54: return "Auxiliary Output Dev Driver #6";
                    case 55: return "Auxiliary Output Dev Driver #7";
                    case 56: return "Auxiliary Output Dev Driver #8";
                    case 57: return "Auxiliary PWM Driver #1       ";
                    case 58: return "Auxiliary PWM Driver #2       ";
                    case 59: return "Auxiliary PWM Driver #3       ";
                    case 60: return "Auxiliary PWM Driver #4       ";
                    case 61: return "Variable Swirl System Valve   ";
                    case 62: return "Prestroke Sensor              ";
                    case 63: return "Prestroke Actuator            ";
                    case 64: return "Engine Speed Sensor #2        ";
                    case 65: return "Heated Oxygen Sensor          ";
                    case 66: return "Ignition Control Mode Signal  ";
                    case 67: return "Ignition Control Timing Signal";
                    case 68: return "Secondary Turbo Inlet Pressure";
                    case 69: return "After-Cooler Coolant Temp     ";
                    case 70: return "Inlet Air Heater Driver #1    ";
                    case 71: return "Inlet Air Heater Driver #2    ";
                    case 72: return "Injector Cylinder #21         ";
                    case 73: return "Injector Cylinder #22         ";
                    case 74: return "Injector Cylinder #23         ";
                    case 75: return "Injector Cylinder #24         ";
                    case 76: return "Knock Sensor                  ";
                    case 77: return "Gas Metering Valve            ";
                    case 78: return "Fuel Supply Pump Actuator     ";
                    case 79: return "Engine Brake Output #1        ";
                    case 80: return "Engine Brake Output #2        ";
                    case 81: return "Engine (Exhaust) Brake Output ";
                    case 82: return "Engine Brake Output #3        ";
                    case 83: return "Fuel Control Valve #2         ";
                    case 84: return "Timing Actuator #2            ";
                    case 85: return "Engine Oil Burn Valve         ";
                    case 86: return "Engine Oil Replacement Valve  ";
                    case 87: return "Idle Shutdown Relay Driver    ";
                    case 88: return "Turbo #2 Wastegate Drive      ";
                    case 89: return "Air Comp. Actuator Circuit    ";
                    case 90: return "Engine Cylinder #1 Knock Sensr";
                    case 91: return "Engine Cylinder #2 Knock Sensr";
                    case 92: return "Engine Cylinder #3 Knock Sensr";
                    case 93: return "Engine Cylinder #4 Knock Sensr";
                    case 94: return "Engine Cylinder #5 Knock Sensr";
                    case 95: return "Engine Cylinder #6 Knock Sensr";
                    case 96: return "Engine Cylinder #7 Knock Sensr";
                    case 97: return "Engine Cylinder #8 Knock Sensr";
                    case 98: return "Engine Cylinder #9 Knock Sensr";
                    case 99: return "Engine Cylinder #10 Knock Snsr";
                    case 100: return "Engine Cylinder #11 Knock Snsr";
                    case 101: return "Engine Cylinder #12 Knock Snsr";
                    case 102: return "Engine Cylinder #13 Knock Snsr";
                    case 103: return "Engine Cylinder #14 Knock Snsr";
                    case 104: return "Engine Cylinder #15 Knock Snsr";
                    case 105: return "Engine Cylinder #16 Knock Snsr";
                    case 106: return "Engine Cylinder #17 Knock Snsr";
                    case 107: return "Engine Cylinder #18 Knock Snsr";
                    case 108: return "Engine Cylinder #19 Knock Snsr";
                    case 109: return "Engine Cylinder #20 Knock Snsr";
                    case 110: return "Engine Cylinder #21 Knock Snsr";
                    case 111: return "Engine Cylinder #22 Knock Snsr";
                    case 112: return "Engine Cylinder #23 Knock Snsr";
                    case 113: return "Engine Cylinder #24 Knock Snsr";
                    case 114: return "Multiple Unit Synchro Switch  ";
                    case 115: return "Engine Oil Change Interval    ";
                    case 116: return "Engine was Shut Down Hot      ";
                    case 117: return "Engine Shut Down by Data Link ";
                    case 129: return "Exhaust Temp  #1 Sensor Volts ";
                    case 130: return "Exhaust Temp  #2 Sensor Volts ";
                    case 131: return "Exhaust Temp  #3 Sensor Volts ";
                    case 132: return "Exhaust Temp  #4 Sensor Volts ";
                    case 133: return "Exhaust Temp  #5 Sensor Volts ";
                    case 134: return "Exhaust Temp  #6 Sensor Volts ";
                    case 135: return "Exhaust Temp  #7 Sensor Volts ";
                    case 136: return "Exhaust Temp  #8 Sensor Volts ";
                    case 137: return "Exhaust Temp  #9 Sensor Volts ";
                    case 138: return "Exhaust Temp #10 Sensor Volts ";
                    case 139: return "Exhaust Temp #11 Sensor Volts ";
                    case 140: return "Exhaust Temp #12 Sensor Volts ";
                    case 141: return "Exhaust Temp #13 Sensor Volts ";
                    case 142: return "Exhaust Temp #14 Sensor Volts ";
                    case 143: return "Exhaust Temp #15 Sensor Volts ";
                    case 144: return "Exhaust Temp #16 Sensor Volts ";
                    case 146: return "EGR Valve                     ";
                    case 147: return "VNT Vanes / EGR               ";
                    case 214: return "RTC Backup Battery Voltage    ";
                    case 216: return "Other ECU Fault               ";
                    case 226: return "Transmission Neutral Switch   ";
                    case 227: return "Aux Analog Input              ";
                    case 230: return "TPS Idle Validation Circuit   ";
                    case 231: return "J1939 Data Link               ";
                    case 232: return "Sensor Supply Voltage         ";
                    case 238: return "Stop Engine Light             ";
                    case 239: return "Check Engine Light            ";
                    case 248: return "Proprietary Data Link         ";
                    case 249: return "J1922 Data Link               ";
                    case 250: return "J1587 Data Link               ";
                    case 253: return "EEPROM Error                  ";
                    case 254: return "A/D Converter                 ";
                    case 277: return "EGR Mass Flow Smart Sensor    ";
                    default: return "UNKNOWN COMPONENT             ";
                }
            }
            else if (source == 0x82)
            {
                switch (sid)
                {
                    case 1: return "C1 Solenoid Valve             ";
                    case 2: return "C2 Solenoid Valve             ";
                    case 3: return "C3 Solenoid Valve             ";
                    case 4: return "C4 Solenoid Valve             ";
                    case 5: return "C5 Solenoid Valve             ";
                    case 6: return "C6 Solenoid Valve             ";
                    case 7: return "Lockup Solenoid Valve         ";
                    case 8: return "Forward Solenoid Valve        ";
                    case 9: return "Low Signal Solenoid Valve     ";
                    case 10: return "Retarder Enable Solenoid Valve";
                    case 11: return "Retarder Modulation Sol. Valve";
                    case 12: return "Retarder Response Solenoid Vlv";
                    case 13: return "Differential Lock Solenoid Vlv";
                    case 14: return "Engine/Transmission Match     ";
                    case 15: return "Retarder Modulation Rqst Sensr";
                    case 16: return "Neutral Start Output          ";
                    case 17: return "Turbine Speed Sensor          ";
                    case 18: return "Primary Shift Selector        ";
                    case 19: return "Secondary Shift Selector      ";
                    case 20: return "Special Function Inputs       ";
                    case 21: return "C1 Clutch Pressure Indicator  ";
                    case 22: return "C2 Clutch Pressure Indicator  ";
                    case 23: return "C3 Clutch Pressure Indicator  ";
                    case 24: return "C4 Clutch Pressure Indicator  ";
                    case 25: return "C5 Clutch Pressure Indicator  ";
                    case 26: return "C6 Clutch Pressure Indicator  ";
                    case 27: return "Lockup Clutch Prssr Indicator ";
                    case 28: return "Forward Range Prssr Indicator ";
                    case 29: return "Neutral Range Prssr Indicator ";
                    case 30: return "Reverse Range Prssr Indicator ";
                    case 31: return "Retarder Response Prsr Indctr ";
                    case 32: return "Diff. Lock Clutch Prsr Indctr ";
                    case 33: return "Multiple Pressure Indicators  ";
                    case 34: return "Reverse Switch                ";
                    case 35: return "Range High Actuator           ";
                    case 36: return "Range Low Actuator            ";
                    case 37: return "Splitter Direct Actuator      ";
                    case 38: return "Splitter Indirect Actuator    ";
                    case 39: return "Shift Finger Rail Actuator 1  ";
                    case 40: return "Shift Finger Gear Actuator 1  ";
                    case 41: return "Upshift Request Switch        ";
                    case 42: return "Downshift Request Switch      ";
                    case 43: return "Torque Cnvrtr Intrrpt Actuator";
                    case 44: return "Torque Cnvrtr Lockup Actuator ";
                    case 45: return "Range High Indicator          ";
                    case 46: return "Range Low Indicator           ";
                    case 47: return "Shift Finger Neutral Indicator";
                    case 48: return "Shift Finger Engagement Indctr";
                    case 49: return "Shift Finger Center Rail Indct";
                    case 50: return "Shift Finger Rail Actuator 2  ";
                    case 51: return "Shift Finger Gear Actuator 2  ";
                    case 52: return "Hydraulic System              ";
                    case 53: return "Defuel Actuator               ";
                    case 54: return "Inertia Brake Actuator        ";
                    case 55: return "Clutch Actuator               ";
                    case 56: return "Auxiliary Range Mech. System  ";
                    case 57: return "Shift Console Data Link       ";
                    case 58: return "Main Box Shift Engagement Sys.";
                    case 59: return "Main Box Rail Selection System";
                    case 60: return "Main Box Shift Neutralztn Sys.";
                    case 61: return "Aux Splitter Mechanical System";
                    default: return "UNKNOWN COMPONENT             ";
                }
            }
            else if ((source == 0x88) || (source == 0x89) || (source == 0x8A) || (source == 0x8B))
            {
                switch (sid)
                {
                    case 1: return "Wheel Sensor ABS Axle 1 Left  ";
                    case 2: return "Wheel Sensor ABS Axle 1 Right ";
                    case 3: return "Wheel Sensor ABS Axle 2 Left  ";
                    case 4: return "Wheel Sensor ABS Axle 2 Right ";
                    case 5: return "Wheel Sensor ABS Axle 3 Left  ";
                    case 6: return "Wheel Sensor ABS Axle 3 Right ";
                    case 7: return "Prssr Valve ABS Axle 1 Left   ";
                    case 8: return "Prssr Valve ABS Axle 1 Right  ";
                    case 9: return "Prssr Valve ABS Axle 2 Left   ";
                    case 10: return "Prssr Valve ABS Axle 2 Right  ";
                    case 11: return "Prssr Valve ABS Axle 3 Left   ";
                    case 12: return "Prssr Valve ABS Axle 3 Right  ";
                    case 13: return "Retarder Control Relay        ";
                    case 14: return "Relay Diagonalcase  1              ";
                    case 15: return "Relay Diagonal 2              ";
                    case 16: return "Mode Switch ABS               ";
                    case 17: return "Mode Switch ASR               ";
                    case 18: return "DIFcase  1— ASR Valve              ";
                    case 19: return "DIF 2— ASR Valve              ";
                    case 20: return "Pneumatic Engine Control      ";
                    case 21: return "Engine Servomotor Control     ";
                    case 22: return "Speed Signal Input            ";
                    case 23: return "Warning Light Bulb            ";
                    case 24: return "ASR Light Bulb                ";
                    case 25: return "Wheel Sensor, ABS Axle 1 Ave. ";
                    case 26: return "Wheel Sensor, ABS Axlecase  2 Ave. ";
                    case 27: return "Wheel Sensor, ABS Axle 3 Ave. ";
                    case 28: return "Prs Mdltr Drive Axle Relay Vlv";
                    case 29: return "Prs Trnsducer Drive Axle Relay";
                    case 30: return "Master Control Relay          ";
                    case 31: return "Trlr Brake Slack Frwrd Axle Lt";
                    case 32: return "Trlr Brake Slack Frwrd Axle Rt";
                    case 33: return "Trlr Brake Slack Rear Axle Lft";
                    case 34: return "Trlr Brake Slack Rear Axle Rt ";
                    case 35: return "Trtr Brake Slack Axle 1 Left  ";
                    case 36: return "Trlr Brake Slack Axle 1 Right ";
                    case 37: return "Trlr Brake Slack Axle 2 Left  ";
                    case 38: return "Trlr Brake Slack Axle 2 Right ";
                    case 39: return "Trlr Brake Slack Axlecase  3 Left  ";
                    case 40: return "Trlr Brake Slack Axle 3 Right ";
                    case 41: return "Ride Height Relay             ";
                    case 42: return "Hold Mdltr Valve Solnd Ax#1 Lt";
                    case 43: return "Hold Mdltr Valve Solnd Ax#1 Rt";
                    case 44: return "Hold Mdltr Valve Solnd Ax#2 Lt";
                    case 45: return "Hold Mdltr Valve Solnd Ax#2 Rt";
                    case 46: return "Hold Mdltr Valve Solnd Ax#3 Lt";
                    case 47: return "Hold Mdltr Valve Solnd Ax#3 Rt";
                    case 48: return "Dump Mdltr Valve Solnd Ax#1 Lt";
                    case 49: return "Hold Mdltr Valve Solnd Ax#1 Rt";
                    case 50: return "Hold Mdltr Valve Solnd Ax#2 Lt";
                    case 51: return "Hold Mdltr Valve Solnd Ax#2 Rt";
                    case 52: return "Hold Mdltr Valve Solnd Ax#3 Lt";
                    case 53: return "Hold Mdltr Valve Solnd Ax#3 Rt";
                    case 54: return "Hydraulic Pump Motor          ";
                    case 55: return "Brake Light Switch 1          ";
                    case 56: return "Brake Light Switch 2          ";
                    case 57: return "Elect. Pressure Control Axle 1";
                    case 58: return "Pneum. Pressure Control Axle 1";
                    case 59: return "Brake Pressure Sensing, Axle 1";
                    case 60: return "Elect. Pressure Control Axle 2";
                    case 61: return "Pneum. Pressure Control Axle 2";
                    case 62: return "Brake Pressure Sensing, Axle 2";
                    case 63: return "Elect. Pressure Control Axle 3";
                    case 64: return "Pneum. Back-up Pressure Axle 3";
                    case 65: return "Brake Pressure Sensing, Axle 3";
                    case 66: return "Elect. Prssr Control - Trailer";
                    case 67: return "Pneum. Prssr Control - Trailer";
                    case 68: return "Brake Prssr Sensing - Trailer ";
                    case 69: return "Axle Load Sensor              ";
                    case 70: return "Lining Wear Sensor, Axle 1 Lt ";
                    case 71: return "Lining Wear Sensor, Axle 1 Rt ";
                    case 72: return "Lining Wear Sensor, Axle 2 Lt ";
                    case 73: return "Lining Wear Sensor, Axle 2 Rt ";
                    case 74: return "Lining Wear Sensor, Axle 3 Lt ";
                    case 75: return "Lining Wear Sensor, Axle 3 Rt ";
                    case 76: return "Brake Signal Transmitter      ";
                    case 77: return "Brake Signal Sensor 1         ";
                    case 78: return "Brake Signal Sensor 2         ";
                    case 79: return "Tire Dimension Supervision    ";
                    case 80: return "Vehicle Deceleration Control  ";
                    default: return "UNKNOWN COMPONENT             ";
                }
            }
            else
            {
                return "UNKNOWN COMPONENT             ";
            }
        }

        public static string GetModeString(byte mode)
        {
            switch (mode)
            {
                case 0: return "High Reading    ";
                case 1: return "Low Reading     ";
                case 2: return "Invalid Data    ";
                case 3: return "High Voltage    ";
                case 4: return "Low Voltage     ";
                case 5: return "Low Current     ";
                case 6: return "High Current    ";
                case 7: return "Not Responding  ";
                case 8: return "Frequency Error ";
                case 9: return "Abnormal Update ";
                case 10: return "Abnormal Change ";
                case 11: return "Unknown Failure ";
                case 12: return "Bad Program     ";
                case 13: return "Calibratn Error ";
                case 14: return "Special Error   ";
                case 15: return "High Reading    ";
                case 16: return "High Reading    ";
                case 17: return "Low Reading     ";
                case 18: return "Low Reading     ";
                case 19: return "Bad Network Data";
                default: return "UNKNOWN MODE    ";
            }
        }

        public uint GetSPN(byte[] message)
        {
            uint spn = message[0];
            spn <<= 8;
            spn += message[1];
            spn <<= 8;
            spn += message[2];
            if ((spn == 0xFFFFFF) || (spn < 0x000020))
                return 0xFFFFFF;
            spn >>= 5;
            return spn;
        }

        public uint GetLittleEndianSPN(byte[] message)
        {
            uint spn_le = message[2] & 0xFFFFFFE0;
            spn_le <<= 3;
            spn_le += message[1];
            spn_le <<= 8;
            spn_le += message[0];
            return spn_le;
        }

        public byte GetFMI(byte[] message)
        {
            return (byte)(message[2] & 0x1F);
        }

        public void AddRecord(DiagnosticRecord record)
        {
            numDiagCodes = (numDiagCodes + 1) % 20;
            DiagnosticRecords[numDiagCodes] = record;
            OnRaiseCustomEvent(new DiagnosticEventArgs(record));
        }

        public bool IsNewRecordSpn(uint spn)
        {
            for (int i = 0; i < numDiagCodes; i++)
            {
                if ((DiagnosticRecords[i] != null) && (spn == DiagnosticRecords[i].spn))
                    return false;
            }
            return true;
        }

        public void Parse1939Diagnostics(byte [] message)
        {
            uint spn = GetSPN(message);
            uint spn_le = GetLittleEndianSPN(message);
            byte fmi = GetFMI(message);
            if (spn == 0xFFFFFFFF)
            {
                //p.diag_active_flag = 0;
                ChassisParameter.ChassisParam.diagActiveFlag = false;
                return;
            }
            if (spn_le < spn)
            {
                spn = spn_le;
            }

            ChassisParameter.ChassisParam.diagActiveFlag = true;
            if (IsNewRecordSpn(spn))
            {
                J1939Record record = new J1939Record();
                record.dataBusType = DataBusType.J1939;
                AddRecord(record);
            }
        }

        public void Parse1708Diagnostics(byte [] message)
        {
            uint count = numDiagCodes;
            byte length = message[2];
            byte mid = message[0];
            if (mid != 0x80)
                return;
            ChassisParameter.ChassisParam.diagActiveFlag = true;
            for (int i = 1; i < length; i+=2)
            {
                byte id = message[i];
                byte ecode = message[i + 1];
                for (int j = 0; j < numDiagCodes; j++)
                {
                    if ((DiagnosticRecords[j] != null) && (id == DiagnosticRecords[j].spn))
                        continue;
                }
                if (IsNewRecordSpn(id))
                {
                    if ((ecode & 0x40) == 0)
                    {
                        J1708Record record = new J1708Record();
                        record.dataBusType = DataBusType.J1708;
                        record.count = 0xFF;
                        record.mid = mid;
                        record.fmi = (byte)(ecode & 0x0F);
                        record.spn = id;
                        AddRecord(record);
                    }
                }
            }
        }
    }
}
