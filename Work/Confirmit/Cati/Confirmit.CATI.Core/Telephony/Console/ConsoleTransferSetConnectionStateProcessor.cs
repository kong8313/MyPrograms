using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleTransferSetConnectionStateProcessor : IConsoleTransferSetConnectionStateProcessor
    {
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;

        public ConsoleTransferSetConnectionStateProcessor(
            IActiveDialRepository activeDialRepository,
            IActiveDialService activeDialService)
        {
            _activeDialRepository = activeDialRepository;
            _activeDialService = activeDialService;
        }
        public void TransferSetConnectionState(BvTasksEntity task, BvPersonEntity person,
            TransferConnectionState connectionState, TransferSetConnectionStateEvent activityEvent)
        {
            activityEvent.UpdateEventPropertiesFromTask(task);

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            if (dial == null || dial.DialState != DialState.Transfering)
            {
                throw new Exception("There is no active transfer operation");
            }

            if (dial.MainPersonId != 0 && dial.MainPersonId != task.PersonSID)
            {
                throw new Exception("You are not allowed to control a transfer operation, because you are not owner of it");
            }

            var dialerConnectionState = ConvertToDialerConnectionState(connectionState);

            TransferService.CheckDialerErrorCode(_activeDialService.TransferSetConnectionState(dial, dialerConnectionState));

            activityEvent.Details.TransferId = dial.TransferId;
        }

        private ConnectionState ConvertToDialerConnectionState(TransferConnectionState connectionState)
        {
            switch (connectionState)
            {
                case TransferConnectionState.RespondentHold:
                    return ConnectionState.InitiatorToTarget;
                case TransferConnectionState.Conference:
                    return ConnectionState.Conference;
                case TransferConnectionState.TargetHold:
                    return ConnectionState.InitiatorToRespondent;
                default:
                    throw new Exception($"Unknown TransferConnectionState : {connectionState}");
            }
        }
    }
}