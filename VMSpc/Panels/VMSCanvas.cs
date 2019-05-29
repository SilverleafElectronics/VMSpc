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
    public class VMSCanvas : Canvas
    {
        private Border ParentComponent;
        private bool MouseHeld;


        public VMSCanvas(Border parentComponent) : base()
        {
            ParentComponent = parentComponent;
            MouseHeld = false;
        }
        //static VMSCanvas()
        //{ }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetBottom(this, (GetBottom(this) + 1));
            ParentComponent.Height++;
            MouseHeld = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!MouseHeld)
                return;
            Point cursorPosition = e.GetPosition(this);
            
        }
    }
}
