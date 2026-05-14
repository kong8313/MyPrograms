using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ApiClients.Models;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public class InboundCallService : IInboundCallService
    {
        protected readonly IToggleSettings ToggleSettings;
        protected readonly IInboundCallsHistoryRepository InboundCallsHistoryRepository;
        protected readonly IInboundTelephoneNumberRepository InboundTelephoneNumberRepository;
        protected readonly IInterviewRepository InterviewRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IShiftServiceFactory _shiftServiceFactory;
        private readonly ICallCenterRepository _callCenterRepository;
        private readonly IRespondentsClient _respondentsClient;
        private readonly IInterviewService _interviewService;
        protected readonly ITimeService TimeService;
        private readonly ICallQueueService _callQueueService;
        private readonly IContextInfoService _contextInfoService;

        public InboundCallService(IToggleSettings toggleSettings, 
            IInboundCallsHistoryRepository inboundCallsHistoryRepository, 
            IInboundTelephoneNumberRepository inboundTelephoneNumberRepository,
            IInterviewRepository interviewRepository,
            ISurveyRepository surveyRepository,
            IShiftServiceFactory shiftServiceFactory,
            ICallCenterRepository callCenterRepository,            
            IRespondentsClient respondentsClient,
            IInterviewService interviewService,
            ITimeService timeService,
            ICallQueueService callQueueService,
            IContextInfoService contextInfoService)
        {
            ToggleSettings = toggleSettings;
            InboundCallsHistoryRepository = inboundCallsHistoryRepository;
            InboundTelephoneNumberRepository = inboundTelephoneNumberRepository;
            InterviewRepository = interviewRepository;
            _surveyRepository = surveyRepository;
            _shiftServiceFactory = shiftServiceFactory;
            _callCenterRepository = callCenterRepository;
            _respondentsClient = respondentsClient;
            _interviewService = interviewService;
            TimeService = timeService;
            _callQueueService = callQueueService;
            _contextInfoService = contextInfoService;
        }

        public void CreateCallHistory(BvActiveDialEntity activeDial, InboundHandlerOperationType operationType)
        {
            InboundCallsHistoryRepository.Insert(new BvInboundCallsHistoryEntity
            {
                InterviewId = activeDial.InterviewId,
                SurveyId = activeDial.SurveyId,
                FiredTime = TimeService.GetUtcNow(),
                InboundTelNumber = activeDial.DialerTelephoneNumber,
                RespondentTelNumber = activeDial.RespondentTelephoneNumber,
                OperationType = (int)operationType,
                InboundCallId = activeDial.InboundCallId
            });
        }

        private bool CanCreateNewInterviews(InboundSurveyBehavior inboundBehavior)
        {
            return inboundBehavior != InboundSurveyBehavior.MatchOnly;
        }

        private bool CanMatchExistingInterviews(InboundSurveyBehavior inboundBehavior)
        {
            return inboundBehavior != InboundSurveyBehavior.CreateOnly;
        }
        
        public void CheckAndSearchInterview(string inboundLinePhoneNumber, string callerPhoneNumber, InterviewWithCall result)
        {
            if (!ToggleSettings.EnableInbound)
            {
                throw new InboundCallCantProceedException(
                    "'Inbound' feature is disabled",
                    DropInboundCallReason.InboundFeatureIsDisabled);
            }

            var inboundTelephone = InboundTelephoneNumberRepository.TryGetByTelephoneNumber(inboundLinePhoneNumber);
            if (inboundTelephone == null)
            {
                throw new InboundCallCantProceedException(
                    string.Format("DDI record is not found for [{0}] inbound line phone number", inboundLinePhoneNumber),
                    DropInboundCallReason.DdiRecordIsNotFound);
            }

            if (!inboundTelephone.SurveyId.HasValue)
            {
                throw new InboundCallCantProceedException(
                    string.Format("Survey is not defined for [{0}] inbound line phone number", inboundLinePhoneNumber),
                    DropInboundCallReason.SurveyIsNotFound);
            }

            result.Survey = _surveyRepository.TryGetById(inboundTelephone.SurveyId.Value);

            if (result.Survey == null)
            {
                throw new InboundCallCantProceedException(
                    string.Format("Survey is not found /// surveyId={0}", inboundTelephone.SurveyId),
                    DropInboundCallReason.SurveyIsNotFound);
            }

            var surveyState = (SurveyState)result.Survey.State;

            if (surveyState != SurveyState.Open)
            {
                throw new InboundCallCantProceedException(
                    string.Format("Survey is not opened /// surveyId={0}, state=[{1}]", result.Survey.SID, surveyState),
                    DropInboundCallReason.SurveyIsNotOpened);
            }

            if (!string.IsNullOrWhiteSpace(callerPhoneNumber) && CanMatchExistingInterviews(result.Survey.InboundBehavior))
            {
                result.Interview =
                    InterviewRepository.GetByTelephoneNumber(inboundTelephone.SurveyId.Value, callerPhoneNumber);
            }

            if (result.Interview == null && CanCreateNewInterviews(result.Survey.InboundBehavior))
            {
                var respondentsInfo = new RespondentsInfo
                {
                    Id = 0,
                    Values = new Dictionary<string, object>(),
                    Links = new Dictionary<string, string>()
                };
                respondentsInfo.Values.Add("TelephoneNumber", callerPhoneNumber);

                int respondentId = _respondentsClient.AddRespondent(result.Survey.ProjectId, respondentsInfo);

                result.Interview = _interviewService.AddRespondent(result.Survey, respondentId, 
                    new SchedulingScriptExecutionOptions(){
                        IsLogToHistory = false,
                        ITS = (int)CallOutcome.FreshSample,
                        ExecutionReason = SchedulingScriptExecutionReason.Added,
                        opType = OperationType.AddRecordByInboundCall,
                        RoleID = (int)Role.None,
                        PostSchedulingAction = schedule =>
                        {
                            if (schedule.NewCall != null)
                            {
                                schedule.NewCall.CallState = (int) CallState.InterviewInProgress;
                            }
                        }

                    });

                _contextInfoService.ResetContextInfo();

                result.Call = _callQueueService.GetCallWithTryLockAny(result.Interview.SurveySID, result.Interview.ID, out _);
                result.IsCallLockAcquired = result.Call != null;

                return;
            }

            if (result.Interview == null)
            {
                throw new InboundCallCantProceedException(
                    string.Format("Interview is not found /// surveyId={0}, callerPhoneNumber=[{1}]", inboundTelephone.SurveyId, callerPhoneNumber),
                    DropInboundCallReason.InterviewIsNotFound);
            }
            
            result.Call = _callQueueService.GetCallWithTryLock(result.Interview.SurveySID, result.Interview.ID, out bool isCallLocked);
            result.IsCallLockAcquired = isCallLocked;
        }

        public InboundHandlerOperationType InboundHandlerOperationTypeFromDropInboundCallReason(DropInboundCallReason dropInboundCallReason)
        {
            switch (dropInboundCallReason)
            {
                case DropInboundCallReason.InboundFeatureIsDisabled:
                    return InboundHandlerOperationType.DropBySystemInboundDisabled;

                case DropInboundCallReason.DdiRecordIsNotFound:
                    return InboundHandlerOperationType.DropBySystemDdiRecordNotFound;

                case DropInboundCallReason.UnexpectedCallState:
                    return InboundHandlerOperationType.DropBySystemWrongCallState;

                case DropInboundCallReason.CallLockIsNotAcquired:
                    return InboundHandlerOperationType.DropBySystemWrongCallState;

                case DropInboundCallReason.NotAcceptedBySchedulingScript:
                    return InboundHandlerOperationType.DropBySchedulingScript;

                case DropInboundCallReason.InterviewIsNotFound:
                    return InboundHandlerOperationType.DropBySystemInterviewNotFound;

                case DropInboundCallReason.SurveyIsNotOpened:
                    return InboundHandlerOperationType.DropBySystemSurveyIsNotOpened;

                case DropInboundCallReason.SurveyIsNotFound:
                    return InboundHandlerOperationType.DropBySystemSurveyIsNotFound;

                case DropInboundCallReason.ShiftIsNotFound:
                    return InboundHandlerOperationType.DropBySystemShiftIsNotFound;

                case DropInboundCallReason.NoAgentsAvailable:
                    return InboundHandlerOperationType.DropBySystemNoAgentsAvailable;

                case DropInboundCallReason.InternalServerError:
                    return InboundHandlerOperationType.DropBySystemInternalServerError;
            }

            return InboundHandlerOperationType.Undefined;
        }
    }
}