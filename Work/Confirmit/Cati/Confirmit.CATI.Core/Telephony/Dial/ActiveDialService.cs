using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;
using ConnectionState = ConfirmitDialerInterface.ConnectionState;
using TransferType = ConfirmitDialerInterface.TransferType;

namespace Confirmit.CATI.Core.Telephony.Dial
{
    public class ActiveDialService : IActiveDialService
    {
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly ITelephony _telephony;
        private readonly IInboundAudioMessages _inboundAudioMessages;
        private readonly IInboundCallService _inboundCallService;
        private readonly ITimeService _timeService;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly InterviewersAvailabilityService _interviewersAvailabilityService;
        private readonly ICallQueueService _callQueueService;
        private readonly IAssignmentService _assignmentService;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IToggleSettings _toggleSettings;
        private readonly IInterviewerApiClient _interviewerApiClient;
        private readonly ICompanyInfo _companyInfo;
        private readonly IRespondentVariablesService _respondentVariablesService;

        public ActiveDialService(
            IActiveDialRepository activeDialRepository,
            ITelephony telephony,
            IInboundAudioMessages inboundAudioMessages,
            IInboundCallService inboundCallService,
            ITimeService timeService,
            IInboundTelephoneNumberRepository inboundTelephoneNumberRepository,
            ITaskRepository taskRepository,
            InterviewersAvailabilityService interviewersAvailabilityService,
            ICallQueueService callQueueService,
            IAssignmentService assignmentService,
            ISurveyRepository surveyRepository,
            IInterviewRepository interviewRepository,
            IToggleSettings toggleSettings,
            IInterviewerApiClient interviewerApiClient,
            ICompanyInfo companyInfo, 
            IRespondentVariablesService respondentVariablesService)
        {
            _activeDialRepository = activeDialRepository;
            _telephony = telephony;
            _inboundAudioMessages = inboundAudioMessages;
            _inboundCallService = inboundCallService;
            _timeService = timeService;
            _inboundTelephoneNumberRepository = inboundTelephoneNumberRepository;
            _taskRepository = taskRepository;
            _interviewersAvailabilityService = interviewersAvailabilityService;
            _callQueueService = callQueueService;
            _assignmentService = assignmentService;
            _surveyRepository = surveyRepository;
            _interviewRepository = interviewRepository;
            _toggleSettings = toggleSettings;
            _interviewerApiClient = interviewerApiClient;
            _companyInfo = companyInfo;
            _respondentVariablesService = respondentVariablesService;
        }

        public BvActiveDialEntity CreateInboundCall(int dialerId, string inboundCallId, string ddiNumber,
            string telephoneNumber)
        {
            var surveyId = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(ddiNumber)?.SurveyId ?? 0;

            var dial = new BvActiveDialEntity() {
                CallType = CallTypes.Inbound,
                DialerId = dialerId,
                DialerTelephoneNumber = ddiNumber,
                RespondentTelephoneNumber = telephoneNumber,
                DialState = DialState.Pending,
                InboundCallId = inboundCallId,
                InitialSurveyId = surveyId
            };

            return _activeDialRepository.Insert(dial);
        }


        public BvActiveDialEntity CreateOutboundCall(int dialerId, BvSurveyEntity survey, BvInterviewEntity interview,
            string telephoneNumber, int callId)
        {
            var dial = new BvActiveDialEntity() {
                CallType = CallTypes.Outbound,
                DialerId = dialerId,
                DialerTelephoneNumber = interview.ExtensionNumber,
                RespondentTelephoneNumber = telephoneNumber,
                DialState = DialState.Dialing,
                InitialSurveyId = interview.SurveySID,
                SurveyId = interview.SurveySID,
                CampaignId = survey.CampaignId,
                InterviewId = interview.ID,
                CallId = callId
            };

            return _activeDialRepository.Insert(dial);
        }

        public InboundHandlerOperationType AcceptInboundCall(BvActiveDialEntity dial, BvSurveyEntity survey,
            BvInterviewEntity interview, BvCallEntity call)
        {
            dial.SurveyId = call.SurveySID;
            dial.CampaignId = survey.CampaignId;
            dial.InterviewId = call.InterviewID;
            dial.CallId = call.CallID;
            dial.DialState = survey.DialingMode != DialingMode.Predictive ? DialState.Pending : DialState.Queueing;

            call.DialerId = dial.DialerId;
            call.ActiveDialId = dial.Id;

            _activeDialRepository.Update(dial);

            if (survey.DialingMode != DialingMode.Predictive)
            {
                if (!_callQueueService.IsResourceLoggedIn(call.Resource, survey.SID))
                {
                    throw new InboundCallCantProceedException("There are no available interviewers",
                        DropInboundCallReason.NoAgentsAvailable);
                }

                _inboundCallService.CreateCallHistory(dial, InboundHandlerOperationType.PlacedInQueue);

                return InboundHandlerOperationType.PlacedInQueue;
            }

            var campaignIdsToBorrowAgentsFrom = new long[] { survey.CampaignId }; //TODO: Fill the array with campaign ids
            int? surveyId = survey.SID;
            var agentGroupId = 0;
            var agentId = 0;

            var assignment = _assignmentService.GetAssignemntInfo(call);
            switch (assignment.Type)
            {
                case CallAssignemntType.Group:
                case CallAssignemntType.Multi:
                    agentGroupId = call.Resource;
                    if (assignment.Groups.Any(
                            x => x.InboundBehavior == InboundGroupBehavior.DeliverCallsFromOtherSurvey))
                    {
                        surveyId = null;
                        campaignIdsToBorrowAgentsFrom = null;
                    }

                    break;
                case CallAssignemntType.Person:
                    agentId = call.Resource;
                    surveyId = null;
                    campaignIdsToBorrowAgentsFrom = null;
                    break;
            }

            // Predictive mode
            if (!IsAnyAssignedInterviewerAvailable(assignment, dial.DialerId, surveyId))
            {
                throw new InboundCallCantProceedException("There are no available interviewers",
                    DropInboundCallReason.NoAgentsAvailable);
            }

            DialingMode dialingMode;
            if (_toggleSettings.EnableInboundForPreviewInPredictiveMode)
            {
                dialingMode = DialingMode.Predictive;
            }
            else
            {
                dialingMode = interview.DialingMode == 0 ? DialingMode.Predictive : (DialingMode)interview.DialingMode;
            }

            var respondentVariables = _respondentVariablesService.GetVariablesToSend(survey.SID, dial.InterviewId);
            _telephony.ConnectInboundCall(
                dial.DialerId,
                survey.CampaignId,
                agentId,
                call.InterviewID,
                dial.InboundCallId,
                new CallInfo(
                    agentId,
                    call.InterviewID,
                    call.CallID, //TODO CODI changes: propagate callId 'long' type to the CATI DB
                    agentGroupId,
                    null, //phoneNumber,
                    null, //timeToCall
                    dialingMode,
                    false, //'wasAbandoned' must be taken from call in fact
                    0, //'attemptsMade' must be taken from call in fact
                    0, // 'previousConnects' must be taken from call in fact
                    0, // 'numberOfNoAnswer' must be taken from call in fact'
                    "", /*'PROTSInternalFlag' where is it kept? */
                    survey.IsWholeInterviewRecordingEnabled,
                    0, //agingTimeout
                    null, //callerId. potentially we can pass interview.ExtensionNumber
                    respondentVariables
                ),
                campaignIdsToBorrowAgentsFrom,
                null);

            _inboundCallService.CreateCallHistory(dial, InboundHandlerOperationType.SendToDialer);

            return InboundHandlerOperationType.SendToDialer;
        }

        public InboundHandlerOperationType DropInboundCall(BvActiveDialEntity dial,
            DropInboundCallReason dropInboundCallReason)
        {
            var audioMessageDescriptor =
                _inboundAudioMessages.FromDropCallReason(dial.DialerTelephoneNumber, dropInboundCallReason);
            var inboundHandlerOperationType =
                _inboundCallService.InboundHandlerOperationTypeFromDropInboundCallReason(dropInboundCallReason);

            _telephony.DropInboundCall(dial.DialerId, dial.InboundCallId, audioMessageDescriptor);
            _inboundCallService.CreateCallHistory(dial, inboundHandlerOperationType);

            _activeDialRepository.Delete(dial.Id, CallCompleteStatus.DropBySystem);

            return inboundHandlerOperationType;
        }

        private bool IsAnyAssignedInterviewerAvailable(CallAssignemntInfo assignment, int dialerId, int? surveyId)
        {
            switch (assignment.Type)
            {
                case CallAssignemntType.Survey:
                    return _interviewersAvailabilityService.IsAnyInterviewerAvailable(dialerId, surveyId.Value);
                case CallAssignemntType.Multi:
                case CallAssignemntType.Group:
                    var groupIds = assignment.Groups.Select(x => x.SID);
                    return _interviewersAvailabilityService.IsAnyInterviewerAvailable(dialerId, groupIds);
                case CallAssignemntType.Person:
                    return _interviewersAvailabilityService.IsInterviewerAvailable(dialerId, assignment.Person.SID);
                default:
                    return false;
            }
        }

        private DialerErrorCode ConnectDialToAgent(BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey)
        {
            /* Idea to next refactoring:
               Need to split DialingMode into two enums
               - InterviewDialingMode
                   * Manual - we should not use dialing operation at all.
                   * Preview - dial operation should be executed manually through using Dial command
                   * Automatic - dial operation will be executed automaticaly, before delivering interview in console
               - SurveyTelephonyAlgorith
                   * None - without telephony
                   * OnDemand\Manual - use onDemand algorithm which is controled on cati side. 
                   * Predictive - use predictive  algorithm on dialer side
            */
            var dialingMode = BvCallHandlerRoot.GetDialingMode(task, survey, null);
            switch (dialingMode)
            {
                case DialingMode.Preview:
                case DialingMode.Automatic:
                    if (IsPendingInbound(dial))
                    {
                        var respondentVariables = _respondentVariablesService.GetVariablesToSend(survey.SID, dial.InterviewId);
                        var call = new CallInfo
                        {
                            interviewId = dial.InterviewId,
                            callId = dial.CallId,
                            isRecording = survey.IsWholeInterviewRecordingEnabled,
                            agentId = task.PersonSID,
                            diallingMode = DialingMode.Manual,
                            respondentVariables = respondentVariables
                        };

                        return _telephony.ConnectInboundCallToAgent(
                            dial.DialerId,
                            dial.CampaignId,
                            task.PersonSID,
                            task.InterviewID,
                            dial.InboundCallId,
                            call,
                            null);
                    }
                    else if (IsPendingOutbound(dial))
                    {
                        var respondentVariables = _respondentVariablesService.GetVariablesToSend(survey.SID, dial.InterviewId);
                        return _telephony.SendNumberToAgent(
                            dial.DialerId,
                            dial.CampaignId,
                            task.PersonSID.ToString(CultureInfo.InvariantCulture),
                            //(!)We implement Fusion OPEN(=PREVIEW) dialing (DIALLING_MODE_OPEN)
                            //(!)via MN progressive. It's by design. So here we use DIALLING_MODE_PROGRESSIVE independent of Fusion dial mode.
                            DialingMode.Automatic,
                            dial.InterviewId,
                            dial.CallId,
                            dial.RespondentTelephoneNumber,
                            survey.IsWholeInterviewRecordingEnabled,
                            // It was decided to keep the name of ExtensionNumber field but to use it for storing Caller ID
                            dial.DialerTelephoneNumber,
                            respondentVariables);
                    }
                    else if (IsPendingTransfer(dial))
                    {
                        return TransferSetTarget(dial, TargetType.Agent, task.PersonSID.ToString(CultureInfo.InvariantCulture), false);
                    }

                    break;
                case DialingMode.Predictive:
                    if (IsPendingInbound(dial) || IsPendingOutbound(dial) || IsPendingTransfer(dial))
                    {
                        return _telephony.CompletePreview(
                            dial.DialerId,
                            dial.CampaignId,
                            task.PersonSID.ToString(CultureInfo.InvariantCulture),
                            dial.InterviewId,
                            dial.CallId,
                            dial.RespondentTelephoneNumber,
                            survey.IsWholeInterviewRecordingEnabled);
                    }

                    break;
            }

            throw new Exception("Can't connect dial to agent, because active dial has wrong state: {dial}");
        }



        public void OnDialNotifyOutcome(BvActiveDialEntity dial, BvTasksEntity task, CallOutcome callOutcome)
        {
            if (callOutcome == CallOutcome.Connected)
            {
                if (task != null)
                {
                    AttachDialToTaskContextIfNeed(dial, task);
                    task.Context.ActiveDialStart = _timeService.GetUtcNow();
                }

                switch (dial.CallType)
                {
                    case CallTypes.Inbound:
                    case CallTypes.Outbound:
                        dial.State = (byte)DialState.Connected;
                        if (dial.AnswerTime == null)
                            dial.AnswerTime = _timeService.GetUtcNow();
                        _activeDialRepository.Update(dial);
                        break;
                    case CallTypes.Transfer:
                        break;
                }

                if (dial.CallType == CallTypes.Inbound)
                {
                    _inboundCallService.CreateCallHistory(dial, InboundHandlerOperationType.ConnectedToAgent);
                }

                return;
            }

            // Process all not connected cases
            if (callOutcome == CallOutcome.DroppedByRespondent)
            {
                DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.DropByRespondent);
                if (dial?.CallType == CallTypes.Inbound)
                {
                    _inboundCallService.CreateCallHistory(dial, InboundHandlerOperationType.DropByRespondent);
                }
            }
            else
            {
                DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.NotConnected);
            }
        }

        public bool IsPendingInbound(BvActiveDialEntity dial)
        {
            if (dial.CallType != CallTypes.Inbound)
                return false;

            switch (dial.DialState)
            {
                case DialState.Pending:
                case DialState.Queueing:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsPendingOutbound(BvActiveDialEntity dial)
        {
            if (dial.CallType != CallTypes.Outbound)
                return false;

            switch (dial.DialState)
            {
                case DialState.Queueing:
                case DialState.Dialing:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsPendingTransfer(BvActiveDialEntity dial)
        {
            if (dial.CallType != CallTypes.Transfer)
                return false;

            switch (dial.DialState)
            {
                case DialState.Transfering:
                    return true;
                default:
                    return false;
            }
        }

        public DialerErrorCode Dial(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview,
            string telephoneNumber)
        {
            if (dial != null)
            {
                if (!IsPendingInbound(dial) && !IsPendingTransfer(dial))
                {
                    DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.DropByRespondent);
                    dial = null;
                }
            }

            if (dial == null)
            {
                dial = CreateOutboundCall(task.DialerId, survey, interview, telephoneNumber, (int)task.CallID);
                AttachDialToTaskContextIfNeed(dial, task);
            }

            SetCampaignIfNeeded(task, survey.CampaignId);

            return ConnectDialToAgent(dial, task, survey);
        }

        private void SetCampaignIfNeeded(BvTasksEntity task, long campaignId)
        {
            if (task.Context.CurrentCampaignId != campaignId)
            {
                try
                {
                    if (!_telephony.IsReloginNeededOnSurveyChange(task.DialerId))
                    {
                        _telephony.SetCampaign(task.DialerId, campaignId, task.PersonSID);
                    }
                }
                catch(Exception ex)
                {
                    Trace.TraceError($"Error during execution of SetCampaign: {ex}");
                }
                task.Context.CurrentCampaignId = campaignId;
            }
        }

        public DialerErrorCode Redial(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview,
            string telephoneNumber)
        {
            DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.DropByRespondent);

            dial = CreateOutboundCall(task.DialerId, survey, interview, telephoneNumber, (int)task.CallID);

            AttachDialToTaskContextIfNeed(dial, task);

            return _telephony.Redial(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(CultureInfo.InvariantCulture),
                task.InterviewID,
                task.CallID ?? 0,
                telephoneNumber,
                survey.IsWholeInterviewRecordingEnabled,
                // It was decided to keep the name of ExtensionNumber field but to use it for storing Caller ID
                interview.ExtensionNumber);
        }

        public DialerErrorCode SetNextInterview(BvActiveDialEntity dial, InterviewStatus status, BvCallEntity call)
        {
            var nextSurvey = _surveyRepository.GetById(call.SurveySID);

            var result = _telephony.SetNextInterview(
                dial.DialerId,
                dial.CampaignId,
                dial.MainPersonId.ToString(CultureInfo.InvariantCulture),
                status,
                nextSurvey.CampaignId,
                call.InterviewID,
                call.CallID);

            dial.SurveyId = call.SurveySID;
            dial.CampaignId = nextSurvey.CampaignId;
            dial.InterviewId = call.InterviewID;
            dial.CallId = call.CallID;

            return result;
        }

        public DialerErrorCode Hangup(BvTasksEntity task, BvSurveyEntity survey, int initiator)
        {
            var dial = _activeDialRepository.TryGetByCallId(task.CallID ?? 0);
            if (dial != null)
            {
                DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.CompleteByConsole);
            }

            task.CallConnectionState = (byte)CallConnectionState.Disconnected;

            return _telephony.Hangup(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(CultureInfo.InvariantCulture),
                task.InterviewID,
                task.CallID ?? 0);
        }

        public DialerErrorCode KillAgent(BvTasksEntity task, BvSurveyEntity survey)
        {
            var dial = _activeDialRepository.TryGetByCallId(task.CallID);
            if (dial != null)
            {
                DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.Terminated);
            }

            return _telephony.KillAgent(
                task.DialerId,
                survey.CampaignId,
                task.PersonSID.ToString(CultureInfo.InvariantCulture));
        }

        public void AttachDialToTaskContextIfNeed(BvActiveDialEntity dial, BvTasksEntity task)
        {
            if (task.Context.ActiveDialId == dial.Id)
                return;

            DetachDialFromTaskContextIfNeed(task.Context);

            task.Context.ActiveDialId = dial.Id;
            task.Context.TransferId = dial.TransferId;

            if (dial.CallType != CallTypes.Transfer && dial.MainPersonId == 0)
            {
                dial.MainPersonId = task.PersonSID;
                _activeDialRepository.Update(dial);
            }

        }

        public void DetachDialFromTaskContextIfNeed(TaskContext context)
        {
            if (context.ActiveDialId != null && context.ActiveDialStart != null)
            {
                context.DialHistories.Add(new TaskDialHistory()
                {
                    DialId = (long)context.ActiveDialId,
                    StartTime = (DateTime)context.ActiveDialStart,
                    FinishTime = _timeService.GetUtcNow()
                });
            }

            context.ActiveDialId = null;
            context.TransferId = null;
            context.ActiveDialStart = null;
        }

        private void DeleteAndDetachDialFromTaskIfNeed(BvTasksEntity task, BvActiveDialEntity dial,
            CallCompleteStatus callCompleteStatus)
        {
            if (task != null && task.Context.ActiveDialId != null)
            {
                DetachDialFromTaskContextIfNeed(task.Context);

                //Need to remvoe this update from here and do that on top layer
                _taskRepository.Update(task);
            }

            if (dial != null)
            {
                _activeDialRepository.Delete(dial.Id, callCompleteStatus);
            }
        }

        public DialerErrorCode CompleteCall(BvTasksEntity task, int dialerId, long campaignId, long agentId,
            int? callId, bool makeAgentReady, string breakName, InterviewStatus status,
            CallCompleteStatus callCompleteStatus)
        {
            var result = _telephony.CompleteCall(
                dialerId,
                campaignId,
                agentId.ToString(CultureInfo.InvariantCulture),
                task?.InterviewID ?? 0,
                makeAgentReady,
                breakName,
                status,
                callId ?? 0);

            var dial = _activeDialRepository.TryGetByCallId(callId);
            var cancelTransfer = false;
            if (dial?.MainPersonId == agentId)
            {
                switch (dial.DialState)
                {
                    case DialState.Transfering:
                        _telephony.TransferCancel(dial.DialerId, dial.CampaignId, dial.TransferId, task?.PersonSID ?? 0,
                            task?.InterviewID ?? 0);
                        cancelTransfer = true;
                        break;
                }

                _activeDialRepository.Delete(dial.Id, callCompleteStatus);

            }

            if (task != null)
            {
                DetachDialFromTaskContextIfNeed(task.Context);
            }

            if (cancelTransfer)
            {
                _interviewerApiClient.NotifyTransferFinished(_companyInfo.CompanyId, dial.SurveyId, dial.InterviewId, dial.TransferId);
            }

            return result;
        }

        public DialerErrorCode TransferStart(BvActiveDialEntity dial, TransferType transferType, ConsoleTransferState initialTransferState)
        {
            dial.TransferId = $"{dial.CallId}-{Guid.NewGuid()}";
            dial.CallType = CallTypes.Transfer;
            dial.DialState = DialState.Transfering;
            dial.DialTransferType = transferType;
            dial.TransferState = initialTransferState;

            _activeDialRepository.Update(dial);

            return _telephony.TransferStart(dial.DialerId, dial.CampaignId, dial.TransferId, dial.MainPersonId, dial.InterviewId, transferType);
        }

        public DialerErrorCode TransferSetConnectionState(BvActiveDialEntity dial, ConnectionState connectionState)
        {
            return _telephony.TransferSetConnectionState(dial.DialerId, dial.CampaignId, dial.TransferId, dial.MainPersonId, dial.InterviewId, connectionState);
        }

        public DialerErrorCode TransferSetTarget(BvActiveDialEntity dial, TargetType targetType, string resource,
            bool borrowAgentsFromAllCampaigns)
        {
            return _telephony.TransferSetTarget(dial.DialerId, dial.CampaignId, dial.TransferId, dial.MainPersonId, dial.InterviewId, targetType, resource, borrowAgentsFromAllCampaigns);
        }

        public DialerErrorCode TransferComplete(BvActiveDialEntity dial, BvTasksEntity task)
        {
            switch (dial.DialTransferType)
            {
                case TransferType.ExternalCold:
                case TransferType.ExternalWarm:
                    DeleteAndDetachDialFromTaskIfNeed(task, dial, CallCompleteStatus.CompleteByConsole);
                    _interviewerApiClient.NotifyTransferFinished(_companyInfo.CompanyId, dial.SurveyId, dial.InterviewId, dial.TransferId);
                    return _telephony.TransferComplete(dial.DialerId, dial.CampaignId, dial.TransferId, dial.MainPersonId, dial.InterviewId);
                case TransferType.InternalCold:
                case TransferType.InternalWarm:
                    dial.MainPersonId = 0;
                    _interviewerApiClient.NotifyTransferFinished(_companyInfo.CompanyId, dial.SurveyId, dial.InterviewId, dial.TransferId);
                    _activeDialRepository.Update(dial);
                    return DialerErrorCode.Success;
                default:
                    throw new Exception("Unknown Transfer type");
            }
        }

        public DialerErrorCode TransferCancel(BvActiveDialEntity dial)
        {
            var transferId = dial.TransferId;
            
            if (_activeDialRepository.TryGetById(dial.Id) != null)
            {
                dial.CallType = CallTypes.Outbound;
                dial.DialState = DialState.Connected;
                dial.TransferId = null;
                dial.TransferState = null;
                dial.TransferType = null;
                dial.DialTransferType = null;
                _activeDialRepository.Update(dial);
            }

            _interviewerApiClient.NotifyTransferFinished(_companyInfo.CompanyId, dial.SurveyId, dial.InterviewId, transferId);

            return _telephony.TransferCancel(dial.DialerId, dial.CampaignId, transferId, dial.MainPersonId, dial.InterviewId);
        }

        public void SetTransferState(BvActiveDialEntity dial, ConsoleTransferState transferState)
        {
            dial.TransferState = transferState;
            _activeDialRepository.Update(dial);
        }

        public BvActiveDialEntity CreateOutboundCall(int dialerId, long campaignId, long callId)
        {
            var survey = _surveyRepository.GetByCampaignId(campaignId);
            var call = _callQueueService.GetCall(callId);
            var interview = _interviewRepository.GetByIdWithCheck(call.SurveySID, call.InterviewID);
            return CreateOutboundCall(dialerId, survey, interview, interview.TelephoneNumber, (int)callId);
        }

        public ConnectionState GetInitialConnectionState(ConsoleTransferType transferType)
        {
            switch (transferType)
            {
                case ConsoleTransferType.InternalCold:
                case ConsoleTransferType.ExternalCold:
                    return ConnectionState.TargetToRespondent;
                case ConsoleTransferType.InternalWarm:
                case ConsoleTransferType.ExternalWarm:
                    return ConnectionState.InitiatorToTarget;
                default:
                    throw new Exception($"Unsupported transfer type '{transferType}'.");
            }
        }

        public DialerErrorCode TransferConfirm(BvActiveDialEntity dial, BvPersonEntity person)
        {
            if (dial.MainPersonId != 0)
            {
                throw new Exception($"Active dial doesn't ready to be completed, becouse the dial is owned by PID={dial.MainPersonId}");
            }

            dial.MainPersonId = person.SID;
            dial.DialState = DialState.Connected;
            _activeDialRepository.Update(dial);

            return _telephony.TransferComplete(dial.DialerId, dial.CampaignId, dial.TransferId, dial.MainPersonId, dial.InterviewId);
        }

        public static bool IsTransferReadyToComplete(BvActiveDialEntity dial, BvTasksEntity task)
        {
            return dial.DialState == DialState.Transfering &&
                   dial.TransferId == task.Context.TransferId &&
                   dial.MainPersonId == 0;
        }

        public static bool IsDialOwned(BvActiveDialEntity dial, BvTasksEntity task)
        {
            return task.Context.TransferId == null ||
                dial.DialState == DialState.Transfering &&
                dial.TransferId == task.Context.TransferId &&
                (dial.MainPersonId == 0 || dial.MainPersonId == task.PersonSID);
        }

        public int CleanActiveDials(TimeSpan expirationPeriod)
        {
            var expirationTime = DateTime.UtcNow.Subtract(expirationPeriod);
            var deletedRows =  BvActiveDialAdapter.DeleteByConditionAndOutput("StartTime < @ExpirationTime", new SqlParameter("ExpirationTime", expirationTime));

            return deletedRows.Count();
        }
    }
}
