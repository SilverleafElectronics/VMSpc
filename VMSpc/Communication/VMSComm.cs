using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using VMSpc.DevHelpers;
using System.Windows;
using System.Timers;

namespace VMSpc.Communication
{
    public class VMSComm
    {
        //class variables
        private SerialPort portreader;
        private uint messagesReceived;
        private Timer vTimer;

        public VMSComm()
        {
            messagesReceived = 0;
            portreader = new SerialPort("COM10", 9600, Parity.None, 8, StopBits.One);   //CHANGEME - port should be inferred at runtime. User should also be able to override
        }

        public void StartComm()
        {
            ReadData();
            KeepJibAlive();
            SetTimer();
        }

        private void ReadData()
        {
            portreader.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            // Begin communications 
            portreader.Open();
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ProcessData(portreader.ReadExisting());
            });
        }

        private void ProcessData(string message)
        {
            CanMessage canMessage = new CanMessage();
            switch (message[0])
            {
                case 'R':
                    ExtractJ1939(message, ref canMessage);
                    break;
                case 'I':
                    break;
                case 'J':
                    break;
            }
        }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the canMessage struct
        /// </summary>
        /// <param name="message"></param>
        /// <param name="canMessage"></param>
        private void ExtractJ1939(string message, ref CanMessage canMessage)
        {
            try
            {
                canMessage.address = Convert.ToByte(message.Substring(1, 2));
                canMessage.pgn = Convert.ToUInt32(message.Substring(3, 6));
                canMessage.data = new byte[8];
                string dataSection = message.Substring(9, 16);
                for (int i = 0; i < 8; i++)
                    canMessage.data[i] = Convert.ToByte(dataSection.Substring((i*2), 2));
            }
            catch
            {
                canMessage.address = Constants.RVC_BYTE(Constants.RVC_MAXVAL);
            }
        }

        private void ExtractJ1708(string message, out CanMessage canmessage)
        {
            canmessage.address = 0;
            canmessage.pgn = 0;
            canmessage.data = new byte[8];
            for (int i = 0; i < 8; i++)
                canmessage.data[i] = 0;
        }

        private void SetTimer()
        {
            vTimer = new Timer(18000);
            vTimer.Elapsed += OnTimedEvent;
            vTimer.AutoReset = true;
            vTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            KeepJibAlive();
        }

        private void KeepJibAlive()
        {
            portreader.Write("V");
        }
    }
}
