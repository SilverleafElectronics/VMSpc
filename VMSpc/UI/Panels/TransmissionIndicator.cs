using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.Extensions.UI;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Parsers;
using System.Windows.Media;

namespace VMSpc.UI.Panels
{
    public class TransmissionIndicator : VPanel
    {
        protected new TransmissionGaugeSettings panelSettings;
        private StackPanel
            GearDisplay;
        private TextBlock
            GearSelectedBlock,
            GearAttainedBlock;

        public TransmissionIndicator(MainWindow mainWindow, TransmissionGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }
        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            canvas.Background = new SolidColorBrush(panelSettings.backgroundColor);
            GearDisplay = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Orientation = Orientation.Horizontal,
            };
            if (panelSettings.showSelected)
                AddGearSelected();
            if (panelSettings.showAttained)
                AddGearAttained();
            canvas.Children.Add(GearDisplay);
        }

        private void AddGearSelected()
        {
            GearSelectedBlock = new TextBlock()
            {
                Height = canvas.Height,
                Width = (panelSettings.showAttained) ? (canvas.Width / 2) : canvas.Width,
                Text = "?",
                TextAlignment = panelSettings.alignment.ToHorizontalAlignment(),
                Foreground = new SolidColorBrush(panelSettings.valueTextColor),
            };
            GearDisplay.Children.Add(GearSelectedBlock);
            GearSelectedBlock.ScaleText(12, 2);
        }

        private void AddGearAttained()
        {
            GearAttainedBlock = new TextBlock()
            {
                Height = canvas.Height,
                Width = (panelSettings.showSelected) ? (canvas.Width / 2) : canvas.Width,
                Text = "?",
                TextAlignment = panelSettings.alignment.ToHorizontalAlignment(),
                Foreground = new SolidColorBrush(panelSettings.valueTextColor),
            };
            GearDisplay.Children.Add(GearAttainedBlock);
            GearAttainedBlock.ScaleText(12, 2);
        }

        public override void UpdatePanel()
        {
            if ((GearSelectedBlock != null) && ChassisParameter.ChassisParam.rangeSelected != null)
                GearSelectedBlock.Text = ChassisParameter.ChassisParam.rangeSelected;
            if (GearAttainedBlock != null && ChassisParameter.ChassisParam.rangeAttained != null)
                GearAttainedBlock.Text = ChassisParameter.ChassisParam.rangeAttained;
        }

        protected override VMSDialog GenerateDlg()
        {
            return new TransmissionGaugeDlg(panelSettings);
        }
    }
}
