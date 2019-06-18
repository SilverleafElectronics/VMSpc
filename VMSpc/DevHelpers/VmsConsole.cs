//#define DEBUG_CONSOLE //comment out for release builds   
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static VMSpc.Constants;


namespace VMSpc.DevHelpers
{
#if (DEBUG_CONSOLE)
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
            Canvas.SetTop(ConsoleScroller, Canvas.GetTop(wrapper) - 50);
        }
        public static void PrintLine(string logItem)
        {
            TextBlock line = new TextBlock();
            line.Text = logItem;
            line.Foreground = Brushes.White;
            Console.Children.Add(line);
            ConsoleScroller.ScrollToBottom();
        }
        public static void PrintSide(double side)
        {
            switch (side)
            {
                case LEFT:
                    PrintLine("Left");
                    break;
                case UP:
                    PrintLine("Up");
                    break;
                case RIGHT:
                    PrintLine("Right");
                    break;
                case DOWN:
                    PrintLine("Down");
                    break;
                default:
                    PrintLine("Not a side");
                    break;
            }
        }
    }

#else

    static class VMSConsole
    {
        static VMSConsole(){ }
        public static void AddConsoleToWindow(Canvas wrapper){}
        public static void PrintLine(string logItem) { }
        public static void PrintSide(double side) { }
    }

#endif //DEBUG_CONSOLE
}
