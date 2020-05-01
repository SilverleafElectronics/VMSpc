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
using VMSpc.UI.CustomComponents;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;
using System.ComponentModel;
using VMSpc.Enums.UI;
using VMSpc.UI.DlgWindows;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for TirePanelDlg.xaml
    /// </summary>
    public partial class TirePanelDlg : VPanelDlg
    {
        public TirePanelDlg(TireGaugeSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (TireGaugeSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = PanelType.TIRE_GAUGE;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }


}
