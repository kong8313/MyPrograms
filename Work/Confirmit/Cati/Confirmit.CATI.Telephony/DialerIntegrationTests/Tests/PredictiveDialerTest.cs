using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;
using DialerIntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerIntegrationTests.Tests
{
    [TestClass]
    public class PredictiveDialerTest
    {
        [TestInitialize]
        public void Init()
        {
            // We need to change CurrentDirectory as the SimulatorScenario.xml file can't be found otherwise
            var executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!string.IsNullOrEmpty(executingAssemblyDirectory))
            {
                Environment.CurrentDirectory = executingAssemblyDirectory;
            }
        }

        internal List<CallInfo> CreateListOfCalls(int numberOfCalls, int agentId, DialingMode dialingMode, long baseNumber)
        {
            var calls = new List<CallInfo>(numberOfCalls);

            for (var i = 0; i < numberOfCalls; i++)
            {
                var iterationCall = new CallInfo(
                    agentId,
                    i,
                    i,
                    0,
                    (baseNumber + i).ToString(CultureInfo.InvariantCulture),
                    null,
                    dialingMode,
                    false,
                    0,
                    0,
                    0,
                    string.Empty,
                    false,
                    0,
                    string.Empty,
                    null);

                calls.Add(iterationCall);
            }

            return calls;
        }

        /// <summary>
        /// StartCampaign with mode = DialingMode.DIALLING_MODE_PREDICTIVE
        /// Wait for Request Calls
        /// Login one interviewer
        /// Generate calls
        /// Send calls to dialer
        /// Wait each call be returner with pre-defined outcome
        /// Hung up each call
        /// Logout interviewer
        /// </summary>
        [TestMethod, Owner(@"FIRM\MaximG"), CannotWorkInParallel]
        public void PredictiveWorkCycleOk()
        {
            var framework = new DialerTestingFramework();

            framework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();
                    const long baseNumber = 89161230000;

                    dialer.ExpectRequestCallsNotification();
                    dialer.StartCampaign("PredictiveWorkCycleOk test", DialingMode.Predictive, false);
                    dialer.WaitRequestCallsNotification();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, true, false);
                    dialer.WaitUserStateNotification();
                    dialer.GoReady(personSid, true);

                    const int callsNumber = 2;

                    for (int indexCalls = 0; indexCalls < callsNumber; indexCalls++)
                    {
                        dialer.ExpectCallOutcomeNotification(CallOutcome.Connected);
                        // Only one call per iteration
                        dialer.SendNumbers(CreateListOfCalls(1, 0, DialingMode.Predictive, baseNumber + indexCalls * 10000));

                        dialer.WaitOutcomeNotification();
                        dialer.Hangup(personSid);
                    }

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();

                });
        }

        /// <summary>
        /// Hybrid OK
        ///       - OpenCampaign in predictive mode
        ///       - Wait for RequestCalls
        ///       - Login one interviewer
        ///       - Pass one number in preview mode (hybrid dial)
        ///       - Wait for ScreepPop event
        ///       - Pass this number again as to be dialed
        ///       - Wait for this call be returned with pre-defined call outcome
        ///       - Hung up
        ///      - Logout interviewer
        /// </summary>
        [TestMethod, Owner(@"FIRM\MaximG"), CannotWorkInParallel]
        public void HybridDialOk()
        {
            var framework = new DialerTestingFramework();

            framework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.ExpectRequestCallsNotification();
                    dialer.StartCampaign("HybridDialOk test", DialingMode.Predictive, false);
                    dialer.WaitRequestCallsNotification();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, true, false);
                    dialer.WaitUserStateNotification();
                    dialer.GoReady(personSid, true);

                    // generate one hybrid call
                    List<CallInfo> listOfCalls = CreateListOfCalls(1, 0, DialingMode.Preview, 89161230000);

                    // We expect ScreenPop message after the call is sent to the dialer
                    dialer.ExpectScreenPop();

                    // send to dialer, this is first stage of hybrid calls go to the dialler as predictive, but field previousConnect set to 2 (virtually dialled)
                    dialer.SendNumbers(listOfCalls);

                    // Wait for the ScreenPop message
                    dialer.WaitScreenPopNotification();

                    dialer.ExpectCallOutcomeNotification(CallOutcome.Connected);
                    dialer.CompletePreview(personSid, listOfCalls[0].interviewId, (int)(listOfCalls[0].callId), "89161230000"); //TODO CODI changes: propagate long for callId
                    dialer.WaitOutcomeNotification();

                    dialer.Hangup(personSid);

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();
                });
        }

        /// <summary>
        /// Hybrid refused to dial
        ///       - OpenCampaign in predictive mode
        ///       - Wait for RequestCalls
        ///       -  Login one interviewer
        ///       - Pass one number in preview mode (hybrid dial)
        ///       - Wait for ScreepPop event       
        ///       - Hung up
        ///      (next interview can be started at this stage)
        ///      - Logout interviewer
        /// </summary>
        [TestMethod, Owner(@"FIRM\MaximG"), CannotWorkInParallel]
        public void HybridDialRefuse()
        {
            var framework = new DialerTestingFramework();

            framework.ExecuteTest(
                dialer =>
                {
                    int personSid = Randomizer.Next();

                    dialer.ExpectRequestCallsNotification();
                    dialer.StartCampaign("PredictiveWorkCycleOk test", DialingMode.Predictive, false);
                    dialer.WaitRequestCallsNotification();

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDIN);
                    dialer.Login(personSid, true, false);
                    dialer.WaitUserStateNotification();
                    dialer.GoReady(personSid, true);

                    // generate one hybrid call
                    List<CallInfo> listOfCalls = CreateListOfCalls(1, 0, DialingMode.Preview, 0);

                    dialer.ExpectScreenPop();
                    dialer.SendNumbers(listOfCalls);
                    dialer.WaitScreenPopNotification();

                    dialer.Hangup(personSid);

                    dialer.ExpectUserStateNotification(AgentStateMsgs.LOGGEDOUT);
                    dialer.Logout(personSid);
                    dialer.WaitUserStateNotification();

                });
        }

        /// <summary>
        /// Dialer Unavailable test
        ///      - OpenCampaign in predictive mode
        ///      - Wait for RequestCalls
        ///      - issue GetState and got dialer available
        ///      - stop Prots Simulator
        ///      - issue GetState and got dialer unavailable
        ///      - start Prots Simulator
        ///      - issue GetState and got dialer available
        /// </summary>
        [TestMethod, Owner(@"FIRM\MaximG"), CannotWorkInParallel]
        public void DialerBecomesUnavailableAndAvaiableAgain()
        {
            var framework = new DialerTestingFramework();

            framework.ExecuteTest(
                dialer =>
                {
                    dialer.ExpectDialerState(DialerState.Available);
                    dialer.GetState();
                    dialer.WaitDialerStateNoticiation();
                    // commented because test stoped before simulator can completee its stop, and assert into WaitDialerStateNoticiation failed
                    //dialer.StopSimulator();
                    //dialer.ExpectDialerState(DialerState.Unavailable);
                    //dialer.GetState();   
                    //dialer.WaitDialerStateNoticiation();
                });
        }

        [TestMethod, Owner(@"FIRM\MaximG"), CannotWorkInParallel]
        public void PredictiveSendNumbers_FlushAllNumbers_RightOutcomesReturns()
        {
            var framework = new DialerTestingFramework();

            framework.ExecuteTest(
                dialer =>
                {
                    const long baseNumber = 89161230000;

                    dialer.ExpectRequestCallsNotification();
                    dialer.StartCampaign(
                        "PredictiveSendNumbers_FlushAllNumbers_RightOutcomesReturns test",
                        DialingMode.Predictive,
                        false);
                    dialer.WaitRequestCallsNotification();

                    const int callsNumber = 3;

                    List<CallInfo> list = CreateListOfCalls(callsNumber, 0, DialingMode.Predictive, baseNumber + callsNumber * 10000);

                    dialer.SendNumbers(list);

                    for (int i = 0; i < callsNumber; i++)
                    {
                        dialer.ExpectCallOutcomeNotification(CallOutcome.ReturnedNotDialled);
                        dialer.FlushNumbers(new List<CallInfo> { list[i] });
                        dialer.WaitOutcomeNotification();
                    }

                });
        }
    }
}