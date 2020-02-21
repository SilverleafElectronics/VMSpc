using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static VMSpc.XmlFileManagers.ParamDataManager;
using static VMSpc.Constants;
using System.IO;
using System.Timers;
using static VMSpc.XmlFileManagers.OdometerTracker;
using VMSpc.Helpers;

namespace VMSpc.XmlFileManagers
{
    /// <summary> 
    /// Class for handling the creation of specific odometer files. Not 
    /// responsible for writing to the main Odometer.xml file. See OdometerTracker.cs for that implementation 
    /// </summary>
    public class OdometerManager : XmlFileManager
    {
        public double startFuel,
                      startHours,
                      startMiles,
                      startLiters,
                      startKilometers;
        public OdometerManager(string filename) : base ("/odometer_files/" + filename)
        {
            startFuel       = Double.Parse(getNodeByTagName("Start-Fuel").InnerText);
            startHours      = Double.Parse(getNodeByTagName("Start-Hours").InnerText);
            startMiles      = Double.Parse(getNodeByTagName("Start-Miles").InnerText);
            startLiters     = Double.Parse(getNodeByTagName("Start-Liters").InnerText);
            startKilometers = Double.Parse(getNodeByTagName("Start-Km").InnerText);
        }

        protected override void CreateTemplate()
        {
            base.CreateTemplate();
            ResetTrip();
        }

        public void ResetTrip()
        {
            startFuel = ZERO_IF_INVALID(Odometer.Fuel);
            startLiters = ZERO_IF_INVALID(Odometer.Liters);
            startHours = ZERO_IF_INVALID(Odometer.Hours);
            startMiles = ZERO_IF_INVALID(Odometer.Miles);
            startKilometers = ZERO_IF_INVALID(Odometer.Kilometers);
            if (!File.Exists("./history_files/" + CHANGE_FILE_TYPE(docName, ".xml", ".txt")))
                CreateHistoryFile(docName.Substring(docName.LastIndexOf("/") + 1));
            OverwriteFile(
                "<Root>" +
                "   <Version>V4.1</Version>" +
                "   <Odo-Trip-Data>" +
                "       <Start-Fuel>" + startFuel + "</Start-Fuel>" +
                "       <Start-Hours>" + startHours + "</Start-Hours>" +
                "       <Start-Miles>" + startMiles + "</Start-Miles>" +
                "       <Start-Liters>" + startLiters + "</Start-Liters>" +
                "       <Start-Km>" + startKilometers + "</Start-Km>" +
                "   </Odo-Trip-Data>" +
                "</Root> "
                );
            Initialize();
        }

        public void StartFromDayOne()
        {
            OverwriteFile(
                "<Root>" +
                "   <Version>V4.1</Version>" +
                "   <Odo-Trip-Data>" +
                "       <Start-Fuel>0</Start-Fuel>" +
                "       <Start-Hours>0</Start-Hours>" +
                "       <Start-Miles>0</Start-Miles>" +
                "       <Start-Liters>0</Start-Liters>" +
                "       <Start-Km>0</Start-Km>" +
                "   </Odo-Trip-Data>" +
                "</Root> "
                );
            Initialize();
        }

        public static void CreateHistoryFile(string xmlFile)
        {
            string fileName = CHANGE_FILE_TYPE(xmlFile, ".xml", ".txt");
            string historyHeader =
                "Trip History File\n" +
                "Feel free to add notes to this file, but do not move or delete.\n" +
                "End Date    Miles    Time   Fuel  Speed   MPG\n" +
                "=============================================\n";
            File.WriteAllText("./history_files/" + fileName, historyHeader);
        }

        public static void AddHistoryLog(double miles, double hours, double fuel, double speed, double mpg)
        {
            string date = string.Format("{0}/{1}/{2}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year);
            string TimeElapsed = string.Format("{0}:{1:00}", Math.Round(hours), ((hours % 1) * 60));
            string entry = string.Format("{0},    {1},    {2},    {3},    {4},    {5}", date, miles, hours, fuel, speed, mpg);
        }

        public void InitializeOdometerWrite()
        {
            CREATE_TIMER(UpdateOdometer, 10000);
        }

        protected static void UpdateOdometer()
        {

        }

    }
}
