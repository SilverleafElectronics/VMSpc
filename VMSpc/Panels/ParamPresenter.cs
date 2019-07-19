using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static VMSpc.XmlFileManagers.ParamDataManager;
using VMSpc.XmlFileManagers;

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
            lastValue = Double.NaN;
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
            ((showUnit) ? ((UseMetric) ? parameter.Unit : parameter.MetricUnit) : "")
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
            if (!HasNewValue())
                return false;
            if (currentValue >= parameter.GaugeMin && currentValue <= parameter.GaugeMax)
                return true;
            return false;
        }

        private double ValueToPercent(double value, double max, double min)
        {
            double span = max - min;
            return (value - min) / span;
        }

        /// <summary> Returns the current value as a percentage of the difference between gaugeMin and gaugeMax. Useful for visual presentation of the value in gauges </summary>
        public double ValueAsPercent()
        {
            return ValueToPercent(currentValue, parameter.GaugeMax, parameter.GaugeMin);
        }
        public double GreenMaxAsPercent  =>  ValueToPercent(parameter.HighYellow, parameter.GaugeMax, parameter.GaugeMin);
        public double YellowMaxAsPercent => ValueToPercent(parameter.HighRed, parameter.GaugeMax, parameter.GaugeMin);
        public double RedMaxAsPercent    => ValueToPercent(parameter.GaugeMax, parameter.GaugeMax, parameter.GaugeMin);
        
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
