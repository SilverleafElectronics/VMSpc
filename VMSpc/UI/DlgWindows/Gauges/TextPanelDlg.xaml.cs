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
using VMSpc.UI.DlgWindows;
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
            panelSettings.panelId = Enums.UI.PanelType.TEXT;
            panelSettings.text = "Your Message";
        }

        protected override void ApplyBindings()
        {
            ((RadioButton)RadioAlignment.Children[(int)panelSettings.alignment]).IsChecked = true;
            TextEditor.Text = panelSettings.text;
            BackgroundColor = panelSettings.BackgroundColor;
            BorderColor = panelSettings.BorderColor;
            TextColor = panelSettings.CaptionColor;
            UseGlobalColor.IsChecked = panelSettings.useGlobalColorPalette;
        }

        protected void Radio_Checked(object s, RoutedEventArgs e)
        {
            //var radio = s as RadioButton;
            //checkedRadio = Convert.ToInt16(radio.Tag);
        }

        protected override void Init(PanelSettings panelSettings)
        {
            this.panelSettings = (TextGaugeSettings)base.panelSettings;
        }

        private void ChangeBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundColor = panelSettings.BackgroundColor;
            if (ChangeColor(ref BackgroundColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void ChangeBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            BorderColor = panelSettings.BorderColor;
            if (ChangeColor(ref BorderColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void ChangeTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            TextColor = panelSettings.CaptionColor;
            if (ChangeColor(ref TextColor))
            {
                panelSettings.useGlobalColorPalette = false;
                UseGlobalColor.IsChecked = false;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton button in RadioAlignment.Children)
                if (button.IsChecked == true) panelSettings.alignment = (HorizontalAlignment)Convert.ToInt16(button.Tag);    //TODO: SimpleGaugeDlg:1
            panelSettings.text = TextEditor.Text;
            panelSettings.BackgroundColor = BackgroundColor;
            panelSettings.BorderColor = BorderColor;
            panelSettings.CaptionColor = TextColor;
            panelSettings.useGlobalColorPalette = (bool)UseGlobalColor.IsChecked;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UseGlobalColor_Checked(object sender, RoutedEventArgs e)
        {
            ApplyGlobalColorPalette();
        }
    }
}
