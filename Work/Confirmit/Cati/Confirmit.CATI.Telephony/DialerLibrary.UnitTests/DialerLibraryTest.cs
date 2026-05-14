using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Common.WcfTools.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.DialerService.Contract;
using Confirmit.Test.Common;
using Confirmit.Test.Common.Attributes;
using DialerCommon.DialerParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfirmitDialerInterface;

namespace DialerLibrary.UnitTests
{
    /// <summary>
    ///This is a test class for DialerLibraryTest and is intended
    ///to contain all DialerLibraryTest Unit Tests
    ///</summary>
    [TestClass]
    public class DialerLibraryTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Cleanup();
            serviceLocator.Initialize();
            ServiceLocator.Register<IDialerApiClient, StubIDialerApiClient>();
            new SystemSettingUnitTestRegistrator().RegisterTypes(serviceLocator);
        }
        /// <summary>
        /// Related tests are
        /// BvTciLibraryTest.BvTciDialerChannelFactoryWrapperConfiguration_LogExceptionsIsSwitchedOffByDefault(),
        /// PROTSLibraryTest.ProTsDialerChannelFactoryWrapperConfiguration_LogExceptionsIsSwitchedOffByDefault()
        ///</summary>
        [TestMethod, Owner(@"FIRM\alm"), Cr(56936), Ignore]
        public void DialerChannelFactoryWrapperConfiguration_LogExceptionsIsSwitchedOffByDefault()
        {
            var configuration = new DialerChannelFactoryWrapperConfiguration("", "", "", false);
            Assert.AreEqual(false, configuration.LogExceptions, "LogExceptions option must be switched 'off'. See CR #56936 for details.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(59469)]
        public void DialerLibraryObjectIsInitialized_EmptyDialerObjectGetsIsPersonModeSupportedSuccessfully()
        {
            ServiceLocator.Register<ISideBySideManager, SideBySideManager>();

            var supportedPersonModes = "Manual,CampaignAssignment";
            var isReloginNeededOnCampaignChange = true;
            var isHangUpSupported = true;
            var isPauseOrResumePlaybackSupported = true;
            var isToggleAgentListensToPlaybackOrRespondentSupported = true;
            var isDynamicExtensionNumberAllowedForLocalAgents = true;
            var isDynamicExtensionNumberAllowedForRemoteAgents = true;

            string connectionParametersXml = "<DialerConnectionParameters>" +
                                             "<ServiceAddress>http://localhost/DialerService/DialerService.svc</ServiceAddress>" +
                                             "<ServiceEndpoint>DialerServiceEndpoint</ServiceEndpoint>" +
                                             "<AuthorizationKeyForOutgoingRequests>wjkdVnaK4BCsETRMRTgqaBb5boMG1YRzak4Ng4sDB027jOj+KKwrn1RkcO2TMq5SRv4tjDkiFQoopaatHlUZoA7pn85i1goLC7YmvzbNpIdQhI78D7JuuRRzfWs=</AuthorizationKeyForOutgoingRequests>" +
                                             "</DialerConnectionParameters>";
            string configurationParametersXml = "<DialerConfigurationParameters>" +
                                                "<SupportedPersonModes>" + supportedPersonModes + "</SupportedPersonModes>" +
                                                "<IsReloginNeededOnCampaignChange>" + isReloginNeededOnCampaignChange + "</IsReloginNeededOnCampaignChange>" +
                                                "<IsHangUpSupported>" + isHangUpSupported + "</IsHangUpSupported>" +
                                                "<IsPauseOrResumePlaybackSupported>" + isPauseOrResumePlaybackSupported + "</IsPauseOrResumePlaybackSupported>" +
                                                "<IsToggleAgentListensToPlaybackOrRespondentSupported>" + isToggleAgentListensToPlaybackOrRespondentSupported + "</IsToggleAgentListensToPlaybackOrRespondentSupported>" +
                                                "<IsDynamicExtensionNumberAllowedForLocalAgents>" + isDynamicExtensionNumberAllowedForLocalAgents + "</IsDynamicExtensionNumberAllowedForLocalAgents>" +
                                                "<IsDynamicExtensionNumberAllowedForRemoteAgents>" + isDynamicExtensionNumberAllowedForRemoteAgents + "</IsDynamicExtensionNumberAllowedForRemoteAgents>" +
                                                "</DialerConfigurationParameters>";

            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary(new ChannelFactoryWrapperFactory<IDialerService>());
            dialerLibrary.Initialize(1, "1", connectionParametersXml, configurationParametersXml, "");

            // Check that we obtain correct values through the emptyDialerObject
            Assert.AreEqual(true, dialerLibrary.IsPersonModeSupported(AgentTaskChoiceMode.Manual));
            Assert.AreEqual(true, dialerLibrary.IsPersonModeSupported(AgentTaskChoiceMode.CampaignAssignment));
            Assert.AreEqual(false, dialerLibrary.IsPersonModeSupported(AgentTaskChoiceMode.Automatic));
            Assert.AreEqual(false, dialerLibrary.IsPersonModeSupported(AgentTaskChoiceMode.Choice));

            Assert.AreEqual(isReloginNeededOnCampaignChange, dialerLibrary.IsReloginNeededOnSurveyChange());
            Assert.AreEqual(isHangUpSupported, dialerLibrary.IsHangUpSupported);
            Assert.AreEqual(isPauseOrResumePlaybackSupported, dialerLibrary.IsPauseOrResumePlaybackSupported);
            Assert.AreEqual(isToggleAgentListensToPlaybackOrRespondentSupported, dialerLibrary.IsToggleInterviewerListensToPlaybackOrRespondentSupported);
            Assert.AreEqual(isDynamicExtensionNumberAllowedForLocalAgents, dialerLibrary.IsDynamicExtensionNumberAllowed(true));
            Assert.AreEqual(isDynamicExtensionNumberAllowedForRemoteAgents, dialerLibrary.IsDynamicExtensionNumberAllowed(false));
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        [ExpectedException(typeof(Exception))]
        public void DoDialerServiceCall_NonRetryableExceptionIsThrown_NoRetriesAndExceptionRethrown()
        {
            var retryCounter = 0;
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            Assert.IsFalse(
                dialerLibrary.ExceptionTypesToRetry.Contains(typeof(Exception)),
                "Retryable exception is selected to test non-retryable behaviour.");

            const int retryLimit = 10;
            try
            {
                dialerLibrary.DoDialerServiceCall<DialerErrorCode>(() =>
                {
                    retryCounter++;
                    throw new Exception();
                }, retryLimit);
            }
            finally
            {
                Assert.AreEqual(1, retryCounter, "Unexpected amount of retries.");
            }
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void DoDialerServiceCall_AnyRetryableExceptionIsThrown_RetriesAppropriateTimes()
        {
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();
            foreach (var exceptionType in dialerLibrary.ExceptionTypesToRetry)
            {
                var exception = (Exception)Activator.CreateInstance(exceptionType);
                TestOneRetryableException_RetriesAppropriateTimes(exception);
            }
        }

        private void TestOneRetryableException_RetriesAppropriateTimes<T>(T exception) where T : Exception
        {
            var retryCounter = 0;
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            var stopwatch = Stopwatch.StartNew(); // in order to check that there is an appropriate delay between the retries
            //TODO: think of another way how to check the delays

            const int retryLimit = 3;
            try
            {
                dialerLibrary.DoDialerServiceCall<DialerErrorCode>(() =>
                {
                    retryCounter++;
                    throw exception;
                }, retryLimit);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Assert.AreEqual(retryLimit, retryCounter, "Incorrect amount of retries.");
                Assert.IsInstanceOfType(ex, typeof(Exception), "Unexpected exception.");

                var expectedExceptionTextSubstring = string.Format(
                    "Service calls retry limit [{0}] is reached", retryLimit);
                Assert.IsTrue(
                    ex.Message.Contains(
                    expectedExceptionTextSubstring),
                    string.Format(
                        "Exception message does not contain expected string. Expected substring: '{0}'. Actual string: '{1}'",
                        expectedExceptionTextSubstring,
                        ex.Message)
                    );

                var actualTime = stopwatch.ElapsedMilliseconds;

                // "-5" below is bacause of Thread.Sleep() used inside of DoDialerServiceCall is not much accurate.
                var expectedTime =
                    (retryLimit - 1) * (Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary.DelayBetweenRetries - 5);

                Assert.IsTrue(
                    stopwatch.ElapsedMilliseconds >= expectedTime,
                    string.Format("Time spent [{0}] on retries is less then expected [{1}]. " +
                        "/// retryLimit={2}, DelayBetweenRetries={3}",
                        actualTime, expectedTime,
                        retryLimit, Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary.DelayBetweenRetries));

                expectedTime =
                    retryLimit * Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary.DelayBetweenRetries;

                Assert.IsTrue(
                    stopwatch.ElapsedMilliseconds < expectedTime,
                    string.Format("Time spent [{0}] on retries is more then expected [{1}]. " +
                        "/// retryLimit={2}, DelayBetweenRetries={3}",
                        actualTime, expectedTime,
                        retryLimit, Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary.DelayBetweenRetries));

                return;
            }

            Assert.Fail("Exception was expected, but was not thrown.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void DoDialerServiceCall_RetryableExceptionIsThrownLessThanRetryLimitThenTheCallSucceeds_MethodSucceeds()
        {
            var retryCounter = 0;
            const int attemptToSucceed = 1;

            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            var result = dialerLibrary.DoDialerServiceCall(() =>
            {
                if (retryCounter == attemptToSucceed)
                {
                    return DialerErrorCode.Success;
                }

                retryCounter++;
                throw new ServerTooBusyException();
            }, attemptToSucceed + 1); //The method must not throw exceptions.
            Assert.AreEqual(DialerErrorCode.Success, result);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void Execute_CommonExceptionOccures_ExceptionIsNotRethrownAndAppropriateResultIsReturned()
        {
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            var result = dialerLibrary.Execute("", () => { throw new Exception(); });

            Assert.AreEqual(DialerErrorCode.Exception, result, "Unexpected error code");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void Execute_FaultExceptionInstantiatedToDialerExceptionOccures_ExceptionIsNotRethrownAndErrorCodeFromDialerExceptionIsReturned()
        {
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            const DialerErrorCode dialerError = DialerErrorCode.UnknownAgent; //Any code fits for the test

            var result = dialerLibrary.Execute("", () =>
            {
                throw new FaultException<DialerExceptionDetail>(new DialerExceptionDetail(new DialerException(dialerError, "")));
            });

            Assert.AreEqual(dialerError, result, "Unexpected error code");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        [ExpectedException(typeof(DialerParametersException))]
        public void Execute_DialerParametersExceptionOccurs_ExceptionIsRethrownAsDialerParametersException()
        {
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            dialerLibrary.Execute(
                "",
                () => dialerLibrary.DoDialerServiceCall<DialerErrorCode>(() =>
                    {
                        var dialerParameterError = new DialerParameterError("id", "name", "errorDescription");
                        var parametersException = new ParametersException(new[] { dialerParameterError });
                        throw new DialerParametersException(parametersException);
                    },
                    1));

            Assert.Fail("Exception was expected, but was not thrown.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TranslateOutcome_AllExpectedOutcomesHaveDirectTranslationAllUnexpextedAreTranslatedToTelephonlyFailure()
        {
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            foreach (CallOutcome outcome in Enum.GetValues(typeof(CallOutcome)))
            {
                var translatedOutcome = dialerLibrary.TranslateOutcome((int)outcome);

                var expectedOutcome = dialerLibrary.UnexpectedOutcomes.Contains(outcome) ? CallOutcome.TelephonyFailure : outcome;

                Assert.AreEqual(expectedOutcome, translatedOutcome, "Incorrect outcome translation");
            }
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TranslateOutcome_AllOutOfRangeOutcomesAreTranslatedToTelephonlyFailure()
        {
            var outOfRangeOutcomes = new[]
            {
                (CallOutcome) (-100), // under range outcome
                (CallOutcome) 100,    // above range outcome
                (CallOutcome) 19      // reserved for future in CallOutcome enum
            };

            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();

            foreach (var outOfRangeOutcome in outOfRangeOutcomes)
            {
                var translatedOutcome = dialerLibrary.TranslateOutcome((int)outOfRangeOutcome);

                Assert.IsFalse(Enum.IsDefined(typeof(CallOutcome), outOfRangeOutcome), "'OutOfRange' value is in fact a member of CallOutcome enum.");
                Assert.AreEqual(CallOutcome.TelephonyFailure, translatedOutcome, "Incorrect outcome translation");
            }
        }

        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void ConnectInboundCall_CallWithCampaignIdsToBorrowAgentsFromIsNull_CheckException()
        {
            var dialerLibrary = new Confirmit.CATI.Telephony.DialerLibrary.DialerLibrary();
            dialerLibrary.ConnectInboundCall(1, 1, "1", new CallInfo(), null, new AudioMessageDescriptor());
        }
    }
}
