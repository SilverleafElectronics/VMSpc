using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VMSpc.JsonFileManagers
{
    public class ScreenContents : IJsonContents
    {
        public Color
            TextColor;

        public string
            WallPaperFileName;

        public List<PanelSettings> PanelList;
    }
    public class ScreenReader : JsonFileReader<ScreenContents>
    {
        public ScreenReader(string filename) : base(filename)
        {
            ResolveBuggyIds();
        }

        /// <summary>
        /// For some reason, the panelId for SimpleGauge doesn't resolve properly. Add any other fixes here.
        /// 
        /// </summary>
        private void ResolveBuggyIds()
        {
            foreach (var panel in (Contents.PanelList))
            {
                if (panel.GetType() == typeof(SimpleGaugeSettings))
                {
                    panel.panelId = VEnum.UI.PanelType.SIMPLE_GAUGE;
                }
            }
        }

        protected override ScreenContents GetDefaultContents()
        {
            return new ScreenContents()
            {
                TextColor = new Color(),
                WallPaperFileName = null,
                PanelList = new List<PanelSettings>()
                {
                    new SimpleGaugeSettings()
                    {
                        number = 1,
                        panelCoordinates = new PanelCoordinates()
                        {
                            topLeftX = 320,
                            topLeftY = 530,
                            bottomRightX = 793,
                            bottomRightY = 738
                        },
                        alignment = VEnum.UI.UIPosition.CENTER,
                        backgroundColor = Colors.White,
                        pid = 12,
                        showAbbreviation = false,
                        showGraph = true,
                        showInMetric = false,
                        showName = true,
                        showSpot = true,
                        showUnit = true,
                        showValue = true,
                        panelId = VEnum.UI.PanelType.SIMPLE_GAUGE,
                        useGlobalColorPalette = true
                    },
                    //new RadialGaugeSettings(),
                    //new ScanGaugeSettings()
                }
            };
        }

        public void AddNewPanel(PanelSettings panelSettings)
        {
            panelSettings.number = (ulong)(Contents.PanelList.Count() + 1);
            Contents.PanelList.Add(panelSettings);
        }

        internal void DeletePanel(ulong number)
        {
            //remove the panel from the list
            foreach (var panel in Contents.PanelList)
            {
                if (panel.number == number)
                {
                    Contents.PanelList.Remove(panel);
                    break;
                }
            }
            //reset all remaining panel numbers to keep them sequential
            for (int i = 1; i <= Contents.PanelList.Count; i++)
            {
                Contents.PanelList[i - 1].number = (ulong)i;
            }
        }
    }
}
