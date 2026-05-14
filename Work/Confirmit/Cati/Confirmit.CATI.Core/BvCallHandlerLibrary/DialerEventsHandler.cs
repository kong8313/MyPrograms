using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.PerformanceCounters;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Telephony.NotificationHandlers;
using Newtonsoft.Json;

namespace BvCallHandlerLibrary
{
    public class DialerEventsHandler : IDialerEventsHandler
    {
        private readonly Lazy<ICompanyInfo> _companyInfo;
        private readonly Lazy<ISurveyRepository> _surveyRepository;
        private readonly Lazy<ICallDeliveryService> _callDeliveryService;
        private readonly Lazy<IContextInfoService> _contextInfoService;
        private readonly Lazy<ITaskRepository> _taskRepository;
        private readonly Lazy<IInterviewService> _interviewService;
        private readonly Lazy<ITelephony> _telephony;
        private readonly Lazy<IBvCallHandlerRoot> _callHandlerRoot;
        private readonly Lazy<IDialerLoginLogoutManager> _dialerLoginLogoutManager;
        private readonly Lazy<IIvrConsoleService> _ivrConsoleService;
        private readonly Lazy<IDialerNotifyInboundCallHandler> _dialerNotifyInboundCallHandler;
        private readonly Lazy<IDialerNotifyInboundCallDroppedByRespondentHandler> _dialerNotifyInboundCallDroppedByRespondentByRespondentHandler;
        private readonly Lazy<IDialerNotifyCallDroppedByRespondentHandler> _dialerNotifyCallDroppedByRespondentByRespondentHandler;
        private readonly Lazy<IActiveDialService> _activeDialService;
        private readonly Lazy<ITaskExtension> _taskExtension;
        private readonly Lazy<ITelephoneBlacklistService> _telephoneBlacklistService;
        private readonly Lazy<IInterviewRepository> _interviewRepository;
        private readonly Lazy<ITimeBreakService> _timeBreakService;
        private readonly Lazy<ICallQueueService> _callQueueService;
        private readonly Lazy<IActiveDialRepository> _activeDialRepository;
        private readonly Lazy<ITransferService> _transferService;
        private readonly Lazy<IInterviewerApiClient> _interviewerApiClient;
        private readonly Lazy<IDialerCollection> _dialerCollection;
        private readonly Lazy<IToggleSettings> _toggleSettings;
        
        public static readonly int MaxCallAgingTimeoutInMin = 15;

        public DialerEventsHandler()
        {
            _companyInfo = new Lazy<ICompanyInfo>(() => ServiceLocator.Resolve<ICompanyInfo>());
            _surveyRepository = new Lazy<ISurveyRepository>(() => ServiceLocator.Resolve<ISurveyRepository>());
            _callDeliveryService = new Lazy<ICallDeliveryService>(() => ServiceLocator.Resolve<ICallDeliveryService>());
            _contextInfoService = new Lazy<IContextInfoService>(() => ServiceLocator.Resolve<IContextInfoService>());
            _taskRepository = new Lazy<ITaskRepository>(() => ServiceLocator.Resolve<ITaskRepository>());
            _interviewService = new Lazy<IInterviewService>(() => ServiceLocator.Resolve<IInterviewService>());
            _telephony = new Lazy<ITelephony>(() => ServiceLocator.Resolve<ITelephony>());
            _callHandlerRoot = new Lazy<IBvCallHandlerRoot>(() => ServiceLocator.Resolve<IBvCallHandlerRoot>());
            _dialerLoginLogoutManager = new Lazy<IDialerLoginLogoutManager>(() => ServiceLocator.Resolve<IDialerLoginLogoutManager>());
            _ivrConsoleService = new Lazy<IIvrConsoleService>(() => ServiceLocator.Resolve<IIvrConsoleService>());
            _dialerNotifyInboundCallHandler = new Lazy<IDialerNotifyInboundCallHandler>(() => ServiceLocator.Resolve<IDialerNotifyInboundCallHandler>());
            _dialerNotifyInboundCallDroppedByRespondentByRespondentHandler = new Lazy<IDialerNotifyInboundCallDroppedByRespondentHandler>(() => ServiceLocator.Resolve<IDialerNotifyInboundCallDroppedByRespondentHandler>());
            _activeDialService = new Lazy<IActiveDialService>(() => ServiceLocator.Resolve<IActiveDialService>());
            _taskExtension = new Lazy<ITaskExtension>(() => ServiceLocator.Resolve<ITaskExtension>());
            _telephoneBlacklistService = new Lazy<ITelephoneBlacklistService>(() => ServiceLocator.Resolve<ITelephoneBlacklistService>());
            _interviewRepository = new Lazy<IInterviewRepository>(() => ServiceLocator.Resolve<IInterviewRepository>());
            _dialerNotifyCallDroppedByRespondentByRespondentHandler = new Lazy<IDialerNotifyCallDroppedByRespondentHandler>(() => ServiceLocator.Resolve<IDialerNotifyCallDroppedByRespondentHandler>());
            _timeBreakService = new Lazy<ITimeBreakService>(() => ServiceLocator.Resolve<ITimeBreakService>());
            _callQueueService = new Lazy<ICallQueueService>(() => ServiceLocator.Resolve<ICallQueueService>());
            _activeDialRepository = new Lazy<IActiveDialRepository>(() => ServiceLocator.Resolve<IActiveDialRepository>());
            _transferService = new Lazy<ITransferService>(() => ServiceLocator.Resolve<ITransferService>());
            _interviewerApiClient = new Lazy<IInterviewerApiClient>(() => ServiceLocator.Resolve<IInterviewerApiClient>());
            _dialerCollection = new Lazy<IDialerCollection>(() => ServiceLocator.Resolve<IDialerCollection>());
            _toggleSettings = new Lazy<IToggleSettings>(() => ServiceLocator.Resolve<IToggleSettings>());
        }

        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="customerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="callDialingMode"></param>
        public void OnDialerScreenPop(int dialerId, string customerId, long campaignId, int agentId, string contactId, int callId, DialingMode callDialingMode)
        {
            var evt = new OnDialerScreenPopEvent();

            var logStr = string.Format(
                "DialerEvents.OnDialerScreenPop(dialerId='{0}', customerId='{1}', campaignID='{2}'," +
                " agentID='{3}', contactID(interviewId)={4}, callID={5}, callDialingMode={6})",
                dialerId, customerId, campaignId, agentId, contactId, callId, callDialingMode);

            var survey = _surveyRepository.Value.GetByCampaignId(campaignId);

            evt.AddTiming("SurveyRepository.GetByCampaignId");

            if (survey.DialingMode != DialingMode.Predictive)
            {
                return;
            }

            BvActiveDialEntity dial = null;
            
            var task = _taskRepository.Value.GetByPerson(agentId);
            evt.AddTiming("TaskRepository.GetByPerson");
            
            evt.UpdateEventPropertiesFromTask(task);

            if (task?.IsWebConsole == true)
            {
                _interviewerApiClient.Value.NotifyScreenPop(_companyInfo.Value.CompanyId, dialerId, customerId, campaignId, agentId, contactId, callId, callDialingMode);
                evt.AddTiming("InterviewerApiClient.NotifyScreenPop");
                evt.Save(task.InterviewID, callId, callDialingMode);
                return;
            }

            using (TaskLocker.TryLock(agentId, out task))
            {
                evt.AddTiming("TaskLocker.TryLock");

                if (task != null) //task locked
                {
                    //We have to read active dial before attempt to lock call. If it is 
                    dial = _activeDialRepository.Value.TryGetByCallId(callId);
                    if (dial == null)
                        dial = _activeDialService.Value.CreateOutboundCall(dialerId, campaignId, callId);
                    evt.AddTiming("_activeDialRepository.TryGetByCallId/_activeDialService.CreateOutboundCall");

                    var call = _callQueueService.Value.GetCallWithTryLockAny(dial.SurveyId, dial.InterviewId, out var callLocked);
                    evt.AddTiming("_callQueueService.GetCallWithTryLockAny");

                    if (call == null || !callLocked)
                    {
                        _activeDialService.Value.CompleteCall(null, dialerId, campaignId, agentId, callId, false, null,
                            InterviewStatus.FromCallOutcome(CallOutcome.InternalTransfer), CallCompleteStatus.DropBySystem);

                        Trace.TraceError($"OnDialerScreenPop was ignored because call does not exist or already locked. callState={call?.CallState}");
                        return;
                    }

                    var interview = _interviewRepository.Value.GetByIdWithCheck(dial.SurveyId, dial.InterviewId);
                    evt.AddTiming("InterviewRepository.GetById");

                    _taskExtension.Value.AssignCallOnTask(task, survey, interview, call, dial);
                    _taskExtension.Value.SetInterviewingState(task, dial);

                    if (dial.TransferId != null)
                    {
                        task.InterviewState = (byte)InterviewState.WAITING;
                        _activeDialService.Value.Dial(ref dial, task, survey, interview, null);
                    }

                    _taskRepository.Value.Update(task);
                    evt.AddTiming("TaskRepository.Update");
                }
                else //task was terminated
                {
                    OnTaskWasTerminated(dialerId, campaignId, agentId, callId, false, task.BreakTypeId, null);
                    evt.AddTiming("OnTaskWasTerminated");
                }
            }

            evt.Save(dial?.InterviewId ?? 0, callId, callDialingMode);
        }

        public void OnDialerNotifyCallDroppedByRespondent(
            int dialerId,
            string companyId,
            long campaignId,
            long agentId,
            long callId)
        {
            _dialerNotifyCallDroppedByRespondentByRespondentHandler.Value.Execute(dialerId, companyId, campaignId, agentId, callId);
        }

        public void OnDialerNotifyOutcome(
            int dialerId,
            string tenantId,
            long campaignId,
            long agentId,
            string contactId,
            long callId,
            long outcome,
            string callerId, 
            TimeSpan ringTime, 
            Dictionary<string, string> callOutcomeMetadata)
        {
            CallOutcome fusionOutcome = _telephony.Value.TranslateOutcome(outcome);
            var notifyOutcomeParameters = new NotifyOutcomeParameters {
                DialerId = dialerId,
                TenantId = tenantId,
                CampaignId = campaignId,
                AgentId = agentId,
                CallId = callId, //TODO CODI changes: propagate long for callId to GetCallInfo
                RawOutcome = outcome,
                TranslatedOutcome = fusionOutcome,
                DialerCallerId = callerId,
                RingTime = (int)ringTime.TotalSeconds,
                CallOutcomeMetadata = callOutcomeMetadata?.ToArray()
            };

            var dialType = _dialerCollection.Value.GetDialers().FirstOrDefault(x => x.DialerId == dialerId)?.DialType;
            
            CustomMetrics.OnCallOutcome(dialerId, fusionOutcome, dialType);

            OnDialerCallEventBase evt = GetDialerCallEvent(fusionOutcome);
            evt.Details = notifyOutcomeParameters;

            var task = _taskRepository.Value.GetByPerson((int)agentId);
            evt.AddTiming("TaskRepository.GetByPerson");
            
            // Some special outcomes that should be handled here and not in Interviewer.Api
            if (task?.IsWebConsole == true && !(fusionOutcome == CallOutcome.ReturnedDiallerExpired || fusionOutcome == CallOutcome.ReturnedNotDialled || fusionOutcome == CallOutcome.Stopped))
            {
                evt.UpdateEventPropertiesFromTask(task);
                
                var call = _callQueueService.Value.GetCall((int)notifyOutcomeParameters.CallId);
                evt.AddTiming("CallQueueService.GetCall");
                
                var interview = _interviewRepository.Value.GetByIdWithCheck(call.SurveySID, call.InterviewID);
                evt.AddTiming("InterviewRepository.GetByIdWithCheck");
                
                evt.InterviewId = interview.ID;
                evt.PhoneNumber = interview.TelephoneNumber;
                _interviewerApiClient.Value.NotifyOutcome(_companyInfo.Value.CompanyId, dialerId, tenantId, campaignId, (int)agentId, contactId, callId, fusionOutcome, callerId, (int)ringTime.TotalSeconds, callOutcomeMetadata);
                evt.AddTiming("InterviewerApiClient.NotifyOutcome");
                
                evt.Save();
                return;
            }
                
            using (TaskLocker.TryLock((int)agentId, out task))
            {
                evt.AddTiming("TaskLocker.TryLock");
                    
                var dial = _activeDialRepository.Value.TryGetByCallId(notifyOutcomeParameters.CallId);

                if (dial == null)
                {
                    if (notifyOutcomeParameters.TranslatedOutcome == CallOutcome.Connected)
                    {
                        dial = _activeDialService.Value.CreateOutboundCall(notifyOutcomeParameters.DialerId, notifyOutcomeParameters.CampaignId, notifyOutcomeParameters.CallId);
                       
                    }
                }
                else
                {
                    notifyOutcomeParameters.IsPendingInboundCall = dial.CallType == CallTypes.Inbound;
                }

                if (dial != null)
                {
                    dial.JsonCallOutcomeMetadata = callOutcomeMetadata != null ? JsonConvert.SerializeObject(callOutcomeMetadata) : null;
                    dial.RingTime = notifyOutcomeParameters.RingTime;
                    dial.DialerCallerId = notifyOutcomeParameters.DialerCallerId;
                    _activeDialService.Value.OnDialNotifyOutcome(dial, task, notifyOutcomeParameters.TranslatedOutcome);
                }

                if (task != null && task.InterviewState == (int)InterviewState.REDIALLING)
                {
                    if (task.CallID != notifyOutcomeParameters.CallId)
                    {
                        Trace.TraceError($"Incorrect CallId={notifyOutcomeParameters.CallId} in NotifyOutcome on Redial. Task[{task}]");
                        return;
                    }

                    if (notifyOutcomeParameters.TranslatedOutcome == CallOutcome.DroppedByRespondent)
                    {
                        Trace.TraceError($"Ignore incorrect InboundCallDroppedByRespondent outcome for redial operation. Task[{task}]");
                        return;
                    }

                    notifyOutcomeParameters.IsRedialEvent = true;
                    OnRedial(task, fusionOutcome, evt);
                    return;
                }

                if (fusionOutcome == CallOutcome.Connected)
                {
                    OnCallConnected(task, notifyOutcomeParameters, evt);
                }
                else
                {
                    OnCallNotConnected(task, notifyOutcomeParameters, evt);
                }
            }
        }

        public void OnDialerNotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId,
            int agentId, int interviewId, long callId, CallOutcome callOutcome)
        {
            _interviewerApiClient.Value.NotifyCustomIvrInterviewEnd(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
        }

        public void OnDialerNotifyInboundCall(
            int dialerId,
            int companyId,
            string ddiNumber,
            string cliNumber,
            string inboundCallId)
        {
            _dialerNotifyInboundCallHandler.Value.Execute(
                dialerId,
                companyId,
                ddiNumber,
                cliNumber,
                inboundCallId);
        }

        public void OnDialerNotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId)
        {
            _dialerNotifyInboundCallDroppedByRespondentByRespondentHandler.Value.Execute(
                dialerId,
                companyId,
                inboundCallId);
        }

        public void OnDialerIvrSubmit(int dialerId, string companyId, long campaignId, long agentId, KeyValuePair<string, string>[] variables)
        {
            if (_toggleSettings.Value.CatiAgent.IvrThread)
            {                
                _interviewerApiClient.Value.NotifyIvrSubmit(dialerId, companyId, campaignId, agentId, variables);
                return;
            }
            using (var locker = TaskLocker.TryLock((int)agentId))
            {
                var task = _taskRepository.Value.GetByPerson((int)agentId);

                if (locker != null)
                {
                    _ivrConsoleService.Value.ProcessIvrSubmit(task, campaignId, variables);
                }
                else
                {
                    OnTaskWasTerminated(
                        dialerId,
                        int.Parse(companyId),
                        agentId,
                        0,
                        true,
                        task.BreakTypeId,
                        null);
                }
            }
        }

        public void OnTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {
            var logStr =
                $@"DialerEvents.OnTransferState({nameof(dialerId)}='{dialerId}', {nameof(companyId)}='{companyId}'
                , {nameof(transferId)}='{transferId}', {nameof(transferState)}='{transferState})";

            var dial = _activeDialRepository.Value.TryGetByTransferId(transferId);
            if (dial != null)
            {
                var evt = new OnDialerTransferStateEvent();
                var consoleTransferState = _transferService.Value.GetTransferState(transferState, dial);

                _activeDialService.Value.SetTransferState(dial, consoleTransferState);
                _ivrConsoleService.Value.ProcessTransferState(dial, transferId, transferState);

                _interviewerApiClient.Value.NotifyUpdatingTransferState(_companyInfo.Value.CompanyId, dialerId, transferId, consoleTransferState);
                evt.AddTiming("InterviewerApiClient.NotifyUpdatingTransferState");

                evt.Save();
            }
            else
            {
                Trace.TraceWarning($"Unhandled TransferState event. {logStr}");
            }
        }

        public void OnDialerNotifyAgentState(int dialerId, string tenantId, long campaignId, long agentId, string agentStateMsg)
        {
            var logStr = string.Format(
                "DialerEvents.OnDialerNotifyAgentState(dialerId='{0}', tenantId='{1}', campaignId='{2}'," +
                " agentId='{3}', agentStateMsg='{4}', company='{5}'",
                dialerId, tenantId, campaignId,
                agentId, agentStateMsg, _companyInfo.Value.CompanyId);

            var evt = new OnDialerNotifyAgentStateEvent();

            var agentState = (AgentStateMsgs)Convert.ToInt32(agentStateMsg);

            evt.Details.NotificationState = agentState;
            evt.InterviewerSid = (int)agentId;

            if (campaignId > 0)
            {
                var survey = _surveyRepository.Value.GetByCampaignId(campaignId);
                evt.SurveySid = survey.SID;
                evt.SurveyName = survey.Name;
            }

            BvTasksEntity task = null;

            switch (agentState)
            {
                case AgentStateMsgs.LOGGEDIN:
                    task = _taskRepository.Value.GetByPerson((int)agentId);
                    evt.AddTiming("TaskRepository.GetByPerson");

                    if (task == null)
                    {
                        Trace.TraceError(
                            "There is no record in BvTasks, but AgentStateMsgs.NOTREADY is received. /// " + logStr,
                            agentId);

                        return;
                    }

                    evt.Details.CurrentState = (LoginState)task.StatusLogout;
                    evt.Details.CurrentDialerState = (LoginState)task.LoggedInToDialerState;

                    if (task.IsWebConsole)
                    {
                        _interviewerApiClient.Value.NotifyUpdatingAgentState(_companyInfo.Value.CompanyId, dialerId, tenantId,
                            campaignId, (int)agentId, agentState);
                        evt.AddTiming("InterviewerApiClient.NotifyUpdatingAgentState");
                        evt.Save();
                        return;
                    }

                    if (!_callHandlerRoot.Value.IsPendingSurveySwitch(task))
                    {
                        // We send GoReady if there is no survey switch in progress. Otherwise GoReady is sent after the switch is complete.
                        using (new EventDetailsScope(evt.Details))
                        {
                            _telephony.Value.SendGoReady(dialerId, campaignId, agentId, () => logStr);
                        }
                    }

                    task.LoggedInToDialerState = (byte)LoginState.LOGGED_IN;

                    BvSpTasks_UpdateLoggedInToDialerStateAdapter.ExecuteNonQuery(
                        (int)agentId,
                        (byte)LoginState.LOGGED_IN);
                    evt.AddTiming("BvSpTasks_UpdateLoggedInToDialerStateAdapter");

                    //Predictive support                    

                    BvSurveyEntity survey = null;
                    if (campaignId > 0)
                    {
                        survey = _surveyRepository.Value.GetByCampaignId(campaignId);
                        evt.AddTiming("SurveyRepository.GetByCampaignId");
                    }

                    //Please review this changes attentively
                    if (survey != null && task.SurveySID != survey.SID)
                    {
                        if (task.InterviewID != 0)
                        {
                            Trace.TraceError($"OnDialerNotifyAgentState notification was ignored, because related to unexpected survey(SID:{survey.SID}, ProjectId:{survey.ProjectId}). Task:{task}");

                            return;
                        }

                        task.SurveySID = survey.SID;

                        task.DialingMode = BvCallHandlerRoot.GetDialingMode(task, survey, null);

                        evt.AddTiming("SurveyService.GetDialingMode");
                    }

                    if (task.DialingMode == DialingMode.Predictive)
                    {
                        evt.AddTiming("PREDICTIVE");

                        _taskRepository.Value.Update(task);
                        evt.AddTiming("TaskRepository.Update");


                        var userGroups = PersonTools.GetUserGroups((int)agentId);
                        evt.AddTiming("GetUserGroupsForSurvey");

                        using (new EventDetailsScope(evt.Details))
                        {
                            _telephony.Value.SendSetGroups(dialerId, campaignId, agentId, userGroups);
                        }
                    }

                    break;

                case AgentStateMsgs.LOGGEDOUT:
                    evt.AddTiming("LOGGEDOUT");

                    // Changes to the inter state have to be syncronized
                    using (var taskLock = TaskLocker.TryLock((int)agentId))
                    {
                        evt.AddTiming("TaskLocker.TryLock");

                        if (taskLock != null)
                        {
                            task = _taskRepository.Value.GetByPerson((int)agentId);
                            evt.AddTiming("TaskRepository.GetByPerson");

                            evt.Details.CurrentState = (LoginState)task.StatusLogout;
                            evt.Details.CurrentDialerState = (LoginState)task.LoggedInToDialerState;

                            if (task.IsWebConsole)
                            {
                                _interviewerApiClient.Value.NotifyUpdatingAgentState(_companyInfo.Value.CompanyId, dialerId,
                                    tenantId, campaignId, (int)agentId, agentState);
                                evt.AddTiming("InterviewerApiClient.NotifyUpdatingAgentState");
                                evt.Save();
                                return;
                            }

                            if (task.StatusLogout != (byte)LoginState.LOGGING_OUT)
                            {
                                Trace.TraceWarning("Unexpected AgentStateMsgs.LOGGEDOUT is received. /// " + logStr);
                                break;
                            }

                            if ((task.LoggedInToDialerState != (byte)LoginState.LOGGING_IN))
                            {
                                /* Set NOT_LOGGED_IN if interviewer is NOT logging into dialer otherwise allow him to retry */
                                task.StatusLogout = (byte)LoginState.NOT_LOGGED_IN;
                            }

                            task.LoggedInToDialerState = (byte)LoginState.NOT_LOGGED_IN;

                            _taskRepository.Value.Update(task);
                            evt.AddTiming("TaskRepository.Update");
                        }
                    }

                    break;

                case AgentStateMsgs.NOTREADY:
                    evt.AddTiming("NOTREADY");

                    using (var taskLock = TaskLocker.TryLock((int)agentId))
                    {
                        evt.AddTiming("TaskLocker.TryLock");

                        if (taskLock != null)
                        {
                            task = _taskRepository.Value.GetByPerson((int)agentId);
                            evt.AddTiming("TaskRepository.GetByPerson");

                            evt.Details.CurrentState = (LoginState)task.StatusLogout;
                            evt.Details.CurrentDialerState = (LoginState)task.LoggedInToDialerState;

                            if (task.IsWebConsole)
                            {
                                _interviewerApiClient.Value.NotifyUpdatingAgentState(_companyInfo.Value.CompanyId, dialerId,
                                    tenantId, campaignId, (int)agentId, agentState);
                                evt.AddTiming("InterviewerApiClient.NotifyUpdatingAgentState");
                                evt.Save();
                                return;
                            }

                            if (_callHandlerRoot.Value.IsPendingSurveySwitch(task))
                            {
                                _callHandlerRoot.Value.SwitchSurvey(dialerId, task);
                                evt.AddTiming("BvCallHandlerRoot.SwitchSurvey");
                            }
                        }
                        else
                        {
                            Trace.TraceError(
                                "Task lock is failed on AgentStateMsgs.NOTREADY. /// " + logStr);
                        }
                    }

                    break;

                default:

                    Trace.TraceWarning(
                        "Incorrect agentState=[{0}] received from dialer ///" + logStr, agentStateMsg);

                    break;
            }

            if (task != null)
                _ivrConsoleService.Value.ProcessAgentState(task);

            evt.Save();
        }

        public void OnDialerRequestCalls(
            int dialerId,
            string requestId,
            string tenantId,
            long campaignId,
            int? groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int callCount)
        {
            var performanceCounters = ServiceLocator.Resolve<IPerformanceCountersContainer>();

            performanceCounters.RequestCallsCount.Increment();

            try
            {
                var timer = Stopwatch.StartNew();

                var evt = new DialerRequestCallsEvent(
                    requestId,
                    groupId,
                    callCount,
                    callsSelectionAlgorithm,
                    tenantId,
                    dialerId);

                var survey = _surveyRepository.Value.GetByCampaignId(campaignId);

                var isRecording = survey.IsWholeInterviewRecordingEnabled;

                List<GroupInfo> aggregatedGroupsInfo;
                var callList = _callDeliveryService.Value.LookupCalls(
                    survey.SID,
                    dialerId,
                    groupId,
                    callsSelectionAlgorithm,
                    callCount,
                    survey.RecWholeInt > 0,
                    out aggregatedGroupsInfo);
                //TODO CODI changes: for now we get the isRecording flag from the survey properties, later we would add isRecording property to each call?

                performanceCounters.RequestCallsDuration.IncrementBy(timer.Elapsed);

                if (survey.IsTelephoneBlacklistSupported)
                {
                    callList = DeleteBlacklistedCalls(survey.SID, callList);
                }

                evt.ObjectId = survey.SID;
                evt.ObjectName = survey.Name;
                evt.Details.IsRecording = isRecording;
                evt.Details.CallsSent = callList.Count;
                evt.Details.AggregatedGroupsInfo = aggregatedGroupsInfo;
                evt.Finish();

                var snevt = new SendNumbersEvent(requestId,
                    groupId,
                    callCount,
                    callsSelectionAlgorithm,
                    tenantId,
                    dialerId);

                //TODO: a test which ensures that SendNumbers is called even while the callList is empty is required.
                var sendNumbersResult = _telephony.Value.SendNumbers(
                    dialerId,
                    requestId,
                    campaignId,
                    (DialingMode)survey.DialMode,
                    callList,
                    MaxCallAgingTimeoutInMin, // minutes //TODO: Should be configurable
                    isRecording);

                snevt.Details.SendNumbersResult = sendNumbersResult;
                snevt.ObjectId = survey.SID;
                snevt.ObjectName = survey.Name;
                snevt.Details.IsRecording = isRecording;
                snevt.Details.CallsSent = callList.Count;
                snevt.Finish();
                
                CustomMetrics.OnSendCallsToDialer(callsSelectionAlgorithm, callCount, callList.Count);
            }
            finally
            {
                performanceCounters.RequestCallsCount.Decrement();
            }
        }

        /// <summary>
        /// Removes blacklisted calls from the callsList.
        /// Sets Blacklist state for the corresponding interviews.
        /// Deletes calls from the system.
        /// </summary>
        /// <param name="surveySid">Survey Id</param>
        /// <param name="callsList">List of call to process</param>
        private List<CallInfo> DeleteBlacklistedCalls(int surveySid, IReadOnlyList<CallInfo> callsList)
        {
            var badNumbers = _telephoneBlacklistService.Value.GetBlacklistedNumbers(callsList.Select(x => x.phoneNumber));
            var badCalls = callsList.Where(x => badNumbers.Contains(x.phoneNumber)).ToList();

            if (badCalls.Any())
            {
                Trace.TraceWarning(
                    "Dialing was rejected because following respondent telephone numbers exist in the telephone blacklist." +
                    Environment.NewLine +
                    "Telephone Numbers: {0}",
                    string.Join(Environment.NewLine, badCalls.Select(x => x.phoneNumber).ToArray()));

                using (var batch = TransferBatch.Create())
                {
                    batch.Insert(badCalls.Select(x => x.interviewId));

                    BvSpInterviews_UpdateState_BatchAdapter.ExecuteNonQuery(
                        surveySid,
                        batch.Value,
                        (int)CallOutcome.Blacklist);

                    CallQueueService.DeleteCalls(surveySid, batch.Value);
                }
            }

            return callsList.Except(badCalls).ToList();
        }

        private OnDialerCallEventBase GetDialerCallEvent(CallOutcome callOutcome)
        {
            if (callOutcome == CallOutcome.Connected)
            {
                return new OnDialerCallConnectedEvent();
            }

            return new OnDialerCallNotConnectedEvent();
        }

        private void OnRedial(BvTasksEntity task, CallOutcome outcome, OnDialerCallEventBase evt)
        {
            evt.UpdateEventPropertiesFromTask(task);

            task.InterviewState = (byte)InterviewState.INTERVIEWING;
            task.CallOutcome = (int)outcome;

            task.CallConnectionState = outcome == CallOutcome.Connected
                ? (byte)CallConnectionState.Connected
                : (byte)CallConnectionState.Disconnected;

            _taskRepository.Value.Update(task);

            evt.AddTiming("TaskRepository.Update");
            evt.Save();
        }

        private string FormatNotifyOutcomeParametersString(
            string method,
            NotifyOutcomeParameters parameters)
        {
            var formattedParameters = string.Format(
                "DialerEvents.{0} (" +
                  "DialerId = '{1}', " +
                  "TenantId = '{2}', " +
                  "CampaignID = '{3}', " +
                  "AgentID = '{4}', " +
                  "CallID = '{5}', " +
                  "InterviewId = '{6}', " +
                  "RawOutcome = '{7}', " +
                  "TranslatedOutcome = '{8}')",
                method,
                parameters.DialerId,
                parameters.TenantId,
                parameters.CampaignId,
                parameters.AgentId,
                parameters.CallId,
                parameters.InterviewId,
                parameters.RawOutcome,
                parameters.TranslatedOutcome);

            return formattedParameters;
        }

        //Auxiliary functions
        private void OnCallConnected(
            BvTasksEntity task,
            NotifyOutcomeParameters notifyOutcomeParameters,
            OnDialerCallEventBase evt
            )
        {
            if (task != null)//task locked
            {
                BvCallEntity call = _callQueueService.Value.GetCall((int)notifyOutcomeParameters.CallId);
                BvInterviewEntity interview = _interviewRepository.Value.GetByIdWithCheck(call.SurveySID, call.InterviewID);

                _taskExtension.Value.UpdateOnCallConnected(task, interview, call);
                
                if (call.CallState != (int)CallState.InterviewInProgress)
                {
                    _interviewService.Value.BindDialerIdToInterview(interview, notifyOutcomeParameters.DialerId);
                    evt.AddTiming("BindDialerIdToInterview");

                    _callQueueService.Value.GetCallWithTryLockAny(call.SurveySID, call.InterviewID, out _);
                    evt.AddTiming("callQueueService.GetCallWithTryLockAny");
                }

                _ivrConsoleService.Value.ProcessCallOnConnect(task);

                evt.InterviewId = task.InterviewID;
                evt.PhoneNumber = interview.TelephoneNumber;
                evt.UpdateEventPropertiesFromTask(task);

                //TODO 1: We could get the phone number from the interview service, but it would be the wrong number in case:
                // "survey gets the telephone number from one of previous questions of the survey." So the strigng above is commented for a while. 
                //TODO 2: InterviewService.GetPhoneNumber can throw exception if interview is not found, 
                //Here we call this method for logging purposes only, so we must ignore any exceptions. 
                //We should either catch it or make another GetPhoneNumber method which does not throw any exceptions.
            }
            else //task no lock
            {
                OnTaskWasTerminated(
                    notifyOutcomeParameters.DialerId,
                    notifyOutcomeParameters.CampaignId,
                    notifyOutcomeParameters.AgentId,
                    (int)notifyOutcomeParameters.CallId,
                    true,
                    null,
                    GetCallOutcomeData(notifyOutcomeParameters)); //TODO CODI changes: propagate long for callId to GetCallInfo
                evt.AddTiming("OnTaskWasTerminated");
            }

            evt.Save();
        }

        private void OnTaskWasTerminated(int dialerId, long campaignId, long agentId, int callId, bool incrementCallAtemptCount, int? breakTypeId, CallOutcomeData callOutcomeData)
        {
            int surveySid;

            //
            // get necessary information from call because there is no such info in the task table
            var call = CallQueueService.GetCallInfo(callId);

            if (call != null)
            {
                surveySid = call.SurveySID;
            }
            else
            {
                Trace.TraceWarning("A call is received from dialer, but task and call don't exist");
                surveySid = _surveyRepository.Value.GetByCampaignId(campaignId).SID;
            }

            //There is no record in BvTasks for the interviewer, so the interviewer is logged out.
            //But we obtained a connected call for the interviewer, so we must:
            //Send CompleteCall to dialer and logout the interviewer from the dialer.

            //CompleteCall 
            var result = _activeDialService.Value.CompleteCall(
                null,
                dialerId,
                campaignId,
                agentId,
                callId,
                false,
                _timeBreakService.Value.GetBreakTypeName(breakTypeId),
                InterviewStatus.FromCallOutcome(CallOutcome.Terminated),
                CallCompleteStatus.Error);

            if (result != DialerErrorCode.Success)
            {
                Trace.TraceError(
                    "DialerEvents.OnTaskWasTerminated: Failed to complete call on dialer while terminating a task. " +
                    "/// Error code={0}, dialerId={1}, campaignID={2}, agentID={3}, callID={4}, incrementCallAtemptCount={5}",
                    result, dialerId, campaignId, agentId, callId, incrementCallAtemptCount);
            }

            //MaximL:we don't have ehogth information here to detect dial type correctly.
            DialingMode dialingMode = BvCallHandlerRoot.GetDialingMode(dialerId, surveySid);

            if (call != null)
            {
                var personId = Convert.ToInt32(agentId);

                var options = new SchedulingScriptExecutionOptions {
                    ExecutionReason = SchedulingScriptExecutionReason.Terminated,
                    ITS = (int)CallOutcome.ReturnedNotDialled,
                    LastCallTime = DateTime.UtcNow,
                    LastCallPersonSID = personId,
                    CallCenterID = PersonService.GetPersonCallCenterId(personId),
                    opType = OperationType.TerminateTask,
                    DialingAttempts = callOutcomeData?.ToDialingAttempts()
                };

                if (incrementCallAtemptCount)
                {
                    var callAttemptNumber = InterviewService.SafeIncrementAndFetchCallAttemptCount(call.SurveySID, call.InterviewID, dialingMode);
                    options.CallAttemptNumber = callAttemptNumber;
                }

                InterviewService.Schedule(call.SurveySID, call.InterviewID, options);
            }

            var logoutResult = _dialerLoginLogoutManager.Value.Logout(
                dialerId,
                campaignId,
                dialingMode == DialingMode.Predictive,
                (int)agentId);

            if (logoutResult != DialerErrorCode.Success)
            {
                Trace.TraceError(
                    "DialerEvents.OnTaskWasTerminated: Failed to logout from dialer while terminating a task. " +
                    "/// Error code={0}, dialerId={1}, campaignID={2}, agentID={3}, callID={4}, incrementCallAtemptCount={5}",
                    result, dialerId, campaignId, agentId, callId, incrementCallAtemptCount);
            }
        }

        private void OnCallNotConnected(
            BvTasksEntity task,
            NotifyOutcomeParameters notifyOutcomeParameters,
            OnDialerCallEventBase evt)
        {
            var callOutcomeData = GetCallOutcomeData(notifyOutcomeParameters);
            
            var call = CallQueueService.GetCallInfo((int)notifyOutcomeParameters.CallId);
            evt.AddTiming("CallQueueService.GetCallInfo");

            if ((task != null) && (task.SurveySID == call.SurveySID && task.InterviewID == call.InterviewID)) //task locked
            {
                evt.UpdateEventPropertiesFromTask(task);

                //new Sytel changes generates NotifyOutcome with InboundCallDroppedByRespondent ITS in all cases when respondent drop call.
                //Initially, to fix side effects in our behavior we will ignore processing of the notification.
                if (notifyOutcomeParameters.TranslatedOutcome == CallOutcome.DroppedByRespondent &&
                    !notifyOutcomeParameters.IsPendingInboundCall)
                {
                    evt.Save(
                        "Outcome 'DroppedByRespondent' was ignored. Sytel generates CallDroppedByRespondent event in all cases when respondent drop call.");

                    return;
                }

                var dialingMode = BvCallHandlerRoot.GetDialingMode(task);
                evt.AddTiming("BvCallHandlerRoot.GetDialingMode");

                task.DiallingMode = (int)dialingMode;

                if (ProcessSpecialOutcomesForPredictive(
                    (int)notifyOutcomeParameters.CallId,
                    notifyOutcomeParameters.AgentId,
                    dialingMode,
                    callOutcomeData)
                    )
                {
                    evt.Save();

                    return;
                }

                var personTaskChoice = (AgentTaskChoiceMode)PersonRepository.GetById((int)notifyOutcomeParameters.AgentId).ManualSelection;
                evt.AddTiming("PersonRepository.GetById");

                if (dialingMode == DialingMode.Automatic)
                {
                    var makeAgentReady = task.StatusLogout != (byte)LoginState.PENDING_LOGOUT;

                    _activeDialService.Value.CompleteCall(
                        task,
                        notifyOutcomeParameters.DialerId,
                        notifyOutcomeParameters.CampaignId,
                        notifyOutcomeParameters.AgentId,
                        (int)notifyOutcomeParameters.CallId,
                        makeAgentReady,
                        makeAgentReady ? null : _timeBreakService.Value.GetBreakTypeName(task.BreakTypeId),
                        InterviewStatus.FromCallOutcome(notifyOutcomeParameters.TranslatedOutcome),
                        CallCompleteStatus.NotConnected);
                    evt.AddTiming("CompleteCall");
                }

                if (dialingMode == DialingMode.Automatic || dialingMode == DialingMode.Predictive)
                {
                    //
                    // runs OnSchedule event and updates an ITS of the interview
                    var options = new SchedulingScriptExecutionOptions() {
                        ExecutionReason = SchedulingScriptExecutionReason.NotConnected,
                        ITS = (int)notifyOutcomeParameters.TranslatedOutcome,
                        LastCallTime = task.TimeCallDelivered ?? DateTime.UtcNow,
                        LastCallPersonSID = Convert.ToInt32(notifyOutcomeParameters.AgentId),
                        CallCenterID = task.CallCenterID,
                        opType = OperationType.NotConnectedCall,
                        DialingAttempts = callOutcomeData.ToDialingAttempts()
                    };

                    ScheduleInterviewAndIncrementCallAttemptCountIfNeed(task.SurveySID, task.InterviewID, options, dialingMode, evt.Details);

                    BvCallHandlerRoot.ResetTaskState(task, notifyOutcomeParameters.TranslatedOutcome, personTaskChoice);
                    evt.AddTiming("ResetTaskState");

                    if (task.StatusLogout == (byte)LoginState.PENDING_LOGOUT)
                    {
                        task.StatusLogout = (byte)LoginState.LOGGING_OUT;
                        task.LoggedInToDialerState = (byte)LoginState.LOGGING_OUT;

                        _taskRepository.Value.Update(task);
                        evt.AddTiming("TaskRepository.Update");

                        var survey = _surveyRepository.Value.GetById(task.SurveySID);

                        var logoutResult = _dialerLoginLogoutManager.Value.Logout(
                            notifyOutcomeParameters.DialerId,
                            survey.CampaignId,
                            !task.IsLoginRCToDialer,
                            task.PersonSID);
                        evt.AddTiming("Logout");

                        if (logoutResult != DialerErrorCode.Success)
                        {
                            var parameters = FormatNotifyOutcomeParametersString("OnCallNotConnected", notifyOutcomeParameters);

                            //Report the Telephony error and continue logout.
                            Trace.TraceError("Person '{0}' logout from Dialer failed. Error code: '{1}'" + parameters,
                                task.PersonSID, logoutResult);

                            task.ProblemId = (int)logoutResult;

                            _taskRepository.Value.Update(task);
                            evt.AddTiming("TaskRepository.Update");
                        }

                        evt.Save();

                        return;
                    }

                    if (dialingMode != DialingMode.Predictive &&
                        personTaskChoice != AgentTaskChoiceMode.Manual)
                    {
                        // OnCallNotConnected
                        var person = PersonRepository.GetById((int)notifyOutcomeParameters.AgentId);

                        _contextInfoService.Value.ResetContextInfo();
                        _callHandlerRoot.Value.LookupCallForInterviewer(
                            task,
                            person,
                            evt.Details);
                        evt.AddTiming("LookupCallForInterviewer");
                    }

                    _taskRepository.Value.Update(task);
                    evt.AddTiming("TaskRepository.Update");
                }
                else if (dialingMode == DialingMode.Preview ||
                         dialingMode == DialingMode.SpecialDial
                    )
                {
                    //Dial unsucceeded, set interviewer state from DIALLING back to INTERVIEWING, so CATI console 
                    //will allow interviewer to continue the interview.
                    task.InterviewState = (byte)InterviewState.INTERVIEWING;

                    task.CallOutcome = (int)notifyOutcomeParameters.TranslatedOutcome;
                    _taskRepository.Value.Update(task);
                    evt.AddTiming("TaskRepository.Update");
                }
            }
            else //task no lock
            {
                evt.AddTiming("Task is not locked");

                int surveySid;

                evt.UpdateEventPropertiesFromCall(call);

                if (call != null)
                {
                    surveySid = call.SurveySID;
                }
                else
                {
                    var parameters = FormatNotifyOutcomeParametersString("OnCallNotConnected", notifyOutcomeParameters);

                    Trace.TraceWarning(
                        "Receive OnNotifyOutcome, but task and call don't exist(Scheduling does not run ///"
                        + parameters);

                    surveySid = _surveyRepository.Value.GetByCampaignId(notifyOutcomeParameters.CampaignId).SID;
                    evt.AddTiming("SurveyRepository.GetByBvId");
                }

                var diallingMode = BvCallHandlerRoot.GetDialingMode(notifyOutcomeParameters.DialerId, surveySid);
                evt.AddTiming("SurveyService.GetDialingMode");

                if (ProcessSpecialOutcomesForPredictive(
                    (int)notifyOutcomeParameters.CallId,
                    notifyOutcomeParameters.AgentId,
                    diallingMode,
                    callOutcomeData)
                    )
                {
                    evt.Save();

                    return;
                }

                evt.AddTiming("ProcessSpecialOutcomesForPredictive");

                if (call != null)
                {
                    // runs OnSchedule event and updates an ITS of the interview by info from call                        
                    var options = new SchedulingScriptExecutionOptions {
                        ExecutionReason = SchedulingScriptExecutionReason.NotConnected,
                        ITS = (int)notifyOutcomeParameters.TranslatedOutcome,
                        LastCallTime = DateTime.UtcNow,
                        LastCallPersonSID = Convert.ToInt32(notifyOutcomeParameters.AgentId),
                        opType = OperationType.NotConnectedCall,
                        DialingAttempts = callOutcomeData.ToDialingAttempts()
                    };

                    ScheduleInterviewAndIncrementCallAttemptCountIfNeed(call.SurveySID, call.InterviewID, options, diallingMode, evt.Details);
                }

                //There is no record in BvTasks for the interviewer, so the interviewer is logged out.
                //But we obtained a not-connected call notification for the interviewer, so we must
                //try to complete call and logout the interviewer from the dialer.

                if (diallingMode != DialingMode.Predictive)
                {
                    if (notifyOutcomeParameters.DialerId == 0)
                    {
                        var parameters = FormatNotifyOutcomeParametersString("OnCallNotConnected", notifyOutcomeParameters);

                        Trace.TraceError(
                            "DialerEvents.OnCallNotConnected: DialerId is zero, so CompleteCall and Logout were not performed, " +
                            parameters);

                        evt.Save();
                        return;
                    }

                    _activeDialService.Value.CompleteCall(
                        null,
                        notifyOutcomeParameters.DialerId,
                        notifyOutcomeParameters.CampaignId,
                        notifyOutcomeParameters.AgentId,
                        (int)notifyOutcomeParameters.CallId,
                        false,
                        null,
                        InterviewStatus.FromCallOutcome(notifyOutcomeParameters.TranslatedOutcome),
                        CallCompleteStatus.NotConnected);
                    evt.AddTiming("CompleteCall");

                    var survey = _surveyRepository.Value.GetById(surveySid);
                    var logoutResult = _dialerLoginLogoutManager.Value.Logout(
                        notifyOutcomeParameters.DialerId,
                        survey.CampaignId,
                        false,
                        (int)notifyOutcomeParameters.AgentId);
                    evt.AddTiming("DialerLoginLogoutManager().Logout");

                    if (logoutResult != DialerErrorCode.Success)
                    {
                        var parameters = FormatNotifyOutcomeParametersString("OnCallNotConnected", notifyOutcomeParameters);

                        Trace.TraceError(
                            "DialerEvents.OnCallNotConnected: Failed to logout from dialer while handling outcome for not logged in agent. /// Error code={0}, " +
                            parameters,
                            logoutResult);
                    }
                }
                else
                {
                    // We must not logout on not connected calls notification in predictive
                }
            }

            evt.Save();
        }

        private static void ScheduleInterviewAndIncrementCallAttemptCountIfNeed(int surveyId, int interviewId,
            SchedulingScriptExecutionOptions options, DialingMode diallingMode, IEventDetails eventDetails)
        {
            if (options.ITS != (int)CallOutcome.ReturnedDiallerExpired &&
                options.ITS != (int)CallOutcome.ReturnedNotDialled)
            {
                var callAttemptNumber = InterviewService.SafeIncrementAndFetchCallAttemptCount(surveyId, interviewId, diallingMode);
                options.CallAttemptNumber = callAttemptNumber;
                
                eventDetails.AddTiming("InterviewService.SafeIncrementCallAttemptCount");
            }
            else
            {
                options.IsLogToHistory = false;
                if (options.ITS == (int)CallOutcome.ReturnedDiallerExpired)
                    options.opType = OperationType.ExpireByDialler;
                else if (options.ITS == (int)CallOutcome.ReturnedNotDialled)
                    options.opType = OperationType.ReturnNotDialled;
            }

            InterviewService.Schedule(surveyId, interviewId, options);

            eventDetails.AddTiming("InterviewService.Schedule");
        }

        private static bool ProcessSpecialOutcomesForPredictive(int callId,
            long agentId,
            DialingMode diallingMode,
            CallOutcomeData callOutcomeData)
        {
            if (diallingMode == DialingMode.Predictive)
            {
                if (callOutcomeData.DialerCallOutcome == CallOutcome.Stopped)
                {
                    ProcessOnNotifyOutcomePredictiveNotUsedImpl(callId);
                    return true;
                }

                if (callOutcomeData.DialerCallOutcome == CallOutcome.ReturnedNotDialled)
                {
                    // predictive call not dialed
                    ProcessOnNotifyOutcomePredictiveNotSentImpl(callId, agentId, callOutcomeData);
                    return true;
                }
            }

            return false;
        }

        private static void ProcessOnNotifyOutcomePredictiveNotSentImpl(
            int callId,
            long agentId,
            CallOutcomeData callOutcomeData)
        {
            var call = CallQueueService.GetCallInfo(callId);

            if (call == null)
            {
                throw new Exception(string.Format("call [{0}] is not found", callId));
            }

            var options = new SchedulingScriptExecutionOptions {
                ExecutionReason = SchedulingScriptExecutionReason.NotConnected,
                ITS = (int)callOutcomeData.DialerCallOutcome,
                LastCallTime = DateTime.UtcNow,
                LastCallPersonSID = Convert.ToInt32(agentId),
                opType = OperationType.ReturnNotDialled,
                IsLogToHistory = false,
                DialingAttempts = callOutcomeData.ToDialingAttempts()
            };

            InterviewService.Schedule(call.SurveySID, call.InterviewID, options);
        }

        private static void ProcessOnNotifyOutcomePredictiveNotUsedImpl(int callId)
        {
            var call = CallQueueService.GetCallInfo(callId);

            if (call == null)
            {
                throw new Exception(string.Format("call [{0}] is not found", callId));
            }
            
            CallQueueService.ReleaseCall(call.SurveySID, call.InterviewID);
        }

        private CallOutcomeData GetCallOutcomeData(NotifyOutcomeParameters notifyOutcomeParameters)
        {
            return new CallOutcomeData() {
                DialerCallerId = notifyOutcomeParameters.DialerCallerId,
                RingTime = notifyOutcomeParameters.RingTime,
                DialerCallOutcome = notifyOutcomeParameters.TranslatedOutcome,
                CallOutcomeMetadata = notifyOutcomeParameters.CallOutcomeMetadata?.ToDictionary(x => x.Key, x => x.Value)
            };
        }

        private class CallOutcomeData
        {
            public string DialerCallerId { get; set; }
            public int RingTime { get; set; }
            public CallOutcome DialerCallOutcome { get; set; }
            public Dictionary<string, string> CallOutcomeMetadata { get; set; }

            public CatiDialingAttempt[] ToDialingAttempts()
            {
                return new[] {
                    new CatiDialingAttempt() {
                        DialerCallerId = DialerCallerId,
                        RingTime = RingTime,
                        DialerCallOutcome = (int)DialerCallOutcome,
                        CallOutcomeMetadata = CallOutcomeMetadata
                    }
                };
            }
        }
    }
}
