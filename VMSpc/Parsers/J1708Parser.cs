using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;
using VMSpc.DevHelpers;
using static VMSpc.Parsers.PIDWrapper;

namespace VMSpc.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    class J1708Parser
    {
        private Dictionary<byte, J1708ParsingHelper> J1708ParseMethodMap;

        public J1708Parser()
        {
            J1708ParseMethodMap = new Dictionary<byte, J1708ParsingHelper>();
            DefineParsers();
        }

        public void Parse(J1708Message canMessage)
        {
            foreach (var pair in canMessage.data)
            {
                J1708ParseMethodMap[pair.Key].ParseMethod(pair.Key, pair.Value, J1708ParseMethodMap[pair.Key]);
            }
        }

        #region Standard Converters

        private void ByteConverter(byte PID, byte[] data, J1708ParsingHelper conversionStruct)
        {
            double rVal, mVal, stdVal;
            rVal = data[0];
            stdVal = data[0] * conversionStruct.standardMultiplier + conversionStruct.standardOffset;
            mVal = stdVal * conversionStruct.metricMultiplier + conversionStruct.metricOffset;
            PIDManager.PIDList[PID].standardValue = stdVal;
            PIDManager.PIDList[PID].rawValue = rVal;
            PIDManager.PIDList[PID].metricValue = mVal;
        }

        private void DoubleConverter(byte PID, byte[] data, J1708ParsingHelper conversionStruct)
        {
            double rawValue, metricValue, standardValue;
            rawValue = data[0] + data[1];
            standardValue = data[0] * conversionStruct.standardMultiplier + conversionStruct.standardOffset;
            standardValue += (data[1] * conversionStruct.secondByteMultiplier);
            metricValue = standardValue * conversionStruct.metricMultiplier + conversionStruct.metricOffset;
        }

        #endregion

        #region Custom Converters

        private void CustomConverter(byte PID, byte[] data, J1708ParsingHelper conversionStruct)
        {
            switch (PID)
            {
                case PIDWrapper.rangeSelected:
                    SetRangeSelected(data);
                    break;
                case PIDWrapper.rangeAttained:
                    SetRangeAttained(data);
                    break;
                case PIDWrapper.cruiseSetStatus:
                    SetCruiseSetStatus(data);
                    break;
                case PIDWrapper.totalMilesCummins:
                case PIDWrapper.totalMiles:
                    SetTotalMiles(data);
                    break;
                case PIDWrapper.engineHours:
                    SetEngineHours(data);
                    break;
                case PIDWrapper.totalFuel:
                    SetTotalFuel(data);
                    break;
                case PIDWrapper.diagnosticsPID:
                    Parse1708Diagnostics(data);
                    break;
                case PIDWrapper.multipartMessage:
                    ParseMultipartMessage(data);
                    break;
                default:
                    break;
            }
        }

        private void SetRangeSelected(byte[] data)
        {

        }

        private void SetRangeAttained(byte[] data)
        {

        }

        private void SetCruiseSetStatus(byte[] data)
        {

        }

        private void SetTotalMiles(byte[] data)
        {

        }

        private void SetEngineHours(byte[] data)
        {

        }

        private void SetTotalFuel(byte[] data)
        {

        }

        private void Parse1708Diagnostics(byte[] data)
        {

        }

        private void ParseMultipartMessage(byte[] data)
        {

        }

        #endregion //Custom Converters

        //CHANGEME - move to CanMessage.cs
        private void SplitByPID(Dictionary<byte, List<byte>> PIDDataPair, J1708Message canMessage)
        {
            byte pos = 1;
            int bytes_unprocessed = canMessage.messageLength;
        }

        #region Map Definition Methods

        private void DefineParsers()
        {
            /*-------------------------   These fit standard conversion formats ---------------- */
            //Single Byte Converters
            J1708ParseMethodMap.Add(retarderSwitch,                new J1708ParsingHelper(ByteConverter,   0, 0,      1,     1    ));
            J1708ParseMethodMap.Add(retarderOilPressure,           new J1708ParsingHelper(ByteConverter,   0, 0,      0.6,   6.895));
            J1708ParseMethodMap.Add(retarderOilTemp,               new J1708ParsingHelper(ByteConverter,   0, -17.77, 2.0,   0.555));
            J1708ParseMethodMap.Add(retarderStatus,                new J1708ParsingHelper(ByteConverter,   0, 0,      1,     1    ));
            J1708ParseMethodMap.Add(percentAcceleratorPosition,    new J1708ParsingHelper(ByteConverter,   0, 0,      0.4,   1    ));
            J1708ParseMethodMap.Add(voltage,                       new J1708ParsingHelper(ByteConverter,   0, 0,      0.4,   1    ));
            J1708ParseMethodMap.Add(cruiseSpeed,                   new J1708ParsingHelper(ByteConverter,   0, 0,      0.5,   1.609));
            J1708ParseMethodMap.Add(coolantTemp,                   new J1708ParsingHelper(ByteConverter,   0, -17.7,  1,     0.555));
            J1708ParseMethodMap.Add(engineLoad,                    new J1708ParsingHelper(ByteConverter,   0, 0,      0.5,   1    ));
            J1708ParseMethodMap.Add(intakeTemp,                    new J1708ParsingHelper(ByteConverter,   0, -17.77, 1,     0.555));
            J1708ParseMethodMap.Add(oilPSI,                        new J1708ParsingHelper(ByteConverter,   0, 0,      0.5,   6.895));
            J1708ParseMethodMap.Add(retarderPercent,               new J1708ParsingHelper(ByteConverter,   0, 0,      0.5,   1    ));
            J1708ParseMethodMap.Add(roadSpeed,                     new J1708ParsingHelper(ByteConverter,   0, 0,      0.5,   1.609));
            J1708ParseMethodMap.Add(turboBoost,                    new J1708ParsingHelper(ByteConverter,   0, 0,      0.125, 6.895));

            //Double Byte Converters
            J1708ParseMethodMap.Add(oilTemp,                       new J1708ParsingHelper(DoubleConverter, 0, -17.77, 0.25,        0.555, 64));
            J1708ParseMethodMap.Add(engineSpeed,                   new J1708ParsingHelper(DoubleConverter, 0, 0,      0.25,        1,     64));
            J1708ParseMethodMap.Add(transmissionTemp,              new J1708ParsingHelper(DoubleConverter, 0, -17.77, 0.25,        0.555, 64));
            J1708ParseMethodMap.Add(transmissionSpeed,             new J1708ParsingHelper(DoubleConverter, 0, 0,      0.25,        1,     64));
            J1708ParseMethodMap.Add(fuelRate,                      new J1708ParsingHelper(DoubleConverter, 0, 0,      1,           1,     4));
            J1708ParseMethodMap.Add(fuelTemp,                      new J1708ParsingHelper(DoubleConverter, 0, 0,      1,           1,     64));
            J1708ParseMethodMap.Add(instantMPG,                    new J1708ParsingHelper(DoubleConverter, 0, 0,      0.00390625,  0.425, 1));
            J1708ParseMethodMap.Add(airInletTemp,                  new J1708ParsingHelper(DoubleConverter, 0, -17.77, 0.25,        0.555, 64));

            /*-------------------------   These require custom conversions ---------------- */
            J1708ParseMethodMap.Add(rangeSelected,                 new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(rangeAttained,                 new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(cruiseSetStatus,               new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(totalMilesCummins,             new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(totalMiles,                    new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(engineHours,                   new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(totalFuel,                     new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(diagnosticsPID,                new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
            J1708ParseMethodMap.Add(multipartMessage,              new J1708ParsingHelper(CustomConverter, 0, 0, 0, 0));
        }

        #endregion //Map Definition Methods

        public struct J1708ParsingHelper
        {
            public Action<byte, byte[], J1708ParsingHelper> ParseMethod;
            public double standardOffset;
            public double metricOffset;
            public double standardMultiplier;
            public double metricMultiplier;
            public double secondByteMultiplier;

            public J1708ParsingHelper(Action<byte, byte[], J1708ParsingHelper> parseMethod, double soffset, double mooffset, double smultiplier, double mmultiplier)
            {
                ParseMethod = parseMethod;
                standardOffset = soffset;
                metricOffset = mooffset;
                standardMultiplier = smultiplier;
                metricMultiplier = mmultiplier;
                secondByteMultiplier = 1;
            }

            public J1708ParsingHelper(Action<byte, byte[], J1708ParsingHelper> parseMethod, double soffset, double mooffset, double smultiplier, double mmultiplier, double secMultiplier)
                : this(parseMethod, soffset, mooffset, smultiplier, mmultiplier)
            {
                secondByteMultiplier = secMultiplier;
            }
        }
    }
}
