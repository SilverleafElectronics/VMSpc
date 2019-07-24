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
using static VMSpc.XmlFileManagers.ParamDataManager;
using VMSpc.DevHelpers;
using VMSpc.CustomComponents;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;
using System.ComponentModel;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for SimpleGaugeDlg.xaml
    /// </summary>
    public partial class SimpleGaugeDlg : VPanelDlg
    {
        protected new SimpleGaugeSettings panelSettings;
        public int checkedRadio
        {
            get { return panelSettings.TextPosition; }
            set { SetProperty(ref panelSettings.TextPosition, value); }
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
            panelSettings.ID = PanelIDs.SIMPLE_GAUGE_ID;
            panelSettings.PID = 84;
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
            if (panelSettings.TextPosition < 0 || panelSettings.TextPosition > 2)
                panelSettings.TextPosition = 0;
            ((RadioButton)RadioAlignment.Children[panelSettings.TextPosition]).IsChecked = true;

            UseAbbr.IsChecked = panelSettings.showAbbreviation;
            ShowGraph.IsChecked = panelSettings.showGraph;
            ShowGaugeName.IsChecked = panelSettings.showName;
            ShowUnit.IsChecked = panelSettings.showUnit;
            ShowMetric.IsChecked = panelSettings.showInMetric;
            ShowValue.IsChecked = panelSettings.showValue;
        }

        protected void AddParameterChoices()
        {
            foreach (var param in ParamData.parameters.OrderBy(key => key.Value.ParamName))
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = param.Value.ParamName, ID = param.Value.Pid };
                GaugeTypes.Items.Add(item);
                if (item.ID == panelSettings.PID)
                    item.IsSelected = true;
            }
        }

        protected void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton button in RadioAlignment.Children)
                if (button.IsChecked == true) panelSettings.TextPosition = Convert.ToInt16(button.Tag);
            VMSConsole.PrintLine("" + ((RadioButton)RadioAlignment.Children[0]).IsChecked);
            panelSettings.PID = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
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
