using System;
using System.Diagnostics;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.NotificationHandlers
{
    public class DialerNotifyCallDroppedByRespondentHandler : IDialerNotifyCallDroppedByRespondentHandler
    {
        private readonly Lazy<ICompanyInfo> _companyInfo;
        private readonly Lazy<ICallQueueService> _callQueueService;
        private readonly Lazy<IInterviewRepository> _interviewRepository;
        private readonly Lazy<IActiveDialRepository> _activeDialRepository;
        private readonly Lazy<IPersonRepository> _personRepository;
        private readonly Lazy<IConsoleWrapUpProcessor> _consoleWrapUpProcessor;
        private readonly Lazy<IActiveDialService> _activeDialService;
        private readonly Lazy<ITaskRepository> _taskRepository;
        private readonly Lazy<IInterviewerApiClient> _interviewerApiClient;

        public DialerNotifyCallDroppedByRespondentHandler()
        {
            _companyInfo = new Lazy<ICompanyInfo>(() => ServiceLocator.Resolve<ICompanyInfo>());
            _callQueueService = new Lazy<ICallQueueService>(() => ServiceLocator.Resolve<ICallQueueService>());
            _interviewRepository = new Lazy<IInterviewRepository>(() => ServiceLocator.Resolve<IInterviewRepository>());
            _activeDialRepository = new Lazy<IActiveDialRepository>(() => ServiceLocator.Resolve<IActiveDialRepository>());
            _personRepository = new Lazy<IPersonRepository>(() => ServiceLocator.Resolve<IPersonRepository>());
            _consoleWrapUpProcessor = new Lazy<IConsoleWrapUpProcessor>(() => ServiceLocator.Resolve<IConsoleWrapUpProcessor>());
            _activeDialService = new Lazy<IActiveDialService>(() => ServiceLocator.Resolve<IActiveDialService>());
            _taskRepository = new Lazy<ITaskRepository>(() => ServiceLocator.Resolve<ITaskRepository>());
            _interviewerApiClient = new Lazy<IInterviewerApiClient>(() => ServiceLocator.Resolve<IInterviewerApiClient>());
        }

        public void Execute(int dialerId,
            string companyId,
            long campaignId,
            long agentId,
            long callId)
        {
            var evt = new OnDialerNotifyCallDroppedByRespondentEvent(companyId, campaignId, agentId, callId);

            BvTasksEntity task = _taskRepository.Value.GetByPerson((int)agentId);

            if (task?.IsWebConsole == true)
            {
                _interviewerApiClient.Value.NotifyCallDroppedByRespondent(_companyInfo.Value.CompanyId, dialerId, campaignId,
                    (int) agentId, callId);
                evt.AddTiming("InterviewerApiClient.NotifyCallDroppedByRespondent");
                evt.Save();
                return;
            }
            
            var activeDial = _activeDialRepository.Value.TryGetByCallId((int)callId);
            evt.AddTiming("ActiveDialRepository.TryGetByCallId");
            
            using (TaskLocker.TryLock((int)agentId, out task))
            {
                _activeDialService.Value.OnDialNotifyOutcome(activeDial, task, CallOutcome.DroppedByRespondent);
                evt.AddTiming("ActiveDialService.OnDialNotifyOutcome");

                // if task is null is means that we need to drop a transferred call
                if (task == null)
                {
                    DropTransferedCall(callId, agentId, evt);

                    return;
                }
            }

            var person = _personRepository.Value.GetById((int)agentId);
            evt.AddTiming("PersonRepository.GetById");

            // need to drop a call during ivr process
            if (person.Type == (byte)AgentType.IvrAgent)
            {
                DropCallForIvrAgent(person, task, activeDial, callId, evt);
            }
            else if (person.Type == (byte)AgentType.LiveAgent)
            {
                DropCallForLiveAgent(agentId, callId, evt);
            }
        }

        private void DropTransferedCall(long callId, long agentId, OnDialerNotifyCallDroppedByRespondentEvent evt)
        {
            evt.Details.IsTransferedCall = true;

            var unlockedCall = CallQueueService.GetCallInfo(callId);
            evt.AddTiming("CallQueueService.GetCallInfo");

            if (unlockedCall == null)
            {
                Trace.TraceError("Unknown call id '{0}'", callId);
                return;
            }

            evt.AddTiming("CallQueueService.GetCallWithTryLock");
            var call = _callQueueService.Value.GetCallWithTryLock(
                unlockedCall.SurveySID,
                unlockedCall.InterviewID,
                out _);

            if ((CallState?)call?.CallState == CallState.InterviewInProgress || call == null)
            {
                // Ignore the drop in this case. In future we will have active dial and be able to log this information here
                //SaveCallHistoryAndActivityEvent(activeDial, InboundHandlerOperationType.Skipped, inboundCallDroppedNotifyEvent);
                return;
            }

            var interview = _interviewRepository.Value.GetById(call.SurveySID, call.InterviewID);
            evt.AddTiming("InterviewRepository.GetById");

            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.NotConnected,
                IsExecuteSchedulingScript = true,
                ITS = (int)CallOutcome.DroppedByRespondent,
                LastCallPersonSID = (int)agentId,
                CallProvider = new CallMemoryProvider(call)
            };

            InterviewRepository.Update(interview, options);
            evt.AddTiming("InterviewRepository.Update");

            evt.Save(call.InterviewID);
        }

        private void DropCallForLiveAgent(
            long agentId,
            long callId,
            OnDialerNotifyCallDroppedByRespondentEvent evt)
        {
            evt.Details.AgentType = AgentType.LiveAgent;

            using (var locker = TaskLocker.TryLock((int)agentId, out var task))
            {

                if (task == null)
                {
                    Trace.TraceError("DialerNotifyCallDroppedByRespondentHandler.DropCallForLiveAgent:task for person with id={0}" +
                                     "was deleted after lock. It is incorrect.", agentId);
                    return;
                }

                if (task.CallID != callId)
                {
                    Trace.TraceWarning($"Unexpected situation: dialer tries to drop a wrong call for live agent.\r\nCurrent InterviewId={task.InterviewID}\r\nCurrent CallId={task.CallID}\r\nCallId from dialer={callId}");
                    return;
                }

                task.CallConnectionState = (byte)CallConnectionState.Disconnected;

                evt.Details.Task = task;

                _taskRepository.Value.Update(task);
                evt.AddTiming("TaskRepository.Update");

                evt.Save(task.InterviewID);
            }
        }

        private void DropCallForIvrAgent(
            BvPersonEntity person,
            BvTasksEntity task,
            BvActiveDialEntity activeDial,
            long callId,
            OnDialerNotifyCallDroppedByRespondentEvent evt)
        {
            if (task.CallID != callId)
            {
                Trace.TraceWarning($"Unexpected situation: dialer tries to drop a wrong call for IVR agent.\r\nCurrent InterviewId={task.InterviewID}\r\nCurrent CallId={task.CallID}\r\nCallId from dialer={callId}");
                return;
            }

            evt.Details.AgentType = AgentType.IvrAgent;
            evt.Details.Task = task;

            var wrapUpEvent = new WrapUpEvent();
            var details = new CompletedInterviewDetails
            {
                InterviewDuration = 0,
                Its = ((int)CallOutcome.DroppedByRespondent).ToString(),
                Status = null
            };
            wrapUpEvent.Details.InterviewDetails = details;

            _consoleWrapUpProcessor.Value.WrapUp(person, task, task.InterviewID, true, 1, details, 
                WrapUpReason.CompeteInterview, wrapUpEvent, activeDial);
            evt.AddTiming("ConsoleWrapUpProcessor.WrapUp");

            evt.Save(task.InterviewID);
        }
    }
}