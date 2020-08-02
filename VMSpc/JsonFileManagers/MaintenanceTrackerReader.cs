using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.AdvancedParsers;

namespace VMSpc.JsonFileManagers
{
    public enum MaintenanceIntervalType
    {
        DateOnly,
        MileageOnly,
        DateAndMileage,
    }
    public class CompletedItem
    {
        public DateTime DateCompleted;
        public ulong MilesCompletedOn;
        [DefaultValue("")]
        public string Actions;

        public override string ToString()
        {
            return "Completed on " + DateCompleted.ToString("MM/dd/yyyy") + " at " + MilesCompletedOn + " Miles.";
        }

        public CompletedItem()
        {
        }
    }


    public class MaintenanceItem
    {
        public string Name;
        public MaintenanceIntervalType MaintenanceIntervalType;
        public int MonthsMaintenanceInterval;
        public ulong MilesMaintenanceInterval;
        public List<CompletedItem> CompletedItems;
        [DefaultValue("")]
        public string Actions;

        public MaintenanceItem()
        {
            if (CompletedItems == null)
                CompletedItems = new List<CompletedItem>();
        }

        public CompletedItem GetLastCompleted()
        {
            DateTime maxDate = new DateTime(1, 1, 1, 1, 1, 1);
            CompletedItem maxItem = null;
            foreach (var completedItem in CompletedItems)
            {
                if (completedItem.DateCompleted > maxDate)
                {
                    maxDate = completedItem.DateCompleted;
                    maxItem = completedItem;
                }
            }
            return maxItem;
        }

        public DateTime NextDueDate()
        {
            var lastCompleted = GetLastCompleted();
            if (lastCompleted != null)
            {
                return lastCompleted.DateCompleted.AddMonths(MonthsMaintenanceInterval);
            }
            else
            {
                return DateTime.Now;
            }
        }

        public ulong NextDueMiles()
        {
            var lastCompleted = GetLastCompleted();
            if (lastCompleted != null)
            {
                return lastCompleted.MilesCompletedOn + MilesMaintenanceInterval;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            string value = Name + " -- ";
            if (MaintenanceIntervalType != MaintenanceIntervalType.MileageOnly)
            {
                value += "Next Due Date: " + NextDueDate().ToString("MM/dd/yyyy");
            }
            if (MaintenanceIntervalType == MaintenanceIntervalType.DateAndMileage)
            {
                value += ", ";
            }
            if (MaintenanceIntervalType != MaintenanceIntervalType.DateOnly)
            {
                value += "Next Due Miles: " + NextDueMiles().ToString();
            }
            return value;
        }

        public bool DueNow()
        {
            switch (MaintenanceIntervalType)
            {
                case MaintenanceIntervalType.DateAndMileage:
                    return (NextDueDate() <= DateTime.Now) || (NextDueMiles() <= ChassisParameters.Instance.CurrentMiles);
                case MaintenanceIntervalType.DateOnly:
                    return (NextDueDate() <= DateTime.Now);
                case MaintenanceIntervalType.MileageOnly:
                    return (NextDueMiles() <= ChassisParameters.Instance.CurrentMiles);
                default:
                    return false;
            }
        }
    }

    public class MaintenanceTrackerContents : IJsonContents
    {
        public List<MaintenanceItem> MaintenanceItems;
    }
    public class MaintenanceTrackerReader : JsonFileReader<MaintenanceTrackerContents>
    {
        public MaintenanceTrackerReader() 
            :base("\\configuration\\maintenance.json")
        {

        }

        protected override MaintenanceTrackerContents GetDefaultContents()
        {
            return new MaintenanceTrackerContents()
            {
                MaintenanceItems = new List<MaintenanceItem>()
            };
        }
    }
}
