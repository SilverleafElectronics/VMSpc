using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMSpc.DevHelpers;
using VMSpc.Common;
using static VMSpc.Common.RVCUpdaters;
using VMSpc.Enums.Parsing;

/// <summary>
/// Update Notes:
///     - Removed parameter index from TSPNDatum. It appeared to have no effect
///     - Removed parselong() from original source. It was never called in 
///       previous versions. Be sure to implement in accordance with the parsers
///       below if necessary.
/// </summary>

namespace VMSpc.Parsers
{
    public abstract class TSPNDatum
    {
        public uint rawValue;
        public double value, valueMetric;
        public double recipNum;
        public bool seen;
        public bool prioritize1708;
        public ushort spn;
        public ParseStatus ParseStatus = ParseStatus.Parsed;
        /// <summary>
        /// If true, new values must be greater than previous values. Otherwise, they will be discarded
        /// </summary>
        public bool enforceIncreasedValues { get; set; }
        protected uint lastRawValue;

        public TSPNDatum(ushort spn)
        {
            rawValue = lastRawValue = 0;
            value = valueMetric = 0.0;
            seen = false;
            prioritize1708 = false;
            this.spn = spn;
        }

        public abstract void Parse(J1939Message message);

        protected virtual void ConvertAndStore(J1939Message message)
        {

            message.CanMessageSegments.Add(
                new J1939MessageSegment()
                {
                    PGN = message.pgn,
                    Pid = spn,
                    SourceAddress = message.address,
                    RawValue = this.rawValue,
                    StandardValue = this.value,
                    MetricValue = this.valueMetric,
                    ParseStatus = this.ParseStatus,
                });
        }
    }

    //Standard, dynamic Datum types
    #region Standard SPNs

    #region TSPNFlag
    public class TSPNFlag : TSPNDatum
    {
        protected byte byteIndex, bitIndex;

        public TSPNFlag(ushort spn, byte byte_index, byte bit_index) : base(spn)
        {
            byteIndex = byte_index;
            bitIndex = bit_index;
        }

        public override void Parse(J1939Message message)
        {
            byte b = 0;
            if (UpdateByte(ref b, message.rawData[byteIndex]) != 0)
            {
                b >>= byteIndex;
                rawValue = (byte)(b & 0x03);
                ConvertAndStore(message);
            }
        }

        protected override void ConvertAndStore(J1939Message message)
        {
            if (!enforceIncreasedValues || rawValue > value)
            {
                value = valueMetric = rawValue;
                base.ConvertAndStore(message);
            }
        }
    }
    #endregion TSPNFlag

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

        public override void Parse(J1939Message message)
        {
            byte b = 0;
            if (UpdateBits(ref b, message.rawData[byteIndex], bitIndex, length) != 0)
            {
                rawValue = (uint)(b & bitmask[length]);
                ConvertAndStore(message);
            }
        }

        protected override void ConvertAndStore(J1939Message message)
        {
            if (!enforceIncreasedValues || rawValue > value)
            {
                value = valueMetric = rawValue;
                base.ConvertAndStore(message);
            }
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

        public override void Parse(J1939Message message)
        {
            byte b = 0;
            if (UpdateByte(ref b, message.rawData[byteIndex]) != 0)
            {
                rawValue = b;
                ConvertAndStore(message);
            }
        }

        protected override void ConvertAndStore(J1939Message message)
        {
            if (rawValue > lastRawValue)
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
                base.ConvertAndStore(message);
                lastRawValue = rawValue;
            }
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

        public override void Parse(J1939Message message)
        {
            ushort temp = 0;
            if (UpdateWord(ref temp, message.rawData.ToArray(), byteIndex) != 0)
            {
                rawValue = temp;
                ConvertAndStore(message);
            }
        }
    }
    #endregion //TSPNWord

    #region TSPNUint
    public class TSPNUint : TSPNByte
    {
        public TSPNUint(ushort spn, byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
             : base(spn, byte_index, scale, offset, metric_scale, metric_offset) { }

        public override void Parse(J1939Message message)
        {
            uint temp = 0;
            if (UpdateUint(ref temp, message.rawData.ToArray(), byteIndex) != 0)
            {
                rawValue = temp;
                ConvertAndStore(message);
            }
        }
    }
    #endregion //TSPNUint

    #endregion //Standard SPNs

    //Special Datum types
    #region Special SPNs

    #region TSPNRetarder

    public class TSPNRetarder : TSPNDatum
    {
        public TSPNRetarder(ushort spn) : base(spn) { }

        public override void Parse(J1939Message message)
        {
            byte b = 0;
            if (UpdateByte(ref b, message.rawData[1]) != 0)
            {
                if (b < 125)
                    rawValue = (uint)(250 - b);
                else
                    rawValue = b;
                ConvertAndStore(message);
            }
        }
    }

    #endregion

    #region TSPNCruiseStatus

    /// <summary>
    /// Takes the raw value and converts it to one of the associative numeric values for CruiseSetStatus: 0 = "Off", 1 = "On", 2 = "Set"
    /// </summary>
    public class TSPNCruise : TSPNDatum
    {
        public TSPNCruise(ushort spn) : base(spn) { }

        public override void Parse(J1939Message message)
        {
            byte b = 0;
            if (UpdateFlag(ref b, message.rawData[3], 0) != 0)
            {
                if (b == 0)
                    rawValue = 0;   //"Off"
                else
                    rawValue = 1;   //"On"
            }
            if (UpdateFlag(ref b, message.rawData[3], 2) != 0)
            {
                rawValue = 2;   //"Set"
            }
            ConvertAndStore(message);
        }
    }

    #endregion TSPNCruiseStatus

    #endregion //Special SPNs

}
