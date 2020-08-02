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
using VMSpc.UI.DlgWindows;
using VMSpc.Panels;
using VMSpc.DevHelpers;
using System.Globalization;

namespace VMSpc.UI.CustomComponents
{
    public class VMSCanvas : Canvas
    {
        public event Action<object, MouseButtonEventArgs> MouseDoubleClickEvent;
        public VMSCanvas() : base()
        {
            MouseLeftButtonDown += PollDoubleClick;
        }

        ~VMSCanvas()
        {
        }

        /// <summary> 
        /// Trigger for our "Custom" canvas event, since for some mind-boggling
        /// reason the WPF canvas doesn't support a double-click event by default
        /// </summary>
        private void PollDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                MouseDoubleClickEvent?.Invoke(sender, e);
        }

        /// <summary> 
        /// Scales the given textblock to the maximum possible font size for the bounding area. An optional font size seed can 
        /// be provided for performance. Note this will automatically pad text for you by subtracting from the bounding area's height or width.
        /// </summary>
        public void ScaleText(TextBlock textBlock, double maxWidth, double maxHeight, int seed = 12)
        {
            textBlock.FontSize = seed;
            Size size = CalculateStringSize(textBlock);
            while (textBlock.FontSize > 2 && (size.Width > maxWidth || size.Height > maxHeight))
            {
                textBlock.FontSize--;
                size = CalculateStringSize(textBlock);
            }
            while (size.Width < (maxWidth) && size.Height < (maxHeight))
            {
                textBlock.FontSize++;
                size = CalculateStringSize(textBlock);
            }
            textBlock.FontSize--;
        }

        /// <summary>
        /// Scales the text in a TextBlock using the TextBlock's dimensions as the maximum bounding area.
        /// </summary>
        /// <param name="textBlock"></param>
        public void ScaleText(TextBlock textBlock)
        {
            ScaleText(textBlock, textBlock.Width, textBlock.Height);
        }

        public Size CalculateStringSize(TextBlock textBlock)
        {
            if (textBlock.Text == "")
                return new Size(0, 0);
            FormattedText text = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);
            return new Size(text.Width, text.Height);
        }

        protected void AddChildren(params UIElement[] list)
        {
            foreach (var child in list)
                Children.Add(child);
        }
    }
}
