using System;
using System.Web;

namespace Confirmit.CATI.Core.Misc
{
    /// <summary>
    /// A cache based on a HttpContext.Current.Items collection. It is maintained only during the processing of a single HTTP request.
    /// </summary>
    public static class HttpContextCache
    {
        public static T Get<T>(object key, Func<T> valueProvider)
        {
            if (HttpContext.Current != null)
            {
                if (!HttpContext.Current.Items.Contains(key))
                {
                    HttpContext.Current.Items.Add(key, valueProvider());
                }

                return (T)HttpContext.Current.Items[key];
            }

            return valueProvider();
        }

        public static void Set(object key, object value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[key] = value;
            }
        }
    }
}