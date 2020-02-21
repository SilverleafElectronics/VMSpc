using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VMSpc.Extensions.UI
{
    public static class TextBlockExtensions
    {
        /// <summary> 
        /// Scales the given textblock to the maximum possible font size for the bounding area. An optional font size seed can 
        /// be provided for performance. Note this will automatically pad text for you by subtracting from the bounding area's height or width.
        /// </summary>
        public static void ScaleText(this TextBlock textBlock, double maxWidth, double maxHeight, int seed = 12, int maxCharacters=0)
        {
            textBlock.FontSize = seed;
            if (string.IsNullOrEmpty(textBlock.Text) && maxCharacters == 0)
                return;
            string originalText = textBlock.Text;
            if (maxCharacters > 0)
            {
                textBlock.Text = "";
                for (int i = 0; i < maxCharacters; i++)
                    textBlock.Text += "W";
            }
            Size size = CalculateStringSize(textBlock);
            SlideFontSize(textBlock, size, maxWidth, maxHeight);
            textBlock.Text = originalText;
        }

        /// <summary>
        /// Scales the text in a TextBlock using the TextBlock's dimensions as the maximum bounding area.
        /// </summary>
        /// <param name="textBlock"></param>
        public static void ScaleText(this TextBlock textBlock, int seed = 12, int maxCharacters = 0)
        {
            textBlock.ScaleText(textBlock.Width, textBlock.Height, seed, maxCharacters);
        }

        public static void SlideFontSize(TextBlock textBlock, Size size, double maxWidth, double maxHeight)
        {
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

        public static Size CalculateStringSize(TextBlock textBlock)
        {
            if (string.IsNullOrEmpty(textBlock.Text))
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
    }
}
