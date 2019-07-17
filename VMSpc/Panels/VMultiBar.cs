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
using VMSpc.CustomComponents;
using VMSpc.DevHelpers;
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.ParamDataManager;

namespace VMSpc.Panels
{
    class VMultiBar : VPanel
    {
        protected Dictionary<ushort, BarColumn> PidDict;

        public VMultiBar(MainWindow mainWindow, MultiBarSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            PidDict = new Dictionary<ushort, BarColumn>();
        }

        public override void Init()
        {
            base.Init();
        }

        public override void GeneratePanel()
        {
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            canvas.Children.Clear();
            PidDict.Clear();
            foreach (ushort pid in ((MultiBarSettings)panelSettings).PIDList)
            {
                PidDict.Add(pid, new BarColumn(
                                                canvas,
                                                pid,
                                                ((MultiBarSettings)panelSettings).numPids,
                                                panelSettings.showInMetric,
                                                ((MultiBarSettings)panelSettings).showGraph,
                                                ((MultiBarSettings)panelSettings).showName,
                                                ((MultiBarSettings)panelSettings).showValue,
                                                ((MultiBarSettings)panelSettings).showUnit
                                              ));
            }
        }

        protected override VMSDialog GenerateDlg()
        {
            return new MultiBarDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
            foreach (var barColumn in PidDict.Values)
            {
                barColumn.Update();
            }
        }
    }

    public class BarColumn
    {
        private bool showGraph,
                     showName,
                     showValue,
                     showUnit;

        private TextBlock titleText;
        private TextBlock valueText;
        private Rectangle valueRect;
        private ParamPresenter presenter;
        private Canvas canvas;

        private double graphBottom;
        private double numTextLines;
        private double width;

        public BarColumn(Canvas canvas, ushort pid, int numPids, bool useMetric, bool showGraph, bool showName, bool showValue, bool showUnit)
        {
            this.showGraph = showGraph;
            this.showName = showName;
            this.showValue = showValue;
            this.showUnit = showUnit;
            width = (canvas.Width / numPids);

            titleText = new TextBlock();
            valueText = new TextBlock();
            valueRect = new Rectangle();
            presenter = new ParamPresenter(pid, useMetric);
            numTextLines = (Convert.ToInt32(showValue) + Convert.ToInt32(showName || showUnit)) + 1;
            graphBottom = canvas.Height / numTextLines;
            graphBottom = (2 + numTextLines) * (canvas.Height / 4);
            Draw();
        }

        private void Draw()
        {
            if (showGraph)
            {
                canvas.Children.Add(valueRect);
                Canvas.SetBottom(valueRect, graphBottom);
            }
            if (showName)
            {
                canvas.Children.Add(titleText);
            }
            if (showValue || showUnit)
            {
                canvas.Children.Add(valueText);
            }
        }

        public void Update()
        {

        }
    }
}
