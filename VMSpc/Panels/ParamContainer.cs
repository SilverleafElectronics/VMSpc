using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace VMSpc.Panels
{
    public class ParamPresenter
    {
        protected bool UseMetric;
        protected VParameter parameter;
        public double lastValue;
        public double currentValue => ((UseMetric) ? parameter.LastValue : parameter.LastMetricValue);
        public string valueAsString => (currentValue + ((UseMetric) ? parameter.Unit : parameter.MetricUnit));

        public ParamPresenter(ushort pid, bool UseMetric)
        {
            this.UseMetric = UseMetric;
        }

    }
}
