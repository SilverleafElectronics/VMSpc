using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using VMSpc.DevHelpers;
using VMSpc.Parsers;

namespace VMSpc.Panels
{
    class VSimpleGauge : VBarGauge
    {
        private MainWindow mainWindow;
        private ushort pid;

        public VSimpleGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.mainWindow = mainWindow;
            dlgWindow = new SimpleGaugeDlg();
            pid = (ushort)panelSettings.PID;
        }

        public override void GeneratePanel()
        {
            base.GeneratePanel();
        }

        public override void UpdatePanel()
        {
            double newValue = GetPidValue(pid);
            UpdateFillBar((int)newValue);
            UpdateValueText((int)newValue);
        }
    }
}
