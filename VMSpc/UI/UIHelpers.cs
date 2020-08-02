using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VMSpc.UI
{
    class UIHelpers
    {
        public static void BalanceTextblocks(params TextBlock[] blocks)
        {
            double min = Double.MaxValue;
            foreach (TextBlock block in blocks)
            {
                if ( (block != null)  && (!string.IsNullOrEmpty(block.Text)) && (block.FontSize > 0) && (block.FontSize < min))
                    min = block.FontSize;
            }
            foreach (TextBlock block in blocks)
            {
                if (block != null)
                    block.FontSize = min;
            }
        }

        /// <summary>
        /// Returns the Minimum of any number of doubles. Placed here because this is currently only used
        /// for comparing font sizes.
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static double Min(params double[] values)
        {
            double max = double.MaxValue;
            foreach (var value in values)
            {
                if (value < max)
                {
                    max = value;
                }
            }
            return max;
        }
    }
}
