using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.JsonFileManagers;
using System.Windows.Controls;
using System.Windows.Media;
using VMSpc.Parsers;
using System.Windows;
using VMSpc.UI.DlgWindows;
using VMSpc.CustomComponents;
using System.Timers;
using VMSpc.UI.GaugeComponents;

namespace VMSpc.UI.Panels
{
    class VDiagAlarmPanel : VPanel, IEventConsumer
    {
        //ScrollViewer scrollViewer;
        private StackPanel recordStack;
        private ClockComponent Clock;
        private SolidColorBrush
            OkColor,
            WarningColor;
        private List<DiagnosticRecord> DisplayedRecords;
        private int numDisplayedRecords;
        private bool inWarningMode;
        public VDiagAlarmPanel(MainWindow mainWindow, DiagnosticGaugeSettings panelSettings)
            : base(mainWindow, panelSettings)
        {
            EventBridge.EventProcessor.SubscribeToEvent(this, Constants.DIAGNOSTIC_BASE);
            OkColor = new SolidColorBrush(panelSettings.backgroundColor);
            WarningColor = new SolidColorBrush(panelSettings.WarningColor);
            DisplayedRecords = new List<DiagnosticRecord>();
            inWarningMode = false;
            numDisplayedRecords = 0;
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            Update(e as DiagnosticEventArgs);
        }

        public void Update(DiagnosticEventArgs e)
        {
            if (e == null || e.record == null)
                return;
            AddRecord(e.record);
            DisplayedRecords.Add(e.record);
        }

        public override void GeneratePanel()
        {
            canvas.Children.Clear();
            numDisplayedRecords = 0;
            recordStack = new StackPanel()
            {
                Width = canvas.Width,
                Height = canvas.Height,
            };
            Clock = new ClockComponent((panelSettings as DiagnosticGaugeSettings).ShowAmPm)
            {
                Width = recordStack.Width,
                Height = recordStack.Height,
            };
            canvas.Children.Add(recordStack);
            recordStack.Children.Add(Clock);
            foreach (DiagnosticRecord record in DisplayedRecords)
            {
                if (record != null)
                {
                    AddRecord(record);
                    inWarningMode = true;
                }
            }
            SetBackground();
        }

        private void AddRecord(DiagnosticRecord record)
        {
            var entry = new RecordEntry(record, canvas.Width, (canvas.Height / (3)), RemoveRecord);
            recordStack.Children.Add(entry);
            if (!inWarningMode)
            {
                inWarningMode = true;
                SetBackground();
            }
            numDisplayedRecords++;
            AdjustRecordHeight();
        }

        private void RemoveRecord(DiagnosticRecord record)
        {
            foreach (var child in recordStack.Children)
            {
                var recordEntry = (child as RecordEntry);
                if (recordEntry != null && recordEntry.record.spn == record.spn)
                {
                    recordStack.Children.Remove(recordEntry);
                    DisplayedRecords.Remove(recordEntry.record);
                    break;
                }
            }
            numDisplayedRecords--;
            if (numDisplayedRecords == 0 && inWarningMode)
            {
                inWarningMode = false;
                SetBackground();
            }
            AdjustRecordHeight();
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

    public class RecordEntry : VMSCanvas
    {
        public readonly DiagnosticRecord record;
        private TextBlock textBlock;
        private Button clearButton;
        private Action<DiagnosticRecord> RemoveRecord;
        public RecordEntry(DiagnosticRecord record, double width, double height, Action<DiagnosticRecord> RemoveRecord)
        {
            this.record = record;
            Width = width;
            Height = height;
            textBlock = new TextBlock()
            {
                Text = this.record.ToString(),
                Width = (width * 0.875),
                Height = height,
                VerticalAlignment = VerticalAlignment.Center,
            };
            clearButton = new Button()
            {
                Content = "Clear",
                Width = (width / 8),
                Height = height,
            };
            AddChildren(textBlock, clearButton);
            SetLeft(textBlock, 0);
            SetLeft(clearButton, (width * 0.875));
            clearButton.Click += RemoveButton_Click;
            this.RemoveRecord = RemoveRecord;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRecord(record);
        }

        public void SetHeight(double height)
        {
            this.Height = height;
            textBlock.Height = this.Height;
            clearButton.Height = this.Height;
        }
    }
}
