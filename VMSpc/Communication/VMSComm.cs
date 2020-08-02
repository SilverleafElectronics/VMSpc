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
using VMSpc.Common;

namespace VMSpc.Communication
{
    public class CommunicationManager : ISingleton
    {
        //Data readers
        private DataReader dataReader;
        public static CommunicationManager Instance { get; private set; }

        static CommunicationManager() { }

        public CommunicationManager()
        {
            CreateDataReader();
            StartComm();
        }

        public static void Initialize()
        {
            Instance = new CommunicationManager();
        }

        /// <summary>
        /// Sends full messages to the JIB that abide the message protocols for the specified DataBus message type
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(OutgoingMessage message)
        {
            dataReader?.SendMessage(message);
        }

        /// <summary>
        /// Sends a message after the specified millisecondDelay. Note that there is no guarantee on the millisecond precision, which depends on the
        /// implementation of the datareader's sending mechanism.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="millisecondDelay"></param>
        public void SendDelayedMessage(OutgoingMessage message, int millisecondDelay)
        {
            var timer = Constants.CREATE_TIMER(() => SendMessage(message), millisecondDelay, Enums.UI.DispatchType.OnMainThread);
            timer.AutoReset = false;
            timer.Start();
        }

        /// <summary>
        /// Sends simple string messages to the JIB
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            dataReader?.SendMessage(message);
        }

        /// <summary>
        /// Sends a message after the specified millisecondDelay. Note that there is no guarantee on the millisecond precision, which depends on the
        /// implementation of the datareader's sending mechanism.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="millisecondDelay"></param>
        public void SendDelayedMessage(string message, int millisecondDelay)
        {
            var timer = Constants.CREATE_TIMER(() => SendMessage(message), millisecondDelay, Enums.UI.DispatchType.OnMainThread);
            timer.AutoReset = false;
            timer.Start();
        }

        ~CommunicationManager()
        {
            StopComm();
        }

        private void ValidateDataReader()
        {

        }

        private void CreateDataReader()
        {
            switch(ConfigManager.Settings.Contents.jibType)
            {
                case JibType.SERIAL:
                    dataReader = new RS232Reader(ConfigManager.Settings.Contents.comPort);
                    break;
                case JibType.USB:
                    dataReader = new USBReader(ConfigManager.Settings.Contents.comPort);
                    break;
                case JibType.WIFI:
                    dataReader = new WifiSocketReader(ConfigManager.Settings.Contents.ipAddress, ConfigManager.Settings.Contents.ipPort);
                    break;
                case JibType.LOGPLAYER:
                    dataReader = new LogFileReader(ConfigManager.Settings.Contents.jibPlayerFilePath);
                    break;
                default:
                    break;
            }
        }

        public void ChangeDataReader(JibType jibType)
        {
            if (jibType != ConfigManager.Settings.Contents.jibType)
            {
                ConfigManager.Settings.Contents.jibType = jibType;
                StopComm();
                CreateDataReader();
                StartComm();
            }
        }

        /// <summary>   Initializes all communications activity. Begins the InitializeDataReader() thread and sends messages to the JIB to keep it in VMS mode  </summary>
        public void StartComm()
        {
            //TODO - maybe not necessary to spin up a new thread for this, since all data 
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

        public void RestartComm()
        {
            StopComm();
            CreateDataReader();
            StartComm();
        }
    }
}
