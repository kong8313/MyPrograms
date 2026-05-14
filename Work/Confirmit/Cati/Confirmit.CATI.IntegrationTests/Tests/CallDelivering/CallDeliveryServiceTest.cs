using System;
using System.Collections.Generic;
using BvCallHandlerLibrary;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class CallDeliveryServiceTest : BaseMockedIntegrationTest
    {

        private CompletedInterviewDetails details = new CompletedInterviewDetails
        {
            InterviewDuration = 10,
            Its = "13",
            Status = "Complete"
        };


        [TestMethod, Owner(@"Firm\MaximL")]
        public void LookupCallsAssignedToCampaignOnly_CallsWithExpiredTime_ResultAreCorrect()
        {
            const int expirationTimeout = 10;
            var time = DateTime.Parse("2015-03-13T08:00:00");
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(time.AddMinutes(-expirationTimeout));

            var context = new TestData
            {
                Surveys = new[] { new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", Call = new CallData {TimeToExpire = time, Resource="P1" }},
                        new InterviewData { Tag="S1.I2", Call = new CallData {TimeToExpire = time, Resource="PG1" }},
                        new InterviewData { Tag="S1.I3", Call = new CallData {TimeToExpire = time }}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            context.GetPerson("P1");
            var interview = context.GetInterview("S1.I3");

            List<GroupInfo> groups;
            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(survey.Id, 0, 0, CallsSelectionAlgorithm.CallsAssignedToCampaignOnly, 2, false, out groups);

            Assert.AreEqual(1, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(interview.Id, result[0].interviewId, "Wrong Interview Id");
            Assert.AreEqual(expirationTimeout, result[0].agingTimeout, "Wrong expiration timeout of call");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void LookupCallsByCampaign_CallsWithExpiredTime_ResultListOfCallsHasCorrectAgingTimeout()
        {
            const int expirationTimeout = 10;
            var time = DateTime.Parse("2015-03-13T08:00:00");
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(time.AddMinutes(-expirationTimeout));

            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData {TimeToExpire = time, Resource="P1"}},
                        new InterviewData {Tag="S1.I2", Call = new CallData {TimeToExpire = time, Resource="PG1"}},
                        new InterviewData {Tag="S1.I3", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            List<GroupInfo> groups;
            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(
                survey.Id,
                dialer.Id,
                0,
                CallsSelectionAlgorithm.ByCampaign,
                2,
                false,
                out groups);
            // Because explicitly assigned interview I2 is assigned on the interviewer on the break it gets skipped
            Assert.AreEqual(2, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(expirationTimeout, result[0].agingTimeout, "Wrong expiration timeout of call");
            Assert.AreEqual(DialerEventsHandler.MaxCallAgingTimeoutInMin, result[1].agingTimeout, "Wrong expiration timeout of call");
        }

        [TestMethod, Owner(@"Firm\DmitryS")]
        public void LookupCallsByCampaign_CallsNotDeliveredToInterviewerOnBreak()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1", "P2"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData {Resource="P1"}},
                        new InterviewData {Tag="S1.I2", Call = new CallData {Resource="P2"}},
                        new InterviewData {Tag="S1.I3", Call = new CallData {Resource="PG1"}},
                        new InterviewData {Tag="S1.I4", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment }, new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            var console2 = new AutomaticConsoleController(context, person2, survey, dialer);
            console2.Login();
            console2.LoginToDialer();

            var task = TaskRepository.GetByPerson(person.Id);
            task.BreakTypeId = 1;
            TaskRepository.Update(task);

            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(
                survey.Id,
                dialer.Id,
                0,
                CallsSelectionAlgorithm.ByCampaign,
                3,
                false,
                out var groups);

            Assert.AreEqual(2, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(context.GetInterview("S1.I2").Id, result[0].interviewId);
            Assert.AreEqual(context.GetInterview("S1.I4").Id, result[1].interviewId);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void LookupCallsByPersonGroup_CallsWithExpiredTime_ResultAreCorrect()
        {
            const int expirationTimeout = 10;
            var time = DateTime.Parse("2015-03-13T08:00:00");
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(time.AddMinutes(-expirationTimeout));

            var context = new TestData
            {
                Surveys = new[] { new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1",
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", Call = new CallData { TimeToExpire = time, Resource="P1" }},
                        new InterviewData { Tag="S1.I2", Call = new CallData { TimeToExpire = time, Resource="PG1" }},
                        new InterviewData { Tag="S1.I3", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var group = context.GetResource("PG1");
            var interview = context.GetInterview("S1.I2");
            List<GroupInfo> groups;
            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(survey.Id, 0, group.Id, CallsSelectionAlgorithm.ByPersonGroup, 2, false, out groups);

            Assert.AreEqual(1, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(interview.Id, result[0].interviewId, "Wrong Interview Id");
            Assert.AreEqual(expirationTimeout, result[0].agingTimeout, "Wrong expiration timeout of call");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void LookupCallsByPersonGroup_CallsWithBigExpiredTime_ResultAreCorrect()
        {
            var expirationTimeout = DialerEventsHandler.MaxCallAgingTimeoutInMin * 2;
            var time = DateTime.Parse("2015-03-13T08:00:00");
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(time.AddMinutes(-expirationTimeout));

            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1",
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", Call = new CallData { TimeToExpire = time, Resource="P1" }},
                        new InterviewData { Tag="S1.I2", Call = new CallData { TimeToExpire = time, Resource="PG1" }},
                        new InterviewData { Tag="S1.I3", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var group = context.GetResource("PG1");
            var interview = context.GetInterview("S1.I2");
            List<GroupInfo> groups;
            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(survey.Id, 0, group.Id, CallsSelectionAlgorithm.ByPersonGroup, 2, false, out groups);

            Assert.AreEqual(1, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(interview.Id, result[0].interviewId, "Wrong Interview Id");
            Assert.AreEqual(DialerEventsHandler.MaxCallAgingTimeoutInMin, result[0].agingTimeout, "Wrong expiration timeout of call");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void LookupCallsByPersonGroup_CallsWithVerySmallExpiredTime_ResultAreCorrect()
        {
            var expirationTimeout = TimeSpan.FromSeconds(10);
            var time = DateTime.Parse("2015-03-13T08:00:00");
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(time.Add(-expirationTimeout));

            var context = new TestData
            {
                Surveys = new[] { new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1",
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", Call = new CallData { TimeToExpire = time, Resource="P1" }},
                        new InterviewData { Tag="S1.I2", Call = new CallData { TimeToExpire = time, Resource="PG1" }},
                        new InterviewData { Tag="S1.I3", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var group = context.GetResource("PG1");
            var interview = context.GetInterview("S1.I2");
            List<GroupInfo> groups;
            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(survey.Id, 0, group.Id, CallsSelectionAlgorithm.ByPersonGroup, 2, false, out groups);

            Assert.AreEqual(1, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(interview.Id, result[0].interviewId, "Wrong Interview Id");
            Assert.AreEqual(1, result[0].agingTimeout, "Wrong expiration timeout of call");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void LookupCallsAssignedToAgentsExplicitly_CallsWithExpiredTime_ResultAreCorrect()
        {
            const int expirationTimeout = 10;
            var time = DateTime.Parse("2015-03-13T08:00:00");
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(time.AddMinutes(-expirationTimeout));

            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", Call = new CallData { TimeToExpire = time, Resource="P1" }},
                        new InterviewData { Tag="S1.I2", Call = new CallData { TimeToExpire = time, Resource="PG1" }},
                        new InterviewData { Tag="S1.I3", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var interview = context.GetInterview("S1.I1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            List<GroupInfo> groups;
            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(
                survey.Id,
                dialer.Id,
                person.Id,
                CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly,
                2,
                false,
                out groups
                );

            Assert.AreEqual(1, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(interview.Id, result[0].interviewId, "Wrong Interview Id");
            Assert.AreEqual(expirationTimeout, result[0].agingTimeout, "Wrong expiration timeout of call");
        }

        [TestMethod, Owner(@"Firm\DmitryS")]
        public void LookupCallsAssignedToAgentsExplicitly_ExcludeInterviewerOnBreak()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1",
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", Call = new CallData {Resource="P1" }},
                        new InterviewData { Tag="S1.I2", Call = new CallData {Resource="P2" }},
                        new InterviewData { Tag="S1.I3", Call = new CallData {Resource="PG1" }},
                        new InterviewData { Tag="S1.I4", Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment }, new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                PersonGroups = new[] { new PersonGroupData { Tag = "PG1" } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var interview2 = context.GetInterview("S1.I2");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            var console2 = new AutomaticConsoleController(context, person2, survey, dialer);
            console2.Login();
            console2.LoginToDialer();

            var task = TaskRepository.GetByPerson(person.Id);
            task.BreakTypeId = 1;
            TaskRepository.Update(task);

            var result = ServiceLocator.Resolve<ICallDeliveryService>().LookupCalls(
                survey.Id,
                dialer.Id,
                person.Id,
                CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly,
                4,
                false,
                out var groups
                );

            // Skip call assigned to the interviewer on the break
            Assert.AreEqual(1, result.Count, "Wrong count of delivered calls");
            Assert.AreEqual(interview2.Id, result[0].interviewId, "Wrong Interview Id");
        }

        [TestMethod]
        public void CallDeliveryInAutomaticSurvey_FirstCallDeliveryWithNotConnected_CallsHaveCorrectITSs()
        {
            BackendToolsObject.LaunchAllHoursScript();

            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Automatic, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData() {Tag="S1.I1", Call = new CallData()},
                        new InterviewData() {Tag="S1.I2", Call = new CallData()},
                        new InterviewData() {Tag="S1.I3", Call = new CallData()},
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomes(CallOutcome.NoReply, CallOutcome.Connected, CallOutcome.Connected);

            var interview = console.StartInterview();
            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);


            interview = console.NextInterview(interview, details);
            Assert.AreEqual(context.GetInterview("S1.I3").Id, interview.Id);

            interview = console.NextInterview(interview, details);
            Assert.IsNull(interview);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.NoReply);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Completed);
            context.GetInterview("S1.I3").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Completed);
        }


        [TestMethod, Ignore]
        //TODO : need to review how we use ITimeService when we pass it as interface. this test is faling now. One way to fix it - use ServiceLocator.Resolve inline
        public void CallDeliveryInAutomaticSurvey_NoDialer_CallDelieveryTimeSetCorrecty()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true, SchedulingScript = AllHoursSchedule.Name, DialMode = DialingMode.Automatic, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData() {Tag="S1.I1", Call = new CallData()},
                        new InterviewData() {Tag="S1.I2", Call = new CallData()},
                        new InterviewData() {Tag="S1.I3", Call = new CallData()},
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();

            var interview = console.StartInterview();

            var date = new DateTime(2017, 12, 13, 14, 15, 16);

            new DateTimeMocker(TestingFramework).MockDate(date);

            console.NextInterview(interview, details);

            Assert.AreEqual(TaskRepository.GetByPerson(person.Id).TimeCallDelivered, date);
        }

        [TestMethod]
        public void CallDeliveryInAutomaticSurvey_SecondCallDeliveryWithNotConnected_CallsHaveCorrectITSs()
        {
            BackendToolsObject.LaunchAllHoursScript();

            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Automatic, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData() {Tag="S1.I1", Call = new CallData()},
                        new InterviewData() {Tag="S1.I2", Call = new CallData()},
                        new InterviewData() {Tag="S1.I3", Call = new CallData()},
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomes(CallOutcome.Connected, CallOutcome.NoReply, CallOutcome.Connected);

            var interview = console.StartInterview();
            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);

            interview = console.NextInterview(interview, details);
            Assert.AreEqual(context.GetInterview("S1.I3").Id, interview.Id);

            interview = console.NextInterview(interview, details);
            Assert.IsNull(interview);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Completed);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.NoReply);
            context.GetInterview("S1.I3").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Completed);
        }

        [TestMethod]
        public void CallDeliveryInAutomaticSurvey_LastCallDeliveryWithNotConnected_CallsHaveCorrectITSs()
        {
            BackendToolsObject.LaunchAllHoursScript();

            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Automatic, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData() {Tag="S1.I1", Call = new CallData()},
                        new InterviewData() {Tag="S1.I2", Call = new CallData()},
                        new InterviewData() {Tag="S1.I3", Call = new CallData()},
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomes(CallOutcome.Connected, CallOutcome.Connected, CallOutcome.NoReply);

            var interview = console.StartInterview();
            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);

            interview = console.NextInterview(interview, details);
            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);

            interview = console.NextInterview(interview, details);
            Assert.IsNull(interview);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Completed);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Completed);
            context.GetInterview("S1.I3").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.NoReply);
        }
    }
}
