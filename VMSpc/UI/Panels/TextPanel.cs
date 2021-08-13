using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Extensions.UI;
using System.Windows.Media;
using System.Windows;
using VMSpc.UI.CustomComponents;

namespace VMSpc.UI.Panels
{
    class TextPanel : VPanel
    {
        GaugeTextBlock textBlock;
        Viewbox viewbox;
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
                canvas.Background = new SolidColorBrush(panelSettings.BackgroundColor);
            }
            viewbox = new Viewbox()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                HorizontalAlignment = panelSettings.alignment,
            };
            textBlock = new GaugeTextBlock()
            {
                Text = panelSettings.text,
                Foreground = new SolidColorBrush(panelSettings.CaptionColor),
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                //Margin = new Thickness(0, 0, 0, 0),
                //TextWrapping = TextWrapping.Wrap,//(panelSettings.wrapText) ? TextWrapping.Wrap : TextWrapping.NoWrap,
            };
            viewbox.Child = textBlock;
            canvas.Children.Add(viewbox);
            textBlock.LineHeight = textBlock.FontSize;
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
