using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Monitoring;

namespace Confirmit.CATI.Core.Services
{
    public interface IMonitoringService
    {
        /// <summary>
        /// Start monitoring if it is in not progress. Otherwise, return active monitoring supervisor name.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <param name="supervisorName">Name of supervisor for monitoring.</param>
        /// <param name="projectId">Project (survey) name. Uses for log event </param>
        /// <param name="telephoneNumber">The monitoring interviewer telephone number</param>
        /// <param name="isWebMonitoring">Shows if current monitoring is started in browser based monitoring console</param>
        /// <returns>ID of monitoring session if monitoring was started; -1 otherwise.</returns>
        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        long StartMonitoring(int interviewerId, string supervisorName, string projectId, string telephoneNumber = null, bool isWebMonitoring = false);

        /// <summary>
        /// Stop monitoring if it is in progress. Nothing done, if not monitored
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <param name="monitoringSessionId">ID of monitoring session.</param>
        /// <param name="supervisorName">Name of supervisor for stop monitoring.</param>
        /// <exception cref="ArgumentException">Wrong monitoring session ID.</exception>
        /// <exception cref="UserMessageException">Monitoring is started by another supervisor.</exception>
        void StopMonitoring(int interviewerId, long monitoringSessionId, string supervisorName);

        /// <summary>
        /// Returns description of active monitoring for given interviewer.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <returns>Description of active monitoring. Null if there is no active monitoring.</returns>        
        FusionMonitoringDescription GetActiveMonitoring(int interviewerId);

        /// <summary>
        /// Return monitoring state for person.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <returns>If person is monitoring, true. Otherwise, false</returns>        
        bool IsMonitored(int interviewerId);

        /// <summary>
        /// Returns if given monitoring session ID is monitoring session ID of active monitoring session.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <param name="monitoringSessionId">ID of monitoring session.</param>
        /// <returns>True if given monitoring session ID is monitoring session ID of active monitoring session. False, otherwise.</returns>
        bool IsActiveMonitoringSession(int interviewerId, long monitoringSessionId);

        /// <summary>
        /// Returns state of audio monitoring session
        /// </summary>        
        /// <returns>True if audio monitoring is started otherwise false</returns>
        bool IsAudioMonitoringSessionStarted(string supervisorName);

        bool IsLiveMonitoringEnabled(int interviewerId);

        void SetLiveMonitoringState(int personId, bool isLiveMonitoringEnabled);
    }
}