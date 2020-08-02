using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.UI.DlgWindows;
using VMSpc.Panels;
using VMSpc.JsonFileManagers;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.Parsers;
using System.Windows;
using VMSpc.UI.CustomComponents;
using System.Timers;
using VMSpc.UI.GaugeComponents;
using VMSpc.Common;
using VMSpc.AdvancedParsers;
using VMSpc.Extensions.UI;
using VMSpc.DevHelpers;
using System.Windows.Shapes;

namespace VMSpc.UI.Panels
{
    class DiagAlarmPanel : VPanel, IEventConsumer
    {
        //ScrollViewer scrollViewer;
        protected new DiagnosticGaugeSettings panelSettings;
        private StackPanel recordStack;
        private Button ClearButton;
        private ClockComponent Clock;
        private SolidColorBrush
            OkColor,
            WarningColor;
        private List<DiagnosticMessage> CurrentRecords => DiagnosticsParser.Instance.ActiveDiagnosticMessages;
        private List<RecordEntry> RecordEntries;
        private int maxDisplayedRecords => ((int)(canvas.Height / 30) - 1 > 3) ? (int)(canvas.Height / 30) - 1 : 3;   //2 is subtracted for the clear button
        private int numDisplayedRecords;
        private int UpdateCount;
        private bool inWarningMode;
        private bool IgnoreUntilReset = false;
        private DateTime Last5MinuteIgnore = new DateTime(01, 01, 01);  //starts at "null" date.
        public DiagAlarmPanel(MainWindow mainWindow, DiagnosticGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.DIAGNOSTIC_BASE);
            RecordEntries = new List<RecordEntry>();
            inWarningMode = false;
            numDisplayedRecords = 0;
            UpdateCount = 0;
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            switch (e.eventID & EventIDs.EVENT_BASE_MASK)
            {
                case EventIDs.DIAGNOSTIC_BASE:
                    Update(e as DiagnosticEventArgs);
                    break;
                default:
                    break;
            }
        }

        public void Update(DiagnosticEventArgs e)
        {
            if (e == null || e.message == null)
                return;
            if (IgnoreUntilReset || (Last5MinuteIgnore > DateTime.Now))
                return;
            AddRecord(e.message);
            GeneratePanel();
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            RecordEntries.Clear();
            OkColor = new SolidColorBrush(panelSettings.BackgroundColor);
            WarningColor = new SolidColorBrush(panelSettings.WarningColor);
            numDisplayedRecords = 0;
            inWarningMode = false;
            recordStack = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
            };
            canvas.Children.Add(recordStack);
            AddClock();
            AddClearButton();
            AddExistingRecords();
        }

        private void AddClock()
        {
            Clock = new ClockComponent((panelSettings as DiagnosticGaugeSettings).useMilitaryTime)
            {
                Width = recordStack.Width,
                Height = recordStack.Height,
                TextAlignment = TextAlignment.Center,
            };
            recordStack.Children.Add(Clock);
        }

        private void AddClearButton()
        {
            ClearButton = new Button()
            {
                Name = "ClearButton",
                Width = 75,
                Height = canvas.Height / (maxDisplayedRecords * 2),
                MaxHeight = 25,
                FontSize = 10,
                Content = "Clear",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };
            ClearButton.Click += ClearButton_Click;
        }

        private void AddExistingRecords()
        {
            if ((CurrentRecords.Count > 0) && !IgnoreUntilReset && (DateTime.Now > Last5MinuteIgnore))
            {
                foreach (var record in CurrentRecords)
                {
                    if (record != null && !record.Acknowledged)
                    {
                        AddRecord(record);
                        inWarningMode = true;
                    }
                }
            }
        }

        private void SetBackground()
        {
            recordStack.Background = (inWarningMode) ? WarningColor : OkColor;
            if (inWarningMode)
            {
                Clock.Disable();
                if (recordStack.Children.Contains(Clock))
                {
                    recordStack.Children.Remove(Clock);
                    recordStack.Children.Add(ClearButton);
                }
            }
            else
            {
                Clock.Enable();
                if (!recordStack.Children.Contains(Clock))
                {
                    recordStack.Children.Add(Clock);
                    recordStack.Children.Remove(ClearButton);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ClearDiagnosticsBox();
            dlg.Owner = mainWindow;
            dlg.ShowDialog();
            switch (dlg.ClearDiagnosticsResult)
            {
                case ClearDiagnosticsResult.ClearInactiveRecords:
                    foreach (var record in CurrentRecords.FindAll(x => x.IsOldRecord == true))
                    {
                        record.Acknowledged = true;
                    }
                    break;
                case ClearDiagnosticsResult.ClearAllRecords:
                    foreach (var record in CurrentRecords)
                        record.Acknowledged = true;
                    break;
                case ClearDiagnosticsResult.ClearAllForFiveMinutes:
                    CurrentRecords.Clear();
                    Last5MinuteIgnore = DateTime.Now.AddMinutes(5);
                    break;
                case ClearDiagnosticsResult.ClearAllUntilRestart:
                    IgnoreUntilReset = true;
                    break;
            }
            GeneratePanel();
        }

        private void AddRecord(DiagnosticMessage message)
        {
            if (numDisplayedRecords < maxDisplayedRecords)
            {
                if ((RecordEntries.Find(x => (x.message.ID == message.ID) && (x.message.Fmi == message.Fmi)) == null))
                {
                    if (!inWarningMode)
                    {
                        inWarningMode = true;
                        SetBackground();
                    }
                    var entry = new RecordEntry(message, canvas.Width, ((canvas.Height - (canvas.Height / (maxDisplayedRecords * 2))) / (double)maxDisplayedRecords), RemoveRecord)
                    {
                        //Background = new SolidColorBrush(Colors.Blue),
                    };
                    RecordEntries.Add(entry);
                    recordStack.Children.Add(entry);
                    numDisplayedRecords++;
                    //AdjustRecordHeight();
                }
            }
        }

        private void RemoveRecord(DiagnosticMessage message)
        {
            message.Acknowledged = true;
            GeneratePanel();
        }

        private void AdjustRecordHeight()
        {
            ClearButton.Height = canvas.Height / maxDisplayedRecords;
            foreach (var entry in RecordEntries)
            {
                entry.SetHeight(canvas.Height / maxDisplayedRecords);
            }
        }

        public override void UpdatePanel()
        {
            UpdateCount++;
            //Updates once every second
            if (UpdateCount % 2 == 0)
            {
                foreach (var entry in RecordEntries)
                {
                    entry.Update();
                }
            }
        }

        protected override VMSDialog GenerateDlg()
        {
            return new DiagnosticAlarmDlg((DiagnosticGaugeSettings)panelSettings);
        }
    }

    public class RecordEntry : Grid
    {
        private Action<DiagnosticMessage> ClearMethod;
        public DiagnosticMessage message { get; private set; }

        private static SolidColorBrush
            AlertToggle1 = new SolidColorBrush(Colors.White),
            AlertToggle2 = new SolidColorBrush(Colors.Yellow);
        private TogglingEllipse 
            AlertSignal; 
        private Label
            SourceBlock,
            TypeBlock,
            IDBlock,
            MIDBlock,
            ComponentBlock,
            ModeBlock,
            DateBlock;
        public RecordEntry(DiagnosticMessage message, double width, double height, Action<DiagnosticMessage> ClearMethod)
        {
            Width = width;
            Height = height;
            this.ClearMethod = ClearMethod;
            this.message = message;
            AddColumns();
        }

        protected void AddColumns()
        {
            ColumnDefinitions.Clear();
            Children.Clear();
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 20d)});    //Alert signal
            //ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 20d)});    //Source
            //ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 20d)});    //Type
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (2 / 20d))});    //ID
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (2 / 20d))});    //MID
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (9d / 20d))});    //Component
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (4d / 20d))});    //Mode string
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (2d / 20d))});    //Time Stamp


            AddAlertSignal();
            //AddColumn(ref SourceBlock, message.SourceString, 1);
            //AddColumn(ref TypeBlock, message.TypeString, 2);
            AddColumn(ref IDBlock, message.IDString, 1);
            AddColumn(ref MIDBlock, message.MidString, 2);
            AddColumn(ref ComponentBlock, message.Component, 3);
            AddColumn(ref ModeBlock, message.FmiString, 4);
            AddColumn(ref DateBlock, message.TimeStamp.ToString("h:mm"), 5);
            //AddButton();
            //SourceBlock.ScaleText(Width / 20d, Height);
            //TypeBlock.ScaleText(Width / 20d, Height);
            IDBlock.ScaleText(Width / 20d, Height);
            MIDBlock.ScaleText(Width / 20d, Height);
            ComponentBlock.ScaleText(Width * (9d / 20d), Height);
            ModeBlock.ScaleText(Width * (5d / 20d), Height);
            DateBlock.ScaleText(Width * (3d / 20d), Height);
            Children.BalanceTextBlocks();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMethod(message);
        }

        protected void AddAlertSignal()
        {
            var alertHeight = Width / 35d;
            var padding = (Height - alertHeight) / 2;
            AlertSignal = new TogglingEllipse()
            {
                VerticalAlignment = VerticalAlignment.Top,
                Width = Width / 35d,
                Height = Width / 35d,
                Margin = new Thickness(0, padding, 0, padding),
                ToggleBrush1 = AlertToggle1,
                ToggleBrush2 = AlertToggle2,
                SolidBrush = new SolidColorBrush(Colors.Green),
            };
            Children.Add(AlertSignal);
            SetColumn(AlertSignal, 0);
            AlertSignal.ToggleColor();
        }

        protected void AddColumn(ref Label block, string text, int index)
        {
            block = new Label()
            {
                Content = text,
                Height = this.Height,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
            };
            Children.Add(block);
            SetColumn(block, index);
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25) });    //Alert signal
        }

        public void SetHeight(double height)
        {
            this.Height = height;
            AddColumns();
        }

        public void Update()
        {
            var time = DateTime.Now;
            if (message.IsOldRecord)  //set alert signal to solid green if we haven't seen the error in 5 minutes or more
            {
                AlertSignal.ChangeToSolidColor();
            }
            else
            {
                AlertSignal.ToggleColor();
            }
            if (message.TimeStamp.ToString("h:mm") != DateBlock.Content.ToString()) //update to the latest timestamp
            {
                DateBlock.Content = message.TimeStamp.ToString("h:mm");
            }
        }
    }

}
