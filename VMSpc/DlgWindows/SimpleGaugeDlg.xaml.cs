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

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for SimpleGaugeDlg.xaml
    /// </summary>
    public partial class SimpleGaugeDlg : VPanelDlg
    {
        public SimpleGaugeDlg(PanelSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            AddParameterChoices();
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
            ((SimpleGaugeSettings)panelSettings).PID = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
            Close();
        }
    }
}
