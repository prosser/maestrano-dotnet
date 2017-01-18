using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maestrano.Helpers
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Convert a string to snake case
        /// </summary>
        /// <param name="word">String to convert</param>
        /// <returns>string</returns>
        public static string ToSnakeCase(this string word)
        {
            return
                Regex.Replace(
                    Regex.Replace(Regex.Replace(word, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                        "$1_$2"), @"[-\s]", "_").ToLower();
        }
    }
}
