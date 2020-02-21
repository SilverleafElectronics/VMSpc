using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Parsers.PresenterWrapper;
using static VMSpc.Constants;

namespace VMSpc.JsonFileManagers
{
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
        public double Offset;
        public double Multiplier;
        public double LastValue => (PresenterList[Pid].datum.seen) ? PresenterList[Pid].datum.value * Multiplier + Offset : DUB_NODATA;
        public double LastMetricValue => (PresenterList[Pid].datum.seen) ? PresenterList[Pid].datum.valueMetric : DUB_NODATA;
    }
    public class ParameterContents : IJsonContents
    {
        public List<JParameter> Parameters;
    }
    public class ParamDataReader : JsonFileReader<ParameterContents>
    {
        public ParamDataReader() : base(cwd + "/configuration/ParamData.json")
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
    }
}
