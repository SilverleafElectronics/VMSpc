//#define DEBUG_CONSOLE
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
using VMSpc.DevHelpers;
using VMSpc.CustomComponents;
using System.Threading;
using VMSpc.Communication;
using static VMSpc.Parsers.PIDWrapper;
using VMSpc.Parsers;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.Constants;
using System.ComponentModel;

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
            ParamData.Load();
            PIDManager.InitializePIDs();
            PresenterWrapper.InitializePresenterList();
            GeneratePanels();
            Application.Current.MainWindow = this;
            InitializeComm();
            //Application.Current.MainWindow
        }

        private void InitializeComm()
        {
            commreader = new VMSComm();
            Thread commThread = new Thread(commreader.StartComm);
            commThread.Start();
        }

        private void GeneratePanels()
        {
            VMSConsole.AddConsoleToWindow(ContentGrid);
            ContentGrid.InitPanels(this);
        }

        private void SaveConfig()
        {
            ParamData.SaveConfiguration();
            DefaultScrnManager.scrnManager.SaveConfiguration();
        }

        #region EVENT HANDLERS

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!forceClose)
                SaveConfig();
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
            SimpleGaugeSettings panelSettings = new SimpleGaugeSettings(0);
            SimpleGaugeDlg dlgWindow = new SimpleGaugeDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewScanGaugeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewScanGaugeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScanGaugeSettings panelSettings = new ScanGaugeSettings(0);
            ScanGaugeDlg dlgWindow = new ScanGaugeDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewMultiBarCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewMultiBarCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MultiBarSettings panelSettings = new MultiBarSettings(0);
            MultiBarDlg dlgWindow = new MultiBarDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void InitiateNewGaugeDlg(VPanelDlg dlgWindow, PanelSettings panelSettings)
        {
            dlgWindow.Owner = this;
            bool result = (bool)dlgWindow.ShowDialog(null);
            if (result)
                ContentGrid.CreateNewPanel(panelSettings);
        }

        private void CommSettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommDlg commdlg = new CommDlg(commreader);
            commdlg.ShowDialog();
        }

        private void AboutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutDlg aboutdlg = new AboutDlg();
            aboutdlg.ShowDialog();
        }

        private void RawLogCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RawLogCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RawLogDlg rawlogdlg = new RawLogDlg(commreader);
            rawlogdlg.ShowDialog();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Delete)
                ContentGrid.DeletePanel();
        }



        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ContentGrid.ProcessMouseMove(this, e);
        }
    }

    #endregion //Event Handlers

    #region Custom Event Handlers
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
    }
    #endregion //Custom Event Handlers
}
