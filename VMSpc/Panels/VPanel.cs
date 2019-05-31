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

namespace VMSpc.Panels
{
    public abstract class VPanel
    {
        public char cID;
        private MainWindow mainWindow;
        private PanelSettings panelSettings;
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

        private int counter;

        public VPanel(MainWindow mainWindow, PanelSettings panelSettings)
        {
            this.mainWindow = mainWindow;
            this.panelSettings = panelSettings;
            border = new Border();
            canvas = new VMSCanvas(mainWindow, border, panelSettings);
            border.Child = canvas;
            isLeftClipped = isTopClipped = isRightClipped = isBottomClipped = false;
            InitLimits();
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
        /// compares limits against the dimensions of the provided VPanel. Changes the limits if the dimensions are closer to this panel's current offset
        /// </summary>
        /// <param name="panel"></param>
        public void SetDirectionalLimits(VPanel panel)
        {
            var panelLeft = Canvas.GetLeft(panel.border);
            var panelTop = Canvas.GetTop(panel.border);
            var panelRight = Canvas.GetRight(panel.border);
            var panelBottom = Canvas.GetBottom(panel.border);
            if (
                (Canvas.GetTop(border) >= panelTop && Canvas.GetTop(border) <= panelBottom)
                ||
                (Canvas.GetBottom(border) <= panelBottom && Canvas.GetBottom(border) >= panelTop)
            )
            {
                if ((panelRight > (leftLimit)) && (panelRight <= Canvas.GetLeft(border)))
                    leftLimit = panelRight;
                if ((panelLeft < (rightLimit)) && (panelLeft >= Canvas.GetRight(border)))
                    rightLimit = panelLeft;
            }
            if (
                (Canvas.GetLeft(border) >= panelLeft && Canvas.GetLeft(border) <= panelRight)
                ||
                (Canvas.GetRight(border) <= panelRight && Canvas.GetRight(border) >= panelLeft)
            )
            {
                if ((panelBottom > (topLimit)) && (panelBottom <= Canvas.GetTop(border)))
                    topLimit = panelBottom;
                if ((panelTop < (bottomLimit)) && (panelTop >= Canvas.GetBottom(border)))
                    bottomLimit = panelTop;
            }
        }

        /// <summary>
        /// Determines if there is still space to move between the panel and it's bounding Horizontal or Vertical neighbor
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="newVal"></param>
        /// <returns></returns>
        public bool CanMove(int direction, double newVal)
        {
            if (!BreaksClip(direction, newVal))
                return false;
            switch (direction)
            {
                case Constants.HORIZONTAL:
                    return (newVal > leftLimit && (newVal + border.Width) < rightLimit);
                case Constants.VERTICAL:
                    return (newVal > topLimit && (newVal + border.Height) < bottomLimit);
                default:
                    return false;
            }
        }

        private bool BreaksClip(int direction, double newVal)
        {
            bool allowed = true;
            if (direction == Constants.VERTICAL)
            {
                if (isTopClipped)
                {
                    if (newVal < (topLimit + 20))
                    {
                        allowed = false;
                        isTopClipped = false;
                    }
                    else if (newVal >= topLimit)
                    {

                    }
                }
                if (isBottomClipped)
                {
                    if ((newVal + border.Height) > (bottomLimit - 20))
                    {
                        allowed = false;
                        isBottomClipped = false;
                    }
                    else if ((newVal + border.Height) <= bottomLimit)
                    {

                    }
                }
            }
            if (direction == Constants.HORIZONTAL)
            {
                if (isLeftClipped)
                {
                    if (newVal < (leftLimit + 20))
                    {
                        allowed = false;
                        isLeftClipped = false;
                    }
                }
                if (isRightClipped)
                {
                    if (newVal > (rightLimit - 20))
                    {
                        allowed = false;
                        isRightClipped = false;
                    }
                }
            }
            return allowed;
        }

        public void SetVertical(double newTop, Point newCursorPoint)
        {
            double clipSide;
            if (CanMove(Constants.VERTICAL, newTop))
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
            if (CanMove(Constants.HORIZONTAL, newLeft))
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
                return Constants.UP;
            else if (newBottom >= (bottomLimit - 20))
                return Constants.DOWN;
            return Double.NaN;
        }

        /// <summary>
        /// Determines which vertical side needs to be clipped to the bounding neighbor.
        /// </summary>
        /// <param name="newTop"></param>
        /// <returns>The side to be clipped. NaN, if no clipping should occur</returns>
        private double GetHorizontalClipSide(double newLeft)
        {
            double newRight = newLeft + border.Width;
            if (newLeft <= (leftLimit + 20))
                return Constants.LEFT;
            else if (newRight >= (rightLimit - 20))
                return Constants.RIGHT;
            return Double.NaN;
        }

        private void ClipVertical(double side)
        {
            if (side == Constants.UP)
            {
                Canvas.SetTop(border, topLimit);
                Canvas.SetBottom(border, topLimit + border.Height);
                isTopClipped = true;
            }
            else if (side == Constants.DOWN)
            {
                Canvas.SetTop(border, bottomLimit - border.Height);
                Canvas.SetBottom(border, bottomLimit);
                isBottomClipped = true;
            }
        }

        private void ClipHorizontal(double side)
        {
            if (side == Constants.LEFT)
            {
                Canvas.SetLeft(border, leftLimit);
                Canvas.SetRight(border, leftLimit + border.Width);
                isLeftClipped = true;
            }
            else if (side == Constants.RIGHT)
            {
                Canvas.SetLeft(border, rightLimit - border.Width);
                Canvas.SetRight(border, rightLimit);
                isRightClipped = true;
            }
        }

        public abstract void GeneratePanel();

    }
}
