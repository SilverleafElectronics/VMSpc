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
    class RadialBar : Shape
    {
        protected override Geometry DefiningGeometry => GetGeometry();

        public RadialBar()
            :base()
        {
            
        }

        private Geometry GetGeometry()
        {
            Point p1 = new Point();

            ArcSegment bottomArc = new ArcSegment();
            bottomArc.Point = new Point(100, 100);
            bottomArc.Size = new Size(50, 25);
            bottomArc.SweepDirection = SweepDirection.Clockwise;
            bottomArc.IsLargeArc = true;
            bottomArc.RotationAngle = 0;

            List<PathSegment> segments = new List<PathSegment>(1);
            segments.Add(bottomArc);

            List<PathFigure> figures = new List<PathFigure>(1);
            PathFigure pf = new PathFigure(p1, segments, true);

            Geometry g = new EllipseGeometry();//(figures, FillRule.EvenOdd, null);
            return g;
        }
    }
}
