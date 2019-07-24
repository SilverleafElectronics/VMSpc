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
using VMSpc.CustomComponents;
using VMSpc.XmlFileManagers;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.Constants;
using VMSpc.DevHelpers;
using System.ComponentModel;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for MultiBarDlg.xaml
    /// </summary>
    public partial class MultiBarDlg : VPanelDlg
    {
        private new MultiBarSettings panelSettings;
        public int checkedRadio
        {
            get { return panelSettings.TextPosition; }
            set { SetProperty(ref panelSettings.TextPosition, value); }
        }
        public MultiBarDlg(MultiBarSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            AddParameterChoices();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            panelSettings = (MultiBarSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.rectCord.topLeftX = 0;
            panelSettings.rectCord.topLeftY = 0;
            panelSettings.rectCord.bottomRightX = 300;
            panelSettings.rectCord.bottomRightY = 300;
            panelSettings.showInMetric = false;
            panelSettings.TextPosition = 0;
            panelSettings.Use_Static_Color = 0;
            panelSettings.showName = true;
            panelSettings.showUnit = true;
            panelSettings.showValue = true;
            panelSettings.showAbbreviation = true;
            panelSettings.showGraph = true;
            panelSettings.ID = PanelIDs.MULTIBAR_ID;
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
            foreach (var param in ParamData.parameters)
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = param.Value.ParamName, ID = param.Value.Pid };
                GaugeTypes.Items.Add(item);
                //if (item.ID == ((SimpleGaugeSettings)panelSettings.PID)
                //    GaugeTypes.SelectedItem = item;
            }
        }

        protected void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton button in RadioAlignment.Children)
                if (button.IsChecked == true) panelSettings.TextPosition = Convert.ToInt16(button.Tag);
            VMSConsole.PrintLine("" + ((RadioButton)RadioAlignment.Children[0]).IsChecked);
            //panelSettings.PID = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
            panelSettings.showAbbreviation = (bool)UseAbbr.IsChecked;
            panelSettings.showGraph = (bool)ShowGraph.IsChecked;
            panelSettings.showName = (bool)ShowGaugeName.IsChecked;
            panelSettings.showUnit = (bool)ShowUnit.IsChecked;
            panelSettings.showInMetric = (bool)ShowMetric.IsChecked;
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
