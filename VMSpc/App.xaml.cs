using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VMSpc.Parsers;
using static VMSpc.XmlFileManagers.AlarmDataManager;
using static VMSpc.XmlFileManagers.ClockSettings;
using static VMSpc.XmlFileManagers.DefaultScrnManager;
using static VMSpc.XmlFileManagers.DiagnosticGaugeSettings;
using static VMSpc.XmlFileManagers.GaugeSettings;
using static VMSpc.XmlFileManagers.MessageBoxSettings;
using static VMSpc.XmlFileManagers.MessagesManager;
using static VMSpc.XmlFileManagers.MultiBarSettings;
using static VMSpc.XmlFileManagers.OdometerManager;
using static VMSpc.XmlFileManagers.OdometerSettings;
using static VMSpc.XmlFileManagers.OdometerTracker;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.XmlFileManagers.TireSettingsManager;
using static VMSpc.XmlFileManagers.XmlFileManager;

namespace VMSpc
{
    /*  Startup process: 
     *  - loads the engine files
     *  - constructs the VMSComm reader object
     *  - constructs the engine manager (which constructs all engine component parser objects)
     *  - constructs the panel manager (which constructs all sub-panel UI component objects)
     *  - displays the splashscreen, if enabled
     */
    public partial class App : Application
    {
        //private VMSComm commreader;
        //private EngineManager enginemanager;
        //private PanelManager panelmanager;
        DateTime appstart;
        MainWindow wnd;
        public long startcounter;
        public App()
        {
            startcounter = DateTime.Now.Ticks;
            Initialize();
            var settings = VMSpc.Properties.Settings.Default;
        }

        /// <summary>
        /// All static classes and singletons meant for global 
        /// usage are activated here. Some areas depend on these classes having their 
        /// data loaded before the program starts (e.g., OdometerTracker), so it
        /// is very important that they are called here. All static classes should at
        /// the bare minimum implement an empty Activate() method to ensure their 
        /// constructors are called
        /// </summary>
        private void ActivateStaticClasses()
        {
            SPNDefinitions.Activate();      //in VMSpc/Parsers/J1939/SPNDefinitions.cs - Defines every SPN object
            PIDManager.Activate();          //in VMSpc/Parsers/J1708/PIDWrapper.cs     - Creates a PID object for all J1708 PIDs and attaches them to PIDList
            PresenterWrapper.Activate();    //in VMSpc/Parsers/PresenterWrapper.cs     - Attaches all TSPNDatum objects to a presenter object, which is then stored in PresenterList
            Odometer.Activate();
            ParamData.Activate();

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            ShowSplashScreen();
            ActivateStaticClasses();
            VMSpcStart();
            AddGlobalEventHandlers();
        }

        private void AddGlobalEventHandlers()
        {
            //For handling release of the mouse. This is the only way to prevent the cursor from freezing on border resizing 
            EventManager.RegisterClassHandler(typeof(Window), Window.MouseLeftButtonUpEvent, new RoutedEventHandler(wnd.OnMouseRelease));
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseLeftButtonUpEvent, new RoutedEventHandler(wnd.OnMouseRelease));
            EventManager.RegisterClassHandler(typeof(Control), Control.MouseLeftButtonUpEvent, new RoutedEventHandler(wnd.OnMouseRelease));
        }

        private void Initialize()
        {
            //configuration = new Configuration();
            //commreader = new VMSComm();
            //enginemanager = new EngineManager();
            //panelmanager = new PanelManager();
            appstart = DateTime.Now;
        }

        private void ShowSplashScreen()
        {
            if (false) //CHANGEME - to if (SettingsManager.showSplashScreen == true)
            {
                SplashScreen splashScreen = new SplashScreen("./Resources/silverleaf_300x200.bmp");
                splashScreen.Show(true);
                //keep splash screen up for at least 3 seconds
                long now = DateTime.Now.Ticks;
                long elapsed = (now - startcounter) / 10000;
                if (elapsed < 3000)
                    System.Threading.Thread.Sleep((int)(3000 - elapsed));
            }
        }

        private void VMSpcStart()
        {
            wnd = new MainWindow();
            wnd.Show();
        }

        private string ExceptionInstructions =>
            (
                "For debugging support, please do one of the following:\n" +
                "1. Press the 'Yes' button below. This will put the contents of the error into your clipboard. You can paste the text into an email, and send the email with the subject \"VMSpc Unhandled Exceptions\" to support@simply-smarter.com\n" +
                "2. Take a screenshot of this screen, and either email us the image or call our support team\n" +
                "3. Leave this window open and call our support team for more instructions"
            );

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "\n\n" + ExceptionInstructions + "\n\nException Details:\n\n" + e.Exception.ToString() + "\n\nDo you want to copy the error message?\n\n", "VMSpc Error", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Clipboard.SetText("VMSpc Error Message: " + e.Exception.ToString());
            wnd.ForceClose();
        }
    }
}
