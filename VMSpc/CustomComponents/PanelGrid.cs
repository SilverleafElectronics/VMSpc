using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.DefaultScrnManager;

namespace VMSpc.CustomComponents
{
    /// <summary>
    /// Acts as manager of all child panels in the UI
    /// </summary>
    public class PanelGrid : Canvas
    {
        private MainWindow mainWindow;
        public List<VPanel> PanelList = new List<VPanel>();

        private bool isDragging;
        private Point cursorStartPoint;
        private Double lastCursorX;
        private Double lastCursorY;
        private Double canvasStartTop;
        private Double canvasStartLeft;
        private VPanel selectedChild;
        private VPanel highlightedChild;

        private int movementType;

        public PanelGrid()
            : base ()
        {
            selectedChild = null;
            Init();
        }

        private void Init()
        {
            isDragging = false;
            canvasStartTop = Double.NaN;
            canvasStartLeft = Double.NaN;
            movementType = MOVEMENT_NONE;
        }

        public void InitPanels(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            scrnManager.LoadPanels();
            foreach(var panelSettings in scrnManager.configurationPanelList)
            {
                AddPanel(panelSettings);
            }
        }

        public void CreateNewPanel(PanelSettings panelSettings)
        {
            scrnManager.AddNewPanel(panelSettings);
            AddPanel(panelSettings);
        }

        public void DeletePanel()
        {
            if (NOT_NULL(highlightedChild))
            {
                highlightedChild.UnHighlight();
                Children.Clear();
                PanelList.Clear();
                scrnManager.DeletePanel(highlightedChild.number);
                InitPanels(mainWindow);
                highlightedChild = null;

            }
        }

        private void AddPanel(PanelSettings panelSettings)
        {
            VPanel panel = null;
            switch (panelSettings.ID)
            {
                case PanelIDs.SIMPLE_GAUGE_ID:
                    panel = new VSimpleGauge(mainWindow, (SimpleGaugeSettings)panelSettings);
                    break;
                case PanelIDs.SCAN_GAUGE_ID:
                    panel = new VScanGauge(mainWindow, (ScanGaugeSettings)panelSettings);
                    break;
                case PanelIDs.ODOMOTER_ID:
                    panel = new VOdometerPanel(mainWindow, (OdometerSettings)panelSettings);
                    break;
                case PanelIDs.TRANSMISSION_ID:
                    break;
                case PanelIDs.MULTIBAR_ID:
                    panel = new VMultiBar(mainWindow, (MultiBarSettings)panelSettings);
                    break;
                case PanelIDs.HISTOGRAM_ID:
                    break;
                case PanelIDs.CLOCK_PANEL_ID:
                    break;
                case PanelIDs.IMG_PANEL_ID:
                    break;
                case PanelIDs.TEXT_PANEL_ID:
                    break;
                case PanelIDs.TANK_MINDER_ID:
                    break;
                case PanelIDs.TIRE_PANEL_ID:
                    break;
                case PanelIDs.MESSAGE_PANEL_ID:
                    break;
                case PanelIDs.DIAG_ALARM_ID:
                    break;
                case PanelIDs.RADIAL_GAUGE_ID:
                    panel = new VRoundGauge(mainWindow, (RoundGaugeSettings)panelSettings);
                    break;
                default:
                    break;
            }
            if (panel != null)  //Append panel to the grid for display
            {
                PanelList.Add(panel);
                Children.Add(panel.border);
                panel.Init();
                panel.GeneratePanel();
            }
        }

        #region overrides

        public void ProcessMouseRelease()
        {
            Init();
            SavePanels();
            if (selectedChild != null)
            {
                selectedChild.isMoving = false;
                selectedChild.isResizing = false;
                selectedChild = null;
            }
            movementType = MOVEMENT_NONE;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            isDragging = false;
            if (SetSelectedChild(e.Source))
            {
                cursorStartPoint = e.GetPosition(this);
                canvasStartTop = GetTop(selectedChild.border);
                if (Double.IsNaN(canvasStartTop)) canvasStartTop = 0;
                canvasStartLeft = GetLeft(selectedChild.border);
                if (Double.IsNaN(canvasStartLeft)) canvasStartLeft = 0;
                lastCursorY = canvasStartTop;
                lastCursorX = canvasStartLeft;
                isDragging = true;
            }
        }

        /// <summary>
        /// Sets selectedChild to the source of the click. If the user has clicked outside of the panel, selectedChild is set to null
        /// </summary>
        /// <param name="src"></param>
        private bool SetSelectedChild(dynamic src)
        {
            if (NOT_NULL(highlightedChild))
            {
                highlightedChild.UnHighlight();
                highlightedChild = null;
            }
            selectedChild = null;
            foreach (VPanel panel in PanelList)
            {
                dynamic source = src;
                if (source.GetType() == panel.border.GetType())
                {
                    if (panel.border == source)
                    {
                        selectedChild = highlightedChild = panel;
                        movementType = MOVEMENT_RESIZE;
                        selectedChild.isResizing = true;
                        selectedChild.Highlight();
                        return true;
                    }
                }
                else
                {
                    while (source.GetType() != panel.canvas.GetType())
                    {
                        if (source.GetType() == mainWindow.GetType())
                        {
                            selectedChild = null;
                            return false;
                        }
                        source = VisualTreeHelper.GetParent(source);
                    }
                    if (panel.canvas == source)
                    {
                        selectedChild = highlightedChild = panel;
                        selectedChild.isMoving = true;
                        movementType = MOVEMENT_MOVE;
                        selectedChild.Highlight();
                        return true;
                    }
                }
            }
            return (selectedChild != null);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ProcessMouseMove(this, e);
        }

        public void ProcessMouseMove(System.Windows.IInputElement src, MouseEventArgs e)
        {
            if (!isDragging)
                return;
            SetNearestNeighbors(selectedChild);
            double newTop, newLeft;
            Point newCursorPoint = e.GetPosition(src);
            double cursorXDiff = newCursorPoint.X - cursorStartPoint.X;
            double cursorYDiff = newCursorPoint.Y - cursorStartPoint.Y;
            newTop = canvasStartTop + cursorYDiff;
            newLeft = canvasStartLeft + cursorXDiff;
            MovePanel(selectedChild, newTop, newLeft, newCursorPoint);
        }

        private void MovePanel(VPanel panel, double newTop, double newLeft, Point newCursorPoint)
        {
            switch (movementType)
            {
                case MOVEMENT_MOVE:
                    panel.SetHorizontal(newLeft, newCursorPoint);
                    panel.SetVertical(newTop, newCursorPoint);
                    break;
                case MOVEMENT_RESIZE:
                    panel.Resize(newCursorPoint);
                    break;
                default:
                    break;
            }
        }

        private void SetNearestNeighbors(VPanel panel)
        {
            panel.InitLimits();
            foreach (VPanel neighbor in PanelList)
            {
                if (neighbor != panel)
                    panel.SetDirectionalLimits(neighbor);
            }
        }

        #endregion

        protected void SavePanels()
        {
            foreach (var panel in PanelList)
                panel.SaveSettings();
            scrnManager.SaveConfiguration();

        }
    }
}
