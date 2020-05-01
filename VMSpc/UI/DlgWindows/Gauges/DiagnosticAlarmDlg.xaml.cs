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
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for DiagnosticAlarmDlg.xaml
    /// </summary>
    public partial class DiagnosticAlarmDlg : VPanelDlg
    {
        protected new DiagnosticGaugeSettings panelSettings;
        protected Color WarningColor;
        public DiagnosticAlarmDlg(DiagnosticGaugeSettings panelSettings)
            :base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            WarningColor = panelSettings.WarningColor;
            BackgroundColor = panelSettings.backgroundColor;
            BorderColor = panelSettings.borderColor;
            UseGlobalColor.IsChecked = panelSettings.useGlobalColorPalette;
            UseMilitaryTime.IsChecked = panelSettings.useMilitaryTime;
            SetColorRects();
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = Enums.UI.PanelType.DIAGNOSTIC_ALARM;
            panelSettings.WarningColor = ConfigManager.ColorPalettes.GetSelectedPalette().Red;
            panelSettings.useMilitaryTime = false;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = base.panelSettings as DiagnosticGaugeSettings;
        }

        protected override void ApplyGlobalColorPalette()
        {
            base.ApplyGlobalColorPalette();

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.backgroundColor = BackgroundColor;
            panelSettings.borderColor = BorderColor;
            panelSettings.WarningColor = WarningColor;
            panelSettings.useGlobalColorPalette = (bool)UseGlobalColor.IsChecked;
            panelSettings.useMilitaryTime = (bool)UseMilitaryTime.IsChecked;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void UseGlobalColor_Checked(object sender, RoutedEventArgs e)
        {
            ApplyGlobalColorPalette();
            WarningColor = ConfigManager.ColorPalettes.GetSelectedPalette().Red;
            SetColorRects();
        }

        private void UseGlobalColor_Unchecked(object sender, RoutedEventArgs e)
        {
            BackgroundColor = panelSettings.backgroundColor;
            BorderColor = panelSettings.borderColor;
            WarningColor = panelSettings.WarningColor;
            SetColorRects();
        }

        private void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
                SetColorRects();
            }
        }

        private void ChangeWarningColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref WarningColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
                SetColorRects();
            }
        }

        private void ChangeBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref BorderColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
                SetColorRects();
            }
        }

        private void SetColorRects()
        {
            BackgroundColorRect.Fill = new SolidColorBrush(BackgroundColor);
            WarningColorRect.Fill = new SolidColorBrush(WarningColor);
            BorderColorRect.Fill = new SolidColorBrush(BorderColor);
        }
    }
}
