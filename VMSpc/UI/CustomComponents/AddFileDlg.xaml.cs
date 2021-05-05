using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VMSpc.UI.CustomComponents
{
    /// <summary>
    /// Interaction logic for AddFileDlg.xaml
    /// </summary>
    public partial class AddFileDlg : Window
    {
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string ReservedWindowsSymbolsRegex = "[/?%*:|\"<>.\\\\]";
        public AddFileDlg()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            FileNameText.Text = FileName;
            ExtensionText.Text = Extension;
            base.OnActivated(e);
            Keyboard.Focus(FileNameText);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ContainsReservedCharacters())
            {
                FileName = FileNameText.Text;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("The file name contains invalid characters. Windows file names may not contain \"., /, ?, %, *, :, |, \", <, >, or \\\"");
            }
        }

        private bool ContainsReservedCharacters()
        {
            Regex regex = new Regex(ReservedWindowsSymbolsRegex);
            MatchCollection matches = regex.Matches(FileNameText.Text);
            return (matches.Count > 0);

        }
    }
}
