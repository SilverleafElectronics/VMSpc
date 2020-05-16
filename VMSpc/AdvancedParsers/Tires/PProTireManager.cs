using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Communication;
using VMSpc.Parsers;

namespace VMSpc.AdvancedParsers.Tires
{
    public class PProTireManager : TireManager
    {
        private byte PProDeviceMid = 0xFF;

        public PProTireManager()
        {
            for (byte i = 0xA6; i < 0xAA; i++)
            {   //Starts out subscribed to all MIDs
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(194, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(210, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(211, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(212, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(241, i));
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1708RawDataEvent(254, i));
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
            throw new NotImplementedException();
        }

        public override void ClearTire(byte instance)
        {
            throw new NotImplementedException();
        }

        public override void LearnTire(byte instance)
        {
            OutgoingJ1708Message message = new OutgoingJ1708Message()
            {
                Mid = 0xB6,
                Pid = 0xFE,
                Data = new List<byte>() { instance, 0xFD },
            };

        }

        public override void Parse(CanMessageSegment segment)
        {
            if ((segment as J1708MessageSegment).Mid != PProDeviceMid)
            {
                PProDeviceMid = (segment as J1708MessageSegment).Mid;
                ResubscribeForMid();
            }
            switch (segment.Pid)
            {
                case 194:
                    break;
                case 210:
                    break;
                case 211:
                    break;
                case 212:
                    break;
                case 241:
                    break;
                case 254:
                    break;
            }
        }
    }
}
