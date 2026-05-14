using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

using BvCallHandlerLibrary.Tools;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.Telephony.Dial;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;

//TODO: There is inconsistence with namespaces.
//      There are 2 namespaces instead of one - BvCallHandlerLibrary and Confirmit.CATI.Core.BvCallHandlerLibrary
namespace BvCallHandlerLibrary
{
    public class BvCallHandlerRoot : IBvCallHandlerRoot
    {
        private readonly ITelephony _telephony;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IInterviewService _interviewService;
        private readonly IDialerSettings _dialerSettings;

        private readonly IDialerCollection _dialerCollection;
        private readonly IMnTciTools _mnTciTools;
        private readonly ITimeService _timeService;
        private readonly IDialerLoginLogoutManager _dialerLoginLogoutManager;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;
        private readonly ITaskExtension _taskExtension;
        private readonly IHistoryRepository _historyRepository;
        private readonly ITelephoneBlacklistService _telephoneBlacklistService;
        private readonly IBreakTypeRepository _breakTypeRepository;
        private readonly ITimeBreakService _timeBreakService;

        public BvCallHandlerRoot(
            ITelephony telephony,
            ISurveyRepository surveyRepository,
            ITaskRepository taskRepository,
            IInterviewService interviewService,
            IDialerSettings dialerSettings,
            IDialerCollection dialerCollection,
            IMnTciTools mnTciTools,
            ITimeService timeService,
            IDialerLoginLogoutManager dialerLoginLogoutManager,
            IDatabaseLockTimeouts databaseLockTimeouts,
            IActiveDialRepository activeDialRepository,
            IActiveDialService activeDialService,
            ITaskExtension taskExtension,
            IHistoryRepository historyRepository,
            ITelephoneBlacklistService telephoneBlacklistService,
            IBreakTypeRepository breakTypeRepository,
            ITimeBreakService timeBreakService)
        {
            _telephony = telephony;
            _surveyRepository = surveyRepository;
            _taskRepository = taskRepository;
            _interviewService = interviewService;
            _dialerSettings = dialerSettings;
            _dialerCollection = dialerCollection;
            _mnTciTools = mnTciTools;
            _timeService = timeService;
            _dialerLoginLogoutManager = dialerLoginLogoutManager;
            _databaseLockTimeouts = databaseLockTimeouts;
            _activeDialRepository = activeDialRepository;
            _activeDialService = activeDialService;
            _taskExtension = taskExtension;
            _historyRepository = historyRepository;
            _telephoneBlacklistService = telephoneBlacklistService;
            _breakTypeRepository = breakTypeRepository;
            _timeBreakService = timeBreakService;
        }

        /// <summary>
        /// This method calls from Backend service on system startup
        /// </summary>
        public void OnStartup()
        {
            if (_mnTciTools.DoesCompanyUseTelephony())
            {
                InitializeDialers();
            }
        }

        //
        // internal CallHandlerRoot methods
        private void InitializeDialers()
        {
            using (var dbLock = DatabaseLockService.CreateLock(
                       DatabaseLockTimeoutsAndRecourceNames.DialerStateOperationLockerResourceName,
                       "DialerAvailabilityManager.EnableDialer",
                       _databaseLockTimeouts.DefaultLockTimeoutInMs,
                       true))
            {
                dbLock.EnterLock();
                
                _telephony.InitializeDialers();
            }
        }

        /// <summary>
        /// Prepares call for interviewing.
        /// Sends number to dialer if needed and number isn't from the blacklist.
        /// </summary>
        public BvInterviewEntity LookupCallForInterviewer(
            BvTasksEntity task,
            BvPersonEntity person,
            IEventDetails eventDetails)
        {
            // Looks for the next call/interview and updates
            //     task.CallID = call.CallID;
            //     task.SurveySID = (int)call.SurveySID;
            //     task.InterviewID = (int)call.iid;
            //     task.DiallingMode = (int)dialingMode;
            BvInterviewEntity nextInterview = TaskService.LookupByPersonSid(
                task,
                0);
            eventDetails.AddTiming("TaskService.LookupByPersonSid");

            if (nextInterview == null)
            {
                // call not found
                // TODO: !!!!! Ask why we do not drop task.CallOutcome if we drop rest columns...
                //MaximL: If person with manual task choice, we should set InterviewState.SELECTED
                ResetCommonTaskFields(task, InterviewState.NO_CALLS, (CallOutcome)task.CallOutcome, (int)DialerErrorCode.Success, true );
                return null;
            }

            if (task.InterviewID == 0)
            {
                throw new Exception("interview not defined (==0)");
            }

            var survey = SurveyRepository.GetById(task.SurveySID);

            if (task.Context.ActiveDialId == null)
            {

                if (survey.IsTelephoneBlacklistSupported &&
                    _telephoneBlacklistService.IsTelephoneNumberFilteredByBlacklist(nextInterview.TelephoneNumber))
                {
                    ProcessBlacklistInterview(
                        task,
                        nextInterview,
                        (AgentTaskChoiceMode) person.ManualSelection);

                    eventDetails.AddTiming("ProcessBlacklistInterview");

                    return nextInterview;
                }
            }

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            if (((LoginState) task.LoggedInToDialerState == LoginState.LOGGED_IN) && (task.DialingMode == DialingMode.Automatic || task.Context.TransferId != null) )
            {
                _interviewService.BindDialerIdToInterview(survey.SID, task.InterviewID, task.DialerId);

                var dialResult = _activeDialService.Dial(ref dial, task, survey, nextInterview, nextInterview.TelephoneNumber);

                eventDetails.AddTiming("ActiveDialService.Dial");

                if (dialResult != DialerErrorCode.Success)
                {
                    ProcessTelephonyError(dial, task, dialResult);

                    return nextInterview;
                }

                task.InterviewState = (byte) InterviewState.DIALLING;
            }
            else
            {
                _taskExtension.SetInterviewingState(task, dial);

                eventDetails.AddTiming("InterviewService.GetInterviewTimezoneOrDefault");

                _taskRepository.Update(task);

                eventDetails.AddTiming("TaskRepository.Update");
            }
            

            return nextInterview;
        }

        public void CompleteCallAtTaskTerminationIfNeeded(BvTasksEntity task)
        {
            // TODO: refactoring is required
            if (task.SurveySID == 0)
            {
                return;
            }

            if (!IsLoggedInToDialer(task))
            {
                return;
            }

            var survey = SurveyRepository.GetById(task.SurveySID);

            var interviewState = (InterviewState)task.InterviewState;

            var dialingMode = GetDialingMode(task, survey, null);

            switch (dialingMode)
            {
                case DialingMode.Automatic:
                case DialingMode.Preview:
                    //idea to refactoring: call complete call, if we has active dial with callId = task.CallId
                    if (interviewState == InterviewState.INTERVIEWING ||
                        interviewState == InterviewState.OPENEND_REVIEW ||
                        interviewState == InterviewState.INTERVIEW_WRAP_UP ||
                        interviewState == InterviewState.OUTGOING_TRANSFER ||
                        interviewState == InterviewState.INCOMING_TRANSFER )
                    {
                        _activeDialService.CompleteCall(
                            task,
                            task.DialerId,
                            survey.CampaignId,
                            task.PersonSID,
                            task.CallID,
                            false,
                            _timeBreakService.GetBreakTypeName(task.BreakTypeId),
                            InterviewStatus.FromCallOutcome(CallOutcome.Terminated),
                            CallCompleteStatus.Terminated);
                    }
                    break;

                case DialingMode.Manual:
                    //nothing
                    break;

                case DialingMode.Predictive:
                    DialerKillAgent(task, survey);
                    break;
            }
        }

        private void DialerKillAgent(BvTasksEntity task, BvSurveyEntity survey)
        {
            try
            {
                var killAgentResult = _activeDialService.KillAgent(task, survey);

                if (killAgentResult != DialerErrorCode.Success)
                {
                    LogDialerError(
                        killAgentResult,
                        "BvCallHandlerRoot.DialerKillAgent: Dialer.KillAgent returned error." +
                        " /// dialerId={0}, campaignId={1}, personSid={2}, error code={3}",
                        task.DialerId,
                        survey.CampaignId,
                        task.PersonSID,
                        killAgentResult);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "BvCallHandlerRoot.DialerKillAgent: " +
                    " /// dialerId={0}, campaignId={1}, personSid={2}: {3}",
                    task.DialerId,
                    survey.CampaignId,
                    task.PersonSID,
                    ex);
            }
        }

        public void LogoutFromDialerAtTaskTerminationIfNeeded(
            BvTasksEntity task)
        {
            // TODO: refactoring is required
            var dialingMode = GetDialingMode(task);

            if (dialingMode == DialingMode.Predictive)
            {
                return;
            }

            var loginToDialer = (LoginState)task.LoggedInToDialerState;
            if (loginToDialer != LoginState.LOGGING_IN && loginToDialer != LoginState.LOGGED_IN)
            {
                return;
            }

            var interviewState = (InterviewState)task.InterviewState;
            if (interviewState == InterviewState.DIALLING || interviewState == InterviewState.REDIALLING)
            {
                return;
            }

            try
            {
                var survey = _surveyRepository.TryGetById(task.SurveySID);

                var logoutResult = _dialerLoginLogoutManager.Logout(
                    task.DialerId,
                    survey?.CampaignId ?? 0,
                    !task.IsLoginRCToDialer,
                    task.PersonSID);

                if (logoutResult != DialerErrorCode.Success)
                {
                    LogDialerError(logoutResult,
                        string.Format(
                            "BvCallHandlerRoot.LogoutFromDialerAtTaskTerminationIfNeeded: Dialer.Logout returned error." +
                            " /// dialerId={0}, loginToDialerState={1}, IsLoginRCToDialer={2}, " +
                            "surveySID={3}, personSID={4}, interviewState={5}, error code={6}",
                            task.DialerId,
                            loginToDialer,
                            task.IsLoginRCToDialer,
                            task.SurveySID,
                            task.PersonSID,
                            interviewState,
                            logoutResult));
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "BvCallHandlerRoot.LogoutFromDialerAtTaskTerminationIfNeeded: " +
                    " /// dialerId={0}, loginToDialerState={1}, IsLoginRCToDialer={2}, " +
                    "surveySID={3}, personSID={4}, interviewState={5}: {6}",
                    task.DialerId, loginToDialer, task.IsLoginRCToDialer,
                    task.SurveySID, task.PersonSID, interviewState, ex);
            }
        }

        private static bool IsBreakOrPendingBreak(LoginState loginState)
        {
            return (loginState == LoginState.PENDING_BREAK) || (loginState == LoginState.BREAK);
        }

        private bool IsPendingLogoutOrBreak(LoginState statusLogout)
        {
            return (statusLogout == LoginState.PENDING_LOGOUT) || IsBreakOrPendingBreak(statusLogout);
        }


        public bool OnWrapUp(
            BvTasksEntity task,
            BvSurveyEntity survey,
            BvInterviewEntity currentInterview,
            BvPersonEntity person,
            BvActiveDialEntity deletedActiveDial,
            bool lookUpForNewCalls,
            WrapUpEvent activityEvent,
            InterviewStatus interviewStatus,
            int attemptNumber,
            out int? linkedInterviewSessionId,
            out TaskContext previousContext)
        {
            linkedInterviewSessionId = null;
            previousContext = null;

            using (TaskLocker taskLock = TaskLocker.TryLock(task.PersonSID))
            {
                activityEvent.AddTiming("TaskLocker.TryLock");

                if (taskLock != null)
                {
                    TaskRepository.GetByPerson(task.PersonSID).CopyTo(task);

                    linkedInterviewSessionId = _taskExtension.SetLinkedInterviewSessionId(task);
                    previousContext = task.Context.Clone();
                    //need to remove after refactoring, Task should be reseted after call of activeDial.CompleteCall operation in InternalWrapUp.
                    //In this case, Context will have corect/completed state.

                    _activeDialService.DetachDialFromTaskContextIfNeed(previousContext);

                    if (attemptNumber > 1 || task.InterviewID == 0)
                    {
                        Trace.TraceWarning(
                            "{0} attempt to make ConsoleService.WrapUp for person {1}. InterviewId = {2}",
                            attemptNumber, person.SID, currentInterview.ID);

                        //Check the state
                        if ((task.InterviewState != (byte)InterviewState.INTERVIEWING && task.InterviewState != (byte)InterviewState.OPENEND_REVIEW) ||
                            task.InterviewID != currentInterview.ID)
                        {
                            //A previous WrapUp made the work, we must not try WrapUp again,
                            return false;
                        }
                    }

                    InternalWrapUp(task, survey, currentInterview, person, deletedActiveDial, lookUpForNewCalls, activityEvent, interviewStatus);

                    _taskRepository.Update(task);
                    activityEvent.AddTiming("TaskRepository.Update");
                }
            }

            return true;
        }

        private void InternalWrapUp(
            BvTasksEntity task,
            BvSurveyEntity survey,
            BvInterviewEntity currentInterview,
            BvPersonEntity person,
            BvActiveDialEntity deletedActiveDial,
            bool lookUpForNewCalls,
            WrapUpEvent activityEvent,
            InterviewStatus interviewStatus)
        {
            var originalTask = task.Clone();
            var statusLogout = (LoginState)task.StatusLogout;
            var loggedInToDialerState = (LoginState)task.LoggedInToDialerState;

            bool pendingSurveySwitch;

            using (new EventDetailsScope(activityEvent.Details))
            {
                pendingSurveySwitch = IsPendingSurveySwitch(task);
            }

            var userTaskChoiceMode = (AgentTaskChoiceMode)person.ManualSelection;

            var isToSendToDialer = IsToSendCommandsToDialer(task, survey, currentInterview);

            activityEvent.UpdateEventPropertiesFromTask(task);
            task.StartTime = _timeService.GetUtcNow();

            if (originalTask.LinkedCallId.HasValue) //Linked survey
            {
                BvCallEntity call = CallQueueService.GetCallInfo((int)originalTask.LinkedCallId);
                BvInterviewEntity interview = InterviewRepository.GetByIdWithCheck(call.SurveySID, call.InterviewID);
                BvActiveDialEntity activeDial = _activeDialRepository.TryGetByCallId(task.CallID);

                if (activeDial != null)
                {
                    var result = _activeDialService.SetNextInterview(activeDial, interviewStatus, call);

                    if (result != DialerErrorCode.Success)
                    {
                        Trace.TraceError($"BvCallHandlerRoot.SendSetNextInterview: Dialer SetNextInterview failed with error [{result}], Task=[{task}]");
                    }

                    _activeDialRepository.Update(activeDial);

                    call.ActiveDialId = (long)task.Context.ActiveDialId;
                    activityEvent.AddTiming("telephonyProvider.SetNextInterview");
                }

                ResetTaskState(task, CallOutcome.NotDefined, userTaskChoiceMode);

                _taskExtension.UpdateOnCallConnected(task, interview, call);

                _taskExtension.ProcessLinkedChain(task, originalTask);

                return;     // we are done - console will get the call via GetState
            }

            //try to cancel not completed transfer
            CancelTransferIfNeed(task, person, deletedActiveDial);

            ResetTaskState(task, CallOutcome.NotDefined, userTaskChoiceMode);

            if (isToSendToDialer)
            {
                var switchSurveyIfWasChanged = false;
                var linkedInterviewState = originalTask.GetLinkedInterviewsPhase();
                if (linkedInterviewState == LinkedInterviewPhase.FinalInterview || linkedInterviewState == LinkedInterviewPhase.NotLinkedInterview)
                {
                    switchSurveyIfWasChanged = originalTask.SelectedSurveyId != 0 && originalTask.SurveySID != originalTask.SelectedSurveyId;
                }

                var makeInterviwerReady = !(IsPendingLogoutOrBreak(statusLogout) || pendingSurveySwitch);

                SendCompleteCall(task, survey, originalTask.CallID.GetValueOrDefault(), interviewStatus, makeInterviwerReady && !switchSurveyIfWasChanged, activityEvent);

                if (switchSurveyIfWasChanged)
                {
                    survey = _surveyRepository.GetById((int)originalTask.SelectedSurveyId);
                    if (survey.State != (int)SurveyState.Open)
                    {
                        TaskService.TerminateTask(
                            person.SID,
                            new DatabaseTransactionOptions("BvCallHandlerRoot.InternalWrapUp", DeadlockPriority.High),
                            null);
                        return;
                    }

                    task.SurveySID = (int)originalTask.SelectedSurveyId;
                    try
                    {
                        //If normal survey switching is set then we will send SetCampaign twice - here and below.
                        _telephony.SetCampaign(task.DialerId, survey.CampaignId, task.PersonSID);
                    }
                    catch (DialerException ex)
                    {
                        task.ProblemId = (int)ex.ErrorCode;
                    }

                    if (makeInterviwerReady)
                    {
                        _telephony.GoReady(task.DialerId, survey.CampaignId, task.PersonSID.ToString());
                    }
                }
            }

            if (userTaskChoiceMode == AgentTaskChoiceMode.Manual &&
                IsLoggedInToDialer(task) &&
                _telephony.IsReloginNeededOnSurveyChange())
            {
                task.StatusLogout = (byte)LoginState.PENDING_LOGOUT;
            }

            if (statusLogout == LoginState.NOT_LOGGED_IN)
            {
                //we should not do anything for this task/inter here if interviewer is logged out for some reasons.
                Trace.TraceError("Unexpected status NOT_LOGGED_IN. /// " + task.LogString());
                return;
            }
            else if (statusLogout == LoginState.PENDING_LOGOUT)
            {
                var logoutEvent = new LogoutOnWrapUpEvent();

                // without dialler
                if (loggedInToDialerState != LoginState.NOT_LOGGED_IN)
                {
                    if (loggedInToDialerState == LoginState.LOGGING_OUT)
                    {
                        // It means that logout from dialer operation was started before.
                        // So we should no start it again but must simply wait LOGGED_OUT notification.
                        return;
                    }

                    task.StatusLogout = (byte)LoginState.LOGGING_OUT;
                    task.LoggedInToDialerState = (byte)LoginState.LOGGING_OUT;

                    var logoutResult = _dialerLoginLogoutManager.Logout(
                        task.DialerId,
                        survey.CampaignId,
                        !task.IsLoginRCToDialer,
                        task.PersonSID);

                    activityEvent.AddTiming("DialerLoginLogoutManager().Logout");

                    if (logoutResult != DialerErrorCode.Success)
                    {
                        task.ProblemId = (int)logoutResult;
                    }
                }
                else
                {
                    task.StatusLogout = (byte)LoginState.NOT_LOGGED_IN;
                }

                logoutEvent.Save(task.PersonSID, task.InterviewID, survey.SID, survey.Name);
                activityEvent.AddTiming("logoutEvent.Save");

                return;
            } // if (task.StatusLogout == (byte)LoginState.PENDING_LOGOUT), Logout on wrapup

            if (pendingSurveySwitch && SurveySwitchMakesSense(statusLogout))
            {
                SwitchSurvey(task.DialerId, task);
                activityEvent.AddTiming("BvCallHandlerRoot.SwitchSurvey");
            }

            if (IsBreakOrPendingBreak(statusLogout))
            {
                // TODO: !!!!! Should in predictive we send GonotReady before sending CompleteCall? And should we send CompleteCall at all in this case???

                TakeBreak(task, survey, DialerAction.None, true);

                return;
            }

            if (!lookUpForNewCalls)
            {
                return;
            }

            if (userTaskChoiceMode == AgentTaskChoiceMode.Manual)
            {
                return;
            }

            var surveyDialingMode = GetDialingMode(task, survey, null);

            if ((surveyDialingMode == DialingMode.Predictive) &&
                (loggedInToDialerState == LoginState.LOGGED_IN))
            {
                return;
            }

            // OnWrapUp
            LookupCallForInterviewer(task, person, activityEvent.Details);

            activityEvent.AddTiming("LookupCallForInterviewer");
        }

        public void CancelTransferIfNeed(BvTasksEntity task, BvPersonEntity person, BvActiveDialEntity deletedActiveDial = null)
        {
            if (task.InterviewState == (int)InterviewState.OUTGOING_TRANSFER)
            {
                BvActiveDialEntity activeDial = deletedActiveDial ?? _activeDialRepository.TryGetByCallId(task.CallID);
                if (activeDial.MainPersonId == person.SID)
                {
                    var transferEvent = new TransferCancelEvent();
                    ServiceLocator.Resolve<IConsoleTransferCancelProcessor>().TransferCancel(task, person, transferEvent, activeDial);
                    transferEvent.Save();
                }
            }
        }


        private void SendCompleteCall(BvTasksEntity task, BvSurveyEntity survey, int callId, InterviewStatus status, bool makeInterviwerReady, WrapUpEvent activityEvent)
        {
            var result = _activeDialService.CompleteCall(
                task,
                task.DialerId,
                survey.CampaignId,
                task.PersonSID,
                callId,
                makeInterviwerReady,
                makeInterviwerReady ? null : _timeBreakService.GetBreakTypeName(task.BreakTypeId),
                status,
                CallCompleteStatus.CompleteByScript);

            activityEvent.AddTiming("telephonyProvider.CompleteCall");

            if (result != DialerErrorCode.Success)
            {
                LogDialerError(
                    result,
                    "BvCallHandlerRoot.OnWrapUp: Dialer CompleteCall failed with error [{0}]. " +
                    "/// dialerId={1}, personSID={2}, CompanyID={3}",
                    result,
                    task.DialerId,
                    task.PersonSID,
                    BackendInstance.Current.CompanyId);

                // A dialer error occured But do not update the task with ProblemId here, allowing the interviewer to continue.
                // So we avoid rescheduling of completed interviews at this point.
            }
        }

        public bool IsPendingSurveySwitch(BvTasksEntity task)
        {
            return (task.NewSurveySID != 0) && (task.NewSurveySID != task.SurveySID);
        }

        private bool SurveySwitchMakesSense(LoginState statusLogout)
        {
            var surveySwitchIsNotMakesSense =
                (statusLogout == LoginState.LOGGING_IN) ||
                (statusLogout == LoginState.LOGGING_OUT) ||
                (statusLogout == LoginState.NOT_LOGGED_IN);

            return !surveySwitchIsNotMakesSense;
        }

        public static bool IsLoggedInToDialer(BvTasksEntity task)
        {
            // (we should consider LoggedInToDialerState == LOGGING_OUT as "still logged in to dialer'
            return (task.LoggedInToDialerState == (byte) LoginState.LOGGED_IN) ||
                   (task.LoggedInToDialerState == (byte) LoginState.LOGGING_OUT);
        }

        public static bool DoesHangupMakeSense(BvTasksEntity taskEntity)
        {
            if (taskEntity.SurveySID == 0 || taskEntity.InterviewID == 0)
            {
                return false;
            }

            // logged in to dialler?
            //May be we should remove it from here, if we move this check to GetDiallingMode method
            if (!IsLoggedInToDialer(taskEntity))
                return false;
            //TODO:Idea to refactoring: we should have to look only on taskEntity.CallOutcome 
            switch ((DialingMode)taskEntity.DiallingMode)
            {
                case DialingMode.Predictive:
                case DialingMode.Automatic:
                    return true;
                case DialingMode.Preview:
                case DialingMode.SpecialDial:
                    return (taskEntity.CallOutcome == (int) CallOutcome.Connected);
                default:
                    return false;
            }
        }

        private static bool IsToSendCommandsToDialer(BvTasksEntity taskEntity, BvSurveyEntity survey, BvInterviewEntity currentInterview)
        {
            // TODO: refactoring is required, this method does common things like ensuring if we are connected to a dialer.

            if (taskEntity.SurveySID == 0 || taskEntity.InterviewID == 0)
            {
                return false;
            }

            if (taskEntity.LinkedChain != null)
            {
                return IsLoggedInToDialer(taskEntity);
            }

            var interviewDialingMode = GetDialingMode(taskEntity, survey, currentInterview);
                       
            var toSend = IsLoggedInToDialer(taskEntity) && (interviewDialingMode != DialingMode.Manual);

            return toSend;
        }

        /// <summary>
        /// Resets task fields to default values.
        /// <remarks>Resets survey sid only for users with not SURVEY_ASSIGNMENT mode.</remarks>
        /// Sets interview state to SELECTING for manual user and WAITING for others.
        /// </summary>
        /// <param name="task">Task entity to reset.</param>
        /// <param name="callOutcome">Extended status to set.</param>
        /// <param name="userTaskChoiceMode">Interviewer task choice mode.</param>
        internal static void ResetTaskState(BvTasksEntity task, CallOutcome callOutcome, AgentTaskChoiceMode userTaskChoiceMode)
        {
            InterviewState interviewState;
            bool isStateChanged = false;
            switch (userTaskChoiceMode)
            {
                case AgentTaskChoiceMode.Manual:
                    interviewState = InterviewState.SELECTING;
                    isStateChanged = true;
                    break;
                default:
                    interviewState = InterviewState.WAITING;
                    break;
            }
            
            ResetCommonTaskFields(task, interviewState, callOutcome, (int)DialerErrorCode.Success, isStateChanged);

            if (userTaskChoiceMode != AgentTaskChoiceMode.CampaignAssignment)
            {
                task.SurveySID = 0;
                task.DiallingMode = (int)DialingMode.Manual;
            }
        }

        internal static void ResetCommonTaskFields(
            BvTasksEntity task,
            InterviewState interviewState,
            CallOutcome callOutcome,
            int problemState,
            bool isStateChanged)
        {
            task.TzID = 0;
            task.CallID = 0;
            task.State = null;
            task.InterviewID = 0;
            task.DiallingMode = (int)DialingMode.Manual;
            task.TimeCallDelivered = null;
            task.ProblemId = problemState;
            task.CallOutcome = (int)callOutcome;
            task.InterviewState = (byte)interviewState;
            task.OpenEndReviewStartTime = null;
            task.CallType = (byte)CallTypes.Outbound;
            task.LinkedCallId = null;
            task.LinkedChain = null;
            task.CallConnectionState = (byte) CallConnectionState.NotDialed;
            task.Context.Clear();

            if (isStateChanged)
                task.TimeStateChanged = DateTime.UtcNow;

            task.DiallingMode = (int)GetDialingMode(task);
        }

        /// <summary>
        /// When a telephony error occurs:
        /// - Finish current interview
        /// - Put TELEPHONY_PROBLEM to "BVTasks"
        /// - Reschedule the interview
        /// </summary>
        /// <param name="dial">Active dial</param>
        /// <param name="task"></param>
        /// <param name="error">Telepnony error code.</param>
        public static void ProcessTelephonyError(BvActiveDialEntity dial, BvTasksEntity task, DialerErrorCode error)
        {
            bool isDialOwned = ActiveDialService.IsDialOwned(dial, task);
            //Reschedule the interview
            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.TelephonyError,
                ITS = (int)CallOutcome.TelephonyFailure,
                LastCallTime = task.TimeCallDelivered ?? DateTime.UtcNow,
                LastCallPersonSID = task.PersonSID,
                CallCenterID = task.CallCenterID,
                IsExecuteSchedulingScript = isDialOwned,
                opType = OperationType.TelephonyError
            };

            if(isDialOwned)
            {
                var callAttemptNumber = InterviewService.SafeIncrementAndFetchCallAttemptCount(task.SurveySID, task.InterviewID, (DialingMode)task.DiallingMode);
                options.CallAttemptNumber = callAttemptNumber;
            }

            InterviewService.Schedule(task.SurveySID, task.InterviewID, options);

            //Finish current interview (Update BvTasks) and Report the Telephony error
            ResetCommonTaskFields(task, InterviewState.NO_CALLS, CallOutcome.NotDefined, (int)error, false);
        }

        /// <summary>
        /// When an interview has telephone number from the blacklist:
        /// - Set ITS = "Blacklist" to the interview
        /// - Do not run scheduling
        /// - Delete call
        /// - Finish current interview
        /// </summary>
        public static void ProcessBlacklistInterview(
            BvTasksEntity task,
            BvInterviewEntity interview,
            AgentTaskChoiceMode taskChoiceMode)
        {
            using (var batch = TransferBatch.Create())
            {
                batch.Insert(new List<int> { task.InterviewID });

                BvSpInterviews_UpdateState_BatchAdapter.ExecuteNonQuery(
                    task.SurveySID,
                    batch.Value,
                    (int)CallOutcome.Blacklist);

                CallQueueService.DeleteCalls(task.SurveySID, batch.Value);
            }

            Trace.TraceWarning(
                "Interview was rejected because respondent telephone number exists in the telephone blacklist." +
                Environment.NewLine +
                " Survey Id: {0}, Interview Id: {1}, Interviewer Id: {2}, Telephone Number: {3}",
                task.SurveySID, task.InterviewID, task.PersonSID, interview.TelephoneNumber);

            ResetTaskState(
                task,
                CallOutcome.Blacklist,
                taskChoiceMode);
        }

        public void TakeBreak(
            BvTasksEntity task,
            BvSurveyEntity survey,
            DialerAction dialerAction,
            bool force)
        {
            var activityEvent = new TakeBreakEvent();
            
            var breakEntity = _breakTypeRepository.TryGetById(task.BreakTypeId.Value);
            if (breakEntity == null)
            {
                Trace.TraceWarning("TakeBreak was called with breakTypeId={0} which does not exist", task.BreakTypeId);
            }

            if ((LoginState)task.LoggedInToDialerState == LoginState.LOGGED_IN &&
                dialerAction == DialerAction.SendNoReady &&
                (DialingMode)task.DiallingMode == DialingMode.Predictive &&
                _dialerCollection.IsDialerInitialized(task.DialerId))
            {
                var result = _telephony.GoNotReady(
                    task.DialerId,
                    survey.CampaignId,
                    task.PersonSID.ToString(CultureInfo.InvariantCulture),
                    _timeBreakService.GetBreakTypeName(task.BreakTypeId));

                if (result != DialerErrorCode.Success)
                {
                    // TODO: !!!!! Why do we continue, user will be anyway logged out because of telephony error.
                    task.ProblemId = (int)result;
                }

                //It is needed to be sure that the call wont be delivered to interviewer in predicive mode after 'GoNotReady' was called.
                Thread.Sleep(_dialerSettings.InterviewerPredictiveSafeBreakWaitTimeout);
            }

            //if we call this method from wrap up, we should not update task directly and check that can we take a break
            if (!force)
            {
                using (var transaction = new DatabaseTransactionScope("TakeBreak"))
                {
                    var updateStatusLogoutEntity = BvSpTasks_UpdateStatusLogoutAdapter.ExecuteEntity(
                        task.PersonSID, (byte)LoginState.BREAK);


                    if ((LoginState)updateStatusLogoutEntity.PreviousStatusLogout == LoginState.BREAK ||
                        updateStatusLogoutEntity.InterviewID != 0)
                    {
                        activityEvent.Save(task.PersonSID);
                        return;
                    }

                    transaction.Commit();
                }

                //We get here when intervview Id = 0 - most likely user in selecting mode or waiting interview in predictive?. in other modes history record is saved in a standard way.
                //this is similar logic as in BvInterviewTimings_Delete SP - maybe some changes are required
                if (task.StartTime != null && task.SurveySID > 0)
                {
                    var history = new BvHistoryEntity()
                    {
                        SurveyId = task.SurveySID,
                        PersonSID = task.PersonSID,
                        RoleID = (byte)Role.Interviewer,
                        FiredTime = task.CurrentUtcTime.Value,
                        InterviewId = 0,
                        WaitingTime = TimeDiff.Seconds(task.StartTime.Value, task.CurrentUtcTime.Value),
                        CallCenterID = task.CallCenterID,
                        SessionId = task.SessionId
                    };

                    _historyRepository.Insert(history);
                }
            }

            if ((LoginState)task.StatusLogout == LoginState.BREAK)
            {
                activityEvent.Save(task.PersonSID);
                return;
            }

            task.StatusLogout = (byte)LoginState.BREAK;

            task.StartTime = null;
            _taskRepository.Update(task);

            BvSpStartInterviewerBreakAdapter.ExecuteNonQuery(task.PersonSID, survey == null ? 0 : survey.SID, task.BreakTypeId);

            activityEvent.Save(task.PersonSID);
        }

        public void SwitchSurvey(int dialerId, BvTasksEntity task)
        {
            var activityEvent = CreateSurveySwitchEvent(task);

            using (new EventDetailsScope(activityEvent.Details))
            {
                try
                {
                    if (task.NewSurveySID == task.SurveySID)
                    {
                        Trace.TraceWarning(string.Format("NewSurveySID [{0}] is the same as SurveySID. /// ",
                            task.NewSurveySID) + task.LogString());

                        activityEvent.Details.Result = "Skipped: NewSurveySID is the same as SurveySID";

                        return;
                    }

                    DoSwitchSurvey(dialerId, task);
                    activityEvent.Details.Result = "Success";
                }
                catch (Exception ex)
                {
                    activityEvent.Details.Result = "Exception: " + ex.Message;

                    // Do not re-throw in order to keep CATI Console working
                    Trace.TraceError(string.Format("SwitchSurvey is failed: {0} /// ", ex) + task.LogString());
                }
                finally
                {
                    activityEvent.Save();
                }
            }
        }

        private SurveySwitchEvent CreateSurveySwitchEvent(BvTasksEntity task)
        {
            var activityEvent = new SurveySwitchEvent
            {
                InterviewerSid = task.PersonSID,
                SurveySid = task.SurveySID,
                SurveyName = _surveyRepository.GetSurveyNameOrErrorString(task.SurveySID),

                Details =
                {
                    OldSurveySid = task.SurveySID,
                    NewSurveySid = task.NewSurveySID,
                    NewSurveyName = _surveyRepository.GetSurveyNameOrErrorString(task.NewSurveySID)
                }
            };

            return activityEvent;
        }

        private void DoSwitchSurvey(int dialerId, BvTasksEntity task)
        {
            var agentId = task.PersonSID;
            var loggedInToDialer = ((LoginState)task.LoggedInToDialerState == LoginState.LOGGED_IN);
            var newSurvey = _surveyRepository.GetById(task.NewSurveySID);

            if (loggedInToDialer)
            {
                try
                {
                    _telephony.SetCampaign(dialerId, newSurvey.CampaignId, agentId);
                    EventDetailsScope.Current.AddTiming("SetCampaign");
                }
                catch (Exception)
                {
                    TaskService.ResetNewSurveyId(task);

                    // Make agent ready if SetCampaign is failed. It's assumed we should continue to work in the old survey in this case.
                    var oldSurvey = _surveyRepository.GetById(task.SurveySID);
                    SendGoReadyIfNotABreak(dialerId, task, oldSurvey.CampaignId, agentId);

                    throw;
                }
            }

            TaskService.ApplyNewSurveyId(task);

            PersonService.FillLoginGroupsAndAsyncReschedule(agentId);
            EventDetailsScope.Current.AddTiming("PersonService.FillLoginGroupsAndAsyncReschedule");

            if (!loggedInToDialer)
            {
                return;
            }

            // Logged in to dialer. Extra steps are required.
            if (newSurvey.DialingMode == DialingMode.Predictive)
            {
                var userGroups = PersonTools.GetUserGroups(agentId);
                EventDetailsScope.Current.AddTiming("GetUserGroupsForSurvey");

                _telephony.SendSetGroups(dialerId, newSurvey.CampaignId, agentId, userGroups);
            }

            SendGoReadyIfNotABreak(dialerId, task, newSurvey.CampaignId, agentId);
        }

        private void SendGoReadyIfNotABreak(int dialerId, BvTasksEntity task, long newCampaignId, int agentId)
        {
            if (!IsBreakOrPendingBreak((LoginState)task.StatusLogout))
            {
                TryToSendGoReady(dialerId, newCampaignId, agentId, () => task.LogString());
            }
        }


        public void TryToSendSetCampaign(int dialerId, long campaignId, int agentId)
        {
            try
            {
                _telephony.SetCampaign(dialerId, campaignId, agentId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public void TryToSendGoReady(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc)
        {
            try
            {
                _telephony.SendGoReady(dialerId, campaignId, agentId, logInfoFunc);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public void TryToSendGoNotReady(int dialerId, long campaignId, long agentId, int? breakTypeId,
            Func<string> logInfoFunc)
        {
            try
            {
                _telephony.SendGoNotReady(
                    dialerId,
                    campaignId,
                    agentId.ToString(CultureInfo.InvariantCulture),
                    _timeBreakService.GetBreakTypeName(breakTypeId),
                    logInfoFunc);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public static DialingMode GetDialingMode(BvTasksEntity task)
        {
            BvSurveyEntity survey = null;
            BvInterviewEntity interview = null;
            
            if (task.SurveySID != 0)
                survey = SurveyRepository.GetById(task.SurveySID);
            if (task.InterviewID != 0)
                interview = InterviewRepository.GetById(task.SurveySID, task.InterviewID);

            return GetDialingMode(task, survey, interview);
        }

        public static DialingMode GetDialingMode(int dialerId, int surveyId)
        {
            var dialer = ServiceLocator.Resolve<IDialerCollection>().GetDialers().FirstOrDefault(x=>x.DialerId == dialerId);
            var survey = SurveyRepository.GetById(surveyId);

            return GetDialingMode(dialer == null? DialType.Landline : dialer.DialType, survey, null);
        }

        public static DialingMode GetDialingMode(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview)
        {
            return GetDialingMode(task.DialType, survey, interview);
        }

        public static DialingMode GetDialingMode(DialType dialType, BvSurveyEntity survey, BvInterviewEntity interview)
        {
            if (survey == null)
                return DialingMode.Manual;
            
            var dialingMode = survey.DialingMode;

            if (interview != null && interview.DialingMode != 0)
                dialingMode = (DialingMode)interview.DialingMode;

            if (dialType == DialType.Cellphone)
            {
                switch (dialingMode)
                {
                    case DialingMode.Manual:
                    case DialingMode.Preview:
                    case DialingMode.SpecialDial:
                        break;
                    case DialingMode.Automatic:
                    case DialingMode.Predictive:
                        dialingMode = DialingMode.Preview;
                        break;
                    default:
                        throw new Exception(string.Format("Unexpected dialing mode: {0}", dialingMode));
                }
            }

            return dialingMode;
        }

        private void LogDialerError(DialerErrorCode code, string message, params object[] args)
        {
            if (DialerErrorSeverityProvider.IsWarning(code))
                Trace.TraceWarning(message, args);

            Trace.TraceError(message, args);
        }
    }
}
