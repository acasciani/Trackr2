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
    }
}