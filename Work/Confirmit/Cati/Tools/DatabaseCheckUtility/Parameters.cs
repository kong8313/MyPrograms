using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DatabaseCheckUtility
{
    public class Parameters
    {
        public int CheckLevel = Int32.MaxValue;

        public Parameters(string[] args)
        {
            foreach (var arg in args)
            {
                var match = new Regex(@"/(cl|checklevel):(?<value>\d+)").Match(arg.ToLower());
                if (match.Success)
                    CheckLevel = Int32.Parse(match.Groups["value"].Value);
            }
        }
    }
}
