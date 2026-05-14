using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports
{
    [Serializable]
    public class SampleUtilisationReportItem
    {
        /// <summary>
        /// Batch ID.
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// Survey name.
        /// </summary>
        public string SurveyName { get; set; }

        /// <summary>
        /// Batch added at time.
        /// </summary>
        public DateTime BatchAddedAt { get; set; }

        /// <summary>
        /// Amount of added interviews.
        /// </summary>
        public string InterviewsAdded { get; set; }

        /// <summary>
        /// Current amount of records.
        /// </summary>
        public int InterviewsCurrent { get; set; }

        /// <summary>
        /// Amount of interviews attempted.
        /// </summary>
        public int InterviewsAttempted { get; set; }

        /// <summary>
        /// Blocked by FCD includedAttempted Interviews.
        /// </summary>
        public int BlockedIncludedAttemptedInterviews { get; set; }

        /// <summary>
        /// Blocked by FCD ExcludedAttempted Interviews.
        /// </summary>
        public int BlockedExcludedAttemptedInterviews { get; set; }

        /// <summary>
        /// Amount of interviews completed.
        /// </summary>
        public int InterviewsCompleted { get; set; }

        /// <summary>
        /// Attempted Records Per Complete
        /// </summary>
        public int AttemptedInterviewsPerComplete { get; set; }

        /// <summary>
        /// Average attempts per complete
        /// </summary>
        public int AvgAttemptsPerComplete { get; set; }

        /// <summary>
        /// Amount of deleted interviews.
        /// </summary>
        public string InterviewsDeleted { get; set; }

        /// <summary>
        /// Amount of deleted interviews.
        /// </summary>
        public int BlockedByBlacklist { get; set; }
    }

    [Serializable]
    public class SampleUtilisationReportItemList : List<SampleUtilisationReportItem>
    {
        public SampleUtilisationReportItemList() { }

        public SampleUtilisationReportItemList(IEnumerable<SampleUtilisationReportItem> items) : base(items) { }
    }
}
