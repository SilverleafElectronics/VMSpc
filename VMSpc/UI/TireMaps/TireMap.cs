using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using VMSpc.Enums.UI;

namespace VMSpc.UI.TireMaps
{
    public static class TireMap
    {
        public static List<TireCell> InitializeTireCellMap(TireMapType tireMapType, bool towVehicleDetached)
        {
            List<TireCell> tireCells = new List<TireCell>();
            switch (tireMapType)
            {
                case TireMapType.SIX_WHEEL:
                    tireCells.AddRange(
                        new List<TireCell>()
                        {
                            new TireCell(0, 0, 0),
                            new TireCell(1, 0, 3),
                            new TireCell(2, 1, 0),
                            new TireCell(3, 1, 1),
                            new TireCell(4, 1, 2),
                            new TireCell(5, 1, 3),
                        });
                    break;
                case TireMapType.EIGHT_WHEEL:
                    tireCells.AddRange(
                        new List<TireCell>()
                        {
                            new TireCell(0, 0, 0),
                            new TireCell(1, 0, 3),
                            new TireCell(2, 1, 0),
                            new TireCell(3, 1, 1),
                            new TireCell(4, 1, 2),
                            new TireCell(5, 1, 3),
                            new TireCell(7, 2, 0),
                            new TireCell(8, 2, 3),
                        });
                    break;
                case TireMapType.SIX_PLUS_4:
                    tireCells.AddRange(
                        new List<TireCell>()
                        {
                            new TireCell(0, 0, 0),
                            new TireCell(1, 0, 3),
                            new TireCell(2, 1, 0),
                            new TireCell(3, 1, 1),
                            new TireCell(4, 1, 2),
                            new TireCell(5, 1, 3),
                        });
                    if (!towVehicleDetached)
                    {
                        tireCells.AddRange(
                            new List<TireCell>()
                            {
                                new TireCell(6, 2, 1),
                                new TireCell(7, 2, 2),
                                new TireCell(8, 3, 1),
                                new TireCell(9, 3, 2),
                            });
                    }
                    break;
                case TireMapType.EIGHT_PLUS_4:
                    tireCells.AddRange( 
                        new List<TireCell>()
                        {
                            new TireCell(0, 0, 0),
                            new TireCell(1, 0, 3),
                            new TireCell(2, 1, 0),
                            new TireCell(3, 1, 1),
                            new TireCell(4, 1, 2),
                            new TireCell(5, 1, 3),
                            new TireCell(7, 2, 0),
                            new TireCell(8, 2, 3),
                        });
                    if (!towVehicleDetached)
                    {
                        tireCells.AddRange(
                            new List<TireCell>
                            {
                                new TireCell(9, 3, 1),
                                new TireCell(10, 3, 2),
                                new TireCell(11, 4, 1),
                                new TireCell(12, 4, 2),
                            });
                    }
                    break;
                default:
                    break;
            }
            return tireCells;
        }
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
