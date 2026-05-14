using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver.Controllers
{
    public abstract class BaseInterviewerController : IInterviewerController
    {
        public Interviewer Interviewer { get; private set; }
        public CallManager.CallInfoEx ActiveCall { get; protected set; }
        protected ISimulator _simulator;
        protected Dialer _dialer;
        private readonly ManualResetEvent _hangupEvent = new ManualResetEvent(false);
        protected readonly Stopwatch WholeInterviewRecordingTimer = new Stopwatch();
        private SectionalRecordingInfo SectionalRecordingInfo;
        private int _callAttempt = 0;
        private int _interviewId = 0;
        private long _surveyId = 0;

        public BaseInterviewerController(ISimulator simulator, Dialer dialer, Interviewer interviewer)
        {
            _simulator = simulator;
            _dialer = dialer;
            Interviewer = interviewer;
        }

        public abstract bool SetReady(bool isReady);
        public abstract void SetCampaignId(long campaignId);
        public abstract void SetNextInterview(long CampaignId, long CallId);
        public abstract void Destroy();
        public abstract void SetGroups(int[] agentGroups);
        public abstract void CompletePreview(int companyId, int dialerId);
        public abstract void SendNumberToAgent(int companyId, int dialerId, CallManager.CallInfoEx call);
        public abstract void ConnectInboundCallToAgent(int companyId, int dialerId, CallManager.CallInfoEx call);
        public void StartSectionalRecording(string label)
        {
            SectionalRecordingInfo = new SectionalRecordingInfo(label);
        }

        public void StopRecording(StopRecordingMode stopRecordingMode)
        {
            if (stopRecordingMode == StopRecordingMode.Sectional || stopRecordingMode == StopRecordingMode.Both)
            {
                if (SectionalRecordingInfo != null && ActiveCall != null )
                {
                    CheckIfNewInterview();
                    
                    new InterviewRecordingFileCreator(_simulator.Logger).CreateAudioFile(ActiveCall.CampaignId.ToString(),
                        ActiveCampaign.Name, ActiveCall.Info.interviewId, SectionalRecordingInfo.Timer.Elapsed,
                        AudioPath, ++_callAttempt, SectionalRecordingInfo.Label);

                    SectionalRecordingInfo = null;
                }
            }
            
            if (stopRecordingMode == StopRecordingMode.WholeInterview || stopRecordingMode == StopRecordingMode.Both)
            {
                StopWholeInterviewRecording();
            }
        }

        public void Redial(int companyId, int dialerId, CallManager.CallInfoEx call)
        {
            Hangup();
            Dial(companyId, dialerId, call);
        }

        protected void Dial(int companyId, int dialerId, CallManager.CallInfoEx call)
        {
            _hangupEvent.Reset();
                
            ActiveCall = call;

            var callOutcomeData = _simulator.CallOutcomeDistributor.GetNextCallOutcomeDistributionData(call.Info.phoneNumber, call.Type);
            var context = new ContextInfo(Interviewer, call.Info.interviewId);
            var processingTime = Generators.CallOutcomeDelay.GetValue(context, callOutcomeData.ProcessingTime);

            _simulator.Logger.Info("SimulatorDialerDriver.BaseInterviewerController",
                    "wait processing time-{0} interviewer-{1} interviewId-{2} callattempt-{3}",
                    processingTime, Interviewer.DisplayName, call.Info.interviewId, _callAttempt );

            AsyncManager.Execute(_simulator.Logger, () =>
            {

                var callOutcome = Generators.CallOutcomeValue.GetValue(context, callOutcomeData.CallOutcome, _hangupEvent);

                if(_hangupEvent.WaitOne(processingTime))
                {
                    callOutcome = CallOutcome.DialingInterrupted; // That means "dialing" is interrupted with Hangup
                }

                var callOutcomeMetadata = _simulator.CallOutcomeDistributor.CallOutcomeDistributionScenario.OutcomeMetadataList?.ToDictionary(x => x.Key, x => x.Value);

                _simulator.DialerEvents.NotifyOutcome(
                    companyId,
                    dialerId,
                    call.CampaignId,
                    Interviewer.AgentId,
                    call.Info.interviewId,
                    call.Info.callId,
                    callOutcome,
                    _simulator.Scenario.DialerCallerId,
                    processingTime,
                    callOutcomeMetadata,
                    Guid.NewGuid().ToString());
                
                WholeInterviewRecordingTimer.Restart();
                
                if (callOutcome == CallOutcome.Connected)
                {
                    SimulatorDialerDriverPerformanceCounters.RateOfNotifyConnectedCallsCountPerSecond.Increment();
                }
                else
                {
                    SimulatorDialerDriverPerformanceCounters.RateOfNotifyNotConnectedCallsCountPerSecond.Increment();
                }
            });
        }

        public void Hangup()
        {
            StopWholeInterviewRecording();

            _hangupEvent.Set();
        }

        private void StopWholeInterviewRecording()
        {
            if (ActiveCall != null)
            {
                var recordWholeInterview = ActiveCampaign.RecordWholeInterview;

                if (WholeInterviewRecordingTimer.IsRunning && recordWholeInterview)
                {
                    CheckIfNewInterview();
                   
                    new InterviewRecordingFileCreator(_simulator.Logger).CreateAudioFile(ActiveCall.CampaignId.ToString(),
                        ActiveCampaign.Name, ActiveCall.Info.interviewId, WholeInterviewRecordingTimer.Elapsed,
                        AudioPath, ++_callAttempt);
                }
            }

            WholeInterviewRecordingTimer.Stop();
        }

        private Campaign ActiveCampaign =>
            _simulator.GetDialerWithCheck(Interviewer.CompanyId, Interviewer.DialerId).CampaignsManager.Get(ActiveCall.CampaignId).Campaign;

        private string AudioPath
        {
            get
            {
                var audioFolderName = string.IsNullOrEmpty(_simulator.Scenario.AudioFolderName) ? "audio"
                    : _simulator.Scenario.AudioFolderName;
                var audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, audioFolderName);
                return audioPath;
            }
        }

        public void CompleteCall()
        {
            Hangup();
            ActiveCall = null;
        }

        private void CheckIfNewInterview()
        {
            if (_surveyId != ActiveCall.CampaignId || _interviewId != ActiveCall.Info.interviewId)
            {
                _surveyId = ActiveCall.CampaignId;
                _interviewId = ActiveCall.Info.interviewId;
                _callAttempt = 0;
            }
        }
    }
}