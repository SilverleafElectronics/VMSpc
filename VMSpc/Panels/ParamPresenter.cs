using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static VMSpc.XmlFileManagers.ParamDataManager;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;

namespace VMSpc.Panels
{
    public class ParamPresenter
    {
        protected bool UseMetric;
        public bool showValue;
        public bool showUnit;
        public bool showName;
        public bool showAbbreviation;
        public int textPosition;
        protected VParameter parameter;
        public double lastValue;
        public double currentValue => ((!UseMetric) ? parameter.LastValue : parameter.LastMetricValue);

        public ParamPresenter(ushort pid, GaugeSettings panelSettings)
        {
            parameter = ParamData.parameters[pid];
            lastValue = DUB_NODATA;
            showName = panelSettings.showName;
            UseMetric = panelSettings.showInMetric;
            showUnit = panelSettings.showUnit;
            showValue = panelSettings.showValue;
            showAbbreviation = panelSettings.showAbbreviation;
            textPosition = panelSettings.TextPosition;
        }

        /// <summary> Returns a stringified version of the current value, which conditionally renders the value text (if showValue is true) + the unit text (if showUnit is true) </summary>
        public string ValueAsString => (
            ((showValue) ? String.Format(parameter.Format, currentValue) : "") +
            ((showUnit) ? ((!UseMetric) ? parameter.Unit : parameter.MetricUnit) : "")
        );

        public string Title => (showAbbreviation) ? parameter.Abbreviation : parameter.ParamName;

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
            return ( HasNewValue() && (currentValue >= parameter.GaugeMin) );
        }

        private double ValueToPercent(double value, double min, double max)
        {
            double span = max - min;
            return (value - min) / span;
        }

        /// <summary> Returns the current value as a percentage of the difference between gaugeMin and gaugeMax. Useful for visual presentation of the value in gauges </summary>
        public double ValueAsPercent => ValueToPercent(currentValue, parameter.GaugeMin, parameter.GaugeMax);

        public double GreenMaxAsPercent  =>  ValueToPercent(parameter.HighYellow, parameter.GaugeMin, parameter.GaugeMax);
        public double YellowMaxAsPercent => ValueToPercent(parameter.HighRed, parameter.GaugeMin, parameter.GaugeMax);
        public double RedMaxAsPercent    => ValueToPercent(parameter.GaugeMax, parameter.GaugeMin, parameter.GaugeMax);
        
    }

    public class MultiBarPresenter : ParamPresenter
    {
        public bool showGraph;
        public MultiBarPresenter(ushort pid, MultiBarSettings panelSettings)
            :base(pid, panelSettings)
        {
            showGraph = panelSettings.showGraph;
        }
    }
}
