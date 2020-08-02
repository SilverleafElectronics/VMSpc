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

namespace VMSpc.UI.DlgWindows.View
{
    /// <summary>
    /// Interaction logic for EditMaintenanceItemDlg.xaml
    /// </summary>
    public partial class EditMaintenanceItemDlg : VMSDialog
    {
        private MaintenanceItem maintenanceItem;
        private MaintenanceIntervalType MaintenanceIntervalType;
        public EditMaintenanceItemDlg(MaintenanceItem maintenanceItem)
        {
            this.maintenanceItem = maintenanceItem;
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            var lastCompletedItem = maintenanceItem.GetLastCompleted();
            if (lastCompletedItem != null)
            {
                LastPerformedDateBox.Text = lastCompletedItem.DateCompleted.ToString("MM/dd/yyyy");
                LastPerformedMilesBox.Text = lastCompletedItem.MilesCompletedOn.ToString();
            }
            else
            {
                LastPerformedDateBox.Text = "Not Performed Yet";
                LastPerformedMilesBox.Text = "Not Performed Yet";
            }
            NameBox.Text = maintenanceItem.Name;
            ScheduledMileage.Text = maintenanceItem.MilesMaintenanceInterval.ToString();
            ScheduledMonths.Text = maintenanceItem.MonthsMaintenanceInterval.ToString();
            MaintenanceIntervalType = maintenanceItem.MaintenanceIntervalType;
            switch (maintenanceItem.MaintenanceIntervalType)
            {
                case MaintenanceIntervalType.DateAndMileage:
                    TimeAndMileageButton.IsChecked = true;
                    break;
                case MaintenanceIntervalType.DateOnly:
                    TimeOnlyButton.IsChecked = true;
                    break;
                case MaintenanceIntervalType.MileageOnly:
                    MileageOnlyButton.IsChecked = true;
                    break;
            }
        }

        private void AdjustScheduleDisplayType()
        {
            EditMileagePanel.Visibility = Visibility.Collapsed;
            EditMonthsPanel.Visibility = Visibility.Collapsed;
            switch (MaintenanceIntervalType)
            {
                case MaintenanceIntervalType.DateOnly:
                    ScheduledSettingsContent.Rows = 1;
                    EditMonthsPanel.Visibility = Visibility.Visible;
                    break;
                case MaintenanceIntervalType.MileageOnly:
                    EditMileagePanel.Visibility = Visibility.Visible;
                    ScheduledSettingsContent.Rows = 1;
                    break;
                case MaintenanceIntervalType.DateAndMileage:
                    ScheduledSettingsContent.Rows = 2;
                    EditMonthsPanel.Visibility = Visibility.Visible;
                    EditMileagePanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void TimeAndMileageButton_Checked(object sender, RoutedEventArgs e)
        {
            MaintenanceIntervalType = MaintenanceIntervalType.DateAndMileage;
            AdjustScheduleDisplayType();
        }

        private void TimeOnlyButton_Checked(object sender, RoutedEventArgs e)
        {
            MaintenanceIntervalType = MaintenanceIntervalType.DateOnly;
            AdjustScheduleDisplayType();
        }

        private void MileageOnlyButton_Checked(object sender, RoutedEventArgs e)
        {
            MaintenanceIntervalType = MaintenanceIntervalType.MileageOnly;
            AdjustScheduleDisplayType();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            maintenanceItem.Name = NameBox.Text;
            maintenanceItem.MaintenanceIntervalType = MaintenanceIntervalType;
            if (MaintenanceIntervalType != MaintenanceIntervalType.DateOnly)
            {
                if (ulong.TryParse(ScheduledMileage.Text, out ulong miles))
                {
                    maintenanceItem.MilesMaintenanceInterval = miles;
                }
                else
                {
                    MessageBox.Show("The specified Mileage is not in the correct format. Please format the miles as a whole number");
                    return;
                }
            }
            if (MaintenanceIntervalType != MaintenanceIntervalType.MileageOnly)
            {
                if (int.TryParse(ScheduledMonths.Text, out int months))
                {
                    maintenanceItem.MonthsMaintenanceInterval = months;
                }
                else
                {
                    MessageBox.Show("The specified Months value is not in the correct format. Please format the months as a whole number");
                    return;
                }
            }
            DialogResult = true;
            Close();
        }
    }
}
