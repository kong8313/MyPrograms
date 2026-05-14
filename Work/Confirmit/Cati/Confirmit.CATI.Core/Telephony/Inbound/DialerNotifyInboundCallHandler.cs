using System;
using System.Diagnostics;
using System.Threading;
using BvDotNetEngine.Events;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public class DialerNotifyInboundCallHandler : IDialerNotifyInboundCallHandler
    {
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly IInboundCallService _inboundCallService;
        private readonly IActiveDialService _activeDialService;
        private readonly ICallQueueService _callQueueService;

        public DialerNotifyInboundCallHandler(
            IDatabaseLockTimeouts databaseLockTimeouts,
            IInboundCallService inboundCallService,
            IActiveDialService activeDialService,
            ICallQueueService callQueueService) 
        {
            _databaseLockTimeouts = databaseLockTimeouts;
            _inboundCallService = inboundCallService;
            _activeDialService = activeDialService;
            _callQueueService = callQueueService;
        }

        public void Execute(int dialerId, int companyId, string ddiNumber, string cliNumber,
            string inboundCallId)
        {
            var inboundCallNotifyEvent = new OnInboundCallNotifyEvent
            {
                CompanyId = companyId,
                Details =
                {
                    DialerId = dialerId,
                    InboundLinePhoneNumber = ddiNumber,
                    CallerPhoneNumber = cliNumber,
                    InboundCallId = inboundCallId
                }
            };

            var activeDial = _activeDialService.CreateInboundCall(dialerId, inboundCallId, ddiNumber, cliNumber);

            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetInboundCallName(inboundCallId),
                "DialerNotifyInboundCallHandler.Execute",
                _databaseLockTimeouts.DefaultLockTimeoutInMs,
                true))
            {
                dbLock.EnterLock();

                InterviewWithCall searchResult = new InterviewWithCall();
                InboundHandlerOperationType operationType = InboundHandlerOperationType.DropBySystemInternalServerError;

                try
                {
                    _inboundCallService.CheckAndSearchInterview(
                        ddiNumber,
                        cliNumber,
                        searchResult);

                    if (searchResult.Call != null && !searchResult.IsCallLockAcquired)
                    {
                        throw new InboundCallCantProceedException(
                            string.Format("Call lock isn't acquired [{0}]", searchResult.Call.CallState),
                            DropInboundCallReason.CallLockIsNotAcquired);
                    }

                    var options = new SchedulingScriptExecutionOptions
                    {
                        ExecutionReason = SchedulingScriptExecutionReason.Inbound,
                        ITS = (int) CallOutcome.InboundCall,
                        CallProvider = new CallMemoryProvider(searchResult.Call),
                        PostSchedulingAction = evt => DoPostSchedulingAction(evt),
                        CliNumber = cliNumber,
                        DdiNumber = ddiNumber,
                        opType = OperationType.InboundCall
                    };

                    InterviewRepository.Update(searchResult.Interview, options);

                    // TODO: need to prevent this additional database call for performance reasons
                    var call = CallQueueService.GetCallAndNoLock(searchResult.Interview.SurveySID,
                        searchResult.Interview.ID);

                    if (call != null && call.Type == (byte) CallTypes.Inbound)
                    {
                        if (call.CallState == (int) CallState.InterviewInProgress ||
                            call.CallState == (int) CallState.LoadedToDialerPredictively )
                        {
                            operationType = _activeDialService.AcceptInboundCall(activeDial, searchResult.Survey, searchResult.Interview, call);

                            if (call.CallState == (int) CallState.InterviewInProgress)
                            {
                                BvSpSetCallStateAdapter.ExecuteNonQuery( call.SurveySID, call.InterviewID, (int)CallState.Scheduled);
                            }

                            _callQueueService.ForceCallDelivery(call);
                        }
                        else
                        {
                            throw new InboundCallCantProceedException(
                                string.Format("Unexpected call state [{0}]", call.CallState),
                                DropInboundCallReason.UnexpectedCallState);
                        }
                    }
                    else
                    {
                        throw new InboundCallCantProceedException(
                            string.Format("Not accepted by scheduling script /// callType=[{0}]",
                                (call == null) ? "call object is (null)" : ((CallTypes) call.Type).ToString()),
                            DropInboundCallReason.NotAcceptedBySchedulingScript);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());

                    var dropReason = (ex is InboundCallCantProceedException inboundCallCantProceedException)
                        ? inboundCallCantProceedException.DropInboundCallReason
                        : DropInboundCallReason.InternalServerError;

                    activeDial.SurveyId = searchResult.Survey?.SID ?? 0;
                    activeDial.CampaignId = searchResult.Survey?.CampaignId ?? 0;
                    activeDial.InterviewId = searchResult.InterviewId ?? 0;
                    activeDial.CallId = searchResult.CallId ?? 0;

                    if (searchResult.Interview != null && dropReason == DropInboundCallReason.NoAgentsAvailable)
                    {
                        var options = new SchedulingScriptExecutionOptions
                        {
                            ExecutionReason = SchedulingScriptExecutionReason.NotConnected,
                            ITS = (int)CallOutcome.InterruptedBySystem,
                            CliNumber = cliNumber,
                            DdiNumber = ddiNumber,
                            opType = OperationType.NotConnectedCall
                        };

                        InterviewRepository.Update(searchResult.Interview, options);
                    }

                    operationType = _activeDialService.DropInboundCall(activeDial, dropReason);
                }


                inboundCallNotifyEvent.Details.InterviewId = searchResult.InterviewId ?? 0;
                inboundCallNotifyEvent.ObjectId = searchResult.SurveyId ?? 0;
                inboundCallNotifyEvent.Save(operationType);
            }
        }

        private void DoPostSchedulingAction(EventSchedule evt)
        {
            if (evt.NewCall != null && evt.NewCall.Type == (byte)CallTypes.Inbound && evt.NewCall.CallState == (int)CallState.Scheduled)
            {
                evt.NewCall.CallState = evt.Survey.DialingMode == DialingMode.Predictive
                    ? (int)CallState.LoadedToDialerPredictively
                    : (int)CallState.InterviewInProgress;
            }
        }

        
    }
}
