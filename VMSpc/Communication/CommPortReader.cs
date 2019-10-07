﻿using System;
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

namespace VMSpc.Communication
{
    class CommPortReader : DataReader
    {
        private SerialPort portReader;
        private int comPort;
        private System.Timers.Timer portCheckTimer;
        private SerialDataReceivedEventHandler dataReceivedHandler;
        private delegate void SplitDataIntoProcess();

        int quietlyTryReconnect;
        System.Timers.Timer quietReconnectTimer;

        public CommPortReader(Action<string> DataProcessor)
            : base(DataProcessor)
        {
            comPort = Settings.Port;
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
            if (NOT_NULL(portCheckTimer))
                portCheckTimer.Stop();
            try
            {
                portReader.Open(); // Begin communications 
                portCheckTimer = CREATE_TIMER(CheckPort, 5000);
                base.InitDataReader();
                quietReconnectTimer.Stop();
                quietlyTryReconnect = 0;
                if (!keepJibAwakeTimer.Enabled)
                    keepJibAwakeTimer = CREATE_TIMER(KeepJibAwake, 10000);
            }
            catch (Exception ex)
            {
                int newPort = FindVmsPort();
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
                if (NOT_NULL(keepJibAwakeTimer))
                    keepJibAwakeTimer.Dispose();
                if (NOT_NULL(portCheckTimer))
                    portCheckTimer.Dispose();
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
        public override bool SendMsg()
        {
            throw new NotImplementedException();
        }
        protected override void KeepJibAwake()
        {
            if (portReader.IsOpen)
                portReader.Write("V");
        }

        private void CheckPort()
        {
            if (lastMessageCount == messagesReceived)
            {
                int newPort = FindVmsPort();
                if (newPort != comPort)
                    ChangeComPort(newPort);
                lastMessageCount = messagesReceived;
            }
        }

        /// <summary>  </summary>
        private int FindVmsPort()
        {
            int newPort = comPort;
            foreach (var portString in GetComPortList())
            {
                if (portString.Contains("VMSpc Virtual Serial Port") && portString.Contains("(COM"))
                {
                    int startIndex = portString.IndexOf("(COM") + 4;
                    int count = portString.LastIndexOf(")") - startIndex;
                    return Int32.Parse(portString.Substring(startIndex, count));
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

        private void ChangeComPort(int newPort)
        {
            comPort = newPort;
            Settings.Port = newPort;
            CloseDataReader();
            InitDataReader();
        }
    }
}