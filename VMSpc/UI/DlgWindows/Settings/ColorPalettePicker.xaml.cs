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
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for ColorPalettePicker.xaml
    /// </summary>
    public partial class ColorPalettePicker : VMSDialog
    {
        public ColorPalette colorPalette;
        public ColorPalette colorPaletteCopy;
        public ColorPalettePicker(ColorPalette colorPalette)
        {
            this.colorPalette = colorPalette;
            colorPaletteCopy = new ColorPalette();
            colorPaletteCopy.Copy(colorPalette);
            InitializeComponent();
            SetColorRects();
        }

        private void SetColorRects()
        {
            ColorPaletteNameBlock.Text = colorPaletteCopy.PaletteName;
            MainBackgroundRect.Fill = new SolidColorBrush(colorPaletteCopy.MainBackground);
            GaugeBackgroundRect.Fill = new SolidColorBrush(colorPaletteCopy.GaugeBackground);
            CaptionsRect.Fill = new SolidColorBrush(colorPaletteCopy.Captions);
            GaugeTextRect.Fill = new SolidColorBrush(colorPaletteCopy.GaugeText);
            GreenRect.Fill = new SolidColorBrush(colorPaletteCopy.Green);
            YellowRect.Fill = new SolidColorBrush(colorPaletteCopy.Yellow);
            RedRect.Fill = new SolidColorBrush(colorPaletteCopy.Red);
        }

        private Color ChangePaletteColor(Color color)
        {
            ChangeColor(ref color);
            return color;
        }
        
        private void ChangeMainBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.MainBackground = ChangePaletteColor(colorPaletteCopy.MainBackground);
            SetColorRects();
        }

        private void ChangeGaugeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.GaugeBackground = ChangePaletteColor(colorPaletteCopy.GaugeBackground);
            SetColorRects();
        }

        private void ChangeCaptionsButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.Captions = ChangePaletteColor(colorPaletteCopy.Captions);
            SetColorRects();
        }

        private void ChangeGaugeTextButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.GaugeText = ChangePaletteColor(colorPaletteCopy.GaugeText);
            SetColorRects();
        }

        private void ChangeGreenButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.Green = ChangePaletteColor(colorPaletteCopy.Green);
            SetColorRects();
        }

        private void ChangeYellowButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.Yellow = ChangePaletteColor(colorPaletteCopy.Yellow);
            SetColorRects();
        }

        private void ChangeRedButton_Click(object sender, RoutedEventArgs e)
        {
            colorPaletteCopy.Red = ChangePaletteColor(colorPaletteCopy.Red);
            SetColorRects();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            colorPalette.Copy(colorPaletteCopy);
            colorPalette.PaletteName = ColorPaletteNameBlock.Text;
            Close();
        }
    }
}
