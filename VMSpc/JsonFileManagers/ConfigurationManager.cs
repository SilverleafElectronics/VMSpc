using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Windows;
using VMSpc.Exceptions;
using VMSpc.Loggers;

namespace VMSpc.JsonFileManagers
{
    /// <summary>
    /// Class for interaction with configuration files
    /// </summary>
    public sealed class ConfigurationManager
    {
        public ColorReader ColorReader;
        public SettingsReader Settings;
        public ScreenReader Screen;
        public ParamDataReader ParamData;
        public MeterReader Meters;
        public ColorPalettesReader ColorPalettes;
        public AlarmsReader AlarmsReader;
        public DiagnosticLogReader DiagnosticLogReader;
        public MaintenanceTrackerReader MaintenanceTrackerReader;

        static ConfigurationManager() { }
        private ConfigurationManager() { }
        ~ConfigurationManager()
        {
            SaveAllConfig();
        }

        public static ConfigurationManager ConfigManager { get; } = new ConfigurationManager();

        public void LoadConfiguration()
        {
            try
            {
                CreateDirectories();
                VerifyEngineDirectory();
                ColorReader = new ColorReader();
                Settings = new SettingsReader();
                Screen = new ScreenReader(Settings.Contents.screenFilePath);
                Meters = new MeterReader(Settings.Contents.meterFilePath);
                ParamData = new ParamDataReader();
                ColorPalettes = new ColorPalettesReader();
                AlarmsReader = new AlarmsReader();
                DiagnosticLogReader = new DiagnosticLogReader();
                MaintenanceTrackerReader = new MaintenanceTrackerReader();
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception ex)
            {
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }

        /// <summary>
        /// Creates all required directories if they don't already exist.
        /// </summary>
        public void CreateDirectories()
        {
            FileOpener.CreateDirectory("\\configuration");
            FileOpener.CreateDirectory("\\configuration\\odometers");
            FileOpener.CreateDirectory("\\configuration\\screens");
            FileOpener.CreateDirectory("\\configuration\\tankminders");
            FileOpener.CreateDirectory("\\history_files");
            FileOpener.CreateDirectory("\\engines");
            FileOpener.CreateDirectory("\\logs");
            if (!FileOpener.DirectoryExists("\\rawlogs"))
            {
                FileOpener.CreateDirectory("\\rawlogs");
                FileOpener.WriteAllText("\\rawlogs\\j1708_demo.vms", FileSaveDefaults.DefaultRawLog.GetDefaultRawLog);
            }
            else
            {
                FileOpener.CreateDirectory("\\rawlogs");
            }
        }

        public void VerifyEngineDirectory()
        {
            if (FileOpener.IsDirectoryEmpty("\\engines"))
            {
                bool downloadEngines = MessageBox.Show(
                    "No engine files are available. Do you want VMSpc to automatically download them now (requires internet connection)?",
                    "No Engine Files",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    ) == MessageBoxResult.Yes;
                if (downloadEngines)
                {
                    if (EngineDownloader.DownloadEngines())
                    {
                        MessageBox.Show("Successfully downloaded Engines");
                    }
                    else
                    {
                        MessageBox.Show("Failed to download Engines. Verify that you have internet connection.");
                    }
                }
            }
        }

        /// <summary>
        /// Outputs all current configuration to their corresponding Json files
        /// </summary>
        public void SaveAllConfig()
        {
            Settings.SaveJson();
            Screen.SaveJson();
            ColorReader.SaveJson();
            Meters.SaveJson();
            ParamData.SaveJson();
            ColorPalettes.SaveJson();
            DiagnosticLogReader.SaveJson();
            MaintenanceTrackerReader.SaveJson();
        }
    }
}
