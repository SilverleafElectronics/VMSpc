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
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows.Advanced
{
    /// <summary>
    /// Interaction logic for ParameterEditorDlg.xaml
    /// </summary>
    public partial class ParameterEditorDlg : VMSDialog
    {
        public ParameterEditorDlg()
        {
            InitializeComponent();
            AddParameterChoices();
        }

        protected void AddParameterChoices()
        {
            GaugeTypes.Items.Clear();
            foreach (var param in ConfigManager.ParamData.Contents.Parameters.OrderBy(key => key.ParamName))
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = param.ParamName, ID = param.Pid };
                GaugeTypes.Items.Add(item);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var itemEditor = new ParameterItemEditor(null)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if ((bool)itemEditor.ShowDialog())
                AddParameterChoices();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if ((GaugeTypes.SelectedItem) != null)
            {
                var parameter = ConfigManager.ParamData.GetParam(((VMSListBoxItem)GaugeTypes.SelectedItem).ID);
                var itemEditor = new ParameterItemEditor(parameter)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                if ((bool)itemEditor.ShowDialog())
                    AddParameterChoices();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((GaugeTypes.SelectedItem) != null)
            {
                var parameter = ConfigManager.ParamData.GetParam(((VMSListBoxItem)GaugeTypes.SelectedItem).ID);
                if (ConfirmDelete(parameter))
                {
                    ConfigManager.ParamData.Contents.Parameters.Remove(parameter);
                    AddParameterChoices();
                    PIDValueStager.Instance.Reset();
                }
            }
        }
        private bool ConfirmDelete(JParameter parameter)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the parameter {parameter.ParamName} at PID {parameter.Pid}? This action cannot be undone.", "Confirm Delete", MessageBoxButton.YesNo);
            return (result == MessageBoxResult.Yes);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
