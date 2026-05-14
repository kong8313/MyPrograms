using System;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Common
{
    /// <summary>
    /// Class represented methods to export data to Delimiter Separated Values format (DSV).
    /// </summary>
    public class DsvManager
    {
        /// <summary>
        /// Exports data to the Delimiter Separated Values format.
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="collection">Collection of objects of type T</param>
        /// <param name="delimiter">Delimiter string to separate values in record</param>
        /// <param name="function">Delegate to return values array for the type T</param>
        /// <returns>String in DSV format</returns>
        public static String ExportToDsv<T>(IEnumerable<T> collection, string delimiter, Func<T, object[]> function)
        {
            return String.Join(
                Environment.NewLine,
                collection.Select(y => String.Join(delimiter,
                                     function.Invoke(y).Select(x => Convert.ToString((object)x)).ToArray())
                                     ).ToArray());
        }

        public static IEnumerable<string> ImportFromDsv(string inputString)
        {
            return inputString
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).First().Trim());
        }
    }
}
