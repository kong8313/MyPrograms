using System;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.SurveyInstances
{
    public class CampaignControllerPredictive : CampaignController
    {
        public CallManager CallManager { get; set; }

        private readonly ManualResetEvent _evtStopThread = new ManualResetEvent(false);
        private readonly Thread _requestCallsThread;
        

        public CampaignControllerPredictive(ISimulator simulator, Dialer dialer, Campaign campaign )
            :base(simulator, dialer, campaign, DialingMode.Predictive)
        {
            CallManager = new CallManager();

            if (Simulator.Scenario.GenerateRequestCalls)
            {
                _requestCallsThread = new Thread(RequestCallsThread);
                _requestCallsThread.Start();
                Simulator.Logger.Info("SimulatorDialerDriver.Initialize", "Request calls generator is started for campaign {0}.", CampaignId);
            }
        }

        public override void Destroy()
        {
            Dialer.InterviewersManager.DestroyByCampaign(Campaign);

            if (Settings.Default.SendReturnedNotDialedOnSurveyClose)
            {
                ReturnNotDialedCalls();
            }

            if (Simulator.Scenario.GenerateRequestCalls)
            {
                _evtStopThread.Set();
                _requestCallsThread.Join();
            }
        }

        private void ReturnNotDialedCalls()
        {
            var calls = CallManager.RemoveAll();

            foreach (var call in calls)
            {
                Simulator.DialerEvents.NotifyOutcome(
                    Campaign.CompanyId,
                    Campaign.DialerId,
                    Campaign.CampaignId,
                    0,
                    call.interviewId,
                    call.callId,
                    CallOutcome.ReturnedNotDialled,
                    null,
                    TimeSpan.Zero,
                    null,
                    null);
            }
        }

        private void RequestCallsThread()
        {
            var groupId = Settings.Default.GroupIdForRequestCalls;
            
            CallsSelectionAlgorithm callsSelectionAlgorithm;
            var selectionAlgorithm = Settings.Default.CallsSelectionAlgorithm;
            switch (selectionAlgorithm)
            {
                case 0:
                    callsSelectionAlgorithm = CallsSelectionAlgorithm.ByPersonGroup;
                    break;
                case 1:
                    callsSelectionAlgorithm = CallsSelectionAlgorithm.ByCampaign;
                    break;
                case 2:
                    callsSelectionAlgorithm = CallsSelectionAlgorithm.CallsAssignedToCampaignOnly;
                    break;
                case 3:
                    callsSelectionAlgorithm = CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly;
                    break;
                default:
                    callsSelectionAlgorithm = groupId == 0 ? CallsSelectionAlgorithm.ByCampaign : CallsSelectionAlgorithm.ByPersonGroup;
                    break;
            }

            var timer = Stopwatch.StartNew();
            TimeSpan lastSentRequestTime = TimeSpan.Zero;

            do
            {
                if (ShouldWeRequestCalls() && CouldWeSendReuestForSurvey(lastSentRequestTime, timer.Elapsed))
                {
                    var callsAmount = HowManyCallsShouldBeRequested();
                    lastSentRequestTime = timer.Elapsed;

                    Simulator.DialerEvents.RequestCalls(Simulator.RequestId.Next().ToString(), Campaign.CompanyId, Campaign.DialerId, CampaignId,
                        groupId, callsSelectionAlgorithm, callsAmount);

                    SimulatorDialerDriverPerformanceCounters.AverageOfRequestedCallsCountPerSecond.IncrementBy(callsAmount);
                    SimulatorDialerDriverPerformanceCounters.NumberOfRequestedPredictiveCalls.IncrementBy(callsAmount);
                    CallManager.DemandCall();

                    Simulator.Logger.Info("SimulatorDialerDriver.NumberRequestGenerator",
                        "surveyId-" + CampaignId + " requested calls count-" + callsAmount);
                }

                foreach (var call in CallManager.GetExpiredCallsAndRemove())
                {
                    Simulator.DialerEvents.NotifyOutcome(Campaign.CompanyId, Campaign.DialerId, CampaignId, 0, call.interviewId, call.callId, CallOutcome.ReturnedDiallerExpired, call.dialerSpecificAccompanyInfo,
                        TimeSpan.Zero,
                        null,
                        null);
                }
            } while (!_evtStopThread.WaitOne(Simulator.Scenario.RequestFrequency, false));
        }

        private bool CouldWeSendReuestForSurvey(TimeSpan lastSentRequestTime, TimeSpan currentTime)
        {
            if (CallManager.WasCallDeliveredSinceLastDemand())
            {
                return true;
            }

            return (currentTime - lastSentRequestTime) > Simulator.Scenario.MaxRequestTime;
        }

        
        private bool ShouldWeRequestCalls()
        {
            return HowManyCallsShouldBeRequested() > CallManager.CallsCount;
        }

        private int HowManyCallsShouldBeRequested()
        {
            return Dialer.InterviewersManager.GetCountOfInterviewersByCampaignId(CampaignId) * Simulator.Scenario.CallsCountPerInterviewer;
        }
    }
}
