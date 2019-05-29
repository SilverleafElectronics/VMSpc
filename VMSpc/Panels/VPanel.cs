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

namespace VMSpc.Panels
{
    abstract class VPanel
    {
        public char cID;
        private PanelSettings panelSettings;
        private MainWindow parent;
        private System.Windows.Controls.Border border;
        private Canvas canvas;

        public VPanel(MainWindow parent, PanelSettings panelSettings)
        {
            cID = Constants.PanelIDs.SIMPLE_GAUGE_ID;
            this.panelSettings = panelSettings;
            this.parent = parent;
            border = new System.Windows.Controls.Border();
            ApplyBorderDimensions();
            canvas = new VMSCanvas(border);
            ApplyCanvasDimensions();
            border.Child = canvas;
            cID = panelSettings.ID;
        }


        public void Show()
        {
            parent.PanelGrid.Children.Add(border);
            //GeneratePanel();
        }

        private void ApplyBorderDimensions()
        {
            border.Width = panelSettings.rectCord.bottomRightX - panelSettings.rectCord.topLeftX;
            border.Height = panelSettings.rectCord.bottomRightY - panelSettings.rectCord.topLeftY;
        }

        private void ApplyCanvasDimensions()
        {
            //Canvas.SetTop(canvas, panelSettings.rectCord.topLeftY);
            //Canvas.SetLeft(canvas, panelSettings.rectCord.topLeftX);
            canvas.SetValue(Canvas.LeftProperty, (Double)panelSettings.rectCord.topLeftX);
            canvas.SetValue(Canvas.TopProperty, (Double)panelSettings.rectCord.topLeftY);
        }

        protected abstract void GeneratePanel();

    }
}
