using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Constants;

namespace VMSpc.JsonFileManagers
{
    public class PidValue
    {
        public double RawValue { get; set; }
        public double StandardValue { get; set; }
        public double MetricValue { get; set; }
        public DateTime TimeReceived { get; set; }
    }
    public class JParameter
    {
        public string ParamName;
        public string Abbreviation;
        public string Unit;
        public string MetricUnit;
        public ushort Pid;
        public double GaugeMin;
        public double GaugeMax;
        public double LowYellow;
        public double LowRed;
        public double HighYellow;
        public double HighRed;
        public string Format;
        public byte DecimalCount;
        public double Offset;
        public double Multiplier;
        public PidValue J1708Value;
        public PidValue J1939Value;
        public double LastValue; //(PresenterList[Pid].datum.seen) ? PresenterList[Pid].datum.value * Multiplier + Offset : DUB_NODATA;
        public double LastMetricValue; //=> (PresenterList[Pid].datum.seen) ? PresenterList[Pid].datum.valueMetric : DUB_NODATA;
    }
    public class ParameterContents : IJsonContents
    {
        public List<JParameter> Parameters;
    }
    public class ParamDataReader : JsonFileReader<ParameterContents>
    {
        public ParamDataReader() : base("\\configuration\\ParamData.json")
        {
            ConvertStringFormatters();
        }

        private void ConvertStringFormatters()
        {
        }

        protected override ParameterContents GetDefaultContents()
        {
            return JsonConvert.DeserializeObject<ParameterContents>(JsonDefaults.ParamDataDefaults.defaults, serializerSettings);
        }

        public JParameter GetParam(ushort pid)
        {
            foreach (JParameter param in Contents.Parameters)
            {
                if (param.Pid == pid)
                    return param;
            }
            return null;
        }

        public void AddParam(JParameter parameter)
        {
            //TODO
        }

        public void ProcessUpdates(JParameter parameter)
        {
            if (parameter.DecimalCount > 0)
            {
                parameter.Format = "{0:0.";
                for (int i = 0; i < parameter.DecimalCount; i++)
                    parameter.Format += "#";
                parameter.Format += "}";
            }
            else
            {
                parameter.Format = "{0: 0}";
            }
        }

        public void ChangeParameterPID(JParameter parameter)
        {
            //TODO
        }

        public double GetConvertedValue(JParameter param, double value)
        {
            return ((value * param.Multiplier) + param.Offset);
        }
    }
}
