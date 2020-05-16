using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Extensions.Standard
{
    public static class ArrayExtensions
    {
        public static void Prepend<T>(this T[] array, T value)
        {
            T[] newArray = new T[array.Length + 1];
            newArray[0] = value;
            Array.Copy(array, 0, newArray, 1, array.Length);
            array = newArray;
        }
    }
}
