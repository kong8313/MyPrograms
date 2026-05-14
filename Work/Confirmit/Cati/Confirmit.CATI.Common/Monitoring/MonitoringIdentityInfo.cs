using System;

namespace Confirmit.CATI.Common.Monitoring
{
    [Serializable]
    public class MonitoringIdentityInfo
    {
        /// <summary>
        /// Checks for equality without InititalQuestion to be sure that we have the same records
        /// </summary>
        /// <param name="other">Entity to compare with</param>
        /// <returns></returns>
        public bool IsTheSameDeferredRecord(MonitoringIdentityInfo other)
        {
            return LaunchType == LaunchFileType.DeferredMonitoring
                   && LaunchType == other.LaunchType
                   && RecordId == other.RecordId
                   && CompanyId == other.CompanyId
                   && ServerName == other.ServerName;
        }

        public LaunchFileType LaunchType { get; set; }

        public string InterviewerName { get; set; }

        public string SupervisorName { get; set; }

        public int CompanyId { get; set; }

        public string ServerName { get; set; }

        public string CompanyAlias { get; set; }

        public int InterviewerId { get; set; }

        public string SurveyId { get; set; }

        public long RecordId { get; set; }

        public DateTime TimeStamp { get; set; }

        public long MonitoringSessionId { get; set; } // This field is currently used for live monitoring only.

        public DateTime CreationUtcTime { get; set; }

        public string InitialQuestion { get; set; }
    }
}