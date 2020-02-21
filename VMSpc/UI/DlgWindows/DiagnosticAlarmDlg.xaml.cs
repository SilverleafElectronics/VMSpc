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
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for DiagnosticAlarmDlg.xaml
    /// </summary>
    public partial class DiagnosticAlarmDlg : VPanelDlg
    {
        protected new DiagnosticGaugeSettings panelSettings;
        public DiagnosticAlarmDlg(DiagnosticGaugeSettings panelSettings)
            :base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            panelSettings.panelId = VEnum.UI.PanelType.DIAGNOSTIC_ALARM;
            panelSettings.backgroundColor = Colors.White;
            panelSettings.WarningColor = Colors.Red;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = base.panelSettings as DiagnosticGaugeSettings;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
    }
}
