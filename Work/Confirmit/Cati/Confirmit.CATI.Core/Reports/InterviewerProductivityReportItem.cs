using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports
{
    /// <summary>
    /// Single row for interviewer productivity report.
    /// </summary>
    [Serializable]
    public class InterviewerProductivityReportItem
    {
        /// <summary>
        /// Person SID.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Person login name.
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// Time while interviewer was logged in (in seconds).
        /// </summary>
        public long LogOnTime { get; set; }

        /// <summary>
        /// Time while interviewer was waiting for the interview (in seconds).
        /// </summary>
        public long WaitingTime { get; set; }

        /// <summary>
        /// Time while interviewer was on a break (in seconds).
        /// </summary>
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

        /// <summary>
        /// Duration of open end review (in seconds).
        /// </summary>
        public int OpenEndReviewDuration { get; set; }


    }

    [Serializable]
    public class InterviewerProductivityReportItemList : List<InterviewerProductivityReportItem>
    {
        public InterviewerProductivityReportItemList() { }

        public InterviewerProductivityReportItemList(IEnumerable<InterviewerProductivityReportItem> items) : base(items) { }
    }
}
