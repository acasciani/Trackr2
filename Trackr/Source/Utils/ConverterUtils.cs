using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Trackr.Utils
{
    public static class ConverterUtils
    {
        public static T? TryParse<T>(this string input) where T : struct
        {
            if (string.IsNullOrEmpty(input)) return (T?)null;
            var conv = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));

            if (conv.CanConvertFrom(typeof(string)))
            {
                try
                {
                    return (T)conv.ConvertFrom(input);
                }
                catch
                {
                    return (T?)null;
                }
            }
            return (T?)null;
        }

        public static string FromListToCSV<T>(this List<T> input) where T : struct
        {
            if (input == null)
            {
                return null;
            }

            return string.Join(",", input);
        }

        public static List<T> FromCSVToList<T>(this string input) where T : struct
        {
            List<T> results = new List<T>();

            if (input == null)
            {
                return results;
            }

            // can have nullables
            var allParsedValues = input.Split(',').Select(i => TryParse<T>(i.Trim()));

            // return only those that actually have values
            return allParsedValues.Where(i => i.HasValue).Select(i => i.Value).Distinct().ToList();
        }
    }
}