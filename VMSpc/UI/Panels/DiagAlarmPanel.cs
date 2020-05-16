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
        private ClockComponent Clock;
        private SolidColorBrush
            OkColor,
            WarningColor;
        private List<DiagnosticMessage> DisplayedRecords;
        private int maxDisplayedRecords => (int)(canvas.Height / 20);
        private int numDisplayedRecords;
        private bool inWarningMode;
        public DiagAlarmPanel(MainWindow mainWindow, DiagnosticGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            this.panelSettings = panelSettings;
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.DIAGNOSTIC_BASE);
            DisplayedRecords = DiagnosticsParser.Instance.ActiveDiagnosticMessages.ToList();
            inWarningMode = false;
            numDisplayedRecords = 0;
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            Update(e as DiagnosticEventArgs);
        }

        public void Update(DiagnosticEventArgs e)
        {
            if (e == null || e.message == null)
                return;
            AddRecord(e.message);
            DisplayedRecords.Add(e.message);
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            OkColor = new SolidColorBrush(panelSettings.backgroundColor);
            WarningColor = new SolidColorBrush(panelSettings.WarningColor);
            numDisplayedRecords = 0;
            inWarningMode = false;
            recordStack = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
            };
            Clock = new ClockComponent((panelSettings as DiagnosticGaugeSettings).useMilitaryTime)
            {
                Width = recordStack.Width,
                Height = recordStack.Height,
                TextAlignment = TextAlignment.Center,
            };
            canvas.Children.Add(recordStack);
            recordStack.Children.Add(Clock);
            foreach (var record in DisplayedRecords)
            {
                if (record != null && !record.Acknowledged)
                {
                    AddRecord(record);
                    inWarningMode = true;
                }
            }
            SetBackground();
        }

        private void AddRecord(DiagnosticMessage message)
        {
            if (numDisplayedRecords < maxDisplayedRecords)
            {
                var entry = new RecordEntry(message, canvas.Width, (canvas.Height / maxDisplayedRecords), RemoveRecord);
                recordStack.Children.Add(entry);
                if (!inWarningMode)
                {
                    inWarningMode = true;
                    SetBackground();
                }
                numDisplayedRecords++;
                //AdjustRecordHeight();
            }
        }

        private void RemoveRecord(DiagnosticMessage message)
        {
            message.Acknowledged = true;
            GeneratePanel();
        }

        private void AdjustRecordHeight()
        {
            foreach (var entry in recordStack.Children)
            {
                if (entry.GetType() == typeof(RecordEntry))
                {
                    (entry as RecordEntry).SetHeight(canvas.Height / numDisplayedRecords);
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
                }
            }
            else
            {
                Clock.Enable();
                if (!recordStack.Children.Contains(Clock))
                {
                    recordStack.Children.Add(Clock);
                }
            }
        }

        public override void UpdatePanel()
        {
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
        private TextBlock
            SourceBlock,
            TypeBlock,
            IDBlock,
            MIDBlock,
            ComponentBlock,
            ModeBlock,
            DateBlock;
        private Button
            ClearButton;
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
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25d)});    //Alert signal
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25d)});    //Source
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25d)});    //Type
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25d)});    //ID
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25d)});    //MID
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (11d / 25d))});    //Component
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (6d / 25d))});    //Mode string
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (2d / 25d))});    //Time Stamp
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width * (1d / 25d))});    //Button


            AddAlertSignal();
            AddColumn(ref SourceBlock, message.SourceString, 1);
            AddColumn(ref TypeBlock, message.TypeString, 2);
            AddColumn(ref IDBlock, message.IDString, 3);
            AddColumn(ref MIDBlock, message.MidString, 4);
            AddColumn(ref ComponentBlock, message.Component, 5);
            AddColumn(ref ModeBlock, message.FmiString, 6);
            AddColumn(ref DateBlock, message.TimeStamp.ToString("t"), 7);
            AddButton();
            SourceBlock.ScaleText(Width / 25d, Height);
            TypeBlock.ScaleText(Width / 25d, Height);
            IDBlock.ScaleText(Width / 25d, Height);
            MIDBlock.ScaleText(Width / 25d, Height);
            var blockWidth = (Width * (12d / 25d));
            ComponentBlock.ScaleText(blockWidth, Height);
            ModeBlock.ScaleText(Width * (6d / 25d), Height);
            DateBlock.ScaleText(Width / 25d, Height);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMethod(message);
        }

        protected void AddAlertSignal()
        {
            AlertSignal = new TogglingEllipse()
            {
                VerticalAlignment = VerticalAlignment.Top,
                Width = Width / 35d,
                Height = Width / 35d,
                ToggleBrush1 = AlertToggle1,
                ToggleBrush2 = AlertToggle2,
                ToggleInterval = 1000,
                Background = new SolidColorBrush(Colors.Pink),
            };
            Children.Add(AlertSignal);
            SetColumn(AlertSignal, 0);
        }

        protected void AddColumn(ref TextBlock block, string text, int index)
        {
            block = new TextBlock()
            {
                Text = text,
                Height = this.Height,
                TextAlignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
            };
            Children.Add(block);
            SetColumn(block, index);
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Width / 25) });    //Alert signal
        }

        private void AddButton()
        {
            ClearButton = new Button()
            {
                Content = "Clear",
                MaxHeight = 20,
                MaxWidth = 40,
                Margin = new Thickness(0, 1, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            Children.Add(ClearButton);
            SetColumn(ClearButton, 8);
            ClearButton.Click += ClearButton_Click;
        }

        public void SetHeight(double height)
        {
            this.Height = height;
            AddColumns();
        }
    }

}
