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
using System.Collections.Concurrent;
using VMSpc.JsonFileManagers;
using VMSpc.Enums.Parsing;
using VMSpc.Extensions.Standard;
using VMSpc.Exceptions;
using VMSpc.Loggers;

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
        private readonly char[] J1708HexToAsciiMap = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        int quietlyTryReconnect;
        System.Timers.Timer quietReconnectTimer;

        public CommPortReader(ushort comPort)
            : base()
        {
            this.comPort = comPort;
            messagesToSend = new ConcurrentQueue<string>();
            dataReceivedHandler = new SerialDataReceivedEventHandler(HandleCommPortData);
            quietlyTryReconnect = 0;
            quietReconnectTimer = CREATE_TIMER(TryReconnect, 10000);
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
                    quietlyTryReconnect++;
                    MessageBox.Show("Something went wrong in attempting to connect to the data source, using a USB connection on COM" + comPort + ". " +
                        "VMSpc will continue trying to reconnect, but if another application is using the COM port, the connection will continue to fail. Please make " +
                        "sure that all other instances of VMSpc are currently shutdown, and call our support " +
                        "team if the problem persists.");
                }
                else if (newPort == comPort)
                {
                    quietlyTryReconnect++;
                    quietReconnectTimer.Start();
                }
                else
                    ChangeComPort(newPort);
                ErrorLogger.GenerateErrorRecord(ex);
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
                            OnDataReceived(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.GenerateErrorRecord(ex);
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
            catch (Exception ex)
            {
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }
        public override void SendMessage(OutgoingMessage message)
        {
            switch (message.DataBusType)
            {
                case DataBusType.J1708:
                    var j1708message = (message as OutgoingJ1708Message);
                    j1708message.GenerateChecksum();
                    j1708message.CheckSum = (byte)(0x100 - j1708message.CheckSum);
                    var messageArr = j1708message.ToByteArray();
                    StringBuilder messageBuilder = new StringBuilder("S");
                    for (int i = 0; i < messageArr.Length; i++)
                    {
                        messageBuilder.Append(J1708HexToAsciiMap[(byte)(messageArr[i] / 0x10)]);
                        messageBuilder.Append(J1708HexToAsciiMap[(byte)(messageArr[i] / 0x0F)]);
                    }
                    messageBuilder.Append((char)0x0D);
                    messageBuilder.Append((char)0x0A);
                    SendMessage(messageBuilder.ToString());
                    break;
                case DataBusType.J1939:
                    SendMessage(message.ToString(true) + "\n\r");
                    break;
            }
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
            if (message != null)
            {
                WriteToPort(message);
            }
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
            var Settings = ConfigurationManager.ConfigManager.Settings.Contents;
            comPort = newPort;
            Settings.comPort = newPort;
            CloseDataReader();
            InitDataReader();
        }
    }
}
