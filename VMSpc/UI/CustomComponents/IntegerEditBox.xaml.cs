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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMSpc.DevHelpers;

namespace VMSpc.UI.CustomComponents
{
    /// <summary>
    /// Interaction logic for IntegerEditBox.xaml
    /// </summary>
    public partial class IntegerEditBox : UserControl
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int ExemptValues { get; set; }
        public Orientation Orientation
        {
            get { return IntEditStack.Orientation; }
            set 
            { 
                IntEditStack.Orientation = value;
                //Reset these so the setters are called with the proper orientation basis
                Width = Width;
                Height = Height;
            }
        }
        private int CurrentValue
        {
            get 
            {
                if (Int32.TryParse(ValueBox.Text, out int result))
                    return result;
                else
                    return 0;
            }
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    ValueBox.Text = value.ToString();
                }
            }
        }
        public int Value
        {
            get { return CurrentValue; }
            set 
            {
                try
                {
                    CurrentValue = value;
                }
                catch (Exception)
                {
                }
            }
        }
        public new double Width
        {
            get { return base.Width; }
            set
            {
                if (Orientation == Orientation.Horizontal)
                {
                    base.Width = value;
                    DecrementButton.Width = (value / 5);
                    IncrementButton.Width = (value / 5);
                    ValueBox.Width = (value * (3 / 5));
                }
                else
                {
                    DecrementButton.Width = value;
                    IncrementButton.Width = value;
                    ValueBox.Width = value;
                }
            }
        }
        public new double Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                if (Orientation == Orientation.Vertical)
                {
                    DecrementButton.Height = (value / 5);
                    IncrementButton.Height = (value / 5);
                    ValueBox.Height = (value * (3 / 5));
                }
                else
                {
                    DecrementButton.Height = value;
                    IncrementButton.Height = value;
                    ValueBox.Height = value;
                }
            }
        }
        public IntegerEditBox()
        {
            InitializeComponent();
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentValue--;
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentValue++;
        }
    }
}
