using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml;

namespace VMSpc.XmlFileManagers
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
        
        public void GetCordsFromXml(XmlNode node)
        {
            topLeftX     = Int32.Parse(node.Attributes["Top-LeftX"].Value);
            topLeftY     = Int32.Parse(node.Attributes["Top-LeftY"].Value);
            bottomRightX = Int32.Parse(node.Attributes["Bottom-RightX"].Value);
            bottomRightY = Int32.Parse(node.Attributes["Bottom-RightY"].Value);
        }

        public void WriteCordsToXml(PanelCoordinates coords)
        {

        }
    }

    public class PanelSettings
    {
        public ushort number;
        public char ID;
        public PanelCoordinates rectCord;
        public Color backgroundColor;
        public Color baseColor;
        public Color explicitColor;
        public Color explicitGaugeColor;
        public int Text_Position;
        public int Use_Static_Color;

        public PanelSettings(ushort number)
        {
            this.number = number;
        }

        public virtual void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "ID":
                    ID = Char.Parse(val);
                    break;
                case "Rect-Cord":
                    rectCord.GetCordsFromXml(panelNode);
                    break;
                case "BackGround-Color":
                    GetColorFromXml(panelNode);
                    break;
                case "Base-Color":
                    GetColorFromXml(panelNode);
                    break;
                case "Explicit-Color":
                    GetColorFromXml(panelNode);
                    break;
                case "Explicit-Gauge-Color":
                    GetColorFromXml(panelNode);
                    break;
            }
        }

        public virtual void SaveSettings(XmlNode panelNode)
        {
            panelNode["Rect-Cord"].SetAttribute("Top-LeftX", rectCord.topLeftX.ToString());
            panelNode["Rect-Cord"].SetAttribute("Top-LeftY", rectCord.topLeftY.ToString());
            panelNode["Rect-Cord"].SetAttribute("Bottom-RightX", rectCord.bottomRightX.ToString());
            panelNode["Rect-Cord"].SetAttribute("Bottom-RightY", rectCord.bottomRightY.ToString());
        }

        protected Color GetColorFromXml(XmlNode node)
        {
            Color color = Color.FromRgb(
                Byte.Parse(node.Attributes["Red"].Value),
                Byte.Parse(node.Attributes["Green"].Value),
                Byte.Parse(node.Attributes["Blue"].Value)
            );
            return color;
        }

        public void SaveColorToXml(string propName, XmlNode node)
        {

        }
    }

    public class ClockSettings : PanelSettings
    {
        public bool showDate;
        public bool showAMPM;

        public ClockSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Show-Date":
                    showDate = Boolean.Parse(val);
                    break;
                case "Show-AMPM":
                    showAMPM = Boolean.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class MessageBoxSettings : PanelSettings
    {
        public int numLines;
        public int PID;
        public int PIDLimited;

        public MessageBoxSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Num-Lines":
                    numLines = Int32.Parse(val);
                    break;
                case "PID":
                    PID = Int32.Parse(val);
                    break;
                case "PID-Limited":
                    PIDLimited = Int32.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class PictureSettings : PanelSettings
    {
        public string BMPFileName;
        public PictureSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "BMP-File-Name":
                    BMPFileName = val;
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class DiagnosticGaugeSettings : PanelSettings
    {
        public Color WarningColor;
        public DiagnosticGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Warning-Color":
                    WarningColor = GetColorFromXml(panelNode);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class TextGaugeSettings : PanelSettings
    {
        public int textLength;
        public string text;
        public TextGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Text-Length":
                    textLength = Int32.Parse(val);
                    break;
                case "Text":
                    text = val;
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class TransmissionGaugeSettings : PanelSettings
    {
        public bool showAttained;
        public bool showSelected;
        public TransmissionGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Show-Attained":
                    showAttained = Boolean.Parse(val);
                    break;
                case "Show-Selected":
                    showSelected = Boolean.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class RecordedSettings : PanelSettings
    {
        public bool showMPG;
        public bool showCaptions;
        public bool showUnits;
        public bool showInMetric;
        public bool layoutHorizontal;
        public string fileName;
        public RecordedSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Show-MPG":
                    showMPG = Boolean.Parse(val);
                    break;
                case "Show-Captions":
                    showCaptions = Boolean.Parse(val);
                    break;
                case "Show-Units":
                    showUnits = Boolean.Parse(val);
                    break;
                case "Show-In-Metric":
                    showInMetric = Boolean.Parse(val);
                    break;
                case "Layout-Horizontal":
                    layoutHorizontal = Boolean.Parse(val);
                    break;
                case "File-Name":
                    fileName = val;
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class TankMinderSettings : RecordedSettings
    {
        public bool showFuel;
        public bool showMilesToEmpty;
        public bool useRollingMPG;
        public int tankSize;
        public int tankSizeMetric;
        public TankMinderSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Show-Fuel":
                    showFuel = Boolean.Parse(val);
                    break;
                case "Show-Miles-To-Empty":
                    showMilesToEmpty = Boolean.Parse(val);
                    break;
                case "Use-Rolling-MPG":
                    useRollingMPG = Boolean.Parse(val);
                    break;
                case "Tank-Size":
                    tankSize = Int32.Parse(val);
                    break;
                case "Tank-Size-Metric":
                    tankSizeMetric = Int32.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class OdometerSettings : RecordedSettings
    {
        public bool showFuelLocked;
        public bool showHours;
        public bool showMiles;
        public bool showSpeed;
        public OdometerSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Show-Fuel-Locked":
                    showFuelLocked = Boolean.Parse(val);
                    break;
                case "Show-Hours":
                    showHours = Boolean.Parse(val);
                    break;
                case "Show-Miles":
                    showMiles = Boolean.Parse(val);
                    break;
                case "Show-Speed":
                    showSpeed = Boolean.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class GaugeSettings : PanelSettings
    {
        public bool showSpot;
        public bool showName;
        public bool showValue;
        public bool showUnit;
        public bool showGraph;
        public bool showAbbreviation;
        public bool showInMetric;
        public GaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Show-Spot":
                    showSpot = Boolean.Parse(val);
                    break;
                case "Show-Name":
                    showName = Boolean.Parse(val);
                    break;
                case "Show-Value":
                    showValue = Boolean.Parse(val);
                    break;
                case "Show-Unit":
                    showUnit = Boolean.Parse(val);
                    break;
                case "Show-Graph":
                    showGraph = Boolean.Parse(val);
                    break;
                case "Show-Abbreviation":
                    showAbbreviation = Boolean.Parse(val);
                    break;
                case "Show-In-Metric":
                    showInMetric = Boolean.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class ScanGaugeSettings : GaugeSettings
    {
        public List<int> PIDList;
        public ScanGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "PID":
                    PIDList.Add(Int32.Parse(val));
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class SimpleGaugeSettings : GaugeSettings
    {
        public int PID;
        public SimpleGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "PID":
                    PID = Int32.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class RoundGaugeSettings : SimpleGaugeSettings
    {
        public Color gaugeColor;
        public Color fillColor;
        public RoundGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Gauge-Color":
                    gaugeColor = GetColorFromXml(panelNode);
                    break;
                case "Fill-Color":
                    fillColor = GetColorFromXml(panelNode);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
}