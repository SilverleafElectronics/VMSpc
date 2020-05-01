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
//using VMSpc.UI.DlgWindows.Gauges;
//using VMSpc.UI.DlgWindows.Help;
//using VMSpc.UI.DlgWindows.Settings;

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
            InitializeComm();
            var contents = ConfigManager.Settings.Contents;
            Left = contents.WindowLeft;
            Top = contents.WindowTop;
            if (contents.WindowWidth > SystemParameters.VirtualScreenWidth)
            {
                Width = SystemParameters.VirtualScreenWidth;
            }
            else
            {
                Width = contents.WindowWidth;
            }
            if (contents.WindowHeight > SystemParameters.VirtualScreenHeight)
            {
                Height = SystemParameters.VirtualScreenHeight;
            }
            else
            {
                Height = contents.WindowHeight;
            }
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
            ConfigManager.Settings.Contents.WindowHeight = Height;
            ConfigManager.Settings.Contents.WindowWidth = Width;
            ConfigManager.Settings.Contents.WindowTop = Top;
            ConfigManager.Settings.Contents.WindowLeft = Left;
            

            ConfigManager.Settings.SaveConfiguration();
            ConfigManager.ParamData.SaveConfiguration();
            ConfigManager.Screen.SaveConfiguration();
        }

        #region EVENT HANDLERS

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!forceClose)
                SaveConfig();
            CloseComm();
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

        private void NewRadialGaugeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewRadialGaugeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RadialGaugeSettings panelSettings = new RadialGaugeSettings();
            RadialGaugeDlg dlgWindow = new RadialGaugeDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewMultiBarCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewMultiBarCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MultiBarSettings panelSettings = new MultiBarSettings();
            MultiBarDlg dlgWindow = new MultiBarDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewOdometerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewOdometerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OdometerSettings panelSettings = new OdometerSettings();
            OdometerDlg dlgWindow = new OdometerDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
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

        private void NewTransmissionIndicator_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTransmissionIndicator_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TransmissionGaugeSettings panelSettings = new TransmissionGaugeSettings();
            TransmissionGaugeDlg dlgWindow = new TransmissionGaugeDlg(panelSettings);
            InitiateNewGaugeDlg(dlgWindow, panelSettings);
        }

        private void NewTankMinder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTankMinder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TankMinderSettings panelSettings = new TankMinderSettings();
            TankMinderDlg dlgWindow = new TankMinderDlg(panelSettings);
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
            dlgWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
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

        private void ChangeEngine_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ChangeEngine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new FileSelector("\\engines", ConfigManager.Settings.Contents.engineFilePath, "eng")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if ((bool)dlg.ShowDialog())
            {
                ConfigManager.Settings.Contents.engineFilePath = dlg.ResultFilePath;
                EngineSpec.SetEngineFile(new FileOpener(ConfigManager.Settings.Contents.engineFilePath).absoluteFilepath);
            }
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
            var rawlogdlg = new RawLogDlg(commreader);
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

        private void AttachGaugeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AttachGaugeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ContentGrid.CanAttachGauge)
            {
                if (MessageBox.Show("Select the panel to which the selected panel should be attached.") == MessageBoxResult.OK)
                {
                    ContentGrid.InAttachMode = true;
                }
            }
        }

        private void ToggleClippingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ToggleClippingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigManager.Settings.Contents.useClipping = !(ConfigManager.Settings.Contents.useClipping);
            ClipToggle.Header = (ConfigManager.Settings.Contents.useClipping) ? "Disable Clipping" : "Enable Clipping";
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ContentGrid.ProcessMouseMove(this, e);
        }

        private void ColorPalette_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ColorPalette_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ColorPaletteDlg dlg = new ColorPaletteDlg();
            if (ShowVMSDlg(dlg))
            {
                GeneratePanels();
            }
        }

        private void ParameterEditor_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ParameterEditor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new ParameterEditorDlg();
            if (ShowVMSDlg(dlg))
            {
                GeneratePanels();
            }
        }

        private void ViewDiagnostics_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ViewDiagnostics_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Layouts_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Layouts_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new FileSelector("\\configuration\\screens", ConfigManager.Settings.Contents.screenFilePath, "scr.json")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExcludeLockedFiles = true,
                AllowNewFiles = true,
                NewFilesExtension = ".scr.json",
                AllowImports = true,
                ImportFilter = "JSON Screen Files (*.scr.json)|*.vms"
            };
            if ((bool)dlg.ShowDialog())
            {
                ConfigManager.Settings.Contents.screenFilePath = dlg.ResultFilePath;
                ContentGrid.InitPanels(this);
                //Common.Helpers.ApplicationControl.Restart();
            }
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

        public static readonly RoutedUICommand NewRadialGauge = new RoutedUICommand(
            "New Radial Gauge",
            "NewRadialGauge",
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
        public static readonly RoutedUICommand NewTransmissionIndicator = new RoutedUICommand(
            "New Transmission Indicator",
            "NewTransmissionIndicator",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand NewTankMinder = new RoutedUICommand(
            "New Tank Minder",
            "NewTankMinder",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand CommSettings = new RoutedUICommand(
            "Communication",
            "CommSettings",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand ChangeEngine = new RoutedUICommand(
            "Change Engine",
            "ChangeEngine",
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

        public static readonly RoutedUICommand AttachGauge = new RoutedUICommand(
            "Attach Gauge",
            "AttachGauge",
            typeof(MainCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.A, ModifierKeys.Control),
            }
        );

        public static readonly RoutedUICommand ColorPalette = new RoutedUICommand(
            "Color Palette",
            "ColorPalette",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand ParameterEditor = new RoutedUICommand(
            "Parameter Editor",
            "ParameterEditor",
            typeof(MainCommands)
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

        public static readonly RoutedUICommand ViewDiagnostics = new RoutedUICommand(
            "View Diagnostics",
            "ViewDiagnostics",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand Layouts = new RoutedUICommand(
            "Layouts",
            "Layouts",
            typeof(MainCommands)
        );
    }
    #endregion Routed UI Commands
}

