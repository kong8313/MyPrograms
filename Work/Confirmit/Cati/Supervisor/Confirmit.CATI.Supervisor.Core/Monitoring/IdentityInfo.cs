using System;

namespace Confirmit.CATI.Supervisor.Core.Monitoring
{
    /// <summary>
    /// Represents data of LaunchInfo.
    /// </summary>
    [Serializable]
    public class IdentityInfo
    {
        #region Properties 

        /// <summary>
        /// Gets/sets CompanyID.
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Gets/sets CompanyAlias.
        /// </summary>
        public string CompanyAlias { get; set; }

        /// <summary>
        /// Gets/sets InterviewerID.
        /// </summary>
        public int InterviewerID { get; set; }

        /// <summary>
        /// Gets/sets interviewer name.
        /// </summary>
        public string InterviewerName { get; set; }

        /// <summary>
        /// Gets/sets SurveyID.
        /// </summary>
        public string SurveyID { get; set; }

        /// <summary>
        /// Gets/sets the monitoring session ID.
        /// </summary>
        public long MonitoringSessionID { get; set; }

        /// <summary>
        /// Gets/sets the name of the supervisor.
        /// </summary>
        public string SupervisorPersonName { get; set; }

        #endregion
    }
}
