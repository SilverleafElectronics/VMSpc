using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Common
{
    public static class PGN
    {
        public static readonly uint RPMS = 0x0F0004;
        public static readonly uint ACCEL_LOADPCT = 0x0F003;
        public static readonly uint RETARDER_PCT = 0x0F000;
        public static readonly uint ABS_ACTIVE_STATUS = 0x0F001;
        public static readonly uint CRUISESPEED_ROADSPEED = 0x0FEF1;
        public static readonly uint FUELRATE_INSTANTMPG_RECIPROCALMPG = 0x0FEF2;
        public static readonly uint BAROPRESSURE_AMBIENTTEMP_INLETTEMP = 0x0FEF5;
        public static readonly uint FUEL_LEVEL = 0x0FEFC;
        public static readonly uint LIQUID_TEMPS = 0x0FEEE;
        public static readonly uint PRESSURES_TEMPS = 0x0FEF6;
        public static readonly uint BATTERY = 0x0FEF7;
        public static readonly uint PRESSURES_LEVELS = 0x0FEEF;
        public static readonly uint ODOMETER = 0x0FEC1;
        public static readonly uint ENGINE_HOURS = 0x0FEE5;
        public static readonly uint TOTAL_FUEL = 0x0FEE9;
        public static readonly uint IDLE_PARAMETERS = 0xFEDC;
        public static readonly uint INJECTORS = 0xFEDB;
        public static readonly uint DPF_OUTLET_TEMP = 0xFDB3;
        public static readonly uint DPFINTAKETEMP_EXHAUSTTEMP = 0xFDB4;
        //public static readonly uint 
    }
}
