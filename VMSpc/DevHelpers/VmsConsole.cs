#define DEBUG   //comment out for release builds

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
using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;

namespace VMSpc.DevHelpers
{
#if(DEBUG)
    static class VMSConsole
    {
        private static ScrollViewer ConsoleScroller;
        private static StackPanel Console;
        private static Canvas Wrapper;
        static VMSConsole()
        {

        }
        public static void AddConsoleToWindow(Canvas wrapper)
        {
            Wrapper = wrapper;
            ConsoleScroller = new ScrollViewer();
            ConsoleScroller.Background = Brushes.Black;
            ConsoleScroller.Width = 600.0;
            ConsoleScroller.Height = 200.0;
            ConsoleScroller.VerticalAlignment = VerticalAlignment.Bottom;
            ConsoleScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Console = new StackPanel();
            Console.Background = Brushes.Black;
            ConsoleScroller.Content = Console;
            Wrapper.Children.Add(ConsoleScroller);
        }
        public static void PrintLine(string logItem)
        {
            TextBlock line = new TextBlock();
            line.Text = logItem;
            line.Foreground = Brushes.White;
            Console.Children.Add(line);
            ConsoleScroller.ScrollToBottom();
        }
    }
#endif //DEBUG
}
