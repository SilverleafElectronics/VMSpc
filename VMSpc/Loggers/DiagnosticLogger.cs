using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.AdvancedParsers;
using VMSpc.Common;

namespace VMSpc.Loggers
{
    public sealed class DiagnosticLogger : IEventConsumer
    {

        public List<DiagnosticMessage> DiagnosticRecords;

        static DiagnosticLogger() { }

        private DiagnosticLogger()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.DIAGNOSTIC_BASE);
        }
        public static DiagnosticLogger Recorder { get; } = new DiagnosticLogger();

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
        public DiagnosticLogRecord(J1708DiagnosticMessage record)
        {
        }

        public DiagnosticLogRecord(J1939DiagnosticMessage record)
        {

        }
    }
}
