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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq.Expressions;
//using System.Drawing;
using VMSpc.Extensions.UI;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.CustomComponents
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public enum SortByType
        {
            Hue,
            Brightness,
            Name,
            RGB
        };
        public Dictionary<string, Color> ColorDictionary;
        public Color SelectedColor { get; internal set; }
        public string SelectedColorName { get; set; }
        private SolidColorBrush NormalBackground;
        private SolidColorBrush HighlightedBackground;
        private StackPanel SelectedElement;
        private SortByType sortByType;

        public ColorPicker(Color SelectedColor)
        {
            NormalBackground = new SolidColorBrush(Colors.White);
            HighlightedBackground = new SolidColorBrush(Colors.LightBlue);
            this.SelectedColor = SelectedColor;
            InitializeComponent();
        }

        private void GenerateColorGrid()
        {
            ColorDictionary = ConfigManager.ColorReader.Contents.ColorDictionary;
            SortDictionaryByColor();
            AddDictionaryToGrid();
        }

        private void Reset()
        {
            GenerateColorGrid();
        }

        private void SortDictionaryByColor()
        {
            Func<Func<KeyValuePair<string, Color>, double>, IOrderedEnumerable<KeyValuePair<string, Color>>> func;
            if ((string)(SortOrderSelector?.SelectedItem as ComboBoxItem)?.Tag == "Descending")
                func = ColorDictionary.OrderByDescending;
            else
                func = ColorDictionary.OrderBy;
            switch (sortByType)
            {
                case SortByType.Hue:
                    ColorDictionary = func(pair => HueComparator(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
                    break;
                case SortByType.Brightness:
                    ColorDictionary = func(pair => BrightnessComparator(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
                    break;
                case SortByType.RGB:
                    ColorDictionary = func(pair => RGBComparator(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
                    break;
                case SortByType.Name:
                    if ((string)(SortOrderSelector.SelectedItem as ComboBoxItem).Tag == "Ascending")
                        ColorDictionary = ColorDictionary.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
                    else
                        ColorDictionary = ColorDictionary.OrderByDescending(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
                    break;
            }
        }

        private double HueComparator(Color color)
        {
            double hue = 0;
            double max = MaxByte(color.R, color.G, color.B);
            double min = MinByte(color.R, color.G, color.B);
            if (max == min) //black or white
            {
                if (max == 0) 
                    return 0; //white
                else
                    return double.MaxValue; //black
            }
            else if (max == color.R) //red is maximum
            {
                hue = (color.G - color.B) / (max - min);
            }
            else if (max == color.G) //green is maximum
            {
                hue = 2.0 + (color.B - color.R) / (max - min);
            }
            else //blue is maximum
            {
                hue = 4.0 + (color.R - color.G) / (max - min);
            }
            hue *= 60; //convert to degrees
            if (hue < 0)
                hue += 360;
            return hue;
        }

        private static byte MaxByte(params byte[] bytes)
        {
            byte max = byte.MinValue;
            foreach (var value in bytes)
            {
                if (value > max)
                    max = value;
            }
            return max;
        }

        private static byte MinByte(params byte[] bytes)
        {
            byte min = byte.MaxValue;
            foreach (var value in bytes)
            {
                if (value < min)
                    min = value;
            }
            return min;
        }

        private double BrightnessComparator(Color color)
        {
            return (color.R * 299 + color.G * 587 + color.B * 114);
        }

        private double RGBComparator(Color color)
        {
            return (color.R * 1000 + color.G * 100 + color.B * 10);
        }

        private void AddDictionaryToGrid()
        {
            ColorPickerGrid.Children.Clear();
            int row = -1;
            int col = 0;
            foreach (var colorItem in ColorDictionary)
            {
                if (col == 0)
                {
                    ColorPickerGrid.RowDefinitions.Add(
                        new RowDefinition()
                        {
                            Height = new GridLength(1, GridUnitType.Star)
                        }
                        );
                    row++;
                }
                GetColorPickerItem(colorItem.Key, colorItem.Value, col, row);
                col = (col + 1) % 3;
            }
            if (SelectedElement != null)
            {

                //var ip = (ItemsPresenter)ColorPickerScroller.Content;
                //var point = SelectedElement.TranslatePoint(new Point() - (Vector)SelectedElement.GetPosition(ColorPickerScroller), ip);
                //sv.ScrollToVerticalOffset(point.Y + (SelectedElement.ActualHeight / 2));
            }

        }

        private UIElement GetColorPickerItem(string colorName, Color color, int column, int row)
        {
            StackPanel block = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            ColorPickerGrid.Children.Add(block);
            Grid.SetColumn(block, column);
            Grid.SetRow(block, row);
            Border border = new Border()
            { 
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(2),
            };
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(color),
                Width = 40,
                Height = 40,
                VerticalAlignment = VerticalAlignment.Center,
            };
            border.Child = rectangle;
            TextBlock textBlock = new TextBlock()
            {
                Text = colorName,
                Height = 40,
                Width = 350,
                VerticalAlignment = VerticalAlignment.Center,
            };
            block.Children.Add(textBlock);
            block.Children.Add(border);
            textBlock.ScaleText();
            if (color.Equals(SelectedColor))
            {
                SelectedElement = block;
            }
            block.Background = (block == SelectedElement) ? HighlightedBackground : NormalBackground;
            block.MouseDown += (object obj, MouseButtonEventArgs args) =>
            {
                SwapSelectedBlock(block, colorName, color);
            };
            return block;
        }

        private void SwapSelectedBlock(StackPanel newBlock, string colorName, Color color)
        {
            if (SelectedElement != null)
            {
                SelectedElement.Background = NormalBackground;
            }
            SelectedElement = newBlock;
            SelectedElement.Background = HighlightedBackground;
            SelectedColorName = colorName;
            SelectedColor = color;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void SortBySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((string)(SortBySelector.SelectedItem as ComboBoxItem).Tag)
            {
                case "Hue":
                    sortByType = SortByType.Hue;
                    break;
                case "Brightness":
                    sortByType = SortByType.Brightness;
                    break;
                case "Name":
                    sortByType = SortByType.Name;
                    break;
                case "RGB":
                    sortByType = SortByType.RGB;
                    break;
                default:
                    sortByType = SortByType.Hue;
                    break;
            }
            GenerateColorGrid();
        }

        private void SortOrderSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateColorGrid();
        }

        private void EditColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedColorName))
            {
                var dlg = new ColorEditor(SelectedColorName, SelectedColor)
                { 
                    Owner = this
                };
                if ((bool)dlg.ShowDialog() && !string.IsNullOrEmpty(dlg.ColorName))
                {
                    if (ConfigManager.ColorReader.Contents.ColorDictionary.ContainsKey(dlg.ColorName))
                    {
                        MessageBox.Show($"{dlg.ColorName} already exists in your color palette.");
                    }
                    else
                    {
                        ConfigManager.ColorReader.Contents.ColorDictionary.Remove(SelectedColorName);
                        ConfigManager.ColorReader.Contents.ColorDictionary.Add(dlg.ColorName, dlg.Color);
                        SelectedColorName = dlg.ColorName;
                        SelectedColor = dlg.Color;
                    }
                }
            }
            Reset();
        }

        private void CreateColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorEditor("New Color", Colors.White)
            { 
                Owner = this
            };
            if ((bool)dlg.ShowDialog())
            {
                if (!string.IsNullOrEmpty(dlg.ColorName))
                {
                    if (ConfigManager.ColorReader.Contents.ColorDictionary.ContainsKey(dlg.ColorName))
                    {
                        bool confirm = MessageBox.Show($"The color {dlg.ColorName} is already defined. Do you want to replace it?",
                            $"Replace {dlg.ColorName}",
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                        if (confirm)
                        {
                            ConfigManager.ColorReader.Contents.ColorDictionary[dlg.ColorName] = dlg.Color;
                        }
                    }
                    else
                    {
                        ConfigManager.ColorReader.Contents.ColorDictionary.Add(dlg.ColorName, dlg.Color);
                    }
                    SelectedColorName = dlg.ColorName;
                    SelectedColor = dlg.Color;
                }
            }
            Reset();
        }

        private void DeleteColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDictionary = ConfigManager.ColorReader.Contents.ColorDictionary;
            if (!string.IsNullOrEmpty(SelectedColorName) && ColorDictionary.ContainsKey(SelectedColorName))
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete {SelectedColorName}? This action is not reversible.",
                    $"Delete {SelectedColorName}",
                    MessageBoxButton.YesNo
                    ) == MessageBoxResult.Yes;
                if (confirm)
                {
                    ColorDictionary.Remove(SelectedColorName);
                    SelectedColorName = null;
                    
                }
            }
            Reset();
        }
    }
}
