using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace ph4n.Common
{
    public static class StringEx
    {
        /// <summary>
        /// Same as Substring, but without exceptions (will instead return empty strings)
        /// </summary>
        /// <param name="input">original input string</param>
        /// <param name="startIndex">start index for substring</param>
        /// <param name="length">length of substring</param>
        /// <returns>returns substring within the input string from startIndex with length,
        /// or "" if invalid startIndex or length</returns>
        [NotNull]
        [ContractAnnotation("input: null => notnull; notnull => notnull")]
        public static string SafeSubstring([CanBeNull] this string input, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (startIndex < 0 || length < 0)
            {
                return string.Empty;
            }

            if (input.Length >= (startIndex + length))
            {
                return input.Substring(startIndex, length);
            }
            else
            {
                if (input.Length > startIndex)
                {
                    return input.Substring(startIndex);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }

    public static class CharEx
    {
        public static bool IsVulgarFraction(this char c)
        {
            //checks against the hex values of the vulgar fractions
            //see http://en.wikipedia.org/wiki/Number_Forms for values
            return (c >= 0x00BC && c <= 0x00BE) ||
                   (c >= 0x2150 && c <= 0x215E);
        }

        public static double VulgarCharToDec(this char c)
        {
            double d = 0.0;

            switch (Convert.ToInt64(c))
            {
                case 0x00BC:
                    d = .25;
                    break;
                case 0x00BD:
                    d = .5;
                    break;
                case 0x00BE:
                    d = .75;
                    break;
                case 0x2150:
                    d = 1 / 7;
                    break;
                case 0x2151:
                    d = 1 / 9;
                    break;
                case 0x2152:
                    d = .1;
                    break;
                case 0x2153:
                    d = 1 / 3;
                    break;
                case 0x2154:
                    d = 2 / 3;
                    break;
                case 0x2155:
                    d = .2;
                    break;
                case 0x2156:
                    d = .4;
                    break;
                case 0x2157:
                    d = .6;
                    break;
                case 0x2158:
                    d = .8;
                    break;
                case 0x2159:
                    d = 1 / 6;
                    break;
                case 0x215A:
                    d = 5 / 6;
                    break;
                case 0x215B:
                    d = .125;
                    break;
                case 0x215C:
                    d = .375;
                    break;
                case 0x215D:
                    d = .625;
                    break;
                case 0x215E:
                    d = .875;
                    break;
            }

            return d;
        }
    }
}
