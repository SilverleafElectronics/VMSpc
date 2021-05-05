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
    public partial class ClockDlg : VPanelDlg
    {
        protected new ClockSettings panelSettings;
        protected override int DefaultPanelWidth => 650;
        protected override int DefaultPanelHeight => 150;
        public ClockDlg(ClockSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            BackgroundColor = panelSettings.BackgroundColor;
            BorderColor = panelSettings.BorderColor;
            UseGlobalColor.IsChecked = panelSettings.useGlobalColorPalette;
            UseMilitaryTime.IsChecked = panelSettings.useMilitaryTime;
            ShowDate.IsChecked = panelSettings.showDate;
            ShowAmPm.IsChecked = panelSettings.showAmPm;
            SetColorRects();
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = Enums.UI.PanelType.CLOCK;
            panelSettings.useMilitaryTime = false;
            panelSettings.showAmPm = false;
            panelSettings.showDate = false;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = base.panelSettings as ClockSettings;
        }

        protected override void ApplyGlobalColorPalette()
        {
            base.ApplyGlobalColorPalette();

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.BackgroundColor = BackgroundColor;
            panelSettings.BorderColor = BorderColor;
            panelSettings.useGlobalColorPalette = (bool)UseGlobalColor.IsChecked;
            panelSettings.useMilitaryTime = (bool)UseMilitaryTime.IsChecked;
            panelSettings.showDate = (bool)ShowDate.IsChecked;
            panelSettings.showAmPm = (bool)ShowAmPm.IsChecked;
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
            SetColorRects();
        }

        private void UseGlobalColor_Unchecked(object sender, RoutedEventArgs e)
        {
            BackgroundColor = panelSettings.BackgroundColor;
            BorderColor = panelSettings.BorderColor;
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
            BorderColorRect.Fill = new SolidColorBrush(BorderColor);
        }
    }
}
