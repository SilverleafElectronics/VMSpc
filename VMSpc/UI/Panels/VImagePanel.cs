using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.Panels
{
    class VImagePanel : VPanel
    {
        public VImagePanel(MainWindow mainWindow, PictureSettings panelSettings)
            :base(mainWindow, panelSettings)
        {

        }
        public override void GeneratePanel()
        {
            if (!string.IsNullOrEmpty(((PictureSettings)panelSettings).bmpFilePath))
            {
                DisplayImage();
            }
            else
            {
                DisplayNoImageMessage();
            }
        }

        private void DisplayImage()
        {
            canvas.Children.Clear();
            var uri = new Uri(((PictureSettings)panelSettings).bmpFilePath, UriKind.Absolute);
            canvas.Children.Add(
                new Image()
                {
                    Width = canvas.Width,
                    Height = canvas.Height,
                    Source = new BitmapImage(uri)
                }
            );
        }

        private void DisplayNoImageMessage()
        {
            canvas.Children.Clear();
            TextBlock emptyMsg = new TextBlock()
            {
                Width = canvas.Width,
                Height = canvas.Height,
            };
            canvas.Children.Add(emptyMsg);
            canvas.ScaleText(emptyMsg);
        }

        public override void UpdatePanel()
        { 
        }

        protected override VMSDialog GenerateDlg()
        {
            return new ImagePanelDlg((PictureSettings)panelSettings);
        }
    }
}
