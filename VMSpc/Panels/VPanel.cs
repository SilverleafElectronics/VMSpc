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

namespace VMSpc.Panels
{
    public abstract class VPanel
    {
        public char cID;
        private MainWindow mainWindow;
        private PanelSettings panelSettings;
        public Border border;
        public VMSCanvas canvas;

        public VPanel(MainWindow mainWindow, PanelSettings panelSettings)
        {
            this.mainWindow = mainWindow;
            this.panelSettings = panelSettings;
            border = new Border();
            canvas = new VMSCanvas(mainWindow, border, panelSettings);
            border.Child = canvas;
        }

        public abstract void GeneratePanel();

    }
}
