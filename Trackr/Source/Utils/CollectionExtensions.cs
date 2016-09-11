using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr.Utils
{
    public static class CollectionExtensions
    {
        public static bool IsFirst<T>(this IEnumerable<T> items, T item)
        {
            T first = items.FirstOrDefault();
            if (first == null)
            {
                return false;
            }

            return item.Equals(first);
        }
    }
}