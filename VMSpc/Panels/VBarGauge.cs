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
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;

namespace VMSpc.Panels
{
    /// <summary> Base class of VSimpleGauge, VScanGauge, and VRoundGauge </summary>
    abstract class VBarGauge : VPanel
    {
        protected Rectangle EmptyBar;
        protected Rectangle FillBar;
        protected TextBlock TitleText;
        protected TextBlock ValueText;

        private static readonly Random getrandom = new Random();

        public VBarGauge(MainWindow mainWindow, PanelSettings panelSettings)
        : base(mainWindow, panelSettings)
        {
            FillBar = new Rectangle();
            EmptyBar = new Rectangle();
            TitleText = new TextBlock();
            ValueText = new TextBlock();
            canvas.Children.Add(EmptyBar);
            canvas.Children.Add(FillBar);
            canvas.Children.Add(TitleText);
            canvas.Children.Add(ValueText);

            GeneratePanel();
        }

        protected override void Init()
        {
            base.Init();
        }

        public override void GeneratePanel()
        {
            EmptyBar.Stroke = new SolidColorBrush(Colors.Black);
            EmptyBar.Fill = new SolidColorBrush(Colors.Black);
            DrawTitleText();
            DrawValueText();
            DrawBar();
            DrawFillBar();
        }

        protected override abstract VMSDialog GenerateDlg();

        //Move to VPanel?
        protected virtual void DrawTitleText()
        {
            TitleText.Text = "Turbo Boost Pressure - Extended";
            TitleText.Width = canvas.Width;
            TitleText.Height = canvas.Height / 4;
            ScaleText(TitleText, TitleText.Width, TitleText.Height); //TODO
            TitleText.VerticalAlignment = VerticalAlignment.Center;
            TitleText.TextAlignment = GET_TEXT_ALIGNMENT(panelSettings.TextPosition);
            Canvas.SetTop(TitleText, 0);
            ApplyRightBottomCoords(TitleText);
        }

        /// <summary>
        /// Generates the rectangle for positioning the gauge's value text. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void DrawValueText()
        {
            ValueText.Text = "No Data";
            ValueText.Width = canvas.Width;
            ValueText.Height = canvas.Height / 4;
            ScaleText(ValueText, ValueText.Width, ValueText.Height);
            ValueText.FontWeight = FontWeights.Bold;
            ValueText.TextAlignment = GET_TEXT_ALIGNMENT(panelSettings.TextPosition);
            Canvas.SetTop(ValueText, Canvas.GetBottom(TitleText));
            ApplyRightBottomCoords(ValueText);
        }

        /// <summary>
        /// Draws the initial empty bar. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void DrawBar()
        {
            Canvas.SetTop(EmptyBar, 3 * (canvas.Height / 4));   //Generates a bar that fills the bottom 1/4 of the panel
            EmptyBar.Height = canvas.Height / 4;
            EmptyBar.Width = canvas.Width;
        }

        /// <summary>
        /// Generates the bar used for filling the bar with color. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void DrawFillBar()
        {
            Canvas.SetTop(FillBar, 3 * (canvas.Height / 4));    //Generates a bar that fills EmptyBar
            FillBar.Height = canvas.Height / 4;
        }

        /// <summary>
        /// Updates the fill bar with the specified value. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void UpdateFillBar(double value)
        {
            if (value != DUB_NODATA)
            {
                FillBar.Width = value;
            }
        }

        /// <summary>
        /// Draws the gauge's value text
        /// </summary>
        protected void UpdateValueText(double value)
        {
            if (value != DUB_NODATA)
            {
                ValueText.Text = "" + value;
                ScaleText(ValueText, ValueText.Width, ValueText.Height);
            }
        }

        //implemented in child classes
        public override void UpdatePanel() { }
    }
}
