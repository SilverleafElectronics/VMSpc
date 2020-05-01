using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using System.IO;

namespace VMSpc.UI.Panels
{
    class ImagePanel : VPanel
    {
        protected new PictureSettings panelSettings;
        public ImagePanel(MainWindow mainWindow, PictureSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }
        public override void GeneratePanel()
        {
            if (!string.IsNullOrEmpty(panelSettings.bmpFilePath) && File.Exists(panelSettings.bmpFilePath))
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
            var uri = new Uri(panelSettings.bmpFilePath, UriKind.Absolute);
            try
            {
                canvas.Children.Add(
                    new Image()
                    {
                        Width = canvas.Width,
                        Height = canvas.Height,
                        Source = new BitmapImage(uri)
                    }
                );
            }
            catch
            {
                DisplayNoImageMessage();
            }
        }

        private void DisplayNoImageMessage()
        {
            canvas.Children.Clear();
            StackPanel stackPanel = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Orientation = Orientation.Vertical
            };
            TextBlock emptyMsg = new TextBlock()
            {
                Width = canvas.Width,
                Height = canvas.Height / 2,
                Text = "No Image to Display",
            };
            TextBlock msgExplanation = new TextBlock()
            {
                Width = canvas.Width,
                Height = canvas.Height / 2,
                Text = GetImageErrorExplanation(),
            };
            stackPanel.Children.Add(emptyMsg);
            stackPanel.Children.Add(msgExplanation);
            canvas.Children.Add(stackPanel);
            canvas.ScaleText(emptyMsg);
            canvas.ScaleText(msgExplanation);
        }

        public string GetImageErrorExplanation()
        {
            if (string.IsNullOrEmpty(panelSettings.bmpFilePath))
            {
                return "The image file path is not specified";
            }
            else if (!File.Exists(panelSettings.bmpFilePath))
            {
                return "The specified file path cannot be found";
            }
            else
            {
                return "The image file cannot be used for unknown reasons";
            }
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
