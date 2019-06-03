using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.DevHelpers;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    /// <summary>
    /// base class of VSimpleGauge, VScanGauge, and VRoundGauge
    /// </summary>
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
           // UpdateFillBar(getrandom.Next(0, (int)canvas.Width)); //CHANGEME - get value from corresponding parser
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

        private void UpdateFillBar(double value)
        {
            FillBar.Width = value;
        }

        private void UpdateText(double value)
        {

        }
    }
}
