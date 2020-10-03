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

namespace VMSpc
{
    public static class Snapshotter
    {
        public static void ZipConfiguration()
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
        }

        private static void CreateSnapshot(string destinationDirectory)
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
                    var fullName = entry.FullName.ToLower();
                    if (!(
                        (fullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".eng", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".vms", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)) ||
                        (fullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        )
                        )
                    {
                        entriesToDelete.Add(entry);
                    }
                }
                for (int i = 0; i < entriesToDelete.Count(); i++)
                {
                    entriesToDelete[i].Delete();
                }
            }
            MessageBox.Show($"Successfully created the snapshot \"{zipFileName}\", which is placed in \"{destinationDirectory}\"");
        }

        private static void ProcessRetry(string attemptedDirectory)
        {
            var result = MessageBox.Show($"Failed to save the snapshot to the directory: {attemptedDirectory}. The path may be invalid, or VMSpc may not have permission to access the specified directory. " +
                $" Do you want to try again?", "Retry", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                ZipConfiguration();
        }

        private static bool ShouldSaveToDesktop()
        {
            var result = MessageBox.Show("Do you want to save the zip file to your Desktop? If you select 'No', a file explorer will allow you to choose " +
                "the file location. Note that you cannot save this file to your VMSpc base directory.", "Save to Desktop", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        private static string GetDesiredDirectory()
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
