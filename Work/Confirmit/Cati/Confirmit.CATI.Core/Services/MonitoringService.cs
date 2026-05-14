using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Monitoring;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class MonitoringService : IMonitoringService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IInterviewerApiClient _interviewerApiClient;

        public MonitoringService(ISurveyRepository surveyRepository, ITaskRepository taskRepository, IInterviewerApiClient interviewerApiClient)
        {
            _surveyRepository = surveyRepository;
            _taskRepository = taskRepository;
            _interviewerApiClient = interviewerApiClient;
        }

        /// <summary>
        /// Start monitoring if it is in not progress. Otherwise, return active monitoring supervisor name.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <param name="supervisorName">Name of supervisor for monitoring.</param>
        /// <param name="projectId">Project (survey) name. Uses for log event </param>
        /// <param name="telephoneNumber">The monitoring interviewer telephone number</param>
        /// <returns>ID of monitoring session if monitoring was started; -1 otherwise.</returns>
        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public long StartMonitoring(int interviewerId, string supervisorName, string projectId,
            string telephoneNumber = null, bool isWebMonitoring = false)
        {
            var evt = new StartVideoMonitoringEvent(interviewerId, PersonRepository.GetById(interviewerId).Name,
                projectId);

            long monitoringSessionId = DateTime.UtcNow.Ticks;
            
            using (var transactionScope = new DatabaseTransactionScope("StartVideoMonitoring", DeadlockPriority.Supervisor))
            {
                FusionMonitoringDescription mDescr = GetActiveMonitoring(interviewerId);

                if (mDescr != null)
                {
                    throw new UserMessageException("Monitoring is already started by [" + mDescr.SupervisorName + "]");
                }

                var entity = BvSpPersonMonitoring_StartAdapter.ExecuteEntity(
                    interviewerId,
                    supervisorName,
                    monitoringSessionId,
                    telephoneNumber,
                    isWebMonitoring,
                    false);

                if (entity == null)
                {
                    throw new InternalErrorException(
                        string.Format(
                            "BvSpPersonMonitoring_Start stored procedure did not return a row. /// interviewerId=[{0}], supervisorName=[{1}]",
                            interviewerId, supervisorName));
                }

                if (entity.result == 0)
                {
                    throw new UserMessageException("Monitoring already running.");
                }
            
                transactionScope.Commit();
            }

            var task = _taskRepository.GetByPerson(interviewerId);
            if (task.IsWebConsole)
            {
                _interviewerApiClient.NotifyUpdatingLiveMonitoringState(true, BackendInstance.Current.CompanyId, interviewerId);
            }

            evt.Finish();

            return monitoringSessionId;
        }

        /// <summary>
        /// Stop monitoring if it is in progress. Nothing done, if not monitored
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <param name="monitoringSessionId">ID of monitoring session.</param>
        /// <param name="supervisorName">Name of supervisor for stop monitoring.</param>
        /// <exception cref="ArgumentException">Wrong monitoring session ID.</exception>
        /// <exception cref="UserMessageException">Monitoring is started by another supervisor.</exception>
        public void StopMonitoring(int interviewerId, long monitoringSessionId, string supervisorName)
        {
            var evt = new StopVideoMonitoringEvent(interviewerId, PersonRepository.GetById(interviewerId).Name,
                monitoringSessionId);

            using (var transactionScope = new DatabaseTransactionScope("StopVideoMonitoring", DeadlockPriority.Supervisor))
            {
                FusionMonitoringDescription mDescr = GetActiveMonitoring(interviewerId);

                if (mDescr == null)
                {
                    evt.Finish();
                    return;
                }

                if (mDescr.SupervisorName != supervisorName)
                {
                    throw new UserMessageException("Monitoring is started by [" + mDescr.SupervisorName + "]");
                }

                if (mDescr.MonitoringSessionId != monitoringSessionId)
                {
                    throw new ArgumentException(
                        string.Format("Wrong monitoring session ID. Expected: [{0}], Actual: [{1}]",
                            mDescr.MonitoringSessionId, monitoringSessionId),
                        "monitoringSessionId");
                }

                BvSpPersonMonitoring_StopAdapter.ExecuteNonQuery(interviewerId, monitoringSessionId);
                
                transactionScope.Commit();
            }
            
            var task = _taskRepository.GetByPerson(interviewerId);
            if (task.IsWebConsole)
            {
                _interviewerApiClient.NotifyUpdatingLiveMonitoringState(false, BackendInstance.Current.CompanyId, interviewerId);
            }

            evt.Finish();
        }

        /// <summary>
        /// Returns description of active monitoring for given interviewer.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <returns>Description of active monitoring. Null if there is no active monitoring.</returns>        
        public FusionMonitoringDescription GetActiveMonitoring(int interviewerId)
        {
            var entity = BvSpPersonMonitoring_IsStartAdapter.ExecuteEntity(interviewerId);

            if (entity == null)
            {
                throw new InternalErrorException(
                    string.Format(
                        "BvSpPersonMonitoring_IsStart stored procedure did not return a row. /// interviewerId=[{0}]",
                        interviewerId));
            }

            if (entity.result == 0)
            {
                return null;
            }

            return new FusionMonitoringDescription(interviewerId, entity);
        }

        /// <summary>
        /// Return monitoring state for person.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <returns>If person is monitoring, true. Otherwise, false</returns>        
        public bool IsMonitored(int interviewerId)
        {
            return GetActiveMonitoring(interviewerId) != null;
        }

        /// <summary>
        /// Returns if given monitoring session ID is monitoring session ID of active monitoring session.
        /// </summary>
        /// <param name="interviewerId">The monitoring interviewer SID</param>
        /// <param name="monitoringSessionId">ID of monitoring session.</param>
        /// <returns>True if given monitoring session ID is monitoring session ID of active monitoring session. False, otherwise.</returns>
        public bool IsActiveMonitoringSession(int interviewerId, long monitoringSessionId)
        {
            FusionMonitoringDescription mDescr = GetActiveMonitoring(interviewerId);

            if (mDescr == null)
            {
                return false;
            }

            return (mDescr.MonitoringSessionId == monitoringSessionId);
        }

        /// <summary>
        /// Returns state of audio monitoring session
        /// </summary>        
        /// <returns>True if audio monitoring is started otherwise false</returns>
        public bool IsAudioMonitoringSessionStarted(string supervisorName)
        {
            string sessionId = "";
            var entity = AudioMonitoringAdapter.GetByCondition(
                "[SupervisorName] = @SupervisorName",
                new SqlParameter("@SupervisorName", supervisorName)).FirstOrDefault();

            if (entity != null)
            {
                sessionId = entity.SessionID;
            }

            return !String.IsNullOrEmpty(sessionId);
        }

        public bool IsLiveMonitoringEnabled(int interviewerId)
        {
            var task = _taskRepository.GetByPerson(interviewerId);

            if (task == null || task.SurveySID == 0)
            {
                return false;
            }

            var survey = _surveyRepository.GetById(task.SurveySID);

            return survey.IsLiveMonitoringEnabled || task.Context.IsLiveMonitoringEnabled.GetValueOrDefault();
        }

        public void SetLiveMonitoringState(int personId, bool isLiveMonitoringEnabled)
        {
            var entity = BvPersonMonitoringAdapter.GetByCondition("[PersonSID] = @PersonSID", new SqlParameter("@PersonSID", personId)).FirstOrDefault();

            if (entity == null)
            {
                return;
            }

            entity.IsLiveMonitoringEnabled = isLiveMonitoringEnabled;
            BvPersonMonitoringAdapter.Update(entity);
        }
    }
}