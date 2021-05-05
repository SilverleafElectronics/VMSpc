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
using VMSpc.Common.DriverInterface;

namespace VMSpc
{
    public partial class MainWindow : Window, IEventConsumer
    {
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

        private void NewClockCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewClockCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClockSettings panelSettings = new ClockSettings();
            ClockDlg dlgWindow = new ClockDlg(panelSettings);
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
            CommDlg commdlg = new CommDlg();
            ShowVMSDlg(commdlg);
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
            var rawlogdlg = new RawLogDlg();
            ShowVMSDlg(rawlogdlg);
        }

        private void MessageTester_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MessageTester_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var messageTesterDlg = new MessageTester();
            messageTesterDlg.Show();
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
            ClipToggle.Header = (ConfigManager.Settings.Contents.useClipping) ? "Disable Snapping" : "Enable Snapping";
        }

        private void ToggleDayNightCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ToggleDayNightCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ConfigManager.Screen.Contents.OnDayPalette)
                DayNightManager.Instance.SetNight();
            else
                DayNightManager.Instance.SetDay();
            DayNightToggle.Header = (ConfigManager.Screen.Contents.OnDayPalette) ? "Night" : "Day";
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

        private void ColorPaletteManager_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ColorPaletteManager_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ColorPicker dlg = new ColorPicker(Colors.White)
            {
                Owner = this,
            };
            dlg.ShowDialog();

        }

        private void ParameterEditor_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ParameterEditor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new ParameterEditorDlg();
            ShowVMSDlg(dlg);
            GeneratePanels();
        }

        private void Tires_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Tires_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new TiresDlg();
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
            var dlg = new DiagnosticLogViewer()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            dlg.ShowDialog();
        }

        private void Maintenance_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Maintenance_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new MaintenanceDlg()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dlg.ShowDialog();
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
                //ConfigManager.Screen.SaveConfiguration();
                ConfigManager.Screen.Reload(dlg.ResultFilePath);
                ContentGrid.InitPanels(this);
                //Common.Helpers.ApplicationControl.Restart();
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigManager.Screen.SaveConfiguration();
        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new FileSelector("\\configuration\\screens", ConfigManager.Settings.Contents.screenFilePath, "scr.json")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExcludeLockedFiles = true,
                AllowNewFiles = true,
                NewFilesExtension = ".scr.json",
                AllowImports = false,
            };
            if ((bool)dlg.ShowDialog())
            {
                ConfigManager.Screen.SaveConfigurationAs(dlg.ResultFilePath);
                ConfigManager.Settings.Contents.screenFilePath = dlg.ResultFilePath;
                ContentGrid.InitPanels(this);
            }
        }

        private void FullScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void TakeSnapshot_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TakeSnapshot_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Snapshotter.ZipConfiguration();
        }

        private void CheckDrivers_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CheckDrivers_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var driverDetails = PnPutilInterface.GetVMSpcDrivers();
            if (driverDetails != null && driverDetails.Count > 0)
            {
                if (driverDetails.Count == 2)
                {
                    MessageBox.Show($"All VMSpc drivers are installed, including {driverDetails[0].Class} and {driverDetails[1].Class}");
                }
                else if (driverDetails.Count == 1)
                {
                    MessageBox.Show($"Only one VMSpc driver is installed: {driverDetails[0].Class}. Select the \"Install Drivers\" option to install the other driver");
                }
            }
            else
            {
                MessageBox.Show($"No VMSpc drivers are installed. Select the \"Install Drivers\" option");
            }
        }

        private void InstallDrivers_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void InstallDrivers_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DriverInstaller.InstallDrivers();
        }

        private void DeleteDrivers_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeleteDrivers_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PnPutilInterface.DeleteDrivers();
        }
    }

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
        public static readonly RoutedUICommand NewClock = new RoutedUICommand(
            "New Clock",
            "NewClock",
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
            "About VMSpc",
            "About",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand RawLog = new RoutedUICommand(
            "Raw Log",
            "RawLog",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand MessageTester = new RoutedUICommand(
            "Message Tester",
            "MessageTester",
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

        public static readonly RoutedUICommand ColorPaletteManager = new RoutedUICommand(
            "Color Palette Manager",
            "ColorPaletteManager",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand Tires = new RoutedUICommand(
            "Tires",
            "Tires",
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

        public static readonly RoutedUICommand Maintenance = new RoutedUICommand(
            "Maintenance",
            "Maintenance",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand Layouts = new RoutedUICommand(
            "Layouts",
            "Layouts",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand Save = new RoutedUICommand(
            "Save",
            "Save",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand SaveAs = new RoutedUICommand(
            "Save As",
            "SaveAs",
            typeof(MainCommands)
        );

        public static readonly RoutedUICommand ToggleDayNight = new RoutedUICommand(
            (ConfigManager.Screen.Contents.OnDayPalette) ? "Night" : "Day",
            "ToggleDayNight",
            typeof(MainCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F6)
            }
        );
        public static readonly RoutedUICommand FullScreen = new RoutedUICommand(
            "Full Screen",
            "FullScreen",
            typeof(MainCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F11),
                new KeyGesture(Key.Escape)
            }
        );
        public static readonly RoutedUICommand TakeSnapshot = new RoutedUICommand(
            "Take Snapshot",
            "TakeSnapshot",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand CheckDrivers = new RoutedUICommand(
            "Check Drivers",
            "TakeSnapshot",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand InstallDrivers = new RoutedUICommand(
            "Install Drivers",
            "InstallDrivers",
            typeof(MainCommands)
        );
        public static readonly RoutedUICommand DeleteDrivers = new RoutedUICommand(
            "Delete Drivers",
            "DeleteDrivers",
            typeof(MainCommands)
        );
    }
}
