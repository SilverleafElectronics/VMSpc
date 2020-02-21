using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VMSpc.UI
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        private Dictionary<string, Color> ColorDictionary;
        public ColorPicker()
        {
            ColorDictionary = new Dictionary<string, Color>();

            //reflect over all properties in Colors (the only property types are colors), and
            //add them to the ColorDictionary
            Type type = typeof(Colors);
            foreach (var p in type.GetProperties())
            {
                string colorName = p.Name;
                Color colorValue = (Color)p.GetValue(null);
                ColorDictionary.Add(colorName, colorValue);
            }
            InitializeComponent();
        }
    }
}
