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
using static VMSpc.Constants;
using VMSpc.XmlFileManagers;
using static VMSpc.XmlFileManagers.SettingsManager;
using System.Threading;

namespace VMSpc.Communication
{
    public class VMSComm
    {
        //Data readers
        private DataReader dataReader;

        public ulong messageCount;
        public ulong badMessageCount;
        public string BadMessageCount { get { return ("" + badMessageCount); } }
        public int parseBehavior;

        private Thread dataReaderThread;
        private MessageExtractor extractor;

        private J1708Parser j1708Parser;
        private J1939Parser j1939Parser;

        public string logDataQueue;
        public string LogRecordingFile;
        public bool LogRecordingEnabled;
        public byte LogType;


        public VMSComm()
        {
            messageCount = badMessageCount = 0;
            extractor = new MessageExtractor();

            LogRecordingEnabled = false;
            LogType = LOGTYPE_RAWLOG;
            LogRecordingFile = null; //CHANGEME - retrieve from config
            logDataQueue = "";
            parseBehavior = Settings.ParseMode;

            j1939Parser = new J1939Parser();
            j1708Parser = new J1708Parser();

            CreateDataReader();
        }

        ~VMSComm()
        {
            StopComm();
        }

        #region Common Logic

        private void CreateDataReader()
        {
            switch(Settings.JibType)
            {
                case SERIAL:
                    dataReader = new SerialPortReader(ProcessData);
                    break;
                case USB:
                    dataReader = new CommPortReader(ProcessData);
                    break;
                case WIFI:
                    dataReader = new WifiSocketReader(ProcessData);
                    break;
                case LOGPLAYER:
                    dataReader = new LogFileReader(ProcessData);
                    break;
                default:
                    break;
            }
        }

        /// <summary>   Initializes all communications activity. Begins the InitializeDataReader() thread and sends messages to the JIB to keep it in VMS mode  </summary>
        public void StartComm()
        {
            //dataReaderThread = new Thread(dataReader.InitDataReader);
            //dataReaderThread.Start();
            dataReader.InitDataReader();
        }

        public void StopComm()
        {
            if (NOT_NULL(dataReader))
            {
                dataReader.CloseDataReader();
                //dataReaderThread.Abort();
                dataReader = null;
            }
        }

        /// <summary>
        /// Receives the message from the data reader, gets the message as a CanMessage from the MessageExtractor, and passes the parsed message to the appropriate parser
        /// </summary>
        private void ProcessData(string message)
        {
            CanMessage canMessage = extractor.GetMessage(message);
            if (LogRecordingEnabled)
                AddLogRecord(message, canMessage);
            if (canMessage == null || canMessage.messageType == INVALID_CAN_MESSAGE)
            {
                badMessageCount++;
                return;
            }
            if (canMessage.messageType == J1939 && parseBehavior != IGNORE_1939)
                j1939Parser.Parse((J1939Message)canMessage);
            else if (canMessage.messageType == J1708 && parseBehavior != IGNORE_1708)
                j1708Parser.Parse((J1708Message)canMessage);
            messageCount++;
        }

        private void AddLogRecord(string message, CanMessage canMessage)
        {
            string logEntry = "" + message;
            if (LogType == LOGTYPE_PARSEREADY || LogType == LOGTYPE_FULL)
                logEntry += canMessage.ToString();
            if (LogType == LOGTYPE_FULL)
            {
                if (canMessage.messageType == J1939 && parseBehavior != IGNORE_1939)
                    logEntry += canMessage.ToParsedString(j1939Parser);
                else if (canMessage.messageType == J1708 && parseBehavior != IGNORE_1708)
                    logEntry += canMessage.ToParsedString(j1708Parser);
            }
            logEntry += "\nEnd of Message\n\n";
            logDataQueue += logEntry;
            if (logDataQueue.Length >= 2000)
            {
                string outputStream = String.Copy(logDataQueue);
                logDataQueue = "";
                using (StreamWriter logWriter = new StreamWriter(LogRecordingFile, true))
                    logWriter.WriteLine(outputStream);
            }
        }

        #endregion //Common Logic


    }
}
