using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Enums.UI;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for TransmissionGaugeDlg.xaml
    /// </summary>
    public partial class TransmissionGaugeDlg : VPanelDlg
    {
        protected new TransmissionGaugeSettings panelSettings;
        public TransmissionGaugeDlg(TransmissionGaugeSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = PanelType.TRANSMISSION_GAUGE;
            panelSettings.alignment = HorizontalAlignment.Center;
            panelSettings.showAttained = true;
            panelSettings.showSelected = true;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            ((RadioButton)RadioAlignment.Children[(int)panelSettings.alignment]).IsChecked = true;
            ShowSelected_checkbox.IsChecked = panelSettings.showSelected;
            ShowAttained_checkbox.IsChecked = panelSettings.showAttained;
            UseGlobalColor.IsChecked = panelSettings.useGlobalColorPalette;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (TransmissionGaugeSettings)panelSettings;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton button in RadioAlignment.Children)
                if (button.IsChecked == true) panelSettings.alignment = (HorizontalAlignment)Convert.ToInt16(button.Tag);    //TODO: SimpleGaugeDlg:1
            panelSettings.showSelected = (bool)ShowSelected_checkbox.IsChecked;
            panelSettings.showAttained = (bool)ShowAttained_checkbox.IsChecked;
            panelSettings.useGlobalColorPalette = (bool)UseGlobalColor.IsChecked;
            panelSettings.backgroundColor = BackgroundColor;
            panelSettings.borderColor = BorderColor;
            panelSettings.valueTextColor = ValueTextColor;
            DialogResult = true;
            Close();
        }

        private void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void ChangeBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref BorderColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void ChangeTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref ValueTextColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void UseGlobalColor_Checked(object sender, RoutedEventArgs e)
        {
            ApplyGlobalColorPalette();
        }
    }
}
