using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    class PanelManager
    {
        public List<VPanel> PanelList = new List<VPanel>();
        private MainWindow parent;
        public PanelManager(MainWindow parent)
        {
            this.parent = parent;
            InitPanels();
            LoadPanels();
        }

        private void InitPanels()
        {
            int numPanels = DefaultScrnManager.scrnManager.GetPanelCount();
            for (int i = 1; i <= numPanels; i++)
            {
                PanelSettings panelSettings = DefaultScrnManager.scrnManager.GetPanel(i);
                VPanel panel = null;
                switch (panelSettings.ID)
                {
                    case Constants.PanelIDs.SIMPLE_GAUGE_ID:
                        panel = new VSimpleGauge(parent, panelSettings);
                        break;
                    case Constants.PanelIDs.SCAN_GAUGE_ID:
                        break;
                    case Constants.PanelIDs.ODOMOTER_ID:
                        break;
                    case Constants.PanelIDs.TRANSMISSION_ID:
                        break;
                    case Constants.PanelIDs.MULTIBAR_ID:
                        break;
                    case Constants.PanelIDs.HISTOGRAM_ID:
                        break;
                    case Constants.PanelIDs.CLOCK_PANEL_ID:
                        break;
                    case Constants.PanelIDs.IMG_PANEL_ID:
                        break;
                    case Constants.PanelIDs.TEXT_PANEL_ID:
                        break;
                    case Constants.PanelIDs.TANK_MINDER_ID:
                        break;
                    case Constants.PanelIDs.TIRE_PANEL_ID:
                        break;
                    case Constants.PanelIDs.MESSAGE_PANEL_ID:
                        break;
                    case Constants.PanelIDs.DIAG_ALARM_ID:
                        break;
                    case Constants.PanelIDs.RADIAL_GAUGE_ID:
                        break;
                    default:
                        break;
                }
                if (panel != null)
                    PanelList.Add(panel);
            }
        }

        private void LoadPanels()
        {
            foreach (VPanel panel in PanelList)
            {
                panel.Show();
            }
        }

    }
}
