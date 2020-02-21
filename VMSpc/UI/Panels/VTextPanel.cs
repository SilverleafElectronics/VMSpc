using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.GaugeComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.Panels
{
    class VTextPanel : VPanel
    {
        TextBlock textBlock;
        public VTextPanel(MainWindow mainWindow, TextGaugeSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
        }
        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            textBlock = new TextBlock()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Text = ((TextGaugeSettings)panelSettings).text
            };
            canvas.Children.Add(textBlock);
            canvas.ScaleText(textBlock);
        }

        public override void UpdatePanel()
        {
        }

        protected override VMSDialog GenerateDlg()
        {
            return new TextPanelDlg((TextGaugeSettings)panelSettings);
        }
    }
}
