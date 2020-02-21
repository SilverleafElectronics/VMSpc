using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace VMSpc.UI.TireMaps
{
    public static class TireMap
    {
        public static List<TireCell> InitializeTireCellMap(TireMapType tireMapType)
        {
            List<TireCell> tireCells = new List<TireCell>();
            switch (tireMapType)
            {
                case TireMapType.SIX_WHEEL:
                    return new List<TireCell>()
                    {
                        new TireCell(0, 0, 0),
                        new TireCell(1, 0, 3),
                        new TireCell(2, 1, 0),
                        new TireCell(3, 1, 1),
                        new TireCell(4, 1, 2),
                        new TireCell(5, 1, 3),
                    };
                case TireMapType.EIGHT_WHEEL:
                    return new List<TireCell>()
                    {
                        new TireCell(0, 0, 0),
                        new TireCell(1, 0, 3),
                        new TireCell(2, 1, 0),
                        new TireCell(3, 1, 1),
                        new TireCell(4, 1, 2),
                        new TireCell(5, 1, 3),
                        new TireCell(7, 2, 0),
                        new TireCell(8, 2, 3),
                    };
                case TireMapType.SIX_PLUS_4:
                    return new List<TireCell>()
                    {
                        new TireCell(0, 0, 0),
                        new TireCell(1, 0, 3),
                        new TireCell(2, 1, 0),
                        new TireCell(3, 1, 1),
                        new TireCell(4, 1, 2),
                        new TireCell(5, 1, 3),
                        new TireCell(6, 2, 1),
                        new TireCell(7, 2, 2),
                        new TireCell(8, 3, 1),
                        new TireCell(9, 3, 2),
                    };
                case TireMapType.EIGHT_PLUS_4:
                    return new List<TireCell>()
                    {
                        new TireCell(0, 0, 0),
                        new TireCell(1, 0, 3),
                        new TireCell(2, 1, 0),
                        new TireCell(3, 1, 1),
                        new TireCell(4, 1, 2),
                        new TireCell(5, 1, 3),
                        new TireCell(7, 2, 0),
                        new TireCell(8, 2, 3),
                        new TireCell(9, 3, 1),
                        new TireCell(10, 3, 2),
                        new TireCell(11, 4, 1),
                        new TireCell(12, 4, 2),
                    };
                default:
                    return new List<TireCell>() { };
            }
        }
    }

    public enum TireMapType
    {
        [Description("6 Wheel")]
        SIX_WHEEL = 0,
        [Description("8 Wheel")]
        EIGHT_WHEEL = 1,
        [Description("6 + 4 Wheel")]
        SIX_PLUS_4 = 2,
        [Description("8 + 4 Wheel")]
        EIGHT_PLUS_4 = 3,
        [Description("None")]
        NONE = 4,
    }


    public struct TireCell
    {
        public TireCell(int index, int row, int column)
        {
            this.index = index;
            this.row = row;
            this.column = column;
        }
        public readonly int row;
        public readonly int column;
        public readonly int index;
    }
}
