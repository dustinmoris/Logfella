using System;
using System.Collections.Generic;

namespace Logfella.Extensions
{
    public static class EnumerableExtensions
    {
        public static IDictionary<TKey, TValue> ToDict<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> array)
        {
            var dict = new Dictionary<TKey, TValue>();

            foreach (var (key, value) in array)
                dict.Add(key, value);

            return dict;
        }
    }
}