using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver.Controllers
{
    public class InterviewerPredictiveController : BaseInterviewerController
    {

        private ManualResetEvent readyEvent = new ManualResetEvent(false);
        private ManualResetEvent stopEvent = new ManualResetEvent(false);
        private Thread _thread;

        private InterviewerPredictiveController(
            ISimulator simulator,
            Dialer dialer,
            Interviewer interviewer)
            :base(simulator, dialer, interviewer)
        {
        }

        public static InterviewerPredictiveController Create(ISimulator _simulator, Dialer dialer, Interviewer interviewer)
        {
            var controller = new InterviewerPredictiveController(_simulator, dialer, interviewer);

            SimulatorDialerDriverPerformanceCounters.NumberOfNotReadyInterviewers.Increment();

            controller._thread = new Thread(controller.CallOutcomeGeneratorThread);
            controller._thread.Start();

            return controller;
        }

        public override bool SetReady(bool isReady)
        {
            var currentReadyState = readyEvent.WaitOne(0);

            if (isReady == currentReadyState)
                return true;

            if (isReady)
            {
                readyEvent.Set();

                SimulatorDialerDriverPerformanceCounters.NumberOfReadyInterviewers.Increment();
                SimulatorDialerDriverPerformanceCounters.NumberOfNotReadyInterviewers.Decrement(); 
            }
            else
            {
                readyEvent.Reset();

                SimulatorDialerDriverPerformanceCounters.NumberOfReadyInterviewers.Decrement();
                SimulatorDialerDriverPerformanceCounters.NumberOfNotReadyInterviewers.Increment();
            }

            return true;
        }

        public override void SetGroups(int[] groups)
        {
            Interviewer.Groups = groups.ToArray();
        }

        public override void SetCampaignId(long campaignId)
        {
            if (IsReady())
            {
                throw new DialerException(DialerErrorCode.WrongAgentState,
                    "Agent should be in NotReady state to perform survey switching.");
            }

            var campaign = _dialer.CampaignsManager.Get(campaignId);
            if (campaign.DialingMode != DialingMode.Predictive)
            {
                throw new DialerException(DialerErrorCode.WrongState,
                    string.Format("'Old' campaign [{0}] is not a predictive one. The campaign dialing mode is [{1}]", campaignId, campaign.DialingMode));
            }

            Interviewer.CampaignId = campaignId;
        }

        public bool IsReady()
        {
            return readyEvent.WaitOne(0);
        }

        public override void Destroy()
        {
            StopCallOutcomeGeneratorThread();

            var isInterviewerReady = readyEvent.WaitOne(0);
                
            if (isInterviewerReady)
            {
                SimulatorDialerDriverPerformanceCounters.NumberOfReadyInterviewers.Decrement();
            }
            else
            {
                SimulatorDialerDriverPerformanceCounters.NumberOfNotReadyInterviewers.Decrement();
            }

        }

        public override void CompletePreview(int companyId, int dialerId)
        {
            Dial(companyId, dialerId, ActiveCall);
            if (ActiveCall != null)
            {
                _dialer.Transfers.OnCallConnected(ActiveCall, this);
            }

        }

        private void StopCallOutcomeGeneratorThread()
        {
            _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController",
                "Stopping CallOutcomeGeneratorThread [interviewer-{0}] ...", Interviewer.DisplayName);

            stopEvent.Set();

            _thread.Join();

            _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController",
                "CallOutcomeGeneratorThread is stopped [interviewer-{0}]", Interviewer.DisplayName);
        }

        public override void SendNumberToAgent(int companyId, int dialerId, CallManager.CallInfoEx call)
        {
            throw new DialerException(DialerErrorCode.NotSupported,
                $"Predicitve agent {Interviewer.AgentId} doesn't support SendNumberToAgent method.");
        }

        public override void ConnectInboundCallToAgent(int companyId, int dialerId, CallManager.CallInfoEx call)
        {
            throw new DialerException(DialerErrorCode.NotSupported,
                $"Predicitve agent {Interviewer.AgentId} doesn't support ConnectInboundCallToAgent method.");
        }

        private void CallOutcomeGeneratorThread()
        {
            const int stopEventIndex = 0;
            const int readyEventIndex = 1;

            var events = new WaitHandle[] { stopEvent, readyEvent };

            while (true)
            {
                var index = WaitHandle.WaitAny(events);

                if (index == stopEventIndex)
                {
                    _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController.CallOutcomeGeneratorThread",
                        "stopEvent is signalled interviewer-{0}. Breaking the loop.", Interviewer.DisplayName);
                    break;
                }

                if (index != readyEventIndex)
                {
                    // Should never get here. Let's log an error and exit in this case.
                    _simulator.Logger.Error("SimulatorDialerDriver.InterviewerPredictiveController.CallOutcomeGeneratorThread",
                        "Wrong index [{0}] is received from WaitHandle.WaitAny() [interviewer-{1}]. Breaking the loop.",
                        index, Interviewer.DisplayName);
                    break;
                }

                var campaignControllerPredictive = _dialer.CampaignsManager.GetPredictive(Interviewer.CampaignId);

                var call = _dialer.GlobalCallManager.GetCallWithRemove(Interviewer);
                
                if (call == null)
                {
                    // There are no inbound calls - look for outbound calls
                    call = campaignControllerPredictive.CallManager.GetCallWithRemove(Interviewer);

                    if (call == null)
                    {
                        continue;
                    }
                }

                var callOutcome = _simulator.CallOutcomeDistributor.GetNextCallOutcomeDistributionData(call.Info.phoneNumber, call.Type);

                var processingTime = Generators.CallOutcomeDelay.GetValue(new ContextInfo(Interviewer, call.Info.interviewId), callOutcome.ProcessingTime);
                var outcome = Generators.CallOutcomeValue.GetValue(new ContextInfo(Interviewer, call.Info.interviewId), callOutcome.CallOutcome);

                _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController",
                    "wait processing time-{0} interviewer-{1} interviewId-{2} callOutcome-{3}",
                    processingTime, Interviewer.DisplayName, call.Info.interviewId, outcome);

                Thread.Sleep(processingTime); // 0 (zero) waiting time for inbound calls

                if (( call.Type == CallManager.CallType.Inbound || call.Type == CallManager.CallType.Transfer) && outcome == CallOutcome.DroppedByRespondent)
                {
                    _simulator.DialerEvents.NotifyCallDroppedByRespondent(
                        Interviewer.CompanyId,
                        Interviewer.DialerId,
                        call.CampaignId,
                        0,//Interviewer.AgentId,
                        call.Info.callId);
                    continue;
                }

                if (call.Info.diallingMode == DialingMode.Preview || call.Info.diallingMode == DialingMode.SpecialDial || call.Type == CallManager.CallType.Transfer)
                {
                    ActiveCall = call;
                    if (!SetReady(false))
                    {
                        //we lost call here, may be we should send outcome here
                        break;
                    }

                    _simulator.DialerEvents.ScreenPop(Interviewer.CompanyId, Interviewer.DialerId, call.CampaignId, Interviewer.AgentId,
                        call.Info.interviewId, call.Info.callId, call.Info.diallingMode);

                    _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController",
                        "finish generate ScreenPop interviewer-{0}", Interviewer.DisplayName);

                    if (call.Type == CallManager.CallType.Transfer)
                        _dialer.Transfers.AssignCallOnInterviewer(call, this);

                }
                else
                {
                    var agentId = 0;

                    if (outcome == CallOutcome.Connected)
                    {
                        ActiveCall = call;
                        if (!SetReady(false))
                        {
                            break;
                        }

                        agentId = Interviewer.AgentId;
                        WholeInterviewRecordingTimer.Restart();
                    }

                    var callOutcomeMetadata = new Dictionary<string, string>() {
                        { "InitializationTime", _dialer.InitializationTime.ToString() },
                        { "RequestId", _dialer.Simulator.RequestId.Value.ToString() }
                    };

                    _simulator.DialerEvents.NotifyOutcome(
                        Interviewer.CompanyId,
                        Interviewer.DialerId,
                        call.CampaignId,
                        agentId,
                        call.Info.interviewId,
                        call.Info.callId,
                        outcome,
                        Interviewer.ConnectionString,
                        processingTime,
                        callOutcomeMetadata,
                        Guid.NewGuid().ToString());

                    if (outcome == CallOutcome.Connected)
                    {
                        SimulatorDialerDriverPerformanceCounters.RateOfNotifyConnectedCallsCountPerSecond.Increment();
                    }
                    else
                    {
                        SimulatorDialerDriverPerformanceCounters.RateOfNotifyNotConnectedCallsCountPerSecond.Increment();
                    }

                    SimulatorDialerDriverPerformanceCounters.NumberOfDialedPredictiveCalls.Increment();

                    _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController",
                        "finish generate outcome interviewer-{0}", Interviewer.AgentId);
                }
            }

            _simulator.Logger.Info("SimulatorDialerDriver.InterviewerPredictiveController",
                "CallOutcomeGeneratorThread is finished [interviewer-{0}]", Interviewer.DisplayName);
        }

        public override void SetNextInterview(long campaignId, long callId)
        {
            ActiveCall.CampaignId = campaignId;
            ActiveCall.Info.callId = callId;
        }
    }
}
