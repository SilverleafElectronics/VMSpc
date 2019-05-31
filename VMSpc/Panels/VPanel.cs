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

        public VPanel(MainWindow mainWindow, PanelSettings panelSettings)
        {
            this.mainWindow = mainWindow;
            this.panelSettings = panelSettings;
            border = new Border();
            canvas = new VMSCanvas(mainWindow, border, panelSettings);
            border.Child = canvas;
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
            if ((panelLeft > leftLimit) && (panelLeft < Canvas.GetLeft(border)))
                leftLimit = panelLeft;
            if ((panelTop > topLimit) && (panelTop < Canvas.GetTop(border)))
                topLimit = panelTop;
            if ((panelRight < rightLimit) && (panelRight > Canvas.GetRight(border)))
                rightLimit = panelRight;
            if ((panelBottom < bottomLimit) && (panelBottom > Canvas.GetBottom(border)))
                bottomLimit = panelBottom;
            VMSConsole.PrintLine("Left Limit: " + leftLimit);
            VMSConsole.PrintLine("Top Limit: " + topLimit);
            VMSConsole.PrintLine("Right Limit: " + rightLimit);
            VMSConsole.PrintLine("Bottom Limit: " + bottomLimit);
        }

        public bool CanMove(int direction)
        {
            switch (direction)
            {
                case Constants.LEFT:
                    return (Canvas.GetLeft(border) > leftLimit + 1);
                case Constants.UP:
                    return (Canvas.GetTop(border) > topLimit + 1);
                case Constants.RIGHT:
                    return (Canvas.GetRight(border) < rightLimit + 1);
                case Constants.DOWN:
                    return (Canvas.GetBottom(border) < bottomLimit + 1);
                default:
                    return false;
            }
        }

        public abstract void GeneratePanel();

    }
}
