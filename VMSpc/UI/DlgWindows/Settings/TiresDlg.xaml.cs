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
using VMSpc.AdvancedParsers.Tires;
using VMSpc.UI.CustomComponents;
using VMSpc.Enums.UI;
using VMSpc.JsonFileManagers;
using VMSpc.Enums.Parsing;
using System.Timers;
using VMSpc.Enums;

namespace VMSpc.UI.DlgWindows.Settings
{
    /// <summary>
    /// Interaction logic for TiresDlg.xaml
    /// </summary>
    public partial class TiresDlg : VMSDialog
    {
        SettingsContents Settings = ConfigurationManager.ConfigManager.Settings.Contents;
        private Timer UpdateTimer;
        public TpmsType tpmsType;
        public TiresDlg()
        {
            InitializeComponent();
            CreateTireLayouts();
            ApplyBindings();
            UpdateTimer = Constants.CREATE_TIMER(UpdateTireDescriptors, 3000, DispatchType.OnMainThreadAsync);
            UpdateTimer.Start();
        }

        private void CreateTireLayouts()
        {
            foreach (Enum name in Enum.GetValues(typeof(TireMapType)))
            {
                var typeRadioButton = new RadioButton()
                {
                    Content = name.GetDescription(),
                };
                TireLayout.Children.Add(typeRadioButton);
            }
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            foreach (RadioButton radioButton in TireLayout.Children)
            {
                if (radioButton.Content.ToString().Equals(Settings.tireMapType.GetDescription()))
                {
                    radioButton.IsChecked = true;
                }
            }
            tpmsType = Settings.tpmsType;
            var tpmsString = tpmsType.GetDescription();
            foreach (RadioButton radioButton in TPMSSelection.Children)
            {
                if (radioButton.Content.ToString().Equals(tpmsString))
                {
                    radioButton.IsChecked = true;
                }
            }
            AdjustUIForTireType();
        }

        private void UpdateTireDescriptors()
        {
            var statusDescriptors = GetByTagName<VMSListBoxItem>("TireStatusDescriptor");
            if (statusDescriptors == null)
                return;
            var WarningBackground = new SolidColorBrush(Colors.Yellow);
            var AlertBackground = new SolidColorBrush(Colors.Red);
            var NormalBackground = new SolidColorBrush(Colors.White);
            for (int i = 0; i < statusDescriptors.Count(); i++)
            {
                var descriptor = statusDescriptors.ElementAt(i);
                if (TireManager.Instance == null)
                {
                    descriptor.Content = $"{i + 1}. No Tire Manager";
                }
                else
                {
                    Tire tire = TireManager.Instance.Tires[i];
                    descriptor.Content = $"{i + 1}. {tire.TireStatusString()}";
                    switch (tire.TireStatus)
                    {
                        case TireStatus.Warning:
                            descriptor.Background = WarningBackground;
                            break;
                        case TireStatus.Alert:
                            descriptor.Background = AlertBackground;
                            descriptor.FontWeight = FontWeights.SemiBold;
                            break;
                        default:
                            descriptor.Background = NormalBackground;
                            descriptor.FontWeight = FontWeights.Bold;
                            break;
                    }
                }
            }
        }

        private byte GetSelectedTire()
        {
            return (byte)((VMSListBoxItem)TireSensorList.SelectedItem).ID;
        }

        private void LearnButton_Click(object sender, RoutedEventArgs e)
        {
            var instance = GetSelectedTire();
            TireManager.Instance?.LearnTire(instance);
            MessageBox.Show($"Learning Tire {instance + 1}");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var instance = GetSelectedTire();
            TireManager.Instance?.ClearTire(instance);
            MessageBox.Show($"Clearing Tire {instance + 1}");
        }

        private void AbortLearnButton_Click(object sender, RoutedEventArgs e)
        {
            TireManager.Instance?.AbortLearn();
            MessageBox.Show("Tire Learning Cancelled");
        }

        private void SelectTST_Click(object sender, RoutedEventArgs e)
        {
            tpmsType = TpmsType.TST;
            //Settings.tpmsType = TpmsType.TST;
            //AdjustUIForTireType();
        }

        private void SelectPPro_Click(object sender, RoutedEventArgs e)
        {
            tpmsType = TpmsType.PressurePro;
            //Settings.tpmsType = TpmsType.PPRO;
            //AdjustUIForTireType();
        }

        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {
            tpmsType = TpmsType.None;
            //Settings.tpmsType = TpmsType.NONE;
            //AdjustUIForTireType();
        }

        private void AdjustUIForTireType()
        {
            switch (tpmsType)
            {
                case TpmsType.TST:
                    TSTSettingsButton.Visibility = Visibility.Visible;
                    break;
                case TpmsType.PressurePro:
                case TpmsType.None:
                    TSTSettingsButton.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void TSTSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TSTSettings()
            {
                Owner = this,
            };
            dlg.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimer?.Stop();
            UpdateTimer = null;
            Close();
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            var tireTypes = Enum.GetValues(typeof(TireMapType));
            foreach (RadioButton radioButton in TireLayout.Children)
            {
                if (radioButton.IsChecked == true)
                {
                    foreach (TireMapType tireMapType in tireTypes)
                    {
                        if (tireMapType.GetDescription() == radioButton.Content.ToString())
                        {
                            Settings.tireMapType = tireMapType;
                        }
                    }
                }
            }
            if (Settings.tpmsType != tpmsType)
            {
                Settings.tpmsType = tpmsType;
                TireManager.Initialize();
            }
            DialogResult = true;
            UpdateTimer?.Stop();
            UpdateTimer.Dispose();
            UpdateTimer = null;
            Close();
        }
    }
}
