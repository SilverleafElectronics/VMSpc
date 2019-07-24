using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using static VMSpc.Constants;
using VMSpc.DevHelpers;


namespace VMSpc.XmlFileManagers
{
    /// <summary>
    /// Singleton object for reading from and writing to default.scr.xml
    /// </summary>
    public sealed class DefaultScrnManager : XmlFileManager
    {
        static DefaultScrnManager()
        {
        }
        private DefaultScrnManager() : base("default.scr.xml")
        {
        }
        ~DefaultScrnManager()
        {
            VMSConsole.PrintLine("Closing");
        }
        public static DefaultScrnManager scrnManager { get; } = new DefaultScrnManager();
        public List<PanelSettings> configurationPanelList;


        public void LoadPanels()
        {
            configurationPanelList = new List<PanelSettings>();
            for (ushort i = 1; i <= GetPanelCount(); i++)
                configurationPanelList.Add(GetPanel(i));
        }

        public int GetPanelCount()
        {
            return Int32.Parse(getNodeValueByTagName("Panel-Count"));
        }

        public void SetPanelCount(int count)
        {
            getNodeByTagName("Panel-Count").InnerText = count.ToString();
        }

        public void AddNewPanel(PanelSettings panelSettings)
        {
            panelSettings.number = (ushort)(configurationPanelList.Last().number + 1);
            configurationPanelList.Add(panelSettings);
            SetPanelCount(GetPanelCount() + 1);
            XmlNode panelNode = panelSettings.GenerateNodeAsXml(this);
            panelSettings.SaveSettings(panelNode);
        }

        public void DeletePanel(int panelNumber)
        {
            SetPanelCount(GetPanelCount() - 1);
            xmlDoc.RemoveChild(getNodeByTagAndAttr("Panel", "Number", panelNumber.ToString()));
            for (ushort i = (ushort)(panelNumber); i < GetPanelCount(); i++)
            {
                configurationPanelList[i] = configurationPanelList[i + 1];
            }
            SaveConfiguration();
        }

        /// <summary>   Returns a PanelSettings object for use in constructing a new panel  </summary>
        public PanelSettings GetPanel(ushort number)
        {
            XmlNode parentNode = getNodeByTagAndAttr("Panel", "Number", number.ToString());
            XmlNode panelNode = parentNode.FirstChild;
            PanelSettings panel = GetPanelType(Char.Parse(parentNode.SelectSingleNode("ID").InnerText), number);
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

        private PanelSettings GetPanelType(char cid, ushort number)
        {
            switch (cid)
            {
                case PanelIDs.SIMPLE_GAUGE_ID:
                    return new SimpleGaugeSettings(number);
                case PanelIDs.SCAN_GAUGE_ID:
                    return new ScanGaugeSettings(number);
                case PanelIDs.RADIAL_GAUGE_ID:
                    return new RoundGaugeSettings(number);
                case PanelIDs.TANK_MINDER_ID:
                    return new TankMinderSettings(number);
                case PanelIDs.TEXT_PANEL_ID:
                    return new TextGaugeSettings(number);
                case PanelIDs.TIRE_PANEL_ID:
                    break;
                case PanelIDs.TRANSMISSION_ID:
                    return new TransmissionGaugeSettings(number);
                case PanelIDs.CLOCK_PANEL_ID:
                    return new ClockSettings(number);
                case PanelIDs.DIAG_ALARM_ID:
                    return new DiagnosticGaugeSettings(number);
                case PanelIDs.HISTOGRAM_ID:
                    break;
                case PanelIDs.IMG_PANEL_ID:
                    return new PictureSettings(number);
                case PanelIDs.MESSAGE_PANEL_ID:
                    return new MessageBoxSettings(number);
                case PanelIDs.MULTIBAR_ID:
                    return new MultiBarSettings(number);
                case PanelIDs.ODOMOTER_ID:
                    return new OdometerSettings(number);
                default:
                    throw new Exception("An invalid panel type was requested");
            }
            return new PanelSettings(number);
        }

        public override void SaveConfiguration()
        {
            foreach (var panel in configurationPanelList)
            {
                XmlNode parentNode = getNodeByTagAndAttr("Panel", "Number", panel.number.ToString());
                panel.SaveSettings(parentNode);
            }
            base.SaveConfiguration();
        }
    }
}

