using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VMSpc.DlgWindows;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for TextPanelDlg.xaml
    /// </summary>
    public partial class TextPanelDlg : VPanelDlg
    {
        protected new TextGaugeSettings panelSettings;
        public TextPanelDlg(TextGaugeSettings panelSettings)
            : base(panelSettings)
        {
            InitializeComponent();
            ApplyBindings();
        }

        protected override void ApplyDefaults()
        {
            base.ApplyDefaults();
            panelSettings.panelId = VEnum.UI.PanelType.TEXT;
            panelSettings.text = "Your Message";
        }

        protected override void ApplyBindings()
        {
            TextEditor.Text = panelSettings.text;
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (TextGaugeSettings)base.panelSettings;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            panelSettings.text = TextEditor.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
    }
}
