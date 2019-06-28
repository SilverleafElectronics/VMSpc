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
using VMSpc.CustomComponents;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using static VMSpc.Parsers.PresenterWrapper;
using System.Globalization;

namespace VMSpc.Panels
{
    /// <summary>
    /// base class of all panel elements
    /// </summary>
    public abstract class VPanel
    {
        public char cID;
        private MainWindow mainWindow;
        protected PanelSettings panelSettings;
        public Border border;
        public VMSCanvas canvas;

        public double leftLimit;
        public double topLimit;
        public double rightLimit;
        public double bottomLimit;

        private bool isLeftClipped;
        private bool isTopClipped;
        private bool isRightClipped;
        private bool isBottomClipped;

        protected VMSDialog dlgWindow;

        public VPanel(MainWindow mainWindow, PanelSettings panelSettings)
        {
            this.mainWindow = mainWindow;
            this.panelSettings = panelSettings;
            dlgWindow = null;
            Init();
        }

        protected void Init()
        {

            border = new Border() { BorderThickness = new Thickness(5, 5, 5, 5) };
            canvas = new VMSCanvas(mainWindow, border, panelSettings);
            border.Child = canvas;
            isLeftClipped = isTopClipped = isRightClipped = isBottomClipped = false;
            InitLimits();
            GenerateEventHandlers();
        }

        ~VPanel()
        {

        }

        private void GenerateEventHandlers()
        {
            //border.MouseEnter += OnMouseOverBorder;
            //border.MouseLeave += OnMouseLeaveBorder;
            //canvas.MouseEnter += OnMouseLeaveBorder;
            canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dlgWindow != null)
            {
                bool? result = dlgWindow.ShowDialog(this);
                if (result == true)
                    Init();
            }
        }

        public void OnMouseOverBorder(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeAll;
        }

        public void OnMouseLeaveBorder(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private bool IsWithinBoundary(double targetPosition, double boundaryLimit, double value)
        {
            return ((targetPosition + boundaryLimit >= targetPosition) && (targetPosition - boundaryLimit <= targetPosition));
        }

        /// <summary>
        /// Sets the limits of the panel to the dimensions of the enclosing window
        /// </summary>
        public void InitLimits()
        {
            leftLimit = 0;
            topLimit = 0;
            rightLimit = mainWindow.Width;
            bottomLimit = mainWindow.Height;
        }

        /// <summary>
        /// compares limits against the dimensions of the VPanel parameter. Changes the limits if the dimensions are closer to this panel's current offset
        /// </summary>
        /// <param name="panel"></param>
        public void SetDirectionalLimits(VPanel panel)
        {
            var panelLeft = Canvas.GetLeft(panel.border);
            var panelTop = Canvas.GetTop(panel.border);
            var panelRight = Canvas.GetRight(panel.border);
            var panelBottom = Canvas.GetBottom(panel.border);
            var thisPanelLeft = Canvas.GetLeft(border);
            var thisPanelRight = Canvas.GetRight(border);
            var thisPanelTop = Canvas.GetTop(border);
            var thisPanelBottom = Canvas.GetBottom(border);
            if (CollisionPossible(thisPanelTop, thisPanelBottom, panelTop, panelBottom))
            {
                if ((panelRight > (leftLimit)) && (panelRight <= Canvas.GetLeft(border)))
                    leftLimit = panelRight;
                if ((panelLeft < (rightLimit)) && (panelLeft >= Canvas.GetRight(border)))
                    rightLimit = panelLeft;
            }
            if (CollisionPossible(thisPanelLeft, thisPanelRight, panelLeft, panelRight))
            {
                if ((panelBottom > (topLimit)) && (panelBottom <= Canvas.GetTop(border)))
                    topLimit = panelBottom;
                if ((panelTop < (bottomLimit)) && (panelTop >= Canvas.GetBottom(border)))
                    bottomLimit = panelTop;
            }
        }

        private bool CollisionPossible(double tPanelEdge1, double tPanelEdge2, double nPanelEdge1, double nPanelEdge2)
        {
            if (        //is this panel's lower border (top or left) in between the parameter panel's corresponding edges?
                    (tPanelEdge1 >= nPanelEdge1 && tPanelEdge1 <= nPanelEdge2)
                    ||  //is this panel's upper border (bottom or right) in between the parameter panel's corresponding edges?
                    (tPanelEdge2 <= nPanelEdge2 && tPanelEdge2 >= nPanelEdge1)
                    ||  //is the parameter panel's lower border (top or left) in between this panel's corresponding edges?
                    (nPanelEdge1 >= tPanelEdge1 && nPanelEdge1 <= tPanelEdge2)
                    ||  //is the parameter panel's upper border (bottom or right) in between this panel's corresponding edges?
                    (nPanelEdge2 <= tPanelEdge2 && nPanelEdge2 >= tPanelEdge1)
                )
                return true;
            return false;
        }

        /// <summary>
        /// Determines if there is still space to move between the panel and it's bounding Horizontal or Vertical neighbor
        /// </summary>
        public bool CanMove(int direction, double newVal)
        {
            switch (direction)
            {
                case HORIZONTAL:
                    return (newVal >= leftLimit && (newVal + border.Width) <= rightLimit);
                case VERTICAL:
                    return (newVal >= topLimit && (newVal + border.Height) <= bottomLimit);
                default:
                    return false;
            }
        }

        public void SetVertical(double newTop, Point newCursorPoint)
        {
            double clipSide;
            if (CanMove(VERTICAL, newTop))
            {
                clipSide = GetVerticalClipSide(newTop);
                if (!Double.IsNaN(clipSide))
                    ClipVertical(clipSide);
                else
                {
                    Canvas.SetTop(border, newTop);
                    Canvas.SetBottom(border, newTop + border.Height);
                }
            }
        }

        public void SetHorizontal(double newLeft, Point newCursorPoint)
        {
            double clipSide;
            if (CanMove(HORIZONTAL, newLeft))
            {
                clipSide = GetHorizontalClipSide(newLeft);
                if (!Double.IsNaN(clipSide))
                    ClipHorizontal(clipSide);
                else
                {
                    Canvas.SetLeft(border, newLeft);
                    Canvas.SetRight(border, newLeft + border.Width);
                }
            }
        }

        /// <summary>
        /// Determines which vertical side needs to be clipped to the bounding neighbor.
        /// </summary>
        /// <param name="newTop"></param>
        /// <returns>The side to be clipped. NaN, if no clipping should occur</returns>
        private double GetVerticalClipSide(double newTop)
        {
            double newBottom = newTop + border.Height;
            if (newTop <= (topLimit + 20))
                return UP;
            else if (newBottom >= (bottomLimit - 20))
                return DOWN;
            return Double.NaN;
        }

        /// <summary>
        /// Determines which horizontal side needs to be clipped to the bounding neighbor.
        /// </summary>
        /// <param name="newTop"></param>
        /// <returns>The side to be clipped. NaN, if no clipping should occur</returns>
        private double GetHorizontalClipSide(double newLeft)
        {
            double newRight = newLeft + border.Width;
            if (newLeft <= (leftLimit + 20))
                return LEFT;
            else if (newRight >= (rightLimit - 20))
                return RIGHT;
            return Double.NaN;
        }

        private void ClipVertical(double side)
        {
            if (side == UP)
            {
                Canvas.SetTop(border, topLimit);
                Canvas.SetBottom(border, topLimit + border.Height);
                isTopClipped = true;
            }
            else if (side == DOWN)
            {
                Canvas.SetTop(border, bottomLimit - border.Height);
                Canvas.SetBottom(border, bottomLimit);
                isBottomClipped = true;
            }
        }

        private void ClipHorizontal(double side)
        {
            if (side == LEFT)
            {
                Canvas.SetLeft(border, leftLimit);
                Canvas.SetRight(border, leftLimit + border.Width);
                isLeftClipped = true;
            }
            else if (side == RIGHT)
            {
                Canvas.SetLeft(border, rightLimit - border.Width);
                Canvas.SetRight(border, rightLimit);
                isRightClipped = true;
            }
        }

        protected double GetPidValue(ushort pid)
        {
            return PresenterList[pid].datum.value;
        }

        protected double MeasureFontSize(string text, double maxWidth, double maxHeight)
        {

            return 12;
        }

        public abstract void GeneratePanel();

        public abstract void UpdatePanel();

        /// <summary>
        /// Assigns the appropriate right and bottom coordinates of an element. The element's width and height must already be set before calling this method
        /// </summary>
        protected void ApplyRightBottomCoords(FrameworkElement element)
        {
            Canvas.SetBottom(element, Canvas.GetTop(element) + element.Height);
            Canvas.SetRight(element, Canvas.GetLeft(element) + element.Width);
        }

        public void SaveSettings()
        {
            panelSettings.rectCord.bottomRightX = (int)Canvas.GetRight(border);
            panelSettings.rectCord.bottomRightY = (int)Canvas.GetBottom(border);
            panelSettings.rectCord.topLeftX = (int)Canvas.GetLeft(border);
            panelSettings.rectCord.topLeftY = (int)Canvas.GetTop(border);
            VMSConsole.PrintLine("Right: "+Canvas.GetRight(border));
            VMSConsole.PrintLine("Left: " + Canvas.GetLeft(border));
            VMSConsole.PrintLine("Top: " + Canvas.GetTop(border));
            VMSConsole.PrintLine("Bottom: " + Canvas.GetBottom(border));
        }

        protected void ScaleText(TextBlock textBlock, double maxWidth, double maxHeight)
        {
            textBlock.FontSize = 12;
            Size size = CalculateStringSize(textBlock);
            while (size.Width > maxWidth || size.Height > maxHeight)
            {
                    textBlock.FontSize--;
                    size = CalculateStringSize(textBlock);
            }
            while (size.Width < (maxWidth - 50) || size.Height < (maxHeight - 50))
            {
                textBlock.FontSize++;
                size = CalculateStringSize(textBlock);
            }
        }

        private Size CalculateStringSize(TextBlock textBlock)
        {
            if (textBlock.Text == "")
                return new Size(0, 0);
            FormattedText text = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);
            return new Size(text.Width, text.Height);
        }
        
        protected void BalanceTextBlocks(dynamic parent)
        {
            double min = Double.MaxValue;

            foreach (var block in parent.Children)
            {
                if (block.GetType().ToString() == "System.Windows.Controls.TextBlock")
                {
                    if (((TextBlock)block).FontSize < min)
                        min = ((TextBlock)block).FontSize;
                }
                else if (block.GetType().ToString() == "System.Windows.Controls.Border")
                {
                    if (((Border)block).Child.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    {
                        TextBlock textBlock = (TextBlock)((Border)block).Child;
                        if (textBlock.FontSize < min)
                            min = textBlock.FontSize;
                    }
                }
            }
            foreach (var block in parent.Children)
            {
                if (block.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    ((TextBlock)block).FontSize = min;
                else if (block.GetType().ToString() == "System.Windows.Controls.Border")
                {
                    if (((Border)block).Child.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    {
                        TextBlock textBlock = (TextBlock)((Border)block).Child;
                        textBlock.FontSize = min;
                    }

                }
            }
        }
    }
}
