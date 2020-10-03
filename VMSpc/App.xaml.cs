using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VMSpc.Parsers;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using VMSpc.UI;
using VMSpc.JsonFileManagers;
using System.Security.Permissions;
using VMSpc.Common;
using VMSpc.AdvancedParsers;
using VMSpc.AdvancedParsers.Tires;
using VMSpc.Loggers;
using VMSpc.Communication;
using VMSpc.Exceptions;
using System.Management;

namespace VMSpc
{
    /*  Startup process: 
     *  - loads the engine files
     *  - constructs the VMSComm reader object
     *  - constructs the engine manager (which constructs all engine component parser objects)
     *  - constructs the panel manager (which constructs all sub-panel UI component objects)
     */
    public partial class App : Application
    {
        //private VMSComm commreader;
        //private EngineManager enginemanager;
        //private PanelManager panelmanager;
        DateTime appstart;
        MainWindow wnd;

        private PIDValueStager _PIDValueStager;
        private EngineDataParser _EngineDataParser;
        private Acceleration _Acceleration;
        private Trackers _Trackers;

        private static bool ShowingException = false;
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
            EventBridge.Initialize();
            DiagnosticsParser.Initialize();
            CanMessageHandler.Initialize();
            ConfigManager.LoadConfiguration();
            DiagnosticLogger.Initialize();
            RawLogger.Initialize();
            PIDValueStager.Initialize();
            _EngineDataParser = new EngineDataParser();
            _Acceleration = new Acceleration();
            _Trackers = new Trackers();
            ChassisParameters.Initialize();
            var engineFilePointer = new FileOpener(ConfigManager.Settings.Contents.engineFilePath);
            if (engineFilePointer.Exists())
            {
                EngineSpec.SetEngineFile(engineFilePointer.absoluteFilepath);
            }
            else
            {
                MessageBox.Show("No engine files can be found. Horsepower and Torque settings will be inaccurate");
            }
            SPNDefinitions.Activate();      //in VMSpc/Parsers/J1939/SPNDefinitions.cs - Defines every SPN object
            //Odometer.Activate();
            //ParamData.Activate();
            TireManager.Initialize();
            CommunicationManager.Initialize();
            DayNightManager.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            VerifyDriverInstallation();
            AddAccessPermissions();
            ActivateStaticClasses();
            VMSpcStart();
            AddGlobalEventHandlers();
        }

        private void VerifyDriverInstallation()
        {
            if (!DriversAreInstalled())
            {
                var result = MessageBox.Show("The required drivers for VMSpc are not installed. Do you want to install them now? This will require an " +
                    "internet connection if the installation file is not already available", "Install Drivers", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (DriverInstaller.InstallDrivers() && DriversAreInstalled())
                    {
                        MessageBox.Show("Drivers successfully installed");
                    }
                    else
                    {
                        MessageBox.Show("Drivers failed to install. Please see instructions on our website or contact support");
                    }
                }
            }
        }

        private bool DriversAreInstalled()
        {
            SelectQuery query = new SelectQuery("Win32_SystemDriver");
            //query.Condition = "Name = 'FTDIBUS'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            var drivers = searcher.Get();
            return (drivers.Count > 0);
        }

        private void AddAccessPermissions()
        {
            FileIOPermission permissions = new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, FileOpener.BaseDirectory + "\\");
            permissions.AllLocalFiles = FileIOPermissionAccess.Read | FileIOPermissionAccess.Write;
            permissions.Demand();
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
            appstart = DateTime.Now;
        }

        private void VMSpcStart()
        {
            wnd = new MainWindow();
            wnd.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!ShowingException)  //In case an exception causes a cascade of accompanying exceptions in other threads (shouldn't happen), only allow the first one to show
            {
                ShowingException = true;
                ExceptionWindow exceptionWindow = new ExceptionWindow(e);
                exceptionWindow.ShowDialog();
                wnd.Close();
                Current.Shutdown();
            }
        }
    }
}
