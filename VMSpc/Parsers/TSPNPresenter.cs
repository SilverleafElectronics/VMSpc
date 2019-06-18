using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.Constants;

namespace VMSpc.Parsers
{
    public class TSPNPresenter
    {
        public TSPNDatum datum;
        public string title;
        public uint index;


        public TSPNPresenter(TSPNDatum spn_datum, string title, uint index)
        {
            datum = spn_datum;
            this.title = title;
            this.index = index;
        }

        public bool Seen()
        {
            return datum.seen;
        }
        public bool SeenOnJ1708()
        {
            return false;
        }
    }

    public class TSPNPresenterLongFloat : TSPNPresenter
    {
        public byte decimals;
        public bool metricDefault;
        public TSPNPresenterLongFloat(TSPNDatum spn_datum, string title, uint index, byte decimals, bool metric_default)
            : base(spn_datum, title, index)
        {
            this.decimals = decimals;
            metricDefault = metric_default;
        }
    }

    public class TSPNPresenterFloat : TSPNPresenterLongFloat
    {
        public string unitName, metricUnitName;
        public TSPNPresenterFloat(TSPNDatum spn_datum, string title, uint index, string unit_name, string metric_unit_name, byte decimals, bool metric_default)
            : base(spn_datum, title, index, decimals, metric_default)
        {
            unitName = unit_name;
            metricUnitName = metric_unit_name;
        }
    }

    public class TSPNPresenterEnum : TSPNPresenter
    {
        Action<string, byte> enumProc;
        public TSPNPresenterEnum(TSPNDatum spn_datum, string title, uint index, Action<string, byte>enum_proc)
            : base(spn_datum, title, index)
        {
            enumProc = enum_proc;
        }
    }


}
