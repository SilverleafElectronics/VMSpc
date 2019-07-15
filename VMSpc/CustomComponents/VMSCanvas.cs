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
        private double BorderThickness;

        public VMSCanvas(MainWindow mainWindow, Border border, PanelSettings panelSettings) 
            : base()
        {
            this.mainWindow = mainWindow;
            this.border = border;
            this.panelSettings = panelSettings;
            BorderThickness = 5;
            ApplyBorderDimensions();
            ApplyCanvasDimensions();
        }

        ~VMSCanvas()
        {
        }

        public void ApplyBorderDimensions()
        {
            border.Width = panelSettings.rectCord.bottomRightX - panelSettings.rectCord.topLeftX;
            border.Height = panelSettings.rectCord.bottomRightY - panelSettings.rectCord.topLeftY;
            SetTop(border, panelSettings.rectCord.topLeftY);
            SetLeft(border, panelSettings.rectCord.topLeftX);
            SetRight(border, Canvas.GetLeft(border) + border.Width);
            SetBottom(border, Canvas.GetTop(border) + border.Height);
        }

        public void ApplyCanvasDimensions()
        {
            SetTop(this, GetTop(border) - BorderThickness);
            SetLeft(this, GetLeft(border) - BorderThickness);
            SetRight(this, GetRight(border) - BorderThickness);
            SetBottom(this, GetBottom(border) - BorderThickness);
            Width = border.Width - (BorderThickness * 2);
            Height = border.Height - (BorderThickness * 2);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }
    }
}
