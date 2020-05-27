using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.AdvancedParsers;
using VMSpc.Parsers;

namespace VMSpc.JsonFileManagers
{
    public class DiagnosticLogContents : IJsonContents
    {
        public List<DiagnosticLogRecord> DiagnosticRecords { get; set; }
    }

    public class DiagnosticLogReader : JsonFileReader<DiagnosticLogContents>
    {
        public DiagnosticLogReader() : base("\\logs\\Diagnostics.json")
        {

        }

        protected override DiagnosticLogContents GetDefaultContents()
        {
            return new DiagnosticLogContents()
            { 
                DiagnosticRecords = new List<DiagnosticLogRecord>()
            };
        }
    }

    public class DiagnosticLogRecord
    {
        public string Source { get; set; }
        public string Type { get; set; }
        public string MID { get; set; }
        public string Component { get; set; }
        public string Mode { get; set; }
        public string Date { get; set; }
    }
}
