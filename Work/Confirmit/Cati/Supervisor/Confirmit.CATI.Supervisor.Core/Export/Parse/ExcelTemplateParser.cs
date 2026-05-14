using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Confirmit.CATI.Supervisor.Core.Export.Parse
{
    /// <summary>
    /// Class is responsible for excel template parsing
    /// </summary>
    public class ExcelTemplateParser
    {              
        /// <summary>
        /// Gets string with all available lexem's types for regex parsing
        /// </summary>
        private static string TypeRegexString
        {
            get
            {
                string result = String.Empty;

                foreach (string typeName in Enum.GetNames(typeof(LexemeType)))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = typeName;
                    }
                    else
                    {
                        result = string.Format("{0}|{1}", result, typeName);
                    }
                    
                }

                return string.Format("({0})", result);
            }
        }

        /// <summary>
        /// Parses cellMarkup string and returns corresponding LexemeInfo enumeration
        /// </summary>
        /// <param name="cellMarkup">String for parsing</param>
        /// <returns>LexemeInfo enumaration</returns>       
        public static LexemeInfo[] ParseCellMarkup(string cellMarkup)
        {
            if (String.IsNullOrEmpty(cellMarkup))
            {
                throw new ArgumentException(cellMarkup);
            }

            List<LexemeInfo> result = new List<LexemeInfo>();

            string markup = cellMarkup.Trim();

            if (Regex.IsMatch(markup, @"(^\<\%\s*)") && Regex.IsMatch(markup, @"(^\<\%\s*)"))
            {
                markup = Regex.Replace(markup, @"(^\<\%\s*)", String.Empty);
                markup = Regex.Replace(markup, @"(\s*\%\>$)", String.Empty);
            }
            else
            {
                return result.ToArray();
            }

            Regex regTypeValue = new Regex("(?<type>" + TypeRegexString + ")" + "([.]" + "(?<value>\\w+))?", RegexOptions.IgnoreCase);
            Regex regDataDetails = new Regex(@"(?<details>HorizontalDetails\s*(\(\s*style\s*=\s*(?<style>[A-Z]+\d+?)\s*\))?)", RegexOptions.IgnoreCase);

            string[] parts = markup.Split(new Char[] { ',' });

            if (parts.Length > 0)
            {
                string firstPart = parts[0];

                Match mch = regTypeValue.Match(firstPart);

                LexemeInfo lexemeInfo = new LexemeInfo();

                if (mch.Success)
                {
                    string type = mch.Groups["type"].Value;

                    LexemeType lexemeType = (LexemeType)Enum.Parse(typeof(LexemeType), type, true);

                    if (lexemeType != LexemeType.HorizontalDetails)
                    {
                        result.Add(new LexemeInfo()
                        {
                            Type = lexemeType,
                            Value = mch.Groups["value"].Success ? mch.Groups["value"].Value : string.Empty
                        });

                        if (lexemeType == LexemeType.Data && parts.Length > 1)
                        {
                            string secondPart = parts[1];

                            mch = regDataDetails.Match(secondPart);

                            if (mch.Groups["details"].Success)
                            {
                                string detailsValue = mch.Groups["style"].Success ? mch.Groups["style"].Value : String.Empty;

                                result.Add(new LexemeInfo()
                                {
                                    Type = LexemeType.HorizontalDetails,
                                    Value = detailsValue
                                });
                            }
                        }
                    }                    
                }            
            }

            return result.ToArray();
        }       
    }
}
