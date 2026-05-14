using System;
using System.Text.RegularExpressions;

namespace DialerCommon.Logging
{
    /// <summary>
    /// Log file name by template generator.
    /// </summary>
    public class LogFileNameGenerator
    {
        private readonly string _templateFileName;
        private readonly Lazy<Regex> _lazyTemplateRegex;
        protected Regex TemplateRegex => _lazyTemplateRegex.Value;

        /// <summary>
        /// Create generator by template.
        /// </summary>
        /// <param name="templateFileName">Template file name that may include 
        /// %datetime% and %instance% variables</param>
        public LogFileNameGenerator(string templateFileName)
        {
            if (string.IsNullOrWhiteSpace(templateFileName))
                throw new ArgumentException("Log file name template can't be empty.", nameof(templateFileName));

            _templateFileName = templateFileName;
            _lazyTemplateRegex = new Lazy<Regex>(() => new Regex(GetRegexString()));
        }

        /// <summary>
        /// Replaces template variables %datetime% and %instance% with values
        /// </summary>
        /// <param name="fileDateTime">File generation date and time</param>
        /// <returns>File name string generated based on the template</returns>
        public string GenerateLogFileName(DateTime fileDateTime)
        {
            return _templateFileName
                .Replace("%datetime%", GetDateTimeFormattedForFileName(fileDateTime))
                .Replace("%date%", GetDateFormattedForFileName(fileDateTime))
                .Replace("%instance%", GetInstanceName());
        }

        /// <summary>
        /// Check that file name implement template.
        /// </summary>
        /// <param name="fileName">File name for checking</param>
        /// <returns></returns>
        public bool CheckFileName(string fileName) => TemplateRegex.IsMatch(fileName);

        private string GetRegexString()
        {
            return $@"^{Regex.Escape(_templateFileName)
                .Replace("%datetime%", GetDateTimeRegexMaskForFileName())
                .Replace("%date%", GetDateRegexMaskForFileName())
                .Replace("%instance%", GetInstanceName())
                .InsertBeforeMaskedFileExtension(GetFileNumberRegexMask())}$";
        }

        private string GetFileNumberRegexMask()
        {
            return @"(\.\d+)?";
        }

        private string GetDateTimeFormattedForFileName(DateTime fileDateTime)
        {
            //Important! If change this format also change RegexMask in GetDateTimeRegexMaskForFileName
            return $"{fileDateTime:yyyyMMdd}T{fileDateTime:HHmmss.fffzzz}".Replace(":", string.Empty);
        }

        private string GetDateTimeRegexMaskForFileName()
        {
            //Based on GetDateTimeFormattedForFileName format
            //Important! If change this format also change file name format in GetDateTimeFormattedForFileName
            return @"\d{8}T\d{6}.\d{3}(\+|-)\d{4}";
        }

        private string GetDateFormattedForFileName(DateTime fileDateTime)
        {
            //Important! If change this format also change RegexMask in GetDateRegexMaskForFileName
            return $"{fileDateTime:yyyyMMdd}";
        }

        private string GetDateRegexMaskForFileName()
        {
            //Based on GetDateFormattedForFileName format
            //Important! If change this format also change file name format in GetDateFormattedForFileName
            return @"\d{8}";
        }

        private string GetInstanceName()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            // Try to find instance name as value of " - Instance" or "/Instance" switch in the command line
            for (var i = 1; i < commandLineArgs.Length; i++)
            {
                if (commandLineArgs[i].Equals("-Instance", StringComparison.InvariantCultureIgnoreCase) ||
                    commandLineArgs[i].Equals("/Instance", StringComparison.InvariantCultureIgnoreCase))
                {
                    ++i;
                    if (i < commandLineArgs.Length)
                    {
                        return commandLineArgs[i];
                    }
                }
            }
            // We've found no instance name in the command line, so assume that it's the default instance
            return "Default";
        }
    }

    internal static class FileNameStringExtension
    {
        public static string InsertBeforeMaskedFileExtension(this string sourceString, string insertString)
        {
            var extensionPos = sourceString.LastIndexOf(@"\.", StringComparison.InvariantCulture);
            var insertPos = extensionPos >= 0 ? extensionPos : sourceString.Length;

            return sourceString.Insert(insertPos, insertString);
        }
    }

}