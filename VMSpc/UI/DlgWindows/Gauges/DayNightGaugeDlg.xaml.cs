using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for DayNightGaugeDlg.xaml
    /// </summary>
    public partial class DayNightGaugeDlg : VPanelDlg
    {
        public DayNightGaugeDlg(DayNightGaugeSettings panelSettings)
            :base(panelSettings)
        {
            InitializeComponent();
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = Enums.UI.PanelType.DAYNIGHT_GAUGE;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (DayNightGaugeSettings)base.panelSettings;
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
