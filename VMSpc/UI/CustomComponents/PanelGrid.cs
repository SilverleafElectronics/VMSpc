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
//using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.VEnum.UI;
using VMSpc.UI.Panels;

namespace VMSpc.CustomComponents
{
    /// <summary>
    /// Acts as manager of all child panels in the UI
    /// </summary>
    public class PanelGrid : Canvas
    {
        private MainWindow mainWindow;
        private ScreenReader Screen;
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
            Screen = ConfigManager.Screen;
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
            foreach(var panelSettings in Screen.Contents.PanelList)
            {
                AddPanel(panelSettings);
            }
        }

        public void CreateNewPanel(PanelSettings panelSettings)
        {
            Screen.AddNewPanel(panelSettings);
            AddPanel(panelSettings);
        }

        public void DeletePanel()
        {
            if (NOT_NULL(highlightedChild))
            {
                highlightedChild.UnHighlight();
                Children.Clear();
                PanelList.Clear();
                Screen.DeletePanel(highlightedChild.number);
                InitPanels(mainWindow);
                highlightedChild = null;
            }
        }

        private void AddPanel(PanelSettings panelSettings)
        {
            VPanel panel = null;
            switch (panelSettings.panelId)
            {
                case PanelType.SIMPLE_GAUGE:
                    panel = new VSimpleGauge(mainWindow, (SimpleGaugeSettings)panelSettings);
                    break;
                case PanelType.SCAN_GAUGE:
                    panel = new ScanGauge(mainWindow, (ScanGaugeSettings)panelSettings);
                    break;
                case PanelType.ODOMETER:
                    //panel = new VOdometer(mainWindow, (OdometerSettings)panelSettings);
                    break;
                case PanelType.TRANSMISSION_GAUGE:
                    break;
                case PanelType.MULTIBAR:
                    //panel = new VMultiBar(mainWindow, (MultiBarSettings)panelSettings);
                    break;
                case PanelType.HISTOGRAM:
                    break;
                case PanelType.CLOCK:
                    break;
                case PanelType.IMAGE:
                    panel = new VImagePanel(mainWindow, (PictureSettings)panelSettings);
                    break;
                case PanelType.TEXT:
                    panel = new VTextPanel(mainWindow, (TextGaugeSettings)panelSettings);
                    break;
                case PanelType.TANK_MINDER:
                    break;
                case PanelType.TIRE_GAUGE:
                    panel = new VTirePanel(mainWindow, (TireGaugeSettings)panelSettings);
                    break;
                case PanelType.MESSAGE:
                    break;
                case PanelType.DIAGNOSTIC_ALARM:
                    panel = new VDiagAlarmPanel(mainWindow, (DiagnosticGaugeSettings)panelSettings);
                    break;
                case PanelType.RADIAL_GAUGE:
                    panel = new VRadialGauge(mainWindow, (RadialGaugeSettings)panelSettings);
                    break;
                default:
                    break;
            }
            if (panel != null)  //Append panel to the grid for display
            {
                PanelList.Add(panel);
                Children.Add(panel.border);
                SetZIndex(panel.canvas, (int)(panelSettings.number + 10));
                SetZIndex(panel.border, (int)(panelSettings.number + 10));
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
                selectedChild.ProcessMouseRelease();
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
            try
            {
                if (src == null)
                    return false;
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
                            SetZIndices();
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
                            SetZIndices();
                            return true;
                        }
                    }
                }
                return (selectedChild != null);
            }
            catch
            {
                return false;
            }
        }

        private void SetZIndices()
        {
            int currentZIndex = GetZIndex(selectedChild.canvas);
            int curMax = 0;
            foreach (VPanel panel in PanelList)
            {
                int oldZIndex = GetZIndex(panel.canvas);
                if (oldZIndex > curMax) curMax = oldZIndex;
                if (oldZIndex > currentZIndex)
                {
                    SetZIndex(panel.canvas, oldZIndex - 1);
                    SetZIndex(panel.border, oldZIndex - 1);
                }
            }
            SetZIndex(selectedChild.canvas, curMax);
            SetZIndex(selectedChild.border, curMax);
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

        public void ProcessArrowNavigation(Key direction)
        {
            if (NOT_NULL(highlightedChild))
                highlightedChild.Slide(direction);
        }

        private void MovePanel(VPanel panel, double newTop, double newLeft, Point newCursorPoint)
        {
            switch (movementType)
            {
                case MOVEMENT_MOVE:
                    panel.SetHorizontal(newLeft);
                    panel.SetVertical(newTop);
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
            if (!ConfigManager.Settings.Contents.useClipping)
                return;
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
            Screen.SaveConfiguration();

        }
    }
}
