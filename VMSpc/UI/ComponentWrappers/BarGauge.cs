using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.Extensions.UI;
using VMSpc.JsonFileManagers;
using VMSpc.UI.CustomComponents;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.ComponentWrappers
{
    public class BarGauge : ComponentCanvas
    {
        public bool
            showGraph,
            showName,
            showAbbreviation,
            showSpot,
            showUnit,
            showValue;
        public ushort pid;
        public int borderEdgePadding = 2;
        protected StackPanel titlePanel;

        protected AlertSpotComponent alertSpot;

        protected bool hasGaugeBlock => showGraph;
        protected bool hasTitleText => (showName || showSpot);
        protected bool hasValueText => (showValue || showUnit);
        protected GaugeSettings gaugeSettings;

        public BarGauge(GaugeSettings gaugeSettings) 
            : base()
        {
            this.gaugeSettings = gaugeSettings;
        }

        public void Draw()
        {
            if (showName || showSpot)
            {
                AddTitleElements();
            }
            if (showGraph)
            {
                AddGraph();
            }
            if (showUnit || showValue)
            {
                AddValueTextBlock();
            }
        }

        protected void AddTitleElements()
        {
            titlePanel = new StackPanel()
            {
                Width = Width,
                Height = GetTitleHeight(),
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = gaugeSettings.alignment,
            };
            Children.Add(titlePanel);
            SetLeft(titlePanel, 0);
            SetTop(titlePanel, 0);
            if (showName)
            {
                AddGaugeTitle();
            }
            if (showSpot)
            {
                AddAlertSpot();
            }
        }

        protected void AddAlertSpot()
        {
            alertSpot = new AlertSpotComponent(pid)
            {
                Width = GetAlertSpotWidth(),
                Height = titlePanel.Height,
            };
            titlePanel.Children.Add(alertSpot);
            alertSpot.Draw();
            SetLeft(alertSpot, 0);
            SetTop(alertSpot, 0);
        }

        protected double GetAlertSpotWidth()
        {
            if (!showName)
            {
                return Width;
            }
            else
            {
                return Math.Min((Width / 5d), titlePanel.Height);
            }
        }

        protected void AddGaugeTitle()
        {
            var titleBlock = new TextBlock()
            {
                Width = GetTitleWidth(),
                Height = titlePanel.Height,
                Foreground = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Captions),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextAlignment = gaugeSettings.alignment.ToHorizontalAlignment(),
            };
            titleBlock.Text =
                (showAbbreviation) ? ConfigManager.ParamData.GetParam(pid).Abbreviation
                                                 : ConfigManager.ParamData.GetParam(pid).ParamName;
            titlePanel.Children.Add(titleBlock);
            titleBlock.ScaleText();
        }

        protected double GetTitleHeight()
        {
            if (hasGaugeBlock || hasValueText)
                return Height / 4d;
            return Height;
        }

        protected double GetTitleWidth()
        {
            if (!showSpot)
            {
                return Width;
            }
            else
            {
                return Width - GetAlertSpotWidth();
            }
        }

        protected void AddValueTextBlock()
        {
            var valueBlock = new PIDTextComponent(pid, gaugeSettings)
            {
                Width = Width,
                Height = GetValueTextHeight(),
                TextColor = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().GaugeText),
                TextAlignment = gaugeSettings.alignment.ToHorizontalAlignment(),
            };
            var top = GetValueTextTop();
            SetTop(valueBlock, GetValueTextTop());
            Children.Add(valueBlock);
            valueBlock.Draw();
        }

        protected double GetValueTextHeight()
        {
            if (hasGaugeBlock && hasTitleText)
                return Height - (GetTitleHeight() + GetGaugeHeight());
            else if (hasGaugeBlock)
                return Height - GetGaugeHeight();
            else if (hasTitleText)
                return Height - (GetTitleHeight());
            else
                return Height;
        }

        protected double GetValueTextTop()
        {
            if (hasGaugeBlock && hasTitleText)
                return Height * (1d / 4d);
            else if (hasGaugeBlock)
                return 0;
            else if (hasTitleText)
                return Height * (1d / 4d);
            else
                return 0;
        }

        protected void AddGraph()
        {
            var gaugeBlock = new BarComponent(pid)
            {
                Width = this.Width - (borderEdgePadding * 2),
                Height = GetGaugeHeight() - (borderEdgePadding * 2),
                Orientation = Orientation.Horizontal,
            };
            var border = new Border()
            {
                BorderThickness = new System.Windows.Thickness(borderEdgePadding, borderEdgePadding, borderEdgePadding, borderEdgePadding),
                Background = new SolidColorBrush(Colors.Black),
            };
            border.Child = gaugeBlock;
            Children.Add(border);
            SetLeft(border, 0);
            SetTop(border, Height - gaugeBlock.Height - (borderEdgePadding * 2));
            gaugeBlock.Draw();
        }

        protected double GetGaugeHeight()
        {
            if (hasTitleText && hasValueText)
                return Height / 4d;
            if (hasTitleText | hasValueText)
                return Height / 2d;
            else
                return Height;
        }
    }
}
