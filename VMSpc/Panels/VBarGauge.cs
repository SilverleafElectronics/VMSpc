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
using VMSpc.DevHelpers;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    /// <summary> Base class of VSimpleGauge, VScanGauge, and VRoundGauge </summary>
    class VBarGauge : VPanel
    {
        private Rectangle EmptyBar;
        private Rectangle FillBar;

        private static readonly Random getrandom = new Random();

        public VBarGauge(MainWindow mainWindow, PanelSettings panelSettings)
        : base(mainWindow, panelSettings)
        {
            FillBar = new Rectangle();
            FillBar.Stroke = new SolidColorBrush(Colors.Black);
            FillBar.Fill = new SolidColorBrush(Colors.Green);
            GeneratePanel();
        }

        public override void GeneratePanel()
        {
            EmptyBar = new Rectangle();
            EmptyBar.Stroke = new SolidColorBrush(Colors.Black);
            EmptyBar.Fill = new SolidColorBrush(Colors.Black);
            DrawBar();
            DrawFillBar();
        }

        public override void UpdatePanel()
        {
           
        }

        private void DrawBar()
        {
            Canvas.SetTop(EmptyBar, 3 * (canvas.Height / 4));
            EmptyBar.Height = canvas.Height / 4;
            EmptyBar.Width = canvas.Width;
            canvas.Children.Add(EmptyBar);
        }

        private void DrawFillBar()
        {
            Canvas.SetTop(FillBar, 3 * (canvas.Height / 4));
            FillBar.Height = canvas.Height / 4;
            canvas.Children.Add(FillBar);
        }

        protected void UpdateFillBar(double value)
        {
            FillBar.Width = value;
        }

        protected void UpdateText(double value)
        {

        }
    }
}
