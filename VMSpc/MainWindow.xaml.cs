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
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.DevHelpers;
using VMSpc.CustomComponents;
using VMSpc.Communication;
using static VMSpc.Parsers.PIDWrapper;
using VMSpc.Parsers;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;
using System.ComponentModel;
using VMSpc.UI.DlgWindows;

namespace VMSpc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public VMSComm commreader;
        //PanelGrid panelGrid;
        public bool forceClose;

        //constructor
        public MainWindow()
        {
            forceClose = false;
            InitializeComponent();
#if DEBUG
            Window debugConsole = new DebugConsole();
            debugConsole.Show();
#endif
            GeneratePanels();
            Application.Current.MainWindow = this;
            InitializeComm();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ProcessKeyDownEvent(e);
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
                    ContentGrid.ProcessArrowNavigation(e.Key);
                    break;
                default:
                    break;
            }
        }

        public void OnMouseRelease(object sender, RoutedEventArgs e)
        {
            ContentGrid.ProcessMouseRelease();
        }

        public void InitializeComm()
        {
            CloseComm();
            commreader = new VMSComm();
            commreader.StartComm();
        }

        public void CloseComm()
        {
            if (NOT_NULL(commreader))
                commreader.StopComm();
        }

        private void GeneratePanels()
        {
            ContentGrid.InitPanels(this);
        }

        private void SaveConfig()
        {
            ConfigManager.ParamData.SaveConfiguration();
            ConfigManager.Screen.SaveConfiguration();
        }

        #region EVENT HANDLERS

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!forceClose)
                SaveConfig();
            CloseComm();
            base.OnClosing(e);
        }

        public void ForceClose()
        {
            forceClose = true;
            Close();
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveConfig();
            this.Close();
        }

        private void NewSimpleGaugeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewSimpleGaugeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SimpleGaugeSettings panelSettings = new SimpleGaugeSettings();
            SimpleGaugeDlg dlgWindow = new SimpleGaugeDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewScanGaugeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewScanGaugeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScanGaugeSettings panelSettings = new ScanGaugeSettings();
            ScanGaugeDlg dlgWindow = new ScanGaugeDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewMultiBarCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewMultiBarCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            MultiBarSettings panelSettings = new MultiBarSettings(0);
            MultiBarDlg dlgWindow = new MultiBarDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
            */
        }

        private void NewOdometerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewOdometerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            OdometerSettings panelSettings = new OdometerSettings(0);
            OdometerDlg dlgWindow = new OdometerDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
            */
        }

        private void NewTextPanelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewTextPanelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TextGaugeSettings panelSettings = new TextGaugeSettings();
            TextPanelDlg dlgWindow = new TextPanelDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }
        private void NewImagePanel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewImagePanel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PictureSettings panelSettings = new PictureSettings();
            ImagePanelDlg dlgWindow = new ImagePanelDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }
        private void NewDiagnosticPanel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewDiagnosticPanel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DiagnosticGaugeSettings panelSettings = new DiagnosticGaugeSettings();
            DiagnosticAlarmDlg dlgWindow = new DiagnosticAlarmDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewTirePanel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTirePanel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TireGaugeSettings panelSettings = new TireGaugeSettings();
            TirePanelDlg dlgWindow = new TirePanelDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void InitiateNewGaugeDlg(VPanelDlg dlgWindow, PanelSettings panelSettings)
        {
            dlgWindow.Owner = this;
            bool result = (bool)dlgWindow.ShowDialog(null);
            if (result)
                ContentGrid.CreateNewPanel(panelSettings);
        }

        private bool ShowVMSDlg(VMSDialog dlgWindow)
        {
            dlgWindow.Owner = this;
            bool result = (bool)dlgWindow.ShowDialog();
            return result;
        }

        private void CommSettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommDlg commdlg = new CommDlg(commreader);
            if (ShowVMSDlg(commdlg))
                InitializeComm();
        }

        private void AboutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutDlg aboutdlg = new AboutDlg();
            ShowVMSDlg(aboutdlg);
        }

        private void RawLogCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RawLogCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RawLogDlg rawlogdlg = new RawLogDlg(commreader);
            if (ShowVMSDlg(rawlogdlg))
            {
                InitializeComm();
            }
        }

        private void DeleteGaugeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeleteGaugeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ContentGrid.DeletePanel();
        }

        private void ToggleClippingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ToggleClippingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigManager.Settings.Contents.useClipping = !(ConfigManager.Settings.Contents.useClipping);
            //ClipToggle.Header = (ConfigManager.Settings.Contents.useClipping) ? "Disable Clipping" : "Enable Clipping";
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ContentGrid.ProcessMouseMove(this, e);
        }
    }

    #endregion //Event Handlers

    #region Routed UI Commands (Maps to Custom Event Handlers)
    public static class MainCommands
    {

        public static readonly RoutedUICommand NewSimpleGauge = new RoutedUICommand(
            "New Simple Gauge",
            "NewSimpleGauge",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand NewScanGauge = new RoutedUICommand(
            "New Scan Gauge",
            "NewScanGauge",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand NewMultiBar = new RoutedUICommand(
            "New Multi Bar",
            "NewMultiBar",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand NewOdometer = new RoutedUICommand(
            "New Odometer",
            "NewOdometer",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand NewTextPanel = new RoutedUICommand(
            "New Text Panel",
            "NewTextPanel",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand NewImagePanel = new RoutedUICommand(
            "New Image Panel",
            "NewImagePanel",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand NewDiagnosticPanel = new RoutedUICommand(
            "New Diagnostic Panel",
            "NewDiagnosticPanel",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand NewTirePanel = new RoutedUICommand(
            "New Tire Panel",
            "NewTirePanel",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand CommSettings = new RoutedUICommand(
            "Communication",
            "CommSettings",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand About = new RoutedUICommand(
            "About VMSpc...",
            "About",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand RawLog = new RoutedUICommand(
            "Raw Log",
            "RawLog",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand DeleteGauge = new RoutedUICommand(
            "Delete Selected Gauge",
            "DeleteGauge",
            typeof(MainCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Delete)
            }
        );

        public static readonly RoutedUICommand ToggleClipping = new RoutedUICommand(
            (ConfigManager.Settings.Contents.useClipping) ? "Disable Clipping" : "Enable Clipping",
            "ToggleClipping",
            typeof(MainCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F5)
            }
       );
    }
    #endregion //Routed UI Commands
}
