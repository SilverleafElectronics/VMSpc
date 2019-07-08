using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;

namespace VMSpc.DlgWindows
{
    public class VPanelDlg : VMSDialog
    {
        public PanelSettings panelSettings;
        
        public VPanelDlg(PanelSettings panelSettings) : base()
        {
            this.panelSettings = panelSettings;
        }
    }
}
