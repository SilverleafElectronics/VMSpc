using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMSpc.Extensions.Standard;

namespace VMSpc.JsonFileManagers
{
    public class ColorContents : IJsonContents
    {
        public Dictionary<string, Color> ColorDictionary;
    }
    public class ColorReader : JsonFileReader<ColorContents>
    {
        public ColorReader()
            : base("\\configuration\\colors.json")
        {
        }
       
        protected override ColorContents GetDefaultContents()
        {
            return new ColorContents()
            {
                ColorDictionary = GetFrameworkColors()
            };
        }

        protected Dictionary<string, Color> GetFrameworkColors()
        {
            var Colors = new Dictionary<string, Color>();
            Type type = typeof(Colors);
            foreach (var p in type.GetProperties())
            {
                if (p.PropertyType.Name == "Color")
                {
                    string colorName = p.Name.PascalCaseToReadable();
                    Color colorValue = (Color)p.GetValue(null);
                    Colors.Add(colorName, colorValue);
                }
            }
            return Colors;
        }
    }
}
