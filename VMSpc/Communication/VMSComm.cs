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
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.Enums.Parsing;
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

        public string logDataQueue;
        public string LogRecordingFile
        {
            get
            {
                return ConfigManager.Settings.Contents.rawLogFilePath;
            }
            set
            {
                ConfigManager.Settings.Contents.rawLogFilePath = value;
            }
        }
        public bool LogRecordingEnabled;
        public byte LogType;


        public VMSComm()
        {
            messageCount = badMessageCount = 0;

            LogRecordingEnabled = false;
            LogType = LOGTYPE_RAWLOG;
            logDataQueue = "";

            ValidateDataReader();
            CreateDataReader();
        }

        /// <summary>
        /// Sends full messages to the JIB that abide the message protocols for the specified DataBus message type
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(OutgoingMessage message)
        {
            dataReader.SendMessage(message);
        }

        /// <summary>
        /// Sends simple string messages to the JIB
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            dataReader.SendMessage(message);
        }

        ~VMSComm()
        {
            StopComm();
        }

        #region Common Logic

        private void ValidateDataReader()
        {

        }

        private void CreateDataReader()
        {
            switch(ConfigManager.Settings.Contents.jibType)
            {
                case JibType.SERIAL:
                    dataReader = new SerialPortReader();
                    break;
                case JibType.USB:
                    dataReader = new CommPortReader(ConfigManager.Settings.Contents.comPort);
                    break;
                case JibType.WIFI:
                    dataReader = new WifiSocketReader();
                    break;
                case JibType.LOGPLAYER:
                    dataReader = new LogFileReader(ConfigManager.Settings.Contents.jibPlayerFilePath);
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
            dataReader?.InitDataReader();
        }

        public void StopComm()
        {
            if (dataReader != null)
            {
                dataReader.CloseDataReader();
                //dataReaderThread.Abort();
                dataReader = null;
            }
        }

        #endregion //Common Logic


    }
}
