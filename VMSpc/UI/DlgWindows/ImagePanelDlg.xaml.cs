using Microsoft.Win32;
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
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for ImagePanelDlg.xaml
    /// </summary>
    public partial class ImagePanelDlg : VPanelDlg
    {
        private new PictureSettings panelSettings;
        public ImagePanelDlg(PictureSettings panelSettings)
            :base(panelSettings)

        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            ((PictureSettings)panelSettings).bmpFilePath = null;
            panelSettings.panelId = VEnum.UI.PanelType.IMAGE;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            ImagePath.Text = panelSettings.bmpFilePath;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (PictureSettings)panelSettings;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.bmpFilePath = ImagePath.Text;
            DialogResult = true;
            Close();
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".bmp",
                Filter = "Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"
            };
            if (dlg.ShowDialog() == true)
                ImagePath.Text = dlg.FileName;
        }
    }
}
