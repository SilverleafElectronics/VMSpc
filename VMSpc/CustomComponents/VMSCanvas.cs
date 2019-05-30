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
    public class VMSCanvas : Canvas
    {
        private MainWindow mainWindow;
        private Border border;
        private PanelSettings panelSettings;
        public bool leftMoveAllowed;
        public bool rightMoveAllowed;
        public bool upMoveAllowed;
        public bool downMoveAllowed;

        public Double leftLimit;
        public Double rightLimit;
        public Double topLimit;
        public Double bottomLimit;


        public VMSCanvas(MainWindow mainWindow, Border border, PanelSettings panelSettings) 
            : base()
        {
            leftMoveAllowed = false;
            rightMoveAllowed = false;
            upMoveAllowed = false;
            downMoveAllowed = false;
            this.mainWindow = mainWindow;
            this.border = border;
            this.panelSettings = panelSettings;
            ApplyBorderDimensions();
        }

        private void ApplyBorderDimensions()
        {
            border.Width = panelSettings.rectCord.bottomRightX - panelSettings.rectCord.topLeftX;
            border.Height = panelSettings.rectCord.bottomRightY - panelSettings.rectCord.topLeftY;
            SetTop(border, panelSettings.rectCord.topLeftY);
            SetLeft(border, panelSettings.rectCord.topLeftX);
        }
    }
}
