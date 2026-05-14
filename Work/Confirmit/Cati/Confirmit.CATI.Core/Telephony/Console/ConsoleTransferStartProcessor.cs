using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;
using System;
using System.Diagnostics;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using TransferType = ConfirmitDialerInterface.TransferType;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleTransferStartProcessor : IConsoleTransferStartProcessor
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonGroupRepository _personGroupRepository;
        private readonly IContextInfoService _contextInfoService;
        private readonly ICallQueueService _callQueueService;
        private readonly IStateRepository _stateRepository;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;
        private readonly ITransferService _transferService;

        public ConsoleTransferStartProcessor(
            ISurveyRepository surveyRepository,
            IPersonGroupRepository personGroupRepository,
            IContextInfoService contextInfoService,
            ICallQueueService callQueueService,
            IStateRepository stateRepository,
            IActiveDialRepository activeDialRepository,
            IActiveDialService activeDialService,
            ITransferService transferService)
        {
            _surveyRepository = surveyRepository;
            _personGroupRepository = personGroupRepository;
            _contextInfoService = contextInfoService;
            _callQueueService = callQueueService;
            _stateRepository = stateRepository;
            _activeDialRepository = activeDialRepository;
            _activeDialService = activeDialService;
            _transferService = transferService;
        }

        public void TransferStart(BvTasksEntity task, BvPersonEntity person, TransferOptions options, TransferStartEvent activityEvent)
        {
            activityEvent.UpdateEventPropertiesFromTask(task);

            var survey = _surveyRepository.GetById(task.SurveySID);

            if (survey.DialingMode == DialingMode.Manual)
            {
                throw new Exception("Transfer is not supported for surveys with manual dialing mode");
            }

            if (task.CallID == null)
                throw new UserMessageException($"Call id '{task.CallID}' not found");
            var dial = _activeDialRepository.GetByCallIdWithCheck((long)task.CallID);

            switch (options.Type)
            {
                case ConsoleTransferType.InternalCold:
                case ConsoleTransferType.InternalWarm:
                    ExecuteInternalTransfer(task, survey, options, activityEvent, dial);
                    break;
                case ConsoleTransferType.ExternalCold:
                case ConsoleTransferType.ExternalWarm:
                    ExecuteExternalTransfer(task, options, activityEvent, dial);
                    break;
                default:
                    throw new Exception($"Unsupported transfer type '{options.Type}'.");
            }
        }

        private void ExecuteInternalTransfer(BvTasksEntity task, BvSurveyEntity survey,
            TransferOptions options, TransferStartEvent activityEvent, BvActiveDialEntity dial)
        {
            BvPersonGroupEntity group = null;
            if (!string.IsNullOrEmpty(options.Resource))
            {
                group = _personGroupRepository.TryGetByName(options.Resource);
                if (group == null || group.IsAdministrative)
                    throw new UserMessageException($"Interviewer group '{options.Resource}' not found");
            }

            var initialTransferState = new TransferState()
            {
                InitiatorAgentId = task.PersonSID,
                InitiatorState = InitiatorState.Connected,
                ConnectionState = _activeDialService.GetInitialConnectionState(options.Type),
                TargetType = TargetType.AgentGroup,
                TargetOutcome = TargetOutcome.NotDefined,
                TargetResource = group?.SID.ToString(),
                TargetState = TargetState.WaitingForAgent
            };

            var consoleTransferState = _transferService.GetTransferState(initialTransferState, dial);

            TransferService.CheckDialerErrorCode(_activeDialService.TransferStart(dial, ConvertToDialerTransferType(options.Type), consoleTransferState));
            TransferService.CheckDialerErrorCode(_activeDialService.TransferSetConnectionState(dial, initialTransferState.ConnectionState));

            task.Context.TransferOptions = options;
            task.Context.TransferId = dial.TransferId;
            task.InterviewState = (int)InterviewState.OUTGOING_TRANSFER;

            var call = new BvCallEntity
            {
                InterviewID = task.InterviewID,
                SurveySID = task.SurveySID,
                CallID = (int)task.CallID,
                ResourceType = (int)CallExplicitType.Survey,
                Priority = _stateRepository.GetByItsAndStateGroupId((int)CallOutcome.InternalTransfer, survey.StateGroupID)
                    .Priority,
                ActiveDialId = dial.Id
            };

            if (group != null)
            {
                call.ResourceType = (int)CallExplicitType.PersonOrPersonGroup;
                call.Resource = (int)group.SID;
            }

            activityEvent.Details.TransferId = dial.TransferId;
            activityEvent.Details.TransferGroupId = group?.SID;
            activityEvent.Details.TransferGroupBehavior = group?.TransferBehavior;

            //TODO: refactor
            switch (survey.DialingMode)
            {
                case DialingMode.Predictive:
                    CreateCall(task, call, CallState.LoadedToDialerPredictively);

                    TransferService.CheckDialerErrorCode(_activeDialService.TransferSetTarget(dial, TargetType.AgentGroup, group?.SID.ToString(),
                            group?.TransferBehavior == TransferGroupBehavior.DeliverCallsFromOtherSurvey));
                    break;
                case DialingMode.Automatic:
                case DialingMode.Preview:
                    CreateCall(task, call, CallState.Scheduled);
                    break;
            }

        }

        private void ExecuteExternalTransfer(BvTasksEntity task, TransferOptions options, TransferStartEvent activityEvent, BvActiveDialEntity dial)
        {
            var initialTransferState = new TransferState()
            {
                InitiatorAgentId = task.PersonSID,
                InitiatorState = InitiatorState.Connected,
                ConnectionState = _activeDialService.GetInitialConnectionState(options.Type),
                TargetType = TargetType.External,
                TargetOutcome = TargetOutcome.NotDefined,
                TargetResource = options.Resource,
                TargetState = TargetState.Dialing
            };

            var consoleTransferState = _transferService.GetTransferState(initialTransferState, dial);

            TransferService.CheckDialerErrorCode(_activeDialService.TransferStart(dial, ConvertToDialerTransferType(options.Type), consoleTransferState));
            TransferService.CheckDialerErrorCode(_activeDialService.TransferSetConnectionState(dial, initialTransferState.ConnectionState));

            task.Context.TransferOptions = options;
            task.Context.TransferId = dial.TransferId;
            task.InterviewState = (int)InterviewState.OUTGOING_TRANSFER;

            activityEvent.Details.TransferId = dial.TransferId;

            //TODO: refactor
            TransferService.CheckDialerErrorCode(_activeDialService.TransferSetTarget(dial, TargetType.External, options.Resource, false));
        }

        private void CreateCall(BvTasksEntity task, BvCallEntity call, CallState callState)
        {
            call.CallState = (int)callState;

            using (new ConnectionScope())
            {
                _contextInfoService.WriteContextInfo(0, OperationType.InternalTransfer, task.CallCenterID);
                _callQueueService.AddCall(call);
                _callQueueService.ForceCallDelivery(call);
            }
        }

        private TransferType ConvertToDialerTransferType(Common.ConsoleService.Abstract.ConsoleTransferType type)
        {
            return (TransferType)Enum.Parse(typeof(TransferType), type.ToString());
        }
    }
}
