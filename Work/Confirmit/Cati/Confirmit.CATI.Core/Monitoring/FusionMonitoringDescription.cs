using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;

namespace Confirmit.CATI.Core.Monitoring
{
    /// <summary>
    /// Description of Fusion monitoring.
    /// </summary>
    public class FusionMonitoringDescription
    {
        /// <summary>
        /// ID of interviewer.
        /// </summary>
        public int InterviewerId { get; private set; }

        /// <summary>
        /// ID of monitoring session.
        /// </summary>
        public long MonitoringSessionId { get; private set; }

        /// <summary>
        /// Name of supervisor started monitoring.
        /// </summary>
        public string SupervisorName { get; private set; }

        /// <summary>
        /// Web monitoring flag.
        /// </summary>
        public bool IsWebMonitoring { get; private set; }

        /// <summary>
        /// Live monitoring flag.
        /// </summary>
        public bool IsLiveMonitoringEnabled { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="interviewerId">Interviewer ID</param>
        /// <param name="entity">generated Confirmit.CATI.Core.DAL.Generated.Entity.Procedure.BvSpPersonMonitoring_IsStartEntity</param>
        public FusionMonitoringDescription(
            int interviewerId,
            BvSpPersonMonitoring_IsStartEntity entity)
        {
            InterviewerId = interviewerId;
            SupervisorName = entity.supervisorNameAlreadyMonitoring;
            MonitoringSessionId = entity.monitoringSessionID.Value;
            IsWebMonitoring = entity.isWebMonitoring.Value;
            IsLiveMonitoringEnabled = entity.isLiveMonitoringEnabled.Value;
        }
    }
}