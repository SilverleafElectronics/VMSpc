using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using VMSpc.DevHelpers;
using VMSpc.UI.DlgWindows;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.CustomComponents
{
    public class ColorPaletteStackItem : StackPanel
    {
        public ColorPalette 
            colorPalette;
        public TextBlock
            Title;
        public Rectangle
            MainBackground,
            GaugeBackground,
            Captions,
            GaugeText,
            Green,
            Yellow,
            Red;
        private ColorPaletteDlg
            parent;
        public SolidColorBrush NormalBackground;
        public SolidColorBrush HighlightedBackground;
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                if (isSelected)
                    Background = HighlightedBackground;
                else
                    Background = NormalBackground;
            }
        }

        public ColorPaletteStackItem(ColorPalette colorPalette, ColorPaletteDlg parent)
            :base()
        {
            NormalBackground = new SolidColorBrush(Colors.White);
            HighlightedBackground = new SolidColorBrush(Colors.LightBlue);
            Orientation = Orientation.Horizontal;
            this.colorPalette = colorPalette;
            IsSelected = false;
            Title = new TextBlock()
            {
                Text = colorPalette.PaletteName,
                Width = 200,
                TextWrapping = TextWrapping.Wrap,
            };
            Children.Add(Title);
            MainBackground = new Rectangle() { Fill = new SolidColorBrush(colorPalette.MainBackground) };
            GaugeBackground = new Rectangle() { Fill = new SolidColorBrush(colorPalette.GaugeBackground) };
            Captions = new Rectangle() { Fill = new SolidColorBrush(colorPalette.Captions) };
            GaugeText = new Rectangle() { Fill = new SolidColorBrush(colorPalette.GaugeText) };
            Green = new Rectangle() { Fill = new SolidColorBrush(colorPalette.Green) };
            Yellow = new Rectangle() { Fill = new SolidColorBrush(colorPalette.Yellow) };
            Red = new Rectangle() { Fill = new SolidColorBrush(colorPalette.Red) };
            this.parent = parent;
            AddColorRect(MainBackground);
            AddColorRect(GaugeBackground);
            AddColorRect(Captions);
            AddColorRect(GaugeText);
            AddColorRect(Green);
            AddColorRect(Yellow);
            AddColorRect(Red);
            ContextMenu //= new ContextMenu();
            //ContextMenu.ContextMenu
            = CreateContextMenu();
            MouseLeftButtonUp += ColorPaletteStackItem_MouseLeftButtonUp;
        }

        private void ColorPaletteStackItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            parent.HandlePaletteStackItemClicked(this);
        }

        public class EditCommand : System.Windows.Input.ICommand
        {
            private readonly Predicate<object> _canExecute;
            private readonly Action<object> _execute;

            public event EventHandler CanExecuteChanged;
            public EditCommand(Predicate<object> canExecute, Action<object> execute)
            {
                CanExecuteChanged?.Invoke(canExecute, new EventArgs());
                _canExecute = canExecute;
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }

        public class DeleteCommand : System.Windows.Input.ICommand
        {
            private readonly Predicate<object> _canExecute;
            private readonly Action<object> _execute;

            public event EventHandler CanExecuteChanged;
            public DeleteCommand(Predicate<object> canExecute, Action<object> execute)
            {
                CanExecuteChanged?.Invoke(canExecute, new EventArgs());
                _canExecute = canExecute;
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }

        private ContextMenu CreateContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem editItem = new MenuItem()
            {
                Header = "Edit",
                Command = new EditCommand(CanExecuteCommand, ExecuteEditCommand)
            };
            MenuItem deleteItem = new MenuItem()
            {
                Header = "Delete",
                Command = new DeleteCommand(CanExecuteCommand, ExecuteDeleteCommand)
            };
            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(deleteItem);
            return contextMenu;
        }

        public bool CanExecuteCommand(object parameter)
        {
            return true;
        }

        public void ExecuteEditCommand(object parameter)
        {
            ColorPalettePicker palettePicker = new ColorPalettePicker(colorPalette);
            palettePicker.Owner = parent;
            bool dlgResult = (bool)palettePicker.ShowDialog();
            if (dlgResult)
            {
                parent.Update();
            }
        }

        public void ExecuteDeleteCommand(object parameter)
        {
            if (
                (
                MessageBox.Show
                ($"Are you sure you want to delete the color palette, \"{colorPalette.PaletteName}\"?",
                $"Delete {colorPalette.PaletteName}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question)
                )
                == MessageBoxResult.Yes
               )
            {
                parent.DeletePalette(colorPalette.Id);
            }
        }

        private void AddColorRect(Rectangle rect)
        {
            Border border = new Border()
            {
                Width = 385/7,
                Child = rect,
                BorderBrush = new SolidColorBrush(Colors.LightBlue),
                BorderThickness = new Thickness(1),
            };
            Children.Add(border);
        }
    }
}
