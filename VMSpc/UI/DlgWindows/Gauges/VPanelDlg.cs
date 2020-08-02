using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMSpc.JsonFileManagers;
using VMSpc.Enums.UI;
using static VMSpc.Constants;
using System.Windows;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows
{
    public abstract class VPanelDlg : VMSDialog
    {
        public PanelSettings panelSettings;
        /// <summary>
        /// 
        /// </summary>
        protected Color BackgroundColor;
        /// <summary>
        /// 
        /// </summary>
        protected Color BorderColor;
        /// <summary>
        /// 
        /// </summary>
        protected Color TextColor;
        /// <summary>
        /// 
        /// </summary>
        protected Color ValueTextColor;

        protected virtual int DefaultPanelWidth => 300;
        protected virtual int DefaultPanelHeight => 300;

        
        public VPanelDlg(PanelSettings panelSettings) : base()
        {
            this.panelSettings = panelSettings;
            Init(panelSettings);
            if (this.panelSettings.panelId == PanelType.NONE)
                ApplyDefaults();
        }

        protected abstract void Init(PanelSettings panelSettings);

        protected virtual void ApplyDefaults()
        {
            
            panelSettings.panelCoordinates.topLeftX = 0;
            panelSettings.panelCoordinates.topLeftY = 0;
            panelSettings.panelCoordinates.bottomRightX = DefaultPanelWidth;
            panelSettings.panelCoordinates.bottomRightY = DefaultPanelHeight;
            panelSettings.parentPanelNumber = 0;
            panelSettings.showInMetric = false;
            panelSettings.alignment = System.Windows.HorizontalAlignment.Left;
            panelSettings.useGlobalColorPalette = true;
            panelSettings.BorderColor = BorderColor = Colors.Black;
            panelSettings.BackgroundColor = BackgroundColor = ConfigManager.ColorPalettes.GetSelectedPalette().GaugeBackground;
            panelSettings.CaptionColor = TextColor = ConfigManager.ColorPalettes.GetSelectedPalette().Captions;
            panelSettings.ValueTextColor = ValueTextColor = ConfigManager.ColorPalettes.GetSelectedPalette().GaugeText;

            ApplyGlobalColorPalette();

            //panelSettings = panelSettings.GetDefaults();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            BackgroundColor = panelSettings.BackgroundColor;
            BorderColor = panelSettings.BorderColor;
            TextColor = panelSettings.CaptionColor;
            ValueTextColor = panelSettings.ValueTextColor;
        }

        protected virtual void ApplyGlobalColorPalette()
        {
            BorderColor = Colors.Black;
            BackgroundColor = ConfigManager.ColorPalettes.GetSelectedPalette().GaugeBackground;
            TextColor = ConfigManager.ColorPalettes.GetSelectedPalette().Captions;
            ValueTextColor = ConfigManager.ColorPalettes.GetSelectedPalette().GaugeText;
        }

        protected virtual void RevertGlobalColorPalette()
        {
            BackgroundColor = panelSettings.BackgroundColor;
            BorderColor = panelSettings.BorderColor;
            TextColor = panelSettings.CaptionColor;
            ValueTextColor = panelSettings.ValueTextColor;
        }

        /*
        protected virtual void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundColor = panelSettings.backgroundColor;
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
            }
        }

        protected virtual void ChangeBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            BorderColor = panelSettings.borderColor;
            ChangeColor(ref BorderColor);
        }
        */
    }
}
