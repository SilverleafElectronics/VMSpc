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
        private enum MessageTestType
        {
            Raw,
            J1708,
            J1939
        }

        private MessageTestType MessageType = MessageTestType.Raw;
        private bool UseHex => (bool)UseHexCheckbox.IsChecked;
        private List<TextBox> J1939DataBoxes
        {
            get
            {
                return new List<TextBox>()
                {
                    DataByte0,
                    DataByte1,
                    DataByte2,
                    DataByte3,
                    DataByte4,
                    DataByte5,
                    DataByte6,
                    DataByte7,
                };
            }
        }

        private List<TextBox> J1708DataBoxes;

        public MessageTester()
        {
            InitializeComponent();
            EventBridge.Instance.AddEventPublisher(this);
            UseHexCheckbox.IsChecked = true;
            J1708DataBoxes = new List<TextBox>()
            {

            };
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        private void Reinitialize()
        {
            RawMessageBuilder.Visibility = Visibility.Hidden;
            J1708MessageBuilder.Visibility = Visibility.Hidden;
            J1939MessageBuilder.Visibility = Visibility.Hidden;
            Width = 400;
            Height = 250;
            switch (MessageType)
            {
                case MessageTestType.Raw:
                    RawMessageBuilder.Visibility = Visibility.Visible;
                    break;
                case MessageTestType.J1708:
                    J1708MessageBuilder.Visibility = Visibility.Visible;
                    break;
                case MessageTestType.J1939:
                    J1939MessageBuilder.Visibility = Visibility.Visible;
                    Width = 650;
                    Height = 400;
                    break;
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message;
            switch (MessageType)
            {
                case MessageTestType.J1708:
                    message = BuildJ1708Message();
                    break;
                case MessageTestType.J1939:
                    message = BuildJ1939Message();
                    break;
                case MessageTestType.Raw:
                default:
                    message = CurrentMessage.Text;
                    break;
            }
            if (!string.IsNullOrEmpty(message))
            {
                RaiseVMSEvent?.Invoke(this, new VMSCommDataEventArgs(message, DateTime.Now));
                MessageBox.Show($"Message sent as {message}");
            }
            else
            {
                MessageBox.Show($"Invalid message");
            }
        }

        private string BuildJ1708Message()
        {
            var message = new StringBuilder('J');
            return message.ToString();
        }

        private string BuildJ1939Message()
        {
            var message = new StringBuilder('R');

            return message.ToString();
        }

        private void Raw_Selected(object sender, RoutedEventArgs e)
        {
            MessageType = MessageTestType.Raw;
            Reinitialize();
        }

        private void J1708_Selected(object sender, RoutedEventArgs e)
        {
            MessageType = MessageTestType.J1708;
            Reinitialize();
        }

        private void J1939_Selected(object sender, RoutedEventArgs e)
        {
            MessageType = MessageTestType.J1939;
            Reinitialize();
        }

        private void UseHex_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var box in J1939DataBoxes)
            {
                try
                {
                    var byteVal = byte.Parse(box.Text);
                    box.Text = byteVal.ToString("X2");
                }
                catch { box.Text = 0xFF.ToString("X2"); }
            }
            try
            {
                var byteVal = byte.Parse(AddressBox.Text);
                AddressBox.Text = byteVal.ToString("X2");
            }
            catch { AddressBox.Text = 0xFF.ToString("X2"); }
            try
            {
                uint pgnVal = uint.Parse(PGNBox.Text);
                PGNBox.Text = pgnVal.ToString("X6");
            }
            catch { PGNBox.Text = 0xFFFFFF.ToString("X6"); }
        }

        private void UseHex_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var box in J1939DataBoxes)
            {
                try
                {
                    byte byteVal;
                    if (string.IsNullOrEmpty(box.Text))
                    {
                        continue;
                    }
                    else if (box.Text.Length == 1)
                    {
                        byteVal = Constants.BinConvert('0', box.Text[0]);
                    }
                    else if (box.Text.Length > 2)
                    {
                        box.Text = 0xFF.ToString();
                    }
                    else
                    {
                        byteVal = Constants.BinConvert(box.Text[0], box.Text[1]);
                        box.Text = byteVal.ToString();
                    }
                }
                catch { box.Text = 0xFF.ToString(); }
            }
            try
            {
                if (!string.IsNullOrEmpty(AddressBox.Text))
                {
                    byte byteVal = Constants.BinConvert(AddressBox.Text[0], AddressBox.Text[1]);
                    AddressBox.Text = byteVal.ToString();
                }
                else
                {
                    AddressBox.Text = 0xFF.ToString();
                }
            }
            catch { AddressBox.Text = 0xFF.ToString(); }
            try
            {
                byte[] bytes = new byte[3];
                Constants.BYTE_STRING_TO_BYTE_ARRAY(bytes, PGNBox.Text, 6);
                uint pgnVal = 0;
                pgnVal |= (uint)(bytes[0] << 16);
                pgnVal |= (uint)(bytes[1] << 8);
                pgnVal |= (uint)(bytes[2]);
                PGNBox.Text = pgnVal.ToString();
            }
            catch { PGNBox.Text = 0xFFFFFF.ToString(); }
        }

        private string GetPGN()
        {
            string pgn = string.Empty;
            
            return pgn;
        }

        private string GetByteString(TextBox box)
        {
            string byteVal = string.Empty;

            return byteVal;
        }
    }
}
