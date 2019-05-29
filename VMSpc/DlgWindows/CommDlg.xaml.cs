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
    /// Interaction logic for DlgCommunication.xaml
    /// </summary>
    public partial class CommDlg : Window
    {
        public CommDlg()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }
    }
}
