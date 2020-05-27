using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using VMSpc.DevHelpers;
using VMSpc.Exceptions;
using VMSpc.JsonFileManagers;
using VMSpc.Loggers;
using static VMSpc.Constants;

namespace VMSpc.Communication
{
    class LogFileReader : DataReader
    {
        private Timer logReadTimer;
        private StreamReader logReader;
        private string LogPlayerFilePath;
        private string[] RecordBuffer;
        private int RecordIndex;
        private bool Reloading;

        public LogFileReader(string logPlayerFilePath)
            : base()
        {
            LogPlayerFilePath = logPlayerFilePath;
            RecordBuffer = new string[100];
            RecordIndex = 1000;
            Reloading = false;
        }
        public override void InitDataReader()
        {
            logReader = new FileOpener(LogPlayerFilePath).GetStreamReader();
            logReadTimer = CREATE_TIMER(ReadLogEntry, 100);
        }

        /// <summary>
        /// Called every 100ms when using the log reader. Reads the next line of the log file and passes the message to ProcessData(). 
        /// Returns to the beginning of the file once reaching the end
        /// </summary>
        private void ReadLogEntry()
        {
            if (!Reloading && logReader != null) //In case reloading the buffer takes longer than the timer, we don't want thread contention over the RecordBuffer
            {
                if (RecordIndex > 99)
                {
                    ReloadBuffer();
                }
                Application.Current.Dispatcher.Invoke(delegate
                {
                    OnDataReceived(RecordBuffer[RecordIndex]);
                });
                RecordIndex++;
            }
        }

        public override void CloseDataReader()
        {
            try
            {
                logReader.Close();
                logReader = null;
                logReadTimer.Stop();
            }
            catch (Exception ex)
            {
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }

        public override void SendMessage(OutgoingMessage message)
        {
        }
        public override void SendMessage(string message)
        {
        }



        private void ReloadBuffer()
        {
            Reloading = true;
            for (int i = 0; i < 100; i++)
            {
                RecordBuffer[i] = GetNextValidLine();
            }
            RecordIndex = 0;
            Reloading = false;
        }
        private string GetNextValidLine()
        {
            string line;
            bool found = false;
            while (!found)
            {
                line = logReader.ReadLine();
                if (line == null)
                {
                    logReader.DiscardBufferedData();
                    logReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    return GetNextValidLine();
                }
                else if ( (line.Length > 2) && (line[0] == 'J' || line[0] == 'R') )
                {
                    return line;
                }
            }
            return null; //This will only happen if the file is invalid - shouldn't ever happen
        }
    }
}
