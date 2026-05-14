using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class TransferService : ITransferService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IPersonGroupRepository _personGroupRepository;
        private readonly IInterviewRepository _interviewRepository;

        public TransferService(
            IPersonRepository personRepository, 
            IPersonGroupRepository personGroupRepository,
            IInterviewRepository interviewRepository)
        {
            _personRepository = personRepository;
            _personGroupRepository = personGroupRepository;
            _interviewRepository = interviewRepository;
        }

        public static void CheckDialerErrorCode(DialerErrorCode errorCode)
        {
            if (errorCode == DialerErrorCode.Success)
                return;

            throw new Exception($"Unexpected dialer error code {errorCode}");
        }

        public ConsoleTransferState GetTransferState(
            TransferState transferState,
            BvActiveDialEntity dial)
        {
            if (dial == null) return null;
            if (transferState == null) return null;

            var interview = _interviewRepository.GetByIdWithCheck(dial.SurveyId, dial.InterviewId);
            var initiator = _personRepository.TryGetById(transferState.InitiatorAgentId);

            return new ConsoleTransferState
            {
                ConnectionState = GetConsoleConnectionState(transferState.ConnectionState),
                Initiator = new TransferParticipant
                {
                    ParticipantType = ParticipantType.Agent,
                    DialingState = GetInitiatorDialingState(transferState.ConnectionState, dial.TransferState),
                    DialingStateOutcome = GetDialingStateOutcome(transferState.ConnectionState),
                    Resource = GetInitiatorName(initiator, dial.TransferState)
                },
                Target = new TransferParticipant
                {
                    ParticipantType = GetTargetType(transferState.TargetType, dial.TransferState),
                    DialingState = GetTargetDialingState(transferState.TargetState, transferState.ConnectionState, dial.TransferState),
                    DialingStateOutcome = GetTargetDialingStateOutcome(transferState.TargetOutcome),
                    Resource = GetTargetName(transferState, dial.TransferState)
                },
                Respondent = new TransferParticipant
                {
                    ParticipantType = ParticipantType.External,
                    DialingState = GetRespondentDialingState(transferState.InitiatorState, transferState.ConnectionState, dial.TransferState),
                    DialingStateOutcome = GetDialingStateOutcome(transferState.ConnectionState),
                    Resource = GetRespondentName(interview, dial.TransferState)
                }
            };
        }

        private ConsoleConnectionState GetConsoleConnectionState(ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case ConnectionState.InitiatorToRespondent:
                    return ConsoleConnectionState.InitiatorToRespondent;
                case ConnectionState.InitiatorToTarget:
                    return ConsoleConnectionState.InitiatorToTarget;
                case ConnectionState.Conference:
                    return ConsoleConnectionState.Conference;
                case ConnectionState.TargetToRespondent:
                    return ConsoleConnectionState.TargetToRespondent;
                case ConnectionState.NotDefined:
                    return ConsoleConnectionState.NotDefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionState), connectionState, null);
            }
        }

        #region Initiator
        private static DialingState GetInitiatorDialingState(ConnectionState connectionState,
            ConsoleTransferState prevState)
        {
            var prevValue = prevState?.Initiator?.DialingState ?? DialingState.NotDefined;
            switch (connectionState)
            {
                case ConnectionState.InitiatorToRespondent:
                case ConnectionState.InitiatorToTarget:
                case ConnectionState.Conference:
                    return DialingState.Connected;
                case ConnectionState.TargetToRespondent:
                    return DialingState.Hold;
                case ConnectionState.NotDefined:
                    return prevValue != DialingState.NotDefined ? prevValue : DialingState.NotDefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionState), connectionState, null);
            }
        }

        private static string GetInitiatorName(BvPersonEntity initiator, ConsoleTransferState prevState)
        {
            return GetNullifiedValue(initiator?.Name) ?? prevState?.Initiator?.Resource ?? "";
        }
        #endregion

        #region Target
        private static ParticipantType GetTargetType(TargetType targetType, ConsoleTransferState prevState)
        {
            var prevValue = prevState?.Target?.ParticipantType ?? ParticipantType.NotDefined;
            switch (targetType)
            {
                case TargetType.External:
                    return ParticipantType.External;
                case TargetType.Agent:
                    return ParticipantType.Agent;
                case TargetType.AgentGroup:
                    return ParticipantType.AgentGroup;
                case TargetType.NotDefined:
                    return prevValue != ParticipantType.NotDefined ? prevValue : ParticipantType.NotDefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }

        private static DialingState GetTargetDialingState(TargetState targetState, ConnectionState connectionState, ConsoleTransferState prevState)
        {
            var prevValue = prevState?.Target?.DialingState ?? DialingState.NotDefined;
            switch (targetState)
            {
                case TargetState.Dialing:
                    return DialingState.Dialing;
                case TargetState.Connected:
                    return connectionState == ConnectionState.InitiatorToRespondent ? DialingState.Hold : DialingState.Connected;
                case TargetState.NotConnected:
                    return DialingState.NotConnected;
                case TargetState.WaitingForAgent:
                    return DialingState.Waiting;
                case TargetState.NotDefined:
                    return prevValue != DialingState.NotDefined ? prevValue : DialingState.NotDefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetState), targetState, null);
            }
        }

        private string GetTargetName(TransferState dialTransferState, ConsoleTransferState prevState)
        {
            switch (dialTransferState.TargetType)
            {
                case TargetType.Agent:
                    return _personRepository.TryGetById(ParseInt(dialTransferState.TargetResource))?.Name ?? dialTransferState.TargetResource;
                case TargetType.AgentGroup:
                    return _personGroupRepository.TryGetById(ParseInt(dialTransferState.TargetResource))?.Name ?? dialTransferState.TargetResource;
                case TargetType.External:
                    return dialTransferState.TargetResource;
                default:
                    return GetNullifiedValue(dialTransferState.TargetResource) ?? prevState?.Target?.Resource ?? "";

            }
        }

        public static int ParseInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }
        #endregion

        #region Respondent
        private static DialingState GetRespondentDialingState(InitiatorState initiatorState, ConnectionState connectionState, ConsoleTransferState prevState)
        {
            DialingState prevValue = prevState?.Respondent?.DialingState ?? DialingState.NotDefined;
            switch (initiatorState)
            {
                case InitiatorState.Connected:
                    return connectionState == ConnectionState.InitiatorToTarget ? DialingState.Hold : DialingState.Connected;
                case InitiatorState.NotConnected:
                    return DialingState.NotConnected;
                case InitiatorState.NotDefined:
                    return prevValue != DialingState.NotDefined ? prevValue : DialingState.NotDefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(initiatorState), initiatorState, null);
            }
        }

        private static DialingStateOutcome GetTargetDialingStateOutcome(TargetOutcome targetOutcome)
        {
            switch (targetOutcome)
            {
                case TargetOutcome.NotDefined:
                    return DialingStateOutcome.NotDefined;
                case TargetOutcome.Connected:
                    return DialingStateOutcome.Connected;
                case TargetOutcome.Busy:
                    return DialingStateOutcome.Busy;
                case TargetOutcome.NoReply:
                    return DialingStateOutcome.NoReply;
                default:
                    return DialingStateOutcome.NotDefined;
            }
        }

        private static string GetRespondentName(BvInterviewWithOriginEntity interview, ConsoleTransferState prevState)
        {
            return GetNullifiedValue(interview.RespondentName) ?? GetNullifiedValue(interview.TelephoneNumber) ?? prevState?.Respondent?.Resource ?? "";
        }
        #endregion

        private static DialingStateOutcome GetDialingStateOutcome(ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case ConnectionState.InitiatorToRespondent:
                case ConnectionState.InitiatorToTarget:
                case ConnectionState.Conference:
                case ConnectionState.TargetToRespondent:
                    return DialingStateOutcome.Connected;
                case ConnectionState.NotDefined:
                    return DialingStateOutcome.NotDefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionState), connectionState, null);
            }
        }

        private static string GetNullifiedValue(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
