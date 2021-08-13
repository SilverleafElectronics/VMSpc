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
using VMSpc.Enums.UI;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using System.ComponentModel;

namespace VMSpc.UI.DlgWindows.Settings
{
    /// <summary>
    /// Interaction logic for EditAlarmDlg.xaml
    /// </summary>
    public partial class EditAlarmDlg : VMSDialog
    {
        private AlarmSettings alarmSettings;
        private MediaPlayer mediaPlayer;
        public EditAlarmDlg(int alarmInstance=-1)
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
            if (alarmInstance > -1)
            {
                alarmSettings = ConfigManager.AlarmsReader.GetAlarmByInstance(alarmInstance);
            }
            else
            {
                alarmSettings = new AlarmSettings()
                {
                    Name = "Road Speed",
                    Instance = -1,
                    AlarmCondition = new AlarmCondition()
                    {
                        Pid = 84,
                        TriggerValue = 70,
                        Comparator = ComparativeOperator.GreaterThan
                    },
                    SoundSettings = new AudibleSettings()
                    {
                        AlarmFrequency = AlarmFrequency.Once,
                        AudioFilePath = "\\audio\\ahooga.wav",
                    },
                };
            }
            AddParameterChoices();
            ApplyBindings();
        }

        protected void AddParameterChoices()
        {
            foreach (var param in ConfigManager.ParamData.Contents.Parameters.OrderBy(key => key.ParamName))
            {
                VMSListBoxItem item = new VMSListBoxItem() { Content = param.ParamName, ID = param.Pid };
                GaugeTypes.Items.Add(item);
                if (item.ID == alarmSettings.AlarmCondition.Pid)
                {
                    item.IsSelected = true;
                    GaugeTypes.ScrollIntoView(item);
                }
            }
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            if (alarmSettings.AlarmCondition.Comparator == ComparativeOperator.GreaterThan)
                OverCondition.IsChecked = true;
            else if (alarmSettings.AlarmCondition.Comparator == ComparativeOperator.LessThan)
                UnderCondition.IsChecked = true;
            switch (alarmSettings.AlarmCondition.Comparator)
            {
                case ComparativeOperator.GreaterThan:
                    OverCondition.IsChecked = true;
                    break;
                case ComparativeOperator.LessThan:
                    UnderCondition.IsChecked = true;
                    break;
                default:
                    OverCondition.IsChecked = true;
                    break;
            }
            switch (alarmSettings.SoundSettings.AlarmFrequency)
            {
                case AlarmFrequency.EveryFifteenMinutes:
                    TriggerFifteen.IsChecked = true;
                    break;
                case AlarmFrequency.EveryMinute:
                    TriggerMinute.IsChecked = true;
                    break;
                case AlarmFrequency.EverySecond:
                    TriggerContinuous.IsChecked = true;
                    break;
                case AlarmFrequency.Once:
                    TriggerOnce.IsChecked = true;
                    break;
                default:
                    TriggerOnce.IsChecked = true;
                    break;
            }
            AlarmNameBox.Text = alarmSettings.Name;
            TriggerValueBox.Text = alarmSettings.AlarmCondition.TriggerValue.ToString();
            SoundFile.Text = alarmSettings.SoundSettings.AudioFilePath;
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(TriggerValueBox.Text, out double triggerValue))
            {
                MessageBox.Show("Please enter a number in the \"Trigger Value\" field.");
                return;
            }
            if (string.IsNullOrEmpty(SoundFile.Text) || string.IsNullOrWhiteSpace(SoundFile.Text))
            {
                MessageBox.Show("Please select a file path for the alarm tone.");
                return;
            }
            alarmSettings.AlarmCondition.Pid = ((VMSListBoxItem)GaugeTypes.SelectedItem).ID;
            alarmSettings.AlarmCondition.Comparator =
                ((bool)OverCondition.IsChecked) ? ComparativeOperator.GreaterThan
                : ComparativeOperator.LessThan;
            if ((bool)TriggerOnce.IsChecked)
                alarmSettings.SoundSettings.AlarmFrequency = AlarmFrequency.Once;
            else if ((bool)TriggerFifteen.IsChecked)
                alarmSettings.SoundSettings.AlarmFrequency = AlarmFrequency.EveryFifteenMinutes;
            else if ((bool)TriggerMinute.IsChecked)
                alarmSettings.SoundSettings.AlarmFrequency = AlarmFrequency.EveryMinute;
            else if ((bool)TriggerContinuous.IsChecked)
                alarmSettings.SoundSettings.AlarmFrequency = AlarmFrequency.EverySecond;
            alarmSettings.AlarmCondition.TriggerValue = triggerValue;
            alarmSettings.Name = AlarmNameBox.Text;
            alarmSettings.SoundSettings.AudioFilePath = SoundFile.Text;
            if (alarmSettings.Instance < 0)
            {
                ConfigManager.AlarmsReader.AddAlarm(alarmSettings);
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FileSelector("\\audio", SoundFile.Text, "wav", "mp3")
            {
                AllowDeletes = true,
                AllowImports = true,
                AllowNewFiles = false,
                ImportFilter = "Audio Files(*.wav;*.mp3)|*.wav;*.mp3",
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExcludeLockedFiles = true,
            };
            if ((bool)dlg.ShowDialog())
            {
                SoundFile.Text = dlg.ResultFilePath;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Close();
                var uri = new Uri(FileOpener.GetAbsoluteFilePath(SoundFile.Text));
                mediaPlayer.Open(uri);
                mediaPlayer.Play();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Close();
            base.OnClosing(e);
        }

        private void GaugeTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlarmNameBox.Text = ((VMSListBoxItem)GaugeTypes.SelectedItem).Content.ToString();
        }
    }
}
