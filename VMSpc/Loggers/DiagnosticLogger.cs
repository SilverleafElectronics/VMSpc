using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Parsers;
using VMSpc.Common;

namespace VMSpc.Loggers
{
    public sealed class DiagnosticLogger : IEventConsumer
    {

        public List<DiagnosticRecord> DiagnosticRecords;

        static DiagnosticLogger() { }

        private DiagnosticLogger()
        {
            EventBridge.EventProcessor.SubscribeToEvent(this, EventIDs.DIAGNOSTIC_BASE);
        }
        public static DiagnosticLogger Recorder { get; } = new DiagnosticLogger();

        public void ConsumeEvent(VMSEventArgs e)
        {
            var record = (e as DiagnosticEventArgs)?.record;
            if (record != null)
            {
                AddLogEntry(record);
            }
        }

        private void AddLogEntry(DiagnosticRecord record)
        {

        }
    }

    public class DiagnosticLogRecord
    {
        string Source { get; set; }
        string Type { get; set; }
        string MID { get; set; }
        string Component { get; set; }
        string Mode { get; set; }
        string Date { get; set; }
        public DiagnosticLogRecord(J1708Record record)
        {
            Source = DiagnosticGuiHelper.CodeTypeAsString(record.mid);
            Type = ((record.ecode & 0x10) == 0x10) ? "SID" : "PID";
            MID = string.Format(" {0, 5}", record.spn);

        }

        public DiagnosticLogRecord(J1939Record record)
        {

        }
    }
}
