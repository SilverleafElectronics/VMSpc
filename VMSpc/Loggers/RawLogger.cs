using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.JsonFileManagers;
using System.Timers;

namespace VMSpc.Loggers
{
    public class RawLogger : IEventConsumer, ISingleton
    {
        private SettingsContents Settings = ConfigurationManager.ConfigManager.Settings.Contents;
        private ConcurrentQueue<VMSCommDataEventArgs> MessagesToWrite;
        private StreamWriter streamWriter;
        private Timer MessageWriteTimer;
        private bool Enabled = false;

        static RawLogger() { }

        public static RawLogger Instance { get; private set; }
        public static void Initialize()
        {
            Instance = new RawLogger();
        }
        private RawLogger()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.NEW_COMM_DATA_EVENT);
            MessageWriteTimer = Constants.CREATE_TIMER(WriteLogEntries, 10000);
            MessageWriteTimer.Stop();
            MessagesToWrite = new ConcurrentQueue<VMSCommDataEventArgs>();
        }

        ~RawLogger()
        {
            Stop();
        }

        public void Start()
        {
            MessageWriteTimer?.Start();
            Enabled = true;
        }

        public void Stop()
        {
            MessageWriteTimer?.Stop();
            WriteLogEntries();
            Enabled = false;
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            if (Enabled)
            {
                var CommDataEvent = (e as VMSCommDataEventArgs);
                if (CommDataEvent != null)
                {
                    MessagesToWrite.Enqueue(CommDataEvent);
                }
            }
        }

        private void WriteLogEntries()
        {
            if (MessagesToWrite.Count > 0)
            {
                using (streamWriter = new StreamWriter(FileOpener.GetAbsoluteFilePath(Settings.rawLogFilePath)))
                {
                    foreach (var DataEvent in MessagesToWrite)
                    {
                        streamWriter.WriteLine(DataEvent.timeStamp.ToString());
                        streamWriter.WriteLine(DataEvent.message);
                    }
                }
            }
        }
    }
}
