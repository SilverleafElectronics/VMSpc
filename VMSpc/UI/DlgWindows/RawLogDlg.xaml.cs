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

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for RawLogDlg.xaml
    /// </summary>
    public partial class RawLogDlg : VMSDialog
    {
        private VMSComm commreader;
        public RawLogDlg(VMSComm commreader)
        {
            this.commreader = commreader;
            Owner = Application.Current.MainWindow;
            InitializeComponent();
        }

        private void UseRaw_Click(object sender, RoutedEventArgs e)
        {
            commreader.LogType = LOGTYPE_RAWLOG;
        }

        private void UseParseReady_Click(object sender, RoutedEventArgs e)
        {
            commreader.LogType = LOGTYPE_PARSEREADY;
        }

        private void UseFullData_Click(object sender, RoutedEventArgs e)
        {
            commreader.LogType = LOGTYPE_FULL;
        }

        private void ChangeLogFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                DefaultExt = ".vms",
                Filter = "VMS Log Files (*.vms)|*.vms",
                InitialDirectory = Directory.GetCurrentDirectory(),
                OverwritePrompt = false
            };

            bool? result = dlg.ShowDialog();
            bool okayToUseFile = true;
            if (result == true)
            {

                if (!File.Exists(dlg.FileName))
                {
                    FileStream fs = File.Create(dlg.FileName);
                    fs.Close();
                }
                else
                {
                    string messageBoxText = "\"" + dlg.SafeFileName + "\" already exists. Logging to this file will overwrite it. Do you want to continue?";
                    string caption = "VMS Logging";
                    MessageBoxButton yesNoBtn = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult messageResult = MessageBox.Show(messageBoxText, caption, yesNoBtn, icon);
                    okayToUseFile = (messageResult == MessageBoxResult.Yes);
                }

                if (okayToUseFile)
                {
                    commreader.LogRecordingFile = dlg.FileName;
                    CurrentLogFile.Text = dlg.SafeFileName;
                }
            }
        }

        private void ToggleLog_Click(object sender, RoutedEventArgs e)
        {
            if (commreader.LogRecordingEnabled)
            {
                commreader.LogRecordingEnabled = false;
                ToggleLog.Content = "Start Logging";
                MessageBox.Show("Logging Stopped");
            }
            else if (!commreader.LogRecordingEnabled)
            {
                commreader.LogRecordingEnabled = true;
                ToggleLog.Content = "Stop Logging";
                MessageBox.Show("Logging Initiated");
                //empty the file contents
                File.WriteAllText(commreader.LogRecordingFile, string.Empty);
            }
        }
    }
}
