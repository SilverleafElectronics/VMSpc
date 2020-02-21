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
using VMSpc.CustomComponents;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;
using System.ComponentModel;
using VMSpc.VEnum.UI;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for SimpleGaugeDlg.xaml
    /// </summary>
    public partial class SimpleGaugeDlg : VPanelDlg
    {
        protected new SimpleGaugeSettings panelSettings;
        public UIPosition checkedRadio
        {
            get { return panelSettings.alignment; }
            set { SetProperty(ref panelSettings.alignment, value); }
        }
        public SimpleGaugeDlg(SimpleGaugeSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            AddParameterChoices();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (SimpleGaugeSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = PanelType.SIMPLE_GAUGE;
            panelSettings.pid = 84;
            panelSettings.showName = true;
            panelSettings.showSpot = true;
            panelSettings.showUnit = true;
            panelSettings.showValue = true;
            panelSettings.showAbbreviation = false;
            panelSettings.showGraph = true;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            //TODO: SimpleGaugeDlg:1 - implement a generic radio button handler, instead of crude maps like these
            ((RadioButton)RadioAlignment.Children[(int)panelSettings.alignment]).IsChecked = true;

            UseAbbr.IsChecked = panelSettings.showAbbreviation;
            ShowGraph.IsChecked = panelSettings.showGraph;
            ShowGaugeName.IsChecked = panelSettings.showName;
            ShowUnit.IsChecked = panelSettings.showUnit;
            ShowMetric.IsChecked = panelSettings.showInMetric;
            ShowValue.IsChecked = panelSettings.showValue;
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
            foreach (RadioButton button in RadioAlignment.Children)
                if (button.IsChecked == true) panelSettings.alignment = (UIPosition)Convert.ToInt16(button.Tag);    //TODO: SimpleGaugeDlg:1
            VMSConsole.PrintLine("" + ((RadioButton)RadioAlignment.Children[0]).IsChecked);
            panelSettings.pid = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
            panelSettings.showAbbreviation = (bool)UseAbbr.IsChecked;
            panelSettings.showGraph = (bool)ShowGraph.IsChecked;
            panelSettings.showInMetric = (bool)ShowMetric.IsChecked;
            panelSettings.showName = (bool)ShowGaugeName.IsChecked;
            panelSettings.showUnit = (bool)ShowUnit.IsChecked;
            panelSettings.showValue = (bool)ShowValue.IsChecked;
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
    }
}
