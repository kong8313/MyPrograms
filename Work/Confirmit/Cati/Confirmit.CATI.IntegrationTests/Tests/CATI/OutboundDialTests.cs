using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class OutboundDialTests : BaseMockedIntegrationTest
    {
        private string TelNumber = "7777777";
        private string ExtNumber = "1111111";
        private string ProjectId = "p12321";

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveSurvey_CallSendToDialer_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){ Tag="S1.I1", TelephoneNumber = TelNumber, ExtensionNumber = ExtNumber, Call = new CallData() }}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var predictive = context.GetDialer("D1").Predictive("S1");

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer();
            predictive.Request();

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wrong count of active dial");
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetCall("S1.I1").Assert.AreEqual((int)CallState.LoadedToDialerPredictively, x => x.CallState, "Wrong call state");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticSurvey_CallSendToDialer_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", ProjectId = ProjectId, DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                        Interviews = new[]{new InterviewData(){ Tag="S1.I1", TelephoneNumber = TelNumber, ExtensionNumber = ExtNumber, Call = new CallData() }}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init();

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start();

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");

            Assert.AreEqual(GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime AnswerTime InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
   Dialing Outbound  1    0 {    D1}               1111111                   7777777 3/23/2018 14:00:00     <NULL>        <NULL>     <NULL> {           S1}     3 {    S1}      12321 {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetCall("S1.I1").Assert.AreEqual((int)CallState.InterviewInProgress, x => x.CallState, "Wrong call state");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveSurvey_CallSentToDialerAndNotConnected_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Predictive, Assigns = new []{"P1"}, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[]{new InterviewData(){ Tag="S1.I1", TelephoneNumber = TelNumber, ExtensionNumber = ExtNumber, Call = new CallData() }}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var predictive = context.GetDialer("D1").Predictive("S1");

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer();
            predictive.Request();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            predictive.Busy("S1.I1");

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wronk count of active dial");
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetInterview("S1.I1").Assert.AreEqual((int)(CallOutcome.Busy), x => x.TransientState, "Wrong interview ITS");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticSurvey_CallSentToDialerAndNotConnected_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Automatic, Assigns = new []{"P1"}, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[]{new InterviewData(){ Tag="S1.I1", TelephoneNumber = TelNumber, ExtensionNumber = ExtNumber, Call = new CallData() }}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
            }.Create();

            var dialer = context.GetDialer("D1");
            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init();

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            dialer.Busy(sendNumberToAgentParams.Single());

            console.Wait();

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Wronk count of active dial");
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(
                BackendTools.Format(BvDialHistoryAdapter.GetAll()),
                BackendTools.Format(context, @"
 ID Type DialerId InitialSurveyId DialerTelephoneNumber RespondentTelephoneNumber InboundCallId CallCompleteStatus          StartTime AnswerTime         FinishTime JsonCallOutcomeMetadata RingTime DialerCallerId DialerCallOutcome
  1    0        1              36               1111111                   7777777        <NULL>                  1 3/23/2018 14:00:00     <NULL> 3/23/2018 14:00:30                  <NULL>   <NULL>         <NULL>            <NULL>"));

            context.GetInterview("S1.I1").Assert.AreEqual((int)(CallOutcome.Busy), x => x.TransientState, "Wrong interview ITS");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveSurvey_CallSentToDialerAndConnect_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData()
            {
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", IsUseDb = true, DialMode = DialingMode.Predictive, Assigns = new []{"P1"}, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[]{new InterviewData(){ Tag="S1.I1", TelephoneNumber = TelNumber, ExtensionNumber = ExtNumber, Call = new CallData() }}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
                SystemSettings = new Dictionary<string, object>() {
                    { SystemSettingConstants.Dialer.RespondentVariablesToSend, "TimeZoneId,Gender" }
                }
            }.Create();
            
            var predictive = context.GetDialer("D1").Predictive("S1");

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer();
            predictive.Request();

            var respondentVariables = predictive.Calls[0].respondentVariables;            
            Assert.AreEqual(1, respondentVariables.Count);
            Assert.AreEqual(0, respondentVariables["TimeZoneId"]);
            
            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            predictive.Connect("S1.I1", console);
            
            var campaignId = context.Surveys[0].Model.CampaignId;
            
            Assert.AreEqual(GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected Outbound  1    0 {    D1}               1111111                   7777777 3/23/2018 14:00:30 3/23/2018 14:00:30        <NULL>     <NULL> {           S1}     1 {    S1} "+campaignId+" {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetCall("S1.I1").Assert.AreEqual((int)CallState.InterviewInProgress, x => x.CallState, "Wrong call state");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticSurvey_CallSentToDialerAndConnect_DialHistoryAreCorrect()
        {
            var timeMocker = new DateTimeMocker(DateTime.Parse("2018-03-23T14:00:00"));

            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() {
                        Tag = "S1", IsUseDb = true, DialMode = DialingMode.Automatic, Assigns = new[] { "P1" }, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] { new InterviewData() { Tag = "S1.I1", TelephoneNumber = TelNumber, ExtensionNumber = ExtNumber, Call = new CallData() } }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
                SystemSettings = new Dictionary<string, object>() {
                    { SystemSettingConstants.Dialer.RespondentVariablesToSend, "CallAttemptCount, RespondentName, Gender" }
                }
            }.Create();

            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init();

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start();

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            dialer.Connect(sendNumberToAgentParams.Single());

            console.Wait();
            
            var campaignId = context.Surveys[0].Model.CampaignId;
            
            Assert.AreEqual(GetAllActiveDial(),
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected Outbound  1    0 {    D1}               1111111                   7777777 3/23/2018 14:00:00 3/23/2018 14:00:30        <NULL>     <NULL> {           S1}     1 {    S1} "+campaignId+" {    S1.I1}      1 {        P1}"));
            Assert.AreEqual(0, BvDialHistoryToInterviewHistoryAdapter.GetAll().Count, "Wrong count of dial history to interview history relations");
            Assert.AreEqual(0, BvDialHistoryAdapter.GetAll().Count, "Wrong count of history dials");

            context.GetCall("S1.I1").Assert.AreEqual((int)CallState.InterviewInProgress, x => x.CallState, "Wrong call state");
            var respondentVariables = sendNumberToAgentParams[0].RespondentVariables;
            Assert.AreEqual(2, respondentVariables.Count);
            Assert.AreEqual("respName", respondentVariables["RespondentName"]);
            Assert.AreEqual(0, respondentVariables["CallAttemptCount"]);
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