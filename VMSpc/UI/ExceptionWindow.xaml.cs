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
            CreateErrorFile();
            Snapshotter.ZipConfiguration();
            Close();
        }

        private void CreateErrorFile()
        {
            var errorFilePath = Constants.BaseDirectory + "\\configuration\\SnapshotErrorFile.txt";
            if (File.Exists(errorFilePath))
            {
                File.Delete(errorFilePath);
            }
            File.WriteAllText(errorFilePath, exception.Exception.ToString());
        }
    }
}
