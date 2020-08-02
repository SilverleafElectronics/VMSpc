using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Communication;
using VMSpc.Parsers;
using VMSpc.Enums.Parsing;

namespace VMSpc.AdvancedParsers.Tires
{
    public class PProTire : Tire
    {
        public byte status = 0xFF;
        public byte pressure = 0;
        public byte temperature = 0;
        public byte strength = 0;
        public byte messageCount = 0;
        public byte targetPressure = 0;
        public byte position = 0xFF;
        public bool alarmActive = false;

        public override double DisplayPressure => pressure * 0x6d;
        public override double DisplayTemperature => temperature * 2.5d;
        public double TargetPressure => targetPressure * 0.6d;
    }

    public class PProTireManager : TireManager
    {
        private byte PProDeviceMid;
        private byte PendingPosition;
        private byte PendingIndex;

        public PProTireManager()
        {
            PProDeviceMid = 0xFF;
            PendingPosition = 0xFF;
            PendingIndex = 0xFF;
            for (byte i = 0xA6; i < 0xAA; i++)
            {   //Starts out subscribed to all MIDs
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(194, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(210, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(211, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(212, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(241, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(254, i));
            }
            for (int i = 0; i < MAX_TIRES; i++)
            {
                Tires[i] = new PProTire();
            }
        }

        /// <summary>
        /// Subscribes to just the MID seen on the network
        /// </summary>
        private void ResubscribeForMid()
        {
            EventBridge.Instance.UnsubscribeFromAllEvents(this);
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(194, PProDeviceMid));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(210, PProDeviceMid));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(211, PProDeviceMid));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(212, PProDeviceMid));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(241, PProDeviceMid));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(254, PProDeviceMid));
        }

        public override void AbortLearn()
        {
            var message = new OutgoingJ1708Message()
            {
                USBPrefix = 'S',
                Mid = 0xB6,
                Pid = 0xFE,
                Data = new List<byte>()
                {
                    PProDeviceMid,
                    0xFD
                }
            };
            PendingPosition = 0xFF;
            CommunicationManager.Instance.SendMessage(message);
        }

        public override void ClearTire(byte instance)
        {
            var message = new OutgoingJ1708Message()
            {
                USBPrefix = 'S',
                Mid = 0xB6,
                Pid = 0xFE,
                Data = new List<byte>()
                {
                    PProDeviceMid,
                    0x01, 
                    (byte)((byte)(0x10 * (byte)(PProDeviceMid - 0xA6)) + instance), 
                    0x00, 
                    0x00, 
                    0x00, 
                    0xFF, 
                    0xFF, 
                    0xFF, 
                    0x00, 
                    0xFF, 
                    0xFF, 
                    0x00
                }
            };
            PendingPosition = instance;
            CommunicationManager.Instance.SendMessage(message);
        }

        public override void LearnTire(byte instance)
        {
            OutgoingJ1708Message message = new OutgoingJ1708Message()
            {
                USBPrefix = 'S',
                Mid = 0xB6,
                Pid = 0xFE,
                Data = new List<byte>() 
                { 
                    PProDeviceMid, 
                    0xFC,
                    0xFF,
                    0xFF
                },
            };
            CommunicationManager.Instance.SendMessage(message);
            PendingIndex = instance;
            PendingPosition = instance;
        }

        public override void Parse(CanMessageSegment segment)
        {
            byte numTires, position;
            List<byte> buffer = segment.RawData;
            if (buffer == null)
                return;
            if ((segment as J1708MessageSegment).Mid != PProDeviceMid)
            {
                PProDeviceMid = (segment as J1708MessageSegment).Mid;
                ResubscribeForMid();
            }
            switch (segment.Pid)
            {
                case 194:
                    ProcessError(buffer);
                    break;
                case 210:
                    ProcessTemperatures(buffer);
                    break;
                case 211:
                    ProcessPressures(buffer);
                    break;
                case 212:
                    ProcessTargetPressures(buffer);
                    break;
                case 254:
                    ProcessConfiguration(buffer);
                    break;
            }
        }

        private void ProcessTemperatures(List<byte> buffer)
        {
            byte numTires = buffer[0];
            if (numTires <= MAX_TIRES)
            {
                if (buffer.Count < (numTires))
                    return;
                for (int i = 0; i < numTires; i++)
                {
                    if (buffer[i + 1] < 0xFE)
                    {
                        (Tires[i] as PProTire).temperature = buffer[i + 1];
                    }
                }
            }
        }

        private void ProcessPressures(List<byte> buffer)
        {
            byte numTires = buffer[0];
            if (numTires <= MAX_TIRES)
            {
                if (buffer.Count < (numTires))
                    return;
                for (int i = 0; i < numTires; i++)
                {
                    var tire = (PProTire)Tires[i];
                    if (buffer[i + 1] < 0xFE)
                    {
                        tire.pressure = buffer[i + 1];
                        if (tire.TireStatus == TireStatus.NoData)
                        {
                            tire.TireStatus = TireStatus.Okay;
                        }
                    }
                    else
                    {
                        tire.pressure = 0;
                        if (tire.TireStatus != TireStatus.None)
                        {
                            tire.TireStatus = TireStatus.NoData;
                        }
                    }
                    if (tire.TireStatus == TireStatus.Alert || tire.TireStatus == TireStatus.Warning)
                    {
                        if ((DateTime.Now - tire.lastUpdate).TotalSeconds > 15) //15 second timeout
                        {
                            tire.TireStatus = TireStatus.Okay;
                        }
                    }
                }
            }
        }

        private void ProcessError(List<byte> buffer)
        {
            byte numTires = (byte)(buffer[0] / 2); // 2 bytes per code
            byte position = 1;
            for (byte i = 0; i < numTires; i++)
            {
                if (buffer.Count < position)
                    break;
                var tire = (PProTire)Tires[(buffer[position] - 18) & 0x0F];  // J1587 Tire SIDs, 18-33 - Pressure, 34-49 - Temp, 50-65 Battery
                if ((buffer[position + 1] & 0x40) == 0) // code active
                {
                    byte error = (byte)(((buffer[position] - 18) & 0xF0) - (buffer[position + 1] & 0x0F));   //SID group, plus failure mode
                    switch (error)
                    {
                        case 0x24:  //low battery
                        case 0x10:  //high temperature
                        case 0x0E:  //low pressure
                        case 0x00:  //high pressure
                            tire.TireStatus = TireStatus.Warning;
                            break;
                        case 0x01:  //very low pressure
                            tire.TireStatus = TireStatus.Alert;
                            break;
                        case 0x02:  //tire missing
                            tire.TireStatus = TireStatus.NoData;
                            break;
                    }
                    tire.lastUpdate = DateTime.Now;
                }
                position += 2;
            }
        }

        private void ProcessTargetPressures(List<byte> buffer)
        {
            byte numTires = buffer[0];
            if (numTires <= MAX_TIRES)
            {
                if (buffer.Count < numTires)
                    return;
                for (int i = 0; i < numTires; i++)
                {
                    if (buffer[i + 1] != 0xFF)
                    {
                        (Tires[i] as PProTire).targetPressure = buffer[i + 1];
                    }
                }
            }
        }

        private void ProcessConfiguration(List<byte> buffer)
        {
            if (buffer.Count < 12)
                return;
            if ((buffer[10] == 0xFF) && (PendingPosition != 0xFF))  //new sensor detected
            {
                if (buffer[3] != 0 || buffer[4] != 0 || buffer[5] != 0)
                {
                    SendLearnVerification(buffer);
                    PendingPosition = PendingIndex = 0xFF;
                }
                else if (buffer[10] != 0xFF)
                {
                    var tire = (Tires[buffer[2]] as PProTire);
                    tire.strength = (byte)((buffer[11] == 0xFF) ? 0 : buffer[11]);
                    tire.messageCount = (byte)((buffer[12] == 0xFF) ? 0 : buffer[12]);
                    tire.alarmActive = !(buffer[9] == 1);
                    if (tire.alarmActive)
                    {
                        tire.TireStatus = TireStatus.Okay;
                    }
                    if (tire.TireStatus == TireStatus.None)
                    {
                        tire.TireStatus = TireStatus.NoData;
                    }
                    if (buffer[6] < 0xFE)
                    {
                        tire.pressure = buffer[6];
                        if (tire.TireStatus == TireStatus.NoData)
                        {
                            tire.TireStatus = TireStatus.Okay;
                        }
                    }
                    tire.position = buffer[10];
                }
            }
        }

        private void SendLearnVerification(List<byte> buffer)
        {
            var clearMessage = new OutgoingJ1708Message()
            {
                USBPrefix = 'S',
                Mid = 0xB6,
                Pid = 0xFE,
                Data = new List<byte>()
                {
                    PProDeviceMid, 
                    0x01, 
                    (byte)(0x10 * (PProDeviceMid - 0xA6) + PendingPosition), 
                    buffer[3], 
                    buffer[4], 
                    buffer[5], 
                    0xFF, 
                    0xFF, 
                    0xFF, 
                    0x00, 
                    PendingPosition, 
                    0xFF, 
                    0x00
                },
            };
            CommunicationManager.Instance.SendMessage(clearMessage);
            var quitListenMessage = new OutgoingJ1708Message()
            { 
                USBPrefix = 'S',
                Mid = 0xB6,
                Pid = 0xFE,
                Data = new List<byte>()
                {
                    PProDeviceMid,
                    0xFD
                }
            };
            CommunicationManager.Instance.SendDelayedMessage(quitListenMessage, 1000);  //allow one second gap for the clearMessage to be processed by the device
        }
    }
}
