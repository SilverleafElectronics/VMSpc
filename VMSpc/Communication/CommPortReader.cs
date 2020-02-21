using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using VMSpc.DevHelpers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.SettingsManager;
using System.Collections.Concurrent;

namespace VMSpc.Communication
{
    class CommPortReader : DataReader
    {
        private SerialPort portReader;
        private ushort comPort;
        private System.Timers.Timer portCheckTimer;
        private System.Timers.Timer sendMessageTimer;
        private SerialDataReceivedEventHandler dataReceivedHandler;
        private ConcurrentQueue<string> messagesToSend;
        private delegate void SplitDataIntoProcess();

        int quietlyTryReconnect;
        System.Timers.Timer quietReconnectTimer;

        public CommPortReader(Action<string> DataProcessor, ushort comPort)
            : base(DataProcessor)
        {
            this.comPort = comPort;
            messagesToSend = new ConcurrentQueue<string>();
            dataReceivedHandler = new SerialDataReceivedEventHandler(HandleCommPortData);
            quietlyTryReconnect = 0;
            quietReconnectTimer = CREATE_TIMER(TryReconnect, 3000);
            quietReconnectTimer.Stop();
        }

        private void TryReconnect()
        {
            InitDataReader();
        }

        public override void InitDataReader()
        {
            portReader = new SerialPort("COM" + comPort, 9600, Parity.None, 8, StopBits.One);
            portReader.DataReceived += dataReceivedHandler;
            portCheckTimer?.Stop();
            try
            {
                portReader.Open(); // Begin communications 
                portCheckTimer = CREATE_TIMER(CheckPort, 5000);
                sendMessageTimer = CREATE_TIMER(PopMessageQueue, 250);
                base.InitDataReader();
                quietReconnectTimer.Stop();
                quietlyTryReconnect = 0;
                base.InitDataReader();
            }
            catch (Exception ex)
            {
                ushort newPort = FindVmsPort();
                if (newPort == comPort && quietlyTryReconnect == 1)
                {
                    MessageBox.Show("Something went wrong in attempting to connect to the data source, using a USB connection on COM" + comPort + ". " +
                        "VMSpc will continue trying to reconnect, but if another application is using the COM port, the connection will continue to fail. Please make " +
                        "sure that all other instances of VMSpc are currently shutdown, and call our support " +
                        "team if the problem persists.");
                    quietlyTryReconnect++;
                }
                else if (newPort == comPort)
                {
                    quietlyTryReconnect++;
                    quietReconnectTimer.Start();
                }
                else
                    ChangeComPort(newPort);
            }
        }

        private void HandleCommPortData(object sender, SerialDataReceivedEventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new SplitDataIntoProcess(() =>
                {
                    //break the buffer into an array of messages and process them individually.
                    //each valid, individual message in the buffer ends in a newline character
                    try
                    {
                        string buffer = portReader.ReadExisting();
                        foreach (string message in buffer.Split('\n'))
                        {
                            messagesReceived++;
                            DataProcessor(message);
                        }
                    }
                    catch
                    {
                    }
                }));
                
            }
            else
            {
                CloseDataReader();
            }
        }
        public override void CloseDataReader()
        {
            try
            {
                keepJibAwakeTimer?.Dispose();
                portCheckTimer?.Dispose();
                sendMessageTimer?.Dispose();
                if (NOT_NULL(portReader))
                {
                    portReader.DataReceived -= dataReceivedHandler;
                    Thread DropPortThread = new Thread(portReader.Close);
                    DropPortThread.Start();
                    portReader = null;
                }
            }
            catch { } //do something useful here
        }
        public override void SendMessage(string message)
        {
            messagesToSend.Enqueue(message);
        }

        /// <summary>
        /// Removes a message from messagesToSend and transmits the message through WriteToPort()
        /// </summary>
        private void PopMessageQueue()
        {
            messagesToSend.TryDequeue(out string message);
            WriteToPort(message);
        }

        /// <summary>
        /// Writes a message to the port
        /// </summary>
        private void WriteToPort(string message)
        {
            if (portReader.IsOpen)
                portReader.Write(message);
        }

        private void CheckPort()
        {
            if (lastMessageCount == messagesReceived)
            {
                ushort newPort = FindVmsPort();
                if (newPort != comPort)
                    ChangeComPort(newPort);
                lastMessageCount = messagesReceived;
            }
        }

        private ushort FindVmsPort()
        {
            ushort newPort = comPort;
            foreach (var portString in GetComPortList())
            {
                if (portString.Contains("VMSpc Virtual Serial Port") && portString.Contains("(COM"))
                {
                    int startIndex = portString.IndexOf("(COM") + 4;
                    int count = portString.LastIndexOf(")") - startIndex;
                    return ushort.Parse(portString.Substring(startIndex, count));
                }
            }
            return comPort; //failed, just defaulting back to original
        }

        private List<string> GetComPortList()
        {
            //see https://stackoverflow.com/questions/9370859/get-friendly-port-name-programmatically
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                return (portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList());
            }
        }

        private void ChangeComPort(ushort newPort)
        {
            comPort = newPort;
            Settings.Port = newPort;
            CloseDataReader();
            InitDataReader();
        }
    }
}
