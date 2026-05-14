using System;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Represents predefined date periods supported by searching.
    /// </summary>
    public enum SearchPredefinedDate
    {
        /// <summary>
        /// No filtering.
        /// </summary>
        All = 0,

        /// <summary>
        /// Today.
        /// </summary>
        Today,

        /// <summary>
        /// Today and yesterday.
        /// </summary>
        LastTwoDays,

        TodayMinus1,

        TodayMinus2,

        TodayMinus3,

        TodayMinus4,

        TodayMinus5,

        TodayMinus6,

        TodayMinus7,

        /// <summary>
        /// Current week from monday.
        /// </summary>
        ThisWeek,

        /// <summary>
        /// Current month.
        /// </summary>
        ThisMonth,

        /// <summary>
        /// Current month and 2 previous.
        /// </summary>
        LastThreeMonths,

        /// <summary>
        /// Current month and 5 previuos.
        /// </summary>
        LastSixMonths,

        /// <summary>
        /// Current year.
        /// </summary>
        ThisYear
    }
}
