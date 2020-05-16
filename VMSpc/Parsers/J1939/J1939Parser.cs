using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;
using static VMSpc.Constants;
using static VMSpc.Parsers.PGNMapper;
using VMSpc.DevHelpers;
using VMSpc.Enums.Parsing;

namespace VMSpc.Parsers
{
    public class J1939Parser : IDataBus
    {
        public readonly int spn;
        public J1939Parser()
        {

        }

        public void Parse(CanMessage canMessage)
        {
            var j1939Message = (canMessage as J1939Message);
            //Parse the data if it is available in the map of standard parsers
            if (PGNMap.ContainsKey(j1939Message.pgn) && PGNMap[j1939Message.pgn] != null)
            {
                foreach (TSPNDatum datum in PGNMap[j1939Message.pgn])
                {
                    datum.Parse(j1939Message);
                }
            }
            //Add a single empty J1939MessageSegment, with ParseStatus left at NotParsed. These messages should either
            //be dropped or handled by an AvcancedParser
            else
            {
                j1939Message.CanMessageSegments.Add(
                    new J1939MessageSegment()
                    {
                        Pid = ushort.MaxValue,
                        PGN = j1939Message.pgn,
                        SourceAddress = j1939Message.address,
                        RawData = j1939Message.data.ToList(),
                        RawValue = 1,
                        StandardValue = 1,
                        MetricValue = 1,
                        ParseStatus = ParseStatus.NotParsed,
                    });
            }
        }

        public void SendMessage(byte[] message)
        {

        }
    }
}
