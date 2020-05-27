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
    public static class TextBlockExtensions
    {
        /// <summary> 
        /// Scales the given textblock to the maximum possible font size for the bounding area. An optional font size seed can 
        /// be provided for performance. Note this will automatically pad text for you by subtracting from the bounding area's height or width.
        /// </summary>
        public static void ScaleText(this TextBlock textBlock, double maxWidth, double maxHeight, int maxCharacters=0)
        {
            textBlock.FontSize = 12;
            if (string.IsNullOrEmpty(textBlock.Text) && maxCharacters == 0)
                return;
            //textBlock.FontSize = CalculateSeed(textBlock);
            string originalText = textBlock.Text;
            if (maxCharacters > 0)
            {
                StringBuilder text = new StringBuilder();
                for (int i = 0; i < maxCharacters; i++)
                    text.Append('W');
                textBlock.Text = text.ToString();
            }
            Size size = CalculateStringSize(textBlock);
            SlideFontSize(textBlock, size, maxWidth, maxHeight);
            textBlock.Text = originalText;
        }

        /// <summary>
        /// Scales the TextBlock's font to the maximum size allowed given the text length and the TextBlock's dimensions
        /// </summary>
        /// <param name="textBlock"></param>
        public static void ScaleText(this TextBlock textBlock, int maxCharacters = 0)
        {
            textBlock.ScaleText(textBlock.Width, textBlock.Height, maxCharacters);
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

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        //public static double CalculateSeed(TextBlock textBlock, double maxWidth, double maxHeight)
        //{
        //    var originalText = textBlock.Text;
        //    textBlock.Text = "W";
        //    var size = new Size(10, 10);
        //    while (textBlock.FontSize > 2 && (size.Width > maxWidth || size.Height > maxHeight))
        //    {
        //        textBlock.FontSize--;
        //        size = CalculateStringSize(textBlock);
        //    }
        //    while (size.Width < (maxWidth) && size.Height < (maxHeight))
        //    {
        //        textBlock.FontSize++;
        //        size = CalculateStringSize(textBlock);
        //    }
        //    textBlock.FontSize--;
        //    textBlock.Text = originalText;
        //}

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
