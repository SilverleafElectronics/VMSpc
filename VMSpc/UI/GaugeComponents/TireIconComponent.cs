using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using VMSpc.Enums.Parsing;
using VMSpc.Extensions.UI;
using VMSpc.Common;

namespace VMSpc.UI.GaugeComponents
{
    class TireIconComponent : GaugeComponent
    {
        private Ellipse tireIndicator;
        private TextBlock tirePressureIndicator;
        private static SolidColorBrush
            NoDataColor = new SolidColorBrush(Colors.White),
            OkColor = new SolidColorBrush(Colors.Green),
            WarningColor = new SolidColorBrush(Colors.Yellow),
            AlertColor = new SolidColorBrush(Colors.Red);
        private TireStatus
            currentStatus,
            lastStatus;
        private double 
            currentPressure,
            lastPressure;
        private readonly int tireIndex;
        public TireIconComponent(int index) 
            : base()
        {
            currentStatus = TireStatus.NO_SENSOR_ASSIGNED;
            lastStatus = TireStatus.NO_REPORT;
            currentPressure = 0;
            lastPressure = -1;
            tireIndex = index;
            SubscribeToEvent(EventIDs.TIRE_BASE | (uint)tireIndex);
        }
        public override void Draw()
        {
            AddTireIndicator();
            AddTirePressureIndicator();
            Update();
        }

        public void AddTireIndicator()
        {
            //The ellipse must have equal height and width to form a circle, so we set both equal to the minimum
            double min = Math.Min(Width, Height);
            tireIndicator = new Ellipse()
            {
                Width = min,
                Height = min,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2,
            };
            Children.Add(tireIndicator);
        }

        public void AddTirePressureIndicator()
        {
            double min = Math.Min(Width, Height);
            //Add a border to allow vertical centering
            Border border = new Border()
            { 
                BorderBrush = null,
                Height = min,
                Width = min,
            };
            tirePressureIndicator = new TextBlock()
            {
                //Width = min,
                //note that Height should not be specified. Otherwise, vertical alignment won't work
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black),
            };
            border.Child = tirePressureIndicator;
            Children.Add(border);
            tirePressureIndicator.ScaleText(border.Width, border.Height, 3);
        }

        public override void Update()
        {
            UpdateIndicator();
            UpdatePressure();
        }

        private void UpdateIndicator()
        {
            if ((tireIndicator != null) && (currentStatus != lastStatus))
            {
                switch (currentStatus)
                {
                    case TireStatus.ALERT:
                        tireIndicator.Fill = AlertColor;
                        break;
                    case TireStatus.WARNING:
                        tireIndicator.Fill = WarningColor;
                        break;
                    case TireStatus.OK:
                        tireIndicator.Fill = OkColor;
                        break;
                    case TireStatus.NO_REPORT:
                    case TireStatus.NO_SENSOR_ASSIGNED:
                        tireIndicator.Fill = NoDataColor;
                        break;
                }
            }
        }

        private void UpdatePressure()
        {
            if ((tirePressureIndicator != null) && (currentPressure != lastPressure))
            {
                tirePressureIndicator.Text = $"{currentPressure}";
                lastPressure = currentPressure;
            }
        }

        protected override void HandleNewData(VMSEventArgs e)
        {
            var dataEvent = (e as TireEventArgs);
            if (dataEvent != null)
            {
                currentPressure = dataEvent.tire.DisplayPressure;
                currentStatus = dataEvent.tire.Status;
                Update();
            }
        }

        public override void Enable()
        {
            //throw new NotImplementedException();
        }

        public override void Disable()
        {
            //throw new NotImplementedException();
        }
    }
}
