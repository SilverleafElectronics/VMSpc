using ColorPickerWPF;
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

namespace VMSpc.UI.CustomComponents
{
    /// <summary>
    /// Interaction logic for ColorEditor.xaml
    /// </summary>
    public partial class ColorEditor : Window
    {
        public string ColorName { get; set; }
        public Color Color { get; set; }
        public ColorEditor(string ColorName, Color Color)
        {
            this.ColorName = ColorName;
            this.Color = Color;
            InitializeComponent();
            ColorNameBlock.Text = this.ColorName;
            ColorRect.Fill = new SolidColorBrush(this.Color);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            ColorName = ColorNameBlock.Text;
        }

        private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = this.Color;
            bool changed = ColorPickerWindow.ShowDialog(out color);
            if (changed)
            {
                this.Color = color;
                ColorRect.Fill = new SolidColorBrush(this.Color);
            }
        }
    }
}
