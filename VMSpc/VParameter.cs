using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static VMSpc.Parsers.PresenterWrapper;
using static VMSpc.Constants;

namespace VMSpc
{
    public class VParameter
    {
        private XmlNode node;

        public string ParamName
        {
            get { return node.Attributes["Name"].Value; }
            set { node.Attributes["Name"].Value = value; }
        }
        public string Abbreviation
        {
            get { return node["Abbreviation"].InnerText; }
            set { node["Abbreviation"].InnerText = value; }
        }
        public string Unit
        {
            get { return node["Unit"].InnerText; }
            set { node["Unit"].InnerText = value; }
        }
        public string MetricUnit
        {
            get { return node["Metric-Unit"].InnerText; }
            set { node["Metric-Unit"].InnerText = value; }
        }
        public ushort Pid
        {
            get { return UInt16.Parse(node["PID"].InnerText); }
            set { node["PID"].InnerText = value.ToString(); }
        }
        public int GaugeMin
        {
            get { return Int32.Parse(node["Gauge-Min"].InnerText); }
            set { node["Gauge-Min"].InnerText = value.ToString();  }
        }
        public int GaugeMax
        {
            get { return Int32.Parse(node["Gauge-Max"].InnerText); }
            set { node["Gauge-Max"].InnerText = value.ToString(); }
        }
        public int LowYellow
        {
            get { return Int32.Parse(node["Low-Yellow"].InnerText); }
            set { node["Low-Yellow"].InnerText = value.ToString(); }
        }
        public int LowRed
        {
            get { return Int32.Parse(node["Low-Red"].InnerText); }
            set { node["Low-Red"].InnerText = value.ToString(); }
        }
        public int HighYellow
        {
            get { return Int32.Parse(node["High-Yellow"].InnerText); }
            set { node["High-Yellow"].InnerText = value.ToString();  }
        }
        public int HighRed
        {
            get { return Int32.Parse(node["High-Red"].InnerText); }
            set { node["High-Red"].InnerText = value.ToString(); }
        }
        public string Format
        {
            get { return node["Format"].InnerText;  }
            set { node["Format"].InnerText = value; }
        }
        public double Offset
        {
            get { return Double.Parse(node["Offset"].InnerText); }
            set { node["Offset"].InnerText = value.ToString(); }
        }
        public double Multiplier
        {
            get { return Double.Parse(node["Multiplier"].InnerText); }
            set { node["Multiplier"].InnerText = value.ToString(); }
        }

        public double LastValue => (PresenterList[Pid].datum.seen) ? PresenterList[Pid].datum.value * Multiplier + Offset : DUB_NODATA;
        public double LastMetricValue => (PresenterList[Pid].datum.seen) ? PresenterList[Pid].datum.valueMetric : DUB_NODATA;

        public VParameter(XmlNode node)
        {
            this.node = node;
        }
    }
}
