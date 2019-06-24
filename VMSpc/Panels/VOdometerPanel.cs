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
        //private int  gridSpan;
        private Dictionary<string, TextBlock> odometerTitleValuePair;
        private Dictionary<string, string> odometerTitleStdUnit;
        private Dictionary<string, string> odometerTitleMetricUnit;

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
            odometerGrid = new Grid()
            {
                Width = canvas.Width,
                Height = canvas.Height,
            };
            canvas.Children.Add(odometerGrid);
            odometerTitleValuePair = new Dictionary<string, TextBlock>();
            odometerTitleStdUnit = new Dictionary<string, string>()
            {
                { "Distance_value", "Miles" },
                { "Fuel_value", "Gallons" },
                { "Time_value", "Hours" },
                { "Speed_value", "MPH" },
                { "Economy_value", "MPG" }

            };
            odometerTitleMetricUnit = new Dictionary<string, string>()
            {
                { "Distance_value", "Kilometers" },
                { "Fuel_value", "Liters" },
                { "Time_value", "Hours" },
                { "Speed_value", "KPH" },
                { "Economy_value", "L/100Km" }
            };
            //gridSpan = Convert.ToByte(showFuelLocked) + Convert.ToByte(showHours) + Convert.ToByte(showMiles) + Convert.ToByte(showMPG) + Convert.ToByte(showSpeed);
            GeneratePanel();
        }

        public override void GeneratePanel()
        {
            DrawGridColumns();
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
        }

        private void DrawGridColumns()
        {
            if (showCaptions)
            {
                odometerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = (new GridLength(odometerGrid.Width / 2)) });
                odometerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = (new GridLength(odometerGrid.Width / 2)) });
            }
            else
                odometerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = (new GridLength(odometerGrid.Width)) });
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
            odometerGrid.RowDefinitions.Add(new RowDefinition());
            TextBlock headerLine = new TextBlock() { Name = name, Text = name, HorizontalAlignment = HorizontalAlignment.Left };
            TextBlock valueLine = new TextBlock() { Name = name + "_value", HorizontalAlignment = HorizontalAlignment.Right };
            odometerTitleValuePair.Add(name, valueLine);

            if (showCaptions)
                odometerGrid.Children.Add(headerLine);
            odometerGrid.Children.Add(valueLine);
            if (showCaptions)
                Grid.SetRow(headerLine, odometerGrid.RowDefinitions.Count - 1);
            Grid.SetRow(valueLine, odometerGrid.RowDefinitions.Count - 1);
            if (showCaptions)
            {
                Grid.SetColumn(headerLine, 0);
                Grid.SetColumn(valueLine, 1);
            }
            else
                Grid.SetColumn(valueLine, 0);
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

        }
    }
}
