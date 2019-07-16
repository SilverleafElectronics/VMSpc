using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMSpc.XmlFileManagers;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for MultiBarDlg.xaml
    /// </summary>
    public partial class MultiBarDlg : VPanelDlg
    {
        public MultiBarDlg(PanelSettings panelSettings)
            :base(panelSettings)
        {
            InitializeComponent();
        }

        protected override void ApplyDefaults()
        {
            throw new NotImplementedException();
        }
    }
}
