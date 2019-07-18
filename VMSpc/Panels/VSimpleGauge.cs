using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using VMSpc.DevHelpers;
using VMSpc.Parsers;
using VMSpc.DlgWindows;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.Constants;
using System.Windows.Shapes;

namespace VMSpc.Panels
{
    class VSimpleGauge : VBarGauge
    {
        protected ushort pid;
        protected new SimpleGaugeSettings panelSettings;

        public VSimpleGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            FillBar = new Rectangle();
            EmptyBar = new Rectangle();
        }

        public override void GeneratePanel()
        {
            pid = panelSettings.PID;
            parameter = ParamData.parameters[pid];
            base.GeneratePanel();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new SimpleGaugeDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
            double value = Math.Round(GetPidValue(pid), 2, MidpointRounding.AwayFromZero);
            if (value != DUB_NODATA && value != lastValue)
            {
                UpdateFillBar(value);
                UpdateValueText(value);
                lastValue = value;
            }
        }
    }
}
