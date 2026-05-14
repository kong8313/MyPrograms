using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ServiceModel;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Telephony.DialerLibrary;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerIntegrationTests.Framework
{
    public abstract class TestBaseCodiDialer : TestDialer
    {
        protected const int ExpectedNumberOfSamples = 2;
        protected DialerLibrary _dialerLibrary;
        private int _expectedDialerState;

        protected TestBaseCodiDialer(string dialerName)
            : base(dialerName)
        {
            // The hack below is needed for PredictiveRequestCalls_GroupIdIsTransmittedFromDialerToDialerWs test only
            // as it has the '9' hardcoded.
            // TODO: Remove the hardcoded value from PredictiveRequestCalls_GroupIdIsTransmittedFromDialerToDialerWs
            CompanyId = "9";
            TestEventsHandlerServiceUri =
                new Uri("http://localhost/Test/Temporary_Listen_Addresses/TestEventsHandlerService" + CompanyId);

            ConnectionParametersXml = string.Format(
                @"<DialerConnectionParameters>
                      <ServiceAddress>{0}</ServiceAddress>
                      <ServiceEndpoint>DialerServiceEndpoint</ServiceEndpoint>
                      <AuthorizationKeyForOutgoingRequests>{1}</AuthorizationKeyForOutgoingRequests>
                      </DialerConnectionParameters>",
                TestDialerServiceUri,
                "Gfkr31ZZ7jyuUDoM+OQ0cHvaz88fqJoy9zoxdoRJjr7FVRYjWYtVX/C/afumpTX8erM0d5cQZPEtQ9khe/sbOUW8lSyswcJLkqzXkCbKy5mFMxJhTMhEcgE286I=");
        }

        protected abstract string CampaignParameters();
        protected abstract string ConfigurationParameters();

        public virtual void TestInitialize()
        {
        }

        public int DialerInitialize(int dialerId)
        {
            return (int)_dialerLibrary.Initialize(
                dialerId,
                CompanyId,
                ConnectionParametersXml,
                ConfigurationParameters(),
                CampaignParameters()).DialerErrorCode;
        }

        public void StartCampaign(string campaignName, DialingMode dialingMode, bool recordWholeInterview)
        {
            Log.Info("TestProTsDialerCDI.StartCampaign", "campaignName={0}, dialerIds=[{1}], dialingMode={2}, recordWholeInterview={3}",
                campaignName, 1, dialingMode, recordWholeInterview);

            var result = (DialerErrorCode)_dialerLibrary.StartCampaign(
                CompanyId,
                new[] { 1 },
                CampaignId,
                campaignName,
                dialingMode,
                "The campaignType parameter is obsolet and not being used anymore",
                recordWholeInterview,
                CampaignParameters());

            Assert.AreEqual(DialerErrorCode.Success, result, "TestProTsDialerConfirmitDialerInterface: StartCampaign failed.");
        }

        public void Login(int personId, bool isPredictive, bool isLocal)
        {
            Login(personId, ExtensionNumber, isPredictive, isLocal, DialerErrorCode.Success);
        }


        public void Login(int personId, string agentExtension, bool isPredictive, bool isLocal, DialerErrorCode expectedCode)
        {
            Log.Info("TestProTsDialerCDI.Login",
                "personId={0}, isPredictive={1}, isLocal={2}, expectedCode={3}",
                personId, isPredictive, isLocal, expectedCode);

            var result = (DialerErrorCode)_dialerLibrary.Login(
                CompanyId,
                CampaignId,
                personId.ToString(CultureInfo.InvariantCulture),
                "CodiDialerIntegrationTestAgent",
                AgentType.LiveAgent,
                agentExtension,
                string.Empty,
                isPredictive,
                true,
                new Collection<KeyValuePair<string, string>>());

            Assert.AreEqual(expectedCode, result, "TestProTsDialerConfirmitDialerInterface: Login result is not as expected.");
        }

        public void LoginFailed(int personId, bool isLocal)
        {
            Log.Info("TestProTsDialerCDI.LoginFailed",
                "personId={0}, isLocal={1} [{2}]",
                personId, isLocal, isLocal ? "local" : "vis");

            try
            {
                // Login is called twice. It's because of socket specific after closing.
                // Socket looks like alive on the very first Send after Close. But the second Send will give exception as expected.
                _dialerLibrary.Login(
                    CompanyId,
                    CampaignId,
                    personId.ToString(CultureInfo.InvariantCulture),
                    "CodiDialerIntegrationTestAgent",
                    AgentType.LiveAgent,
                    ExtensionNumber,
                    string.Empty,
                    false,
                    true,
                    new Collection<KeyValuePair<string, string>>());

                var result = _dialerLibrary.Login(
                    CompanyId,
                    CampaignId,
                    personId.ToString(CultureInfo.InvariantCulture),
                    "CodiDialerIntegrationTestAgent",
                    AgentType.LiveAgent,
                    ExtensionNumber,
                    string.Empty,
                    false,
                    true,
                    new Collection<KeyValuePair<string, string>>());

                if (result == (int)DialerErrorCode.Success)
                {
                    Assert.Fail("TestProTsDialerConfirmitDialerInterface: Login was successful, but it is expected to be failed.");
                }
            }
            catch (FaultException)
            {
                // Now we do not catch exceptions in PROTSLibrary.DoDialerServiceCall so the operation had to throw.
            }
        }

        public void Logout(int personId)
        {
            Log.Info("TestProTsDialerCDI.Logout", "personId={0}", personId);

            var result = _dialerLibrary.Logout(CompanyId, CampaignId, false, personId.ToString(CultureInfo.InvariantCulture));

            Assert.AreEqual(0x0, result, "TestProTsDialerConfirmitDialerInterface: Logout failed.");
        }

        public void Dial(int personId)
        {
            var interviewId = Randomizer.Next() + 1;
            var callId = Randomizer.Next() + 1;
            var phoneNumber = Randomizer.Next(100, 100000).ToString(CultureInfo.InvariantCulture);

            Log.Info("TestProTsDialerCDI.Dial",
                "personId={0}, interviewId={1}, callId={2}, phoneNumber={3}",
                personId, interviewId, callId, phoneNumber);

            var result = _dialerLibrary.SendNumberToAgent(
                CompanyId,
                CampaignId,
                personId.ToString(CultureInfo.InvariantCulture),
                DialingMode.Automatic,
                interviewId,
                callId,
                phoneNumber,
                false,
                string.Empty, 
                null);

            Assert.AreEqual(0x0, result, "TestProTsDialerConfirmitDialerInterface: SendNumberToAgent failed.");
        }

        public void SendNumbers(List<CallInfo> callsList)
        {
            Log.Info("TestProTsDialerCDI.SendNumbers", "callCount={0}", callsList.Count);

            var result = _dialerLibrary.SendNumbers(null, CompanyId, CampaignId, DialingMode.Predictive, callsList, 0, false);

            Assert.AreEqual(0x0, result, "TestProTsDialerConfirmitDialerInterface: SendNumbers failed.");
        }

        public void Hangup(int personId)
        {
            Log.Info("TestProTsDialerCDI.Hangup", "personId={0}", personId);

            var result = _dialerLibrary.CompleteCall(CompanyId, CampaignId, personId.ToString(CultureInfo.InvariantCulture), null, true, null, 0, 0);
            Assert.AreEqual(0x0, result, "TestProTsDialerConfirmitDialerInterface: CompleteCall failed.");
        }

        public void ExpectUserStateNotification(AgentStateMsgs userState)
        {
            ExpectedNotificationMethod = Tools.ParseLambda(x => x.NotifyUserState(1, null, null, null, 0, null, null));
            ExpectedUserState = userState;

            TestDialerEventsHandlerService.ExpectedNotificationMethod = ExpectedNotificationMethod;
            TestDialerEventsHandlerService.NotifyEvent.Reset();
        }

        public void ExpectCallOutcomeNotification(CallOutcome outcomeCode)
        {
            ExpectedNotificationMethod = Tools.ParseLambda(
                x => x.NotifyOutcome(1, null, null, null, 0, null, null, 0, null, null,  null, TimeSpan.Zero, null, null));
            ExpectedOutcome = outcomeCode;

            TestDialerEventsHandlerService.NotifyEvent.Reset();
        }

        public void WaitUserStateNotification()
        {
            Assert.IsTrue(
                TestDialerEventsHandlerService.NotifyEvent.WaitOne(NotificationTimeout),
                "TestProTsDialerConfirmitDialerInterface: User state notification has not been received within maximum allowed timeout ({0} ms).",
                NotificationTimeout);

            Assert.AreEqual(
                ExpectedNotificationMethod,
                TestDialerEventsHandlerService.LastCalledMethod,
                "TestProTsDialerConfirmitDialerInterface: Invalid notification has been received.");

            Assert.AreEqual(
                ExpectedUserState,
                TestDialerEventsHandlerService.LastRecievedUserState,
                "TestProTsDialerConfirmitDialerInterface: Invalid user state has been received in the notification.");
        }

        public void WaitOutcomeNotification()
        {
            Assert.IsTrue(
                TestDialerEventsHandlerService.NotifyEvent.WaitOne(NotificationTimeout),
                "TestProTsDialerConfirmitDialerInterface: Outcome notification has not been received within maximum allowed timeout ({0} ms).",
                NotificationTimeout);

            Assert.AreEqual(
                ExpectedNotificationMethod,
                TestDialerEventsHandlerService.LastCalledMethod,
                "TestProTsDialerConfirmitDialerInterface: Invalid notification has been received.");

            Assert.AreEqual(
                ExpectedOutcome,
                _dialerLibrary.TranslateOutcome(long.Parse(TestDialerEventsHandlerService.LastRecievedOutcome)),
                "TestProTsDialerConfirmitDialerInterface: Invalid outcome has been received in the notification.");
        }

        public void ExpectRequestCallsNotification()
        {
            ExpectedNotificationMethod = Tools.ParseLambda(
                x => x.RequestCalls(1, null, null, null, 0, null, 0, 0));

            TestDialerEventsHandlerService.ExpectedNotificationMethod = ExpectedNotificationMethod;
            TestDialerEventsHandlerService.NotifyEvent.Reset();
        }

        public virtual int WaitRequestCallsNotification()
        {
            TraceInformation("TestProTsDialerConfirmitDialerInterface.WaitRequestCallsNotification", "Start waiting");

            Assert.IsTrue(
                TestDialerEventsHandlerService.NotifyEvent.WaitOne(NotificationTimeout),
                "TestProTsDialerConfirmitDialerInterface: RequestCalls notification has not been received within maximum allowed timeout ({0} ms).",
                NotificationTimeout);

            Assert.AreEqual(
                ExpectedNotificationMethod,
                TestDialerEventsHandlerService.LastCalledMethod,
                "TestProTsDialerConfirmitDialerInterface: Invalid notification has been received.");

            Assert.AreEqual(
                ExpectedNumberOfSamples,
                TestDialerEventsHandlerService.LastReceivedNumberOfCalls,
                "TestProTsDialerConfirmitDialerInterface: Expected numbet of calls not equal to actual");

            TraceInformation("TestProTsDialerConfirmitDialerInterface.WaitRequestCallsNotification", "End of waiting");

            return ExpectedNumberOfSamples;
        }

        public void WaitRequestCallsNotification(Action additionalActionOnRequest)
        {
            WaitRequestCallsNotification();
            additionalActionOnRequest.Invoke();
        }

        public void ExpectScreenPop()
        {
            ExpectedNotificationMethod = Tools.ParseLambda(
                x => x.ScreenPop(1, null, null, null, 0, null, null, 0, 0));
            TestDialerEventsHandlerService.NotifyEvent.Reset();
        }

        public void WaitScreenPopNotification()
        {
            Assert.IsTrue(
                TestDialerEventsHandlerService.NotifyEvent.WaitOne(NotificationTimeout),
                "TestProTsDialerConfirmitDialerInterface: ScreenPop notification has not been received within maximum allowed timeout ({0} ms).",
                NotificationTimeout);

            Assert.AreEqual(
                ExpectedNotificationMethod,
                TestDialerEventsHandlerService.LastCalledMethod,
                "TestProTsDialerConfirmitDialerInterface: Invalid notification has been received.");
        }

        public DialerState GetState()
        {
            var result = _dialerLibrary.GetState(1, CompanyId);
            return result;
        }

        public void ExpectDialerState(DialerState dialerState)
        {
            ExpectedNotificationMethod = Tools.ParseLambda(
                x => x.NotifyDialerState(1, null, 0));
            _expectedDialerState = (int)dialerState;
            TestDialerEventsHandlerService.NotifyEvent.Reset();
        }

        public void WaitDialerStateNoticiation()
        {
            Assert.IsTrue(
                TestDialerEventsHandlerService.NotifyEvent.WaitOne(NotificationTimeout),
                "TestProTsDialerConfirmitDialerInterface: DialerState notification has not been received within maximum allowed timeout ({0} ms).",
                NotificationTimeout);

            Assert.AreEqual(
                ExpectedNotificationMethod,
                TestDialerEventsHandlerService.LastCalledMethod,
                "TestProTsDialerConfirmitDialerInterface: Invalid notification has been received.");

            Assert.AreEqual(_expectedDialerState,
                TestDialerEventsHandlerService.LastDialerState,
                "TestProTsDialerConfirmitDialerInterface:Expected dialer state not equal to actual");

        }

        public void CompletePreview(int personId, int interviewId, int callId, string phoneNumber)
        {
            Log.Info("TestProTsDialerCDI.CompletePreview",
                "personId={0}, interviewId={1}, callId={2}, phoneNumber={3}",
                personId, interviewId, callId, phoneNumber);

            var result = _dialerLibrary.CompletePreview(
                CompanyId, CampaignId, personId.ToString(CultureInfo.InvariantCulture), interviewId, callId, phoneNumber, false);

            Assert.AreEqual(0x0, result, "TestProTsDialerConfirmitDialerInterface: CompletePreview failed.");
        }

        public void FlushNumbers(List<CallInfo> callsList)
        {
            Log.Info("TestProTsDialerCDI.FlushNumbers", "callCount={0}", callsList.Count);

            var result = _dialerLibrary.FlushNumbers(CompanyId, new[] { 1 }, CampaignId, callsList);

            Assert.AreEqual(0x0, result, "TestProTsDialerConfirmitDialerInterface: FlushNumbers failed.");
        }

        public void GoReady(int personSid, bool isInterviewerReady)
        {
            Log.Info("TestProTsDialerCDI.GoReady", "personSid={0}, isInterviewerReady={1}", personSid, isInterviewerReady);

            if (isInterviewerReady)
            {
                _dialerLibrary.GoReady(CompanyId, CampaignId, personSid.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _dialerLibrary.GoNotReady(CompanyId, CampaignId, personSid.ToString(CultureInfo.InvariantCulture), string.Empty);
            }
        }

        protected override void Release()
        {
            _dialerLibrary.Release(0, 0); // dialerId in Release method isn't used by confirmit code, it is introduced for clients inner usage
        }
    }
}