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
using Microsoft.Win32;
using System.IO;
using static VMSpc.XmlFileManagers.SettingsManager;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for CommDlg.xaml
    /// </summary>
    public partial class CommDlg : VMSDialog
    {
        private VMSComm commreader;
        public int testSelection;

        private int dataReaderType;
        
        public CommDlg(VMSComm commreader) : base()
        {
            this.commreader = commreader;
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            CreateBinding("DataReaderType", commreader, BindingMode.TwoWay, CommSelection, ComboBox.SelectedIndexProperty);
            CreateBinding("ComPort", commreader, BindingMode.TwoWay, PortSelection, ComboBox.SelectedIndexProperty);
            CreateBinding("MessageCount", commreader, BindingMode.OneWay, GoodPacketCount, Label.ContentProperty);
            CreateBinding("BadMessageCount", commreader, BindingMode.OneWay, BadPacketCount, Label.ContentProperty);
            //CreateBinding("ParseBehavior", commreader, BindingMode.TwoWay, ParsingBehavior, ComboBox.SelectedIndexProperty);
            CreateBinding("LogFile", commreader, BindingMode.OneWay, LogPlayerFileName, TextBox.TextProperty);
            ParsingBehavior.SelectedIndex = Settings.ParseMode;
        }

        private void ChangeLogPlayerFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".vms",
                Filter = "VMS Log Files (*.vms)|*.vms",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                commreader.LogPlayerFile = dlg.FileName;
                LogPlayerFileName.Text = dlg.FileName;
            }
        }

        private void RestartComm_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Owner).InitializeComm();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ParseMode = ParsingBehavior.SelectedIndex;
            DialogResult = true;

        }
    }
}
