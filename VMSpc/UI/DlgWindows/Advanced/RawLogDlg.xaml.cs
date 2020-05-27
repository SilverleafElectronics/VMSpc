using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using static VMSpc.Constants;
using VMSpc.Communication;
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;
using VMSpc.Loggers;
using VMSpc.Exceptions;

namespace VMSpc.UI.DlgWindows.Advanced
{
    /// <summary>
    /// Interaction logic for RawLogDlg.xaml
    /// </summary>
    public partial class RawLogDlg : VMSDialog
    {
        private static bool LogRecordingEnabled = false;
        private SettingsContents Settings = ConfigurationManager.ConfigManager.Settings.Contents;
        public RawLogDlg()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SetLogButtonText();
            SetCurrentLogFileText();
        }

        private void UseRaw_Click(object sender, RoutedEventArgs e)
        {
            //commreader.LogType = LOGTYPE_RAWLOG;
        }

        private void UseParseReady_Click(object sender, RoutedEventArgs e)
        {
            //commreader.LogType = LOGTYPE_PARSEREADY;
        }

        private void UseFullData_Click(object sender, RoutedEventArgs e)
        {
            //commreader.LogType = LOGTYPE_FULL;
        }

        private void ChangeLogFile_Click(object sender, RoutedEventArgs e)
        {
            //"Text files (*.txt)|*.txt|All files (*.*)|*.*";
            var dlg = new FileSelector("\\rawlogs", CurrentLogFile.Text, "vms")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExcludeLockedFiles = true,
                AllowNewFiles = true,
                NewFilesExtension = ".vms",
                AllowImports = true,
                ImportFilter = "VMS Log Files (*.vms)|*.vms",
            };
            if ((bool)dlg.ShowDialog())
            {
                bool okayToUseFile = true;
                var newFilePath = dlg.ResultFilePath;
                if (!FileOpener.IsFileEmpty(newFilePath))
                {
                    string messageBoxText = FileOpener.GetFileName(newFilePath) + " already exists. Logging to this file will overwrite it. Do you want to continue?";
                    string caption = "VMS Logging";
                    MessageBoxButton yesNoBtn = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult messageResult = MessageBox.Show(messageBoxText, caption, yesNoBtn, icon);
                    okayToUseFile = (messageResult == MessageBoxResult.Yes);
                }
                if (okayToUseFile)
                {
                    Settings.rawLogFilePath = newFilePath;
                    SetCurrentLogFileText();
                }
            }
        }

        private void ToggleLog_Click(object sender, RoutedEventArgs e)
        {
            if (LogRecordingEnabled)
            {
                LogRecordingEnabled = false;
                RawLogger.Instance.Stop();
            }
            else if (!LogRecordingEnabled)
            {
                LogRecordingEnabled = true;
                //empty the file contents
                try
                {
                    MessageBoxResult messageResult = MessageBox.Show("Do you want to erase existing file contents before logging?", "Start Raw Log", MessageBoxButton.YesNo);
                    if (messageResult == MessageBoxResult.Yes)
                    {
                        FileOpener.WriteAllText(Settings.rawLogFilePath, string.Empty);
                    }
                    RawLogger.Instance.Start();
                    MessageBox.Show("Logging Initiated");
                }
                catch (IOException)
                {
                    LogRecordingEnabled = false;
                    MessageBox.Show($"The file {Settings.rawLogFilePath} cannot be written to. It is either being used by a different process, or it does not exist.\n" +
                        $"Verify that it exists by clicking the Change Log File button and viewing the available Raw Log files.\n" +
                        $"To verify that it is not being used by VMSpc, go to Advanced->Communications and check that it is not in use by the Log Player");
                }
                catch (Exception ex)
                {
                    ErrorLogger.GenerateErrorRecord(ex);
                }
            }
            SetLogButtonText();
        }

        private void SetLogButtonText()
        {
            ToggleLog.Content = (LogRecordingEnabled) ? "Stop Logging" : "Start Logging";
        }

        private void SetCurrentLogFileText()
        {
            CurrentLogFile.Text = Settings.rawLogFilePath;
        }
    }
}
