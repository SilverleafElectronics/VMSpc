using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.DevHelpers;
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.GaugeComponents
{
    public class BarComponent : GaugePIDComponent
    {
        protected List<ColoredLine> GaugeLines;
        protected double minGaugeValue, maxGaugeValue;
        protected int numLines;
        protected int previousLineNumber;
        protected ColorDelimiter[] MaxColorBorders;
        //these are only customizable at the global level
        private readonly SolidColorBrush
            RedBrush = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Red),
            YellowBrush = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Yellow),
            GreenBrush = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Green);
        //Not yet implemented - these depend on configuration. SolidFillColor is only used in configs that don't use Red/Yellow/Green schemes
        public SolidColorBrush
            EmptyColor,
            SolidValueColor;

        public Orientation Orientation { get; set; }

        public BarComponent(ushort pid) : base(pid)
        {
            GaugeLines = new List<ColoredLine>();
            minGaugeValue = parameter.GaugeMin;
            maxGaugeValue = parameter.GaugeMax;
            EmptyColor = new SolidColorBrush(Colors.Black);
            SolidValueColor = null;
            MaxColorBorders = new ColorDelimiter[]
            {
                new ColorDelimiter(parameter.LowRed, RedBrush),
                new ColorDelimiter(parameter.LowYellow, YellowBrush),
                new ColorDelimiter(parameter.HighYellow, GreenBrush),
                new ColorDelimiter(parameter.HighRed, YellowBrush),
                new ColorDelimiter(parameter.GaugeMax, RedBrush)
            };
        }
        public override void DrawComponentLayout()
        {
            Children.Clear();
            GaugeLines.Clear();
            previousLineNumber = 0;
            AddLinesByColor();
        }

        public override void Update()
        {
            int cursor = previousLineNumber;
            int target = ValueToLineIndex(currentValue);
            bool increment = (target > cursor);
            if (cursor > numLines)
                cursor = GaugeLines.Count;
            int startCursor = cursor;
            while (cursor != target)
            {
                try
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
                catch (Exception ex)
                {
                    VMSConsole.PrintLine(ex);
                    VMSConsole.PrintLine(startCursor);
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
                if (SolidValueColor == null) //adds lines on a red-yellow-green scheme
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
                else //adds the designated SolidValueColor
                {
                    AddLine(i, SolidValueColor);
                }
            }
        }

        /// <summary>
        /// Generates a line to be added to the canvas. Line color is determined by the position on the grid
        /// </summary>
        protected void AddLine(int position, SolidColorBrush ValueFillColor)
        {
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
            return (Orientation == Orientation.Horizontal) ? position : 0;
        }
        private double ResolveX2(int position)
        {
            return (Orientation == Orientation.Horizontal) ? position : Width;
        }
        private double ResolveY1(int position)
        {
            return (Orientation == Orientation.Vertical) ? (Height - position) : Height;
        }
        private double ResolveY2(int position)
        {
            return (Orientation == Orientation.Vertical) ? (Height - position) : 0;
        }

        private int ValueToLineIndex(double value)
        {
            if (range == 0)
            {
                return 0;
            }
            if (value > maxGaugeValue)
            {
                return numLines - 1;
            }
            double positionalValue = (value - minGaugeValue) / range;
            int lineIndex = (int)Math.Floor((positionalValue * numLines));
            if (lineIndex < 0)
                lineIndex = 0;
            if (lineIndex >= numLines)
                lineIndex = numLines - 1;
            return lineIndex;
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
