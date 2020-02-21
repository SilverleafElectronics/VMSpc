using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;

namespace VMSpc.UI.Panels
{
    public class MultiBar : VPanel
    {
        protected new MultiBarSettings panelSettings;


        public MultiBar(MainWindow mainWindow, MultiBarSettings panelSettings)
            :base(mainWindow, panelSettings)
        {

        }
        public override void GeneratePanel()
        {
            throw new NotImplementedException();
        }

        public override void UpdatePanel()
        {
            throw new NotImplementedException();
        }

        protected override VMSDialog GenerateDlg()
        {
            throw new NotImplementedException();
        }
    }
}
