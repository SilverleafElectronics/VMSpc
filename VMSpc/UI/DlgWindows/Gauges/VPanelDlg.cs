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
            panelSettings.panelCoordinates.bottomRightX = 300;
            panelSettings.panelCoordinates.bottomRightY = 300;
            panelSettings.parentPanelNumber = 0;
            panelSettings.showInMetric = false;
            panelSettings.alignment = System.Windows.HorizontalAlignment.Left;
            panelSettings.useGlobalColorPalette = true;
            panelSettings.borderColor = BorderColor = Colors.Black;
            panelSettings.backgroundColor = BackgroundColor = ConfigManager.ColorPalettes.GetSelectedPalette().GaugeBackground;
            panelSettings.captionColor = TextColor = ConfigManager.ColorPalettes.GetSelectedPalette().Captions;
            panelSettings.valueTextColor = ValueTextColor = ConfigManager.ColorPalettes.GetSelectedPalette().GaugeText;

            ApplyGlobalColorPalette();

            //panelSettings = panelSettings.GetDefaults();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            BackgroundColor = panelSettings.backgroundColor;
            BorderColor = panelSettings.borderColor;
            TextColor = panelSettings.captionColor;
            ValueTextColor = panelSettings.valueTextColor;
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
            BackgroundColor = panelSettings.backgroundColor;
            BorderColor = panelSettings.borderColor;
            TextColor = panelSettings.captionColor;
            ValueTextColor = panelSettings.valueTextColor;
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
