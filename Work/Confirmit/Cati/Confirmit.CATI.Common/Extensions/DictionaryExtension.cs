using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Common
{
    public static class DictionaryExtension
    {
        public static string Stringify<TKey, TVal>(this Dictionary<TKey, TVal> dictionary)
        {
            return $"{{ {string.Join(", ", dictionary.Select(x => $"{{{x.Key}:{x.Value}}}"))} }}";
        }
    }
}