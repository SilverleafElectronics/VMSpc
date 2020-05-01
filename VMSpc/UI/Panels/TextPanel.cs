using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Extensions.UI;
using System.Windows.Media;
using System.Windows;

namespace VMSpc.UI.Panels
{
    class TextPanel : VPanel
    {
        TextBlock textBlock;
        protected new TextGaugeSettings panelSettings;
        public TextPanel(MainWindow mainWindow, TextGaugeSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }
        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            if (panelSettings.useGlobalColorPalette)
            {
                canvas.Background = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().GaugeBackground);
            }
            else
            {
                canvas.Background = new SolidColorBrush(panelSettings.backgroundColor);
            }
            textBlock = new TextBlock()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Text = panelSettings.text,
                TextAlignment = panelSettings.alignment.ToHorizontalAlignment(),
                Foreground = new SolidColorBrush(panelSettings.captionColor),
                //TextWrapping = TextWrapping.Wrap,//(panelSettings.wrapText) ? TextWrapping.Wrap : TextWrapping.NoWrap,
            };
            canvas.Children.Add(textBlock);
            canvas.ScaleText(textBlock);
        }

        public override void UpdatePanel()
        {
        }

        protected override VMSDialog GenerateDlg()
        {
            return new TextPanelDlg((TextGaugeSettings)panelSettings);
        }
    }
}
