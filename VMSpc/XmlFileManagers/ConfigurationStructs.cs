using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml;
using static VMSpc.Constants;

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
        public bool showInMetric;
        public int TextPosition;
        public int Use_Static_Color;

        public PanelSettings(ushort number)
        { 
            this.number = number;
            showInMetric = false;
            ID = PanelIDs.NO_ID;
        }

        protected void GenerateColorNodeAsXml(XmlFileManager fileManager, XmlNode parentNode, string newNodeName)
        {
            XmlNode newNode = fileManager.AddNodeToParentNode(parentNode, newNodeName);
            fileManager.AddAttributeToNode(newNode, "Red", "");
            fileManager.AddAttributeToNode(newNode, "Green", "");
            fileManager.AddAttributeToNode(newNode, "Blue", "");
        }

        public virtual XmlNode GenerateNodeAsXml(XmlFileManager fileManager)
        {
            XmlNode panelNode = fileManager.AddNodeToParentTag("Screen-Elements", "Panel");
            fileManager.AddAttributeToNode(panelNode, "Number", number.ToString());
            fileManager.AddNodeToParentNode(panelNode, "ID");
            XmlNode rectCordNode = fileManager.AddNodeToParentNode(panelNode, "Rect-Cord");
            fileManager.AddAttributeToNode(rectCordNode, "Top-LeftX", rectCord.topLeftX.ToString());
            fileManager.AddAttributeToNode(rectCordNode, "Top-LeftY", rectCord.topLeftY.ToString());
            fileManager.AddAttributeToNode(rectCordNode, "Bottom-RightX", rectCord.bottomRightX.ToString());
            fileManager.AddAttributeToNode(rectCordNode, "Bottom-RightY", rectCord.bottomRightY.ToString());
            GenerateColorNodeAsXml(fileManager, panelNode, "BackGround-Color");
            GenerateColorNodeAsXml(fileManager, panelNode, "Base-Color");
            GenerateColorNodeAsXml(fileManager, panelNode, "Explicit-Color");
            GenerateColorNodeAsXml(fileManager, panelNode, "Explicit-Gauge-Color");
            fileManager.AddNodeToParentNode(panelNode, "Show-In-Metric");
            fileManager.AddNodeToParentNode(panelNode, "Format");
            fileManager.AddNodeToParentNode(panelNode, "Use-Static-Color");
            return panelNode;
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
                    backgroundColor = GetColorFromXml(panelNode);
                    break;
                case "Base-Color":
                    baseColor = GetColorFromXml(panelNode);
                    break;
                case "Explicit-Color":
                    explicitColor = GetColorFromXml(panelNode);
                    break;
                case "Explicit-Gauge-Color":
                    explicitGaugeColor = GetColorFromXml(panelNode);
                    break;
                case "Format":
                    try { TextPosition = Int32.Parse(val); }
                    catch { }
                    break;
                case "Show-In-Metric":
                    try   { showInMetric = Boolean.Parse(val); }
                    catch { }
                    break;
            }
        }

        protected void SafeSave(XmlNode node, string nodeName, string value)
        {
            try { node.SelectSingleNode(nodeName).InnerText = value; }
            catch
            {
                XmlDocument doc = node.OwnerDocument;
                XmlElement element = doc.CreateElement(nodeName);
                element.InnerText = value;
                node.AppendChild(element);
            }
        }

        public virtual void SaveSettings(XmlNode panelNode)
        {
            SafeSave(panelNode, "ID", ID.ToString());
            panelNode["Rect-Cord"].SetAttribute("Top-LeftX", rectCord.topLeftX.ToString());
            panelNode["Rect-Cord"].SetAttribute("Top-LeftY", rectCord.topLeftY.ToString());
            panelNode["Rect-Cord"].SetAttribute("Bottom-RightX", rectCord.bottomRightX.ToString());
            panelNode["Rect-Cord"].SetAttribute("Bottom-RightY", rectCord.bottomRightY.ToString());
            SaveColorToXml(panelNode, "BackGround-Color", ref backgroundColor);
            SaveColorToXml(panelNode, "Base-Color", ref baseColor);
            SaveColorToXml(panelNode, "Explicit-Color", ref explicitColor);
            SaveColorToXml(panelNode, "Explicit-Gauge-Color", ref explicitGaugeColor);
            SafeSave(panelNode, "Show-In-Metric", showInMetric.ToString());
            SafeSave(panelNode, "Format", TextPosition.ToString());
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

        public void SaveColorToXml(XmlNode node, string propName, ref Color prop)
        {
            node[propName].SetAttribute("Red", prop.R.ToString());
            node[propName].SetAttribute("Green", prop.G.ToString());
            node[propName].SetAttribute("Blue", prop.B.ToString());
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
        public ushort PID;
        public ushort PIDLimited;

        public MessageBoxSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Num-Lines":
                    numLines = Int32.Parse(val);
                    break;
                case "PID":
                    PID = UInt16.Parse(val);
                    break;
                case "PID-Limited":
                    PIDLimited = UInt16.Parse(val);
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
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }

        public override void SaveSettings(XmlNode panelNode)
        {
            base.SaveSettings(panelNode);
            SafeSave(panelNode, "Show-Spot", showSpot.ToString());
            SafeSave(panelNode, "Show-Name", showName.ToString());
            SafeSave(panelNode, "Show-Value", showValue.ToString());
            SafeSave(panelNode, "Show-Unit", showUnit.ToString());
            SafeSave(panelNode, "Show-Graph", showGraph.ToString());
            SafeSave(panelNode, "Show-Abbreviation", showAbbreviation.ToString());
        }

        public override XmlNode GenerateNodeAsXml(XmlFileManager fileManager)
        {
            XmlNode panelNode = base.GenerateNodeAsXml(fileManager);
            fileManager.AddNodeToParentNode(panelNode, "Show-Spot");
            fileManager.AddNodeToParentNode(panelNode, "Show-Name");
            fileManager.AddNodeToParentNode(panelNode, "Show-Value");
            fileManager.AddNodeToParentNode(panelNode, "Show-Unit");
            fileManager.AddNodeToParentNode(panelNode, "Show-Graph");
            fileManager.AddNodeToParentNode(panelNode, "Show-Abbreviation");
            return panelNode;
        }
    }
    public class MultiBarSettings : GaugeSettings
    {
        public List<ushort> PIDList;
        public int numPids;
        public MultiBarSettings(ushort number) : base(number)
        {
            PIDList = new List<ushort>();
            numPids = 0;
        }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Param":
                    PIDList.Add(UInt16.Parse(panelNode.Attributes["PID"].InnerText));
                    numPids++;
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class ScanGaugeSettings : MultiBarSettings
    {
        public int scanSpeed;
        public ScanGaugeSettings(ushort number) : base(number)
        {
            scanSpeed = 10;
        }
        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "Scan-Speed":
                    scanSpeed = Int32.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }
    }
    public class SimpleGaugeSettings : GaugeSettings
    {
        public ushort PID;
        public SimpleGaugeSettings(ushort number) : base(number) { }

        public override void StoreSettings(string nodeName, string val, XmlNode panelNode)
        {
            switch (nodeName)
            {
                case "PID":
                    PID = UInt16.Parse(val);
                    break;
                default:
                    base.StoreSettings(nodeName, val, panelNode);
                    break;
            }
        }

        public override void SaveSettings(XmlNode panelNode)
        {
            base.SaveSettings(panelNode);
            SafeSave(panelNode, "PID", PID.ToString());
        }

        public override XmlNode GenerateNodeAsXml(XmlFileManager fileManager)
        {
            XmlNode panelNode = base.GenerateNodeAsXml(fileManager);
            fileManager.AddNodeToParentNode(panelNode, "PID");
            return panelNode;
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