using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static VMSpc.XmlFileManagers.ParamDataManager;

namespace VMSpc.Panels
{
    public class ParamPresenter
    {
        protected bool UseMetric;
        protected bool showValue;
        protected bool showUnit;
        protected VParameter parameter;
        public double lastValue;
        public double currentValue => ((!UseMetric) ? parameter.LastValue : parameter.LastMetricValue);

        public ParamPresenter(ushort pid, bool UseMetric, bool showValue, bool showUnit)
        {
            parameter = ParamData.parameters[pid];
            lastValue = Double.NaN;
            this.UseMetric = UseMetric;
            this.showUnit = showUnit;
        }

        /// <summary> Returns a stringified version of the current value, which conditionally renders the value text (if showValue is true) + the unit text (if showUnit is true) </summary>
        public string ValueAsString => (
            ((showValue) ? currentValue.ToString() : "") +
            ((showUnit) ? ((UseMetric) ? parameter.Unit : parameter.MetricUnit) : "")
        );

        /// <summary> Checks whether or not the currentValue is a fresh value. Note that calling this method will update lastValue, immediately rendering the data stale </summary>
        protected bool HasNewValue()
        {
            bool retval = (currentValue != lastValue);
            lastValue = currentValue;
            return retval;
        }

        /// <summary> Indicates whether or not the value should be used for updating the UI. A valid value is both fresh and sits between gaugeMin and gaugeMax </summary>
        public bool IsValidForUpdate()
        {
            if (!HasNewValue())
                return false;
            if (currentValue >= parameter.GaugeMin && currentValue <= parameter.GaugeMax)
                return true;
            return false;
        }

        /// <summary> Returns the current value as a percentage of the difference between gaugeMin and gaugeMax. Useful for visual presentation of the value in gauges </summary>
        public double ValueAsPercent()
        {
            double gaugeSpan = parameter.GaugeMax - parameter.GaugeMin;
            return (currentValue - parameter.GaugeMin) / gaugeSpan;
        }
    }
}
