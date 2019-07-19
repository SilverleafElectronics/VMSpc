using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Parsers;
using VMSpc.XmlFileManagers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.DlgWindows;

namespace VMSpc.Panels
{
    class VOdometerPanel : VPanel
    {
        private Odometer odometer;
        private bool showCaptions,
                     showUnits,
                     showFuelLocked,
                     showHours,
                     showMiles,
                     showMPG,
                     showSpeed,
                     showInMetric,
                     layoutHorizontal;
        private Grid odometerGrid;
        private int  gridSpan;
        private double cellWidth, cellHeight;
        private Dictionary<string, TextBlock> odometerTitleValuePair;

        public VOdometerPanel(MainWindow mainWindow, OdometerSettings panelSettings)
            : base (mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            odometer = new Odometer(panelSettings.fileName);
            showCaptions = panelSettings.showCaptions;
            showUnits = panelSettings.showUnits;
            showFuelLocked = panelSettings.showFuelLocked;
            showHours = panelSettings.showHours;
            showMiles = panelSettings.showMiles;
            showMPG = panelSettings.showMPG;
            showSpeed = panelSettings.showSpeed;
            showInMetric = panelSettings.showInMetric;
            layoutHorizontal = panelSettings.layoutHorizontal;
            GeneratePanel();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new OdometerDlg(panelSettings);
        }

        public override void GeneratePanel()
        {
            canvas.Children.RemoveRange(0, canvas.Children.Count);
            odometerGrid = new Grid();
            canvas.Children.Add(odometerGrid);
            gridSpan = Convert.ToByte(showFuelLocked) + Convert.ToByte(showHours) + Convert.ToByte(showMiles) + Convert.ToByte(showMPG) + Convert.ToByte(showSpeed);
            odometerGrid.Children.RemoveRange(0, odometerGrid.Children.Count);
            odometerGrid.Width = canvas.Width;
            odometerGrid.Height = canvas.Height;
            odometerTitleValuePair = new Dictionary<string, TextBlock>();
            DrawGridRows();
        }

        public override void UpdatePanel()
        {
            odometer.CalculateValues();
            DrawValues(
                odometer.currentTripHours, 
                odometer.currentTripMiles, 
                odometer.currentTripFuel, 
                odometer.currentTripLiters, 
                odometer.currentTripKilometers,
                odometer.currentTripMPG,
                odometer.currentTripLPK,
                odometer.currentTripMPH,
                odometer.currentTripKPH
                );
            canvas.BalanceTextBlocks(odometerGrid);
        }

        private void DrawGridRows()
        {
            if (showMiles)
                AddRow("Distance");
            if (showFuelLocked)
                AddRow("Fuel");
            if (showHours)
                AddRow("Time");
            if (showSpeed)
                AddRow("Speed");
            if (showMPG)
                AddRow("Economy");
        }

        private void AddRow(string name)
        {
            AddTextBlocks(out TextBlock headerLine, out TextBlock valueLine, out Border headerBorder, out Border valueBorder, name);
            SetTextBlockPositions(headerBorder, valueBorder);
            odometerTitleValuePair.Add(name, valueLine);
            cellHeight = valueBorder.Height;
            cellWidth = valueBorder.Width;
            canvas.ScaleText(headerLine, headerBorder.Width, headerBorder.Height);
        }

        /// <summary>
        /// Creates the headerLine (optional caption) and valueLine and adds them to the odometer panel
        /// </summary>
        private void AddTextBlocks(out TextBlock headerLine, out TextBlock valueLine, out Border headerBorder, out Border valueBorder, string name)
        {
            var headerHzPos = (layoutHorizontal) ? HorizontalAlignment.Center : HorizontalAlignment.Left;
            var valueHzPos = (layoutHorizontal || !showCaptions) ? HorizontalAlignment.Center : HorizontalAlignment.Right;
            var headerVertPos = (layoutHorizontal) ? VerticalAlignment.Top : VerticalAlignment.Center;
            var valueVertPos = (layoutHorizontal || !showCaptions) ? VerticalAlignment.Bottom : VerticalAlignment.Center;

            headerBorder = new Border() { HorizontalAlignment = headerHzPos, VerticalAlignment = headerVertPos };
            valueBorder = new Border()  { HorizontalAlignment = valueHzPos,  VerticalAlignment = valueVertPos };
            headerBorder.Width = valueBorder.Width = (layoutHorizontal) ? (odometerGrid.Width / gridSpan) : (odometerGrid.Width / 2);
            headerBorder.Height = valueBorder.Height = (layoutHorizontal) ? (odometerGrid.Height / 2) : (odometerGrid.Height / gridSpan);


            if (layoutHorizontal)
            {
                headerBorder.BorderThickness = new Thickness(2, 0, 2, 0);
                valueBorder.BorderThickness = new Thickness(2, 0, 2, 0);
            }
            else
            {
                headerBorder.BorderThickness = new Thickness(0, 2, 0, 2);
                valueBorder.BorderThickness = new Thickness(0, 2, 0, 2);
            }
            headerLine = new TextBlock() { Name = name, Text = name };
            valueLine = new TextBlock() { Name = name + "_value" };
            headerLine.Width = headerBorder.Width;
            valueLine.Width = valueBorder.Width;

            headerLine.TextAlignment = valueLine.TextAlignment = TextAlignment.Center;
            headerLine.VerticalAlignment = valueLine.VerticalAlignment = VerticalAlignment.Center;

            headerBorder.Child = headerLine;
            valueBorder.Child = valueLine;

            if (showCaptions)
                odometerGrid.Children.Add(headerBorder);
            odometerGrid.Children.Add(valueBorder);
        }

        /// <summary>
        /// Assigns the position of the header line item (caption) and the corresponding value
        /// </summary>
        private void SetTextBlockPositions(UIElement headerLine, UIElement valueLine)
        {
            //Decide whether the line item's orientation is horizontal or vertical - assigns function pointer accordingly
            Action<UIElement, int> mainGridSet = ((layoutHorizontal) ? (Action<UIElement, int>)Grid.SetColumn : Grid.SetRow);
            Action<UIElement, int> subGridSet = ((layoutHorizontal) ? (Action<UIElement, int>)Grid.SetRow : Grid.SetColumn);
            if (layoutHorizontal)
                odometerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            else
                odometerGrid.RowDefinitions.Add(new RowDefinition());
            int mainGridItemCount = (layoutHorizontal) ? odometerGrid.ColumnDefinitions.Count : odometerGrid.RowDefinitions.Count;

            if (showCaptions)
                mainGridSet(headerLine, mainGridItemCount - 1);
            mainGridSet(valueLine, mainGridItemCount - 1);
            if (showCaptions)
            {
                subGridSet(headerLine, 0);
                subGridSet(valueLine, 1);
            }
            else
                subGridSet(valueLine, 0);
        }

        private void DrawValues(double hours, double miles, double fuel, double liters, double kilometers, double mpg, double lpk, double mph, double kph)
        {
            string valueString;
            if (showHours)
            {
                double minutes = (hours % 1) * 60;
                double roundedHours = (hours - (hours % 1));
                valueString = "" + roundedHours + ":" + minutes;
                odometerTitleValuePair["Time"].Text = valueString;
            }
            if (showMiles)
            {
                valueString = String.Format("{0:0.00} ", ((showInMetric) ? kilometers : miles));
                if (showUnits) valueString += (showInMetric) ? "Kilometers" : "Miles";
                odometerTitleValuePair["Distance"].Text = valueString;
            }
            if (showFuelLocked)
            {
                valueString = "" + ((showInMetric) ? liters : fuel);
                if (showUnits) valueString += (showInMetric) ? "Liters" : "Gallons";
                odometerTitleValuePair["Fuel"].Text = valueString;
            }
            if (showSpeed)
            {
                valueString = "" + ((showInMetric) ? kph : mph);
                if (showUnits) valueString += (showInMetric) ? "MPH" : "MPG";
                odometerTitleValuePair["Speed"].Text = valueString;
            }
            if (showMPG)
            {
                valueString = "" + ((showInMetric) ? lpk : mpg);
                if (showUnits) valueString += (showInMetric) ? "MPG" : "L/100KM";
                odometerTitleValuePair["Economy"].Text = valueString;
            }
            foreach(TextBlock textBlock in odometerTitleValuePair.Values)
                canvas.ScaleText(textBlock, cellWidth, cellHeight);
        }
    }
}
