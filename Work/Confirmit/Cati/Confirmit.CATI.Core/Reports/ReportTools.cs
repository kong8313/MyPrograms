using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.Core.Reports
{
    public class ReportTools
    {
        private const string m_LongStringSuffix = "...";

        /// <summary>
        /// Makes coma-separated string of names.
        /// </summary>
        /// <param name="names">List of names.</param>
        /// <param name="length">Maximum length for the resulting string.</param>
        /// <returns>Coma-separated string of names.</returns>
        public static string MakeArrayString(IEnumerable<string> names, int length)
        {
            var resultString = new StringBuilder();
            foreach (string str in names)
            {
                if (resultString.Length + str.Length > length)
                {
                    resultString.Append(m_LongStringSuffix);
                    break;
                }

                if (resultString.Length > 0)
                {
                    resultString.Append(", ");
                }

                resultString.Append(str);
            }

            return resultString.ToString();
        }

        /// <summary>
        /// Makes coma-separated string of names for multiline display.
        /// </summary>
        /// <param name="names">List of names.</param>
        /// <param name="length">Maximum length for the resulting string.</param>
        /// <param name="lines">Number of lines.</param>
        /// <returns>Coma-separated string of names.</returns>
        public static string MakeArrayStringEx(IEnumerable<string> names, int length, int lines)
        {
            string str = String.Join(", ", names.ToArray());
            int position = 0;

            for (int i = 0; i < lines; i++)
            {
                if (position + length >= str.Length)
                {
                    position = str.Length;
                }
                else
                {
                    position += str.Substring(position, length).LastIndexOf(',');
                }
            }

            return str.Substring(0, position) + (position < str.Length ? m_LongStringSuffix : String.Empty);
        }

        /// <summary>
        /// Cuts the long string. Appends '...' to the end of cut strings.
        /// </summary>
        /// <param name="stringToCut">The string to cut.</param>
        /// <param name="maxLength">Maximum string length.</param>
        public static string CutLongString(string stringToCut, int maxLength)
        {
            if (stringToCut == null)
            {
                throw new ArgumentNullException("stringToCut");
            }

            if (maxLength < m_LongStringSuffix.Length)
            {
                throw new ArgumentOutOfRangeException("maxLength");
            }

            if (stringToCut.Length <= maxLength)
            {
                return stringToCut;
            }

            return stringToCut.Substring(0, maxLength - m_LongStringSuffix.Length).TrimEnd() + m_LongStringSuffix;
        }

        public static string GetCallTime(int hour, DateTime reportStartTime)
        {
            var date = new DateTime(reportStartTime.Year, reportStartTime.Month, reportStartTime.Day, hour, 0, 0, 0);
            var newDate = ServiceLocator.Resolve<ILocalTimeProvider>().ConvertToLocalTime(date);

            return string.Format("{0:00}:00 - {0:00}:59", newDate.Hour);
        }

        public static string GetMinutesAndSecondsFromSeconds(int seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
        }
    }
}