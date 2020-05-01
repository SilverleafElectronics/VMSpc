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
using VMSpc.UI.CustomComponents;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for ColorPalletteDlg.xaml
    /// </summary>
    public partial class ColorPaletteDlg : VMSDialog
    {
        private ColorPaletteStackItem
            selectedColorPalette;
        protected ColorPaletteStackItem SelectedColorPalette
        {
            get { return selectedColorPalette; }
            set 
            {
                if (selectedColorPalette != null)
                    selectedColorPalette.IsSelected = false;
                selectedColorPalette = value;
                if (selectedColorPalette != null)
                    selectedColorPalette.IsSelected = true;
            }
        }
        public ColorPaletteDlg()
        {
            InitializeComponent();
            LoadColorPalettes();
        }

        private void LoadColorPalettes()
        {
            DisplayedColorPalettes.Children.Clear();
            foreach (var palette in ConfigManager.ColorPalettes.Contents.ColorPaletteList)
            {
                var stackItem = new ColorPaletteStackItem(palette, this)
                {
                    NormalBackground = new SolidColorBrush(Colors.White),
                    HighlightedBackground = new SolidColorBrush(Colors.LightBlue),
                };
                DisplayedColorPalettes.Children.Add(stackItem);
                if (palette.Id == ConfigManager.Settings.Contents.selectedColorPaletteId)
                {
                    SelectedColorPalette = stackItem;
                }
            }
        }

        //Called by the child ColorPaletteStackItem on a click event
        public void HandlePaletteStackItemClicked(ColorPaletteStackItem item)
        {
            if (item != null)
            {
                SelectedColorPalette = item;
            }
        }


        public void DeletePalette(int paletteId)
        {
            ConfigManager.ColorPalettes.DeletePalette(paletteId);
            LoadColorPalettes();
        }

        public void Update()
        {
            LoadColorPalettes();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (SelectedColorPalette != null)
                ConfigManager.Settings.Contents.selectedColorPaletteId = SelectedColorPalette.colorPalette.Id;
            Close();
        }

        private void AddNewPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPalette colorPalette = new ColorPalette();
            colorPalette.LoadDefaults();
            ColorPalettePicker palettePicker = new ColorPalettePicker(colorPalette);
            palettePicker.Owner = this;
            bool dlgResult = (bool)palettePicker.ShowDialog();
            if (dlgResult)
            {
                ConfigManager.ColorPalettes.AddNewPalette(colorPalette);
                LoadColorPalettes();
            }
        }
    }
}
