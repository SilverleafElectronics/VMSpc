using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Enums.Parsing;
using static VMSpc.Constants;

namespace VMSpc.Parsers
{
    public abstract class CanMessageSegment
    {
        public VMSDataSource DataSource;
        public string RawMessageSegment;
        public ushort Pid;
        public ulong RawValue;
        public List<byte> RawData;
        public double StandardValue;
        public double MetricValue;
        public DateTime TimeReceived;
        public DateTime TimeParsed;
        public ParseStatus ParseStatus;
    }

    public class J1708MessageSegment : CanMessageSegment
    {
        public byte Mid;
        private byte DataByteIndex;
        private byte NumDataBytes;

        public J1708MessageSegment(string RawMessageSegment, byte Mid)
        {
            DataSource = VMSDataSource.J1708;
            this.Mid = Mid;
            this.RawMessageSegment = RawMessageSegment;
            BYTE_STRING_TO_BYTE_ARRAY(ref RawData, RawMessageSegment, RawMessageSegment.Length);
            SetPidAndRawValue();
        }

        private void SetPidAndRawValue()
        {
            SetPID();
            SetDataStartIndex();
            SetNumDataBytes();
            SetRawValue();
        }

        public void SetPID()
        {
            Pid = RawData[0];
            if (Pid == 0xFF)
            {
                Pid = (ushort)(RawData[1] + 256); //extension PIDs start at 256
            }
        }

        public void SetDataStartIndex()
        {
            if (Pid < 256)
            {
                if (Pid < 192 || Pid > 254)
                    DataByteIndex = 1;  //the pid is the first byte of the parameter, so the data starts at index 1
                else
                    DataByteIndex = 2;  //the pid is only a single byte, but the second byte of the parameter indicates the number of data bytes, so the data starts at index 2
            }
            else
            {
                if (Pid < 384 || Pid > 487)
                    DataByteIndex = 2;  //the pid is transmitted as the first two bytes of the parameter, so the data starts at index 2
                else
                    DataByteIndex = 3; //the pid takes up two bytes, but the third byte indicates the number of data bytes, so the data starts at index 3
            }
        }

        public void SetNumDataBytes()
        {
            if ((Pid >= 0 && Pid < 128) || (Pid > 255 && Pid < 384))
            {   //PIDS 0-127 and 256-383 carry single-byte data values
                NumDataBytes = 1;
            }
            else if ((Pid > 127 && Pid < 192) || (Pid > 383 && Pid < 448))
            {   //PIDs 128-191 and 384-449 carry 2-byte data values
                NumDataBytes = 2;
            }
            else
            {   //all other 
                NumDataBytes = RawData[DataByteIndex - 1];
            }
        }

        private void SetRawValue()
        {
            byte addedIndex = 0;
            RawValue = 0;
            while (addedIndex < NumDataBytes && (addedIndex + DataByteIndex) < RawData.Count)
            {
                RawValue |= ((ulong)(RawData[DataByteIndex + addedIndex]) << (8 * addedIndex));
                addedIndex++;
            }
        }

        public byte GetTotalMessageBytes()
        {
            return (byte)(DataByteIndex + NumDataBytes);
        }
    }

    public class J1939MessageSegment : CanMessageSegment
    {
        public byte SourceAddress { get; set; }
        public uint PGN { get; set; }
        public J1939MessageSegment()
        {
            DataSource = VMSDataSource.J1939;
        }
    }
}
