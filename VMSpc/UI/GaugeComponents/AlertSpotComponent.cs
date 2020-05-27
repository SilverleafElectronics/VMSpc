using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.GaugeComponents
{
    public class AlertSpotComponent : GaugePIDComponent
    {
        protected Ellipse WarningCircle;
        protected SolidColorBrush
            GreenBrush,
            YellowBrush,
            RedBrush;
        protected static SolidColorBrush OutlineBrush = new SolidColorBrush(Colors.Black);

        public AlertSpotComponent(ushort pid) 
            : base(pid)
        {
            GreenBrush = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Green);
            YellowBrush = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Yellow);
            RedBrush = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Red);
        }

        public override void DrawComponentLayout()
        {
            Children.Clear();
            var min = Math.Min(Width, Height);
            WarningCircle = new Ellipse()
            {
                Width = min,
                Height = min,
                Stroke = OutlineBrush,
                StrokeThickness = 2,
                Fill = GreenBrush
            };
            Children.Add(WarningCircle);
        }

        public override void Update()
        {
            WarningCircle.Fill = GetFillBrush();
        }

        public SolidColorBrush GetFillBrush()
        {
            if (currentValue < parameter.GaugeMin)
                return (SolidColorBrush)WarningCircle.Stroke;   //don't change the stroke if the value is invalid
            if (currentValue < parameter.LowRed)
                return RedBrush;
            else if (currentValue < parameter.LowYellow)
                return YellowBrush;
            else if (currentValue < parameter.HighYellow)
                return GreenBrush;
            else if (currentValue < parameter.HighRed)
                return YellowBrush;
            else
                return RedBrush;
        }
    }
}
