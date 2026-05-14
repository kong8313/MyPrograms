using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class InboundDialTests : BaseMockedIntegrationTest
    {
        private string DdiNumber = "7777777";
        private string CliNumber = "1111111";
        private string InboundCallId = "InboundCallId7171";
        private string ProjectId = "p12321";
        private string ProjectId2 = "p23432";

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCall_CallIsDropedByFeatureDissabled_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var dialer = context.GetDialer("D1");

            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(
                BackendTools.Format(dropInboundCallParams.Select(x => new { x.InboundCallId, x.AudioMessageDescriptor })),
                BackendTools.Format(context, @"
     InboundCallId AudioMessageDescriptor
 InboundCallId7171                 <NULL>"));

            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1}               0               7777777                   1111111 InboundCallId7171                  2 3/23/2018 14:00:00     <NULL> 3/23/2018 14:00:00                  <NULL>   <NULL>         <NULL>            <NULL>"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCall_CallIsDropedBySchedulingScript_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");

            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(
                BackendTools.Format(dropInboundCallParams.Select(x => new { x.InboundCallId, x.AudioMessageDescriptor })),
                BackendTools.Format(context, @"
     InboundCallId AudioMessageDescriptor
 InboundCallId7171                 <NULL>"));

            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  2 3/23/2018 14:00:00     <NULL> 3/23/2018 14:00:00                  <NULL>   <NULL>         <NULL>            <NULL>"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.InboundCall);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredictiveSurvey_CallSendToDialer_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();
            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            console.StartInterview();
            dialer.RequestCalls(survey, 3, CallsSelectionAlgorithm.CallsAssignedToCampaignOnly);

            Assert.AreEqual(0, dropInboundCallParams.Count, "Wrong count of DropInboundCall calls");
            Assert.AreEqual(
                BackendTools.Format(connectInboundCallParams.Select(x => new { x.InboundCallId, x.AudioMessageDescriptor })),
                BackendTools.Format(context, @"
     InboundCallId AudioMessageDescriptor
 InboundCallId7171                 <NULL>"));

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
  Queueing  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00     <NULL> InboundCallId7171     <NULL> {           S1}     5 {    S1}      12321 {    S1.I1}      1            0"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.InboundCall);
            var call = context.GetCall("S1.I1");
            call.Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForAutomaticSurvey_CallIsPlacedInQueue_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();
            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count, "Wrong count of ConnectInboundCall calls");
            Assert.AreEqual(0, dropInboundCallParams.Count, "Wrong count of DropInboundCall calls");

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
   Pending  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00     <NULL> InboundCallId7171     <NULL> {           S1}     2 {    S1}      12321 {    S1.I1}      1            0"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.InboundCall);
            var call = context.GetCall("S1.I1");
            call.Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPreviewSurvey_CallIsPlacedInQueue_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();
            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count, "Wrong count of ConnectInboundCall calls");
            Assert.AreEqual(0, dropInboundCallParams.Count, "Wrong count of DropInboundCall calls");

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
   Pending  Inbound  1    1     {D1}               7777777                   1111111 3/23/2018 14:00:00     <NULL> InboundCallId7171     <NULL> {           S1}     2 {    S1}      12321 {    S1.I1}      1            0"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.InboundCall);
            var call = context.GetCall("S1.I1");
            call.Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPreviewSurvey_CallIsPlacedInQueue_DialHistoryAreCorrectAndPreviousHangedUpActiveDialIsLoggedToHisotry()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            ServiceLocator.Resolve<IActiveDialService>().CreateInboundCall(dialer.Id, InboundCallId, DdiNumber, CliNumber);

            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();
            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count, "Wrong count of ConnectInboundCall calls");
            Assert.AreEqual(0, dropInboundCallParams.Count, "Wrong count of DropInboundCall calls");

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
   Pending  Inbound  2    1        1               7777777                   1111111 3/23/2018 14:00:00     <NULL> InboundCallId7171     <NULL> {           S1}     2       36      12321           1      1            0"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1     {D1}            {S1}               7777777                   1111111 InboundCallId7171                  0 3/23/2018 14:00:00     <NULL> 3/23/2018 14:00:00                  <NULL>   <NULL>         <NULL>            <NULL>"));

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.InboundCall);
            var call = context.GetCall("S1.I1");
            call.Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void ReceiveInboundCallForPreviewSurvey_GetCatiDialingAttempts_DdiNumberIsCorrect()
        {
            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object> { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData { Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new [] { "P1" },
                        Interviews = new[] { new InterviewData { Tag="S1.I1", TelephoneNumber = CliNumber } },
                        InboundTelephoneNumbers = new [] { new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = DdiNumber } }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            
            ServiceLocator.Resolve<IActiveDialService>().CreateInboundCall(dialer.Id, InboundCallId, DdiNumber, CliNumber);

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();
            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);
            console.StartInterview();
            
            var dialingAttempts = new ManagementService().GetCatiInterviewDialingAttempts(ProjectId, interview.Id);
            Assert.AreEqual(1, dialingAttempts.Length);
            Assert.AreEqual(DdiNumber, dialingAttempts[0].DialerTelephoneNumber);
        }
        
        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_CallIsDeliveredToConsole_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.WaitInterview(connectInboundCallParams.Single().CallInfo);

            Assert.AreEqual("S1.I1", interview.Tag, "Wrong interview");

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00 3/23/2018 14:00:30 InboundCallId7171     <NULL> {           S1}     1 {    S1}      12321 {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_AcceptWithoutAssignmentAndOutOfShift_ConnectInboundCallAreCorrect()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Second) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);

            var callInfo = connectInboundCallParams[0].CallInfo;

            Assert.AreEqual(0, callInfo.agentGroupId, "Wrong group id in ConnectInboundCall call.");
            Assert.AreEqual(0, callInfo.agentId, "Wrong agent id in ConnectInboundCall call.");
            CollectionAssert.AreEqual(new[] { survey.Model.CampaignId }, connectInboundCallParams[0].CampaignIdsToBorrowAgentsFrom, "Wrong list of CampaignIdsToBorrowAgentsFrom in ConnectInboundCall call.");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_AcceptWithAssignmentOnPersonGroup_ConnectInboundCallAreCorrect()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1"} },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1" } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{PG1}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);

            var callInfo = connectInboundCallParams[0].CallInfo;

            Assert.AreEqual(context.GetPersonGroup("PG1").Id, callInfo.agentGroupId, "Wrong group id in ConnectInboundCall call.");
            Assert.AreEqual(0, callInfo.agentId, "Wrong agent id in ConnectInboundCall call.");
            CollectionAssert.AreEqual(new[] { survey.Model.CampaignId }, connectInboundCallParams[0].CampaignIdsToBorrowAgentsFrom, "Wrong list of CampaignIdsToBorrowAgentsFrom in ConnectInboundCall call.");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_AcceptWithAssignmentOnPersonGroupWithCrossSurveyOption_ConnectInboundCallAreCorrect()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1"} },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", InboundBehavior = InboundGroupBehavior.DeliverCallsFromOtherSurvey } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{PG1}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);

            var callInfo = connectInboundCallParams[0].CallInfo;

            Assert.AreEqual(context.GetPersonGroup("PG1").Id, callInfo.agentGroupId, "Wrong group id in ConnectInboundCall call.");
            Assert.AreEqual(0, callInfo.agentId, "Wrong agent id in ConnectInboundCall call.");
            Assert.AreEqual(null, connectInboundCallParams[0].CampaignIdsToBorrowAgentsFrom, "Wrong list of CampaignIdsToBorrowAgentsFrom in ConnectInboundCall call.");
        }
        
        [TestMethod, Owner(@"Firm\EgorK")]
        public void ReceiveInboundCallForPredicitveSurvey_AssignmentOnPersonGroup_NoInterviewersAvailable_CallNotConnected()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment} },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", InboundBehavior = InboundGroupBehavior.DeliverCallsFromOtherSurvey } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{PG1}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var droppedInboundCalls = dialer.Behavior.Methods.DropInboundCall.Init();
            
            var console = new PredictiveConsoleController(context, person, survey, dialer);//not member of assigned group
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count);
            Assert.AreEqual(1, droppedInboundCalls.Count);
        }


        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_AcceptWithAssignmentOnMultiGroupWithoutCrossSurveyOption_ConnectInboundCallAreCorrect()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG2"} },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG1"},
                    new PersonGroupData() { Tag = "PG2"}
                },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{PG1}, {PG2}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);

            var callInfo = connectInboundCallParams[0].CallInfo;
            var assignmentId = ServiceLocator.Resolve<IAssignmentService>().GetAssignmentResourceId(
                context.GetResources("PG1", "PG2").Select(x => x.Id).ToArray());

            Assert.AreEqual(assignmentId, callInfo.agentGroupId, "Wrong group id in ConnectInboundCall call.");
            Assert.AreEqual(0, callInfo.agentId, "Wrong agent id in ConnectInboundCall call.");
            CollectionAssert.AreEqual(new[] { survey.Model.CampaignId }, connectInboundCallParams[0].CampaignIdsToBorrowAgentsFrom, "Wrong list of CampaignIdsToBorrowAgentsFrom in ConnectInboundCall call.");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_AcceptWithAssignmentOnMultiGroupWithCrossSurveyOption_ConnectInboundCallAreCorrect()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1"} },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG1"},
                    new PersonGroupData() { Tag = "PG2", InboundBehavior = InboundGroupBehavior.DeliverCallsFromOtherSurvey}
                },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{PG1}, {PG2}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);

            var callInfo = connectInboundCallParams[0].CallInfo;
            var assignmentId = ServiceLocator.Resolve<IAssignmentService>().GetAssignmentResourceId(
                context.GetResources("PG1", "PG2").Select(x => x.Id).ToArray());

            Assert.AreEqual(assignmentId, callInfo.agentGroupId, "Wrong group id in ConnectInboundCall call.");
            Assert.AreEqual(0, callInfo.agentId, "Wrong agent id in ConnectInboundCall call.");
            Assert.AreEqual(null, connectInboundCallParams[0].CampaignIdsToBorrowAgentsFrom, "Wrong list of CampaignIdsToBorrowAgentsFrom in ConnectInboundCall call.");
        }
        
        [TestMethod, Owner(@"Firm\EgorK")]
        public void ReceiveInboundCallForPredicitveSurvey_AssignmentOnMultiGroup_NoInterviewersAvailable_CallNotConnected()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment} },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG1"},
                    new PersonGroupData() { Tag = "PG2", InboundBehavior = InboundGroupBehavior.DeliverCallsFromOtherSurvey}
                },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{PG1}, {PG2}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var droppedInboundCalls = dialer.Behavior.Methods.DropInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();//not member of the assigned groups

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count);
            Assert.AreEqual(1, droppedInboundCalls.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_AcceptWithAssignmentOnPerson_ConnectInboundCallAreCorrect()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{P1}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);

            var callInfo = connectInboundCallParams[0].CallInfo;

            Assert.AreEqual(0, callInfo.agentGroupId, "Wrong group id in ConnectInboundCall call.");
            Assert.AreEqual(context.GetPerson("P1").Id, callInfo.agentId, "Wrong agent id in ConnectInboundCall call.");
            Assert.AreEqual(null, connectInboundCallParams[0].CampaignIdsToBorrowAgentsFrom, "Wrong list of CampaignIdsToBorrowAgentsFrom in ConnectInboundCall call.");
        }
        
        [TestMethod, Owner(@"Firm\EgorK")]
        public void ReceiveInboundCallForPredicitveSurvey_AssignmentOnPerson_PersonIsNotAvailable_CallNotConnected()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber }},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment },  new PersonData() { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(
                    new[]
                    {
                        new Action(Action.Operation.AcceptInboundCall),
                        new Action(Action.Operation.AssignResource, "{P2}")
                    }, Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var droppedInboundCalls = dialer.Behavior.Methods.DropInboundCall.Init();
            
            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();//wrong person logged in

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count);
            Assert.AreEqual(1, droppedInboundCalls.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForAutomaticSurvey_CallIsDeliveredToConsole_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData() {
                SystemSettings = new Dictionary<string, object>() {
                    { SystemSettingConstants.Toggle.EnableInbound, "True" }, 
                    { SystemSettingConstants.Dialer.RespondentVariablesToSend, "RespondentName" } 
                },
                Surveys = new[] {
                    new SurveyData() {
                        Tag = "S1", IsUseDb = true, SchedulingScript = "SS1", DialMode = DialingMode.Automatic, Assigns = new[] { "P1" },
                        Interviews = new[] { new InterviewData() { Tag = "S1.I1", TelephoneNumber = CliNumber, Call = new CallData() } },
                        InboundTelephoneNumbers = new[] { new InboundTelephoneNumberData() { Dialer = "D1", TelephoneNumber = DdiNumber } }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.StartInterview();

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);

            Assert.AreEqual("S1.I1", interview.Tag, "Wrong interview");

            var respondentVariables = connectInboundCallToAgentParams[0].CallInfo.respondentVariables;

            Assert.AreEqual(1, respondentVariables.Count);
            Assert.AreEqual("respName", respondentVariables["RespondentName"]);
            
            var campaignId = context.Surveys[0].Model.CampaignId;
            
            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00 3/23/2018 14:00:30 InboundCallId7171     <NULL> {           S1}     1 {    S1} "+campaignId+" {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPreviewSurvey_CallIsDeliveredToConsole_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag, "Wrong interview");

            Assert.AreEqual(0, connectInboundCallToAgentParams.Count);

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
   Pending  Inbound  1    1     {D1}               7777777                   1111111 3/23/2018 14:00:00     <NULL> InboundCallId7171     <NULL>        {    S1}     2 {    S1}      12321           1      1 {        P1}"));

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.Dial(interview);

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00 3/23/2018 14:01:00 InboundCallId7171     <NULL> {           S1}     1 {    S1}      12321 {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveHybridInboundCallForPredictiveSurvey_CallIsDeliveredToConsole_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, DialMode = "2"}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var completePreviewParams = dialer.Behavior.Methods.CompletePreview.Init(DialerMethodBehaviors.SendOutcomeConnected);

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(0, completePreviewParams.Count);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            dialer.SendPredicitvePreviewCall(survey.Model.CampaignId, connectInboundCallParams.Single().CallInfo, person);

            var interview = console.WaitInterview();

            Assert.AreEqual("S1.I1", interview.Tag, "Wrong interview");

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(0, completePreviewParams.Count);

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
  Queueing  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00     <NULL> InboundCallId7171     <NULL> {           S1}     5 {    S1}      12321 {    S1.I1}      1 {        P1}"));

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.Dial(interview);

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(1, completePreviewParams.Count);

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00 3/23/2018 14:01:00 InboundCallId7171     <NULL> {           S1}     1 {    S1}      12321 {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");
        }
        
        
        [TestMethod, Owner(@"Firm\EgorK")]
        public void ReceiveInboundCallForPredictiveSurvey_AssignedToSurvey_NoInterviewersAvailable_CallNotConnected()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, DialMode = "2"}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}},
                    },
                    new SurveyData(){ Tag="S2", ProjectId = ProjectId2, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P2"},
                        Interviews = new[]{new InterviewData(){Tag="S2.I1", TelephoneNumber = "12345", DialMode = "2"}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = "123"}},
                        
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment }, new PersonData() { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person1 = context.GetPerson("P1");
            var survey1 = context.GetSurvey("S1");
            var person2 = context.GetPerson("P2");
            var survey2 = context.GetSurvey("S2");
            
            var console = new PredictiveConsoleController(context, person2, survey2, dialer);
            console.LoginAndStart();//wrong person in a wrong survey

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var droppedInboundCalls = dialer.Behavior.Methods.DropInboundCall.Init();
            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            Assert.AreEqual(0, connectInboundCallParams.Count);
            Assert.AreEqual(1, droppedInboundCalls.Count);
            
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveHybridInboundCallForPredictiveSurvey_CallIsDeliveredToConsoleAndTerminatedBeforeDial_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, DialMode = "2"}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var completePreviewParams = dialer.Behavior.Methods.CompletePreview.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            dialer.SendPredicitvePreviewCall(survey.Model.CampaignId, connectInboundCallParams.Single().CallInfo, person);

            var interview = console.WaitInterview();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.TerminateConsole();

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(1, killAgenParams.Count);
            Assert.AreEqual(0, completePreviewParams.Count);
            Assert.AreEqual(1, logoutParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1        1            {S1}               7777777                   1111111 InboundCallId7171                  6 3/23/2018 14:00:00     <NULL> 3/23/2018 14:00:30                  <NULL>   <NULL>         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveHybridInboundCallForPredictiveSurvey_CallIsDeliveredToConsoleAndTerminatedAfterDial_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, DialMode = "2"}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var completePreviewParams = dialer.Behavior.Methods.CompletePreview.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            dialer.SendPredicitvePreviewCall(survey.Model.CampaignId, connectInboundCallParams.Single().CallInfo, person);

            var interview = console.WaitInterview();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.Dial(interview);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.TerminateConsole();

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(1, killAgenParams.Count);
            Assert.AreEqual(1, completePreviewParams.Count);
            Assert.AreEqual(1, logoutParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  6 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:00                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPreviewSurvey_CallIsDeliveredToConsoleAndTerminatedBeforeDial_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            var interview = console.StartInterview();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.TerminateConsole();

            Assert.AreEqual(0, connectInboundCallToAgentParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(1, logoutParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  6 3/23/2018 14:00:00     <NULL> 3/23/2018 14:00:30                  <NULL>   <NULL>         <NULL>            <NULL>"));

        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPreviewSurvey_CallIsDeliveredToConsoleAndTerminatedAfterDial_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            var interview = console.StartInterview();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.Dial(interview);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.TerminateConsole();

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(1, logoutParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  6 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:00                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPreviewSurvey_CallIsDeliveredToConsoleAndCompleted_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Preview, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            var interview = console.StartInterview();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.Dial(interview);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.NextInterview(interview, null);

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(0, logoutParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  4 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:00                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredicitveSurvey_CallIsDeliveredToConsoleAndCompleted_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.WaitInterview(connectInboundCallParams.Single().CallInfo);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.FinishInterview(interview);

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(0, logoutParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  4 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:00                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForAutomaticSurvey_CallIsDeliveredToConsoleAndCompleted_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.StartInterview();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.NextInterview(interview, null);

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(0, logoutParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  4 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:00                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForAutomaticSurvey_CallIsDeliveredToConsoleAndContinuedWithLinkedInterview_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                        Interviews = new[]
                        {
                            new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()},
                            new InterviewData(){Tag="S1.I2", TelephoneNumber = CliNumber, Call = new CallData()}
                        },
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.StartInterview();
            Assert.AreEqual("S1.I1", interview.Tag);
            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.SetLinkedInterview(context.GetInterview("S1.I2"));

            interview = console.NextInterview(interview, null);
            Assert.AreEqual("S1.I2", interview.Tag);

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(0, logoutParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);

            Assert.AreEqual(
                GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00 3/23/2018 14:00:30 InboundCallId7171     <NULL> {           S1}     1 {    S1}      12321 {    S1.I2}      2 {        P1}"));
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}"));
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForAutomaticSurvey_CallIsDeliveredToConsoleAndCompleteWithLinkedInterview_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                        Interviews = new[]
                        {
                            new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()},
                            new InterviewData(){Tag="S1.I2", TelephoneNumber = CliNumber, Call = new CallData()}
                        },
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallToAgentParams = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var killAgenParams = dialer.Behavior.Methods.KillAgent.Init();
            var logoutParams = dialer.Behavior.Methods.Logout.Init();
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.StartInterview();
            Assert.AreEqual("S1.I1", interview.Tag);
            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.SetLinkedInterview(context.GetInterview("S1.I2"));

            interview = console.NextInterview(interview, null);
            Assert.AreEqual("S1.I2", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.FinishInterview(interview, null);

            Assert.AreEqual(1, connectInboundCallToAgentParams.Count);
            Assert.AreEqual(0, killAgenParams.Count);
            Assert.AreEqual(0, logoutParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}
             1                  3 3/23/2018 14:01:00 3/23/2018 14:01:30 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  4 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:30                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredictiveSurvey_CallIsDeliveredToConsoleAndContinuedWithLinkedInterview_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber, Call = new CallData()}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    },
                    new SurveyData()
                    {
                        Tag="S2",Assigns = new[]{"P1"}, ProjectId = ProjectId2,
                        Interviews = new[]{new InterviewData(){Tag="S2.I1", Call = new CallData()}},
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            console.StartInterview();
            dialer.RequestCalls(survey, 3, CallsSelectionAlgorithm.CallsAssignedToCampaignOnly);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.WaitInterview(connectInboundCallParams.Single().CallInfo);
            Assert.AreEqual("S1.I1", interview.Tag);
            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.SetLinkedInterview(context.GetInterview("S2.I1"));

            interview = console.NextInterview(interview, null);
            Assert.AreEqual("S2.I1", interview.Tag);

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(0, setCampaignParams.Count);

            var expected = BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime     InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected  Inbound  1    1 {    D1}               7777777                   1111111 3/23/2018 14:00:00 3/23/2018 14:00:30 InboundCallId7171     <NULL> {           S1}     1 {    S2}      23432 {    S2.I1}      2 {        P1}");
            var actual = GetAllActiveDial();
            Assert.AreEqual(actual, expected);

            expected = BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}");
            actual = BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll());
            Assert.AreEqual(actual, expected);

            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ReceiveInboundCallForPredictiveSurvey_CallIsDeliveredToConsoleAndCompleteWithLinkedInterview_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>() { { SystemSettingConstants.Toggle.EnableInbound, "True" } },
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S1.I1", TelephoneNumber = CliNumber}},
                        InboundTelephoneNumbers = new []{new InboundTelephoneNumberData(){Dialer = "D1", TelephoneNumber = DdiNumber}}
                    },
                    new SurveyData()
                    {
                        Tag="S2",Assigns = new[]{"P1"},
                        Interviews = new[]{new InterviewData(){Tag="S2.I1", Call = new CallData()}},
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            var interview = console.WaitInterview(connectInboundCallParams.Single().CallInfo);
            Assert.AreEqual("S1.I1", interview.Tag);
            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.SetLinkedInterview(context.GetInterview("S2.I1"));

            interview = console.NextInterview(interview);
            Assert.AreEqual("S2.I1", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.FinishInterview(interview);

            Assert.AreEqual(1, connectInboundCallParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);
            Assert.AreEqual(1, setCampaignParams.Count);

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dials");
            Assert.AreEqual(BackendTools.Format(BvDialHistoryToInterviewHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 DialHistoryId InterviewHistoryId          StartTime         FinishTime PersonId
             1                  2 3/23/2018 14:00:30 3/23/2018 14:01:00 {    P1}
             1                  3 3/23/2018 14:01:00 3/23/2018 14:01:30 {    P1}"));
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber     InboundCallId CallCompleteStatus          StartTime         AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    1 {    D1} {           S1}               7777777                   1111111 InboundCallId7171                  4 3/23/2018 14:00:00 3/23/2018 14:00:30 3/23/2018 14:01:30                  <NULL>        0         <NULL>            <NULL>"));
        }

        [TestMethod]
        public void ReceiveInboundCall_CallSendToDialer_CheckCallStateIsCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData() {
                SystemSettings = new Dictionary<string, object>() {
                    { SystemSettingConstants.Toggle.EnableInbound, "True" },
                    { SystemSettingConstants.Dialer.RespondentVariablesToSend, "RespondentName, TimezoneId" }
                },
                Surveys = new[] {
                    new SurveyData() {
                        Tag = "S1", IsUseDb = true, SchedulingScript = "SS1", DialMode = DialingMode.Predictive, Assigns = new[] { "P1" },
                        Interviews = new[] { new InterviewData() { Tag = "S1.I1", TelephoneNumber = CliNumber } },
                        InboundTelephoneNumbers = new[] { new InboundTelephoneNumberData() { Dialer = "D1", TelephoneNumber = DdiNumber } }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { new ScriptData() { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall), Shift.Week) } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();
            var connectInboundCallParams = dialer.Behavior.Methods.ConnectInboundCall.Init();

            dialer.SendNotifyInboundCall(DdiNumber, CliNumber, InboundCallId);

            console.WaitInterview(connectInboundCallParams.Single().CallInfo);
            var checkIsInbound = new ManagementService().IsInboundCall(person.Id);

            Assert.AreEqual(true, checkIsInbound);
            
            var respondentVariables = connectInboundCallParams[0].CallInfo.respondentVariables;
            Assert.AreEqual(2, respondentVariables.Count);
            Assert.AreEqual("respName", respondentVariables["RespondentName"]);
            Assert.AreEqual(0, respondentVariables["TimezoneId"]);
        }

        public string GetAllActiveDial()
        {
            return BackendTools.Format(BvActiveDialAdapter.GetAll().Select(x => new
            {
                x.DialState,
                x.CallType,
                x.Id,
                x.Type,
                x.DialerId,
                x.DialerTelephoneNumber,
                x.RespondentTelephoneNumber,
                x.StartTime,
                x.AnswerTime,
                x.InboundCallId,
                x.TransferId,
                x.InitialSurveyId,
                x.State,
                x.SurveyId,
                x.CampaignId,
                x.InterviewId,
                x.CallId,
                x.MainPersonId
            }));
        }
    }
}
