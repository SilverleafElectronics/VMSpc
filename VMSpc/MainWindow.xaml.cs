using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using VMSpc.DevHelpers;
using VMSpc.Communication;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;
using VMSpc.UI.DlgWindows;
using VMSpc.UI.DlgWindows.Advanced;
using VMSpc.UI.CustomComponents;
using VMSpc.UI.DlgWindows.Settings;
using VMSpc.UI.DlgWindows.View;
using System.Windows.Media;
using VMSpc.Common;
//using VMSpc.UI.DlgWindows.Gauges;
//using VMSpc.UI.DlgWindows.Help;
//using VMSpc.UI.DlgWindows.Settings;

namespace VMSpc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IEventConsumer
    {
        //PanelGrid panelGrid;
        public bool forceClose;

        private bool 
            isControlDown,
            isShiftDown,
            isAltDown;
#if DEBUG
        Window debugConsole;
#endif
        //constructor
        public MainWindow()
        {
            forceClose = false;
            isControlDown = isShiftDown = isAltDown = false;
            InitializeComponent();
#if DEBUG
            debugConsole = new DebugConsole();
            debugConsole.Show();
#endif
            GeneratePanels();
            Application.Current.MainWindow = this;
            var contents = ConfigManager.Settings.Contents;
            
            if (contents.WindowLeft > SystemParameters.VirtualScreenWidth)
            {
                contents.WindowLeft = SystemParameters.VirtualScreenWidth / 4;
            }
            if (contents.WindowWidth > SystemParameters.VirtualScreenWidth || contents.WindowWidth < 300)
            { 
                contents.WindowWidth = SystemParameters.VirtualScreenWidth / 2;
            }
            if (contents.WindowTop > SystemParameters.VirtualScreenHeight)
            {
                contents.WindowTop = SystemParameters.VirtualScreenHeight / 4;
            }
            if (contents.WindowHeight > SystemParameters.VirtualScreenHeight || contents.WindowHeight < 300)
            {
                contents.WindowHeight = SystemParameters.VirtualScreenHeight / 2;
            }
            Left = contents.WindowLeft;
            Top = contents.WindowTop;
            Width = contents.WindowWidth;
            Height = contents.WindowHeight;

            EventBridge.Instance.SubscribeToEvent(this, EventIDs.GUI_RESET_EVENT);
        }

        ~MainWindow()
        {

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ProcessKeyDownEvent(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            ProcessKeyUpEvent(e);
        }

        //Add any additional event handlers here. NOTE: this method is not for binding event handlers to 
        //Menu Items. That is handled in the Routed UI Commands (Maps to Custom Event Handlers) section at
        //the bottom of this class. Also see README for more info
        public void ProcessKeyDownEvent(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    ContentGrid.ProcessArrowNavigation(e.Key, isAltDown, isControlDown, isShiftDown);
                    break;
                case Key.LeftAlt:
                case Key.RightAlt:
                    isAltDown = true;
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    isControlDown = true;
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    isShiftDown = true;
                    break;
                default:
                    break;
            }
            switch ((ModifierKeys)e.Key)
            {
                case ModifierKeys.Alt:
                    isAltDown = true;
                    break;
                case ModifierKeys.Control:
                    isControlDown = true;
                    break;
                case ModifierKeys.Shift:
                    isShiftDown = true;
                    break;
                default:
                    break;
            }
        }

        public void ProcessKeyUpEvent(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                    isAltDown = false;
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    isControlDown = false;
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    isShiftDown = false;
                    break;
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    ContentGrid.ProcessArrowRelease();
                    break;
                default:
                    break;
            }
            switch ((ModifierKeys)e.Key)
            {
                case ModifierKeys.Alt:
                    isAltDown = false;
                    break;
                case ModifierKeys.Control:
                    isControlDown = false;
                    break;
                case ModifierKeys.Shift:
                    isShiftDown = false;
                    break;
            }
        }

        public void OnMouseRelease(object sender, RoutedEventArgs e)
        {
            ContentGrid.ProcessMouseRelease();
        }

        private void GeneratePanels()
        {
            ContentGrid.InitPanels(this);
        }

        private void SaveConfig()
        {
            ConfigManager.Settings.Contents.WindowHeight = Height;
            ConfigManager.Settings.Contents.WindowWidth = Width;
            ConfigManager.Settings.Contents.WindowTop = Top;
            ConfigManager.Settings.Contents.WindowLeft = Left;
            

            ConfigManager.Settings.SaveConfiguration();
            ConfigManager.ParamData.SaveConfiguration();
            ConfigManager.Screen.SaveConfiguration();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!forceClose)
                SaveConfig();
#if DEBUG
            debugConsole.Close();
#endif
            base.OnClosing(e);
        }

        public void ForceClose()
        {
            forceClose = true;
            Close();
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            switch (e.eventID)
            {
                case EventIDs.GUI_RESET_EVENT:
                    GeneratePanels();
                    DayNightToggle.Header = (ConfigManager.Screen.Contents.OnDayPalette) ? "Night" : "Day";
                    break;
                default:
                    break;
            }
        }
    }
}

