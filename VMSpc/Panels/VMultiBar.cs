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
        protected new MultiBarSettings panelSettings;
        List<TextBlock> TitleTexts;
        List<TextBlock> ValueTexts;

        private Color[] ColorArray = { Color.FromRgb(255, 0, 0), Color.FromRgb(255, 255, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 255, 255), Color.FromRgb(0, 0, 255), };

        public VMultiBar(MainWindow mainWindow, MultiBarSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
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
            TitleTexts = new List<TextBlock>();
            ValueTexts = new List<TextBlock>();

            int i = 0;
            foreach (ushort pid in panelSettings.PIDList)
            {
                ParamPresenter presenter = new ParamPresenter(pid, panelSettings);
                BarColumn barColumn = new BarColumn(presenter, (canvas.Width / panelSettings.numPids), canvas.Height, ColorArray[i % ColorArray.Length]);
                Canvas.SetTop(barColumn, 0);
                Canvas.SetLeft(barColumn, (i * barColumn.Width));
                PidDict.Add(pid, barColumn);
                canvas.Children.Add(barColumn);
                TitleTexts.Add(barColumn.titleText);
                ValueTexts.Add(barColumn.valueText);
                i++;
            }
            canvas.BalanceTextBlocks(TitleTexts);
            canvas.BalanceTextBlocks(ValueTexts);
        }

        protected override VMSDialog GenerateDlg()
        {
            return new MultiBarDlg(panelSettings);
        }

        public override void UpdatePanel()
        {
            bool TextNeedsUpdate = false;
            foreach (var barColumn in PidDict.Values)
            {
                barColumn.Update();
                TextNeedsUpdate = barColumn.ShouldUpdateGrid();
            }
            if (TextNeedsUpdate)
                canvas.BalanceTextBlocks(ValueTexts);
        }
    }

    public class BarColumn : VMSCanvas
    {
        public TextBlock titleText;
        public TextBlock valueText;
        private Rectangle valueRect;
        private Rectangle noValueRect;
        private ParamPresenter presenter;

        private double graphBottom;
        private double graphTop;
        private int numTextLines;
        private bool updated;

        public BarColumn(ParamPresenter presenter, double width, double height, Color color)
        {
            this.presenter = presenter;
            Height = height;
            Width = width;
            titleText = new TextBlock();
            valueText = new TextBlock();
            valueRect = new Rectangle()
            {
                Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(Colors.Black),
                Width = Width
            };
            noValueRect = new Rectangle()
            {
                Fill = new SolidColorBrush(color),
                Width = Width,
                Height = 1
            };
            numTextLines = (Convert.ToInt32(presenter.showName || presenter.showAbbreviation) + Convert.ToInt32(presenter.showValue || presenter.showUnit));
            graphBottom = height * ((double)(8 - numTextLines) / 8);
            Draw();
        }

        private void Draw()
        {
            if (presenter.showGraph)
            {
                Children.Add(valueRect);
                Children.Add(noValueRect);
                SetTop(noValueRect, graphBottom - noValueRect.Height);
            }
            if (presenter.showName || presenter.showAbbreviation)
            {
                titleText.Text = presenter.Title;
                titleText.Width = Width;
                titleText.Height = Height / 8;
                titleText.TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition);
                ScaleText(titleText, Width - 10, (Height / 8));
                Children.Add(titleText);
                SetTop(titleText, graphBottom);
            }
            if (presenter.showValue || presenter.showUnit)
            {
                valueText.Text = "NO DATA";
                valueText.Width = Width;
                valueText.Height = Height / 8;
                valueText.TextAlignment = GET_TEXT_ALIGNMENT(presenter.textPosition);
                ScaleText(valueText, Width - 10, (Height / 8));
                Children.Add(valueText);
                SetTop(valueText, (!(presenter.showName || presenter.showAbbreviation)) ? graphBottom : (graphBottom + titleText.Height));
            }
        }

        public void Update()
        {
            if (presenter.IsValidForUpdate())
            {
                UpdateValueText();
                UpdateBar();
            }
        }

        /// <summary> Indicates whether or not the ValueText has been updated. Can be used to check if BalanceTextBlocks should be called </summary>
        public bool ShouldUpdateGrid()
        {
            bool retval = updated;
            updated = false;
            return retval;
        }

        private void UpdateBar()
        {
            graphTop = graphBottom - (graphBottom * presenter.ValueAsPercent);
            SetTop(valueRect, graphTop);
            valueRect.Height = graphBottom - graphTop;
        }

        private void UpdateValueText()
        {
            valueText.Text = presenter.ValueAsString;
            //Checks if the updated text has caused the rendered string to be wider than the column supports
            if (CalculateStringSize(valueText).Width > (Width - 10))
            {
                ScaleText(valueText, Width - 10, (Height / 8));
                updated = true;
            }
        }
    }
}
