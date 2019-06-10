﻿using System;
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

namespace VMSpc.Panels
{
    /// <summary>
    /// base class of all panel elements
    /// </summary>
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

        private Timer updateTimer;

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
            if (    //is this panel's top in between the parameter panel's vertical edges?
                    (tPanelEdge1 >= nPanelEdge1 && tPanelEdge1 <= nPanelEdge2)
                    ||  //is this panel's bottom in between the parameter panel's vertical edges?
                    (tPanelEdge2 <= nPanelEdge2 && tPanelEdge2 >= nPanelEdge1)
                    ||  //is the parameter panel's top in between this panel's vertical edges?
                    (nPanelEdge1 >= tPanelEdge1 && nPanelEdge1 <= tPanelEdge2)
                    ||  //is the parameter panel's bottom in between this panel's vertical edges?
                    (nPanelEdge2 <= tPanelEdge2 && nPanelEdge2 >= tPanelEdge1)
                )
                return true;
            return false;
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
                case HORIZONTAL:
                    return (newVal > leftLimit && (newVal + border.Width) < rightLimit);
                case VERTICAL:
                    return (newVal > topLimit && (newVal + border.Height) < bottomLimit);
                default:
                    return false;
            }
        }

        private bool BreaksClip(int direction, double newVal)
        {
            bool allowed = true;
            if (direction == VERTICAL)
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
            if (direction == HORIZONTAL)
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

        public abstract void GeneratePanel();

        public abstract void UpdatePanel();

    }
}
