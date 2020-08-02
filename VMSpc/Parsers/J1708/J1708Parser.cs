using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;
using VMSpc.DevHelpers;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.Constants;
using VMSpc.Enums.Parsing;

namespace VMSpc.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    class J1708Parser : IDataBus
    {
        private Dictionary<ushort, J1708ParsingHelper> J1708ParseMethodMap;

        public J1708Parser()
        {
            J1708ParseMethodMap = new Dictionary<ushort, J1708ParsingHelper>();
            DefineParsers();
        }

        public void Parse(CanMessage canMessage)
        {
            J1708Message j1708Message = (canMessage as J1708Message);
            foreach (CanMessageSegment segment in canMessage.CanMessageSegments)
            {
                if (J1708ParseMethodMap.ContainsKey(segment.Pid))
                {
                    J1708ParseMethodMap[segment.Pid].ParseMethod(segment, J1708ParseMethodMap[segment.Pid]);
                }
                else
                {
                    CreateUnsupportedResponse(segment);
                }
            }
        }

        private void StandardConverter(CanMessageSegment segment, J1708ParsingHelper conversionStruct)
        {
            var standardValue = segment.RawValue * conversionStruct.standardMultiplier + conversionStruct.standardOffset;
            var metricValue = standardValue * conversionStruct.metricMultiplier + conversionStruct.metricOffset;
            segment.StandardValue = standardValue;
            segment.MetricValue = metricValue;
            conversionStruct.lastValue = standardValue;
            segment.ParseStatus = conversionStruct.ParseStatus;
        }

        private void RetarderSwitchConverter(CanMessageSegment segment, J1708ParsingHelper conversionStruct)
        {
            double value = 0;
            switch (segment.RawValue)
            {
                case 129:
                    value = 1;
                    break;
                case 130:
                case 132:
                    value = 2;
                    break;
                case 136:
                case 144:
                    value = 3;
                    break;
            }
            segment.StandardValue = segment.MetricValue = value;
            segment.ParseStatus = ParseStatus.Parsed;
        }

        private void RetarderStatusConverter(CanMessageSegment segment, J1708ParsingHelper conversionStruct)
        {
            double value = 0;
            switch (segment.RawValue)
            {
                case 129:
                    value = 1;
                    break;
                case 130:
                case 132:
                    value = 2;
                    break;
                case 136:
                case 144:
                    value = 3;
                    break;
            }
            segment.StandardValue = segment.MetricValue = value;
            segment.ParseStatus = ParseStatus.Parsed;
        }

        private void CruiseSetStatusConverter(CanMessageSegment segment, J1708ParsingHelper conversionStruct)
        {
            if (((byte)segment.RawValue & 0x81b) == 0x81)
            {
                segment.StandardValue = segment.MetricValue = 2;    //"Set"
            }
            else if (((byte)segment.RawValue & 0x01) == 1)
            {
                segment.StandardValue = segment.MetricValue = 1;    //"On"
            }
            else
            {
                segment.StandardValue = segment.MetricValue = 0;    //"Off"
            }
            segment.ParseStatus = ParseStatus.Parsed;
        }

        private void CreateUnsupportedResponse(CanMessageSegment segment)
        {
            segment.StandardValue = segment.MetricValue = segment.RawValue;
            segment.ParseStatus = ParseStatus.NotParsed;
            segment.DataSource = VMSDataSource.J1708;
        }

        public void SendMessage(byte[] message)
        {
            byte check = 0;
            for (int i = 0; i < message.Length; i++)
            {
                check += message[i];
            }
        }


        #region Map Definition Methods

        private void DefineParsers()
        {
            /*-------------------------   These fit standard conversion formats ---------------- */
            //Single Byte Converters
            J1708ParseMethodMap.Add(retarderOilPressure,           new J1708ParsingHelper(StandardConverter,   0, 0,      0.6,   6.895));
            J1708ParseMethodMap.Add(retarderOilTemp,               new J1708ParsingHelper(StandardConverter,   0, -17.77, 2.0,   0.555));
            J1708ParseMethodMap.Add(percentAcceleratorPosition,    new J1708ParsingHelper(StandardConverter,   0, 0,      0.4,   1    ));
            J1708ParseMethodMap.Add(cruiseSpeed,                   new J1708ParsingHelper(StandardConverter,   0, 0,      0.5,   1.609));
            J1708ParseMethodMap.Add(coolantTemp,                   new J1708ParsingHelper(StandardConverter,   0, -17.7,  1,     0.555));
            J1708ParseMethodMap.Add(engineLoad,                    new J1708ParsingHelper(StandardConverter,   0, 0,      0.5,   1    ));
            J1708ParseMethodMap.Add(intakeTemp,                    new J1708ParsingHelper(StandardConverter,   0, -17.77, 1,     0.555));
            J1708ParseMethodMap.Add(oilPSI,                        new J1708ParsingHelper(StandardConverter,   0, 0,      0.5,   6.895));
            J1708ParseMethodMap.Add(retarderPercent,               new J1708ParsingHelper(StandardConverter,   0, 0,      0.5,   1    ));
            J1708ParseMethodMap.Add(roadSpeed,                     new J1708ParsingHelper(StandardConverter,   0, 0,      0.5,   1.609));
            J1708ParseMethodMap.Add(turboBoost,                    new J1708ParsingHelper(StandardConverter,   0, 0,      0.125, 6.895));

            //Double Byte Converters
            J1708ParseMethodMap.Add(oilTemp,                       new J1708ParsingHelper(StandardConverter, 0, -17.77, 0.25,         0.555  ));
            J1708ParseMethodMap.Add(engineSpeed,                   new J1708ParsingHelper(StandardConverter, 0, 0,      0.25,         1      ));
            J1708ParseMethodMap.Add(transmissionTemp,              new J1708ParsingHelper(StandardConverter, 0, -17.77, 0.25,         0.555  ));
            J1708ParseMethodMap.Add(transmissionSpeed,             new J1708ParsingHelper(StandardConverter, 0, 0,      0.25,         1      ));
            J1708ParseMethodMap.Add(fuelRate,                      new J1708ParsingHelper(StandardConverter, 0, 0,      1,            1      ));
            J1708ParseMethodMap.Add(fuelTemp,                      new J1708ParsingHelper(StandardConverter, 0, 0,      1,            1      ));
            J1708ParseMethodMap.Add(instantMPG,                    new J1708ParsingHelper(StandardConverter, 0, 0,      0.00390625,   0.425  ));
            J1708ParseMethodMap.Add(airInletTemp,                  new J1708ParsingHelper(StandardConverter, 0, -17.77, 0.25,         0.555  ));
            J1708ParseMethodMap.Add(voltage,                       new J1708ParsingHelper(StandardConverter,   0, 0,      0.05,       1      ));
            J1708ParseMethodMap.Add(ambientTemp,                   new J1708ParsingHelper(StandardConverter, -459.4, -273.0, 0.05625, 0.03125));

            J1708ParseMethodMap.Add(totalMilesCummins, new J1708ParsingHelper(StandardConverter, 0, 0, 0.1, 1) { ParseStatus = ParseStatus.PartiallyParsed });

            J1708ParseMethodMap.Add(totalMiles, new J1708ParsingHelper(StandardConverter, 0, 0, 0.1, 1) { ParseStatus = ParseStatus.PartiallyParsed });
            J1708ParseMethodMap.Add(engineHours, new J1708ParsingHelper(StandardConverter, 0, 0, 0.05, 1) { ParseStatus = ParseStatus.PartiallyParsed });
            J1708ParseMethodMap.Add(totalFuel, new J1708ParsingHelper(StandardConverter, 0, 0, 0.125, 1) { ParseStatus = ParseStatus.PartiallyParsed });

            //These require custom conversions
            J1708ParseMethodMap.Add(retarderSwitch, new J1708ParsingHelper(RetarderSwitchConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(retarderStatus, new J1708ParsingHelper(RetarderStatusConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(cruiseSetStatus, new J1708ParsingHelper(CruiseSetStatusConverter, 0, 0, 0, 0));
        }

        #endregion Map Definition Methods

        public class J1708ParsingHelper
        {
            public Action<CanMessageSegment, J1708ParsingHelper> ParseMethod;
            public double standardOffset;
            public double metricOffset;
            public double standardMultiplier;
            public double metricMultiplier;
            public double lastValue;
            public ParseStatus ParseStatus = ParseStatus.Parsed;

            public J1708ParsingHelper(Action<CanMessageSegment, J1708ParsingHelper> parseMethod, double soffset, double mooffset, double smultiplier, double mmultiplier)
            {
                ParseMethod = parseMethod;
                standardOffset = soffset;
                metricOffset = mooffset;
                standardMultiplier = smultiplier;
                metricMultiplier = mmultiplier;
                this.lastValue = 0;
            }
        }
    }
}
