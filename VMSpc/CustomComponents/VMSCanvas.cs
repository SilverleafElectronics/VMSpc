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
using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.DevHelpers;
using System.Globalization;

namespace VMSpc.CustomComponents
{
    public class VMSCanvas : Canvas
    {
        

        public VMSCanvas() : base(){}

        ~VMSCanvas()
        {
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        /// <summary> 
        /// Scales the given textblock to the maximum possible font size for the bounding area. An optional font size seed can 
        /// be provided for performance. Note this will automatically pad text for you by subtracting from the bounding area's height or width
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

        /// <summary> 
        /// Takes a parent element and makes a best-effort attempt at balancing all child text blocks to the same font size. 
        /// NOTE: using this method can have unexpected results if there are various levels of nesting. It's advisable to use
        /// The overloaded BalanceTextBlocks instead
        /// </summary>
        public void BalanceTextBlocks(dynamic parent)
        {
            double min = Double.MaxValue;

            foreach (var block in parent.Children)
            {
                if (block.GetType().ToString() == "System.Windows.Controls.TextBlock")
                {
                    if (((TextBlock)block).FontSize < min)
                        min = ((TextBlock)block).FontSize;
                }
                else if (block.GetType().ToString() == "System.Windows.Controls.Border")
                {
                    if (((Border)block).Child.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    {
                        TextBlock textBlock = (TextBlock)((Border)block).Child;
                        if (textBlock.FontSize < min)
                            min = textBlock.FontSize;
                    }
                }
            }
            foreach (var block in parent.Children)
            {
                if (block.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    ((TextBlock)block).FontSize = min;
                else if (block.GetType().ToString() == "System.Windows.Controls.Border")
                {
                    if (((Border)block).Child.GetType().ToString() == "System.Windows.Controls.TextBlock")
                    {
                        TextBlock textBlock = (TextBlock)((Border)block).Child;
                        textBlock.FontSize = min;
                    }
                }
            }
        }

        /// <summary> Balances all text blocks in provided List to the same font size.This overload is safer than BalanceTextBlocks(dynamic parent) </summary>
        public void BalanceTextBlocks(List<TextBlock> blocks)
        {
            double min = Double.MaxValue;
            foreach (TextBlock block in blocks)
            {
                if (block.FontSize < min)
                    min = block.FontSize;
            }
            foreach (TextBlock block in blocks)
            {
                block.FontSize = min;
            }
        }
    }
}
