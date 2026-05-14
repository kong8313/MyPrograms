using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;
using TransferType = ConfirmitDialerInterface.TransferType;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleTransferCompleteProcessor : IConsoleTransferCompleteProcessor
    {
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;

        public ConsoleTransferCompleteProcessor(
            IActiveDialRepository activeDialRepository,
            IActiveDialService activeDialService)
        {
            _activeDialRepository = activeDialRepository;
            _activeDialService = activeDialService;
        }

        public void TransferComplete(BvTasksEntity task, BvPersonEntity person, TransferCompleteEvent activityEvent)
        {
            activityEvent.UpdateEventPropertiesFromTask(task);

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            if (dial == null || dial.DialState != DialState.Transfering)
            {
                throw new Exception("There is no active transfer operation");
            }

            if (dial.MainPersonId != task.PersonSID)
            {
                throw new Exception("You are not allowed to control a transfer operation, because you are not owner of it");
            }

            TransferService.CheckDialerErrorCode(_activeDialService.TransferComplete(dial, task));

            switch (dial.DialTransferType)
            {
                case TransferType.ExternalCold:
                case TransferType.ExternalWarm:
                    task.InterviewState = (byte)InterviewState.INTERVIEWING;
                    break;
                case TransferType.InternalCold:
                case TransferType.InternalWarm:
                    ServiceLocator.Resolve<ICallQueueService>().ForceCallDelivery();
                    var wrapUpEvent = new WrapUpEvent();
                    ServiceLocator.Resolve<IConsoleWrapUpProcessor>().WrapUp(person, task, task.InterviewID, true, 1, 
                        new CompletedInterviewDetails(){Its  = ((int)CallOutcome.InternalTransfer).ToString(), Status = null}, 
                        WrapUpReason.TransferInterview, wrapUpEvent);
                    wrapUpEvent.Save();
                    break;
                default:
                    throw new Exception("Unknown Transfer type");
            }

            activityEvent.Details.TransferId = dial.TransferId;
        }
    }
}