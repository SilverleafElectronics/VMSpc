using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    class VSimpleGauge : VBarGauge
    {
        private MainWindow mainWindow;

        public VSimpleGauge(MainWindow mainWindow, PanelSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.mainWindow = mainWindow;
            panelSettings = (SimpleGaugeSettings)panelSettings;
        }

        public override void GeneratePanel()
        {
            base.GeneratePanel();
        }
    }
}
