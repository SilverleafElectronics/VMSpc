using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.UI.CustomComponents;
using VMSpc.UI.GaugeComponents;
using System.Windows;

namespace VMSpc.UI.ComponentWrappers
{
    /// <summary>
    /// A class for bundling IGaugeComponent elements, allowing for grouped operations on all child elements
    /// </summary>
    public class ComponentCanvas : Canvas
    {
        /// <summary>
        /// Hides this control as well as all child controls of type IGaugeComponent. Deactivates all child
        /// IGaugeComponents 
        /// </summary>
        public void Hide()
        {
            foreach (var child in Children)
            {
                if (typeof(IGaugeComponent).IsInstanceOfType(child))
                {
                    (child as IGaugeComponent).Disable();
                }
                if (typeof(VMSCanvas).IsInstanceOfType(child))
                {
                    (child as VMSCanvas).Visibility = Visibility.Hidden;
                }
            }
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows this control and activates all child IGaugeComponents
        /// </summary>
        public void Show()
        {
            foreach (var child in Children)
            {
                if (typeof(IGaugeComponent).IsInstanceOfType(child))
                {
                    (child as IGaugeComponent).Enable();
                }
                if (typeof(VMSCanvas).IsInstanceOfType(child) && typeof(VMSCanvas).IsInstanceOfType(child))
                {
                    (child as VMSCanvas).Visibility = Visibility.Visible;
                }
            }
            Visibility = Visibility.Visible;
        }
    }
}
