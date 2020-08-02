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
using VMSpc.AdvancedParsers;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for TankMinderDlg.xaml
    /// </summary>
    public partial class TankMinderDlg : VPanelDlg
    {
        protected new TankMinderSettings panelSettings;
        protected TankMinderReader TankMinder;
        private bool showInMetric;
        protected override int DefaultPanelHeight => 300;
        protected override int DefaultPanelWidth => 600;
        protected double FuelLevelGallons => (panelSettings.tankSize - ChassisParameters.Instance.CurrentFuelGallons + TankMinder.Contents.StartGallons);
        protected double FuelLevelLiters => (panelSettings.tankSize - ChassisParameters.Instance.CurrentFuelLiters + TankMinder.Contents.StartLiters);
        public TankMinderDlg(TankMinderSettings panelSettings)
            :base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (TankMinderSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.tankSize = 75;
            panelSettings.panelId = Enums.UI.PanelType.TANK_MINDER;
            panelSettings.showInMetric = false;
            panelSettings.showFuel = true;
            panelSettings.showCaptions = true;
            panelSettings.showMilesToEmpty = true;
            panelSettings.showMPG = true;
            panelSettings.showUnits = true;
            panelSettings.useRollingMPG = true;
            panelSettings.orientation = Orientation.Vertical;
            panelSettings.alignment = HorizontalAlignment.Center;
            panelSettings.fileName = TankMinderReader.GetNewFilePath() + ".json";
        }

        protected override void ApplyBindings()
        {
            TankMinder = new TankMinderReader(panelSettings.fileName);
            ShowCaptionsCheckbox.IsChecked = panelSettings.showCaptions;
            ShowUnitsCheckbox.IsChecked = panelSettings.showUnits;
            ShowInMetricCheckbox.IsChecked = panelSettings.showInMetric;
            UseRecentDistance.IsChecked = panelSettings.useRollingMPG;
            showInMetric = panelSettings.showInMetric;
            if (panelSettings.orientation == Orientation.Horizontal)
                HorizontalLayoutRadio.IsChecked = true;
            else
                VerticalLayoutRadio.IsChecked = true;
            switch (panelSettings.alignment)
            {
                case HorizontalAlignment.Left:
                    LeftAlignmentRadio.IsChecked = true;
                    break;
                case HorizontalAlignment.Center:
                    CenterAlignmentRadio.IsChecked = true;
                    break;
                case HorizontalAlignment.Right:
                    RightAlignmentRadio.IsChecked = true;
                    break;
                default:
                    CenterAlignmentRadio.IsChecked = true;
                    break;
            }
            ShowDistanceCheckbox.IsChecked = panelSettings.showMilesToEmpty;
            ShowFuelRemainingCheckbox.IsChecked = panelSettings.showFuel;
            ShowMPGCheckbox.IsChecked = panelSettings.showMPG;
            TankSizeBox.Value = panelSettings.tankSize;
            CurrentLevelBox.Value = (int)((panelSettings.showInMetric) ? FuelLevelGallons : FuelLevelLiters);
            TankMinder.Contents.StartMiles = ChassisParameters.Instance.CurrentMiles;
            TankMinder.Contents.StartKilometers = ChassisParameters.Instance.CurrentKilometers;
        }

        private void FillTankButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentLevelBox.Value = TankSizeBox.Value;
        }

        private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowInMetricCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CurrentLevelBox.Value = (int)(CurrentLevelBox.Value * 3.78541);
            TankSizeBox.Value = (int)(TankSizeBox.Value * 3.78541);
            CurrentLevelLabel.Content = "Current Level (Liters)";
        }

        private void ShowInMetricCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CurrentLevelBox.Value = (int)(CurrentLevelBox.Value / 3.78541);
            TankSizeBox.Value = (int)(TankSizeBox.Value / 3.78541);
            CurrentLevelLabel.Content = "Current Level (Gallons)";
        }

        protected HorizontalAlignment GetAlignment()
        {
            if ((bool)LeftAlignmentRadio.IsChecked)
            {
                return HorizontalAlignment.Left;
            }
            else if ((bool)CenterAlignmentRadio.IsChecked)
            {
                return HorizontalAlignment.Center;
            }
            else if ((bool)RightAlignmentRadio.IsChecked)
            {
                return HorizontalAlignment.Right;
            }
            else
            {
                return HorizontalAlignment.Center;
            }
        }

        protected Orientation GetOrientation()
        {
            return ((bool)VerticalLayoutRadio.IsChecked)
                ? Orientation.Vertical
                : Orientation.Horizontal;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.showCaptions = (bool)ShowCaptionsCheckbox.IsChecked;
            panelSettings.showInMetric = (bool)ShowCaptionsCheckbox.IsChecked;
            panelSettings.showUnits = (bool)ShowUnitsCheckbox.IsChecked;
            panelSettings.useRollingMPG = (bool)UseRecentDistance.IsChecked;
            panelSettings.showMilesToEmpty = (bool)ShowDistanceCheckbox.IsChecked;
            panelSettings.showFuel = (bool)ShowFuelRemainingCheckbox.IsChecked;
            panelSettings.showMPG = (bool)ShowMPGCheckbox.IsChecked;
            panelSettings.showInMetric = (bool)ShowInMetricCheckbox.IsChecked;
            panelSettings.tankSize = (ushort)TankSizeBox.Value;
            if (CurrentLevelBox.Value > TankSizeBox.Value)
            {
                CurrentLevelBox.Value = TankSizeBox.Value;
            }
            var lastStartFuel = (panelSettings.showInMetric) ? TankMinder.Contents.StartGallons : TankMinder.Contents.StartLiters;
            TankMinder.Contents.StartGallons =
                ChassisParameters.Instance.CurrentFuelGallons - ((ushort)(TankSizeBox.Value - (ushort)CurrentLevelBox.Value));
            TankMinder.Contents.StartLiters =
                ChassisParameters.Instance.CurrentFuelLiters - ((ushort)(TankSizeBox.Value - (ushort)CurrentLevelBox.Value));
            if (lastStartFuel != TankMinder.Contents.StartGallons)
            {
                TankMinder.Contents.StartMiles = ChassisParameters.Instance.CurrentMiles;
                TankMinder.Contents.StartKilometers = ChassisParameters.Instance.CurrentKilometers;
            }
            TankMinder.SaveConfiguration();
            panelSettings.alignment = GetAlignment();
            panelSettings.orientation = GetOrientation();
            DialogResult = true;
        }
    }
}
