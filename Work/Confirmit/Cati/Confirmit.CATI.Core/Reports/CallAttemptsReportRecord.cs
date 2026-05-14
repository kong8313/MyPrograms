using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Core.Reports
{
    /// <summary>
    /// Represents single record of call attempts report.
    /// </summary>
    public class CallAttemptsReportRecord : BvSpGetCallAttemptsReport_ListPageEntity
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of CallAttemptsReportRecord class and fills it with
        /// given data. If timezone identifier <c>localTzId</c> is not null, we convert all dates
        /// to given timezone.
        /// </summary>
        /// <param name="entity">Call attempts report entity.</param>
        /// <param name="localTzId">Nullable timezone identifier. Id given identifier is not null
        /// we should convert dates from UTC to given timezone; otherwise no conversion needed.</param>
        /// <param name="hidePii">Hide PII</param>
        public CallAttemptsReportRecord(BvSpGetCallAttemptsReport_ListPageEntity entity, int? localTzId, bool hidePii)
            : base()
        {
            CallDuration = entity.CallDuration;
            ExtendedStatus = entity.ExtendedStatus;
            ExtendedStatusName = entity.ExtendedStatusName;
            ID = entity.ID;
            InterviewerName = entity.InterviewerName;
            InterviewID = entity.InterviewID;
            ProjectID = entity.ProjectID;
            ProjectName = entity.ProjectName;
            SurveySID = entity.SurveySID;
            TelephoneNumber = hidePii ? "***" : entity.TelephoneNumber;
            WaitingTime = entity.WaitingTime;
            DisplayTime = entity.DisplayTime;
            PreviewTime = entity.PreviewTime;
            ConnectedTime = entity.ConnectedTime;
            WrapTime = entity.WrapTime;
            OpenEndReviewDuration = entity.OpenEndReviewDuration;
            EventDate = localTzId.HasValue ? TimezoneManager.ConvertToTzLocalTime(localTzId.Value, entity.EventDate.Value) : entity.EventDate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets string representaion of call duration.
        /// </summary>
        public string CallDurationString
        {
            get { return TimeSpan.FromSeconds(CallDuration ?? 0).ToString(); }
        }

        /// <summary>
        /// Gets string representaion of display time.
        /// </summary>
        public string DisplayTimeString
        {
            get { return DisplayTime != null ? ((double)DisplayTime / 1000d).ToString("0.0") : "NA"; }
        }

        /// <summary>
        /// Gets string representaion of waiting time.
        /// </summary>
        public string WaitingTimeString
        {
            get { return TimeSpan.FromSeconds(WaitingTime ?? 0).ToString(); }
        }

        /// <summary>
        /// Gets string representaion of preview time.
        /// </summary>
        public string PreviewTimeString
        {
            get { return TimeSpan.FromSeconds(PreviewTime ?? 0).ToString(); }
        }

        /// <summary>
        /// Gets string representaion of connected time.
        /// </summary>
        public string ConnectedTimeString
        {
            get { return TimeSpan.FromSeconds(ConnectedTime ?? 0).ToString(); }
        }

        /// <summary>
        /// Gets string representaion of wrap time.
        /// </summary>
        public string WrapTimeString
        {
            get { return TimeSpan.FromSeconds(WrapTime ?? 0).ToString(); }
        }

        /// <summary>
        /// Gets string representaion of wrap time.
        /// </summary>
        public string ReviewTimeString
        {
            get { return TimeSpan.FromSeconds(OpenEndReviewDuration ?? 0).ToString(); }
        }

        #endregion
    }
}