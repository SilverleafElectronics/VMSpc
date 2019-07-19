using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;
using VMSpc.CustomComponents;
using System.Windows.Media;
using System.Windows.Controls;

namespace VMSpc.Panels
{
    class VRoundGauge : VPanel
    {
        Shape roundBar;
        RoundGaugeSettings panelSettings;

        public VRoundGauge(MainWindow mainWindow, RoundGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        public override void GeneratePanel()
        {

        }

        public override void UpdatePanel()
        {

        }

        protected override VMSDialog GenerateDlg()
        {
            return new RoundGaugeDlg(panelSettings);
        }
    }
}
