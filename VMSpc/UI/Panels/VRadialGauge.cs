using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.Panels
{
    class VRadialGauge : VPanel
    {
        protected new SimpleGaugeSettings panelSettings;
        protected RadialComponent radialComponent;
        protected JParameter parameter;
        public VRadialGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            parameter = ConfigManager.ParamData.GetParam(panelSettings.pid);
        }
        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            radialComponent = new RadialComponent(panelSettings.pid, parameter.GaugeMin, parameter.GaugeMax)
            {
                Width = canvas.Width,
                Height = canvas.Height,
            };
            canvas.Children.Add(radialComponent);
            radialComponent.Draw();
        }

        public override void UpdatePanel()
        {
        }
        
        protected override VMSDialog GenerateDlg()
        {
            return new SimpleGaugeDlg(panelSettings);
        }
    }
}
