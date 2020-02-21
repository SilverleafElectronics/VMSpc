using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;

namespace VMSpc.UI.GaugeComponents
{
    public class BarComponent : GaugePIDComponent
    {
        protected List<ColoredLine> GaugeLines;
        protected double minGaugeValue, maxGaugeValue;
        protected int numLines;
        protected int previousLineNumber;
        protected ColorDelimiter[] MaxColorBorders;
        //these are not customizable
        private static readonly SolidColorBrush
            RedBrush = new SolidColorBrush(Colors.Red),
            YellowBrush = new SolidColorBrush(Colors.Yellow),
            GreenBrush = new SolidColorBrush(Colors.Green);
        //these depend on configuration. SolidFillColor is only used in configs that don't use Red/Yellow/Green schemes
        public SolidColorBrush
            EmptyColor,
            SolidValueColor;

        public Orientation Orientation { get; set; }

        public BarComponent(ushort pid, JParameter parameter) : base(pid)
        {
            GaugeLines = new List<ColoredLine>();
            minGaugeValue = parameter.GaugeMin;
            maxGaugeValue = parameter.GaugeMax;
            EmptyColor = new SolidColorBrush(Colors.Black);
            SolidValueColor = new SolidColorBrush(Colors.Green);
            MaxColorBorders = new ColorDelimiter[]
            {
                new ColorDelimiter(parameter.LowRed, RedBrush),
                new ColorDelimiter(parameter.LowYellow, YellowBrush),
                new ColorDelimiter(parameter.HighYellow, GreenBrush),
                new ColorDelimiter(parameter.HighRed, YellowBrush),
                new ColorDelimiter(parameter.GaugeMax, RedBrush)
            };
        }
        public override void Draw()
        {
            Children.Clear();
            GaugeLines.Clear();
            previousLineNumber = 0;
            AddLinesByColor();
        }

        public override void Update()
        {
            int cursor = previousLineNumber;
            int target = ValueToLineIndex(pidValue);
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
            previousLineNumber = cursor;
        }

        /// <summary>
        /// Adds each value-dependent line to the canvas
        /// </summary>
        protected void AddLinesByColor()
        {
            numLines = (int)((Orientation == Orientation.Horizontal) ? Width : Height);
            var ColorDelimiterIndex = 0;
            for (int i = 0; i < numLines; i++)
            {
                while ((LineIndexToValue(i) >= MaxColorBorders[ColorDelimiterIndex].MaxValue) || (MaxColorBorders[ColorDelimiterIndex].MaxValue == 0))
                {
                    if (ColorDelimiterIndex < MaxColorBorders.Length)
                    {
                        ColorDelimiterIndex++;
                    }
                }
                AddLine(i, MaxColorBorders[ColorDelimiterIndex].Brush);
            }
        }

        /// <summary>
        /// Generates a line to be added to the canvas. Line color is determined by the position on the grid
        /// </summary>
        protected void AddLine(int position, SolidColorBrush ValueFillColor)
        {
            var y1 = ResolveY1(position);
            var y2 = ResolveY2(position);
            ColoredLine line = new ColoredLine()
            {
                StrokeThickness = 2,
                EmptyFillColor = EmptyColor,
                ValueFillColor = ValueFillColor,
                X1 = ResolveX1(position),
                X2 = ResolveX2(position),
                Y1 = ResolveY1(position),
                Y2 = ResolveY2(position),
            };
            GaugeLines.Add(line);
            Children.Add(line);
            line.UseEmptyColor();
        }

        private double ResolveX1(int position)
        {
            return (Orientation == Orientation.Horizontal) ? position : Width;
        }
        private double ResolveX2(int position)
        {
            return (Orientation == Orientation.Horizontal) ? position : 0;
        }
        private double ResolveY1(int position)
        {
            return (Orientation == Orientation.Vertical) ? position : Height;
        }
        private double ResolveY2(int position)
        {
            return (Orientation == Orientation.Vertical) ? position : 0;
        }

        private int ValueToLineIndex(double value)
        {
            double positionalValue = (value - minGaugeValue) / range;
            return (int)Math.Floor((positionalValue * numLines));
        }

        private double LineIndexToValue(int lineIndex)
        {
            var valuePerLine = range / numLines;
            return valuePerLine * lineIndex;
        }

        private double range => maxGaugeValue - minGaugeValue;
    }

    public struct ColorDelimiter
    {
        public double MaxValue;
        public SolidColorBrush Brush;
        public ColorDelimiter(double MaxValue, SolidColorBrush Brush)
        {
            this.MaxValue = MaxValue;
            this.Brush = Brush;
        }
    }
}
