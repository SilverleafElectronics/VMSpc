using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;
using VMSpc.CustomComponents;
using System.Windows.Media;
using System.Windows.Controls;

namespace VMSpc.Panels
{
    class VRoundGauge : VSimpleGauge
    {
        Shape roundBar;

        public VRoundGauge(MainWindow mainWindow, RoundGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            FillBar = new Rectangle();
            EmptyBar = new Rectangle();
            roundBar = new RadialBar();
            roundBar.Width = 500;
            roundBar.Height = 500;
            roundBar.Stroke = new SolidColorBrush(Colors.Black);
            roundBar.Fill = new SolidColorBrush(Colors.Red);
        }

        public override void Init()
        {
            canvas.Children.Add(roundBar);
            Canvas.SetTop(roundBar,  (canvas.Height / 4));   //Generates a bar that fills the bottom 1/4 of the panel
            base.Init();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new RoundGaugeDlg(panelSettings);
        }

        protected override void DrawTitleText()
        {
            base.DrawTitleText();
        }
    }
}
