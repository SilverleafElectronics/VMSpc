using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Communication;
using VMSpc.Parsers;
using VMSpc.Enums.Parsing;
using static VMSpc.Common.RVCUpdaters;

//TODO - remove the spn definitions from J1939Parser, and place them in VMSpc.Common.Parsing. Not necessary for their current limited 
//use cases, but this would be useful if J1939 parsing capabilities need to be extended in the future.

namespace VMSpc.AdvancedParsers.Tires
{
    public class TSTTire : Tire
    {
        public byte pressure = 0;
        public ushort temperature = 0;
        public byte enableStatus = 0;
        public byte leakStatus = 0;
        public byte electricalStatus = 0;
        public byte pressureStatus = 7;
        public byte extendedPressure = 0;

        public override double DisplayPressure => pressure * 0.580150951 + extendedPressure * 148.5186;
        public override double DisplayTemperature => temperature * 1.8d - 40d;
    }

    public class TSTTireManager : TireManager
    {
        byte messageCount = 0;

        public const int MAX_AXLES = 5;
        public byte PendingPosition { get; private set; } = 0xFF;

        static byte[] positionToAxle = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x10, 0x11, 0x12, 0x13, 0x20, 0x21, 0x22, 0x23, 0x30, 031, 0x32, 0x33, 0x40, 0x41, 0x42, 0x43 };

        private DateTime LastMessageTimeStamp;
        public bool Learning { get; private set; }
        /// <summary>
        /// Returns true if a message has been received within the last 20 seconds.
        /// </summary>
        public bool IsActive => ((DateTime.Now - LastMessageTimeStamp).TotalSeconds < 20);
        public byte[] targetPsi;
        private byte underPressureThreshold;
        private byte overPressureThreshold;
        private byte overTemperatureThreshold;
        private byte extremeUnderPressureThreshold;
        public double TargetPsi(int axle) => (axle < MAX_AXLES) ? (targetPsi[axle] * 1.160301902) : 0;
        public double UnderPressureThreshold => underPressureThreshold * 0.5d;
        public double ExtremeUnderPressureThreshold => extremeUnderPressureThreshold * 0.5d;
        public double OverPressureThreshold => overPressureThreshold * 0.5d;
        public double OverTemperatureThreshold => (overTemperatureThreshold * 1.8d - 40d);

        public TSTTireManager()
            : base()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xEF33, 0x33));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xFEF4, 0x33));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xFDB9, 0x33));
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.Get_J1939RawDataEvent(0xFC43, 0x33));
            LastMessageTimeStamp = new DateTime(1, 1, 1, 1, 1, 1);
            InitializeTires();
            InitializeConfiguration();
        }

        private void InitializeTires()
        {
            for (int i = 0; i < MAX_TIRE_POSITIONS; i++)
            {
                Tires[i] = new TSTTire();
            }
        }

        private void InitializeConfiguration()
        {
            targetPsi = new byte[MAX_AXLES];
            for (int i = 0; i < MAX_AXLES; i++)
            {
                targetPsi[i] = 0;
            }
        }

        public void ConfigureTargetPSI(int axle, double newValue)
        {
            byte dataAxle = (byte)axle;
            if (axle < MAX_AXLES || axle == 0xEE)
            {
                OutgoingJ1939Message message = new OutgoingJ1939Message()
                {
                    USBPrefix = 'T',
                    SA = 0x33,
                    PGN = 0xAE00,
                };
                message.Data[0] = (byte)(dataAxle << 4);
                message.Data[1] = (byte)(newValue / 1.160301902);
                CommunicationManager.Instance.SendMessage(message);
            }
        }

        public void ConfigureUnderPressure(double newValue)
        {
            byte value = (byte)(newValue / 0.5);
            ConfigureTireThreshholds(3, value);
        }

        public void ConfigureExtremeUnderPressure(double newValue)
        {
            byte value = (byte)(newValue / 0.5);
            ConfigureTireThreshholds(4, value);
        }

        public void ConfigureOverPressure(double newValue)
        {
            byte value = (byte)(newValue / 0.5);
            ConfigureTireThreshholds(5, value);
        }

        public void ConfigureOverTemperature(double newValue)
        {
            byte value = (byte)((newValue / 1.8) + 40);
            ConfigureTireThreshholds(6, value);
        }

        public void ResetDefaults()
        {
            ConfigureTargetPSI(0xEE, 86); // 100 psi
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0x8200,
            };
            message.Data[3] = 40;
            message.Data[4] = 60;
            message.Data[5] = 40;
            message.Data[6] = 117;
            CommunicationManager.Instance.SendMessage(message);
        }

        private void ConfigureTireThreshholds(int byteIndex, byte value)
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0x8200,
            };
            message.Data[byteIndex] = value;
            CommunicationManager.Instance.SendMessage(message);
        }

        public override void AbortLearn()
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0xEF33,
            };
            message.Data[0] = 0xFC;
            message.Data[6] = 0x55;
            message.Data[7] = 0x49;
            if (PendingPosition < MAX_TIRE_POSITIONS)
                Tires[PendingPosition].sensorReporting = false;
            PendingPosition = 0xFF;
            CommunicationManager.Instance.SendMessage(message);
            RequestConfiguration();
        }

        public override void ClearTire(byte instance)
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0xEF33,
            };
            message.Data[0] = 0xFE;
            message.Data[1] = positionToAxle[instance];
            message.Data[6] = (byte)'U';
            message.Data[7] = (byte)'I';

            Tires[instance].sensorReporting = false;
            CommunicationManager.Instance.SendMessage(message);
        }

        public void RequestConfiguration()
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0xEA33,
            };
            message.Data[0] = 0x43;
            message.Data[1] = 0xFC;
            message.Data[2] = 0x00;
            CommunicationManager.Instance.SendMessage(message);
            OutgoingJ1939Message message2 = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0xEA33,
            };
            message2.Data[0] = 0xB9;
            message2.Data[1] = 0xFD;
            message2.Data[2] = 0x00;
            CommunicationManager.Instance.SendMessage(message);
        }

        public override void LearnTire(byte instance)
        {
            OutgoingJ1939Message message = new OutgoingJ1939Message()
            {
                USBPrefix = 'T',
                SA = 0x33,
                PGN = 0xEF33,
            };
            message.Data[0] = 0xFD;
            message.Data[1] = positionToAxle[instance];
            message.Data[6] = 0x55;
            message.Data[7] = 0x49;
            PendingPosition = instance;
            CommunicationManager.Instance.SendMessage(message);
        }

        public override void Parse(CanMessageSegment segment)
        {
            messageCount++;
            LastMessageTimeStamp = DateTime.Now;
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
                            Tires[position].id = (data[3] << 16) | (data[4] << 8) | data[5];
                            byte idStatus = (byte)(data[2] & 0x03);
                            switch (idStatus)
                            {
                                case 0: //Sensor position is in learning mode. Do nothing
                                    break;
                                case 1: //Sensor has been learned
                                    if (Tires[position].TireStatus == TireStatus.None)
                                    {
                                        Tires[position].TireStatus = TireStatus.NoData;
                                    }
                                    break;
                                case 2: //Sensor has been forgotten
                                    Tires[position] = new TSTTire();
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
                    TSTTire currentTire = (Tires[position] as TSTTire);
                    if (position < 20)
                    {
                        currentTire.sensorReporting = true;
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
                            currentTire.TireStatus = TireStatus.Warning;
                        if (currentTire.electricalStatus == 1 || currentTire.electricalStatus == 2)
                            currentTire.TireStatus = TireStatus.Warning;
                        switch (currentTire.pressureStatus)
                        {
                            case 0:
                            case 1:
                            case 3:
                                currentTire.TireStatus = TireStatus.Warning;
                                break;
                            case 4:
                                currentTire.TireStatus = TireStatus.Alert;
                                break;
                            case 6:
                                currentTire.TireStatus = TireStatus.NoData;
                                break;
                        }
                        if ((currentTire.id == 0) || (currentTire.id == 0xFFFFFF))
                        {
                            if (PendingPosition == position)
                            {
                                PendingPosition = 0xFF;
                            }
                        }
                    }
                    break;
                case 0xFDB9:
                    axle = (byte)((data[0] >> 4) & 0x0F);
                    if (axle < MAX_AXLES)
                    {
                        targetPsi[axle] = data[1];
                    }
                    else if (axle == 0xE) //all axles
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            UpdateByte(ref targetPsi[i], data[1]);
                        }
                    }
                    break;
                case 0xFC43:
                    UpdateByte(ref underPressureThreshold, data[3]);
                    UpdateByte(ref extremeUnderPressureThreshold, data[4]);
                    UpdateByte(ref overPressureThreshold, data[5]);
                    UpdateByte(ref overTemperatureThreshold, data[6]);
                    break;
            }
        }
    }
}
