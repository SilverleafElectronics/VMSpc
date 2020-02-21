using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.VEnum.UI
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
