using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using VMSpc.DevHelpers;
using VMSpc.Parsers;
using VMSpc.DlgWindows;

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
        }

        protected override void Init()
        {
            pid = ((SimpleGaugeSettings)panelSettings).PID;
            base.Init();
        }

        public override void GeneratePanel()
        {
            base.GeneratePanel();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new SimpleGaugeDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
            double newValue = GetPidValue(pid);
            UpdateFillBar((int)newValue);
            UpdateValueText((int)newValue);
        }
    }
}
