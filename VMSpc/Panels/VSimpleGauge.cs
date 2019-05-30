using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    class VSimpleGauge : VPanel
    {
        private MainWindow mainWindow;

        public VSimpleGauge(MainWindow mainWindow, PanelSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.mainWindow = mainWindow;
        }

        public override void GeneratePanel()
        {

        }
    }
}
