using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc
{
    public static class About
    {
        public const string version = "5.0.15";
    }
}

/*********************************************************************************************
 * 
 * Release Notes
 * 
 * 5.0.11: 10/06/2020
 *      -   Fixed DisplayTemperature multiplier/offset.
 *      -   Set TireStatus to TireStatus.None when clearing a tire or aborting the learning procedure.
 * 5.0.12: 
 *      -   Added Save Layout option
 *      -   Fixed bug causing frozen values in J1939 data
 * 5.0.13:
 *      -   Implemented manual save
 * 5.0.14:
 *      -   Fixed Parameter editor bug, which threw an exception when 
 *      -   Fixed Bar gauge bug, which had an array indexing error when red/yellow/green values were unset in the parameter
 *      -   Added Clock
 * 5.0.15:
 *      -   Fixed TST configuration options
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 *********************************************************************************************/
