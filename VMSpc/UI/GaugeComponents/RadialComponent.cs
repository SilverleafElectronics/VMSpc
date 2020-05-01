using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.UI.CustomComponents;

namespace VMSpc.UI.GaugeComponents
{
    public class RadialComponent : GaugePIDComponent
    {
        protected List<ColoredLine> GaugeLines;
        protected double minValue, maxValue;
        private int PreviousLineNumber;
        public RadialComponent(ushort pid, double minValue, double maxValue) : base(pid)
        {
            GaugeLines = new List<ColoredLine>();
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
        public override void Draw()
        {
            Children.Clear();
            GaugeLines.Clear();
            var centerPoint = GetCenterPoint(this);
            double factorDelimiter = (Width > Height) ? centerPoint.Y : centerPoint.X;
            var lineStartFactor = factorDelimiter - ((factorDelimiter / 2));
            var lineEndFactor = factorDelimiter - ((factorDelimiter / 10));
            var fillColor = new SolidColorBrush(Colors.Green);
            var emptyColor = new SolidColorBrush(Colors.Black);
            var lineSeparationFactor = GetLineSeparationFactor();
            for (double degrees = 135d; degrees < 405d; degrees += lineSeparationFactor)
            {
                var radians = ((Math.PI / 180) * degrees);
                var startX = (centerPoint.X + (lineStartFactor * Math.Cos(radians)));
                var startY = (centerPoint.Y + (lineStartFactor * Math.Sin(radians)));
                var endX = (centerPoint.X + (lineEndFactor * Math.Cos(radians)));
                var endY = (centerPoint.Y + (lineEndFactor * Math.Sin(radians)));
                ColoredLine line = new ColoredLine()
                {
                    X1 = startX,
                    X2 = endX,
                    Y1 = startY,
                    Y2 = endY,
                    StrokeThickness = 2,
                    ValueFillColor = fillColor,
                    EmptyFillColor = emptyColor,
                };
                GaugeLines.Add(line);
                Children.Add(line);
                line.UseEmptyColor();
            }
            PreviousLineNumber = 0;
        }

        private double GetLineSeparationFactor()
        {
            double LimitingFactor = Math.Min(Width, Height);
            int DecayFactor = (int)Math.Floor(LimitingFactor / 250);
            return 1 / (Math.Pow(2, DecayFactor));
        }

        public override void Update()
        {
            int cursor = PreviousLineNumber;
            int target = ValueToLineIndex(currentValue);
            bool increment = (target > cursor);
            while (cursor != target)
            {
                if (increment)
                {
                    GaugeLines[cursor].UseValueColor();
                    cursor++;
                }
                else
                {
                    GaugeLines[cursor].UseEmptyColor();
                    cursor--;
                }
            }
            PreviousLineNumber = cursor;
        }

        private int ValueToLineIndex(double value)
        {
            double range = (maxValue - minValue);
            double positionalValue = (value - minValue) / range;
            return (int)Math.Floor((positionalValue * 270));
        }

        static Point GetCenterPoint(Canvas element)
        {
            Point point = new Point()
            {
                X = element.Width / 2,
                Y = element.Height / 2
            };
            return point;
        }
    }
}
