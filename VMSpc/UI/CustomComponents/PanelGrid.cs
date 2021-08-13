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
using VMSpc.UI.DlgWindows;
using VMSpc.Panels;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Enums.UI;
using VMSpc.UI.Panels;
using VMSpc.UI.Managers.Alarms;
using VMSpc.Common;
using VMSpc.Exceptions;
using VMSpc.Loggers;

namespace VMSpc.UI.CustomComponents
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

        private VPanel SelectedChild;
        private VPanel selectedChild
        {
            get { return SelectedChild; }
            set { SelectedChild = value; }
        }

        private VPanel HighlightedChild;
        private VPanel highlightedChild
        {
            get { return HighlightedChild; }
            set
            {
                HighlightedChild?.UnHighlight();
                HighlightedChild = value;
                HighlightedChild?.Highlight();
            }
        }

        public bool CanAttachGauge => highlightedChild != null;
        private VPanel PanelToAttach;
        private bool inAttachMode;
        public bool InAttachMode 
        { 
            get { return inAttachMode; }
            set
            {
                inAttachMode = value;
                PanelToAttach = (value) ? highlightedChild : null;
            }
        }

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
            InAttachMode = false;
            canvasStartTop = Double.NaN;
            canvasStartLeft = Double.NaN;
            movementType = MOVEMENT_NONE;
        }

        public void InitPanels(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            Children.Clear();
            PanelList.Clear();
            Background = new SolidColorBrush(ConfigManager.ColorPalettes.GetSelectedPalette().MainBackground);
            EventBridge.Instance.RemoveGUIRegistryItems();
            AlarmManager.LoadAlarms();
            foreach (var panelSettings in Screen.Contents.PanelList)
            {
                AddPanel(panelSettings);
            }
            SetPanelParents();
        }

        public void CreateNewPanel(PanelSettings panelSettings)
        {
            Screen.AddNewPanel(panelSettings);
            AddPanel(panelSettings);
        }

        public void DeletePanel()
        {
            if (highlightedChild != null)
            Screen.DeletePanel(highlightedChild.number);
            {
                highlightedChild = null;
                InitPanels(mainWindow);
            }
        }

        private void AddPanel(PanelSettings panelSettings)
        {
            VPanel panel = null;
            switch (panelSettings.panelId)
            {
                case PanelType.SIMPLE_GAUGE:
                    panel = new SimpleGauge(mainWindow, (SimpleGaugeSettings)panelSettings);
                    break;
                case PanelType.SCAN_GAUGE:
                    panel = new ScanGauge(mainWindow, (ScanGaugeSettings)panelSettings);
                    break;
                case PanelType.ODOMETER:
                    panel = new OdometerPanel(mainWindow, (OdometerSettings)panelSettings);
                    break;
                case PanelType.TRANSMISSION_GAUGE:
                    panel = new TransmissionIndicator(mainWindow, (TransmissionGaugeSettings)panelSettings);
                    break;
                case PanelType.MULTIBAR:
                    panel = new MultiBar(mainWindow, (MultiBarSettings)panelSettings);
                    break;
                case PanelType.HISTOGRAM:
                    break;
                case PanelType.CLOCK:
                    panel = new Clock(mainWindow, (ClockSettings)panelSettings);
                    break;
                case PanelType.IMAGE:
                    panel = new ImagePanel(mainWindow, (PictureSettings)panelSettings);
                    break;
                case PanelType.TEXT:
                    panel = new TextPanel(mainWindow, (TextGaugeSettings)panelSettings);
                    break;
                case PanelType.TANK_MINDER:
                    panel = new TankMinderPanel(mainWindow, (TankMinderSettings)panelSettings);
                    break;
                case PanelType.TIRE_GAUGE:
                    panel = new TirePanel(mainWindow, (TireGaugeSettings)panelSettings);
                    break;
                case PanelType.MESSAGE:
                    break;
                case PanelType.DIAGNOSTIC_ALARM:
                    panel = new DiagAlarmPanel(mainWindow, (DiagnosticGaugeSettings)panelSettings);
                    break;
                case PanelType.RADIAL_GAUGE:
                    panel = new RadialGauge(mainWindow, (RadialGaugeSettings)panelSettings);
                    break;
                case PanelType.DAYNIGHT_GAUGE:
                    panel = new DayNightGauge(mainWindow, (DayNightGaugeSettings)panelSettings);
                    break;
                default:
                    break;
            }
            if (panel != null)  //Append panel to the grid for display
            {
                PanelList.Add(panel);
                Children.Add(panel.border);
                panel.PromoteToFront();
                panel.Init();
                panel.Refresh();
            }
        }

        private void SetPanelParents()
        {
            foreach (var panel in PanelList)
            {
                if (panel.ParentPanelNumber != 0 && panel.ParentPanelNumber != panel.number)
                {
                    var parentPanel = PanelList.Find(x => (x.number == panel.ParentPanelNumber));
                    panel.BecomeChild(parentPanel);
                }
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
            var panel = GetClickedPanel(e);
            if (InAttachMode)
            {
                if (SetSelectedChild(e.Source))
                {
                    selectedChild.isMoving = selectedChild.isResizing = false;
                    PanelToAttach?.BecomeChild(selectedChild);
                    selectedChild = PanelToAttach;
                    InAttachMode = false;
                }
            }
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

        protected void WritePoint(MouseButtonEventArgs e)
        {
            //VMSConsole.PrintLine($"X: {e.GetPosition(this).X}");
            //VMSConsole.PrintLine($"Y: {e.GetPosition(this).Y}");
        }

        protected VPanel GetClickedPanel(dynamic src)
        {
            if (src == null)
            {
                return null;
            }
            try
            {
                foreach (VPanel panel in PanelList)
                {
                    dynamic source = src;
                    if (source.GetType() == panel.border.GetType())
                    {
                        if (panel.border == source)
                        {
                            movementType = MOVEMENT_RESIZE;
                            selectedChild.isResizing = true;
                            return panel;
                        }
                    }
                    else
                    {
                        while (source.GetType() != panel.canvas.GetType())
                        {
                            if (source.GetType() == mainWindow.GetType())
                            {
                                return panel;
                            }
                            source = VisualTreeHelper.GetParent(source);
                        }
                        if (panel.canvas == source)
                        {
                            movementType = MOVEMENT_MOVE;
                            return panel;
                        }
                    }
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                //do nothing - figure out how to do this without throwing this exception...
            }
            catch (Exception ex)
            {
                ErrorLogger.GenerateErrorRecord(ex);
                return null;
            }
            return null;
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
                highlightedChild = null;
                foreach (VPanel panel in PanelList)
                {
                    dynamic source = src;
                    if (source.GetType() == panel.border.GetType())
                    {
                        if (panel.border == source)
                        {
                            highlightedChild = selectedChild = panel;
                            movementType = MOVEMENT_RESIZE;
                            selectedChild.isResizing = true;
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
                            SetZIndices();
                            return true;
                        }
                    }
                }
                return (selectedChild != null);
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                //do nothing - figure out how to do this without throwing this exception...
            }
            catch (Exception ex)
            {
                ErrorLogger.GenerateErrorRecord(ex);
            }
            return false;
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
                    panel.SetZIndex(oldZIndex - 1);
                }
            }
            selectedChild.SetZIndex(curMax);
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

        public void ProcessArrowNavigation(Key direction, bool isAltDown, bool isControlDown, bool isShiftDown)
        {
            if (isShiftDown)
            {
                switch (direction)
                {
                    case Key.Left:
                        if (isControlDown)
                            highlightedChild?.DecrementRight();
                        else
                            highlightedChild?.IncrementLeft();
                        break;
                    case Key.Up:
                        if (isControlDown)
                            highlightedChild?.DecrementBottom();
                        else
                            highlightedChild?.IncrementTop();
                        break;
                    case Key.Right:
                        if (isControlDown)
                            highlightedChild?.DecrementLeft();
                        else
                            highlightedChild?.IncrementRight();
                        break;
                    case Key.Down:
                        if (isControlDown)
                            highlightedChild?.DecrementTop();
                        else
                            highlightedChild?.IncrementBottom();
                        break;
                }
            }
            else
            {
                highlightedChild?.Slide(direction);
            }
        }

        public void ProcessArrowRelease()
        {
            highlightedChild?.Refresh();
            SavePanels();
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
            //Screen.SaveConfiguration();
        }
    }
}
