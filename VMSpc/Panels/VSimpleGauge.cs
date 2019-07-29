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
    class VSimpleGauge : VPanel
    {
        protected new SimpleGaugeSettings panelSettings;
        protected VBarGauge barGauge;

        public VSimpleGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            barGauge = new VBarGauge(new GaugePresenter(panelSettings.PID, panelSettings), canvas.Width, canvas.Height);
            canvas.Children.Add(barGauge);
        }

        protected override VMSDialog GenerateDlg()
        {
            return new SimpleGaugeDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
            barGauge.Update();
        }
    }
}
