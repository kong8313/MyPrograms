using System;
using System.Text.RegularExpressions;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.Security
{
    public class DataValidationManager
    {
        /// <summary>
        /// Checks parameter value for sql injection. 
        /// If value is incorrect throws UserMessageException
        /// </summary>
        /// <param name="value">Value for validation</param>
        public static void CheckForSqlInjection(string value)
        {
            if (IsStringValid(value) == false)
            {
                throw new UserMessageException($"Using of keyword \"{value}\" is not permitted");
            }
        }

        /// <summary>
        /// Function checks parameter value for sql injection. 
        /// Returns true if value is correct otherwise returns false.
        /// </summary>
        /// <param name="value">Value for validation</param>
        public static bool IsStringValid(string value)
        {
            var blackSymbolList = new string[] { "--", ";", "/*", "*/", "@@", "'" };

            var blackWordList = new[]{
                                              "char", "nchar", "varchar", "nvarchar",
                                              "alter", "begin", "cast", "create", "cursor",
                                              "declare", "delete", "drop", "end", "exec",
                                              "execute", "fetch", "insert", "kill", "open",
                                              "select", "sysobjects", "syscolumns",
                                              "table", "update"};

            foreach (var s in blackSymbolList)
            {
                if (value.Contains(s))
                {
                    return false;
                }
            }

            foreach (var s in blackWordList)
            {
                if (Regex.IsMatch(value, "\\b" + s + "\\b", RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
