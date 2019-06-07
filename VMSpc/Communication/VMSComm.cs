using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using VMSpc.DevHelpers;
using System.Windows;
using System.Timers;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using VMSpc.Parsers;

namespace VMSpc.Communication
{
    public class VMSComm
    {
        //Data readers
        private SerialPort portReader;
        private Socket wifiReader;
        private StreamReader logReader;

        private ulong messageCount;
        private ulong lastMessageCount;
        private ulong badMessageCount;

        private Timer portCheckTimer;
        private Timer logReadTimer;
        private Timer keepJibAwakeTimer;

        private Action InitializeDataReader;
        private Dictionary<int, Action> dataReaderMap;
        private int dataReaderType;

        private MessageExtractor extractor;

        private J1708Parser j1708Parser;
        private J1939Parser j1939Parser;

        private string COMMPort;
        private string LogFile;


        public VMSComm()
        {
            messageCount = 0;
            lastMessageCount = 0;
            badMessageCount = 0;
            extractor = new MessageExtractor();

            dataReaderType = Constants.USB;
            dataReaderMap = new Dictionary<int, Action>();
            dataReaderMap.Add(Constants.USB, InitPortReader);
            dataReaderMap.Add(Constants.SERIAL, InitPortReader);
            dataReaderMap.Add(Constants.WIFI, null);
            dataReaderMap.Add(Constants.LOGPLAYER, InitLogReader);

            COMMPort = "COM10"; //CHANGEME - port should be inferred at runtime. User should also be able to override
            LogFile = "J1939log.vms";   //CHANGEME - should rely on user input

            //j1939Parser = new J1939Parser();
            j1708Parser = new J1708Parser();

            SetDataReader();
        }

        ~VMSComm()
        {
            CloseDataReader();
        }

        /// <summary>   Initializes all communications activity. Begins the InitializeDataReader() thread and sends messages to the JIB to keep it in VMS mode  </summary>
        public void StartComm()
        {
            try
            {
                InitializeDataReader();
                //InitLogReader();
                KeepJibAwake(null, null);
                keepJibAwakeTimer = Constants.CreateTimer(KeepJibAwake, 10000);
            }
            catch { } // CHANGEME - put something useful here
        }

        /// <summary>
        /// Receives the message from the data reader, gets the message as a CanMessage from the MessageExtractor, and passes the parsed message to the appropriate parser
        /// </summary>
        private void ProcessData(string message)
        {
            CanMessage canMessage = extractor.GetMessage(message);
            if (canMessage == null || canMessage.messageType == Constants.INVALID_CAN_MESSAGE)
            {
                badMessageCount++;
                return;
            }
            if (canMessage.messageType == Constants.J1939)
                j1939Parser.Parse(canMessage);
            else if (canMessage.messageType == Constants.J1708)
                j1708Parser.Parse((J1708Message)canMessage);
            messageCount++;
        }

        #region Communication Settings

        /// <summary>   Closes any existing connections and sets portReader, logReader, and wifiReader to null  </summary>
        private void CloseDataReader()
        {
            if (portReader != null && portReader.IsOpen)
                portReader.Close();
            if (logReader != null)
                logReader.Close();
            if (wifiReader != null)
            {
                wifiReader.Shutdown(SocketShutdown.Both);
                wifiReader.Close();
            }
            portReader = null;
            logReader = null;
            wifiReader = null;
        }

        /// <summary>   Sets the data reader to either InitPortReader(), InitWifiReader(), or InitLogReader(), depending on the current dataReaderType  </summary>
        private void SetDataReader()
        {
            InitializeDataReader = dataReaderMap[dataReaderType];
        }

        /// <summary>   Changes the I/O channel to either Wifi, RS-232, USB, or Logplayer   </summary>
        public void ChangeDataReader(int newType)
        {
            if (newType == dataReaderType)
                return;
            dataReaderType = newType;
            SetDataReader();
            StartComm();
        }

        #endregion //Communication Settings

        #region WIFI Reader


        private void InitWifiReader()
        {
            wifiReader = new Socket(SocketType.Stream, ProtocolType.Tcp);
            // CHANGEME - finish
        }

        #endregion //WIFI Reader

        #region Port Reader
        /// <summary>
        /// Creates a new event handler and attaches it to the portReader. Calling this method sets a trigger on HandleCommPortData whenever data is received
        /// </summary>
        private void InitPortReader()
        {
            portReader = new SerialPort(COMMPort, 9600, Parity.None, 8, StopBits.One);   
            portReader.DataReceived += new SerialDataReceivedEventHandler(HandleCommPortData);
            portReader.Open(); // Begin communications 
            portCheckTimer = Constants.CreateTimer(CheckPort, 5000);
        }

        /// <summary>
        /// Automatically called any time data comes through the USB Port. Splits all messages in the buffer and send them to ProcessData
        /// </summary>
        private void HandleCommPortData(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    //break all of the buffer into an array of messages and process them individually
                    //each valid, individual message in the buffer ends in a newline character
                    string buffer = portReader.ReadExisting();
                    foreach (string message in buffer.Split('\n'))
                        ProcessData(message);
                });
            }
            catch { }
        }
        #endregion //Port Reader

        #region Log Reader
        /// <summary>
        /// Opens the log file to be read by the program. Sets a timer to call ReadLogEntry() every 100ms
        /// </summary>
        private void InitLogReader()
        {
            logReader = new StreamReader(LogFile);
            logReadTimer = Constants.CreateTimer(ReadLogEntry, 100);

        }

        /// <summary>
        /// Called every 100ms when using the log reader. Reads the next line of the log file and passes the message to ProcessData(). 
        /// Returns to the beginning of the file once reaching the end
        /// </summary>
        private void ReadLogEntry(Object source, ElapsedEventArgs e)
        {
            string line = " ";
            while (line != null && line.Length < 2)
                line = logReader.ReadLine();
            if (line != null && (line[0] == 'J' || line[0] == 'R'))
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ProcessData(line);
                });
            else if (line != null)
                ReadLogEntry(null, null);
            else
            {
                logReader.DiscardBufferedData();
                logReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
        }

        #endregion //Log Reader

        #region Event Timer Callbacks

        /// <summary>
        /// sends a "V" message to the JIB to keep it in VMS mode
        /// </summary>
        private void KeepJibAwake(Object source, ElapsedEventArgs e)
        {
            portReader.Write("V");
        }

        /// <summary>
        /// Checks whether or not data is being received. If no data is being received, creates a new CommChecker object to automatically find the right port.
        /// Resets the port if it can find the correct one.
        /// </summary>
        private void CheckPort(Object source, ElapsedEventArgs e)
        {
            if (lastMessageCount != messageCount)
            {
                lastMessageCount = messageCount;
                return;
            }

        }
        #endregion //Event Timer Callbacks
    }
}
