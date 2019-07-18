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
    public class VMultiBar : VPanel
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
            int i = 0;
            foreach (ushort pid in ((MultiBarSettings)panelSettings).PIDList)
            {
                PidDict.Add(pid, new BarColumn(
                                                canvas,
                                                pid,
                                                ((MultiBarSettings)panelSettings).numPids,
                                                i,
                                                panelSettings.showInMetric,
                                                ((MultiBarSettings)panelSettings).showGraph,
                                                ((MultiBarSettings)panelSettings).showName,
                                                ((MultiBarSettings)panelSettings).showValue,
                                                ((MultiBarSettings)panelSettings).showUnit
                                              ));
                i++;
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
        private double graphTop;
        private double numTextLines;
        private double width;
        private int barNumber;

        public BarColumn(Canvas canvas, ushort pid, int numPids, int barNumber, bool useMetric, bool showGraph, bool showName, bool showValue, bool showUnit)
        {
            this.showGraph = showGraph;
            this.showName = showName;
            this.showValue = showValue;
            this.showUnit = showUnit;
            this.canvas = canvas;
            this.barNumber = barNumber;
            width = (canvas.Width / numPids);

            titleText = new TextBlock();
            valueText = new TextBlock();
            valueRect = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.Blue),
                Stroke = new SolidColorBrush(Colors.Black),
                Width = width
            };
            presenter = new ParamPresenter(pid, useMetric, showValue, showUnit);
            numTextLines = (Convert.ToInt32(showValue) + Convert.ToInt32(showName || showUnit)) + 1;
            graphBottom = canvas.Height / numTextLines;
            //graphBottom = (2 + numTextLines) * (canvas.Height / 4);
            Draw();
        }

        private void Draw()
        {
            if (showGraph)
            {
                canvas.Children.Add(valueRect);
                Canvas.SetLeft(valueRect, (barNumber * width));
                //Canvas.SetBottom(valueRect, graphBottom);
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
            if (presenter.IsValidForUpdate())
            {
                UpdateBar();
                UpdateValueText();
            }
        }

        private void UpdateBar()
        {
            graphTop = graphBottom - (graphBottom * presenter.ValueAsPercent());
            Canvas.SetTop(valueRect, graphTop);
            valueRect.Height = graphBottom - graphTop;
        }

        private void UpdateValueText()
        {

        }
    }
}
