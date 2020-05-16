using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Extensions.Standard
{
    public static class ListExtensions
    {
        /// <summary>
        /// Modifies the list to contain only elements from the startIndex to the end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="startIndex"></param>
        public static List<T> SubList<T>(this List<T> list, int startIndex)
        {
            var newList = list.ToList();
            newList.RemoveRange(0, startIndex);
            return newList;
        }

        /// <summary>
        /// Modifies the list to contain only elements from the startIndex to the startIndex + count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public static List<T> SubList<T>(this List<T> list, int startIndex, int count)
        {
            var countToEnd = list.Count - (startIndex + count);
            var newList = list.ToList();
            newList.RemoveRange(0, startIndex);
            newList.RemoveRange(startIndex + count, countToEnd);
            return newList;
        }

        public static string ToHexString(this List<byte> list)
        {
            var hexString = new StringBuilder();
            hexString.Append("0x");
            foreach (var val in list)
            {
                hexString.Append(string.Format("{0:X2}", val));
            }
            return hexString.ToString();
        }
    }
}
