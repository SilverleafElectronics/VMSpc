﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (this.panelSettings.ID == PanelIDs.NO_ID)
                ApplyDefaults();
        }

        protected abstract void ApplyDefaults();
    }
}
