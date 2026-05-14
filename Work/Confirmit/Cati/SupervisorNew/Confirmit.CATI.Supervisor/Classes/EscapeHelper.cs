using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class EscapeHelper
    {
        public string EscapeString(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            var result = WebUtility.UrlEncode(line);

            result = result.Replace("(", "%28");
            result = result.Replace(")", "%29");

            return result;
        }

        public Dictionary<string, string> EscapeParameters(Dictionary<string, string> parameters)
        {
            var result = new Dictionary<string, string>();

            foreach (var parameter in parameters)
            {
                result[EscapeString(parameter.Key)] = EscapeString(parameter.Value);
            }

            return result;
        }

        public List<Tuple<string, string>> EscapeParameters(NameValueCollection parameters)
        {
            var result = new List<Tuple<string, string>>();

            foreach (var key in parameters.AllKeys)
            {
                var escapedKey = EscapeString(key);
                var escapedValue = EscapeString(parameters[key]);

                result.Add(new Tuple<string, string>(escapedKey, escapedValue));
            }

            return result;
        }
    }
}