using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.DevHelpers;
using VMSpc.Parsers;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using System.Windows.Shapes;
using VMSpc.UI.ComponentWrappers;
using System.Windows.Media;

namespace VMSpc.Panels
{
    public class SimpleGauge : VPanel
    {
        protected new SimpleGaugeSettings panelSettings;
        protected BarGauge barGauge;

        public SimpleGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings) 
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            canvas.Background = new SolidColorBrush(panelSettings.BackgroundColor);
            barGauge = new BarGauge(panelSettings)
            {
                Width = canvas.Width,
                Height = canvas.Height,
                pid = panelSettings.pid,
                showGraph = panelSettings.showGraph,
                showName = panelSettings.showName,
                showAbbreviation = panelSettings.showAbbreviation,
                showSpot = panelSettings.showSpot,
                showUnit = panelSettings.showUnit,
                showValue = panelSettings.showValue
            };
            canvas.Children.Add(barGauge);
            barGauge.Draw();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new SimpleGaugeDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
        }
    }
}
