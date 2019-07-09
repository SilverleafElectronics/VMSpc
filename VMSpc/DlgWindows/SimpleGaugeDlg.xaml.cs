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
        public int checkedRadio
        {
            get { return panelSettings.TextPosition; }
            set { SetProperty(ref panelSettings.TextPosition, value); }
        }
        public SimpleGaugeDlg(PanelSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            AddParameterChoices();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            if (panelSettings.TextPosition < 0 || panelSettings.TextPosition > 2)
                panelSettings.TextPosition = 0;
            ((RadioButton)RadioAlignment.Children[panelSettings.TextPosition]).IsChecked = true;
        }

        private void AddParameterChoices()
        {
            foreach (var param in ParamData.parameters)
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = param.Value.ParamName, ID = param.Value.Pid };
                GaugeTypes.Items.Add(item);
                if (item.ID == ((SimpleGaugeSettings)panelSettings).PID)
                    GaugeTypes.SelectedItem = item;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton button in RadioAlignment.Children)
                if (button.IsChecked == true) panelSettings.TextPosition = Convert.ToInt16(button.Tag);
            VMSConsole.PrintLine("" + ((RadioButton)RadioAlignment.Children[0]).IsChecked);
            ((SimpleGaugeSettings)panelSettings).PID = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Radio_Checked(object s, RoutedEventArgs e)
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
