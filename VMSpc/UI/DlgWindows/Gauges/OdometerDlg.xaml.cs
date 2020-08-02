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
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;
using VMSpc.AdvancedParsers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for OdometerDlg.xaml
    /// </summary>
    public partial class OdometerDlg : VPanelDlg
    {
        protected new OdometerSettings panelSettings;
        protected OdometerReader OdometerReader;
        protected OdometerDataFileReader OdometerDataFileReader;
        protected double CurrentDistance => ChassisParameters.Instance.CurrentMiles - OdometerReader.Contents.StartMiles;
        protected double CurrentHours => ChassisParameters.Instance.CurrentEngineHours - OdometerReader.Contents.StartHours;
        protected double CurrentFuel => ChassisParameters.Instance.CurrentFuelGallons - OdometerReader.Contents.StartGallons;
        protected double CurrentSpeed => (CurrentHours != 0) ? (CurrentDistance / CurrentHours) : 0;
        protected double CurrentMPG => (CurrentFuel != 0) ? (CurrentDistance / CurrentFuel) : 0;
        protected override int DefaultPanelHeight => 300;
        protected override int DefaultPanelWidth => 600;
        public OdometerDlg(OdometerSettings panelSettings)
            :base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (OdometerSettings)panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = Enums.UI.PanelType.ODOMETER;
            var fileName = OdometerReader.GetNewFilePath();
            panelSettings.fileName = fileName + "_odo.json";
            panelSettings.dataFileName = fileName + "_data.json";
            panelSettings.showCaptions = true;
            panelSettings.showFuel = true;
            panelSettings.showHours = true;
            panelSettings.showInMetric = false;
            panelSettings.showMiles = true;
            panelSettings.showMPG = true;
            panelSettings.showSpeed = true;
            panelSettings.showUnits = true;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            OdometerReader = new OdometerReader(panelSettings.fileName);
            OdometerDataFileReader = new OdometerDataFileReader(panelSettings.dataFileName);
            ShowCaptionsCheckbox.IsChecked = panelSettings.showCaptions;
            ShowUnitsCheckbox.IsChecked = panelSettings.showUnits;
            ShowInMetricCheckbox.IsChecked = panelSettings.showInMetric;
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
            if (panelSettings.orientation == Orientation.Horizontal)
                HorizontalLayoutRadio.IsChecked = true;
            else
                VerticalLayoutRadio.IsChecked = true;
            ShowDistanceCheckbox.IsChecked = panelSettings.showMiles;
            ShowRuntimeCheckbox.IsChecked = panelSettings.showHours;
            ShowAverageSpeedCheckbox.IsChecked = panelSettings.showSpeed;
            ShowFuelUsedCheckbox.IsChecked = panelSettings.showFuel;
            ShowEconomyCheckbox.IsChecked = panelSettings.showMPG;
        }

        private void ResetTripButton_Click(object sender, RoutedEventArgs e)
        {
            var contents = OdometerReader.Contents;
            contents.StartGallons = CurrentFuel;
            contents.StartHours = CurrentHours;
            contents.StartMiles = CurrentDistance;
            var entry = new OdometerDataEntry()
            {
                EndDate = DateTime.Now,
                Miles = contents.StartMiles,
                Fuel = contents.StartGallons,
                Time = contents.StartHours,
                Speed = CurrentSpeed,
                MPG = CurrentMPG
            };
            OdometerDataFileReader.Contents.OdometerDataEntries.Add(entry);
            OdometerDataFileReader.SaveConfiguration();
            OdometerReader.SaveConfiguration();
            MessageBox.Show("The Odometer has been reset");
        }

        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var contents = new OdometerDataFileReader(panelSettings.dataFileName).Contents;
            var viewer = new OdometerDataViewer(contents)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            viewer.ShowDialog();
        }

        private void StartFromDayOneButton_Click(object sender, RoutedEventArgs e)
        {
            var contents = OdometerReader.Contents;
            contents.StartGallons = 0;
            contents.StartHours = 0;
            contents.StartKilometers = 0;
            contents.StartLiters = 0;
            contents.StartMiles = 0;
            OdometerReader.SaveConfiguration();
            MessageBox.Show("The Odometer has been reset to day one");
        }

        private void ChangeDataFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref BorderColor))
            {
                panelSettings.useGlobalColorPalette = false;
            }
        }

        private void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.showCaptions = (bool)ShowCaptionsCheckbox.IsChecked;
            panelSettings.showUnits = (bool)ShowUnitsCheckbox.IsChecked;
            panelSettings.showInMetric = (bool)ShowInMetricCheckbox.IsChecked;
            panelSettings.showMiles = (bool)ShowDistanceCheckbox.IsChecked;
            panelSettings.showHours = (bool)ShowRuntimeCheckbox.IsChecked;
            panelSettings.showSpeed = (bool)ShowAverageSpeedCheckbox.IsChecked;
            panelSettings.showFuel = (bool)ShowFuelUsedCheckbox.IsChecked;
            panelSettings.showMPG = (bool)ShowEconomyCheckbox.IsChecked;
            panelSettings.alignment = GetAlignment();
            panelSettings.orientation = GetOrientation();
            panelSettings.BackgroundColor = BackgroundColor;
            panelSettings.BorderColor = BorderColor;
            DialogResult = true;
            Close();
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
            else  if ((bool)RightAlignmentRadio.IsChecked)
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
    }
}
