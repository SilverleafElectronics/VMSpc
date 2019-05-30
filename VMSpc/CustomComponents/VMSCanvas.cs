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


namespace VMSpc.Panels
{
    public class VMSCanvas : Canvas
    {
        private Border border;
        private PanelSettings panelSettings;
        private PanelManager panelManager;
        private MainWindow parent;
        private Point cursorStartPoint;
        private double canvasStartTop;
        private double canvasStartLeft;
        private int zIndex;
        private bool isDragging;



        public VMSCanvas(Border border, PanelSettings panelSettings, PanelManager panelManager, MainWindow parent) 
            : base()
        {
            zIndex = 0;
            isDragging = false;
            this.border = border;
            this.panelSettings = panelSettings;
            this.panelManager = panelManager;
            this.parent = parent;
            ApplyBorderDimensions();
        }

        private void ApplyBorderDimensions()
        {
            border.Width = panelSettings.rectCord.bottomRightX - panelSettings.rectCord.topLeftX;
            border.Height = panelSettings.rectCord.bottomRightY - panelSettings.rectCord.topLeftY;
            SetTop(border, panelSettings.rectCord.topLeftY);
            SetLeft(border, panelSettings.rectCord.topLeftX);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            isDragging = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            isDragging = false;
            cursorStartPoint = e.GetPosition(parent);
            canvasStartTop = GetTop(border);
            if (Double.IsNaN(canvasStartTop)) canvasStartTop = 0;
            canvasStartLeft = GetLeft(border);
            if (Double.IsNaN(canvasStartLeft)) canvasStartLeft = 0;
            isDragging = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!isDragging)
                return;
            double newTop, newLeft;
            Point newCursorPoint = e.GetPosition(parent);
            double cursorXDiff = newCursorPoint.X - cursorStartPoint.X;
            double cursorYDiff = newCursorPoint.Y - cursorStartPoint.Y;
#if (DEBUG)
            VMSConsole.PrintLine("X Beg: " + cursorStartPoint.X);
            VMSConsole.PrintLine("Y Beg: " + cursorStartPoint.Y);
            VMSConsole.PrintLine("X Diff: "+cursorXDiff);
            VMSConsole.PrintLine("Y Diff:" + cursorYDiff);
#endif
            newTop = canvasStartTop + cursorYDiff;
            newLeft = canvasStartLeft + cursorXDiff;
            SetTop(border, newTop);
            SetLeft(border, newLeft);
        }


/*
        /// <summary>
        /// Changes the 
        /// </summary>
        public void UpdateZIndex(bool isTop)
        {
            if (isTop)
            {

            }
        }

        /// <summary>
        /// Sets the z-index of the VMSCanvas, which should have previously been saved by UPdateZIndex
        /// </summary>
        /// <param name="canvas"></param>
        public void SetZIndex(VMSCanvas canvas, bool isTop)
        {
            Canvas.SetZIndex(this, ZIndex);
        }
*/
    }
}
