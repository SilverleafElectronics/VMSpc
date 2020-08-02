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
using VMSpc.AdvancedParsers;
using VMSpc.DevHelpers;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows.View
{
    /// <summary>
    /// Interaction logic for Maintenance.xaml
    /// </summary>
    public partial class MaintenanceDlg : VMSDialog
    {
        private List<MaintenanceItem> MaintenanceItems => ConfigurationManager.ConfigManager.MaintenanceTrackerReader.Contents.MaintenanceItems;
        private MaintenanceItem SelectedMaintenanceItem;
        private CompletedItem SelectedCompleteItem;
        public MaintenanceDlg()
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            DrawMaintenanceItems();
        }

        private void DrawMaintenanceItems()
        {
            MaintenanceItemsListBox.Items.Clear();
            if (MaintenanceItems != null)
            {
                foreach (var item in MaintenanceItems)
                {
                    var listBoxItem = new ListBoxItem()
                    {
                        Content = item.ToString(),
                        Tag = item,
                        FontWeight = FontWeights.Bold,
                        Background = (item.DueNow()) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White),
                    };
                    MaintenanceItemsListBox.Items.Add(listBoxItem);
                    if (item == SelectedMaintenanceItem)
                    {
                        listBoxItem.IsSelected = true;
                    }
                }
            }
            MaintenanceItemActions.IsReadOnly = (SelectedMaintenanceItem == null);
            if (SelectedCompleteItem != null)
            {
                SelectedCompleteItem.Actions = CompletedItemActions.Text;
                SelectedCompleteItem = null;
            }
            CompletedItemActions.Text = "";
            CompletedItemActions.IsReadOnly = true;
            SetSelectedItem(SelectedMaintenanceItem);
        }

        private void SetSelectedItem(MaintenanceItem maintenanceItem)
        {
            if (maintenanceItem == null)
                return;
            if (SelectedMaintenanceItem != null && SelectedMaintenanceItem != maintenanceItem)
            {
                SelectedMaintenanceItem.Actions = MaintenanceItemActions.Text;
            }
            SelectedMaintenanceItem = maintenanceItem;
            if (SelectedMaintenanceItem != null)
            {
                MaintenanceItemActions.Text = SelectedMaintenanceItem.Actions;
                MaintenanceItemActions.IsReadOnly = false;
                SelectedCompleteItem = null;
                DrawCompleteItems();
            }
            else
            {
                MaintenanceItemActions.IsReadOnly = true;
            }
        }

        private void DrawCompleteItems()
        {
            if (SelectedMaintenanceItem != null && SelectedMaintenanceItem.CompletedItems != null)
            {
                CompletedItemsListBox.Items.Clear();
                foreach (var item in SelectedMaintenanceItem.CompletedItems)
                {
                    CompletedItemsListBox.Items.Add(
                        new ListBoxItem()
                        {
                            Content = item.ToString(),
                            Tag = item,
                        }
                        );
                }
            }
        }

        private void MaintenanceItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                SetSelectedItem(((e.AddedItems[0] as ListBoxItem).Tag as MaintenanceItem));
            }
            if (SelectedCompleteItem != null)
            {
                SelectedCompleteItem.Actions = CompletedItemActions.Text;
                SelectedCompleteItem = null;
            }
            CompletedItemActions.Text = "";
            CompletedItemActions.IsReadOnly = true;
        }

        private void CompletedItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                if (SelectedCompleteItem != null)
                {
                    SelectedCompleteItem.Actions = CompletedItemActions.Text;
                }
                SelectedCompleteItem = ((e.AddedItems[0] as ListBoxItem).Tag as CompletedItem);
                if (SelectedCompleteItem != null)
                {
                    CompletedItemActions.Text = SelectedCompleteItem.Actions;
                    CompletedItemActions.IsReadOnly = false;
                }
            }
        }

        private void AddMaintenanceItemButton_Click(object sender, RoutedEventArgs e)
        {
            MaintenanceItem maintenanceItem = new MaintenanceItem()
            {
                Name = "New Maintenance Item",
                MaintenanceIntervalType = MaintenanceIntervalType.DateAndMileage,
                MonthsMaintenanceInterval = 12,
                MilesMaintenanceInterval = 12000,
                Actions = string.Empty
            };
            EditMaintenanceItemDlg editMaintenanceItemDlg = new EditMaintenanceItemDlg(maintenanceItem)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            if ((bool)editMaintenanceItemDlg.ShowDialog())
            {
                MaintenanceItems.Add(maintenanceItem);
                SelectedMaintenanceItem = maintenanceItem;
                DrawMaintenanceItems();
            }
        }

        private void EditMaintnenanceItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompleteItem != null)
            {
                EditSelectedCompleteItem();
            }
            else if (SelectedMaintenanceItem != null)
            {
                EditMaintenanceItemDlg editMaintenanceItemDlg = new EditMaintenanceItemDlg(SelectedMaintenanceItem)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                if ((bool)editMaintenanceItemDlg.ShowDialog())
                {
                    DrawMaintenanceItems();
                }
            }
        }

        private void EditSelectedCompleteItem()
        {
            if (SelectedCompleteItem != null)
            {
                DrawMaintenanceItems();
            }
        }

        private void DeleteMaintenanceItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompleteItem != null)
            {
                DeleteSelectedCompleteItem();
            }
            else if (SelectedMaintenanceItem != null)
            {
                MaintenanceItems.Remove(SelectedMaintenanceItem);
                SelectedMaintenanceItem = null;
                DrawMaintenanceItems();
            }
        }

        private void DeleteSelectedCompleteItem()
        {
            if (SelectedMaintenanceItem != null && SelectedCompleteItem != null)
            {
                SelectedMaintenanceItem.CompletedItems?.Remove(SelectedCompleteItem);
            }
            DrawMaintenanceItems();
        }

        private void PerformMaintenanceItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMaintenanceItem != null)
            {
                SelectedMaintenanceItem.CompletedItems.Add(new CompletedItem()
                {
                    DateCompleted = DateTime.Now,
                    MilesCompletedOn = (ulong)ChassisParameters.Instance.CurrentMiles,
                    Actions = string.Empty,
                });
                DrawMaintenanceItems();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMaintenanceItem != null)
            {
                SelectedMaintenanceItem.Actions = MaintenanceItemActions.Text;
            }
            if (SelectedCompleteItem != null)
            {
                SelectedCompleteItem.Actions = CompletedItemActions.Text;
                SelectedCompleteItem = null;
            }
            CompletedItemActions.Text = "";
            CompletedItemActions.IsReadOnly = true;
            Close();
        }
    }
}
