using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.JsonFileManagers;
using static VMSpc.Constants;
using VMSpc.Extensions.UI;
using VMSpc.Common;
using System.Windows;
using System.Windows.Threading;

namespace VMSpc.UI.GaugeComponents
{
    /// <summary>
    /// Component used in Odometer and TankMinder panels. These components respond to events published by a manager class
    /// </summary>
    public class ManagedComponent : GaugeComponent
    {
        protected double
            currentValue,
            cachedValue;
        private int 
            lastLength;
        private bool
            updating,
            enabled;
        protected static double
            STALE_DATA = double.NaN,
            VOID_DATA = double.MaxValue;
        public Label valueText;
        protected ulong
            eventID;
        protected byte
            publisherInstance;
        public double FontSize
        {
            get => valueText.FontSize;
            set => valueText.FontSize = value;
        }
        public HorizontalAlignment HorizontalContentAlignment
        {
            get => valueText.HorizontalContentAlignment;
            set => valueText.HorizontalContentAlignment = value;
        }
        public ManagedComponent(ulong eventID, byte publisherInstance)
            :base()
        {
            cachedValue = VOID_DATA;
            currentValue = STALE_DATA;
            this.eventID = eventID;
            this.publisherInstance = publisherInstance;
            lastLength = 0;
            Enable();
        }
        public override void Disable()
        {
            UnsubscribeFromEvent(eventID, publisherInstance);
            enabled = false;
        }

        public override void Enable()
        {
            SubscribeToEvent(eventID, publisherInstance);
            enabled = true;
        }

        public override void Draw()
        {
            Children.Clear();
            valueText = new Label()
            {
                Content = "No Data",
                Width = this.Width,
                Height = this.Height,
                FontWeight = FontWeights.Bold,
            };
            Children.Add(valueText);
            lastLength = valueText.Content.ToString().Length;
            valueText.ScaleText();
        }

        public override void Update()
        {
            if (currentValue == STALE_DATA || currentValue == VOID_DATA)
            {
                valueText.Content = "No Data";
            }
            else
            {
                valueText.Content = string.Format("{0:0.##}", currentValue);
            }
            lastLength = valueText.Content.ToString().Length;
        }

        protected override void HandleNewData(VMSEventArgs e)
        {
            var value = (e as InstancedVMSDataEventArgs)?.value;
            HandleNewData((double)value);
        }

        protected void HandleNewData(double newValue)
        {
            if (newValue == currentValue)
            {
                return;
            }
            cachedValue = newValue;
            if (!updating)
            {
                //HandleNewData() is called asynchronously each time a new data event is raised,
                while (cachedValue != VOID_DATA)
                {
                    updating = true;
                    currentValue = cachedValue;
                    cachedValue = VOID_DATA;
                    if (enabled)
                        Update();
                    updating = false;
                }
            }
        }
    }
}
