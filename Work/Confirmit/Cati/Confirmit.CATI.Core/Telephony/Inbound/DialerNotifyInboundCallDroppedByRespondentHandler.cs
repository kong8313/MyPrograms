using System.Diagnostics;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public class DialerNotifyInboundCallDroppedByRespondentHandler : IDialerNotifyInboundCallDroppedByRespondentHandler
    {
        private readonly InboundCallService _inboundCallService;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly ICallQueueService _callQueueService;

        public DialerNotifyInboundCallDroppedByRespondentHandler(
            InboundCallService inboundCallService,
            IDatabaseLockTimeouts databaseLockTimeouts,
            IActiveDialRepository activeDialRepository,
            IInterviewRepository interviewRepository,
            ICallQueueService callQueueService)
        {
            _inboundCallService = inboundCallService;
            _databaseLockTimeouts = databaseLockTimeouts;
            _activeDialRepository = activeDialRepository;
            _interviewRepository = interviewRepository;
            _callQueueService = callQueueService;
        }

        public void Execute(int dialerId, int companyId, string inboundCallId)
        {
            var inboundCallDroppedNotifyEvent =
                new OnInboundCallDroppedNotifyEvent
                {
                    Details =
                    {
                        InboundCallId = inboundCallId,
                        DialerId = dialerId
                    },
                    CompanyId = companyId
                };

            var activeDial = _activeDialRepository.TryGetByInboundCallId(inboundCallId);
            if (activeDial == null)
            {
                Trace.TraceWarning($"No previous inbound call history for inboundCallId={inboundCallId}, skip processing");
                inboundCallDroppedNotifyEvent.Save(InboundHandlerOperationType.Skipped);
                return;
            }

            inboundCallDroppedNotifyEvent.Details.InboundLinePhoneNumber = activeDial.DialerTelephoneNumber;
            inboundCallDroppedNotifyEvent.Details.CallerPhoneNumber = activeDial.RespondentTelephoneNumber;
            inboundCallDroppedNotifyEvent.Details.InterviewId = activeDial.InterviewId;
            inboundCallDroppedNotifyEvent.ObjectId = activeDial.SurveyId;

            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetInboundCallName(inboundCallId),
                "DialerNotifyInboundCallDroppedByRespondentHandler.Execute",
                _databaseLockTimeouts.DefaultLockTimeoutInMs,
                true))
            {
                dbLock.EnterLock();

                if (activeDial.SurveyId == 0 || activeDial.InterviewId == 0)
                {
                    Trace.TraceWarning(
                        $"SurveyId={activeDial.SurveyId} or InterviewId={activeDial.InterviewId} is zero, skip processing");
                    inboundCallDroppedNotifyEvent.Save(InboundHandlerOperationType.Skipped);
                    return;
                }

                var call = _callQueueService.GetCallWithTryLock(
                    activeDial.SurveyId,
                    activeDial.InterviewId,
                    out _);

                if ((CallState?) call?.CallState == CallState.InterviewInProgress)
                {
                    // Ignore the drop in this case
                    SaveCallHistoryAndActivityEvent(activeDial, InboundHandlerOperationType.Skipped, inboundCallDroppedNotifyEvent);
                    return;
                }

                var interview = _interviewRepository.GetById(activeDial.SurveyId, activeDial.InterviewId);

                var options = new SchedulingScriptExecutionOptions
                {
                    ExecutionReason = SchedulingScriptExecutionReason.NotConnected,
                    opType = OperationType.DroppedByRespondent,
                    ITS = (int)CallOutcome.DroppedByRespondent,
                    CallProvider = new CallMemoryProvider(call)
                };

                InterviewRepository.Update(interview, options);

                _activeDialRepository.Delete(activeDial.Id, CallCompleteStatus.DropByRespondent);

                SaveCallHistoryAndActivityEvent(activeDial, InboundHandlerOperationType.DropByRespondent, inboundCallDroppedNotifyEvent);
            }
        }

        private void SaveCallHistoryAndActivityEvent(
            BvActiveDialEntity activeDial,
            InboundHandlerOperationType operationType,
            OnInboundCallDroppedNotifyEvent inboundCallNotifyEvent)
        {
            _inboundCallService.CreateCallHistory(activeDial, operationType);

            inboundCallNotifyEvent.Save(operationType);
        }
    }
}