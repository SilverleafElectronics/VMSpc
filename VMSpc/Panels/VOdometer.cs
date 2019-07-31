using System;
using VMSpc.XmlFileManagers;
using System.Windows.Controls;
using VMSpc.DlgWindows;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.SettingsManager;
using VMSpc.CustomComponents;
using System.Collections.Generic;
using System.Windows;
using VMSpc.DevHelpers;

namespace VMSpc.Panels
{
    class VOdometer : VPanel
    {

        protected new OdometerSettings panelSettings;
        protected int numCells;
        protected StackPanel odometerGrid;
        protected OdometerManager manager;
        List<TextBlock> captions;
        List<TextBlock> values;

        public VOdometer(MainWindow mainWindow, OdometerSettings panelSettings)
            : base (mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        protected override void GenerateCustomEventHandlers()
        {
            base.GenerateCustomEventHandlers();
            Canvas newCanvas = new Canvas();
            canvas.MouseDoubleClickEvent += Canvas_MouseDoubleClickEvent;
        }

        private void Canvas_MouseDoubleClickEvent(object arg1, System.Windows.Input.MouseButtonEventArgs arg2)
        {
            ResetTrip();
        }

        /// <summary>
        /// Sets the odometer file to the current values for hours, fuel, and distance
        /// </summary>
        public void ResetTrip()
        {
//            OdometerManager.AddHistoryLog(manager.)
            manager.ResetTrip();
            GeneratePanel();
        }

        public void StartFromDayOne()
        {
            manager.StartFromDayOne();
            GeneratePanel();
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
            odometerGrid = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Orientation = (panelSettings.layoutHorizontal) ? Orientation.Horizontal : Orientation.Vertical
            };
            DrawGridCells();
            canvas.Children.Add(odometerGrid);
            captions = new List<TextBlock>();
            values = new List<TextBlock>();
            foreach (OdometerCell cell in odometerGrid.Children)
            {
                captions.Add(cell.captionBlock);
                values.Add(cell.valueBlock);
            }
            canvas.BalanceTextBlocks(captions);
            canvas.BalanceTextBlocks(values);
        }

        private void DrawGridCells()
        {
            CreateGridCell(panelSettings.showMiles, "Distance", "Miles", "Kilometers", Settings.odometerPID, (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
            CreateGridCell(panelSettings.showFuelLocked, "Fuel", "Gallons", "Liters", 250, (!panelSettings.showInMetric) ? manager.startFuel : manager.startLiters);
            CreateGridCell(panelSettings.showHours, "Time", "Hrs", "Hrs", 247, manager.startHours);
            CreateGridCell(panelSettings.showSpeed, "Speed", "MPH", "KPH", 510, (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
            CreateGridCell(panelSettings.showMPG, "Economy", "MPG", "L/100Km", 509, (!panelSettings.showInMetric) ? manager.startMiles : manager.startKilometers);
        }

        private void CreateGridCell(bool create, string name, string alt_Unit, string alt_MetricUnit, ushort pid, double startVal)
        {
            if (!create) return;
            OdometerPresenter presenter = new OdometerPresenter(pid, panelSettings, manager, startVal, alt_Unit, alt_MetricUnit);
            var width = (panelSettings.layoutHorizontal) ? (odometerGrid.Width / numCells) : (odometerGrid.Width);
            var height = (panelSettings.layoutHorizontal) ? (odometerGrid.Height) : (odometerGrid.Height / numCells);
            OdometerCell cell = new OdometerCell(presenter, name, panelSettings.layoutHorizontal, width, height);
            odometerGrid.Children.Add(cell);
        }

        public override void UpdatePanel()
        {
            foreach (OdometerCell cell in odometerGrid.Children)
                cell.Update();
            canvas.BalanceTextBlocks(values);
        }
    }

    public class OdometerCell : VMSCanvas
    {
        protected OdometerPresenter presenter;
        static readonly bool LAYOUT_HORIZONTAL = true;
        static readonly bool LAYOUT_VERTICAL = false;
        protected bool layout;
        protected double textblockWidth;
        protected double textblockHeight;
        protected string title;

        public TextBlock captionBlock, valueBlock;

        public OdometerCell(OdometerPresenter presenter, string title, bool layoutHorizontal, double width, double height)
        {
            this.presenter = presenter;
            layout = layoutHorizontal;
            Width = width;
            Height = height;
            textblockWidth = (layout == LAYOUT_HORIZONTAL) ? (width) : (width / (TruthCount(presenter.showCaptions) + 1));
            textblockHeight = (layout == LAYOUT_VERTICAL) ? (height) : (height / (TruthCount(presenter.showCaptions) + 1));
            this.title = title;
            Draw();
        }

        protected void Draw()
        {
            if (presenter.showCaptions)
                DrawTitleText();
            DrawValueText();
        }

        protected void DrawTitleText()
        {
            captionBlock = new TextBlock
            {
                Text = title,
                Width = textblockWidth,
                Height = textblockHeight,
                TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition)
            };
            Children.Add(captionBlock);
            SetLeft(captionBlock, 0);
            SetTop(captionBlock, 0);
            ScaleText(captionBlock, captionBlock.Width, captionBlock.Height);
        }

        protected void DrawValueText()
        {
            valueBlock = new TextBlock
            {
                Text = presenter.ValueAsString,
                Width = textblockWidth,
                Height = textblockHeight,
                TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition)
            };
            Children.Add(valueBlock);
            if (layout == LAYOUT_HORIZONTAL)
            {
                SetLeft(valueBlock, 0);
                if (presenter.showCaptions)
                    SetTop(valueBlock, captionBlock.Height);
                else
                    SetTop(valueBlock, 0);
            }
            else
            {
                SetTop(valueBlock, 0);
                if (presenter.showCaptions)
                    SetLeft(valueBlock, captionBlock.Width);
                else
                    SetTop(valueBlock, 0);
            }
            ScaleText(valueBlock, valueBlock.Width, valueBlock.Height);
        }

        protected void UpdateValueText()
        {
            int lastLength = valueBlock.Text.Length;
            valueBlock.Text = presenter.ValueAsString;
            if (valueBlock.Text.Length > lastLength)
                ScaleText(valueBlock, valueBlock.Width, valueBlock.Height);
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
