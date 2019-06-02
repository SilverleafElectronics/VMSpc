using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    /// <summary>
    /// base class of VSimpleGauge and VScanGauge
    /// </summary>
    class VBarGauge : VPanel
    {
        public VBarGauge(MainWindow mainWindow, PanelSettings panelSettings)
        : base(mainWindow, panelSettings)
        { }

        public override void GeneratePanel() { }
    }
}
