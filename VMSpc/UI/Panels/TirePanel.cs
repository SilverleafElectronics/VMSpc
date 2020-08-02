using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;
using VMSpc.Panels;
using VMSpc.UI.GaugeComponents;
using VMSpc.UI.TireMaps;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.Panels
{
    public class TirePanel : VPanel
    {
        private int MaxTireRow, MaxTireColumn;
        private double CellHeight, CellWidth;
        private Grid TireGrid;
        protected new TireGaugeSettings panelSettings;
        List<TireCell> tireCells;
        public TirePanel(MainWindow mainWindow, TireGaugeSettings panelSettings)
            :base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
        }
        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            tireCells = TireMap.InitializeTireCellMap(ConfigManager.Settings.Contents.tireMapType, panelSettings.detachTowed);
            SetGridLimits();
            LayoutGrid();
            AddGridCells();
        }

        private void SetGridLimits()
        {
            MaxTireRow = MaxTireColumn = 0;
            foreach(var tireCell in tireCells)
            {
                if (tireCell.row > MaxTireRow)
                    MaxTireRow = tireCell.row;
                if (tireCell.column > MaxTireColumn)
                    MaxTireColumn = tireCell.column;
            }
            CellHeight = canvas.Height / (double)(MaxTireRow + 1d);
            CellWidth = canvas.Width / (double)(MaxTireColumn + 1d);
        }

        /// <summary>
        /// Allocates space for each cell in the grid.
        /// </summary>
        private void LayoutGrid()
        {
            TireGrid = new Grid()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Background = new SolidColorBrush(panelSettings.BackgroundColor),
            };
            for (int i = 0; i <= MaxTireRow; i++)
            {
                TireGrid.RowDefinitions.Add(
                    new RowDefinition()
                    {
                        Height = new GridLength(CellHeight)
                    });
            }
            for (int i = 0; i <= MaxTireColumn; i++)
            {
                TireGrid.ColumnDefinitions.Add(
                    new ColumnDefinition()
                    {
                        Width = new GridLength(CellWidth)
                    });
            }
            canvas.Children.Add(TireGrid);
        }

        private void AddGridCells()
        {
            foreach (var cell in tireCells)
            {
                var TireIcon = new TireIconComponent(cell.index)
                {
                    Width = CellWidth,
                    Height = CellHeight,
                    ShowIndicator = panelSettings.showIcon,
                    ShowPressure = panelSettings.showPressure,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                TireGrid.Children.Add(TireIcon);
                Grid.SetColumn(TireIcon, cell.column);
                Grid.SetRow(TireIcon, cell.row);
                TireIcon.Draw();
            }
        }

        public override void UpdatePanel()
        {
            //throw new NotImplementedException();
        }

        protected override VMSDialog GenerateDlg()
        {
            return new TirePanelDlg(panelSettings);
        }
    }
}
