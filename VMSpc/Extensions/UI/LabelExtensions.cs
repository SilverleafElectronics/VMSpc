using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace VMSpc.Extensions.UI
{
    public static class LabelExtensions
    {
        public static void ScaleText(this Label label, double maxWidth, double maxHeight, int maxCharacters = 0)
        {
            //if the label is empty or if the label's content is not a string, don't scale
            if ((label.Content == null && maxCharacters == 0) || (label.Content as string) == null)
                return;
            string originalText = label.Content.ToString();
            if (maxCharacters > 0)
            {
                StringBuilder text = new StringBuilder();
                for (int i = 0; i < maxCharacters; i++)
                    text.Append('W');
                label.Content = text.ToString();
            }
            Size size = CalculateStringSize(label);
            SlideFontSize(label, size, (maxWidth * 3/4), maxHeight);
            label.Content = originalText;
        }

        public static void BalanceTextBlocks(this UIElementCollection collection)
        {
            double maxFontSize = double.MaxValue;
            foreach (var child in collection)
            {
                if (child.GetType() == typeof(Label))
                {
                    if ((child as Label).FontSize < maxFontSize)
                    {
                        maxFontSize = (child as Label).FontSize;
                    }
                }
                else if (child.GetType() == typeof(TextBlock))
                {
                    if ((child as TextBlock).FontSize < maxFontSize)
                    {
                        maxFontSize = (child as TextBlock).FontSize;
                    }
                }
            }
            foreach (var child in collection)
            {
                if (child.GetType() == typeof(Label))
                {
                    (child as Label).FontSize = maxFontSize;
                }
                else if (child.GetType() == typeof(TextBlock))
                {
                    (child as TextBlock).FontSize = maxFontSize;
                }
            }
        }

        /// <summary>
        /// Scales the TextBlock's font to the maximum size allowed given the text length and the TextBlock's dimensions
        /// </summary>
        /// <param name="textBlock"></param>
        public static void ScaleText(this Label label, int maxCharacters = 0)
        {
            label.ScaleText(label.Width, label.Height, maxCharacters);
        }

        public static void SlideFontSize(Label label, Size size, double maxWidth, double maxHeight)
        {
            while (size.Width < (maxWidth) && size.Height < (maxHeight))
            {
                label.FontSize++;
                size = CalculateStringSize(label);
            }
            while (label.FontSize > 2 && (size.Width > maxWidth || size.Height > maxHeight))
            {
                label.FontSize--;
                size = CalculateStringSize(label);
            }
            label.FontSize--;
        }

        public static Size CalculateStringSize(Label label)
        {
            string content = label.Content as string;
            if (content == null)
                return new Size(0, 0);
            if (string.IsNullOrEmpty(content))
                return new Size(0, 0);
            FormattedText text = new FormattedText(
                content,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);
            return new Size(text.Width, text.Height);
        }
    }
}
