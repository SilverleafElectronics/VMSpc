using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
using System.Windows.Threading;
using VMSpc.Loggers;

namespace VMSpc.UI
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        private DispatcherUnhandledExceptionEventArgs exception;
        public ExceptionWindow(DispatcherUnhandledExceptionEventArgs exception)
        {
            this.exception = exception;
            InitializeComponent();
            ExceptionHeader.Text = $"An unhandled exception just occurred: {exception.Exception.Message}" +
                                    "\nPlease select \"Copy\" if you want to copy the message to send it to us." +
                                    "\nSelect \"Close\" to stop the application.\n\n" +
                                    "NOTE: If this error continues and prevents you from using VMSpc, please select 'Copy Zip'. This will create a snapshot zip file\n" +
                                    "           of your program's configuration, which you can send to us for troubleshooting.";
            ExceptionBody.Text = exception.Exception.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (exception != null)
            {
                Clipboard.SetText(exception.ToString());
            }
        }

        private void ZipButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessZip();
        }

        private void ProcessZip()
        {
            string directoryPath;
            if (ShouldSaveToDesktop())
            {
                directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                directoryPath = GetDesiredDirectory();
            }

            if (!string.IsNullOrEmpty(directoryPath))
            {
                try
                {
                    CreateSnapshot(directoryPath);
                }
                catch
                {
                    ProcessRetry(directoryPath);
                }
            }
            else
            {
                ProcessRetry("Empty Path");
            }
            Close();
        }

        private void CreateSnapshot(string destinationDirectory)
        {
            var zipFileName = $"VMSSnapshot_{Environment.UserName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
            var zipPath = destinationDirectory + "\\" + zipFileName;
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(Constants.BaseDirectory, zipPath);
            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
            {
                List<ZipArchiveEntry> entriesToDelete = new List<ZipArchiveEntry>();
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        entriesToDelete.Add(entry);
                    }
                }
                for (int i = 0; i < entriesToDelete.Count(); i++)
                {
                    entriesToDelete[i].Delete();
                }
            }
            AddErrorFileToSnapshot(zipPath);
            MessageBox.Show($"Successfully created the snapshot \"{zipFileName}\", which is placed in \"{destinationDirectory}\"");
        }

        /// <summary>
        /// Creates an error file, copies the file to the zip archive, then deletes the original error file.
        /// </summary>
        /// <param name="zipPath"></param>
        private void AddErrorFileToSnapshot(string zipPath)
        {
            var errorFilePath = Constants.BaseDirectory + "\\SnapshotErrorFile.txt";
            if (File.Exists(errorFilePath))
            {
                File.Delete(errorFilePath);
            }
            File.WriteAllText(errorFilePath, exception.Exception.ToString());
            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
            {
                archive.CreateEntryFromFile(errorFilePath, "SnapshotErrorFile.txt");
            }
            if (File.Exists(errorFilePath))
            {
                File.Delete(errorFilePath);
            }
        }

        private void ProcessRetry(string attemptedDirectory)
        {
            var result = MessageBox.Show($"Failed to save the snapshot to the directory: {attemptedDirectory}. The path may be invalid, or VMSpc may not have permission to access the specified directory. " +
                $" Do you want to try again?", "Retry", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                ProcessZip();

        }

        private bool ShouldSaveToDesktop()
        {
            var result = MessageBox.Show("Do you want to save the zip file to your Desktop? If you select 'No', a file explorer will allow you to choose " +
                "the file location. Note that you cannot save this file to your VMSpc base directory.", "Save to Desktop", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        private string GetDesiredDirectory()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                InitialDirectory = System.IO.Path.GetPathRoot(Environment.SystemDirectory),
            };
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
