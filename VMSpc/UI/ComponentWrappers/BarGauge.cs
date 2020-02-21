using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.Extensions.UI;
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

        protected bool hasGaugeBlock => showGraph;
        protected bool hasTitleText => showName;
        protected bool hasValueText => (showValue || showUnit);

        public BarGauge() 
            : base()
        {

        }

        public void Draw()
        {
            if (showGraph)
            {
                AddGraph();
            }
            if (showName)
            {
                AddGaugeTitle();
            }
            if (showSpot)
            {
                AddAlertSpot();
            }
            if (showUnit || showValue)
            {
                AddValueTextBlock();
            }
        }

        protected void AddValueTextBlock()
        {
            var valueBlock = new PIDTextComponent(pid)
            {
                Width =  Width,
                Height = GetValueTextHeight()
            };
            var top = GetValueTextTop();
            SetTop(valueBlock, GetValueTextTop());
            Children.Add(valueBlock);
            valueBlock.Draw();
        }

        protected double GetValueTextHeight()
        {
            if (hasGaugeBlock && hasValueText)
                return Height / 4d;
            else if (hasGaugeBlock)
                return Height / 2d;
            else if (hasValueText)
                return Height * (3d / 4d);
            else
                return Height;
        }

        protected double GetValueTextTop()
        {
            if (hasGaugeBlock && hasTitleText)
                return Height * (1d / 4d);
            else if (hasGaugeBlock)
                return Height;
            else if (hasTitleText)
                return Height * (1d / 4d);
            else
                return Height;
        }

        protected void AddAlertSpot()
        {
            //TODO
        }

        protected void AddGaugeTitle()
        {
            var titleBlock = new TextBlock()
            {
                Width = this.Width,
                Height = GetTitleHeight()
            };
            titleBlock.Text =
                (showAbbreviation) ? ConfigManager.ParamData.GetParam(pid).Abbreviation
                                                 : ConfigManager.ParamData.GetParam(pid).ParamName;
            Children.Add(titleBlock);
            SetLeft(titleBlock, 0);
            SetTop(titleBlock, 0);
            titleBlock.ScaleText();
        }

        protected double GetTitleHeight()
        {
            if (hasGaugeBlock || hasValueText)
                return Height / 4d;
            return Height;
        }

        protected void AddGraph()
        {
            var gaugeBlock = new BarComponent(pid, ConfigManager.ParamData.GetParam(pid))
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
