using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Parsers.SpecialParsers.TireParsers;
using VMSpc.Enums.Parsing;

namespace VMSpc.Parsers.TireParsers
{
    public class PressureProParser : TireParser
    {
        private byte[] getSetListenModeBase() => new byte[] { 0xB6, 0xFE, 0xFF, 0xFC, 0xFF, 0xFF };
        private byte[] getQuitListenModeBase() => new byte[] { 0xB6, 0xFE, 0xFF, 0xFD };
        private byte[] getClearRowBase() => new byte[] { 0xB6, 0xFE, 0xFF, 0x01, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0x00 };

        private byte tireMessageId;
        private byte pendingPosition;
        private byte pendingIndex;
        private byte messageCount;
        private IDataBus dataBus;

        static PressureProParser() { }
        public PressureProParser() 
        {
            tireMessageId = 0xA6;
            pendingPosition = 0xFF;
            pendingIndex = 0xFF;
            messageCount = 0;
            for (int i = 0; i < MAX_NUM_TIRES; i++)
            {
                Tires[i] = new PProTire(i);
            }
        }
        public static PressureProParser PproParser = new PressureProParser();
        public void SetDataBuse(IDataBus dataBus)
        {
            this.dataBus = dataBus;
        }

        public bool IsPressureProPid(byte pid)
        {
            return (pid >= 0xA6 || pid <= 0xA9);
        }

        public override void ClearTire(byte position)
        {
            throw new NotImplementedException();
        }

        public void ClearRow()
        {
            byte[] message = getClearRowBase();
            message[2] = tireMessageId;
            message[4] = (byte)((byte)(0x10 * (tireMessageId - 0xA6)) + pendingIndex);  // the upper nybble is the receiver address

            message[12] = pendingPosition;
            pendingPosition = 0xFF;
            pendingIndex = 0xFF;
        }

        public override void LearnTire(byte position)
        {
            throw new NotImplementedException();
        }

        public void Parse(int index, byte[] data)
        {
            byte pos,
                 bytesUnprocessed,
                 pid,
                 length = (byte)(index - 4);
            if ((length > 5) && (length < 22))
            {
                pos = 1;
                bytesUnprocessed = (byte)(length - 1);
                messageCount++;
                pid = data[pos];
                pos++;
                if (bytesUnprocessed > 1)
                {
                    ProcessByPid(pid, pos, data);
                }
            }
        }
        private void ProcessByPid(byte pid, byte pos, byte[] data)
        {
            byte numTires;
            //byte error;
            PProTire tire;
            switch (pid)
            {
                case 210:
                    numTires = data[2];
                    if (numTires < MAX_NUM_TIRES)
                    {
                        for (int i = 0; i < numTires; i++)
                        {
                            tire = (PProTire)Tires[i];
                            if (data[i + 3] < 0xFE)
                            {
                                tire.Temperature = data[i + 3];
                            }
                        }
                    }
                    break;
                case 211:
                    numTires = data[2];
                    if (numTires <= MAX_NUM_TIRES)
                    {
                        for (int i = 0; i < numTires; i++)
                        {
                            tire = (PProTire)Tires[i];
                            if (data[i + 3] < 0xFE)
                            {
                                tire.Pressure = data[i + 3];
                                if (tire.Status == TireStatus.NO_REPORT)
                                {
                                    tire.Status = TireStatus.OK;
                                }
                            }
                            else
                            {
                                tire.Pressure = 0;
                                if (tire.Status != TireStatus.NO_SENSOR_ASSIGNED)
                                {
                                    tire.Status = TireStatus.NO_REPORT;
                                }
                            }
                        }
                    }
                    ResetErrors();
                    break;
                case 212:
                    numTires = data[2];
                    if (numTires <= MAX_NUM_TIRES)
                    {
                        for (int i = 0; i < numTires; i++)
                        {
                            tire = (PProTire)Tires[i];
                            tire.TargetPressure = data[i + 3];
                        }
                    }
                    break;
                case 254:
                    HandleSensorData(data);
                    break;
                case 241: //make-up pressure. Should not happen
                case 194:
                    ParseDiagnostics(data);
                    break;
                default:
                    break;
            }
        }

        private void HandleSensorData(byte[] data)
        {

        }

        private void ParseDiagnostics(byte[] data)
        {
            int numTires = data[2] / 2;
            byte tireIndex;
            PProTire tire;
            byte pos = 3;
            for (int i = 0; i < numTires; i++)
            {
                tireIndex = (byte)((byte)(data[pos] - 18) & 0x0F);
                tire = (PProTire)Tires[tireIndex];
                if ( (data[pos + 1] & 0x40) == 0 )
                {
                    byte error = (byte)((byte)((byte)(data[pos] - 18) & 0xF0) + tireIndex);
                    switch (error)
                    {
                        case 0x24:  //low battery
                        case 0x10:  //hi temperature
                        case 0x0E:  //low pressure
                        case 0x00:  //hi pressure
                            tire.Status = TireStatus.WARNING;
                            break;
                        case 0x02:  //missing
                            tire.Status = TireStatus.NO_REPORT;
                            break;
                        case 0x01:  //very low pressure
                            tire.Status = TireStatus.ALERT;
                            break;
                    }
                    tire.LastUpdate = DateTime.Now;
                }
                pos += 2;
            }
        }

        private void ResetErrors()
        {
            DateTime now = DateTime.Now;
            foreach(var tire in Tires)
            {
                if (tire.Status == TireStatus.ALERT || tire.Status == TireStatus.WARNING)
                {
                    if ( (now - tire.LastUpdate).TotalSeconds > 15)
                    {
                        tire.Status = TireStatus.OK;
                    }
                }
            }
        }

        private void ProcessChange()
        {
            ProcessChange();
            foreach (var tire in Tires)
            {
                tire.ProcessChange();
            }
        }
    }
}
