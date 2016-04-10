using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr.Utils
{
    public static class DBUtils
    {
        public static double? ToNullableDouble(this object input)
        {
            if (input == null)
            {
                return (double?)null;
            }

            double _try;
            return double.TryParse(input.ToString(), out _try) ? _try : (double?)null;
        }

        public static DateTime? ToNullableDateTime(this object input)
        {
            if (input == null)
            {
                return (DateTime?)null;
            }

            DateTime _try;
            return DateTime.TryParse(input.ToString(), out _try) ? _try : (DateTime?)null;
        }

        public static string ToNullableString(this object input)
        {
            if (input == null)
            {
                return null;
            }

            return input.ToString();
        }
    }
}