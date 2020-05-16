using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static VMSpc.Constants;


namespace VMSpc.DevHelpers
{
#if (DEBUG)
    static class VMSConsole
    {
        private static DebugConsole console;

        static VMSConsole()
        {
            
        }

        public static void InitializeConsoleHelpers(DebugConsole console)
        {
            VMSConsole.console = console;
        }

        public static void PrintLine(string logItem)
        {
            console.AddLine(logItem);
        }
        public static void PrintLine(object obj)
        {
            PrintLine(obj.ToString());
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
        public static void InitializeConsoleHelpers(DebugConsole console) { }
        public static void AddConsoleToWindow(Canvas wrapper){}
        public static void PrintLine(string logItem) { }
        public static void PrintSide(double side) { }
    }

#endif //DEBUG_CONSOLE
}
