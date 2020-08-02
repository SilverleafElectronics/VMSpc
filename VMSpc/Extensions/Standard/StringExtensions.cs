using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Extensions.Standard
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts Pascal Case to standard sentence format. Does not support acronyms or initialisms. Should
        /// only be used to convert Property/Field/Variable names to readable text.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PascalCaseToReadable(this string str)
        {
            var newStr = string.Copy(str);
            int i = 1;
            while (i < newStr.Length)
            {
                if (!IsLowerCaseCharacter(newStr[i]))
                {
                    newStr = newStr.Insert(i, " ");
                    i++;
                }
                i++;
            }
            return newStr;
        }

        private static bool IsLowerCaseCharacter(char character)
        {
            return ((character >= 'a' && character <= 'z'));
        }
    }
}
