using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.ComponentWrappers;
using System.Windows.Media;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.Panels
{
    public class MultiBar : VPanel
    {
        protected new MultiBarSettings panelSettings;
        protected StackPanel multiBarStack;
        protected List<MultiBarComponentWrapper> MultiBarWrappers;
        protected static Color[] GaugeColors = { Colors.Purple, Colors.Green, Colors.Red, Colors.Pink, Colors.Blue };

        public MultiBar(MainWindow mainWindow, MultiBarSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            MultiBarWrappers = new List<MultiBarComponentWrapper>();
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
                canvas.Background = new SolidColorBrush(panelSettings.BackgroundColor);
            }
            multiBarStack = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Orientation = Orientation.Horizontal,
            };
            int i = 0;
            foreach (var pid in panelSettings.pidList)
            {
                AddMultiBarComponent(pid, i);
                i = (i + 1) % GaugeColors.Length;
            }
            canvas.Children.Add(multiBarStack);
        }

        private void AddMultiBarComponent(ushort pid, int colorIndex)
        {
            var index = panelSettings.pidList.IndexOf(pid);
            var wrapper = new MultiBarComponentWrapper(pid, panelSettings)
            {
                Width = (canvas.Width / panelSettings.pidList.Count),
                Height = canvas.Height,
                GraphColor = GaugeColors[colorIndex],
            };
            multiBarStack.Children.Add(wrapper);
            wrapper.Draw();
        }

        public override void UpdatePanel()
        {
            //throw new NotImplementedException();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new MultiBarDlg(panelSettings);
        }
    }
}
