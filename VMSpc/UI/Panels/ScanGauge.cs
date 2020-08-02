using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using static VMSpc.Constants;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.UI.ComponentWrappers;
using VMSpc.Extensions.UI;
using System.Windows.Media;

namespace VMSpc.UI.Panels
{ 
    public class ScanGauge : VPanel
    {
        protected new ScanGaugeSettings panelSettings;
        protected List<BarGauge> SimpleGaugeSlider;
        protected int CurrentSimpleSliderIndex;
        Timer ToggleGaugeCounter;

        public ScanGauge(MainWindow mainWindow, ScanGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            SimpleGaugeSlider = new List<BarGauge>();
            CurrentSimpleSliderIndex = 0;
        }

        ~ScanGauge()
        {
            ToggleGaugeCounter?.Stop();
        }

        public override void GeneratePanel()
        {
            SimpleGaugeSlider.Clear();
            canvas.Children.Clear();
            CurrentSimpleSliderIndex = 0;
            canvas.Background = new SolidColorBrush(panelSettings.BackgroundColor);
            foreach (var pid in panelSettings.pidList)
            {
                AddGaugeCanvas(pid);
                CurrentSimpleSliderIndex++;
            }
            CurrentSimpleSliderIndex = 0;
            ChangeGaugeCanvas();
            ToggleGaugeCounter?.Stop();
            ToggleGaugeCounter = CREATE_TIMER(ChangeGaugeCanvas, panelSettings.scanSpeed);
        }

        private void AddGaugeCanvas(ushort pid)
        {
            var gaugeCanvas = new BarGauge(panelSettings)
            {
                Width = canvas.Width,
                Height = canvas.Height,
                pid = pid,
                showGraph = panelSettings.showGraph,
                showName = panelSettings.showName,
                showAbbreviation = panelSettings.showAbbreviation,
                showSpot = panelSettings.showSpot,
                showUnit = panelSettings.showUnit,
                showValue = panelSettings.showValue
            };
            SimpleGaugeSlider.Add(gaugeCanvas);
            canvas.Children.Add(gaugeCanvas);
            gaugeCanvas.Draw();
            gaugeCanvas.Hide();
        }

        private void ChangeGaugeCanvas()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SimpleGaugeSlider[CurrentSimpleSliderIndex].Hide();
                CurrentSimpleSliderIndex = (CurrentSimpleSliderIndex + 1) % SimpleGaugeSlider.Count;
                SimpleGaugeSlider[CurrentSimpleSliderIndex].Show();
            }
            );
        }

        public override void UpdatePanel()
        {
            //throw new NotImplementedException();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new ScanGaugeDlg(panelSettings);
        }
    }
}
