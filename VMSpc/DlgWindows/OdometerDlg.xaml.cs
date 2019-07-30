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
using System.Windows.Shapes;
using static VMSpc.XmlFileManagers.ParamDataManager;
using VMSpc.DevHelpers;
using VMSpc.CustomComponents;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;
using System.ComponentModel;
using System.IO;
using VMSpc.Panels;
using Microsoft.Win32;

namespace VMSpc.DlgWindows
{
    /// <summary>
    /// Interaction logic for SimpleGaugeDlg.xaml
    /// </summary>
    public partial class OdometerDlg : VPanelDlg
    {
        protected new OdometerSettings panelSettings;
        private string textFile;
        private string newOdoFile;

        public OdometerDlg(OdometerSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
            textFile = "./history_files/" + CHANGE_FILE_TYPE(panelSettings.fileName, ".xml", ".txt");
            newOdoFile = panelSettings.fileName;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (OdometerSettings)base.panelSettings;
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.ID = PanelIDs.ODOMOTER_ID;
            panelSettings.fileName = string.Format("Odo{0}.trp.xml",
                                                    (DateTime.Now.ToString("yyyyMMddHHmmssf"))
                                                  );
            panelSettings.showCaptions = true;
            panelSettings.showFuelLocked = true;
            panelSettings.showHours = true;
            panelSettings.showInMetric = false;
            panelSettings.showMiles = true;
            panelSettings.showMPG = true;
            panelSettings.showSpeed = true;
            panelSettings.showUnits = true;
            panelSettings.TextPosition = 0;
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            if (panelSettings.TextPosition < 0 || panelSettings.TextPosition > 2)
                panelSettings.TextPosition = 0;
            ((RadioButton)OdometerAlignment.Children[panelSettings.TextPosition]).IsChecked = true;
            if (panelSettings.layoutHorizontal)
                LayoutHorizontal.IsChecked = true;
            else
                LayoutVertical.IsChecked = true;
            ShowCaptions.IsChecked = panelSettings.showCaptions;
            ShowUnits.IsChecked = panelSettings.showUnits;
            ShowInMetric.IsChecked = panelSettings.showInMetric;

            ShowDistance.IsChecked = panelSettings.showMiles;
            ShowFuel.IsChecked = panelSettings.showFuelLocked;
            ShowRunTime.IsChecked = panelSettings.showHours;
            ShowMPG.IsChecked = panelSettings.showMPG;
            ShowAverageSpeed.IsChecked = panelSettings.showSpeed;
            Locked.IsChecked = panelSettings.showFuelLocked;
        }

        protected void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton button in OdometerAlignment.Children)
                if (button.IsChecked == true) panelSettings.TextPosition = Convert.ToInt16(button.Tag);
            panelSettings.fileName = newOdoFile;
            panelSettings.layoutHorizontal = (bool)LayoutHorizontal.IsChecked;
            panelSettings.showInMetric = (bool)ShowInMetric.IsChecked;
            panelSettings.showCaptions = (bool)ShowCaptions.IsChecked;
            panelSettings.showUnits = (bool)ShowUnits.IsChecked;
            panelSettings.showMiles = (bool)ShowDistance.IsChecked;
            panelSettings.showFuelLocked = (bool)ShowFuel.IsChecked;
            panelSettings.showHours = (bool)ShowRunTime.IsChecked;
            panelSettings.showMPG = (bool)ShowMPG.IsChecked;
            panelSettings.showSpeed = (bool)ShowAverageSpeed.IsChecked;
            DialogResult = true;
            Close();
        }

        protected void LayoutRadio_Checked(object s, RoutedEventArgs e)
        {
            //var radio = s as RadioButton;
            //checkedRadio = Convert.ToInt16(radio.Tag);
        }

        protected void AlignmentRadio_Checked(object s, RoutedEventArgs e)
        {

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void ResetTripBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NOT_NULL(panel))
            {
                ((VOdometer)panel).ResetTrip();
                Close();
            }
        }

        private void ViewHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(textFile))
                OdometerManager.CreateHistoryFile(panelSettings.fileName);
            Window dlg = new HistoryViewerDlg(textFile);
            dlg.Owner = this;
            dlg.Show();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ResetToDayOne_Click(object sender, RoutedEventArgs e)
        {
            if (NOT_NULL(panel))
            {
                ((VOdometer)panel).StartFromDayOne();
                Close();
            }
        }

        private void ChangeDataFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".trp.xml",
                Filter = "Odometer Trip Files (*.trp.xml)|*.trp.xml",
                InitialDirectory = Directory.GetCurrentDirectory() + "\\configuration\\odometer_files"
            };
            if ((bool)dlg.ShowDialog() == true)
            {
                newOdoFile = dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);
            }
        }
    }
}
