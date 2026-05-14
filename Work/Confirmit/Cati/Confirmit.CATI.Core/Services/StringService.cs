using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Services
{
    public class StringService
    {
        public static string Join(string separator, string wrapper, IEnumerable<string> strings)
        {
            return String.Join(separator, strings.Select(x => String.Format(wrapper, x)).ToArray());
        }

        public static string Join<T>(string separator, Func<T, string>wrapper, IEnumerable<T> data)
        {
            return String.Join(separator, data.Select(wrapper).ToArray());
        }
    }
}
