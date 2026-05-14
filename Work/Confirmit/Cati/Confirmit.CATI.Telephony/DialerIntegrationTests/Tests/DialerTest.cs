using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using Confirmit.CATI.Common.Random;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;

using DialerIntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.DialerLibrary;
using Confirmit.CATI.Telephony.DialerService.Contract;
using DialerCommon;

namespace DialerIntegrationTests.Tests
{
    [TestClass]
    public class DialerTest
    {
        private DialerTestingFramework _dialerTestingFramework;
        private readonly Logger _logger = new Logger("DialerTest");
        private CatiCommonILoggerToCodiILogger _catiCommonILoggerToCodiILogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _catiCommonILoggerToCodiILogger = new CatiCommonILoggerToCodiILogger(_logger);

            _dialerTestingFramework = new DialerTestingFramework();

            // We need to change CurrentDirectory as the SimulatorScenario.xml file can't be found otherwise
            var executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!string.IsNullOrEmpty(executingAssemblyDirectory))
            {
                Environment.CurrentDirectory = executingAssemblyDirectory;
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), CannotWorkInParallel]
        public void LoginExternalInterviewer_Logout_Success()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, false, false);
                    dialer.WaitUserStateNotification();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();
                });
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), CannotWorkInParallel]
        public void LoginLocalInterviewer_Logout_Success()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, false, true);
                    dialer.WaitUserStateNotification();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();
                });
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), CannotWorkInParallel]
        public void LoginExternalInterviewer_Dial_Hangup_Logout_Success()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.StartCampaign("LoginExternalInterviewer_Dial_Hangup_Logout_Success test", DialingMode.Preview, false);

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, false, false);
                    dialer.WaitUserStateNotification();

                    dialer.ExpectCallOutcomeNotification(CallOutcome.Connected);
                    dialer.Dial(personSid);
                    dialer.WaitOutcomeNotification();

                    dialer.Hangup(personSid);

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();
                });
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), CannotWorkInParallel]
        public void LoginLocalInterviewer_Dial_Hangup_Logout_Success()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.StartCampaign("LoginLocalInterviewer_Dial_Hangup_Logout_Success test", DialingMode.Preview, false);

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, false, true);
                    dialer.WaitUserStateNotification();

                    dialer.ExpectCallOutcomeNotification(CallOutcome.Connected);
                    dialer.Dial(personSid);
                    dialer.WaitOutcomeNotification();

                    dialer.Hangup(personSid);

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();
                });
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), CannotWorkInParallel, Ignore] // Todo: randomly failed test. Need to fix
        public void LoginExternalInterviewer_DialerUnavailiable_LoginFailed()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.StopSimulator();
                    Thread.Sleep(500);

                    dialer.LoginFailed(personSid, false);
                });
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), CannotWorkInParallel]
        [Ignore]
        public void LoginLocalInterviewer_DialerUnavailiable_LoginFailed()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.StopSimulator();

                    dialer.LoginFailed(personSid, true);
                });
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(72555), CannotWorkInParallel]
        public void DialerIdInInitialiseCommandDiffersFromTheOneInDialerWSConfigFile_InitializationResultIsCodeException()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    Confirmit.CATI.Telephony.DialerService.Settings.Default["DialerId"] = 1;
                    var initializationResult = dialer.DialerInitialize(2); // TODO: it would be great to check somehow 
                    // that this exception result is exactly because of throw 
                    // on incorrect dialerId in DialerService.Initialise
                    Assert.AreEqual((int)DialerErrorCode.Exception, initializationResult);
                });
        }

        [TestMethod, Owner(@"FIRM\alm"), CannotWorkInParallel]
        public void DialerIdInInitialiseCommandDiffersFromTheOneInDialerWSConfigFile_DialerWsThrowsException()
        {
            _dialerTestingFramework.ExecuteTest(
                dialer =>
                {
                    const int initialDialerId = 1;
                    const int anotherDialerId = 2;

                    Confirmit.CATI.Telephony.DialerService.Settings.Default["DialerId"] = initialDialerId;

                    var connectionParameters = new ConnectionParameters(dialer.ConnectionParametersXml);
                    const string configurationParameters =
                        @"<DialerConfigurationParameters>
                            <RootDirectoryForAudioRecords>C:\DSM</RootDirectoryForAudioRecords>
                        </DialerConfigurationParameters>";

                    var dialerChannelFactoryWrapperConfiguration = new DialerChannelFactoryWrapperConfiguration(
                        connectionParameters.DialerServiceEndpoint,
                        connectionParameters.DialerServiceAddress,
                        "", false);

                    var channelFactoryWrapperFactory = new ChannelFactoryWrapperFactory<IDialerService>();

                    var channelFactoryWrapper = channelFactoryWrapperFactory.Create(dialerChannelFactoryWrapperConfiguration, _catiCommonILoggerToCodiILogger);

                    try
                    {
                        channelFactoryWrapper.Execute(x => x.Initialize(
                            1,
                            anotherDialerId,
                            configurationParameters));

                        Assert.Fail("FaultException<DialerExceptionDetail> was expected, but was not thrown.");
                    }
                    catch (FaultException<DialerExceptionDetail> ex)
                    {
                        Assert.IsTrue(ex.Message.Contains(
                            "DialerId (2) differs from the one in web.config (1)."));
                    }
                });
        }
    }
}
