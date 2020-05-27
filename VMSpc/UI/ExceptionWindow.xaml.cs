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
                                    "\nSelect \"Close\" to stop the application.";
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
    }
}
