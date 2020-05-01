using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VMSpc.Extensions.UI
{
    public static class TextAlignmentExtensions
    {
        public static HorizontalAlignment ToTextAlignment(this TextAlignment value)
        {
            switch (value)
            {
                case TextAlignment.Left:
                    return HorizontalAlignment.Left;
                case TextAlignment.Center:
                    return HorizontalAlignment.Center;
                case TextAlignment.Right:
                    return HorizontalAlignment.Right;
                default:
                    return HorizontalAlignment.Left;
            }
        }
    }
    public static class HorizontalAlignmentExtensions
    {
        public static TextAlignment ToHorizontalAlignment(this HorizontalAlignment value)
        {
            switch (value)
            {
                case HorizontalAlignment.Left:
                    return TextAlignment.Left;
                case HorizontalAlignment.Center:
                    return TextAlignment.Center;
                case HorizontalAlignment.Right:
                    return TextAlignment.Right;
                default:
                    return TextAlignment.Left;
            }
        }
    }
}
