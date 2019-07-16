using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.DevHelpers;
using static VMSpc.Constants;
using static VMSpc.Parsers.ChassisParameter;

/// <summary>
/// Update Notes:
///     - Removed parameter index from TSPNDatum. It appeared to have no effect
///     - Removed parselong() from original source. It was never called in 
///       previous versions. Be sure to implement in accordance with the parsers
///       below if necessary.
/// </summary>

namespace VMSpc.Parsers
{
    public class TSPNDatum
    {
        public uint rawValue;
        public double value, valueMetric;
        public double recipNum;
        public bool seen;
        public bool prioritize1708;
        public ushort spn;

        public TSPNDatum(ushort spn)
        {
            rawValue = 0;
            value = valueMetric = 0.0;
            seen = false;
            prioritize1708 = false;
            this.spn = spn;
        }

        public virtual void Parse(byte address, byte[] data) { }

        protected virtual void ConvertAndStore()
        {
            seen = true;
        }

        #region Update Methods
        //Note: these all compile with the unsafe modifiers.

        /// <summary>
        /// updates flag according to the input two-bit pattern:
        ///    00 = sets datum to 0, returns 1
        ///    01 = sets datum to 1, returns 1
        ///    10 = sets datum to 0, returns 2 for error (not allowed in RV-C standard)
        ///    11 = no change,       returns 0
        /// </summary>
        protected byte UpdateFlag(ref byte dest, byte src, byte pos)
        {
            byte f = (byte)(((src) >> pos) & 0x03);
            if ((f & 0x02) == 0)
            {
                dest = f;
                return 1;
            }
            if (f == 0x02)
            {
                dest = 0;
                return 2;
            }
            dest = 0xFF;
            return 0;
        }

        protected byte UpdateBits(ref byte dest, byte src, byte pos, byte numbits)
        {
            byte f = src;
            byte mask = (byte)((1 << numbits) - 1);
            f >>= pos;
            f &= mask;
            if (f < (mask - 1))
            {
                dest = f;
                return 1;
            }
            if (f < mask)
            {
                dest = 0;
                return 2;
            }
            dest = 0xFF;
            return 0;
        }

        protected byte UpdateByte(ref byte dest, byte src)
        {
            if (src <= RVC_BYTE(RVC_MAXVAL))
            {
                dest = src;
                return 1;
            }
            if (src != RVC_BYTE(RVC_NODATA))
            {
                dest = 0;
                return 2;
            }
            return 0;
        }

        protected byte UpdateWord(ref ushort dest, byte[] data, byte pos)
        {
            ushort s = (ushort)((data[pos + 1] << 8) | (data[pos]));
            if (s <= RVC_WORD(RVC_MAXVAL))
            {
                dest = s;
                return 1;
            }
            if (s != RVC_WORD(RVC_NODATA))
            {
                dest = 0;
                return 2;
            }
            dest = s;
            return 0;
        }

        protected byte UpdateUint(ref uint dest, byte[] data, byte pos)
        {
            uint s = (uint)((data[pos + 3] << 24) | (data[pos + 2] << 16) | (data[pos + 1] << 8) | (data[pos]));
            if (s <= RVC_LONG(RVC_MAXVAL))
            {
                dest = s;
                return 1;
            }
            if (s != RVC_LONG(RVC_NODATA))
            {
                dest = 0;
                return 2;
            }
            dest = s;
            return 0;
        }

        #endregion //Update Methods
    }

    //Standard, flexible Datum types

    #region TSPNFlag
    public class TSPNFlag : TSPNDatum
    {
        protected byte byteIndex, bitIndex;

        public TSPNFlag(ushort spn, byte byte_index, byte bit_index) : base(spn)
        {
            byteIndex = byte_index;
            bitIndex = bit_index;
        }

        public override void Parse(byte address, byte[] data)
        {
            byte b = 0;
            if (UpdateByte(ref b, data[byteIndex]) != 0)
            {
                b >>= byteIndex;
                rawValue = (byte)(b & 0x03);
                ConvertAndStore();
            }
        }

        protected override void ConvertAndStore()
        {
            value = valueMetric = rawValue;
            base.ConvertAndStore();
        }
    }
    #endregion //TSPNFlag

    #region TSPNBits
    public class TSPNBits : TSPNDatum
    {
        protected byte length;
        static readonly byte[] bitmask = { 0x00, 0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F };
        protected byte byteIndex, bitIndex;

        public TSPNBits(ushort spn, byte byte_index, byte bit_index, byte bit_length) : base(spn)
        {
            byteIndex = byte_index;
            bitIndex = bit_index;
            length = bit_length;
        }

        public override void Parse(byte address, byte[] data)
        {
            byte b = 0;
            if (UpdateBits(ref b, data[byteIndex], bitIndex, length) != 0)
            {
                rawValue = (uint)(b & bitmask[length]);
                ConvertAndStore();
            }
        }

        protected override void ConvertAndStore()
        {
            base.ConvertAndStore();
            value = valueMetric = rawValue;
        }
    }
    #endregion //TSPNBits

    #region TSPNByte
    public class TSPNByte : TSPNDatum
    {
        protected byte byteIndex;
        protected double scale, offset, metricScale, metricOffset;

        public TSPNByte(ushort spn, byte byte_index, double scale, double offset, double metric_scale, double metric_offset) : base(spn)
        {
            byteIndex = byte_index;
            this.scale = scale;
            this.offset = offset;
            metricScale = metric_scale;
            metricOffset = metric_offset;
        }

        public override void Parse(byte address, byte[] data)
        {
            byte b = 0;
            if (UpdateByte(ref b, data[byteIndex]) != 0)
            {
                rawValue = b;
                ConvertAndStore();
            }
        }

        protected override void ConvertAndStore()
        {
            if (recipNum != 0.0)
            {
                value = rawValue * scale + offset;
                valueMetric = (recipNum / value); //right now only instant mpg recent and rolling mpg use this
                value = valueMetric; //this only applies to gauges using L/100Km
            }
            else
            {
                value = rawValue * scale + offset;
                valueMetric = rawValue * metricScale + metricOffset;
            }
            base.ConvertAndStore();
        }
    }
    #endregion //TSPNByte

    #region TSPNWord
    public class TSPNWord : TSPNByte
    {

        public TSPNWord(ushort spn, byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
            : base(spn, byte_index, scale, offset, metric_scale, metric_offset)
        {
            recipNum = 0.0;
        }

        public TSPNWord(ushort spn, byte byte_index, double scale, double offset, double metric_scale, double metric_offset, double recip_numerator)
            : base(spn, byte_index, scale, offset, metric_scale, metric_offset)
        {
            recipNum = recip_numerator;
        }

        public override void Parse(byte address, byte[] data)
        {
            ushort temp = 0;
            if (UpdateWord(ref temp, data, byteIndex) != 0)
            {
                rawValue = temp;
                ConvertAndStore();
            }
        }
    }
    #endregion //TSPNWord

    #region TSPNUint
    public class TSPNUint : TSPNByte
    {
        public TSPNUint(ushort spn, byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
             : base(spn, byte_index, scale, offset, metric_scale, metric_offset) { }

        public override void Parse(byte address, byte[] data)
        {
            uint temp = 0;
            if (UpdateUint(ref temp, data, byteIndex) != 0)
            {
                rawValue = temp;
                ConvertAndStore();
            }
        }
    }
    #endregion //TSPNUint

    //Special Datum types

    #region TSPNRange

    public class TSPNRange : TSPNDatum
    {
        public TSPNRange(ushort spn) : base(spn) {}

        public override void Parse(byte address, byte[] data)
        {
            if (data[4] < 250)
                ChassisParam.rangeSelected = "" + (char)data[4];
            if (data[3] < 125)
                ChassisParam.rangeAttained = "R";
            else if (data[3] == 125)
                ChassisParam.rangeAttained = "N";
            else if (data[3] < 135)
                ChassisParam.rangeAttained = (char)(data[3] - 125) + "0";
            else if (data[3] < 161)
                ChassisParam.rangeAttained = (char)(data[3] - 135) + "a";
        }
    }
    #endregion TSPNRange

    #region TSPNTransMode

    public class TSPNTransMode : TSPNDatum
    {
        public TSPNTransMode(ushort spn) : base(spn) { }

        public override void Parse(byte address, byte[] data)
        {
            byte temp = data[5];
            if (temp == 16 || temp == 21 || temp == 32)
            {
                rawValue = 1;
                ChassisParam.mode = 1;
                ConvertAndStore();
                seen = true;
            }
            else if (temp == 5 || temp == 26 || temp == 31)
            {
                rawValue = 2;
                ConvertAndStore();
                seen = true;
            }
        }
    }

    #endregion //TSPNTransMode

    #region TSPNRetarder

    public class TSPNRetarder : TSPNDatum
    {
        public TSPNRetarder(ushort spn) : base(spn) { }

        public override void Parse(byte address, byte[] data)
        {
            byte b = 0;
            if (UpdateByte(ref b, data[1]) != 0)
            {
                if (b < 125)
                    rawValue = (uint)(250 - b);
                else
                    rawValue = b;
                ConvertAndStore();
            }
        }
    }

    #endregion

    #region TSPNCruise (Cruise status)

    public class TSPNCruise : TSPNDatum
    {
        public byte cruiseStatus;
        public byte cruiseAdjust;
        public string cruiseStat;

        public TSPNCruise(ushort spn) : base(spn) { }

        public override void Parse(byte address, byte[] data)
        {
            byte b = 0;
            ChassisParam.cruiseAdjust = 0;
            if (UpdateFlag(ref b, data[3], 0) != 0)
            {
                cruiseStatus = (byte)((cruiseStatus & 0x01) | ((b != 0) ? 2 : 0));
                if (b == 0)
                    ChassisParam.cruiseStat = "Off";
                else
                    ChassisParam.cruiseStat = "On";
            }
            if (UpdateFlag(ref b, data[3], 2) != 0)
            {
                ChassisParam.cruiseStatus = (byte)((ChassisParam.cruiseStatus & 0x02) | ((b != 0) ? 1 : 0));
                ChassisParam.cruiseStat = "Set";
            }
            for (byte i = 0; i < 8; i += 2)
            {
                if (UpdateFlag(ref b, data[4], i) != 0)
                    ChassisParam.cruiseAdjust |= b;
            }
        }
    }

    #endregion

    //TSPNOdometer, TSPNHourMeter, and TSPNFuelMeter inherit from this datum
    #region TSPNInferred (inferred values that get stored in the ChassisParam)

    public class TSPNInferred : TSPNUint
    {
        public double lastVal;
        protected uint maxVal;

        public TSPNInferred(ushort spn, byte byte_index, double scale, double offset, double metric_scale, double metric_offset, uint max_val) 
            : base(spn, byte_index, scale, offset, metric_scale, metric_offset)
        {
            lastVal = 0.0;
            maxVal = max_val;
        }

        public override void Parse(byte address, byte[] data)
        {
            uint b = 0;
            if (UpdateUint(ref b, data, 0) != 0)
            {
                double temp = b * scale;
                if ((temp > lastVal) && (temp < maxVal))
                {
                    rawValue = b;
                    ConvertAndStore();
                    lastVal = value;
                }
            }
        }
    }

    #endregion //TSPNInferred

    //TODO
    #region TSPNDiag (diagnostics)

    public class TSPNDiag : TSPNDatum
    {
        public TSPNDiag(ushort spn) : base(spn)
        {

        }
    }

    #endregion //TSPNDiag (diagnostics)



}
