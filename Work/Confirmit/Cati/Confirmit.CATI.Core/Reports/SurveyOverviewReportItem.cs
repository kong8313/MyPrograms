using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports
{
    /// <summary>
    /// Single row for interviewer productivity report.
    /// </summary>
    [Serializable]
    public class SurveyOverviewReportItem
    {
        /// <summary>
        /// Project ID.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Project title.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Time while interviewer was logged in (in seconds).
        /// </summary>
        public long LogOnTime { get; set; }

        /// <summary>
        /// Time while interviewer was waiting for the interview (in seconds).
        /// </summary>
        public long WaitingTime { get; set; }

        public long BreakTimePaid { get; set; }

        public long BreakTimeUnpaid { get; set; }

        /// <summary>
        /// Count of dialings.
        /// </summary>
        public int DialigsCount { get; set; }

        /// <summary>
        /// Count of completed interviews.
        /// </summary>
        public int Completes { get; set; }

        /// <summary>
        /// Average duration of a completed interview (in seconds).
        /// </summary>
        public int AverageCompletedInterviewDuration { get; set; }
    }

    [Serializable]
    public class SurveyOverviewReportItemList : List<SurveyOverviewReportItem>
    {
        public SurveyOverviewReportItemList() { }

        public SurveyOverviewReportItemList(IEnumerable<SurveyOverviewReportItem> items) : base(items) { }
    }
}
