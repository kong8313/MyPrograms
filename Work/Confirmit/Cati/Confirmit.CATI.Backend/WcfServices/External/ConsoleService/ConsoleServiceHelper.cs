using System;
using System.Diagnostics;
using BvCallHandlerLibrary;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;
using DialingMode = ConfirmitDialerInterface.DialingMode;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public class ConsoleServiceHelper : IConsoleServiceHelper
    {
        private readonly ITimezoneService _timezoneService;
        private readonly IDialerCollection _dialerCollection;
        private readonly IDialerLoginLogoutManager _dialerLoginLogoutManager;
        private readonly IBvCallHandlerRoot _callHandlerRoot;
        private readonly ITimezoneRepository _timezoneRepository;
        private readonly IBreakTypeRepository _breakTypeRepository;

        public ConsoleServiceHelper(
            ITimezoneService timezoneService,
            IDialerCollection dialerCollection,
            IDialerLoginLogoutManager dialerLoginLogoutManager,
            IBvCallHandlerRoot callHandlerRoot,
            ITimezoneRepository timezoneRepository,
            IBreakTypeRepository breakTypeRepository)
        {
            _timezoneService = timezoneService;
            _dialerCollection = dialerCollection;
            _dialerLoginLogoutManager = dialerLoginLogoutManager;
            _callHandlerRoot = callHandlerRoot;
            _timezoneRepository = timezoneRepository;
            _breakTypeRepository = breakTypeRepository;
        }

        public Timezone GetTimeZone(int timezoneId)
        {
            return _timezoneService.GetTimeZone(timezoneId);
        }

        /// <summary>
        /// The person a logout process.
        /// This internal method runs in a separate thread.
        /// </summary>
        public void LogoutProcess(
            int personId,
            string company,
            LoginState loggedInToDialerState,
            bool isLoginRcToDialer,
            string projectId,
            int dialerId)
        {
            var logStr = "CATIConsoleWS.LogoutProcess ";

            try
            {
                var activityEvent = new LogoutProcessEvent();

                logStr += string.Format(
                    "(personId={0}, company ={1}, loggedInToDialerState={2}, isLoginRCToDialer={3}, dialerId={4})",
                    personId,
                    company,
                    loggedInToDialerState,
                    isLoginRcToDialer,
                    dialerId);

                //Logout person from dialer if needed

                if (loggedInToDialerState == LoginState.LOGGED_IN)
                {
                    DialerErrorCode logoutResult;

                    if (_dialerCollection.IsDialerInitialized(dialerId))
                    {
                        BvSpTasks_UpdateStatusLogoutAdapter.ExecuteNonQuery(
                            personId,
                            (byte)LoginState.LOGGING_OUT);

                        BvSpTasks_UpdateLoggedInToDialerStateAdapter.ExecuteNonQuery(
                            personId,
                            (byte)LoginState.LOGGING_OUT);

                        long campaignId = 0;

                        if (projectId.Length > 0)
                        {
                            campaignId = ProjectIdConverter.ProjectIdToCampaignId(projectId);
                        }

                        logoutResult = _dialerLoginLogoutManager.Logout(
                            dialerId,
                            campaignId,
                            !isLoginRcToDialer,
                            personId);

                    }
                    else
                    {
                        logoutResult = DialerErrorCode.NotAvailable;
                    }

                    if (logoutResult != DialerErrorCode.Success)
                    {
                        //Report the error and continue logout.
                        Trace.TraceError(
                            logStr + "Person '{0}' logout from dialer failed. Error code: [{1}]",
                            personId,
                            logoutResult);

                        BvSpTasks_UpdateProblemStateAdapter.ExecuteNonQuery(
                            personId,
                            (int)logoutResult);
                    }
                }
                else
                {
                    // we are not connected to dialer
                    BvSpTasks_UpdateStatusLogoutAdapter.ExecuteNonQuery(
                        personId,
                        (byte)LoginState.NOT_LOGGED_IN);
                }

                activityEvent.Save(personId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(logStr + ex);
            }
        }

        /// <summary>
        /// Checks is person mode allowed by permissions
        /// </summary>
        /// <param name="mode">Person mode</param>
        /// <param name="persmissions"><see cref="TaskChoicePermissions"/></param>
        /// <returns>Returns <c>true</c> if person mode is allowed; <c>false</c> otherwise</returns>
        internal static bool IsPersonModeAllowed(AgentTaskChoiceMode mode, TaskChoicePermissions persmissions)
        {
            switch (mode)
            {
                case AgentTaskChoiceMode.Automatic:
                    if ((persmissions & TaskChoicePermissions.Automatic) == TaskChoicePermissions.Automatic) return true;
                    break;
                case AgentTaskChoiceMode.Manual:
                    if ((persmissions & TaskChoicePermissions.Manual) == TaskChoicePermissions.Manual) return true;
                    break;
                case AgentTaskChoiceMode.CampaignAssignment:
                    if ((persmissions & TaskChoicePermissions.SurveyAssignment) == TaskChoicePermissions.SurveyAssignment) return true;
                    break;
            }

            return false;
        }

        public bool SetPendingBreakStatus(
            BvTasksEntity task,
            BvPersonEntity person,
            PendingBreakStatus status,
            int? breakTypeId)
        {
            var newLoginState = (status == PendingBreakStatus.Break
                                         ? LoginState.PENDING_BREAK
                                         : LoginState.LOGGED_IN);

            var currentLoginState = (LoginState)task.StatusLogout;

            // TODO: !!!!! Discuss why we check for these states
            if (currentLoginState != LoginState.PENDING_BREAK && currentLoginState != LoginState.LOGGED_IN)
            {
                throw new InternalErrorException(
                    string.Format("Incorrect task state: {0} for interviewer with id = {1}. It should be LOGGED_IN.",
                    (LoginState)task.StatusLogout,
                    person.SID));
            }

            if (status == PendingBreakStatus.None &&
                currentLoginState == LoginState.PENDING_BREAK &&
                task.InterviewID == 0)
            {
                return false;
            }

            //Double call ?
            //We may switch to another break type so should check if it just was changed
            // TODO: method should be refactored
            if (currentLoginState == newLoginState && task.BreakTypeId == breakTypeId)
            {
                return true;
            }

            // Handle case when service is being called from old console which passes only status and doesn't pass breakTypeId
            if (breakTypeId == null && status == PendingBreakStatus.Break)
            {
                breakTypeId = 1;
            }

            task.StatusLogout = (byte)newLoginState;
            task.BreakTypeId = breakTypeId;

            TaskRepository.Update(task);

            if (breakTypeId != null)
            {
                var breakEntity = _breakTypeRepository.TryGetById(breakTypeId.Value);
                if (breakEntity == null)
                {
                    Trace.TraceWarning("SetPendingBreakStatus was called with breakTypeId={0} which does not exist", breakTypeId);
                }
            }

            //We can break right now
            if (status == PendingBreakStatus.Break && task.InterviewID == 0)
            {
                _callHandlerRoot.TakeBreak(
                    task,
                    SurveyRepository.TryGetById(task.SurveySID),
                    DialerAction.SendNoReady,
                    false);
            }

            return true;
        }

        /// <summary>
        /// This method is called from console when person is returning from break;
        /// Console retries this method if communication error has occured.
        /// It can be in several cases: 
        ///     1) this method was not called
        ///     2) this method threw exception but this exception didn't reach the console 'cause communication problem (console got communication error)
        ///     3) this method was successfully completed but console got communication error
        /// </summary>
        public void ContinueWorkAfterBreak(BvTasksEntity task, int attemptNumber)
        {
            try
            {
                BvSpTasks_UpdateStatusLogoutEntity updateStatusLogoutEntity;

                using (var transaction = new DatabaseTransactionScope("ContinueWorkAfterBreak"))
                {
                    updateStatusLogoutEntity = BvSpTasks_UpdateStatusLogoutAdapter.ExecuteEntity(
                        task.PersonSID, (byte)(LoginState.LOGGED_IN));

                    // If previous task state is not 'break' then we're trying to continue work after break
                    // but we are not on a break now. We should process this error.
                    if (updateStatusLogoutEntity == null ||
                        (LoginState)updateStatusLogoutEntity.PreviousStatusLogout != LoginState.BREAK)
                    {
                        // If it is first call then unknown error has occured. Task is in incorrect state.
                        // we should throw exception.
                        if (attemptNumber == 1)
                        {
                            throw new InternalErrorException(
                                string.Format("Incorrect task state: {0} for interviewer id={1}. It should be BREAK.",
                                (LoginState)updateStatusLogoutEntity.PreviousStatusLogout,
                                task.PersonSID));
                        }

                        // it is second (or third) call. Some work could be done during previous call.                        
                        Trace.TraceWarning("ContinueWorkAfterBreak was called {0} times for person with id {1}", attemptNumber, task.PersonSID);
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }

                // We need to switch survey if needed _before_ sending GoReady
                SwitchSurveyIfNeeded(task);

                //In predictive mode we must send GoReady command to dialler to start calls delivering.
                //This routine can be called several times. It does not lead to any errors.
                if ((LoginState)updateStatusLogoutEntity.LoggedInToDialerState == LoginState.LOGGED_IN &&
                    (DialingMode)updateStatusLogoutEntity.DiallingMode == DialingMode.Predictive)
                {
                    _callHandlerRoot.TryToSendGoReady(
                            task.DialerId,
                            ProjectIdConverter.ProjectIdToCampaignId(updateStatusLogoutEntity.ProjectID),
                            task.PersonSID,
                            () => task.LogString());
                }
            }
            finally
            {
                //We should set finish time anyway. If finish time has been set
                //then method does nothing.
                BvSpFinishInterviewerBreakAdapter.ExecuteNonQuery(task.PersonSID);
            }
        }

        public void SwitchSurveyIfNeeded(BvTasksEntity task)
        {
            if (!_callHandlerRoot.IsPendingSurveySwitch(task))
            {
                return;
            }

            _callHandlerRoot.SwitchSurvey(task.DialerId, task);
            EventDetailsScope.Current.AddTiming("BvCallHandlerRoot.SwitchSurvey");
        }
    }
}
