using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Enums.UI
{
    public enum PanelType
    {
        NONE = 0,
        SIMPLE_GAUGE = 1,
        SCAN_GAUGE = 2,
        TANK_MINDER = 3,
        ODOMETER = 4,
        TRANSMISSION_GAUGE = 5,
        MULTIBAR = 6,
        HISTOGRAM = 7,
        CLOCK = 8,
        IMAGE = 9,
        TEXT = 10,
        MESSAGE = 11,
        DIAGNOSTIC_ALARM = 12,
        RADIAL_GAUGE = 13,
        TIRE_GAUGE = 14,
    };

    /// <summary>
    /// Operators to be used to determine a truthy result of comparing two operands
    /// </summary>
    public enum ComparativeOperator
    {
        /// <summary>
        /// Used to evaluate whether operand_1 &gt; operand_2
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Used to evaluate whether operand_1 &lt; operand_2
        /// </summary>
        LessThan,
        /// <summary>
        /// Used to evaluate whether operand_1 == operand_2
        /// </summary>
        EqualTo,
        /// <summary>
        /// Used to evaluate whether the operand_1 masked with operand_2 matches operand_2. Namely, whether (operand_1 &amp; operand_2) == operand_2
        /// </summary>
        BitMatch,
        /// <summary>
        /// Used to evaluate whether the bit representations of operand_1 and operand_2 are opposite. Nameley, whether (operand_1 &amp; operand_2) == 0
        /// </summary>
        BitZero,
    }

    public enum RelativePanelPointLocation
    {
        InsidePanel,
        OnBorder,
        Outside
    }

    public enum AlarmFrequency
    {
        Once,
        EveryFifteenMinutes,
        EveryMinute,
        EverySecond,
    };

    public enum UIPosition
    {
        CENTER = 0,
        LEFT = 1,
        RIGHT = 2
    }

    public enum UILayout
    {
        HORIZONTAL = 0,
        VERTICAL = 1
    }
}
