using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.ComponentWrappers;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.Managers;
using static VMSpc.Constants;
using VMSpc.Common;

namespace VMSpc.UI.Panels
{
    class OdometerPanel : VPanel
    {
        protected new OdometerSettings panelSettings;
        protected StackPanel managedComponentStack;
        protected OdometerManager manager;
        protected ManagedComponentWrapper
            distanceComponent,
            fuelComponent,
            runtimeComponent,
            speedComponent,
            economyComponent;
        public OdometerPanel(MainWindow mainWindow, OdometerSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }
            
        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            canvas.Background = new SolidColorBrush(panelSettings.BackgroundColor);
            manager = new OdometerManager(panelSettings);
            managedComponentStack = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Orientation = panelSettings.orientation,
            };
            canvas.Children.Add(managedComponentStack);
            GenerateManagedComponents();
            AddManagedComponents();
            Draw();
        }

        public override void UpdatePanel() { }

        protected override VMSDialog GenerateDlg()
        {
            return new OdometerDlg(panelSettings);
        }

        private void GenerateManagedComponents()
        {
            distanceComponent = new ManagedComponentWrapper(EventIDs.DISTANCE_TRAVELLED_EVENT, manager.managerInstance)
            {
                title = "Distance",
                unitText = (panelSettings.showInMetric) ? "Kilometers" : "Miles",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
            fuelComponent = new ManagedComponentWrapper(EventIDs.FUEL_READING_EVENT, manager.managerInstance)
            {
                title = "Fuel",
                unitText = (panelSettings.showInMetric) ? "Liters" : "Gallons",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
            runtimeComponent = new ManagedComponentWrapper(EventIDs.HOURS_EVENT, manager.managerInstance)
            {
                title = "Run Time",
                unitText = "",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
            speedComponent = new ManagedComponentWrapper(EventIDs.AVERAGE_SPEED_EVENT, manager.managerInstance)
            {
                title = "Speed",
                unitText = (panelSettings.showInMetric) ? "KPH" : "MPH",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
            economyComponent = new ManagedComponentWrapper(EventIDs.CURRENT_MPG_EVENT, manager.managerInstance)
            {
                title = "Economy",
                unitText = (panelSettings.showInMetric) ? "L/100Km" : "MPG",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
        }

        private void AddManagedComponents()
        {
            if (panelSettings.showMiles)
            {
                managedComponentStack.Children.Add(distanceComponent);
            }
            if (panelSettings.showFuel)
            {
                managedComponentStack.Children.Add(fuelComponent);
            }
            if (panelSettings.showHours)
            {
                managedComponentStack.Children.Add(runtimeComponent);
            }
            if (panelSettings.showSpeed)
            {
                managedComponentStack.Children.Add(speedComponent);
            }
            if (panelSettings.showMPG)
            {
                managedComponentStack.Children.Add(economyComponent);
            }
            SetComponentDimensions();
        }

        /// <summary>
        /// Set the height/width for all components based on orientation
        /// </summary>
        protected void SetComponentDimensions()
        {
            int componentCount = managedComponentStack.Children.Count;
            if (panelSettings.orientation == Orientation.Horizontal)
            {
                foreach (ComponentCanvas component in managedComponentStack.Children)
                {
                    component.Width = canvas.Width / componentCount;
                    component.Height = canvas.Height;
                }
            }
            else if (panelSettings.orientation == Orientation.Vertical)
            {
                foreach (ComponentCanvas component in managedComponentStack.Children)
                {
                    component.Height = canvas.Height / componentCount;
                    component.Width = canvas.Width;
                }
            }
        }

        /// <summary>
        /// Draws all children of the managedComponentStack
        /// </summary>
        protected void Draw()
        {
            double maxFontSize = double.MaxValue;
            foreach (var component in managedComponentStack.Children)
            {
                var managedComponentWrapper = (component as ManagedComponentWrapper);
                if (managedComponentWrapper != null)
                {
                    managedComponentWrapper.Draw();
                    if (managedComponentWrapper.FontSize < maxFontSize)
                    {
                        maxFontSize = managedComponentWrapper.FontSize;
                    }
                }
            }
            foreach (var component in managedComponentStack.Children)
            {
                var managedComponentWrapper = (component as ManagedComponentWrapper);
                if (managedComponentWrapper != null)
                    managedComponentWrapper.FontSize = maxFontSize;
                managedComponentWrapper.HorizontalContentAlignment = panelSettings.alignment;
            }
        }
    }
}
