using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMSpc.JsonFileManagers;
using VMSpc.VEnum.UI;
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
            if (this.panelSettings.panelId == PanelType.NONE)
                ApplyDefaults();
        }

        protected abstract void Init(PanelSettings panelSettings);

        protected virtual void ApplyDefaults()
        {
            
            panelSettings.panelCoordinates.topLeftX = 0;
            panelSettings.panelCoordinates.topLeftY = 0;
            panelSettings.panelCoordinates.bottomRightX = 300;
            panelSettings.panelCoordinates.bottomRightY = 300;
            panelSettings.showInMetric = false;
            panelSettings.alignment = UIPosition.CENTER;
            panelSettings.useGlobalColorPalette = true;
            panelSettings.backgroundColor = Colors.White;
            
            //panelSettings = panelSettings.GetDefaults();
        }
    }
}
