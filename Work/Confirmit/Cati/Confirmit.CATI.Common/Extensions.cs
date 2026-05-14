using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Confirmit.CATI.Common
{
    public static class Extensions
    {
        private const char ENUM_SEPERATOR_CHARACTER = ',';

        public static string Description(this Enum value)
        {
            var entries = value.ToString().Split(ENUM_SEPERATOR_CHARACTER);
            var description = new string[entries.Length];
            for (var i = 0; i < entries.Length; i++)
            {
                var fieldInfo = value.GetType().GetField(entries[i].Trim());
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                description[i] = (attributes.Length > 0) ? attributes[0].Description : entries[i].Trim();
            }
            return String.Join(", ", description);
        }

        public static T[] CreateArray<T>(this T value)
        {
            return new[] { value };
        }

        public static List<T> CreateList<T>(this T value)
        {
            return new List<T> { value };
        }

        public static Func<TResult> WrapInFunc<TResult>(this Action action)
        {
            return delegate
                {
                    action();
                    return default(TResult);
                };
        }

        public static Func<T, TResult> WrapInFunc<T, TResult>(this Action<T> action)
        {
            return delegate(T arg)
                {
                    action(arg);
                    return default(TResult);
                };
        }

        public static string JoinInString(this IEnumerable<string> list, string separator)
        {
            return String.Join(separator, list.ToArray());
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            return dictionary.TryGetValue(key, out result) ? result : defaultValue;
        }
    }
}
