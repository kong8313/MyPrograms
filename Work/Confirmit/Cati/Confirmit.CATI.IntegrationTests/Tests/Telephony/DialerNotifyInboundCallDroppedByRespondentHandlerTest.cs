using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class DialerNotifyInboundCallDroppedByRespondentHandlerTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void Handler_should_do_nothing_if_no_active_dial()
        {
            // arrange
            var inboundCallId = Guid.NewGuid().ToString();
            ServiceLocator.RegisterInstance<IToggleSettings>(new StubIToggleSettings
            {
                EnableInboundGet = () => false
            });

            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1" } },

            }.Create();

            // act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyDropInboundCall(inboundCallId);
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundCallId == inboundCallId);

            // assert
            Assert.IsNull(inboundCallsHistory);
        }
        
        [TestMethod]
        public void Handler_should_createFullHistory_if_interview_exists_in_predictive()
        {
            // arrange
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var inboundCallId = Guid.NewGuid().ToString();

            var timeMocker = new DateTimeMocker("2015-03-13T08:00:00");

            ServiceLocator.RegisterInstance<IToggleSettings>(new StubIToggleSettings
            {
                EnableInboundGet = () => true,
                EnableDesktopConsoleLoginGet = () => true
            });

            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, Assigns = new []{"P1"}, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData(), TelephoneNumber = callerNumber },
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new []{ new ScriptData(){Tag="SS1", 
                    Script = new TestScript( new Action(Action.Operation.AcceptInboundCall, string.Empty), new Shift(1, 1, "0.00:00:00", "6.00:00:00"))} },
                Dialers = new[] { new DialerData { Tag = "D1" } },

            }.Create();

            // act
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, inboundCallId);
            timeMocker.AddTime(TimeSpan.FromSeconds(30));
            dialer.SendNotifyDropInboundCall(inboundCallId);

            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().Where(x => x.InboundCallId == inboundCallId);
            var interview = context.GetInterview("S1.I1");

            const InboundHandlerOperationType expectedOperationType = InboundHandlerOperationType.DropByRespondent;

            var history =
                inboundCallsHistory.FirstOrDefault(x => (InboundHandlerOperationType)x.OperationType == expectedOperationType);

            // assert
            Assert.IsNotNull(history,
                string.Format("Inbound call history collection is expected to contain a record with [{0}] operation type." +
                " Actual collection: [{1}]",
                expectedOperationType,
                string.Join(", ", inboundCallsHistory.Select(x => (InboundHandlerOperationType)x.OperationType))));

            var dialHistory = BvDialHistoryAdapter.GetAll();
            Assert.AreEqual(1, dialHistory.Count, "Wrong amount of dial history records");

            Assert.AreEqual(interview.Id, history.InterviewId);
            Assert.AreEqual(interview.Survey.Id, history.SurveyId);
            Assert.AreEqual((CallOutcome)interview.Model.TransientState, CallOutcome.DroppedByRespondent);

            Assert.AreEqual(
                BackendTools.Format(BvCallHistoryExAdapter.GetAll()),
                BackendTools.Format(context, @"
 Id          FiredTime ApptID ShiftTypeID InterviewID SurveyId  ITS DialingMode CallState Priority         TimeInShift         ExpireTime ExplicitSID ExplicitType CellId OperationId OperationType CallCenterId BlockedByFcd DialTypeId
  1 3/13/2015 08:00:00      0 -2147483648           1       36 1000           0        -2        1 12/30/1899 00:00:00 1/01/9999 00:00:00          36            1      0           0            36            0        False          0
  2 3/13/2015 08:00:30      0 -2147483648           1       36 1001           0         0        1 12/30/1899 00:00:00 1/01/9999 00:00:00          36            1      0           0            38            0        False          0"));
        }

        [TestMethod]
        public void Handler_should_createFullHistory_if_interview_exists_in_non_predictive()
        {
            // arrange
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var inboundCallId = Guid.NewGuid().ToString();

            var startTime = DateTime.Parse("2015-03-13T08:00:00");
            var timeMocker = new DateTimeMocker(startTime);

            ServiceLocator.RegisterInstance<IToggleSettings>(new StubIToggleSettings
            {
                EnableInboundGet = () => true,
                EnableDesktopConsoleLoginGet = () => true
            });

            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                        SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData(), TelephoneNumber = callerNumber },
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[]{ new ScriptData(){Tag="SS1", 
                    Script = new TestScript( new Action(Action.Operation.AcceptInboundCall, string.Empty), new Shift(1, 1, "0.00:00:00", "6.00:00:00"))} },
                Dialers = new[] { new DialerData { Tag = "D1" } },

            }.Create();

            // act
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, inboundCallId);
            timeMocker.AddTime(TimeSpan.FromSeconds(30));
            dialer.SendNotifyDropInboundCall(inboundCallId);

            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().Where(x => x.InboundCallId == inboundCallId);
            var interview = context.GetInterview("S1.I1");

            const InboundHandlerOperationType expectedOperationType = InboundHandlerOperationType.DropByRespondent;

            var history =
                inboundCallsHistory.FirstOrDefault(x => (InboundHandlerOperationType)x.OperationType == expectedOperationType);

            // assert
            Assert.IsNotNull(history,
                string.Format("Inbound call history collection is expected to contain a record with [{0}] operation type." +
                              " Actual collection: [{1}]",
                    expectedOperationType,
                    string.Join(", ", inboundCallsHistory.Select(x => (InboundHandlerOperationType)x.OperationType))));

            var dialHistory = BvDialHistoryAdapter.GetAll();
            Assert.AreEqual(1, dialHistory.Count, "Wrong amount of dial hsitory record");
            Assert.AreEqual(startTime, dialHistory[0].StartTime, "Wrong StartTime of dial hsitory record");
            Assert.IsNull(dialHistory[0].AnswerTime, "Wrong AnswerTime if dial hsitory record");
            Assert.AreEqual(startTime.AddSeconds(30), dialHistory[0].FinishTime, "Wrong FinishTime of dial hsitory record");

            Assert.AreEqual(interview.Id, history.InterviewId);
            Assert.AreEqual(interview.Survey.Id, history.SurveyId);

            Assert.AreEqual(
                BackendTools.Format(BvCallHistoryExAdapter.GetAll()),
                BackendTools.Format(context, @"
 Id          FiredTime ApptID ShiftTypeID InterviewID SurveyId  ITS DialingMode CallState Priority         TimeInShift         ExpireTime ExplicitSID ExplicitType CellId OperationId OperationType CallCenterId BlockedByFcd DialTypeId
  1 3/13/2015 08:00:00      0 -2147483648           1       36 1000           0        -1        1 12/30/1899 00:00:00 1/01/9999 00:00:00          36            1      0           0            36            0        False          0
  2 3/13/2015 08:00:30      0 -2147483648           1       36 1001           0         0        1 12/30/1899 00:00:00 1/01/9999 00:00:00          36            1      0           0            38            0        False          0"));
        }
    }
}