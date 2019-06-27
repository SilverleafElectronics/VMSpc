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
using System.Windows.Shapes;
using VMSpc.Communication;
using static VMSpc.Constants;
using VMSpc.DevHelpers;
using VMSpc.Panels;

namespace VMSpc.DlgWindows
{
    public class VMSDialog : Window
    {
        protected VPanel panel;

        protected virtual void ApplyBindings()
        {
            DataContext = this;
        }

        public bool? ShowDialog(VPanel panel)
        {
            this.panel = panel;
            return ShowDialog();
        }

        protected void CreateBinding(string sourcePropName, object sourceObject, BindingMode mode, FrameworkElement UIElement, DependencyProperty UIElementProp)
        {
            Binding newBind = new Binding(sourcePropName)
            {
                Mode = mode,
                Source = sourceObject
            };
            UIElement.SetBinding(UIElementProp, newBind);
        }

        protected void CreateBinding(string sourcePropName, object sourceObject, BindingMode mode, FrameworkElement UIElement, DependencyProperty UIElementProp, int ConditionalValue)
        {
            Binding newBind = new Binding(sourcePropName)
            {
                Mode = mode,
                Source = sourceObject
            };
            UIElement.SetBinding(UIElementProp, newBind);
        }
    }
}
