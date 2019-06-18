using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using static VMSpc.Constants;

namespace VMSpc.DlgWindows
{
    public class VMSDialog : Window
    {
        Timer bindingTimer;
        public VMSDialog()
        {
            bindingTimer = CREATE_TIMER(ApplyDataBindings, 5000);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ApplyDataBindings(Object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                BindData();
            });
        }

        protected virtual void BindData() { }
    }
}
