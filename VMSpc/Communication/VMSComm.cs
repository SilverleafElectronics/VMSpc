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

namespace VMSpc.Communication
{
    public class VMSComm
    {
        //Data readers
        private SerialPort portReader;
        private StreamReader logReader;

        private ulong messageCount;
        private ulong lastMessageCount;
        private ulong badMessageCount;

        private Timer portCheckTimer;
        private Timer logReadTimer;
        private Timer keepJibAwakeTimer;


        public VMSComm()
        {
            messageCount = 0;
            lastMessageCount = 0;
            badMessageCount = 0;
            portReader = new SerialPort("COM10", 9600, Parity.None, 8, StopBits.One);   //CHANGEME - port should be inferred at runtime. User should also be able to override
        }

        ~VMSComm()
        {
            if (portReader != null && portReader.IsOpen)
                portReader.Close();
            if (logReader != null)
                logReader.Close();
        }

        /// <summary>
        /// Initializes all communications activity. Begins the ReadData() thread and sends messages to the JIB to keep it in VMS mode
        /// </summary>
        public void StartComm()
        {
            //InitPortReader();
            InitLogReader();
            //KeepJibAwake(null, null);
            Constants.CreateTimer(portCheckTimer, CheckPort, 5000);
            //Constants.CreateTimer(keepJibAwakeTimer, KeepJibAwake, 10000);
        }

        /// <summary>
        /// Creates a new event handler and attaches it to the portReader. Calling this method sets a trigger on HandleCommPortData whenever data is received
        /// </summary>
        private void InitPortReader()
        {
            portReader.DataReceived += new SerialDataReceivedEventHandler(HandleCommPortData);
            portReader.Open(); // Begin communications 
        }

        /// <summary>
        /// Automatically called any time data comes through the USB Port. Passes the received message to ProcessData()
        /// </summary>
        private void HandleCommPortData(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ProcessData(portReader.ReadExisting());
            });
        }

        /// <summary>
        /// Opens the log file to be read by the program. Sets a timer to call ReadLogEntry() every 100ms
        /// </summary>
        private void InitLogReader()
        {
            logReader = new StreamReader("J1939log.vms");
            Constants.CreateTimer(logReadTimer, ReadLogEntry, 100);

        }

        /// <summary>
        /// Called every 100ms when using the log reader. Reads the next line of the log file and passes the message to ProcessData(). Returns to the beginning of the file once reaching the end
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

        /// <summary>
        /// Receives the message from the data reader, creates a new CanMessage, and unboxes the values into the CanMessage struct
        /// </summary>
        /// <param name="message"></param>
        private void ProcessData(string message)
        {
            VMSConsole.PrintLine("Message: " + message);
            CanMessage canMessage = new CanMessage();
            bool ExtractionResult = canMessage.FromString(message);
            if (!ExtractionResult)
            {
                badMessageCount++;
                return;
            }
            messageCount++;
        }

        #region Event Timer Callbacks

        /// <summary>
        /// sends a "V" message to the JIB to keep it in VMS mode
        /// </summary>
        private void KeepJibAwake(Object source, ElapsedEventArgs e)
        {
            portReader.Write("V");
        }

        /// <summary>
        /// Checks whether or not data is being received. If no data is being received, creates a new CommChecker object to automatically find the right port
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
