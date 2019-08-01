using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using static VMSpc.Constants;

namespace VMSpc.Communication
{
    class USBPortReader : DataReader
    {
        private SerialPort portReader;
        private string portString;
        private Timer portCheckTimer;

        public USBPortReader(Action<string> DataProcessor)
            : base(DataProcessor)
        {
        }
        public override void InitDataReader()
        {
            portReader = new SerialPort(portString, 9600, Parity.None, 8, StopBits.One);
            portReader.DataReceived += new SerialDataReceivedEventHandler(HandleCommPortData);
            portReader.Open(); // Begin communications 
            portCheckTimer = portCheckTimer = CREATE_TIMER(CheckPort, 5000);
        }

        public override void CloseDataReader()
        {
            throw new NotImplementedException();
        }

        private void HandleCommPortData(object sender, SerialDataReceivedEventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    //break the buffer into an array of messages and process them individually.
                    //each valid, individual message in the buffer ends in a newline character
                    try
                    {
                        string buffer = portReader.ReadExisting();
                        foreach (string message in buffer.Split('\n'))
                            DataProcessor(message);
                    }
                    catch
                    {
                    }
                });
            }
            else
            {
                CloseDataReader();
            }
        }
        protected override void KeepJibAwake(object source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckPort(object source, ElapsedEventArgs e)
        {

        }
    }
}
