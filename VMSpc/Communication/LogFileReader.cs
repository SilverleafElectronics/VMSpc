using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using static VMSpc.Constants;

namespace VMSpc.Communication
{
    class LogFileReader : DataReader
    {
        private Timer logReadTimer;
        private StreamReader logReader;
        private string logPlayerFile;

        public LogFileReader(Action<string> DataProcessor)
            : base(DataProcessor)
        {

        }
        public override void InitDataReader()
        {
            logReader = new StreamReader(logPlayerFile);
            logReadTimer = CREATE_TIMER(ReadLogEntry, 100);
        }

        /// <summary>
        /// Called every 100ms when using the log reader. Reads the next line of the log file and passes the message to ProcessData(). 
        /// Returns to the beginning of the file once reaching the end
        /// </summary>
        private void ReadLogEntry(object sender, ElapsedEventArgs e)
        {
            string line = " ";
            while (line != null && line.Length < 2)
                line = logReader.ReadLine();
            if (line != null && (line[0] == 'J' || line[0] == 'R'))
                Application.Current.Dispatcher.Invoke(delegate
                {
                    DataProcessor(line);
                });
            else if (line != null)          //continue reading until a valid line is found
                ReadLogEntry(null, null);
            else
            {
                logReader.DiscardBufferedData();
                logReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
        }

        public override void CloseDataReader()
        {
            throw new NotImplementedException();
        }
        public override bool SendMsg()
        {
            throw new NotImplementedException();
        }
        protected override void KeepJibAwake(object source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
