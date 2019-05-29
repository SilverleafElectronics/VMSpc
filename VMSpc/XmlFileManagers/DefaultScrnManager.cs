using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;


namespace VMSpc.XmlFileManagers
{
    /// <summary>
    /// Interface for reading from and writing to default.scr.xml
    /// </summary>
    public sealed class DefaultScrnManager : XmlFileManager
    {
        static DefaultScrnManager()
        {
        }
        private DefaultScrnManager() : base("default.scr.xml")
        {
        }
        public static DefaultScrnManager scrnManager { get; } = new DefaultScrnManager();

        public int GetPanelCount()
        {
            return Int32.Parse(getNodeValueByTagName("Panel-Count"));
        }

        /// <summary>
        /// Returns a PanelSettings object for use in constructing a new panel
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public PanelSettings GetPanel(int number)
        {
            XmlNode parentNode = getNodeByTagAndAttr("Panel", "Number", number.ToString());
            XmlNode panelNode = parentNode.FirstChild;
            PanelSettings panel = new PanelSettings();
            do
            {
                string val = panelNode.InnerText;
                switch (panelNode.Name)
                {
                    case "ID":
                        panel.ID = Char.Parse(val);
                        break;
                    case "Rect-Cord":
                        panel.rectCord.GetCordsFromXml(panelNode);
                        break;
                    case "BackGround-Color":
                        panel.StoreColorFromXml("backgroundColor", panelNode);
                        break;
                    case "Base-Color":
                        panel.StoreColorFromXml("baseColor", panelNode);
                        break;
                    case "Explicit-Color":
                        panel.StoreColorFromXml("explicitColor", panelNode);
                        break;
                    case "Explicit-Gauge-Color":
                        panel.StoreColorFromXml("explicitGaugeColor", panelNode);
                        break;
                    case "Explicit-Fill-Color":
                        panel.StoreColorFromXml("explicitFillColor", panelNode);
                        break;
                    case "PID":
                        panel.PID = Int32.Parse(val);
                        break;
                    case "Show-Spot":
                        panel.showSpot = Boolean.Parse(val);
                        break;
                    case "Show-Name":
                        panel.showName = Boolean.Parse(val);
                        break;
                    case "Show-Value":
                        panel.showValue = Boolean.Parse(val);
                        break;
                    case "Show-Unit":
                        panel.showUnit = Boolean.Parse(val);
                        break;
                    case "Show-Graph":
                        panel.showGraph = Boolean.Parse(val);
                        break;
                    case "Show-Abbreviation":
                        panel.showAbbreviation = Boolean.Parse(val);
                        break;
                    case "Show-In-Metric":
                        panel.showInMetric = Boolean.Parse(val);
                        break;
                    case "Use-Static-Color":
                        panel.useStaticColor = Boolean.Parse(val);
                        break;
                    case "Format":
                        panel.format = Int32.Parse(val);
                        break;
                    default:
                        break;
                }
                panelNode = panelNode.NextSibling;
            } while (panelNode != null);

            return panel;

        }
    }
}

