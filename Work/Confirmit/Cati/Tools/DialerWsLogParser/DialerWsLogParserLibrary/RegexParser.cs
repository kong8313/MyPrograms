using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DialerWsLogParserLibrary
{
    public class RegexParser
    {
        private string _row;

        public RegexParser (string str = "")
        {
            _row = str;
        }

        public string FindParameterByRegex(string expression)
        {
            string result = string.Empty;
            var regex = new Regex(expression, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(_row);

            if (matches.Count > 0)
            {
                var regexToDelete = new Regex(@"\D");
                result = regexToDelete.Replace(matches[0].Value, string.Empty);
            }

            return result;
        }

        public string FindManyParametersByRegex(string expression)
        {
            string result = string.Empty;
            var regex = new Regex(expression, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(_row);
            var listMatches = new List<string>();

            foreach (var match in matches)
                listMatches.Add(match.ToString());
            
            var distinctMatches = listMatches.Distinct().ToList<string>();
            var regexToDelete = new Regex(@"\D");

            foreach (var match in distinctMatches)
                result += regexToDelete.Replace(match.ToString(), string.Empty) + ", ";

            if (result != string.Empty)
                result = result.Remove(result.Length - 2);
            return result;
        }

        public string FindTimeByRegex()
        {
            var regex = new Regex(@"\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d\.\d\d\d");
            string result = string.Empty;

            MatchCollection matches = regex.Matches(_row);
            if (matches.Count > 0)
                result = matches[0].Value;

            return result;
        }

        public string ExtractName()
        {
            string result = FindNameByRegex();
            if (Regex.IsMatch(result, @"<.*>.*"))
            {
                result = result.Trim(new char[] { '<' });
                result = result.Substring(0, result.IndexOf('>'));
            }

            int errIndex = _row.IndexOf("Error");
            if (errIndex > -1)
            {
                if (result != string.Empty && Regex.IsMatch(_row, @"^DialerService Error:"))
                    result += " (Error)";
                else if (result == string.Empty)
                    result = "Error";
            }

            return result;
        }

        public long FindRequestId()
        {
            var regex = new Regex(@"\[rid=\d+\]");

            MatchCollection matches = regex.Matches(_row);
            if (matches.Count > 0)
                return long.Parse(Regex.Replace(matches[0].Value, @"[^0-9]", ""));
            else
                return -1;
        }

        private string FindNameByRegex()
        {
            var regex = new Regex(@"\t\S+\s");
            string result = string.Empty;

            MatchCollection matches = regex.Matches(_row);
            if (matches.Count > 0)
                result = matches[0].Value.Trim(new char[] { ' ', '\t' });

            return result;
        }
    }
}
