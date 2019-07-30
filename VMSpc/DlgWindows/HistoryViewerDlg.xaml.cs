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
using VMSpc.DevHelpers;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for HistoryViewerDlg.xaml
    /// </summary>
    public partial class HistoryViewerDlg : Window
    {
        private string fileName;
        public HistoryViewerDlg(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;
            Title = "Odometer Trip History Viewer for \"" + fileName.Substring(fileName.LastIndexOf("/") + 1) + "\""; 
            PopulateFileContents();
            OdometerHistoryContent.MouseEnter += OdometerHistoryContent_MouseEnter;
            OdometerHistoryContent.MouseLeave += OdometerHistoryContent_MouseLeave;
        }

        private void OdometerHistoryContent_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void OdometerHistoryContent_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.IBeam;
        }

        private void PopulateFileContents()
        {
            string[] historyContents = File.ReadAllLines(fileName);
            for (int i = 2; i < historyContents.Length; i++)
                OdometerHistoryContent.AppendText(historyContents[i] + "\n");
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string newFileText = "Trip History File\n" +
                                 "Feel free to add notes to this file, but do not move or delete.\n" +
                                 OdometerHistoryContent.Text;
            File.WriteAllText(fileName, newFileText);
            Close();
        }
    }
}
