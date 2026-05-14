using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary
{
    public class AudioMonitoring : IAudioMonitoring
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITelephony _telephony;

        public AudioMonitoring(ITaskRepository taskRepository, ITelephony telephony)
        {
            _taskRepository = taskRepository;
            _telephony = telephony;
        }

        public void StartAudioMonitor(string supervisorName, int interviewerId, string telephoneNumber)
        {
            var task = _taskRepository.GetByPerson(interviewerId);

            if (task == null)
            {
                throw new UserMessageException("Unable to start audio monitoring: there is no active task for this interviewer.");
            }

            if (task.LoggedInToDialerState != (byte)LoginState.LOGGED_IN)
            {
                throw new UserMessageException("Unable to start audio monitoring: interviewer is not logged in to dialer.");
            }

            var sessionId = string.Empty;
            var previousTelephoneNumber = string.Empty;

            var entity = AudioMonitoringAdapter.GetByCondition(
                "[InterviewerSID] = @InterviewerId",
                new SqlParameter("@InterviewerId", interviewerId)).FirstOrDefault();

            if (entity != null)
            {
                StopAudioMonitoringIfInterviewerIsAudioMonitoredByAnotherSupervisor(entity, supervisorName);
            }

            telephoneNumber = telephoneNumber.Trim();

            //Check if the monitoring resource is used by another supervisor
            entity = AudioMonitoringAdapter.GetByCondition(
                "[TelephoneNumber] = @TelephoneNumber",
                new SqlParameter("@TelephoneNumber", telephoneNumber)).FirstOrDefault();
            if (entity != null)
            {
                CheckMonitoringResourceIsUsedByAnotherSupervisor(entity.SupervisorName, supervisorName, telephoneNumber);
            }

            //Now check if the supervisor already audio monitors someone 
            entity = AudioMonitoringAdapter.GetByCondition(
                "[SupervisorName] = @SupervisorName",
                new SqlParameter("@SupervisorName", supervisorName)).FirstOrDefault();

            if (entity != null)
            {
                sessionId = entity.SessionID;
                previousTelephoneNumber = entity.TelephoneNumber;
            }

            if (String.IsNullOrEmpty(telephoneNumber))
            {
                telephoneNumber = previousTelephoneNumber;
            }

            // TODO: Use more curious way to define dialerId: may be we should keep dialer id with monitoring session, 
            // and choose dialerId from BvPerson or monitoring session, or (somehow) assosiate it with supervisorName
            var result = _telephony.StartMonitor(
                task.DialerId,
                interviewerId.ToString(),
                telephoneNumber,
                ref sessionId);

            if (result == DialerErrorCode.Success)
            {
                BvSpInsertUpdateAudioMonitoringSessionAdapter.ExecuteNonQuery(supervisorName, interviewerId, telephoneNumber, sessionId);
            }
            else
            {
                ProcessStartAudioMonitorError(interviewerId, supervisorName, task.DialerId, telephoneNumber, result);
            }
        }

        public void StopAudioMonitor(string supervisorName, int interviewerId)
        {
            var sessionId = "";
            string logStr = string.Format(
                " supervisorName = '{0}', interviewerId = '{1}'), company = '{2}'",
                supervisorName,
                interviewerId,
                BackendInstance.Current.CompanyId);

            var entity = AudioMonitoringAdapter.GetByCondition(
                "[InterviewerSID] = @InterviewerId",
                new SqlParameter("@InterviewerId", interviewerId)).FirstOrDefault();

            if (entity != null)
            {
                sessionId = entity.SessionID;
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                //Nothing to stop
                Trace.TraceWarning("AudioMonitoring.StopAudioMonitor: Stop called for non-existing audio moniroting session. ///" + logStr);
                return;
            }

            var task = _taskRepository.GetByPerson(interviewerId);

            if (task == null || task.DialerId == 0)
            {
                // Person already logged out from dialer
                throw new UserMessageException(
                    "AudioMonitoring.StopAudioMonitor: Can not stop the audio monitoring session because the interviewer is already logged out. /// " + logStr);
            }

            var result = _telephony.StopMonitor(task.DialerId, interviewerId.ToString(CultureInfo.InvariantCulture), task.InterviewID, sessionId);

            AudioMonitoringAdapter.DeleteByCondition(
                "[InterviewerSID] = @InterviewerId",
                new SqlParameter("@InterviewerId", interviewerId));

            if (result == DialerErrorCode.UnknownSupervisor)
            {
                // At least for TCI it means that TCI dialer WS did not find monitoring session,
                // and so it could not send StopMonitor command to dialer.
                // It seems other dialers (PROTS) can not return this code on StopMonitor.
                // So for now we use DialerErrorCode.RESULT_DIALER_UNKNOWN_SUPERVISOR.
                // TODO: may be later we will make a new DialerErrorCode for the case.
                throw new UserMessageException(
                    "Monitoring session is lost. Try to start monitoring again.");
            }

            if (result != DialerErrorCode.Success)
            {
                throw new InternalErrorException(
                    string.Format(
                        "AudioMonitoring.StopMonitor: StopMonitor failed with code {0}. " +
                        " /// dialerId={1}, sessionId={2}" + logStr,
                        result,
                        task.DialerId,
                        sessionId));
            }
        }

        public void SetMonitorMode(string supervisorName, int interviewerId, MonitorMode monitorMode)
        {
            var sessionId = "";
            string logStr = $" supervisorName = '{supervisorName}', interviewerId = '{interviewerId}'), monitorMode={monitorMode}, company = '{BackendInstance.Current.CompanyId}'";

            var entity = AudioMonitoringAdapter.GetByCondition(
                "[InterviewerSID] = @InterviewerId",
                new SqlParameter("@InterviewerId", interviewerId)).FirstOrDefault();

            if (entity != null)
            {
                sessionId = entity.SessionID;
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                //No monitoring session in CATI database
                Trace.TraceWarning("AudioMonitoring.SetMonitorMode: Set monitoring mode called for non-existing audio moniroting session. ///" + logStr);
                return;
            }

            var task = _taskRepository.GetByPerson(interviewerId);

            if (task == null || task.DialerId == 0)
            {
                // Person already logged out from dialer
                throw new UserMessageException(
                    "AudioMonitoring.SetMonitorMode: Can not set the audio monitoring mode because the interviewer is already logged out. /// " + logStr);
            }

            var result = _telephony.SetMonitorMode(task.DialerId, interviewerId.ToString(CultureInfo.InvariantCulture), sessionId, monitorMode);

            if (result == DialerErrorCode.UnknownSupervisor)
            {
                throw new UserMessageException("Monitoring session is lost. Try to start monitoring again.");
            }

            if (result != DialerErrorCode.Success)
            {
                throw new InternalErrorException(
                    $"AudioMonitoring.SetMonitorMode: SetMonitorMode failed with code {result}. " +
                    $" /// dialerId={task.DialerId}, sessionId={sessionId}, monitorMode={monitorMode}" + logStr);
            }
            else
            {
                entity.MonitorMode = (int)monitorMode;
                AudioMonitoringAdapter.Update(entity);
            }
        }

        private void StopAudioMonitoringIfInterviewerIsAudioMonitoredByAnotherSupervisor(AudioMonitoringEntity entity, string oldSupervisorName)
        {
            if (oldSupervisorName.Equals(entity.SupervisorName))
            {
                return;
            }

            StopAudioMonitor(oldSupervisorName, entity.InterviewerSID);
        }

        private void CheckMonitoringResourceIsUsedByAnotherSupervisor(
            string entitySupervisorName, string supervisorName, string telephoneNumber)
        {
            if (entitySupervisorName.Equals(supervisorName))
            {
                return;
            }

            throw new UserMessageException(string.Format(
                "The monitoring resource '{0}' is currently being used by another supervisor: '{1}'.",
                telephoneNumber, entitySupervisorName));
        }

        /// <summary>
        /// Handlers errors occurred during call StartMonitor for dialer
        /// </summary>
        internal static void ProcessStartAudioMonitorError(
            int interviewerId,
            string supervisorName,
            int dialerId,
            string telephoneNumber,
            DialerErrorCode dialerErrorCode)
        {
            string errorString;

            switch (dialerErrorCode)
            {
                case DialerErrorCode.UnknownAgent:
                    errorString = string.Format(
                        "Interviewer with identifier '{0}' is not found.", interviewerId);
                    break;

                case DialerErrorCode.UnknownSupervisor:
                    errorString = string.Format(
                        "The extension '{0}' is incorrect.", telephoneNumber);
                    break;

                case DialerErrorCode.WrongStateDialingInProgress:
                    errorString = "Interview is being dialed. Please try again later.";
                    break;

                case DialerErrorCode.WrongStateResourceIsBusy:
                    errorString = "Supervisor’s resource is busy.";
                    break;

                case DialerErrorCode.AgentIsNotLoggedin:
                    errorString = string.Format(
                        "Interviewer with identifier '{0}' is not logged in.", interviewerId);
                    break;

                case DialerErrorCode.AgentAlreadyBeingMonitored:
                    errorString = string.Format(
                        "Interviewer with identifier '{0}' is already being monitored.", interviewerId);
                    break;

                case DialerErrorCode.NoMoreConferenceResources:
                    errorString = "No more conference resources are available.";
                    break;

                case DialerErrorCode.NoMoreSupervisorResources:
                    errorString = "No more supervisor resources are available.";
                    break;

                case DialerErrorCode.PhoneNumberAlreadyInUse:
                    errorString = string.Format(
                        "Phone number '{0}' is already in use by an interviewer or supervisor.", telephoneNumber);
                    break;

                case DialerErrorCode.MonitoringIsAlreadyStarted:
                    errorString = string.Format(
                        "Supervisor with phone number or name '{0}' is already monitoring an interviewer.", telephoneNumber);
                    break;

                default:
                    Trace.TraceError("AudioMonitoring.StartAudioMonitor: StartMonitor failed with code {0}. " +
                                     "/// supervisorName={1}, interviewerId={2}, " +
                                     "dialerId={3}, telephoneNumber={4}, company={5}", dialerErrorCode, supervisorName, interviewerId, dialerId, telephoneNumber, BackendInstance.Current.CompanyId);

                    errorString = string.Format(
                        "Unexpected error [{0}]. Please contact your system administrator.",
                        dialerErrorCode);
                    break;
            }

            throw new UserMessageException(string.Format("Unable to start audio monitoring: {0}", errorString));
        }
    }
}