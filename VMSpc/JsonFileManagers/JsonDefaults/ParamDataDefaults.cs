using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.JsonFileManagers.JsonDefaults
{
    public static class ParamDataDefaults
    {
        public static string defaults =
"{                                                                       " +
"  \"Parameters\": [                                                     " +
/*"    {                                                                   " +
"      \"ParamName\": \"Turbo Boost Pressure - Extended\",               " +
"      \"Abbreviation\": \"TURBO\",                                      " +
"      \"Unit\": \"PSI\",                                                " +
"      \"MetricUnit\": \"kPa\",                                          " +
"      \"PID\": \"439\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"60\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"60\",                                           " +
"      \"HighRed\": \"60\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " + TODO - uncomment when implemented
"    },                                                                  " +*/
"    {                                                                   " +
"      \"ParamName\": \"Rolling MPG\",                                   " +
"      \"Abbreviation\": \"RMPG\",                                       " +
"      \"Unit\": \"MPG\",                                                " +
"      \"MetricUnit\": \"KPL\",                                          " +
"      \"PID\": \"9\",                                                   " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"20\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"20\",                                           " +
"      \"HighRed\": \"20\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Road Speed\",                                    " +
"      \"Abbreviation\": \"SPEED\",                                      " +
"      \"Unit\": \"MPH\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"84\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"90\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"55\",                                           " +
"      \"HighRed\": \"75\",                                              " +
"      \"Format\": \"{0:0.##}\",                                         " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Average Speed\",                                 " +
"      \"Abbreviation\": \"SPEED\",                                      " +
"      \"Unit\": \"MPH\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"512\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"90\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"55\",                                           " +
"      \"HighRed\": \"75\",                                              " +
"      \"Format\": \"{0:0.##}\",                                         " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Torque\",                                        " +
"      \"Abbreviation\": \"TORQ\",                                       " +
"      \"Unit\": \"FTLB\",                                               " +
"      \"MetricUnit\": \"NM\",                                           " +
"      \"PID\": \"510\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"2200\",                                           " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"2200\",                                         " +
"      \"HighRed\": \"2200\",                                            " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Horse Power\",                                   " +
"      \"Abbreviation\": \"HP\",                                         " +
"      \"Unit\": \"HP\",                                                 " +
"      \"MetricUnit\": \"KW\",                                           " +
"      \"PID\": \"509\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"750\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"700\",                                          " +
"      \"HighRed\": \"750\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Max Speed\",                                     " +
"      \"Abbreviation\": \"MxSPD\",                                      " +
"      \"Unit\": \"MPH\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"508\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"90\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"55\",                                           " +
"      \"HighRed\": \"75\",                                              " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Max RPMs\",                                      " +
"      \"Abbreviation\": \"MxRPM\",                                      " +
"      \"Unit\": \"RPM\",                                                " +
"      \"MetricUnit\": \"RPM\",                                          " +
"      \"PID\": \"507\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"3000\",                                           " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"2400\",                                         " +
"      \"HighRed\": \"2600\",                                            " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Max Oil Temp\",                                  " +
"      \"Abbreviation\": \"MxOIL\",                                      " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"505\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"300\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"250\",                                          " +
"      \"HighRed\": \"300\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Max Trans Temp\",                                " +
"      \"Abbreviation\": \"MxTRN\",                                      " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"504\",                                                 " +
"      \"GaugeMin\": \"80\",                                             " +
"      \"GaugeMax\": \"300\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"250\",                                          " +
"      \"HighRed\": \"280\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Max Coolant Temp\",                              " +
"      \"Abbreviation\": \"MxTMP\",                                      " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"503\",                                                 " +
"      \"GaugeMin\": \"140\",                                            " +
"      \"GaugeMax\": \"260\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"215\",                                          " +
"      \"HighRed\": \"225\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Recent MPG\",                                    " +
"      \"Abbreviation\": \"RMPG\",                                       " +
"      \"Unit\": \"MPG\",                                                " +
"      \"MetricUnit\": \"KPL\",                                          " +
"      \"PID\": \"502\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"20\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"20\",                                           " +
"      \"HighRed\": \"20\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Average MPG\",                                   " +
"      \"Abbreviation\": \"MPG\",                                        " +
"      \"Unit\": \"MPG\",                                                " +
"      \"MetricUnit\": \"KPL\",                                          " +
"      \"PID\": \"511\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"20\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"20\",                                           " +
"      \"HighRed\": \"20\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Total Fuel\",                                    " +
"      \"Abbreviation\": \"FUEL\",                                       " +
"      \"Unit\": \"Gal\",                                                " +
"      \"MetricUnit\": \"Lit\",                                          " +
"      \"PID\": \"250\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"1000000\",                                        " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"1000000\",                                      " +
"      \"HighRed\": \"1000000\",                                         " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Engine Hours\",                                  " +
"      \"Abbreviation\": \"HOURS\",                                      " +
"      \"Unit\": \"Hrs\",                                                " +
"      \"MetricUnit\": \"Hrs\",                                          " +
"      \"PID\": \"247\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"1000000\",                                        " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"1000000\",                                      " +
"      \"HighRed\": \"1000000\",                                         " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Total Miles\",                                   " +
"      \"Abbreviation\": \"MLS\",                                        " +
"      \"Unit\": \"Mi\",                                                 " +
"      \"MetricUnit\": \"Km\",                                           " +
"      \"PID\": \"245\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"999999\",                                         " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"1000000\",                                      " +
"      \"HighRed\": \"1000000\",                                         " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Instantaneous MPG\",                             " +
"      \"Abbreviation\": \"IMPG\",                                       " +
"      \"Unit\": \"MPG\",                                                " +
"      \"MetricUnit\": \"KPL\",                                          " +
"      \"PID\": \"184\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"20\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"20\",                                           " +
"      \"HighRed\": \"20\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Fuel Rate\",                                     " +
"      \"Abbreviation\": \"FRATE\",                                      " +
"      \"Unit\": \"GPH\",                                                " +
"      \"MetricUnit\": \"LPH\",                                          " +
"      \"PID\": \"183\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"15\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"15\",                                           " +
"      \"HighRed\": \"15\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Transmission Temperature\",                      " +
"      \"Abbreviation\": \"TRANS\",                                      " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"177\",                                                 " +
"      \"GaugeMin\": \"32\",                                             " +
"      \"GaugeMax\": \"300\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"250\",                                          " +
"      \"HighRed\": \"280\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Oil Temperature\",                               " +
"      \"Abbreviation\": \"OIL\",                                        " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"175\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"300\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"200\",                                          " +
"      \"HighRed\": \"300\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Engine Coolant Temp\",                           " +
"      \"Abbreviation\": \"TEMP\",                                       " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"110\",                                                 " +
"      \"GaugeMin\": \"32\",                                             " +
"      \"GaugeMax\": \"260\",                                            " +
"      \"LowYellow\": \"32\",                                            " +
"      \"LowRed\": \"32\",                                               " +
"      \"HighYellow\": \"215\",                                          " +
"      \"HighRed\": \"225\",                                             " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Battery Volts\",                                 " +
"      \"Abbreviation\": \"VOLTS\",                                      " +
"      \"Unit\": \"V DC\",                                               " +
"      \"MetricUnit\": \"V DC\",                                         " +
"      \"PID\": \"168\",                                                 " +
"      \"GaugeMin\": \"8\",                                              " +
"      \"GaugeMax\": \"16\",                                             " +
"      \"LowYellow\": \"13\",                                            " +
"      \"LowRed\": \"12\",                                               " +
"      \"HighYellow\": \"14.5\",                                         " +
"      \"HighRed\": \"15\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Oil Pressure\",                                  " +
"      \"Abbreviation\": \"OIL\",                                        " +
"      \"Unit\": \"PSI\",                                                " +
"      \"MetricUnit\": \"kPa\",                                          " +
"      \"PID\": \"100\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"30\",                                            " +
"      \"LowRed\": \"10\",                                               " +
"      \"HighYellow\": \"100\",                                          " +
"      \"HighRed\": \"100\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Cruise Set Speed\",                              " +
"      \"Abbreviation\": \"CSPD\",                                       " +
"      \"Unit\": \"MPH\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"86\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"90\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"55\",                                           " +
"      \"HighRed\": \"75\",                                              " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Turbo Boost Pressure\",                          " +
"      \"Abbreviation\": \"TURBO\",                                      " +
"      \"Unit\": \"PSI\",                                                " +
"      \"MetricUnit\": \"kPa\",                                          " +
"      \"PID\": \"102\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"32\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"24\",                                           " +
"      \"HighRed\": \"32\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Intake Manifold Temp\",                          " +
"      \"Abbreviation\": \"MFOLD\",                                      " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"105\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"250\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"150\",                                          " +
"      \"HighRed\": \"250\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
//"    {                                                                   " +
//"      \"ParamName\": \"Cruise Status\",                                 " +
//"      \"Abbreviation\": \"CSTAT\",                                      " +
//"      \"Unit\": \"\",                                                   " +
//"      \"MetricUnit\": \"\",                                             " +
//"      \"PID\": \"85\",                                                  " +
//"      \"GaugeMin\": \"0\",                                              " +
//"      \"GaugeMax\": \"255\",                                            " +
//"      \"LowYellow\": \"0\",                                             " +
//"      \"LowRed\": \"0\",                                                " +
//"      \"HighYellow\": \"255\",                                          " +
//"      \"HighRed\": \"255\",                                             " +
//"      \"Format\": \"{0: 0}\",                                           " +
//"      \"Offset\": \"0\",                                                " +
//"      \"Multiplier\": \"1\"                                             " +
//"    },                                                                  " + TODO - uncomment when implemented
"    {                                                                   " +
"      \"ParamName\": \"Engine Speed (Tachometer)\",                     " +
"      \"Abbreviation\": \"TACH\",                                       " +
"      \"Unit\": \"RPM\",                                                " +
"      \"MetricUnit\": \"RPM\",                                          " +
"      \"PID\": \"190\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"3000\",                                           " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"2400\",                                         " +
"      \"HighRed\": \"2600\",                                            " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Coolant Level\",                                 " +
"      \"Abbreviation\": \"CLVL\",                                       " +
"      \"Unit\": \"%\",                                                  " +
"      \"MetricUnit\": \"%\",                                            " +
"      \"PID\": \"111\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"20\",                                            " +
"      \"LowRed\": \"5\",                                                " +
"      \"HighYellow\": \"100\",                                          " +
"      \"HighRed\": \"100\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Engine Load\",                                   " +
"      \"Abbreviation\": \"LOAD\",                                       " +
"      \"Unit\": \"%\",                                                  " +
"      \"MetricUnit\": \"%\",                                            " +
"      \"PID\": \"92\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"75\",                                           " +
"      \"HighRed\": \"100\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
//"    {                                                                   " +
//"      \"ParamName\": \"PressurePro Warning\",                           " +
//"      \"Abbreviation\": \"TWARN\",                                      " +
//"      \"Unit\": \"\",                                                   " +
//"      \"MetricUnit\": \"\",                                             " +
//"      \"PID\": \"501\",                                                 " +
//"      \"GaugeMin\": \"0\",                                              " +
//"      \"GaugeMax\": \"3\",                                              " +
//"      \"LowYellow\": \"0.050000001\",                                   " +
//"      \"LowRed\": \"0\",                                                " +
//"      \"HighYellow\": \"1.5\",                                          " +
//"      \"HighRed\": \"2.5\",                                             " +
//"      \"Format\": \"{0: 0}\",                                           " +
//"      \"Offset\": \"0\",                                                " +
//"      \"Multiplier\": \"1\"                                             " +
//"    },                                                                  " + TODO - uncomment when implemented
"    {                                                                   " +
"      \"ParamName\": \"Air Inlet Temp\",                                " +
"      \"Abbreviation\": \"AirInlet\",                                   " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"172\",                                                 " +
"      \"GaugeMin\": \"32\",                                             " +
"      \"GaugeMax\": \"300\",                                            " +
"      \"LowYellow\": \"150\",                                           " +
"      \"LowRed\": \"200\",                                              " +
"      \"HighYellow\": \"200\",                                          " +
"      \"HighRed\": \"275\",                                             " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Instant Liters Per 100km\",                      " +
"      \"Abbreviation\": \"LPKM\",                                       " +
"      \"Unit\": \"L/100Km\",                                            " +
"      \"MetricUnit\": \"L/100Km\",                                      " +
"      \"PID\": \"603\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"75\",                                             " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"40\",                                           " +
"      \"HighRed\": \"40\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Recent L/100Km\",                                " +
"      \"Abbreviation\": \"Recent LPK\",                                 " +
"      \"Unit\": \"L/100KM\",                                            " +
"      \"MetricUnit\": \"L/100KM\",                                      " +
"      \"PID\": \"602\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"50\",                                           " +
"      \"HighRed\": \"75\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Rolling L/100km\",                               " +
"      \"Abbreviation\": \"RLPK\",                                       " +
"      \"Unit\": \"L/100Km\",                                            " +
"      \"MetricUnit\": \"L/100Km\",                                      " +
"      \"PID\": \"601\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"75\",                                           " +
"      \"HighRed\": \"75\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Total Miles (Alternative)\",                     " +
"      \"Abbreviation\": \"Mls\",                                        " +
"      \"Unit\": \"Mi\",                                                 " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"244\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"1000000\",                                        " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"1000000\",                                      " +
"      \"HighRed\": \"1000000\",                                         " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"703.03998\",                                        " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Fuel Temperature\",                              " +
"      \"Abbreviation\": \"FTEMP\",                                      " +
"      \"Unit\": \"F\",                                                  " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"174\",                                                 " +
"      \"GaugeMin\": \"60\",                                             " +
"      \"GaugeMax\": \"200\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"200\",                                          " +
"      \"HighRed\": \"200\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Trans Output Speed\",                            " +
"      \"Abbreviation\": \"TSPD\",                                       " +
"      \"Unit\": \"RPM\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"191\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"5000\",                                           " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"5000\",                                         " +
"      \"HighRed\": \"5000\",                                            " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Accelerator Position\",                          " +
"      \"Abbreviation\": \"ACCEL\",                                      " +
"      \"Unit\": \"%\",                                                  " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"91\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"100\",                                          " +
"      \"HighRed\": \"100\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Engine Idle Fuel\",                              " +
"      \"Abbreviation\": \"IDLEG\",                                      " +
"      \"Unit\": \"Gal\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"236\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"1000000\",                                        " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"1000000\",                                      " +
"      \"HighRed\": \"1000000\",                                         " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Engine Idle Hours\",                             " +
"      \"Abbreviation\": \"IDLEH\",                                      " +
"      \"Unit\": \"Hrs\",                                                " +
"      \"MetricUnit\": \"Hrs\",                                          " +
"      \"PID\": \"235\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"1000000\",                                        " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"1000000\",                                      " +
"      \"HighRed\": \"1000000\",                                         " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Ambient Temp\",                                  " +
"      \"Abbreviation\": \"Amb. Temp\",                                  " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"171\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"150\",                                            " +
"      \"LowYellow\": \"100\",                                           " +
"      \"LowRed\": \"120\",                                              " +
"      \"HighYellow\": \"120\",                                          " +
"      \"HighRed\": \"150\",                                             " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Acceleration\",                                  " +
"      \"Abbreviation\": \"ACC\",                                        " +
"      \"Unit\": \"MPHPS\",                                              " +
"      \"MetricUnit\": \"KPHPS\",                                        " +
"      \"PID\": \"10\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"0\",                                            " +
"      \"HighRed\": \"0\",                                               " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Peak Braking\",                                  " +
"      \"Abbreviation\": \"Braking\",                                    " +
"      \"Unit\": \"MPHPS\",                                              " +
"      \"MetricUnit\": \"KPHPS\",                                        " +
"      \"PID\": \"11\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"0\",                                            " +
"      \"HighRed\": \"0\",                                               " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Peak Accelleration\",                            " +
"      \"Abbreviation\": \"Peak Acc\",                                   " +
"      \"Unit\": \"MPHPS\",                                              " +
"      \"MetricUnit\": \"KPHPS\",                                        " +
"      \"PID\": \"12\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"0\",                                            " +
"      \"HighRed\": \"0\",                                               " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Transmission Mode\",                             " +
"      \"Abbreviation\": \"TMODE\",                                      " +
"      \"Unit\": \"MPH\",                                                     " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"13\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"2\",                                              " +
"      \"LowYellow\": \"1.5\",                                           " +
"      \"LowRed\": \"0.5\",                                              " +
"      \"HighYellow\": \"2.5\",                                          " +
"      \"HighRed\": \"3.5\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Max Manifold Temp\",                             " +
"      \"Abbreviation\": \"MxMAN\",                                      " +
"      \"Unit\": \"DEG\",                                                " +
"      \"MetricUnit\": \"KPH\",                                          " +
"      \"PID\": \"506\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"250\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"150\",                                          " +
"      \"HighRed\": \"180\",                                             " +
"      \"Format\": \"{0: 0}\",                                           " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Retarder Percent\",                              " +
"      \"Abbreviation\": \"RTDR %\",                                     " +
"      \"Unit\": \"%\",                                                  " +
"      \"MetricUnit\": \"%\",                                            " +
"      \"PID\": \"520\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"80\",                                           " +
"      \"HighRed\": \"90\",                                              " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Retarder Switch (Hydraulic)\",                   " +
"      \"Abbreviation\": \"RTDR SW\",                                    " +
"      \"Unit\": \"\",                                                     " +
"      \"MetricUnit\": \"\",                                               " +
"      \"PID\": \"47\",                                                  " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"0\",                                              " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"0\",                                            " +
"      \"HighRed\": \"0\",                                               " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Engine Retarder Status (Hydraulic)\",            " +
"      \"Abbreviation\": \"ERTDR STAT\",                                 " +
"      \"Unit\": \"\",                                                     " +
"      \"MetricUnit\": \"\",                                               " +
"      \"PID\": \"121\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"0\",                                              " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"0\",                                            " +
"      \"HighRed\": \"0\",                                               " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Retarder Oil Temp (Hydraulic)\",                 " +
"      \"Abbreviation\": \"RTDR TEMP\",                                  " +
"      \"Unit\": \"°F\",                                                 " +
"      \"MetricUnit\": \"°C\",                                           " +
"      \"PID\": \"120\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"300\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"200\",                                          " +
"      \"HighRed\": \"250\",                                             " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    },                                                                  " +
"    {                                                                   " +
"      \"ParamName\": \"Retarder Oil Pressure (Hydraulic)\",             " +
"      \"Abbreviation\": \"RTDR PRES\",                                  " +
"      \"Unit\": \"PSI\",                                                " +
"      \"MetricUnit\": \"KPa\",                                          " +
"      \"PID\": \"119\",                                                 " +
"      \"GaugeMin\": \"0\",                                              " +
"      \"GaugeMax\": \"100\",                                            " +
"      \"LowYellow\": \"0\",                                             " +
"      \"LowRed\": \"0\",                                                " +
"      \"HighYellow\": \"100\",                                          " +
"      \"HighRed\": \"100\",                                             " +
"      \"Format\": \"{0:0.#}\",                                          " +
"      \"Offset\": \"0\",                                                " +
"      \"Multiplier\": \"1\"                                             " +
"    }                                                                   " +
"  ]                                                                     " +
"}";
    }
}
