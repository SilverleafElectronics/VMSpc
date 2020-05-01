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
using VMSpc.UI.DlgWindows;
using VMSpc.Panels;
using VMSpc.UI.CustomComponents;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using System.Globalization;
using System.Windows.Controls;
using VMSpc.Enums.UI;

/*
 * TODO - this class is wayyy too big. Factor out the following into helper classes:
 *  - Sliding
 *  - Resizing
 *  - probably a PanelMovement class used by Sliding/Resising (also, remove all movement functionality from PanelGrid.cs)
 *  - ChildPanelHandler - should interact with PanelMovement somehow
 */

namespace VMSpc.Panels
{
    /// <summary>
    /// base class of all panel elements
    /// </summary>
    public abstract class VPanel
    {
        public PanelType panelId { get; set; }
        protected MainWindow mainWindow;
        protected PanelSettings panelSettings;
        public Border border;
        public VMSCanvas canvas;

        public ulong ParentPanelNumber
        {
            get
            {
                return panelSettings.parentPanelNumber;
            }
            set
            {
                panelSettings.parentPanelNumber = value;
            }
        }
        private VPanel _ParentPanel { get; set; }
        private VPanel ParentPanel
        {
            get 
            {
                return _ParentPanel;
            }
            set
            {
                _ParentPanel = value;
                if (value != null)
                {
                    ParentPanelNumber = value.number;
                }
                else
                {
                    ParentPanelNumber = 0;
                }
            }
        }
        private List<VPanel> ChildPanels;
        private ParentPaddingRelation ParentPadding;

        private double BorderThickness;

        private int resizeType;

        public double leftLimit;
        public double topLimit;
        public double rightLimit;
        public double bottomLimit;

        public double Top
        {
            get { return Canvas.GetTop(border); }
            set { Canvas.SetTop(border, value); Canvas.SetBottom(border, value + Height); }
        }
        public double Right
        {
            get { return Canvas.GetRight(border); }
            set { Canvas.SetRight(border, value); }
        }
        public double Bottom
        {
            get { return Canvas.GetBottom(border); }
            set { Canvas.SetBottom(border, value); }
        }
        public double Left
        {
            get { return Canvas.GetLeft(border); }
            set { Canvas.SetLeft(border, value); Canvas.SetRight(border, value + Width); }
        }
        public double Height
        {
            get { return border.Height; }
            set { border.Height = value; Bottom = Top + Height; }
        }
        public double Width
        {
            get { return border.Width; }
            set { border.Width = value; Right = Left + Width; }
        }

        public bool isMoving, isResizing, isHighlighted;

        public ulong number;

        private VMSDialog dlgWindow;

        private Timer panelTimer;

        public VPanel(MainWindow mainWindow, PanelSettings panelSettings)
        {
            this.mainWindow = mainWindow;
            this.panelSettings = panelSettings;
            ParentPadding = new ParentPaddingRelation()
            {
                TopPadding = 0,
                RightPadding = 0,
                BottomPadding = 0,
                LeftPadding = 0
            };
            number = panelSettings.number;
            panelId = panelSettings.panelId;
            BorderThickness = 5;
            isMoving = false;
            isResizing = false;
            resizeType = RESIZE_NONE;
            dlgWindow = null;
            InitLimits();
            border = new Border()
            {
                BorderThickness = new Thickness(BorderThickness),
                BorderBrush = new SolidColorBrush(panelSettings.borderColor),
            };
            canvas = new VMSCanvas();
            ApplyBorderDimensions();
            ApplyCanvasDimensions();
            border.Child = canvas;
            GenerateCustomEventHandlers();
            panelTimer = CREATE_TIMER(OnUpdateTimedEvent, 500);
            ChildPanels = new List<VPanel>();
        }

        public void ApplyBorderDimensions()
        {
            double width = panelSettings.panelCoordinates.bottomRightX - panelSettings.panelCoordinates.topLeftX;
            double height = panelSettings.panelCoordinates.bottomRightY - panelSettings.panelCoordinates.topLeftY;
            border.Width = (width > 0) ? width : 5d;
            border.Height = (height > 0) ? height : 5d;
            Canvas.SetTop(border, panelSettings.panelCoordinates.topLeftY);
            Canvas.SetLeft(border, panelSettings.panelCoordinates.topLeftX);
            Canvas.SetRight(border, Canvas.GetLeft(border) + border.Width);
            Canvas.SetBottom(border, Canvas.GetTop(border) + border.Height);
        }

        public void ApplyCanvasDimensions()
        {
            Canvas.SetTop(canvas, Canvas.GetTop(border) - BorderThickness);
            Canvas.SetLeft(canvas, Canvas.GetLeft(border) - BorderThickness);
            Canvas.SetRight(canvas, Canvas.GetRight(border) - BorderThickness);
            Canvas.SetBottom(canvas, Canvas.GetBottom(border) - BorderThickness);
            double width = border.Width - (BorderThickness * 2);
            double height = border.Height - (BorderThickness * 2);
            canvas.Width = (width > 0) ? width : 1d;
            canvas.Height = (height > 0) ? height : 1d;
        }


        public virtual void Init()
        {
            canvas.Background = new SolidColorBrush(panelSettings.backgroundColor);
        }

        ~VPanel()
        {

        }

        protected virtual void GenerateCustomEventHandlers()
        {
            border.MouseEnter += OnMouseOverBorder;
            border.MouseLeave += OnMouseLeaveBorder;
            canvas.MouseEnter += OnMouseLeaveBorder;
            canvas.MouseLeave += OnMouseOverBorder;
            canvas.MouseMove += OnMouseLeaveBorder;
            canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            dlgWindow = GenerateDlg();
            dlgWindow.Owner = mainWindow;
            if (dlgWindow != null)
            {
                bool? result = dlgWindow.ShowDialog(this);
                if (result == true)
                {
                    Refresh();
                    ConfigManager.Screen.SaveConfiguration();
                }
            }
        }

        public void AcquireChild(VPanel panel)
        {
            ChildPanels.Add(panel);
        }

        public void LoseChild(VPanel panel)
        {
            ChildPanels.Remove(this);
        }

        public void BecomeChild(VPanel panel)
        {
            if (ChildPanels.Contains(panel))    //avoid circular dependencies
                return;
            ParentPanel?.LoseChild(this);
            ParentPanel = panel;
            panel.AcquireChild(this);
            AdjustParentPadding();
        }

        public void PromoteToFront()
        {
            SetZIndex((int)(panelSettings.number + 10));
            SetZIndex((int)(panelSettings.number + 10));
        }

        public void SetZIndex(int zIndex)
        {
            Panel.SetZIndex(this.canvas, zIndex);
            Panel.SetZIndex(this.border, zIndex);
            foreach (var child in ChildPanels)
            {
                child.SetZIndex(zIndex + 1);
            }
        }

        public bool IsInsidePanel(Point point)
        {
            return
                (point.X > (Canvas.GetLeft(border))) &&
                (point.X < (Canvas.GetRight(border))) &&
                (point.Y > (Canvas.GetTop(border))) &&
                (point.Y < (Canvas.GetBottom(border)));
        }

        public void Highlight()
        {
            border.BorderBrush = Brushes.DarkGray;
        }

        public void UnHighlight()
        {
            border.BorderBrush = new SolidColorBrush(panelSettings.borderColor);
        }


        #region Movement

        public void OnMouseOverBorder(object sender, MouseEventArgs e)
        {
            if (isMoving) return;
            DetermineCursor(e.GetPosition(mainWindow));
        }

        private void DetermineCursor(Point pos)
        {
            resizeType = 0;
            if (pos.X <= Canvas.GetLeft(border) + 20)
                resizeType |= RESIZE_LEFT;
            if (pos.X >= Canvas.GetRight(border) - 20)
                resizeType |= RESIZE_RIGHT;
            if (pos.Y <= Canvas.GetTop(border) + 20)
                resizeType |= RESIZE_TOP;
            if (pos.Y >= Canvas.GetBottom(border) - 20)
                resizeType |= RESIZE_BOTTOM;

            switch (resizeType)
            {
                case RESIZE_LEFT: 
                case RESIZE_RIGHT: 
                    Mouse.OverrideCursor = Cursors.SizeWE; 
                    break;
                case RESIZE_TOP: 
                case RESIZE_BOTTOM: 
                    Mouse.OverrideCursor = Cursors.SizeNS; 
                    break;
                case RESIZE_TOPLEFT: 
                case RESIZE_BOTTOMRIGHT: 
                    Mouse.OverrideCursor = Cursors.SizeNWSE; 
                    break;
                case RESIZE_BOTTOMLEFT: 
                case RESIZE_TOPRIGHT: 
                    Mouse.OverrideCursor = Cursors.SizeNESW; 
                    break;
                default: 
                    Mouse.OverrideCursor = Cursors.Arrow; 
                    break;
            }
        }

        public void OnMouseLeaveBorder(object sender, MouseEventArgs e)
        {
            if (isResizing) return;
            Mouse.OverrideCursor = Cursors.Arrow;
            resizeType = RESIZE_NONE;
        }

        public void ProcessMouseRelease()
        {
            if (isResizing)
            {
                Refresh();
                AdjustChildren(AdjustmentFor.Resizing);
                AdjustParentPadding();
            }
            else
            {
                AdjustChildren(AdjustmentFor.Movement);
                AdjustParentPadding();
            }
            isMoving = false;
            isResizing = false;
        }

        public void Resize(Point cursorPoint)
        {
            switch (resizeType)
            {
                case RESIZE_LEFT:
                    ResizeLeft(cursorPoint.X);
                    break;
                case RESIZE_RIGHT:
                    ResizeRight(cursorPoint.X);
                    break;
                case RESIZE_TOP:
                    ResizeTop(cursorPoint.Y);
                    break;
                case RESIZE_BOTTOM:
                    ResizeBottom(cursorPoint.Y);
                    break;
                case RESIZE_BOTTOMLEFT:
                    ResizeBottom(cursorPoint.Y);
                    ResizeLeft(cursorPoint.X);
                    break;
                case RESIZE_BOTTOMRIGHT:
                    ResizeBottom(cursorPoint.Y);
                    ResizeRight(cursorPoint.X);
                    break;
                case RESIZE_TOPLEFT:
                    ResizeTop(cursorPoint.Y);
                    ResizeLeft(cursorPoint.X);
                    break;
                case RESIZE_TOPRIGHT:
                    ResizeTop(cursorPoint.Y);
                    ResizeRight(cursorPoint.X);
                    break;
                default:
                    break;
            }
            //AdjustChildren(AdjustmentFor.Resizing);
            ApplyCanvasDimensions();
        }

        public void ResizeTop(double newTop)
        {
            var oldBottom = Canvas.GetBottom(border);
            Canvas.SetTop(border, newTop);
            Height = oldBottom - newTop;
        }

        public void IncrementTop()
        {
            Top -= 1;
            Height += 1;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void DecrementTop()
        {
            Top += 1;
            Height -= 1;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void ResizeRight(double newRight)
        {
            Width = (newRight - Canvas.GetLeft(border));
            Canvas.SetRight(border, (Canvas.GetLeft(canvas) + border.Width));
        }

        public void IncrementRight()
        {
            Width++;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void DecrementRight()
        {
            Width--;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void ResizeBottom(double newBottom)
        {
            Height = (newBottom - Canvas.GetTop(border));
        }

        public void IncrementBottom()
        {
            Height++;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void DecrementBottom()
        {
            Height--;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void ResizeLeft(double newLeft)
        {
            var oldRight = Canvas.GetRight(border);
            Canvas.SetLeft(border, newLeft);
            Width = oldRight - newLeft;
        }

        public void IncrementLeft()
        {
            Left -= 1;
            Width += 1;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void DecrementLeft()
        {
            Left += 1;
            Width -= 1;
            ApplyCanvasDimensions();
            AdjustParentPadding();
            AdjustChildren(AdjustmentFor.Resizing);
        }

        public void Slide(Key direction)
        {
            switch (direction)
            {
                case Key.Left:
                    BoundlessSetHorizontal(Canvas.GetLeft(border) - 1);
                    break;
                case Key.Right:
                    BoundlessSetHorizontal(Canvas.GetLeft(border) + 1);
                    break;
                case Key.Up:
                    BoundlessSetVertical(Canvas.GetTop(border) - 1);
                    break;
                case Key.Down:
                    BoundlessSetVertical(Canvas.GetTop(border) + 1);
                    break;
            }
            AdjustChildren(AdjustmentFor.Movement);
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
            rightLimit = mainWindow.ContentGrid.ActualWidth;
            bottomLimit = mainWindow.ContentGrid.ActualHeight;
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

        /// <summary>
        /// Moves the panel vertically if doing so will not trespass on another panel's territory. The panel can move freely if Clipping is disabled.
        /// </summary>
        public void SetVertical(double newTop)
        {
            double clipSide;
            if (CanMove(VERTICAL, newTop))
            {
                clipSide = GetVerticalClipSide(newTop);
                if (!Double.IsNaN(clipSide))
                    ClipVertical(clipSide);
                else
                {
                    Top = newTop;
                    Bottom = newTop + border.Height;
                    //AdjustChildren(AdjustmentFor.Movement);
                }
                AdjustParentPadding();
            }
        }

        /// <summary>
        /// Moves the panel vertically regardless of clipping or the position of neighboring panels
        /// </summary>
        public void BoundlessSetVertical(double newTop)
        {
            Top = newTop;
            Bottom = newTop + border.Height;
            //AdjustChildren(AdjustmentFor.Movement);
            AdjustParentPadding();
        }

        /// <summary>
        /// Moves the panel horizontally if doing so will not trespass on another panel's territory. The panel can move freely if Clipping is disabled.
        /// </summary>
        public void SetHorizontal(double newLeft)
        {
            double clipSide;
            if (CanMove(HORIZONTAL, newLeft))
            {
                clipSide = GetHorizontalClipSide(newLeft);
                if (!Double.IsNaN(clipSide))
                    ClipHorizontal(clipSide);
                else
                {
                    Left = newLeft;
                    Right = newLeft + border.Width;
                    //AdjustChildren(AdjustmentFor.Movement);
                }
                AdjustParentPadding();
            }
        }

        /// <summary>
        /// Moves the panel horizontally regardless of clipping or the position of neighboring panels
        /// </summary>
        /// <param name="newTop"></param>
        public void BoundlessSetHorizontal(double newLeft)
        {
            Left = newLeft;
            Right = newLeft + border.Width;
            //AdjustChildren(AdjustmentFor.Movement);
            AdjustParentPadding();
        }

        public enum AdjustmentFor
        {
            Movement,
            Resizing,
        }

        private void AdjustChildren(AdjustmentFor For)
        {
            foreach (var child in ChildPanels)
                child.Adjust(For);
        }

        public void Adjust(AdjustmentFor For)
        {
            if (ParentPanel == null)
                return;
            switch (For)
            {
                case AdjustmentFor.Movement:
                    Left = ParentPanel.Left + ParentPadding.LeftPadding;
                    Top = ParentPanel.Top + ParentPadding.TopPadding;
                    AdjustParentPadding();
                    ApplyCanvasDimensions();
                    //Right = ParentPanel.Right + ParentPadding.RightPadding;
                    //Bottom = ParentPanel.Bottom + ParentPadding.BottomPadding;
                    break;
                case AdjustmentFor.Resizing:
                    double oldHeight = Height;
                    double oldWidth = Width;
                    Height = ParentPanel.Height * ParentPadding.HeightRatio;
                    Width = ParentPanel.Width * ParentPadding.WidthRatio;
                    Left = (ParentPadding.LeftPaddingRatio * ParentPanel.Width);
                    Top = (ParentPadding.TopPaddingRatio * ParentPanel.Height);
                    AdjustParentPadding();
                    //Left += ((Width - oldWidth) / 2);
                    //Top += ((Height - oldHeight) / 2);
                    ApplyCanvasDimensions();
                    Refresh();
                    break;
            }
            AdjustChildren(For);
        }

        public struct ParentPaddingRelation
        {
            public double
                TopPadding,
                RightPadding,
                BottomPadding,
                LeftPadding;
            public double
                LeftPaddingRatio,
                TopPaddingRatio,
                HeightRatio,
                WidthRatio;
        }

        /// <summary>
        /// Called on the child panel by the child panel when it has been resized or moved to maintain the padding between itself and its parent
        /// </summary>
        /// <param name="For"></param>
        private void AdjustParentPadding()
        {
            if (ParentPanel == null)
                return;
            ParentPadding.LeftPadding = Left - ParentPanel.Left;
            ParentPadding.TopPadding = Top - ParentPanel.Top;
            ParentPadding.RightPadding = ParentPanel.Right - Right;
            ParentPadding.BottomPadding = ParentPanel.Bottom - Bottom;
            ParentPadding.LeftPaddingRatio = Left / ParentPanel.Width;
            ParentPadding.TopPaddingRatio = Top / ParentPanel.Height;
            ParentPadding.HeightRatio = Height / ParentPanel.Height;
            ParentPadding.WidthRatio = Width / ParentPanel.Width;
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
            }
            else if (side == DOWN)
            {
                Canvas.SetTop(border, bottomLimit - border.Height);
                Canvas.SetBottom(border, bottomLimit);
            }
        }

        private void ClipHorizontal(double side)
        {
            if (side == LEFT)
            {
                Canvas.SetLeft(border, leftLimit);
                Canvas.SetRight(border, leftLimit + border.Width);
            }
            else if (side == RIGHT)
            {
                Canvas.SetLeft(border, rightLimit - border.Width);
                Canvas.SetRight(border, rightLimit);
            }
        }

        #endregion Movement

        public void Refresh()
        {
            GeneratePanel();
            foreach (var child in ChildPanels)
                child.Refresh();
        }

        public abstract void GeneratePanel();

        public abstract void UpdatePanel();

        /// <summary> Returns a call to the constructor of the corresponding Dialog Window for this panel </summary>
        protected abstract VMSDialog GenerateDlg();

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
            panelSettings.panelCoordinates.bottomRightX = (int)Canvas.GetRight(border);
            panelSettings.panelCoordinates.bottomRightY = (int)Canvas.GetBottom(border);
            panelSettings.panelCoordinates.topLeftX = (int)Canvas.GetLeft(border);
            panelSettings.panelCoordinates.topLeftY = (int)Canvas.GetTop(border);
            VMSConsole.PrintLine("Top: " + Canvas.GetTop(border));
            VMSConsole.PrintLine("Bottom: " + Canvas.GetBottom(border));
            VMSConsole.PrintLine("Left: " + Canvas.GetLeft(border));
            VMSConsole.PrintLine("Right: " + Canvas.GetRight(border));
        }

        private void OnUpdateTimedEvent()
        {
            try // Wrapped in try block, in case the user closes while this thread is executing
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdatePanel();
                }
                );
            }
            catch { }
        }
    }
}
