using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;

namespace DialerIntegrationTests.Framework
{
    /// <summary>
    /// This interface looks as IDialerAPI with reduced number of parameters
    /// it creates to make the test calls more brief (less number of parameters)
    /// in general name of the method must be the same as in IDialerAPI 
    /// </summary>
    public interface ITestDialer : IDisposable
    {
        void Init();
        int DialerInitialize(int dialerId);
        string ConnectionParametersXml { get; }

        void LogStateFileExists();
        void DeleteStateFile();

        void StartCampaign(
            string campaignName,
            DialingMode dialingMode,
            bool recordWholeInterview);

        void Login(int personId, bool isPredictive, bool isLocal);
        void Login(int personId, string agentExtension, bool isPredictive, bool isLocal, DialerErrorCode expectedCode);

        void LoginFailed(int personSid, bool isLocal);

        void Logout(int personSid);

        void ExpectUserStateNotification(AgentStateMsgs userState);
        void WaitUserStateNotification();

        void Dial(int personId);
        void Hangup(int personId);

        void ExpectCallOutcomeNotification(CallOutcome outcomeCode);
        void WaitOutcomeNotification();
        void StopSimulator();
        
        void Clear();
        void ExpectRequestCallsNotification();
        int WaitRequestCallsNotification();
        void WaitRequestCallsNotification(Action additionalActionOnRequest);
        void SendNumbers(List<CallInfo> callsList);
        void ExpectScreenPop();
        void WaitScreenPopNotification();
        DialerState GetState();
        void ExpectDialerState(DialerState dialerState);
        void WaitDialerStateNoticiation();
        void CompletePreview(int personId, int interviewId, int callId, string phoneNumber);
        void FlushNumbers(List<CallInfo> callsList);
        void GoReady(int personSid, bool isInterviewerReady);
    }
}