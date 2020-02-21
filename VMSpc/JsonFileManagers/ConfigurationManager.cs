using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace VMSpc.JsonFileManagers
{
    /// <summary>
    /// Class for interaction with configuration files
    /// </summary>
    public sealed class ConfigurationManager
    {
        public SettingsReader Settings;
        public ScreenReader Screen;
        public ParamDataReader ParamData;

        static ConfigurationManager() { }
        private ConfigurationManager() { }
        ~ConfigurationManager()
        {
            SaveAllConfig();
        }

        public static ConfigurationManager ConfigManager { get; } = new ConfigurationManager();

        public void LoadConfiguration()
        {
            CreateDirectories();
            Settings = new SettingsReader();
            Screen = new ScreenReader(Settings.Contents.screenFilePath);
            ParamData = new ParamDataReader();
        }

        /// <summary>
        /// Creates all required directories if they don't already exist.
        /// </summary>
        public void CreateDirectories()
        {
            string cwd = Directory.GetCurrentDirectory();
            Directory.CreateDirectory(cwd + "/configuration");
            Directory.CreateDirectory(cwd + "/configuration/odometer_files");
            Directory.CreateDirectory(cwd + "/configuration/screens");
            Directory.CreateDirectory(cwd + "/history_files");
            Directory.CreateDirectory(cwd + "/rawlogs");
        }
        /// <summary>
        /// Outputs all current configuration to their corresponding Json files
        /// </summary>
        public void SaveAllConfig()
        {
            Settings.SaveJson();
            Screen.SaveJson();
        }
    }
}
