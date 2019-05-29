using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMSpc.Managers;

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
        public long startcounter;
        public App()
        {
            startcounter = DateTime.Now.Ticks;
            Initialize();
            var settings = VMSpc.Properties.Settings.Default;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            ShowSplashScreen();
            VMSpcStart();
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
            MainWindow wnd = new MainWindow();
            wnd.Show();
        }
    }
}
