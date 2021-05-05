using System.Collections.Generic;
using VMSpc.UI.TireMaps;

namespace VMSpc.JsonFileManagers
{
    public class TireMapContents : IJsonContents
    {
        public List<TireCell> TireMap;
    }
    public class TireMapReader : JsonFileReader<TireMapContents>
    {
        public TireMapReader() : base("\\configuration\\TireMap.json")
        {
        }

        protected override TireMapContents GetDefaultContents()
        {
            return new TireMapContents()
            {
                TireMap = new List<TireCell>()
                {
                    new TireCell(0, 0, 0),
                    new TireCell(1, 0, 3),
                    new TireCell(2, 1, 0),
                    new TireCell(3, 1, 1),
                    new TireCell(4, 1, 2),
                    new TireCell(5, 1, 3),
                    new TireCell(7, 2, 0),
                    new TireCell(8, 2, 3),
                }
            };
        }
    }
}
