using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMSpc.UI.TireMaps;
using VMSpc.Enums.UI;
using System.Windows.Controls;
using System.Windows;

namespace VMSpc.JsonFileManagers
{
    public struct WindowPlacement
    {
        public int Length;
        public int flags;
        public int showCmd;
        public int pointMinX;
        public int pointMinY;
        public int pointMaxX;
        public int pointMaxY;
        public int rcnpTopLeftX;
        public int rcnpTopLeftY;
        public int rcnpBottomRightX;
        public int rcnpBottomRightY;
    }

    public struct PanelCoordinates
    {
        public int topLeftX;
        public int topLeftY;
        public int bottomRightX;
        public int bottomRightY;
    }

    public abstract class PanelSettings
    {
        public ulong
            number;
        public PanelType
            panelId;
        public Color
            backgroundColor,
            captionColor,
            valueTextColor;
        public ulong
            parentPanelNumber;
        public bool
            showInMetric,
            useGlobalColorPalette;
        public HorizontalAlignment 
            alignment;
        public PanelCoordinates
            panelCoordinates;
        public Color borderColor;
        public PanelSettings(PanelType panelId) { /*this.panelId = panelId*/ }
    }

    public class ClockSettings : PanelSettings
    {
        public bool
            showDate,
            useMilitaryTime;
        public ClockSettings() : base(PanelType.CLOCK) { }
    }

    public class MessageBoxSettings : PanelSettings
    {
        public ushort
            numLines,
            pid,
            pidLimited;
        public MessageBoxSettings() : base(PanelType.MESSAGE) { }
    }

    public class PictureSettings : PanelSettings
    {
        public string
            bmpFilePath;
        public PictureSettings() : base(PanelType.IMAGE) { }
    }

    public class DiagnosticGaugeSettings : PanelSettings
    {
        public Color
            WarningColor;
        public bool
            useMilitaryTime;
        public DiagnosticGaugeSettings() : base(PanelType.DIAGNOSTIC_ALARM) { }
    }

    public class TextGaugeSettings : PanelSettings
    {
        public string
            text;
        public bool
            wrapText;
        public TextGaugeSettings() : base(PanelType.TEXT) { }
    }

    public class TransmissionGaugeSettings : PanelSettings
    {
        public bool
            showAttained,
            showSelected;
        public TransmissionGaugeSettings() : base(PanelType.TRANSMISSION_GAUGE) { }
    }

    public abstract class RecordedSettings : PanelSettings
    {
        public bool
            showMPG,
            showCaptions,
            showUnits;
        public Orientation
            orientation;
        public string
            fileName;
        public RecordedSettings(PanelType panelId) : base(panelId) { }
    }

    public class TankMinderSettings : RecordedSettings
    {
        public bool
            showFuel,
            showMilesToEmpty,
            useRollingMPG;
        public ushort
            tankSize;
        public TankMinderSettings() : base(PanelType.TANK_MINDER) { }
    }

    public class OdometerSettings : RecordedSettings
    {
        public bool
            showFuel,
            showHours,
            showMiles,
            showSpeed;
        public string
            dataFileName;
        public OdometerSettings() : base(PanelType.ODOMETER) { }
    }

    public abstract class GaugeSettings : PanelSettings
    {
        public bool
            showSpot,
            showName,
            showValue,
            showUnit,
            showGraph,
            showAbbreviation;
        public GaugeSettings(PanelType panelId) : base(panelId) { }
    }

    public class MultiBarSettings : GaugeSettings
    {
        public List<ushort>
            pidList;

        public MultiBarSettings() : base(PanelType.MULTIBAR) { }
    }

    public class SimpleGaugeSettings : GaugeSettings
    {
        public ushort
            pid;
        public SimpleGaugeSettings(PanelType panelId = PanelType.SIMPLE_GAUGE) : base(panelId) { }
    }

    public class RadialGaugeSettings : SimpleGaugeSettings
    {
        public Color
            gaugeColor, // color of the radial dial's background
            fillColor; // color of the radial dial's value fill
        public RadialGaugeSettings() : base(PanelType.RADIAL_GAUGE) { }
    }

    public class ScanGaugeSettings : GaugeSettings
    {
        public List<ushort>
            pidList;
        /// <summary> Interval for switching displayed PID </summary>
        public ushort
            scanSpeed;
        public ScanGaugeSettings() : base(PanelType.SCAN_GAUGE) { }
    }

    public class TireGaugeSettings : PanelSettings
    {
        public bool
            showIcon,
            showValue,
            detachTowed;
        public TireMapType
            tireMapType;
        public TireGaugeSettings() : base(PanelType.TIRE_GAUGE) { }
    }
}

