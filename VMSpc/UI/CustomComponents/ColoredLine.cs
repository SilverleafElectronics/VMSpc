using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VMSpc.UI.CustomComponents
{

    public class ColoredLine : Shape
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public Brush ValueFillColor { get; set; }
        public Brush EmptyFillColor { get; set; }

        protected override Geometry DefiningGeometry
        {
            get
            {
                LineGeometry line = new LineGeometry(
                   new Point(X1, Y1),
                   new Point(X2, Y2)
                );
                return line;
            }
        }

        public void UseValueColor()
        {
            if (ValueFillColor != null)
                Stroke = ValueFillColor;
        }
        public void UseEmptyColor()
        {
            if (EmptyFillColor != null)
                Stroke = EmptyFillColor;
        }
        public void ToggleColor()
        {
            Stroke = (Stroke == EmptyFillColor) ? ValueFillColor : EmptyFillColor;
        }
    }
}
