using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.AdvancedParsers;
using VMSpc.Common;
using VMSpc.JsonFileManagers;

namespace VMSpc.Loggers
{
    public sealed class DiagnosticLogger : IEventConsumer
    {

        private List<DiagnosticLogRecord> DiagnosticRecords = ConfigurationManager.ConfigManager.DiagnosticLogReader.Contents.DiagnosticRecords;

        static DiagnosticLogger() { }

        private DiagnosticLogger()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.DIAGNOSTIC_BASE);
        }
        public static DiagnosticLogger Instance { get; private set; }

        public static void Initialize()
        {
            Instance = new DiagnosticLogger();
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            var record = (e as DiagnosticEventArgs)?.message;
            if (record != null)
            {
                AddLogEntry(record);
            }
        }
        
        private void AddLogEntry(DiagnosticMessage record)
        {
            DiagnosticRecords.Add(new DiagnosticLogRecord()
            {
                Source = record.SourceString,
                Type = record.TypeString,
                MID = record.MidString,
                Component = record.Component,
                Mode = record.FmiString,
                Date = record.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss tt"),
            });
        }
    }
}
