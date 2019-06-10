using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using static VMSpc.Constants;


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
            PanelSettings panel = GetPanelType(Char.Parse(parentNode.SelectSingleNode("ID").InnerText));
            do
            {
                string val = panelNode.InnerText;
 //               if (panelNode.Name = "ID")
 //                   CastPanelToChild(panel, Char.Parse(panelNode.InnerText))
                panel.StoreSettings(panelNode.Name, val, panelNode);
                panelNode = panelNode.NextSibling;
            } while (panelNode != null);
            return panel;
        }
        public void SavePanelSettings(PanelSettings settings)
        {

        }
        private PanelSettings GetPanelType(char cid)
        {
            switch (cid)
            {
                case PanelIDs.SIMPLE_GAUGE_ID:
                    return new SimpleGaugeSettings();
                case PanelIDs.SCAN_GAUGE_ID:
                    return new ScanGaugeSettings();
                case PanelIDs.RADIAL_GAUGE_ID:
                    return new RoundGaugeSettings();
                case PanelIDs.TANK_MINDER_ID:
                    return new TankMinderSettings();
                case PanelIDs.TEXT_PANEL_ID:
                    return new TextGaugeSettings();
                case PanelIDs.TIRE_PANEL_ID:
                    break;
                case PanelIDs.TRANSMISSION_ID:
                    return new TransmissionGaugeSettings();
                case PanelIDs.CLOCK_PANEL_ID:
                    return new ClockSettings();
                case PanelIDs.DIAG_ALARM_ID:
                    return new DiagnosticGaugeSettings();
                case PanelIDs.HISTOGRAM_ID:
                    break;
                case PanelIDs.IMG_PANEL_ID:
                    return new PictureSettings();
                case PanelIDs.MESSAGE_PANEL_ID:
                    return new MessageBoxSettings();
                case PanelIDs.MULTIBAR_ID:
                    break;
                case PanelIDs.ODOMOTER_ID:
                    return new OdometerSettings();
                default:
                    return new PanelSettings();
            }
            return new PanelSettings();
        }
    }
}

