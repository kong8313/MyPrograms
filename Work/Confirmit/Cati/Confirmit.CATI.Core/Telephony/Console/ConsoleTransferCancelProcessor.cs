using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleTransferCancelProcessor : IConsoleTransferCancelProcessor
    {
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;
        private readonly ICallQueueService _callQueueService;

        public ConsoleTransferCancelProcessor(
            IActiveDialRepository activeDialRepository,
            IActiveDialService activeDialService,
            ICallQueueService callQueueService)
        {
            _activeDialRepository = activeDialRepository;
            _activeDialService = activeDialService;
            _callQueueService = callQueueService;
        }
        public void TransferCancel(BvTasksEntity task, BvPersonEntity person, TransferCancelEvent activityEvent, BvActiveDialEntity activeDial = null)
        {
            activityEvent.UpdateEventPropertiesFromTask(task);

            if (task.InterviewState != (int)InterviewState.OUTGOING_TRANSFER)
                return;

            var dial = activeDial ?? _activeDialRepository.TryGetByCallId(task.CallID);

            if (dial == null || dial.DialState != DialState.Transfering)
            {
                task.InterviewState = (int)InterviewState.INTERVIEWING;
                return;
            }

            if (dial.MainPersonId != 0 && dial.MainPersonId != task.PersonSID)
            {
                throw new Exception($"Transfer cancel failed because interviewer {task.PersonSID} attempted to cancel a transfer owned by interviewer {dial.MainPersonId}.");
            }

            if (dial.DialTransferType == TransferType.InternalCold ||
                dial.DialTransferType == TransferType.InternalWarm)
            {
                _callQueueService.GetCallWithTryLockAny(task.SurveySID, task.InterviewID, out _);
            }

            TransferService.CheckDialerErrorCode(_activeDialService.TransferCancel(dial));

            task.InterviewState = (int)InterviewState.INTERVIEWING;

            activityEvent.Details.TransferId = dial.TransferId;
        }
    }
}