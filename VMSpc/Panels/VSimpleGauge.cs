using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using VMSpc.DevHelpers;
using static VMSpc.Parsers.PIDWrapper;

namespace VMSpc.Panels
{
    class VSimpleGauge : VBarGauge
    {
        private MainWindow mainWindow;
        private int pid;

        public VSimpleGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.mainWindow = mainWindow;
            pid = panelSettings.PID;
        }

        public override void GeneratePanel()
        {
            base.GeneratePanel();
        }

        public override void UpdatePanel()
        {
            double newValue = PIDManager.PIDList[(byte)pid].standardValue;
            UpdateFillBar(newValue);
        }
    }
}
