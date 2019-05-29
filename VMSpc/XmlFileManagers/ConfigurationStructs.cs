using System;
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
        public char ID;
        public PanelCoordinates rectCord;
        public Color backgroundColor;
        public Color baseColor;
        public Color explicitColor;
        public Color explicitGaugeColor;
        public Color explicitFillColor;
        public int PID;
        public int[] PIDList;
        public bool showSpot;
        public bool showName;
        public bool showValue;
        public bool showUnit;
        public bool showGraph;
        public bool showAbbreviation;
        public bool showInMetric;
        public bool useStaticColor;
        public int format;
        public PanelSettings()
        {

        }

        /// <summary>
        /// Stores the xml value in the specified propName. Only pass propNames
        /// of type Color to this method
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="node"></param>
        public void StoreColorFromXml(string propName, XmlNode node)
        {
            switch (propName)
            {
                case "backgroundColor":
                    backgroundColor = GetColorFromXml(node);
                    break;
                case "baseColor":
                    baseColor = GetColorFromXml(node);
                    break;
                case "explicitColor":
                    explicitColor = GetColorFromXml(node);
                    break;
                case "explicitGaugeColor":
                    explicitGaugeColor = GetColorFromXml(node);
                    break;
                case "explicitFillColor":
                    explicitFillColor = GetColorFromXml(node);
                    break;
                default:
                    break;
            }
        }

        private Color GetColorFromXml(XmlNode node)
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
}