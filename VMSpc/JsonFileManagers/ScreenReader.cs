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
        public DateTime
            DayStartTime,
            NightStartTime;
        public bool
            UseDayNightTimer,
            OnDayPalette;
        public int
            dayColorPaletteId,
            nightColorPaletteId;
        public int selectedColorPaletteId => (OnDayPalette) ? dayColorPaletteId : nightColorPaletteId;

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
                    panel.panelId = Enums.UI.PanelType.SIMPLE_GAUGE;
                }
            }
        }

        public override void Reload(string filepath)
        {
            base.Reload(filepath);
            ResolveBuggyIds();
        }

        protected override ScreenContents GetDefaultContents()
        {
            return new ScreenContents()
            {
                TextColor = new Color(),
                WallPaperFileName = null,
                PanelList = new List<PanelSettings>()
                {
                },
                UseDayNightTimer = false,
                DayStartTime = DateTime.Parse("08:00:00AM"),
                NightStartTime = DateTime.Parse("06:00:00PM"),
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
                var panel = Contents.PanelList[i - 1];
                panel.number = (ulong)i;
                //drop the parent panel numbers to avoid shifting them when panels are deleted (see example)
                if (panel.parentPanelNumber > number)
                {
                    panel.parentPanelNumber--;
                }
            }
        }
    }
}

/*

    Example:
    panel_A.number = 1;
    panel_B.number = 2;
    panel_C.number = 3;
    panel_C.parentPanelNumber = 2
    delete panel_A, causing panel_B.number = 1 and panel_C.number = 2;
    panel_C.parentPanelNumber now equals panel_C.number, which is bad in a stack overflow exception causing kinda way
*/