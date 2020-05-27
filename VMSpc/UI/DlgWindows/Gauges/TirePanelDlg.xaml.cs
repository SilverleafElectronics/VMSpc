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
using VMSpc.DevHelpers;
using VMSpc.UI.CustomComponents;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;
using System.ComponentModel;
using VMSpc.Enums.UI;
using VMSpc.UI.DlgWindows;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for TirePanelDlg.xaml
    /// </summary>
    public partial class TirePanelDlg : VPanelDlg
    {
        protected new TireGaugeSettings panelSettings;
        public TirePanelDlg(TireGaugeSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (TireGaugeSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = PanelType.TIRE_GAUGE;
            panelSettings.showPressure = true;
            panelSettings.showIcon = true;
            panelSettings.detachTowed = false;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            ShowPressureCheckbox.IsChecked = panelSettings.showPressure;
            ShowTireIconCheckbox.IsChecked = panelSettings.showIcon;
            DetachTowVehicleCheckbox.IsChecked = panelSettings.detachTowed;
            UseGlobalColor.IsChecked = panelSettings.useGlobalColorPalette;
            BackgroundColor = panelSettings.backgroundColor;
            BorderColor = panelSettings.borderColor;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UseGlobalColor_Checked(object sender, RoutedEventArgs e)
        {
            ApplyGlobalColorPalette();
        }

        private void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundColor = panelSettings.backgroundColor;
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void BorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            BorderColor = panelSettings.borderColor;
            if (ChangeColor(ref BorderColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.backgroundColor = BackgroundColor;
            panelSettings.borderColor = BorderColor;
            panelSettings.showPressure = (bool)ShowPressureCheckbox.IsChecked;
            panelSettings.showIcon = (bool)ShowTireIconCheckbox.IsChecked;
            panelSettings.detachTowed = (bool)DetachTowVehicleCheckbox.IsChecked;
            panelSettings.useGlobalColorPalette = (bool)UseGlobalColor.IsChecked;
            DialogResult = true;
            Close();
        }
    }


}
