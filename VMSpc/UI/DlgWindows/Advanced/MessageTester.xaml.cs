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
using VMSpc.Common;

namespace VMSpc.UI.DlgWindows.Advanced
{
    /// <summary>
    /// Interaction logic for MessageTester.xaml
    /// </summary>
    public partial class MessageTester : VMSDialog, IEventPublisher
    {
        public MessageTester()
        {
            InitializeComponent();
            EventBridge.Instance.AddEventPublisher(this);
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var message = CurrentMessage.Text;
            if (!string.IsNullOrEmpty(message))
            {
                RaiseVMSEvent?.Invoke(this, new VMSCommDataEventArgs(message, DateTime.Now));
            }
        }
    }
}
