using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMSpc.AdvancedParsers.Tires;

namespace VMSpc.UI.DlgWindows.Settings
{
    /// <summary>
    /// Interaction logic for TSTSettings.xaml
    /// </summary>
    public partial class TSTSettings : VMSDialog
    {
        private Timer UpdateTimer;
        private bool UserIsEditing => TextboxIsFocused();
        private int selectedAxle;
        private static TSTTireManager TSTManager => (TireManager.Instance as TSTTireManager);
        private IEnumerable<TextBox> ConfigItems;
        private int timerCount;
        private bool useMetric;
        public TSTSettings()
        {
            InitializeComponent();
            useMetric = false;
            InitializeExtras();
            ApplyBindings();
            timerCount = 0;
            UpdateTimer = Constants.CREATE_TIMER(OnTimer, 1000, Enums.UI.DispatchType.OnMainThreadAsync);
            UpdateTimer.Start();
            TSTManager.RequestConfiguration();
        }

        protected void InitializeExtras()
        {
            selectedAxle = 0;
            for (int i = 0; i < TSTTireManager.MAX_AXLES; i++)
            {
                var item = new ComboBoxItem()
                {
                    Content = (i + 1).ToString(),
                };
                SelectedAxleBox.Items.Add(item);
            }
            SelectedAxleBox.SelectedIndex = 0;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
        }

        private void OnTimer()
        {
            timerCount++;
            UpdateFields();
            //Request configuration every 20 seconds, if active
            if (timerCount >= 5)
            {
                timerCount = 0;
                if (TSTManager.IsActive)
                {
                    TSTManager.RequestConfiguration();
                }
            }
        }

        private void UpdateFields()
        {
            if (UserIsEditing)
                return;
            TargetPSIBox.Text = TSTManager.TargetPsi(selectedAxle).ToString();
            UnderPressurePctBox.Text = TSTManager.UnderPressureThreshold.ToString();
            ExtremeUnderPressureBox.Text = TSTManager.ExtremeUnderPressureThreshold.ToString();
            OverPressureBox.Text = TSTManager.OverPressureThreshold.ToString();
            OverTemperatureBox.Text = ((int)(useMetric
                ? TSTManager.OverTemperatureThreshold
                : ((TSTManager.OverPressureThreshold - 32) * (5 / 9))
                )).ToString();
            TSTCommStatusBox.Text = TSTManager.IsActive ? "Connected" : "No Communication";
        }

        private bool TextboxIsFocused()
        {
            if (ConfigItems == null)
            {
                ConfigItems = GetByTagName<TextBox>("ConfigItem");
            }
            if (ConfigItems == null)
            {
                return false;
            }
            foreach (var child in ConfigItems)
            {
                if (child.IsFocused)
                {
                    return true;
                }
            }
            return false;
        }

        private void ResetDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            TSTManager.ResetDefaults();
        }

        private void TargetPSISendButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue;
            if (double.TryParse(TargetPSIBox.Text, out newValue))
            {
                TSTManager.ConfigureTargetPSI(selectedAxle, newValue);
            }
            else
            {
                ShowInputError(TargetPSIBox.Text);
            }
        }

        private void UnderPressurePctSendButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue;
            if (double.TryParse(UnderPressurePctBox.Text, out newValue))
            {
                TSTManager.ConfigureUnderPressure(newValue);
            }
            else
            {
                ShowInputError(UnderPressurePctBox.Text);
            }
        }

        private void ExtremeUnderPressureSendButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue;
            if (double.TryParse(ExtremeUnderPressureBox.Text, out newValue))
            {
                TSTManager.ConfigureExtremeUnderPressure(newValue);
            }
            else
            {
                ShowInputError(ExtremeUnderPressureBox.Text);
            }
        }

        private void OverPressureSendButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue;
            if (double.TryParse(OverPressureBox.Text, out newValue))
            {
                TSTManager.ConfigureOverPressure(newValue);
            }
            else
            {
                ShowInputError(OverPressureBox.Text);
            }
        }

        private void OverTemperatureSendButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue;
            if (double.TryParse(OverTemperatureBox.Text, out newValue))
            {
                if (useMetric)    //If Celsius is selected
                {
                    newValue = (newValue * (9 / 5)) + 32;   //Convert to Fahrenheit
                }
                TSTManager.ConfigureOverTemperature(newValue);
            }
            else
            {
                ShowInputError(OverTemperatureBox.Text);
            }
        }

        private void ShowInputError(string attemptedInput)
        {
            MessageBox.Show($"{attemptedInput} is not a valid value. Please enter a decimal value (format ##.##)");
        }

        private void SelectedAxleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedAxle = int.Parse(((e.AddedItems[0] as ComboBoxItem)?.Content as string)) - 1;
            UpdateFields();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimer?.Stop();
            Close();
        }

        private void OverTempUnitType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            useMetric = (((e.AddedItems[0] as ComboBoxItem).Content as string) == "Fahrenheit") ? false : true;
            UpdateFields();
        }
    }
}
