using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports
{
    /// <summary>
    /// Single row for number of attempts report.
    /// </summary>
    [Serializable]
    public class NumberOfAttemptsReportItem
    {
        /// <summary>
        /// Number of attempts.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Interviews count for current attempts number.
        /// </summary>
        public int Records { get; set; }
    }

    [Serializable]
    public class NumberOfAttemptsReportItemList : List<NumberOfAttemptsReportItem>
    {
        public NumberOfAttemptsReportItemList() { }

        public NumberOfAttemptsReportItemList(IEnumerable<NumberOfAttemptsReportItem> items) : base(items) { }
    }
}