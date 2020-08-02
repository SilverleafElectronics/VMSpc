using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.ComponentWrappers;
using VMSpc.UI.Managers;
using VMSpc.Common;

namespace VMSpc.UI.Panels
{

    class TankMinderPanel : VPanel
    {
        protected new TankMinderSettings panelSettings;
        protected StackPanel managedComponentStack;
        protected TankMinderManager manager;
        protected ManagedComponentWrapper
            fuelComponent,
            milesToEmptyComponent,
            mpgComponent;
        public TankMinderPanel(MainWindow mainWindow, TankMinderSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            manager = new TankMinderManager(panelSettings);
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
            return new TankMinderDlg(panelSettings);
        }

        private void GenerateManagedComponents()
        {
            fuelComponent = new ManagedComponentWrapper(EventIDs.FUEL_READING_EVENT, manager.managerInstance)
            {
                title = "Fuel: ",
                unitText = (panelSettings.showInMetric) ? "Liters" : "Gallons",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
            milesToEmptyComponent = new ManagedComponentWrapper(EventIDs.DISTANCE_REMAINING_EVENT, manager.managerInstance)
            {
                title = "To Empty: ",
                unitText = (panelSettings.showInMetric) ? "Kilometers" : "Miles",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
            mpgComponent = new ManagedComponentWrapper(EventIDs.CURRENT_MPG_EVENT, manager.managerInstance)
            {
                title = "Recent: ",
                unitText = (panelSettings.showInMetric) ? "L/100Km" : "MPG",
                showTitle = panelSettings.showCaptions,
                showUnit = panelSettings.showUnits,
                orientation = (panelSettings.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal,
            };
        }

        private void AddManagedComponents()
        {
            if (panelSettings.showFuel)
            {
                managedComponentStack.Children.Add(fuelComponent);
            }
            if (panelSettings.showMilesToEmpty)
            {
                managedComponentStack.Children.Add(milesToEmptyComponent);
            }
            if (panelSettings.showMPG)
            {
                managedComponentStack.Children.Add(mpgComponent);
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
