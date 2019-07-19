﻿//#define DEBUG_CONSOLE
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

        //constructor
        public MainWindow()
        {
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
            SaveConfig();
            base.OnClosing(e);
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
            dlgWindow.Owner = this;
            bool? result = dlgWindow.ShowDialog(null);
            if (result == true)
            {
                ContentGrid.CreateNewPanel(panelSettings);
            }
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ContentGrid.ProcessMouseMove(this, e);
        }

        #endregion //EVENT HANDLERS
    }


    #region Custom Event Handlers
    public static class MainCommands
    {
        public static readonly RoutedUICommand NewSimpleGauge = new RoutedUICommand(
            "New Simple Gauge",
            "NewSimpleGauge",
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
