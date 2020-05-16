using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.Extensions.UI;

namespace VMSpc.UI.Panels
{
    class RadialGauge : VPanel
    {
        protected new SimpleGaugeSettings panelSettings;
        protected RadialComponent radialComponent;
        protected PIDTextComponent valueBlock;
        protected TextBlock titleText;
        protected JParameter parameter;
        public RadialGauge(MainWindow mainWindow, SimpleGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            parameter = ConfigManager.ParamData.GetParam(panelSettings.pid);
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            if (panelSettings.useGlobalColorPalette)
            {
                canvas.Background = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().GaugeBackground);
            }
            else
            {
                canvas.Background = new SolidColorBrush(panelSettings.backgroundColor);
            }
            AddGaugeComponent();
            if (panelSettings.showName)
            {
                AddTitle();
            }
            if (panelSettings.showUnit | panelSettings.showValue)
            {
                AddValueComponent();
            }
        }

        public override void UpdatePanel()
        {
        }
        
        protected override VMSDialog GenerateDlg()
        {
            return new RadialGaugeDlg(panelSettings);
        }

        protected void AddGaugeComponent()
        {
            radialComponent = new RadialComponent(panelSettings.pid, parameter.GaugeMin, parameter.GaugeMax)
            {
                Width = Math.Min(canvas.Width, GetGaugeHeight()),
                Height = Math.Min(canvas.Width, GetGaugeHeight()),
            };
            canvas.Children.Add(radialComponent);
            radialComponent.Draw();
            Canvas.SetLeft(radialComponent, (canvas.Width - radialComponent.Width) / 2);
            Canvas.SetTop(radialComponent, (canvas.Height - radialComponent.Height) / 2);
        }

        protected void AddValueComponent()
        {
            valueBlock = new PIDTextComponent(panelSettings.pid, panelSettings)
            {
                Width = radialComponent.Width / 2,
                Height = radialComponent.Height / 4,
                TextColor = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().GaugeText),
                TextAlignment = panelSettings.alignment.ToHorizontalAlignment(),
            };
            canvas.Children.Add(valueBlock);
            valueBlock.Draw();
            Canvas.SetLeft(valueBlock, GetValueTextLeft());
            Canvas.SetTop(valueBlock, GetValueTextTop());
        }

        protected void AddTitle()
        {
            titleText = new TextBlock()
            {
                Width = canvas.Width,
                Height = (canvas.Height / 5),
                Foreground = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().Captions),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextAlignment = panelSettings.alignment.ToHorizontalAlignment(),
                Text = (panelSettings.showAbbreviation)
                    ? ConfigManager.ParamData.GetParam(panelSettings.pid).Abbreviation
                    : ConfigManager.ParamData.GetParam(panelSettings.pid).ParamName,
            };
            canvas.Children.Add(titleText);
            Canvas.SetTop(titleText, canvas.Height - titleText.Height);
            titleText.ScaleText();
        }

        protected double GetGaugeHeight()
        {
            //if (false)//(panelSettings.showName)
            //{
            //    return canvas.Height - (canvas.Height / 5);
            //}
            //else
            //{
                return canvas.Height;
            //}
        }

        protected double GetValueTextTop()
        {
            return Canvas.GetTop(radialComponent) + radialComponent.Height / 2.5;// + (radialComponent.Height / 2);
        }

        protected double GetValueTextLeft()
        {
            return Canvas.GetLeft(radialComponent) + radialComponent.Width / 4;// + (radialComponent.Width / 2);
        }

        protected void GetValueTextHeight()
        {

        }

        protected void GetValueTextWidth()
        {

        }
    }
}
