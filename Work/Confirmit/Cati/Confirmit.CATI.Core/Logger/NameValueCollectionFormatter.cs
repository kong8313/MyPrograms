using System;
using System.Collections.Specialized;

namespace Confirmit.CATI.Core.Logger
{
    public class NameValueCollectionFormatter
    {
        public static string FormatToString(NameValueCollection collection)
        {
            string result = Environment.NewLine;
            foreach (var key in collection.AllKeys)
            {
                if (key != null && key.ToUpperInvariant().Contains("VIEWSTATE") == false)
                {
                    result += string.Format("{0} = {1}" + Environment.NewLine, key, collection[key]);
                }
            }

            return result;
        }
    }
}