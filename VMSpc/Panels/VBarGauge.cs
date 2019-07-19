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
using VMSpc.CustomComponents;
using VMSpc.DevHelpers;
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.ParamDataManager;

namespace VMSpc.Panels
{
    /// <summary> Base class of VSimpleGauge, VScanGauge, and VRoundGauge </summary>
    public class VBarGauge : VMSCanvas
    {
        protected Rectangle EmptyBar;

        protected Rectangle GreenFillBar;
        protected Rectangle YellowFillBar;
        protected Rectangle RedFillBar;
        protected double MaxGreenWidth;
        protected double MaxYellowWidth;
        protected double MaxRedWidth;

        protected TextBlock TitleText;
        protected TextBlock ValueText;
        protected VParameter parameter;
        protected ParamPresenter presenter;

        protected double lastValue;

        public VBarGauge(ParamPresenter presenter, double width, double height)
        {
            this.presenter = presenter;
            Width = width;
            Height = height;
            TitleText = new TextBlock()
            {
                Width = Width,
                Height = (Height / 4),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition)
            };
            ValueText = new TextBlock()
            {
                Text = "No Data",
                Width = Width,
                Height = (Height / 4),
                FontWeight = FontWeights.Bold,
                TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition)
            };
            EmptyBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Black), Height = (Height / 4), Width = Width };
            GreenFillBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Green), Stroke = new SolidColorBrush(Colors.Black), Height = (Height / 4) };
            YellowFillBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Yellow), Stroke = new SolidColorBrush(Colors.Black), Height = (Height / 4) };
            RedFillBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Red), Stroke = new SolidColorBrush(Colors.Black), Height = (Height / 4) };
            AddChildren(EmptyBar, GreenFillBar, YellowFillBar, RedFillBar, TitleText, ValueText);
            Draw();
        }

        private void Draw()
        {
            DrawTitleText();
            DrawValueText();
            DrawBar();
            DrawFillBars();
        }

        protected virtual void DrawTitleText()
        {
            TitleText.Text = presenter.Title;
            ScaleText(TitleText, TitleText.Width, TitleText.Height);
            SetTop(TitleText, 0);
        }

        protected virtual void DrawValueText()
        {
            ScaleText(ValueText, ValueText.Width, ValueText.Height);
            SetTop(ValueText, TitleText.Height);
        }

        protected virtual void DrawBar()
        {
            SetTop(EmptyBar, 3 * (Height / 4));
        }

        protected virtual void DrawFillBars()
        {
            SetTop(GreenFillBar, 3 * (Height / 4));
            SetTop(YellowFillBar, 3 * (Height / 4));
            SetTop(RedFillBar, 3 * (Height / 4));
            SetLeft(GreenFillBar, 0);
            SetLeft(YellowFillBar, (presenter.GreenMaxAsPercent * Width));
            SetLeft(RedFillBar, (presenter.YellowMaxAsPercent * Width));
        }

        public void Update()
        {
            if (presenter.IsValidForUpdate())
            {
                UpdateFillBars();
                UpdateValueText();
            }
        }

        protected virtual void UpdateFillBars()
        {
            double 
            GreenFillLevel  = (presenter.ValueAsPercent() < presenter.GreenMaxAsPercent)  ? (presenter.ValueAsPercent()                               ) : presenter.GreenMaxAsPercent,
            YellowFillLevel = (presenter.ValueAsPercent() < presenter.YellowMaxAsPercent) ? (presenter.ValueAsPercent() - presenter.GreenMaxAsPercent ) : presenter.YellowMaxAsPercent,
            RedFillLevel    = (presenter.ValueAsPercent() < presenter.RedMaxAsPercent)    ? (presenter.ValueAsPercent() - presenter.YellowMaxAsPercent) : presenter.RedMaxAsPercent;

            if (GreenFillLevel >= 0) GreenFillBar.Width = (Width * GreenFillLevel);
            else GreenFillBar.Width = 0;
            if (YellowFillLevel >= 0) YellowFillBar.Width = (Width * YellowFillLevel);
            else YellowFillBar.Width = 0;
            if (RedFillLevel >= 0) RedFillBar.Width = (Width * RedFillLevel);
            else RedFillBar.Width = 0;

        }

        protected void UpdateValueText()
        {
            ValueText.Text = presenter.ValueAsString;
        }
    }
}
