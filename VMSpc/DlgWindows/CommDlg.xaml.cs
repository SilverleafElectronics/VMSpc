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
            CreateBinding("MessageCount", commreader, BindingMode.OneWay, GoodPacketCount, Label.ContentProperty);
            CreateBinding("BadMessageCount", commreader, BindingMode.OneWay, BadPacketCount, Label.ContentProperty);
            LogPlayerFileName.Text = Settings.LogPlayerFileName;
            CommSelection.SelectedIndex = Settings.JibType;
            PortSelection.SelectedIndex = Settings.Port - 1;
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
                LogPlayerFileName.Text = dlg.FileName;
            }
        }

        private void RestartComm_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Owner).InitializeComm();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.LogPlayerFileName = LogPlayerFileName.Text;
            Settings.ParseMode = ParsingBehavior.SelectedIndex;
            Settings.Port = PortSelection.SelectedIndex + 1;
            DialogResult = true;
            Close();
        }
    }
}
