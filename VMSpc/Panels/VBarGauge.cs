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
        protected double
            MaxGreenWidth,
            MaxYellowWidth,
            MaxRedWidth;

        protected TextBlock TitleText;
        protected TextBlock ValueText;
        protected VParameter parameter;
        protected ParamPresenter presenter;

        protected double
            graphHeight,
            titleHeight,
            valueHeight;

         
        protected double lastValue;

        public VBarGauge(ParamPresenter presenter, double width, double height)
        {
            this.presenter = presenter;
            Width = width;
            Height = height;
            if (presenter.showName)
                titleHeight = (!(presenter.showUnit || presenter.showValue || presenter.showGraph)) ? height : (height / (2 * TruthCount(presenter.showGraph, (presenter.showValue || presenter.showUnit))));
            else titleHeight = 0;
            if (presenter.showGraph)
                graphHeight = (presenter.showName || presenter.showUnit || presenter.showValue) ? (height / 4) : height;
            else graphHeight = 1;
            if (presenter.showValue || presenter.showUnit)
                valueHeight = (!(presenter.showName || presenter.showGraph)) ? height : (height / (2 * TruthCount(presenter.showGraph, presenter.showName)));
            else valueHeight = 0;
            TitleText = new TextBlock();
            ValueText = new TextBlock();
            EmptyBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Black), Height = graphHeight, Width = Width };
            GreenFillBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Green), Height = EmptyBar.Height - 1 };
            YellowFillBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Yellow), Height = EmptyBar.Height - 1 };
            RedFillBar = new Rectangle() { Fill = new SolidColorBrush(Colors.Red), Height = EmptyBar.Height - 1 };
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

        protected virtual void DoNothing() { }

        protected virtual void DrawTitleText()
        {
            if (!presenter.showName) return;

            TitleText.Width = Width;
            TitleText.Height = titleHeight;
            TitleText.VerticalAlignment = VerticalAlignment.Center;
            TitleText.TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition);
            TitleText.Text = presenter.Title;
            ScaleText(TitleText, TitleText.Width, TitleText.Height);
            SetTop(TitleText, 0);
        }

        protected virtual void DrawValueText()
        {
            if (!presenter.showValue && !presenter.showUnit) return;

            ValueText.Text = presenter.ValueAsString;
            ValueText.Width = Width;
            ValueText.Height = valueHeight;
            ValueText.FontWeight = FontWeights.Bold;
            ValueText.TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition);
            ScaleText(ValueText, ValueText.Width, ValueText.Height);
            SetTop(ValueText, Height - (graphHeight + valueHeight));
        }

        protected virtual void DrawBar()
        {
            if (!presenter.showGraph) return;
            SetTop(EmptyBar, (Height - graphHeight));
            //SetTop(EmptyBar, (3 * (Height / 4)));
        }

        protected virtual void DrawFillBars()
        {
            if (!presenter.showGraph) return;

            SetTop(GreenFillBar, GetTop(EmptyBar) + 1);
            SetTop(YellowFillBar, GetTop(EmptyBar) + 1);
            SetTop(RedFillBar, GetTop(EmptyBar) + 1);
            SetLeft(GreenFillBar, 0);
            SetLeft(YellowFillBar, (presenter.GreenMaxAsPercent * Width));
            SetLeft(RedFillBar, (presenter.YellowMaxAsPercent * Width));
        }

        public void Update()
        {
            if (presenter.IsValidForUpdate())
            {
                UpdateValueText();
                UpdateFillBars();
            }
        }

        protected virtual void UpdateFillBars()
        {
            if (!presenter.showGraph) return;

            double 
            GreenFillLevel  = (presenter.ValueAsPercent < presenter.GreenMaxAsPercent)  ? (presenter.ValueAsPercent                               ) : presenter.GreenMaxAsPercent,
            YellowFillLevel = (presenter.ValueAsPercent < presenter.YellowMaxAsPercent) ? (presenter.ValueAsPercent - presenter.GreenMaxAsPercent ) : presenter.YellowMaxAsPercent - presenter.GreenMaxAsPercent,
            RedFillLevel    = (presenter.ValueAsPercent < presenter.RedMaxAsPercent)    ? (presenter.ValueAsPercent - presenter.YellowMaxAsPercent) : presenter.RedMaxAsPercent - presenter.YellowMaxAsPercent;

            if (GreenFillLevel >= 0) GreenFillBar.Width = (Width * GreenFillLevel);
            else GreenFillBar.Width = 0;
            if (YellowFillLevel >= 0) YellowFillBar.Width = (Width * YellowFillLevel);
            else YellowFillBar.Width = 0;
            if (RedFillLevel >= 0) RedFillBar.Width = (Width * RedFillLevel);
            else RedFillBar.Width = 0;
        }

        protected void UpdateValueText()
        {
            if (!presenter.showValue && !presenter.showUnit) return;
            ValueText.Text = presenter.ValueAsString;
        }
    }
}
