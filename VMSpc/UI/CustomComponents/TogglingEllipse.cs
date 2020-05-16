using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VMSpc.UI.CustomComponents
{
    public class TogglingEllipse : Canvas
    {
        private Ellipse Ellipse;
        /// <summary>
        /// The first color to be displayed in the ellipse on the ToggleInterval
        /// </summary>
        public SolidColorBrush ToggleBrush1 { get; set; }
        /// <summary>
        /// The second color to be displayed in the ellipse on the ToggleInterval
        /// </summary>
        public SolidColorBrush ToggleBrush2 { get; set; }
        private Timer ToggleTimer;
        public new double Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                Ellipse.Width = value;
            }
        }
        public new double Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
                Ellipse.Height = value;
            }
        }
        public int ToggleInterval 
        {  
            set
            {
                ToggleColor();
                ToggleTimer = Constants.CREATE_TIMER(ToggleColor, value);
                ToggleTimer.Start();
            }
        }
        public TogglingEllipse()
        {
            Ellipse = new Ellipse()
            {
                Width = this.Width,
                Height = this.Height,
            };
            Children.Add(Ellipse);
        }

        private void ToggleColor()
        {
            Application.Current.Dispatcher.Invoke(() =>
           {
               Ellipse.Width = Width;
               Ellipse.Height = Height;
               if (ToggleBrush1 != null && ToggleBrush2 != null)
               {
                   if (Ellipse.Fill == ToggleBrush1)
                   {
                       Ellipse.Fill = ToggleBrush2;
                   }
                   else
                   {
                       Ellipse.Fill = ToggleBrush1;
                   }
               }
           });
        }
    }
}
