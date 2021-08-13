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
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;
using VMSpc.UI.Managers.Alarms;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows.Settings
{
    /// <summary>
    /// Interaction logic for AudibleAlarmsDlg.xaml
    /// </summary>
    public partial class AudibleAlarmsDlg : VMSDialog
    {
        private static List<AlarmSettings> AlarmSettingsList = ConfigManager.AlarmsReader.Contents.AlarmSettingsList;
        public AudibleAlarmsDlg()
        {
            InitializeComponent();
            AddAudibleAlarmItems();
        }

        private void AddAudibleAlarmItems()
        {
            AlarmsList.Items.Clear();
            foreach (var alarm in AlarmSettingsList.OrderBy(key => key.Name))
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = alarm.Name, ID = (ushort)alarm.Instance };
                AlarmsList.Items.Add(item);
            }
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new EditAlarmDlg(-1)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
            };
            if ((bool)dlg.ShowDialog())
            {
                AddAudibleAlarmItems();
                AlarmManager.LoadAlarms();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ((VMSListBoxItem)AlarmsList.SelectedItem);
            if (selectedItem != null)
            {
                var itemId = selectedItem.ID;
                var dlg = new EditAlarmDlg(itemId)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                };
                if ((bool)dlg.ShowDialog())
                {
                    AddAudibleAlarmItems();
                    AlarmManager.LoadAlarms();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ((VMSListBoxItem)AlarmsList.SelectedItem);
            if (selectedItem != null)
            {
                var itemId = selectedItem.ID;
                ConfigManager.AlarmsReader.DeleteAlarm(itemId);
                AddAudibleAlarmItems();
                AlarmManager.LoadAlarms();
            }
        }
    }
}
