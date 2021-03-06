﻿using System;
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
    public class Clock : VPanel
    {
        protected new ClockSettings panelSettings;
        private ClockComponent clockComponent;
        public Clock(MainWindow mainWindow, ClockSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            if (panelSettings.useGlobalColorPalette)
            {
                canvas.Background = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().GaugeBackground);
            }
            else
            {
                canvas.Background = new SolidColorBrush(panelSettings.BackgroundColor);
            }
            clockComponent = new ClockComponent(panelSettings.useMilitaryTime, panelSettings.showAmPm, panelSettings.showDate)
            {
                Width = canvas.Width,
                Height = canvas.Height,
                TextAlignment = TextAlignment.Center,
            };
            canvas.Children.Add(clockComponent);
        }

        public override void UpdatePanel()
        {
        }

        protected override VMSDialog GenerateDlg()
        {
            return new ClockDlg(panelSettings);
        }
    }
}
