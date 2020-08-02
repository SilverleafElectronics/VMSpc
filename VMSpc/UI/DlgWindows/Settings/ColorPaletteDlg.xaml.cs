using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for ColorPalletteDlg.xaml
    /// </summary>
    public partial class ColorPaletteDlg : VMSDialog
    {
        private ColorPaletteStackItem
            selectedDayPalette,
            selectedNightPalette;
        private ScreenContents Screen => ConfigManager.Screen.Contents;
        protected ColorPaletteStackItem SelectedDayPalette
        {
            get { return selectedDayPalette; }
            set
            {
                if (selectedDayPalette != null)
                    selectedDayPalette.IsSelected = false;
                selectedDayPalette = value;
                if (selectedNightPalette != null)
                    selectedNightPalette.IsSelected = false;
                if (selectedDayPalette != null)
                    selectedDayPalette.IsSelected = true;
            }
        }
        protected ColorPaletteStackItem SelectedNightPalette
        {
            get { return selectedNightPalette; }
            set
            {
                if (selectedNightPalette != null)
                    selectedNightPalette.IsSelected = false;
                selectedNightPalette = value;
                if (selectedDayPalette != null)
                    selectedDayPalette.IsSelected = false;
                if (selectedNightPalette != null)
                    selectedNightPalette.IsSelected = true;
            }
        }

        protected bool editingDayPalette;
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex matching digits

        public ColorPaletteDlg()
        {
            InitializeComponent();
            ApplyBindings();
            LoadColorPalettes();
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            editingDayPalette = Screen.OnDayPalette;
            if (Screen.UseDayNightTimer)
            {
                UseDayNightCheckBox.IsChecked = true;
            }
            else
            {
                UseDayNightCheckBox.IsChecked = false;
            }
            DayStartAMPM.SelectedIndex = (Screen.DayStartTime.Hour > 12) ? 1 : 0;       //sets AM/PM
            NightStartAMPM.SelectedIndex = (Screen.NightStartTime.Hour > 12) ? 1 : 0;   //Sets AM/PM
            if (Screen.OnDayPalette)
            {
                DayTimeSelector.IsSelected = true;
            }
            else
            {
                NightTimeSelector.IsSelected = true;
            }
            int hourText = Screen.DayStartTime.Hour;
            if (hourText > 12)
                hourText -= 12;
            DayStartHourBox.Text = hourText.ToString("0#");
            DayStartMinuteBox.Text = Screen.DayStartTime.Minute.ToString("0#");
            DayStartSecondBox.Text = Screen.DayStartTime.Second.ToString("0#");
            hourText = Screen.NightStartTime.Hour;
            if (hourText > 12)
                hourText -= 12;
            NightStartHourBox.Text = hourText.ToString("0#");
            NightStartMinuteBox.Text = Screen.NightStartTime.Minute.ToString("0#");
            NightStartSecondBox.Text = Screen.NightStartTime.Second.ToString("0#");
        }

        private void LoadColorPalettes()
        {
            DisplayedColorPalettes.Children.Clear();
            foreach (var palette in ConfigManager.ColorPalettes.Contents.ColorPaletteList)
            {
                var stackItem = new ColorPaletteStackItem(palette, this)
                {
                    NormalBackground = new SolidColorBrush(Colors.White),
                    HighlightedBackground = new SolidColorBrush(Colors.LightBlue),
                };
                DisplayedColorPalettes.Children.Add(stackItem);
                if (palette.Id == Screen.dayColorPaletteId)
                {
                    SelectedDayPalette = stackItem;
                }
                if (palette.Id == Screen.nightColorPaletteId)
                {
                    SelectedNightPalette = stackItem;
                }
            }
            ToggleDayNight();
        }

        private void ToggleDayNight()
        {
            if (editingDayPalette)
            {
                SelectedDayPalette = SelectedDayPalette;
            }
            else
            {
                SelectedNightPalette = SelectedNightPalette;
            }
        }

        //Called by the child ColorPaletteStackItem on a click event
        public void HandlePaletteStackItemClicked(ColorPaletteStackItem item)
        {
            if (item != null)
            {
                if (editingDayPalette)
                    SelectedDayPalette = item;
                else
                    SelectedNightPalette = item;
            }
        }


        public void DeletePalette(int paletteId)
        {
            ConfigManager.ColorPalettes.DeletePalette(paletteId);
            LoadColorPalettes();
        }

        public void Update()
        {
            LoadColorPalettes();
        }

        private void AddNewPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPalette colorPalette = new ColorPalette();
            colorPalette.LoadDefaults();
            ColorPalettePicker palettePicker = new ColorPalettePicker(colorPalette);
            palettePicker.Owner = this;
            bool dlgResult = (bool)palettePicker.ShowDialog();
            if (dlgResult)
            {
                ConfigManager.ColorPalettes.AddNewPalette(colorPalette);
                LoadColorPalettes();
            }
        }

        private void UseDayNightCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DayNightTimerSettings.Visibility = Visibility.Visible;
        }

        private void UseDayNightCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DayNightTimerSettings.Visibility = Visibility.Hidden;
        }

        private void DayTimeSelector_Selected(object sender, RoutedEventArgs e)
        {
            editingDayPalette = true;
            ToggleDayNight();
        }

        private void NightTimeSelector_Selected(object sender, RoutedEventArgs e)
        {
            editingDayPalette = false;
            ToggleDayNight();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PreviewTimeTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (SelectedDayPalette != null)
                Screen.dayColorPaletteId = SelectedDayPalette.colorPalette.Id;
            if (SelectedNightPalette != null)
                Screen.nightColorPaletteId = SelectedNightPalette.colorPalette.Id;
            SaveDayNightTimerSettings();
            Close();
        }

        private void SaveDayNightTimerSettings()
        {
            var dayStart = GetDateFromText(DayStartHourBox.Text, DayStartMinuteBox.Text, DayStartSecondBox.Text, DayStartAMPM.Text);
            var nightStart = GetDateFromText(NightStartHourBox.Text, NightStartMinuteBox.Text, NightStartSecondBox.Text, NightStartAMPM.Text);
            bool changed = false;
            if (dayStart.Hour != Screen.DayStartTime.Hour || dayStart.Minute != Screen.DayStartTime.Minute || dayStart.Second != Screen.DayStartTime.Second)
            {
                Screen.DayStartTime = dayStart;
                DayNightManager.Instance.SetDayStartTime(Screen.DayStartTime);
                changed = true;
            }
            if (nightStart.Hour != Screen.NightStartTime.Hour || nightStart.Minute != Screen.NightStartTime.Minute || nightStart.Second != Screen.NightStartTime.Second)
            {
                Screen.NightStartTime = nightStart;
                DayNightManager.Instance.SetNightStartTime(Screen.NightStartTime);
                changed = true;
            }
            if (Screen.UseDayNightTimer != (bool)UseDayNightCheckBox.IsChecked)
            {
                Screen.UseDayNightTimer = (bool)UseDayNightCheckBox.IsChecked;
                changed = true;
            }
            if (changed)
            {
                if (Screen.UseDayNightTimer)
                    DayNightManager.Instance.ActivateTimer();
                else
                    DayNightManager.Instance.DectivateTimer();
            }
        }

        private DateTime GetDateFromText(string hour, string minute, string second, string ampm)
        {
            var adjustedHour = int.Parse(hour);
            if (adjustedHour == 12)
                adjustedHour = 0;
            if (ampm == "PM")
                adjustedHour += 12;
            return new DateTime(1, 1, 1, adjustedHour, int.Parse(minute), int.Parse(second));
        }
    }
}
