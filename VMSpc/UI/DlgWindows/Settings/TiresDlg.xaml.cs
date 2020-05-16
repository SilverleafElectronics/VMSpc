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

namespace VMSpc.UI.DlgWindows.Settings
{
    /// <summary>
    /// Interaction logic for TiresDlg.xaml
    /// </summary>
    public partial class TiresDlg : VMSDialog
    {
        SettingsContents Settings = ConfigurationManager.ConfigManager.Settings.Contents;
        public TiresDlg()
        {
            InitializeComponent();
            CreateTireLayouts();
            ApplyBindings();
        }

        private void CreateTireLayouts()
        {
            foreach (var name in Enum.GetNames(typeof(TireMapType)))
            {
                var typeRadioButton = new RadioButton()
                {
                    Content = name,
                };
                TireLayout.Children.Add(typeRadioButton);
            }
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            foreach (RadioButton radioButton in TireLayout.Children)
            {
                if (radioButton.Content.ToString().Equals(Settings.tireMapType.ToString()))
                {
                    radioButton.IsChecked = true;
                }
            }
        }

        private ushort GetSelectedTire()
        {
            return ((VMSListBoxItem)TireSensorList.SelectedItem).ID;
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton radioButton in TireLayout.Children)
            {
                if (radioButton.IsChecked == true)
                {
                    Settings.tireMapType = (TireMapType)Enum.Parse(typeof(TireMapType), radioButton.Content.ToString());
                }
            }
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LearnButton_Click(object sender, RoutedEventArgs e)
        {
            //TireManager.Instance.LearnTire(GetSelectedTire());
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //TireManager.Instance.ClearTire(GetSelectedTire());
        }

        private void AbortLearnButton_Click(object sender, RoutedEventArgs e)
        {
            TireManager.Instance.AbortLearn();
        }
    }
}
