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
        private Border border;
        private Canvas canvas;

        public VPanel(MainWindow parent, PanelSettings panelSettings)
        {
            cID = Constants.PanelIDs.SIMPLE_GAUGE_ID;
            this.panelSettings = panelSettings;
            this.parent = parent;
            border = new Border();
            canvas = new VMSCanvas(border, panelSettings);
            border.Child = canvas;
            cID = panelSettings.ID;
        }


        public void Show()
        {
            parent.PanelGrid.Children.Add(border);
            //GeneratePanel();
        }

        protected abstract void GeneratePanel();

    }
}
