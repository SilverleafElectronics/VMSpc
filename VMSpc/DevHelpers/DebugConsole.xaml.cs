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

namespace VMSpc.DevHelpers
{
    /// <summary>
    /// Interaction logic for DebugConsole.xaml
    /// </summary>
    public partial class DebugConsole : Window
    {
        public DebugConsole()
        {
            VMSConsole.InitializeConsoleHelpers(this);
            InitializeComponent();
        }

        public void AddLine(string logItem)
        {
            TextBlock line = new TextBlock();
            line.Text = logItem;
            line.Foreground = Brushes.White;
            Console.Children.Add(line);
            ConsoleScroller.ScrollToBottom();
        }
    }
}
