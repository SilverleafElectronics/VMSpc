using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VMSpc.DevHelpers;
using VMSpc.UI.CustomComponents;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;
using System.ComponentModel;
using VMSpc.Enums.UI;
using System.Windows.Media;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for RadialGaugeDlg.xaml
    /// </summary>
    public partial class RadialGaugeDlg : VPanelDlg
    {
        protected new RadialGaugeSettings panelSettings;

        public HorizontalAlignment checkedRadio
        {
            get { return panelSettings.alignment; }
            set { SetProperty(ref panelSettings.alignment, value); }
        }
        public RadialGaugeDlg(SimpleGaugeSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            AddParameterChoices();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (RadialGaugeSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = PanelType.RADIAL_GAUGE;
            panelSettings.pid = 84;
            panelSettings.showName = true;
            panelSettings.showSpot = true;
            panelSettings.showUnit = true;
            panelSettings.showValue = true;
            panelSettings.showAbbreviation = false;
            panelSettings.showGraph = true;
            panelSettings.alignment = HorizontalAlignment.Center;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();

            UseAbbr.IsChecked = panelSettings.showAbbreviation;
            ShowGraph.IsChecked = panelSettings.showGraph;
            ShowGaugeName.IsChecked = panelSettings.showName;
            ShowUnit.IsChecked = panelSettings.showUnit;
            ShowMetric.IsChecked = panelSettings.showInMetric;
            ShowValue.IsChecked = panelSettings.showValue;
            UseGlobalColor.IsChecked = panelSettings.useGlobalColorPalette;
            BackgroundColor = panelSettings.backgroundColor;
            BorderColor = panelSettings.borderColor;
        }

        protected void AddParameterChoices()
        {
            foreach (var param in ConfigManager.ParamData.Contents.Parameters.OrderBy(key => key.ParamName))
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = param.ParamName, ID = param.Pid };
                GaugeTypes.Items.Add(item);
                if (item.ID == panelSettings.pid)
                {
                    item.IsSelected = true;
                    GaugeTypes.ScrollIntoView(item);
                }
            }
        }

        protected void OkButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.pid = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
            panelSettings.showAbbreviation = (bool)UseAbbr.IsChecked;
            panelSettings.showGraph = (bool)ShowGraph.IsChecked;
            panelSettings.showInMetric = (bool)ShowMetric.IsChecked;
            panelSettings.showName = (bool)ShowGaugeName.IsChecked;
            panelSettings.showUnit = (bool)ShowUnit.IsChecked;
            panelSettings.showValue = (bool)ShowValue.IsChecked;
            panelSettings.useGlobalColorPalette = (bool)UseGlobalColor.IsChecked;
            panelSettings.backgroundColor = BackgroundColor;
            panelSettings.borderColor = BorderColor;
            DialogResult = true;
            Close();
        }

        protected void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected void Radio_Checked(object s, RoutedEventArgs e)
        {
            //var radio = s as RadioButton;
            //checkedRadio = Convert.ToInt16(radio.Tag);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundColor = panelSettings.backgroundColor;
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void ChangeBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            BorderColor = panelSettings.borderColor;
            ChangeColor(ref BorderColor);
        }

        private void UseGlobalColor_Checked(object sender, RoutedEventArgs e)
        {
            ApplyGlobalColorPalette();
        }
    }
}
