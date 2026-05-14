using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Misc
{
    public class DelimitedStringCleaner
    {
        public IEnumerable<string> ParseString(string inputString)
        {
            if (inputString == null)
            {
                return new string[] {};
            }

            return inputString
                .Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(y => !String.IsNullOrEmpty(y));
        }

        public string CleanString(string inputString)
        {
            var emails = ParseString(inputString).ToArray();
            return string.Join(";", emails);
        }
    }
}