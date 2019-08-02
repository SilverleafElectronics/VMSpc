using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using VMSpc.DevHelpers;
using VMSpc.Parsers;
using VMSpc.DlgWindows;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.Constants;
using System.Windows.Shapes;
using System.Timers;
using System.Windows;

namespace VMSpc.Panels
{
    class VScanGauge : VPanel
    {
        protected new ScanGaugeSettings panelSettings;
        protected List<VBarGauge> barGaugeList;

        private int currentVisibilityIndex;
        private Timer scanTimer;

        public VScanGauge(MainWindow mainWindow, ScanGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            barGaugeList = new List<VBarGauge>();
            scanTimer = null;
        }

        public override void GeneratePanel()
        {
            if (NOT_NULL(scanTimer))
            {
                scanTimer.Dispose();
                scanTimer = null;
            }
            scanTimer = CREATE_TIMER(ChangeGaugeFace, panelSettings.scanSpeed * 1000);
            canvas.Children.Clear();
            barGaugeList.Clear();
            foreach (var pid in panelSettings.PIDList)
            {
                VBarGauge barGauge = new VBarGauge(new GaugePresenter(pid, panelSettings), canvas.Width, canvas.Height);
                barGaugeList.Add(barGauge);
                canvas.Children.Add(barGauge);
                barGauge.Visibility = Visibility.Hidden;
            }
            currentVisibilityIndex = 0;
            UnhideCurrentIndex();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new ScanGaugeDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
            if (barGaugeList.Count < 1) return;
            barGaugeList[currentVisibilityIndex].Update();
        }

        private void ChangeGaugeFace()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                barGaugeList[currentVisibilityIndex].Visibility = Visibility.Hidden;
                currentVisibilityIndex = (currentVisibilityIndex + 1) % barGaugeList.Count;
                UnhideCurrentIndex();
            });
        }

        private void UnhideCurrentIndex()
        {
            if (barGaugeList.Count < 1) return;
            barGaugeList[currentVisibilityIndex].Visibility = Visibility.Visible;
        }

    }
}
