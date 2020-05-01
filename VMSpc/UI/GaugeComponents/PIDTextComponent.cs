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

namespace VMSpc.UI.GaugeComponents
{
    public class PIDTextComponent : GaugePIDComponent
    {
        private StackPanel panel;
        private TextBlock textBlock;
        private GaugeSettings gaugeSettings;
        public TextAlignment TextAlignment
        {
            get
            {
                return textBlock.TextAlignment;
            }
            set
            {
                textBlock.TextAlignment = value;
            }
        }
        public Brush TextColor
        {
            get
            {
                return textBlock.Foreground;
            }
            set
            {
                textBlock.Foreground = value;
            }
        }
        public PIDTextComponent(ushort pid, GaugeSettings gaugeSettings) : base(pid)
        {
            panel = new StackPanel();
            textBlock = new TextBlock();
            textBlock.Height = Height;
            Children.Add(panel);
            panel.Children.Add(textBlock);
            this.gaugeSettings = gaugeSettings;
        }

        public override void Draw()
        {
            panel.Width = this.Width;
            panel.Height = this.Height;
            textBlock.Width = Width;
            textBlock.Height = Height;
            textBlock.Text = "No Data";
            textBlock.ScaleText();
        }

        public override void Update()
        {
            if (currentValue != STALE_DATA && !double.IsNaN(currentValue))
            {
                string unitText = string.Empty;
                if (gaugeSettings.showUnit)
                {
                    unitText = " " + (gaugeSettings.showInMetric ? parameter.MetricUnit : parameter.Unit);
                }
                var lastLength = textBlock.Text.Length;
                textBlock.Text = string.Format(parameter.Format + unitText, currentValue);
                //Since the format of the gauge is defined, this should only occur when going from "No Data" to a valid value
                if (textBlock.Text.Length > lastLength)
                {
                    textBlock.ScaleText();
                }
            }
            else
            {
                textBlock.Text = "No Data";
            }
        }
    }
}
