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

namespace VMSpc.UI.CustomComponents
{
    public enum ClearDiagnosticsResult
    {
        ClearInactiveRecords,
        ClearAllRecords,
        ClearAllForFiveMinutes,
        ClearAllUntilRestart,
        Cancelled,
    }

    /// <summary>
    /// Interaction logic for ClearDiagnosticsBox.xaml
    /// </summary>
    public partial class ClearDiagnosticsBox : Window
    {
        public ClearDiagnosticsResult ClearDiagnosticsResult;
        public ClearDiagnosticsBox()
        {
            InitializeComponent();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            Close(ClearDiagnosticsResult.ClearAllRecords);
        }

        private void ClearInactiveButton_Click(object sender, RoutedEventArgs e)
        {
            Close(ClearDiagnosticsResult.ClearInactiveRecords);
        }

        private void ClearFor5Button_Click(object sender, RoutedEventArgs e)
        {
            Close(ClearDiagnosticsResult.ClearAllForFiveMinutes);
        }

        private void ClearUntilResetButton_Click(object sender, RoutedEventArgs e)
        {
            Close(ClearDiagnosticsResult.ClearAllUntilRestart);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close(ClearDiagnosticsResult.Cancelled);
        }

        private void Close(ClearDiagnosticsResult clearDiagnosticsResult)
        {
            this.ClearDiagnosticsResult = clearDiagnosticsResult;
            Close();
        }
    }
}
