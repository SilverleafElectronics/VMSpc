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
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Enums.Parsing;
using VMSpc.Enums;
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for CommDlg.xaml
    /// </summary>
    public partial class CommDlg : VMSDialog
    {
        private VMSComm commreader;
        public int testSelection;

        //private int dataReaderType;
        
        public CommDlg(VMSComm commreader) : base()
        {
            this.commreader = commreader;
            InitializeComponent();
            PopulateComboBox<JibType>(CommSelection);
            PopulateComboBox<ParseBehavior>(ParsingBehavior);
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            //CreateBinding("MessageCount", commreader, BindingMode.OneWay, GoodPacketCount, Label.ContentProperty);
            //CreateBinding("BadMessageCount", commreader, BindingMode.OneWay, BadPacketCount, Label.ContentProperty);
            //CreateBinding(GoodPacketCount, "Content", commreader, "MessageCount", ONE_WAY, true);

            LogPlayerFileName.Text = ConfigManager.Settings.Contents.jibPlayerFilePath;
            CommSelection.SelectedIndex = (int)ConfigManager.Settings.Contents.jibType;
            PortSelection.SelectedIndex = ConfigManager.Settings.Contents.comPort - 1;
            ParsingBehavior.SelectedIndex = (int)ConfigManager.Settings.Contents.globalParseBehavior;
        }

        private void ChangeLogPlayerFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FileSelector("\\rawlogs", LogPlayerFileName.Text, "vms")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExcludeLockedFiles = true,
            };
            if ((bool)dlg.ShowDialog())
            {
                LogPlayerFileName.Text = dlg.ResultFilePath;
            }
            /*OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".vms",
                Filter = "VMS Log Files (*.vms)|*.vms",
                InitialDirectory = RawLogOpener.RawLogBaseDirectory,
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                LogPlayerFileName.Text = dlg.FileName;
            }*/
        }

        private void RestartComm_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Owner).InitializeComm();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.Settings.Contents.jibPlayerFilePath = LogPlayerFileName.Text;
            ConfigManager.Settings.Contents.globalParseBehavior = (ParseBehavior)ParsingBehavior.SelectedIndex;
            ConfigManager.Settings.Contents.comPort = (ushort)(PortSelection.SelectedIndex + 1);
            ConfigManager.Settings.Contents.jibType = (JibType)CommSelection.SelectedIndex;
            DialogResult = true;
            Close();
        }
    }
}
