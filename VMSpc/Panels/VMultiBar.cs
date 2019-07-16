using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.DevHelpers;
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.ParamDataManager;

namespace VMSpc.Panels
{
    class VMultiBar : VPanel
    {
        protected TextBlock titleText;
        protected TextBlock valueText;
        protected List<Rectangle> valueRects;

        private double graphBottom;
        private double numTextLines;

        public VMultiBar(MainWindow mainWindow, MultiBarSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
        }

        private void RefreshChildren()
        {
            canvas.Children.RemoveRange(0, canvas.Children.Count);
            titleText = new TextBlock();
            valueText = new TextBlock();
            foreach (var pid in ((MultiBarSettings)panelSettings).PIDList)
            {
                Rectangle valueRect = new Rectangle();
                valueRect.Width = canvas.Width / ((MultiBarSettings)panelSettings).numPids;
                valueRects.Add(valueRect);
                canvas.Children.Add(valueRect);
            }
        }

        protected override VMSDialog GenerateDlg()
        {
            return new MultiBarDlg(panelSettings);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void GeneratePanel()
        {
            RefreshChildren();
            numTextLines = Convert.ToInt32(((MultiBarSettings)panelSettings).showValue) + Convert.ToInt32(((MultiBarSettings)panelSettings).showName);

        }

        public override void UpdatePanel()
        {
            throw new NotImplementedException();
        }

    }
}
