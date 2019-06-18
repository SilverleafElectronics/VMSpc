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
using VMSpc.Communication;
using static VMSpc.Constants;
using VMSpc.DevHelpers;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for CommDlg.xaml
    /// </summary>
    public partial class CommDlg : VMSDialog
    {
        private VMSComm commreader;
        private bool CommTypeSelectionInProgress;
        
        public CommDlg(VMSComm commreader)
        {
            this.commreader = commreader;
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            CommTypeSelectionInProgress = false;
        }

        protected override void BindData()
        {
            //CommSelection.SelectedIndex = commreader.DataReaderType;
        }

        private void ComboBox_SelectionChanged_PORT(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ComboBox_SelectionChanged_COMTYPE(object sender, SelectionChangedEventArgs e)
        {
            if (CommTypeSelectionInProgress)
            {
                commreader.ChangeDataReader(CommSelection.SelectedIndex);
                CommTypeSelectionInProgress = false;
            }
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            CommTypeSelectionInProgress = true;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            CommTypeSelectionInProgress = false;
        }
    }
}
