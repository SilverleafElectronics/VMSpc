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

namespace VMSpc.CustomComponents
{
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
        }

        public void InitPanels(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            int numPanels = DefaultScrnManager.scrnManager.GetPanelCount();
            for (int i = 1; i <= numPanels; i++)
            {
                PanelSettings panelSettings = DefaultScrnManager.scrnManager.GetPanel(i);
                VPanel panel = null;
                switch (panelSettings.ID)
                {
                    case Constants.PanelIDs.SIMPLE_GAUGE_ID:
                        panel = new VSimpleGauge(mainWindow, panelSettings);
                        break;
                    case Constants.PanelIDs.SCAN_GAUGE_ID:
                        break;
                    case Constants.PanelIDs.ODOMOTER_ID:
                        break;
                    case Constants.PanelIDs.TRANSMISSION_ID:
                        break;
                    case Constants.PanelIDs.MULTIBAR_ID:
                        break;
                    case Constants.PanelIDs.HISTOGRAM_ID:
                        break;
                    case Constants.PanelIDs.CLOCK_PANEL_ID:
                        break;
                    case Constants.PanelIDs.IMG_PANEL_ID:
                        break;
                    case Constants.PanelIDs.TEXT_PANEL_ID:
                        break;
                    case Constants.PanelIDs.TANK_MINDER_ID:
                        break;
                    case Constants.PanelIDs.TIRE_PANEL_ID:
                        break;
                    case Constants.PanelIDs.MESSAGE_PANEL_ID:
                        break;
                    case Constants.PanelIDs.DIAG_ALARM_ID:
                        break;
                    case Constants.PanelIDs.RADIAL_GAUGE_ID:
                        break;
                    default:
                        break;
                }
                if (panel != null)
                    PanelList.Add(panel);
            }
        }

        public void LoadPanels()
        {
            foreach (VPanel panel in PanelList)
            {
                panel.GeneratePanel();
                Children.Add(panel.border);
            }
        }

        #region overrides

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Init();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            isDragging = false;
            SetSelectedChild(e.Source);
            cursorStartPoint = e.GetPosition(this);
            canvasStartTop = GetTop(selectedChild.border);
            if (Double.IsNaN(canvasStartTop)) canvasStartTop = 0;
            canvasStartLeft = GetLeft(selectedChild.border);
            if (Double.IsNaN(canvasStartLeft)) canvasStartLeft = 0;
            lastCursorY = canvasStartTop;
            lastCursorX = canvasStartLeft;
            isDragging = true;
        }

        /// <summary>
        /// Sets selectedChild to the source of the click. If the user has clicked outside of the panel, selectedChild is set to null
        /// </summary>
        /// <param name="src"></param>
        private void SetSelectedChild(object src)
        {
            foreach (VPanel panel in PanelList)
            {
                VMSCanvas source;
                try
                {
                    source = (VMSCanvas)src;
                }
                catch //indicates the user has clicked outside of a panel
                {
                    selectedChild = null;
                    return;
                }
                if (panel.canvas == source)
                {
#if (Debug)
                    VMSConsole.PrintLine("Found");
#endif
                    selectedChild = panel;
                }
            }
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
            SetDirectionalLimits();
            double newTop, newLeft;
            Point newCursorPoint = e.GetPosition(src);
            double cursorXDiff = newCursorPoint.X - cursorStartPoint.X;
            double cursorYDiff = newCursorPoint.Y - cursorStartPoint.Y;
            newTop = canvasStartTop + cursorYDiff;
            newLeft = canvasStartLeft + cursorXDiff;
            SetTop(selectedChild.border, newTop);
            SetLeft(selectedChild.border, newLeft);
        }

        private void SetDirectionalLimits()
        {
            selectedChild.canvas.leftLimit = 0;
            selectedChild.canvas.rightLimit = mainWindow.Width;
            selectedChild.canvas.topLimit = mainWindow.Height;
            selectedChild.canvas.bottomLimit = 0;
            VMSConsole.PrintLine("Left limit: " + mainWindow.Width);
            VMSConsole.PrintLine("Current Left Position: " + GetLeft(selectedChild.border));
            VMSConsole.PrintLine("Current Right Position: " + GetRight(selectedChild.border));
        }

        #endregion

        private int GetVerticalDirection(double difference)
        {
            if (difference > lastCursorY)
                return Constants.DOWN;
            return Constants.UP;
        }
        private int GetHorizontalDirection(double difference)
        {
            if (difference > lastCursorX)
                return Constants.RIGHT;
            return Constants.LEFT;
        }
    }
}
