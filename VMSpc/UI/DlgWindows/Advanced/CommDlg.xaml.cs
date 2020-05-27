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
        public int testSelection;
        private SettingsContents Settings = ConfigManager.Settings.Contents;

        //private int dataReaderType;
        
        public CommDlg() : base()
        {
            InitializeComponent();
            PopulateComboBox<JibType>(CommSelection);
            PopulateComboBox<ParseBehavior>(ParsingBehavior);
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            //TODO
            //CreateBinding("MessageCount", commreader, BindingMode.OneWay, GoodPacketCount, Label.ContentProperty);
            //CreateBinding("BadMessageCount", commreader, BindingMode.OneWay, BadPacketCount, Label.ContentProperty);
            //CreateBinding(GoodPacketCount, "Content", commreader, "MessageCount", ONE_WAY, true);

            LogPlayerFileName.Text = Settings.jibPlayerFilePath;
            CommSelection.SelectedIndex = (int)Settings.jibType;
            PortSelection.SelectedIndex = Settings.comPort - 1;
            ParsingBehavior.SelectedIndex = (int)Settings.globalParseBehavior;
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
            //((MainWindow)Owner).InitializeComm();
            //TODO
            CommunicationManager.Instance.RestartComm();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool shouldRestart = false;
            if (
                Settings.jibType != (JibType)CommSelection.SelectedIndex ||
                Settings.comPort != (ushort)(PortSelection.SelectedIndex + 1) ||
                Settings.jibPlayerFilePath != LogPlayerFileName.Text
                )
            {
                shouldRestart = true;
            }
            Settings.jibPlayerFilePath = LogPlayerFileName.Text;
            Settings.globalParseBehavior = (ParseBehavior)ParsingBehavior.SelectedIndex;
            Settings.comPort = (ushort)(PortSelection.SelectedIndex + 1);
            Settings.jibType = (JibType)CommSelection.SelectedIndex;
            DialogResult = true;
            if (shouldRestart)
            {
                CommunicationManager.Instance.RestartComm();
            }
            Close();
        }
    }
}
