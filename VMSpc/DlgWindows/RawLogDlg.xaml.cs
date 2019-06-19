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

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for RawLogDlg.xaml
    /// </summary>
    public partial class RawLogDlg : VMSDialog
    {
        public RawLogDlg()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
        }
    }
}
