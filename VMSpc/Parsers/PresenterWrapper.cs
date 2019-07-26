using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Constants;
using static VMSpc.Parsers.SPNDefinitions;

namespace VMSpc.Parsers
{
    public static class PresenterWrapper
    {
        static public void SetValueSPN(ushort pid, uint raw, double v_metric, double v_standard, byte src)
        {
            if (!PresenterList.ContainsKey(pid))
                return;
            TSPNPresenter temp = PresenterList[pid];
            if (src == J1708)
                temp.datum.prioritize1708 = true;
            temp.datum.SetValue(raw, v_standard, v_metric);
        }

        static public uint GetRawValueSPN(ushort pid)
        {
            return PresenterList[pid].datum.rawValue;
        }

        static public double GetStandardValueSPN(ushort pid)
        {
            return PresenterList[pid].datum.value;
        }

        static public double GetMetricValueSPN(ushort pid)
        {
            return PresenterList[pid].datum.valueMetric;
        }

        static public bool Exists(ushort pid)
        {
            return PresenterList.ContainsKey(pid);
        }

        static public bool Seen(ushort pid)
        {
            return PresenterList[pid].datum.seen;
        }

        public static readonly Dictionary<ushort, TSPNPresenter> PresenterList = new Dictionary<ushort, TSPNPresenter>();
        static PresenterWrapper() { }

        public static void InitializePresenterList()
        {
            PresenterList.Add(91, new TSPNPresenterFloat(spn_accelPos, "ACCEL-POS", 91, "pct", "pct", 0, false));
            PresenterList.Add(171, new TSPNPresenterFloat(spn_ambientTemp, "AMB.-TEMP", 171, "degF", "degC", 0, false));
            PresenterList.Add(108, new TSPNPresenterFloat(spn_baroPressure, "BAROMETER", 108, "inHg", "kPa", 2, false));
            PresenterList.Add(522, new TSPNPresenterFloat(spn_clutchSlipPercent, "CLTCH-SLIP", 522, "pct", "pct", 0, false));
            PresenterList.Add(110, new TSPNPresenterFloat(spn_coolantTemp, "COOLANT", 110, "degF", "degC", 0, false));
            PresenterList.Add(111, new TSPNPresenterFloat(spn_coolantLevel, "COOL-LEVEL", 111, "pct", "pct", 0, false));
            PresenterList.Add(109, new TSPNPresenterFloat(spn_coolantPressure, "COOL-PRSSR", 109, "psi", "kPa", 0, false));
            PresenterList.Add(101, new TSPNPresenterFloat(spn_crankCasePressure, "CRANKCASE", 101, "psi", "kPa", 2, false));
            PresenterList.Add(86, new TSPNPresenterFloat(spn_cruiseSetSpeed, "CRUISE", 86, "mph", "kph", 0, true));
            PresenterList.Add(1761, new TSPNPresenterFloat(spn_defTankLevel, "DEF-LEVEL", 1761, "pct", "pct", 0, false));
            PresenterList.Add(3031, new TSPNPresenterFloat(spn_defTankTemp, "DEF-TEMP", 3031, "degF", "degC", 0, false));
            PresenterList.Add(3242, new TSPNPresenterFloat(spn_dpfIntakeTemp, "DPF-INTAKE", 3242, "degF", "degC", 0, false));
            PresenterList.Add(3246, new TSPNPresenterFloat(spn_dpfOutletTemp, "DPF-OUTLET", 3246, "degF", "degC", 0, false));
            PresenterList.Add(1136, new TSPNPresenterFloat(spn_ecuTemp, "ECU-TEMP", 1136, "degF", "degC", 0, false));
            PresenterList.Add(41, new TSPNPresenterFloat(spn_egrDiffPressure, "EGR-DFRNTL", 411, "psi", "kPa", 0, false));
            PresenterList.Add(412, new TSPNPresenterFloat(spn_egrTemp, "EGR-TEMP", 412, "degF", "degC", 0, false));
            PresenterList.Add(173, new TSPNPresenterFloat(spn_exhaustTemp173, "EXHAUST", 173, "degF", "degC", 0, false));
            PresenterList.Add(3241, new TSPNPresenterFloat(spn_exhaustTemp3241, "EXHAUST-1", 3241, "degF", "degC", 0, false));
            PresenterList.Add(986, new TSPNPresenterFloat(spn_fanSpeed, "FAN-SPEED", 986, "pct", "pct", 0, false));
            PresenterList.Add(107, new TSPNPresenterFloat(spn_airFilterDiffPressure, "FILTER-DIF", 107, "psi", "kPa", 2, false));
            PresenterList.Add(96, new TSPNPresenterFloat(spn_fuelLevel, "FUEL-LEVEL", 96, "pct", "pct", 0, false));
            PresenterList.Add(94, new TSPNPresenterFloat(spn_fuelPressure, "FUEL-PRSR", 94, "psi", "kPa", 0, false));
            PresenterList.Add(183, new TSPNPresenterFloat(spn_fuelRate, "FUEL-RATE", 183, "gph", "lph", 1, true));
            PresenterList.Add(174, new TSPNPresenterFloat(spn_fuelTemp, "FUEL-TEMP", 174, "degF", "degC", 0, false));
            PresenterList.Add(250, new TSPNPresenterLongFloat(spn_fuel, "FUELMETER", 250, 1, true));
            PresenterList.Add(509, new TSPNPresenterFloat(spn_horsepower, "HORSEPOWER", 509, "hp", "kw", 0, false));
            PresenterList.Add(247, new TSPNPresenterLongFloat(spn_hours, "HOURMETER", 247, 1, false));
            PresenterList.Add(236, new TSPNPresenterLongFloat(spn_idleFuel, "IDLE-FUEL", 236, 1, true));
            PresenterList.Add(235, new TSPNPresenterLongFloat(spn_idleHours, "IDLE-HOURS", 235, 1, false));
            PresenterList.Add(157, new TSPNPresenterFloat(spn_injectorRailPressure, "INJ.-PRSR", 157, "kPSI", "MPa", 0, true));
            PresenterList.Add(106, new TSPNPresenterFloat(spn_airInletPressure, "INLET-PRSR", 106, "psi", "kPa", 1, false));
            PresenterList.Add(172, new TSPNPresenterFloat(spn_inletTemp, "INLET-TEMP", 172, "degF", "degC", 0, false));
            PresenterList.Add(184, new TSPNPresenterFloat(spn_instantMPG, "INST.-MPG", 184, "mpg", "kpl", 1, true));
            PresenterList.Add(105, new TSPNPresenterFloat(spn_intakeManifoldTemp, "INTAKE", 105, "degF", "degC", 0, false));
            PresenterList.Add(52, new TSPNPresenterFloat(spn_interCoolerTemp, "INTRCOOLER", 52, "degF", "degC", 0, false));
            PresenterList.Add(92, new TSPNPresenterFloat(spn_loadPercent, "LOAD", 92, "pct", "pct", 0, false));
            PresenterList.Add(245, new TSPNPresenterLongFloat(spn_odometer, "ODOMETER", 245, 1, true));
            PresenterList.Add(244, new TSPNPresenterLongFloat(spn_odometerALT, "ODOMETER", 244, 1, true));
            PresenterList.Add(98, new TSPNPresenterFloat(spn_oilLevel, "OIL-LEVEL", 98, "pct", "pct", 0, false));
            PresenterList.Add(100, new TSPNPresenterFloat(spn_oilPressure, "OIL-PRESS", 100, "psi", "kPa", 0, false));
            PresenterList.Add(175, new TSPNPresenterFloat(spn_oilTemp, "OIL-TEMP", 175, "degF", "degC", 0, false));
            PresenterList.Add(9, new TSPNPresenterFloat(spn_rollingMPG, "REAL-MPG", 9, "mpg", "kpl", 1, true));
            PresenterList.Add(502, new TSPNPresenterFloat(spn_recentMPG, "RECENT-MPG", 502, "mpg", "kpl", 1, true));
            PresenterList.Add(520, new TSPNPresenterFloat(spn_retarderPct, "RETARDER", 520, "pct", "pct", 0, false));
            PresenterList.Add(84, new TSPNPresenterFloat(spn_roadSpeed, "SPEED", 84, "mph", "kph", 0, true));
            PresenterList.Add(190, new TSPNPresenterFloat(spn_rpms, "TACHOMETER", 190, "rpms", "rpms", 0, false));
            PresenterList.Add(510, new TSPNPresenterFloat(spn_torque, "TORQUE", 510, "ftlb", "nm", 0, false));
            //PresenterList.Add(92, new TSPNPresenterFloat(spn_torquePercent, "TORQUE-PCT", 92, "pct", "pct", 0, false));   //TODO - figure out how to incorporate this...
            PresenterList.Add(161, new TSPNPresenterFloat(spn_inputShaftSpeed, "TRANS-IN", 161, "rpms", "rpms", 0, false));
            PresenterList.Add(191, new TSPNPresenterFloat(spn_outputShaftSpeed, "TRANS-OUT", 191, "rpms", "rpms", 0, false));
            PresenterList.Add(177, new TSPNPresenterFloat(spn_transTemp, "TRANS-TEMP", 177, "degF", "degC", 0, false));
            PresenterList.Add(102, new TSPNPresenterFloat(spn_turboBoostPressure, "TURBO", 102, "psi", "kPa", 1, false));
            PresenterList.Add(103, new TSPNPresenterLongFloat(spn_turboSpeed, "TURBO-RPMS", 103, 0, false));
            PresenterList.Add(168, new TSPNPresenterFloat(spn_batteryVolts, "VOLTAGE", 168, "V dc", "V dc", 1, false));
            PresenterList.Add(603, new TSPNPresenterFloat(spn_reciprocalMPG, "INST.-L/100km", 603, "G/100M", "L/100KM", 1, true));
            PresenterList.Add(601, new TSPNPresenterFloat(spn_rollingLPK, "ROLLING L/100km", 601, "G/100M", "L/100KM", 1, true));
            PresenterList.Add(602, new TSPNPresenterFloat(spn_recentLPK, "RECENT L/100km", 602, "G/100M", "L/100KM", 1, true));
            PresenterList.Add(10, new TSPNPresenterFloat(spn_acceleration, "Acceleration", 10, "Ft/s^2", "M/s^2", 0, false));
            PresenterList.Add(11, new TSPNPresenterFloat(spn_braking, "Peak Braking", 11, "Ft/s^2", "M/s^2", 0, false));
            PresenterList.Add(12, new TSPNPresenterFloat(spn_peakAcceleration, "Peak Acceleration", 12, "Ft/s^2", "M/s^2", 0, false));
            PresenterList.Add(13, new TSPNPresenterFloat(spn_transMode, "Transmission Mode", 13, "", "", 0, false));
            PresenterList.Add(47, new TSPNPresenterFloat(spn_1708_RtdrSwtich, "Retarder Switch", 47, "", "", 0, false));
            PresenterList.Add(121, new TSPNPresenterFloat(spn_1708_RtdrStatus, "Retarder Status", 121, "", "", 0, false));
            PresenterList.Add(119, new TSPNPresenterFloat(spn_1708_RtdrPressure, "Retarder Oil Pressure", 119, "psi", "kPa", 0, false));
            PresenterList.Add(120, new TSPNPresenterFloat(spn_1708_RtdrOilTemp, "Retarder Oil Temp", 120, "degF", "degC", 0, false));
            PresenterList.Add(503, new TSPNPresenterFloat(spn_MaxCoolant, "", 503, "", "", 0, false));
            PresenterList.Add(504, new TSPNPresenterFloat(spn_MaxTransmission, "", 504, "", "", 0, false));
            PresenterList.Add(505, new TSPNPresenterFloat(spn_MaxOil, "", 505, "", "", 0, false));
            PresenterList.Add(506, new TSPNPresenterFloat(spn_MaxManifoldTemp, "", 506, "", "", 0, false));
            PresenterList.Add(507, new TSPNPresenterFloat(spn_MaxRPMs, "", 507, "", "", 0, false));
            PresenterList.Add(508, new TSPNPresenterFloat(spn_MaxSpeed, "", 508, "", "", 0, false));
        }
    }
}
