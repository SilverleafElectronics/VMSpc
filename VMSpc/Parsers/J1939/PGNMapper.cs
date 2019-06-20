using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.Parsers.SPNDefinitions;

namespace VMSpc.Parsers
{
    /// <summary>
    /// Contains the subsriptions of each TSPNDatum object to the PGN which carries the datum's data. Access the list of associated TSPNDatum objects with PGNMap[`pgn`]
    /// </summary>
    static class PGNMapper
    {
        public static Dictionary<uint, TSPNDatum[]> PGNMap;
        static PGNMapper()
        {
            PGNMap = new Dictionary<uint, TSPNDatum[]>
            {
                { 0x0F005, new TSPNDatum[] { spn_range, spn_transMode } },
                { 0x0F004, new TSPNDatum[] { /*spn_torquePercent,*/ spn_rpms } },
                { 0x0F003, new TSPNDatum[] { spn_accelPos, spn_loadPercent }  },
                { 0x0F000, new TSPNDatum[] { spn_retarderPct } },
                { 0x0F001, new TSPNDatum[] { spn_absActive } },
                { 0x0FEF1, new TSPNDatum[] { spn_cruiseSetSpeed, spn_roadSpeed } },
                { 0x0FEF2, new TSPNDatum[] { spn_fuelRate, spn_instantMPG, spn_reciprocalMPG } },
                { 0x0FEF5, new TSPNDatum[] { spn_baroPressure, spn_ambientTemp, spn_inletTemp } },
                { 0x0FEFC, new TSPNDatum[] { spn_fuelLevel } },
                { 0x0FEEE, new TSPNDatum[] { spn_coolantTemp, spn_fuelTemp, spn_oilTemp, spn_interCoolerTemp } },
                { 0x0FEF6, new TSPNDatum[] { spn_turboBoostPressure, spn_intakeManifoldTemp, spn_airInletPressure, spn_airFilterDiffPressure, spn_exhaustTemp173 } },
                { 0x0FEF7, new TSPNDatum[] { spn_batteryVolts } },
                { 0x0FEEF, new TSPNDatum[] { spn_fuelPressure, spn_oilLevel, spn_oilPressure, spn_crankCasePressure, spn_coolantPressure, spn_coolantLevel } },
                { 0x0FEC1, new TSPNDatum[] { spn_odometer } },
                { 0x0FEE9, new TSPNDatum[] { spn_fuel } },
                { 0xFEDC,  new TSPNDatum[] { spn_idleHours, spn_idleFuel } },
                { 0xFEDB,  new TSPNDatum[] { spn_injectorRailPressure } },
                { 0xFDB3,  new TSPNDatum[] { spn_dpfOutletTemp } },
                { 0xFDB4,  new TSPNDatum[] { spn_dpfIntakeTemp, spn_exhaustTemp3241 } },
                { 0xFEE4,  new TSPNDatum[] { spn_waitToStart, spn_engineShutdownApproaching, spn_engineShutdown } },
                { 0xFEBD,  new TSPNDatum[] { spn_fanSpeed, spn_fanState } },
                { 0xFEA4,  new TSPNDatum[] { spn_ecuTemp, spn_egrDiffPressure, spn_egrTemp } },
                { 0xFEDD,  new TSPNDatum[] { spn_turboSpeed } },
                { 0xFEFF,  new TSPNDatum[] { spn_waterInFuel } },
                { 0x0F002, new TSPNDatum[] { spn_outputShaftSpeed, spn_clutchSlipPercent, spn_inputShaftSpeed } },
                { 0x0FEF8, new TSPNDatum[] { spn_transTemp } },
                { 0xFECA,  new TSPNDatum[] { spn_diagnostic1939 } },
                { 0xFD7C,  new TSPNDatum[] { spn_dpfLamp, spn_dpfActiveStatus, spn_dpfInhibitClutch, spn_dpfInhibitSwitch, spn_dpfInhibitStatus, spn_dpfInhibitSpeed, spn_dpfInhibitOffidle,
                                             spn_dpfInhibitPTO, spn_dpfInhibitParkbrake, spn_dpfHighTempLamp } },
                { 0x0FEFB, null },
                { 0x0FEDF, null },

                /*
                 * TODO - These all fell under the updatelong() method in 4.1 and previous versions, which was never called.
                 *        Find whether or not they actually need to be implemented
                { 0x0FE4A, new TSPNDatum[] { spn_chassisIdleHours } },
                { 0xFEEB,  new TSPNDatum[] { spn_chassisComponentID } },
                { 0xFEEC,  new TSPNDatum[] { spn_chassisVehicleID } },
                { 0xFEDA,  new TSPNDatum[] { spn_chassisSoftwareID } },

                */
            };
        }
    }
}
