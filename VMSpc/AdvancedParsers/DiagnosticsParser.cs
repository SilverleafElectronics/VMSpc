using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.DevHelpers;
using VMSpc.Enums.Parsing;
using VMSpc.Extensions.Standard;
using VMSpc.Parsers;

namespace VMSpc.AdvancedParsers
{
    public sealed class DiagnosticsParser : AdvancedParser, IEventConsumer, IEventPublisher, ISingleton
    {
        public List<DiagnosticMessage> ActiveDiagnosticMessages { get; private set; }
        static DiagnosticsParser()
        {

        }
        public DiagnosticsParser()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(0xC2, 0x80));
            EventBridge.Instance.AddEventPublisher(this);
            ActiveDiagnosticMessages = new List<DiagnosticMessage>();
        }

        public static DiagnosticsParser Instance { get; set; }

        public static void Initialize()
        {
            Instance = new DiagnosticsParser();
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        public void ConsumeEvent(VMSEventArgs e)
        {
            var segment = (e as VMSRawDataEventArgs)?.messageSegment;
            switch (segment?.DataSource)
            {
                case VMSDataSource.J1708:
                    var j1708Segment = (segment as J1708MessageSegment);
                    if (j1708Segment.Mid == 0x80)
                    {   //we only parse engine faults at MID 0x80
                        ProcessJ1708Diagnostic(j1708Segment.RawData);
                    }
                    break;
                case VMSDataSource.J1939:
                    var j1939Segment = (segment as J1939MessageSegment);
                    if (j1939Segment.SourceAddress == 0)
                    {   //we only parse engine faults at SA 0
                        ProcessJ1939Diagnostic(j1939Segment.RawData);
                    }
                    break;
            }
        }

        public void ProcessJ1708Diagnostic(List<byte> RawData)
        {
            byte totalMessages = RawData[1];
            RawData = RawData.SubList(2);
            var count = RawData.Count;
            while (count >= 2)
            {
                var message = new J1708DiagnosticMessage(RawData);
                RawData = RawData.SubList(message.MessageLength);
                var identicalMessage = GetStoredDiagnosticMessage(message.ID, message.Fmi);
                if (identicalMessage == null)
                {
                    ActiveDiagnosticMessages.Add(message);
                    PublishEvent(message);
                }
                else
                {
                    //identicalMessage.
                }
                count = RawData.Count;
            }
        }

        public void ProcessJ1939Diagnostic(List<byte> RawData)
        {
            var message = new J1939DiagnosticMessage(RawData);
            if (IsNewMessage(message))
            {
                ActiveDiagnosticMessages.Add(message);
                PublishEvent(message);
            }
        }

        /// <summary>
        /// Looks up a stored Diagnostic Record by ID and FMI (regardless of ID Type)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fmi"></param>
        /// <returns></returns>
        public DiagnosticMessage GetStoredDiagnosticMessage(uint id, byte fmi)
        {
            return ActiveDiagnosticMessages.Find(x => (x.ID == id) && (x.Fmi == fmi) );
        }

        /// <summary>
        /// Looks up a stored Diagnostic Record by ID, regardless of ID Type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DiagnosticMessage GetStoredDiagnosticMessage(uint id)
        {
            return ActiveDiagnosticMessages.Find(x => (x.ID == id));
        }

        /// <summary>
        /// Determines whether or not the record is already in the ActiveDiagnosticMessages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool IsNewMessage(DiagnosticMessage message)
        {
            return (GetStoredDiagnosticMessage(message.ID, message.Fmi) == null);
        }

        public void PublishEvent(DiagnosticMessage message)
        {
            RaiseVMSEvent?.Invoke(this, new DiagnosticEventArgs(message));
        }
    }

    public abstract class DiagnosticMessage
    {
        public uint ID { get; protected set; }
        public byte Mid { get; set; }
        public DateTime TimeStamp { get; protected set; }
        public byte Fmi { get; protected set; }
        public virtual string SourceString => DiagnosticStrings.CodeTypeAsString(Mid);
        public abstract string TypeString { get; }
        public string IDString => ID.ToString();
        public string MidString => Mid.ToString();
        public abstract string Component { get; }
        public string FmiString => DiagnosticStrings.GetModeString(Fmi);
        public bool Acknowledged { get; set; } = false;

        public DiagnosticMessage()
        {
            TimeStamp = DateTime.Now;
        }
    }

    public class J1939DiagnosticMessage : DiagnosticMessage
    {
        public override string SourceString => "ENG";
        public override string TypeString => "SPN";
        public override string Component => DiagnosticStrings.GetPidString(ID);

        public J1939DiagnosticMessage(List<byte> rawData)
            :base()
        {
            SetSPN(rawData);
            SetFMI(rawData);
        }

        private void SetSPN(List<byte> rawData)
        {
            ID = rawData[1];
            ID <<= 8;
            ID |= rawData[2];
            ID <<= 8;
            ID |= rawData[3];
            ID >>= 5;
            var spn = GetLittleEndianSPN(rawData);
            if (ID > spn)
            {
                ID = spn;
            }
        }

        public void SetFMI(List<byte> message)
        {
            Fmi = (byte)(message[3] & 0x1F);
        }

        public uint GetLittleEndianSPN(List<byte> message)
        {
            uint spn_le = message[3] & 0xFFFFFFE0;
            spn_le <<= 3;
            spn_le += message[2];
            spn_le <<= 8;
            spn_le += message[1];
            return spn_le;
        }
    }

    public class J1708DiagnosticMessage : DiagnosticMessage
    {
        public string RawCode { get; private set; }
        public int MessageLength;
        public J1708DiagnosticIDType IDType { get; private set; }
        public byte ErrorCode { get; private set; }
        public bool CountIncluded { get; private set; }
        public bool IsActive { get; private set; }
        public override string TypeString => IDType.ToString();
        public override string Component => (IDType == J1708DiagnosticIDType.PID) ? DiagnosticStrings.GetPidString(ID) : DiagnosticStrings.GetSIDString(Mid, ID);


        //public byte OccurrenceCount { get; private set; }

        public J1708DiagnosticMessage(List<byte> rawData)
            :base()
        {
            ID = rawData[0];
            Mid = 0x80; //We only take engine faults from J1708
            CountIncluded = ((rawData[1] & 0x80b) == 1);
            IsActive = ((rawData[1] & 0x40b) == 0);
            Fmi = (byte)(rawData[1] & 0x0Fb);
            MessageLength = (CountIncluded) ? 3 : 2;
            var byteList = rawData.SubList(0, MessageLength);
            RawCode = byteList.ToHexString();
        }
    }
}
