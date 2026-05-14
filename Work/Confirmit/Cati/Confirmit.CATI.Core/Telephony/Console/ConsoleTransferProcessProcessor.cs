using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony.Dial;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleTransferProcessProcessor : IConsoleTransferProcessProcessor
    {
        private readonly IActiveDialService _activeDialService;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly ITaskRepository _taskRepository;

        public ConsoleTransferProcessProcessor(
            IActiveDialService activeDialService,
            IActiveDialRepository activeDialRepository,
            ITaskRepository taskRepository)
        {
            _activeDialService = activeDialService;
            _activeDialRepository = activeDialRepository;
            _taskRepository = taskRepository;
        }

        public bool ShouldProcessTransfer(BvTasksEntity task)
        {
            if (task.InterviewState != (byte)InterviewState.INCOMING_TRANSFER)
                return false;

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            if (ShouldConfirmTransfer(task, dial) || ShouldWrapUp(task, dial))
                return true;

            return false;
        }

        private static bool ShouldWrapUp(BvTasksEntity task, BvActiveDialEntity dial)
        {
            return dial == null || 
                   dial.DialState != DialState.Transfering || 
                   dial.TransferId != task.Context.TransferId;
        }

        private static bool ShouldConfirmTransfer(BvTasksEntity task, BvActiveDialEntity dial)
        {
            return dial != null && ActiveDialService.IsTransferReadyToComplete(dial, task);
        }

        public void ProcessTransfer(BvPersonEntity person)
        {
            using (TaskLocker.Lock(person, out var task))
            {
                if (task.InterviewState != (byte)InterviewState.INCOMING_TRANSFER)
                    return;

                var dial = _activeDialRepository.TryGetByCallId(task.CallID);

                if (ShouldWrapUp(task, dial))
                {
                    var activityEvent = new WrapUpEvent();
                    ServiceLocator.Resolve<IConsoleWrapUpProcessor>().WrapUp(person, task, task.InterviewID, true, 1,
                        new CompletedInterviewDetails() { Its = ((int)CallOutcome.CanceledTransfer).ToString() }, WrapUpReason.CancelTransfering, activityEvent);
                    activityEvent.Save();
                }

                if (ShouldConfirmTransfer(task, dial))
                {
                    _activeDialService.TransferConfirm(dial, person);
                    

                    task.InterviewState = (byte)InterviewState.INTERVIEWING;
                    _taskRepository.Update(task);

                    ServiceLocator.Resolve<IIvrConsoleService>().ProcessCallOnConnect(task);
                }
            }
        }

    }
}