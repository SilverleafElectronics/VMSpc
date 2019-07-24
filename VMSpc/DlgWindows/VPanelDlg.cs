using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;

namespace VMSpc.DlgWindows
{
    public abstract class VPanelDlg : VMSDialog
    {
        public PanelSettings panelSettings;
        
        public VPanelDlg(PanelSettings panelSettings) : base()
        {
            this.panelSettings = panelSettings;
            Init(panelSettings);
            if (this.panelSettings.ID == PanelIDs.NO_ID)
                ApplyDefaults();
        }

        protected abstract void Init(PanelSettings panelSettings);

        protected virtual void ApplyDefaults()
        {
            panelSettings.rectCord.topLeftX = 0;
            panelSettings.rectCord.topLeftY = 0;
            panelSettings.rectCord.bottomRightX = 300;
            panelSettings.rectCord.bottomRightY = 300;
            panelSettings.showInMetric = false;
            panelSettings.TextPosition = 0;
            panelSettings.Use_Static_Color = 0;
            panelSettings.backgroundColor = Colors.White;
        }
    }
}
