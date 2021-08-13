using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Extensions.UI;
using System.Windows.Media;
using System.Windows;
namespace VMSpc.UI.Panels
{
    class DayNightGauge : VPanel
    {
        private Label TogglePaletteButton;
        private StackPanel ButtonStack;
        protected new DayNightGaugeSettings panelSettings;

        private static SolidColorBrush ButtonColor = new SolidColorBrush(Colors.LightGray);
        private static SolidColorBrush ButtonHoverColor = new SolidColorBrush(Colors.LightBlue);

        public DayNightGauge(MainWindow mainWindow, DayNightGaugeSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            ButtonStack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Width = canvas.Width,
                Height = canvas.Height,
            };
            canvas.Children.Add(ButtonStack);
            TogglePaletteButton = new Label()
            {
                Content = (DayNightManager.Instance.IsDay()) ? "Use Night Palette" : "Use Day Palette",
                Width = canvas.Width,
                Height = canvas.Height,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = ButtonColor,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Colors.Black),
            };
            ButtonStack.Children.Add(TogglePaletteButton);
            TogglePaletteButton.ScaleText();

            TogglePaletteButton.MouseEnter += TogglePaletteButton_MouseEnter;
            TogglePaletteButton.MouseLeave += TogglePaletteButton_MouseLeave;
            TogglePaletteButton.MouseLeftButtonUp += TogglePaletteButton_MouseLeftButtonUp;
        }

        private void TogglePaletteButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DayNightManager.Instance.IsDay())
                DayNightManager.Instance.SetNight();
            else
                DayNightManager.Instance.SetDay();
            TogglePaletteButton.Content = (DayNightManager.Instance.IsDay()) ? "Use Night Palette" : "Use Day Palette";
        }

        private void TogglePaletteButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TogglePaletteButton.Background = ButtonColor;
        }

        private void TogglePaletteButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TogglePaletteButton.Background = ButtonHoverColor;
        }

        public override void UpdatePanel()
        {
        }

        protected override VMSDialog GenerateDlg()
        {
            return new DayNightGaugeDlg((DayNightGaugeSettings)panelSettings);
        }
    }
}
