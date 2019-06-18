using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMSpc.Communication;
using static VMSpc.Constants;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for CommDlg.xaml
    /// </summary>
    public partial class CommDlg : Window
    {
        private VMSComm commreader;
        Timer bindingTimer;
        public CommDlg(VMSComm commreader)
        {
            this.commreader = commreader;
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            bindingTimer = CREATE_TIMER(ApplyDataBindings, 5000);
            ApplyDataBindings(null, null);
        }

        private void ApplyDataBindings(Object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                CommSelection.SelectedIndex = commreader.dataReaderType;
            });
        }

        private void ComboBox_SelectionChanged_PORT(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ComboBox_SelectionChanged_COMTYPE(object sender, SelectionChangedEventArgs e)
        {
            commreader.ChangeDataReader(CommSelection.SelectedIndex);
        }
    }
}
