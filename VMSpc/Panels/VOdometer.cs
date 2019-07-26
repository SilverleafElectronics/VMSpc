using System;
using VMSpc.XmlFileManagers;
using System.Windows.Controls;
using VMSpc.DlgWindows;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.SettingsManager;
using VMSpc.CustomComponents;

namespace VMSpc.Panels
{
    class VOdometer : VPanel
    {

        protected new OdometerSettings panelSettings;
        protected int numCells;
        protected Grid odometerGrid;
        protected OdometerManager manager;

        public VOdometer(MainWindow mainWindow, OdometerSettings panelSettings)
            : base (mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        protected override VMSDialog GenerateDlg()
        {
            return new OdometerDlg(panelSettings);
        }

        public override void GeneratePanel()
        {
            manager = new OdometerManager(panelSettings.fileName); 
            numCells = TruthCount(panelSettings.showFuelLocked, panelSettings.showHours, panelSettings.showMiles, panelSettings.showMPG, panelSettings.showSpeed);
            canvas.Children.Clear();
            odometerGrid = new Grid()
            {
                Width = canvas.Width,
                Height = canvas.Height
            };
            DrawGridCells();
        }

        private void DrawGridCells()
        {
            CreateGridCell(panelSettings.showMiles, "Distance", "Miles", "Kilometers", Settings.get_odometerPID(), (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
            CreateGridCell(panelSettings.showFuelLocked, "Fuel", "Gallons", "Liters", Settings.get_odometerPID(), (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
            CreateGridCell(panelSettings.showHours, "Time", Settings.get_odometerPID(), (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
            CreateGridCell(panelSettings.showSpeed, "Speed", Settings.get_odometerPID(), (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
            CreateGridCell(panelSettings.showMPG, "Economy", Settings.get_odometerPID(), (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
        }

        private void CreateGridCell(bool create, string name, ushort pid, double startVal)
        {
            if (!create) return;
            OdometerPresenter presenter = new OdometerPresenter(pid, panelSettings, startVal);
            var width = (panelSettings.layoutHorizontal) ? (odometerGrid.Width / numCells) : (odometerGrid.Width);
            var height = (panelSettings.layoutHorizontal) ? (odometerGrid.Height) : (odometerGrid.Height / numCells);
            OdometerCell cell = new OdometerCell(presenter, panelSettings.layoutHorizontal, width, height);
        }

        public override void UpdatePanel()
        {
            throw new NotImplementedException();
        }
    }

    public class OdometerCell : VMSCanvas
    {
        protected OdometerPresenter presenter;
        static readonly bool LAYOUT_HORIZONTAL = true;
        static readonly bool LAYOUT_VERTICAL = false;
        protected bool layout;

        public OdometerCell(OdometerPresenter presenter, bool layoutHorizontal, double width, double height)
        {
            this.presenter = presenter;
            layout = layoutHorizontal;
            Width = width;
            Height = height;
        }

        protected void Draw()
        {

        }

        protected void DrawTitleText()
        {

        }

        protected void DrawValueText()
        {

        }

        protected void UpdateValueText()
        {

        }

        public void Update()
        {
            if (presenter.IsValidForUpdate())
            {
                UpdateValueText();
            }
        }
    }
}
