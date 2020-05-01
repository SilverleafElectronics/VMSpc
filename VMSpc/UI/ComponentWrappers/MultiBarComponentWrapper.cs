using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.Extensions.UI;
using VMSpc.JsonFileManagers;
using VMSpc.UI.GaugeComponents;

namespace VMSpc.UI.ComponentWrappers
{
    public class MultiBarComponentWrapper : ComponentCanvas
    {
        private GaugeSettings gaugeSettings;
        private JParameter parameter;
        private StackPanel ContentPanel;
        public Color GraphColor;
        public ushort pid;
        protected bool hasTitleText => gaugeSettings.showName;
        protected bool hasValueText => (gaugeSettings.showValue || gaugeSettings.showUnit);
        public MultiBarComponentWrapper(ushort pid, GaugeSettings gaugeSettings)
        {
            this.gaugeSettings = gaugeSettings;
            this.pid = pid;
            parameter = ConfigurationManager.ConfigManager.ParamData.GetParam(pid);
        }

        public void Draw()
        {
            ContentPanel = new StackPanel()
            {
                Height = this.Height,
                Width = this.Width,
            };
            AddGraph();
            if (gaugeSettings.showName)
            {
                AddGaugeTitle();
            }
            if (gaugeSettings.showUnit || gaugeSettings.showValue)
            {
                AddValueTextBlock();
            }
            Children.Add(ContentPanel);
        }

        private void AddGraph()
        {
            BarComponent barGraph = new BarComponent(pid)
            {
                Width = ContentPanel.Width,
                Height = GetGaugeHeight() - 2,
                Orientation = Orientation.Vertical,
                EmptyColor = new SolidColorBrush(gaugeSettings.backgroundColor),
                SolidValueColor = new SolidColorBrush(GraphColor)
            };
            ContentPanel.Children.Add(barGraph);
            barGraph.Draw();
            /*ContentPanel.Children.Add(
                new Line()
                {
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(GraphColor),
                    X1 = X2 = Y1 = Y2 =
                }
            );*/
        }

        private double GetGaugeHeight()
        {
            if (hasTitleText && hasValueText)
                return (ContentPanel.Height * (3d / 4d));
            else if (hasTitleText || hasValueText)
                return (ContentPanel.Height * (7d / 8d));
            else
                return ContentPanel.Height;
        }

        private void AddGaugeTitle()
        {
            TextBlock titleBlock = new TextBlock()
            {
                Width = ContentPanel.Width,
                Height = (ContentPanel.Height / 8d),
                Text = (gaugeSettings.showAbbreviation) ? parameter.Abbreviation : parameter.ParamName,
                TextAlignment = gaugeSettings.alignment.ToHorizontalAlignment(),
            };
            ContentPanel.Children.Add(titleBlock);
        }

        private void AddValueTextBlock()
        {
            PIDTextComponent valueTextBlock = new PIDTextComponent(pid, gaugeSettings)
            {
                Width = ContentPanel.Width,
                Height = (ContentPanel.Height / 8d),
                TextAlignment = gaugeSettings.alignment.ToHorizontalAlignment(),
            };
            ContentPanel.Children.Add(valueTextBlock);
            valueTextBlock.Draw();
        }
    }
}
