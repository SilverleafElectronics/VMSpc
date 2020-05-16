using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Communication;
using VMSpc.Parsers;
using static VMSpc.Common.RVCUpdaters;

namespace VMSpc.AdvancedParsers.Tires
{

    public enum TireStatus
    { 
        None,
        NoData,
        Green,
        Yellow,
        Red
    }


    public class TSTTire
    {
        public byte pressure = 0;
        public double displayPressure => pressure * 0.580150951 + extendedPressure * 148.5186;
        public ushort temperature = 0;
        public byte enableStatus = 0;
        public byte leakStatus = 0;
        public byte electricalStatus = 0;
        public byte pressureStatus = 7;
        public byte extendedPressure = 0;
        public int id = 0;
        public bool sensorReporting = false;
        public TireStatus TireStatus = TireStatus.None;
    }

    public class TSTTireManager : TireManager
    {
        private const int MaxTirePositions = 20;
        byte messageCount = 0;
        private TSTTire[] TSTTires = new TSTTire[20];
        public byte PendingPosition { get; private set; } = 0xFF;

        static byte[] clearPosition = new byte[] { 0xFE, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, (byte)'U', (byte)'I' };
        static byte[] positionToAxle = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x10, 0x11, 0x12, 0x13, 0x20, 0x21, 0x22, 0x23, 0x30, 031, 0x32, 0x33, 0x40, 0x41, 0x42, 0x43 };

        public TSTTireManager()
            :base()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xEF33, 0x33));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xFEF4, 0x33));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xFDB9, 0x33));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xFC43, 0x33));
            InitializeTires();
        }

        private void InitializeTires()
        {
            for (int i = 0; i < MaxTirePositions; i++)
            {
                TSTTires[i] = new TSTTire();
            }
        }

        public override void AbortLearn()
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                SA = 0x33,
                PGN = 0xEF33,
            };
            if (PendingPosition < MaxTirePositions)
                TSTTires[PendingPosition].sensorReporting = false;
            PendingPosition = 0xFF;
            //CommManager.Instance.SendMessage(message);
        }

        public override void ClearTire(byte instance)
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                SA = 0x33,
                PGN = 0xEF33,
            };
            message.Data[0] = 0xFE;
            message.Data[1] = positionToAxle[instance];
            message.Data[6] = (byte)'U';
            message.Data[7] = (byte)'I';

            TSTTires[instance].sensorReporting = false;
        }

        public void RequestConfiguration()
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                SA = 0x33,
                PGN = 0xEF33,
            };
            message.Data[0] = 0x43;
            message.Data[1] = 0xFC;
            message.Data[2] = 0x00;
            //CommManager.Instance.SendMessage(message);
            OutgoingJ1939Message message2 = new OutgoingJ1939Message()
            {
                SA = 0x33,
                PGN = 0xEF33,
            };
            message2.Data[0] = 0xB9;
            message2.Data[1] = 0xFD;
            message2.Data[2] = 0x00;
            //CommManager.Instance.SendMessage(message);
        }

        public override void LearnTire(byte instance)
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                SA = 0x33,
                PGN = 0xEF33,
            };
            message.Data[0] = 0x33;
            message.Data[1] = 0xFD;
            message.Data[2] = positionToAxle[instance];
            PendingPosition = instance;
            //CommManager.Instance.SendMessage(message);
        }

        public override void Parse(CanMessageSegment segment)
        {
            messageCount++;
            byte axle;
            byte tire;
            byte position;
            var data = (segment as J1939MessageSegment)?.RawData.ToArray();
            var pgn = (segment as J1939MessageSegment)?.PGN;
            switch (pgn)
            {
                case 0xFE33:
                    if ((data[6] == 54) && data[7] == 50)   //indicating 'T' and 'P'
                    {
                        axle = (byte)((data[1] >> 4) & 0x0F);
                        tire = (byte)(data[1] & 0x0F);
                        position = (byte)((byte)(axle * 4) + tire);
                        if (position < 20)
                        {
                            TSTTires[position].id = (data[3] << 16) | (data[4] << 8) | data[5];
                            byte idStatus = (byte)(data[2] & 0x03);
                            switch (idStatus)
                            {
                                case 0: //Sensor position is in learning mode. Do nothing
                                    break;
                                case 1: //Sensor has been learned
                                    if (TSTTires[position].TireStatus == TireStatus.None)
                                    {
                                        TSTTires[position].TireStatus = TireStatus.NoData;
                                    }
                                    break;
                                case 2: //Sensor has been forgotten
                                    TSTTires[position] = new TSTTire();
                                    break;
                                case 3: //Sensor ID report. Do nothing
                                    break;

                            }
                        }
                    }
                    break;
                case 0xFEF4:
                    axle = (byte)((data[0] >> 4) & 0x0F);
                    tire = (byte)(data[0] & 0x0F);
                    position = (byte)((axle * 4) + tire);
                    var currentTire = TSTTires[position];
                    if (position < 20)
                    {
                        TSTTires[position].sensorReporting = true;
                        if (UpdateByte(ref currentTire.pressure, data[1]) != 0)
                        {
                            UpdateFlag(ref currentTire.extendedPressure, data[4], 6);
                        }
                        UpdateWord(ref currentTire.temperature, data, 2);
                        UpdateFlag(ref currentTire.leakStatus, data[4], 2);
                        UpdateFlag(ref currentTire.electricalStatus, data[4], 4);
                        UpdateBits(ref currentTire.pressureStatus, data[7], 5, 3);
                        UpdateFlag(ref currentTire.enableStatus, data[4], 0);

                        if (currentTire.leakStatus != 0)
                            currentTire.TireStatus = TireStatus.Yellow;
                        if (currentTire.electricalStatus == 1 || currentTire.electricalStatus == 2)
                            currentTire.TireStatus = TireStatus.Yellow;
                        switch (currentTire.pressureStatus)
                        {
                            case 0:
                            case 1:
                            case 3:
                                currentTire.TireStatus = TireStatus.Yellow;
                                break;
                            case 4:
                                currentTire.TireStatus = TireStatus.Red;
                                break;
                            case 6:
                                currentTire.TireStatus = TireStatus.NoData;
                                break;
                        }
                        if ( (currentTire.id == 0) || (currentTire.id == 0xFFFFFF) )
                        {
                            if (PendingPosition == position)
                            {
                                PendingPosition = 0xFF;
                            }
                        }
                    }
                    break;
                    //TODO for these reports - set up public TSPNPresenters
                case 0xFDB9:
                    axle = (byte)((data[0] >> 4) & 0x0F);
                    if (axle < 5)
                    {
                        //spn_target_psi[axle]->parse(data);
                    }
                    else if (axle == 0xE ) //all axles
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            //spn_target_psi[i]->parse(data);
                        }
                    }
                    break;
                case 0xFC43:
                    //spn_under_pressure_threshold->parse(data);        //TSPNByte
                    //spn_ext_under_pressure_threshold->parse(data);    //T
                    //spn_over_pressure_threshold->parse(data);
                    //spn_over_temp_threshold->parse(data);
                    break;
            }
        }
    }
}
