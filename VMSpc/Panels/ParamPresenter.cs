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
        public bool showGraph;
        public int textPosition;
        protected VParameter parameter;
        public double lastValue;

        private string unit;
        private string metricUnit;

        public virtual double CurrentValue => ((!UseMetric) ? parameter.LastValue : parameter.LastMetricValue);

        public ParamPresenter(ushort pid, GaugeSettings panelSettings, string alt_unit = "", string alt_mUnit = "")
        {
            parameter = ParamData.parameters[pid];
            lastValue = DUB_NODATA;
            showName = panelSettings.showName;
            UseMetric = panelSettings.showInMetric;
            showUnit = panelSettings.showUnit;
            showValue = panelSettings.showValue;
            showAbbreviation = panelSettings.showAbbreviation;
            textPosition = panelSettings.TextPosition;
            showGraph = panelSettings.showGraph;
            unit = (alt_unit.Length > 0) ? alt_unit : parameter.Unit;
            metricUnit = (alt_mUnit.Length > 0) ? alt_mUnit : parameter.MetricUnit;
        }

        /// <summary> Returns a stringified version of the current value, which conditionally renders the value text (if showValue is true) + the unit text (if showUnit is true) </summary>
        public virtual string ValueAsString => (
            (CurrentValue == DUB_NODATA && showValue)
            ? "No Data" 
            : (
                ((showValue) ? String.Format(parameter.Format, CurrentValue) : "") +
                ((showUnit) ? ((!UseMetric) ? unit : metricUnit) : "")
              )
        );

        public string Title => (showAbbreviation) ? parameter.Abbreviation : parameter.ParamName;

        /// <summary> Checks whether or not the currentValue is a fresh value. Note that calling this method will update lastValue, immediately rendering the data stale </summary>
        protected bool HasNewValue()
        {
            bool retval = (CurrentValue != lastValue);
            lastValue = CurrentValue;
            return retval;
        }

        /// <summary> Indicates whether or not the value should be used for updating the UI. A valid value is both fresh and sits between gaugeMin and gaugeMax </summary>
        public bool IsValidForUpdate()
        {
            return ( HasNewValue() && (CurrentValue >= parameter.GaugeMin) );
        }

        private double ValueToPercent(double value, double min, double max)
        {
            double span = max - min;
            return (value - min) / span;
        }

        /// <summary> Returns the current value as a percentage of the difference between gaugeMin and gaugeMax. Useful for visual presentation of the value in gauges </summary>
        public double ValueAsPercent => ValueToPercent(CurrentValue, parameter.GaugeMin, parameter.GaugeMax);

        public double GreenMaxAsPercent  =>  ValueToPercent(parameter.HighYellow, parameter.GaugeMin, parameter.GaugeMax);
        public double YellowMaxAsPercent => ValueToPercent(parameter.HighRed, parameter.GaugeMin, parameter.GaugeMax);
        public double RedMaxAsPercent    => ValueToPercent(parameter.GaugeMax, parameter.GaugeMin, parameter.GaugeMax);
        
    }

    public class OdometerPresenter : ParamPresenter
    {
        private double startValue;
        public OdometerPresenter(ushort pid, OdometerSettings panelSettings, double startValue, string alt_unit = "", string alt_mUnit = "")
            :base(pid, panelSettings, alt_unit, alt_mUnit)
        {
            this.startValue = startValue;
        }

        public override double CurrentValue => ((!UseMetric) ? parameter.LastValue : parameter.LastMetricValue);
    }
}
