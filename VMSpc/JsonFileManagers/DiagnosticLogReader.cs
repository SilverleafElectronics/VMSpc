﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Parsers;

namespace VMSpc.JsonFileManagers
{
    public class DiagnosticLogContents : IJsonContents
    {
        public List<DiagnosticRecord> DiagnosticRecords { get; set; }
    }

    public class DiagnosticLogReader : JsonFileReader<DiagnosticLogContents>
    {
        public DiagnosticLogReader() : base("\\Logs\\Diagnostics.json")
        {

        }

        protected override DiagnosticLogContents GetDefaultContents()
        {
            return new DiagnosticLogContents()
            { 
                DiagnosticRecords = new List<DiagnosticRecord>()
            };

        }
    }
}
