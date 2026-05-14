using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class TzBalancingTest : BaseMockedIntegrationTest
    {
        //private ITimeService _timeService;

        DateTime timeTZBalancingThresholdNotMet = new DateTime(2017, 02, 23, 16, 8, 0);
        DateTime timeTZBalancingThresholdMet = new DateTime(2017, 02, 23, 16, 35, 0);       //less then 30 min till the end of shift in TZ 3

        
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            TimezoneManager.AddTimezone(1);     //GMT
            TimezoneManager.AddTimezone(3);     //GMT+1
            ServiceLocator.Resolve<ITimeZoneBalancingSettings>().EndOfShiftThreshold = 30;
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallsHaveValidShiftType_TzBalancingThresholdNotMet_FirstCallByIdIsDelievered()
        {

            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdNotMet);
            var context = CreateSurvey(DialingMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;

            InterviewController interview = RunTest((CallShiftType) dbShiftTypeID, context);

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallsHaveValidShiftType_TzBalancingThresholdMet_SecondCallIdIsDelievered()
        {

            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var context = CreateSurvey(DialingMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;
            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context);

            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_ClusteredQuotaEnabled_CallsHaveValidShiftType_TzBalancingThresholdNotMet_FirstCallByIdIsDelievered()
        {
            ServiceLocator.Resolve<IQuotaClusteringSettings>().Enabled = true;
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdNotMet);
            var context = CreateSurvey(DialingMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;

            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context);

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_ClusteredQuotaEnabled_CallsHaveValidShiftType_TzBalancingThresholdMet_SecondCallIdIsDelievered()
        {
            ServiceLocator.Resolve<IQuotaClusteringSettings>().Enabled = true;
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var context = CreateSurvey(DialingMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;
            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context);

            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_PersonInAutomaticAssignment_CallsHaveValidShiftType_TzBalancingThresholdNotMet_FirstCallByIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdNotMet);
            var context = CreateSurvey(DialingMode.Automatic, TaskChoiceMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;

            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context, TaskChoiceMode.Automatic);

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_PersonInAutomaticAssignment_CallsHaveValidShiftType_TzBalancingThresholdMet_SecondCallIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var context = CreateSurvey(DialingMode.Automatic, TaskChoiceMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;
            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context, TaskChoiceMode.Automatic);

            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallGroups_CallsHaveValidShiftType_TzBalancingThresholdNotMet_FirstCallByIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdNotMet);
            var context = CreateSurveyWithCallGroup(DialingMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;

            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context);

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallGroups_CallsHaveValidShiftType_TzBalancingThresholdMet_SecondCallIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var context = CreateSurveyWithCallGroup(DialingMode.Automatic);

            var survey = context.GetSurvey("S1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;
            InterviewController interview = RunTest((CallShiftType)dbShiftTypeID, context);

            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);
        }
        
        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallsHasAnyValidShiftType_TzBalancingThresholdNotMet_FirstCallByIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdNotMet);

            var context = CreateSurvey(DialingMode.Automatic);

            InterviewController interview = RunTest(CallShiftType.AnyValid, context);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallsHasAnyValidShiftType_TzBalancingThresholdMet_SecondCallIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var context = CreateSurvey(DialingMode.Automatic);
            InterviewController interview = RunTest(CallShiftType.AnyValid, context);

            Assert.AreEqual(context.GetInterview("S1.I2").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallsHaveANoneShiftType_TzBalancingThresholdNotMet_FirstCallByIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdNotMet);

            var context = CreateSurvey(DialingMode.Automatic);

            InterviewController interview = RunTest(CallShiftType.None, context);

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_CallsHaveANoneShiftType_TzBalancingThresholdMet_FirstCallByIdIsDelievered()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var context = CreateSurvey(DialingMode.Automatic);

            InterviewController interview = RunTest(CallShiftType.None, context);

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_PredictiveSurvey_CAllsRequestByCampaign_TzBalancingThresholdMet_First3CallsWithTZ3()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var callsList = CreateContextForPredictiveSurvey(CallsSelectionAlgorithm.ByCampaign);

            Assert.IsTrue(callsList.Select(x=>x.callId).SequenceEqual(new long []{ 2, 4, 6, 1, 3, 5}));
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_PredictiveSurvey_CAllsRequestByPersonGroup_TzBalancingThresholdMet_FirstCallsWithTZ3()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var callsList = CreateContextForPredictiveSurvey(CallsSelectionAlgorithm.ByPersonGroup);

            Assert.IsTrue(callsList.Select(x => x.callId).SequenceEqual(new long[] { 2, 1 }));
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_PredictiveSurvey_CAllsRequestCallsAssignedToCampaignOnly_TzBalancingThresholdMet_FirstCallsWithTZ3()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var callsList = CreateContextForPredictiveSurvey(CallsSelectionAlgorithm.CallsAssignedToCampaignOnly);

            Assert.IsTrue(callsList.Select(x => x.callId).SequenceEqual(new long[] { 6, 5 }));
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TzBalancing_PredictiveSurvey_CAllsRequestCallsAssignedToAgentsExplicitly_TzBalancingThresholdMet_FirstCallsWithTZ3()
        {
            new DateTimeMocker(TestingFramework).MockDate(timeTZBalancingThresholdMet);

            var callsList = CreateContextForPredictiveSurvey(CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly);

            Assert.IsTrue(callsList.Select(x => x.callId).SequenceEqual(new long[] { 4, 3 }));
        }

        private InterviewController RunTest(CallShiftType type, TestDataContext context, TaskChoiceMode mode = TaskChoiceMode.SurveyAssignment)
        {
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id, new[] { context.GetInterview("S1.I1").Id, context.GetInterview("S1.I2").Id }, 1, 0, (int) type, CallQueueService.DefaultTimeInShift, CallStates.All, false);

            var console = new AutomaticConsoleController(context, person, mode == TaskChoiceMode.Automatic ? null : survey);

            console.Login();
            BackendTools.RunSchedulingProcedure();

            return(console.StartInterview());
        }

        private TestDataContext CreateSurvey(DialingMode diallingMode, TaskChoiceMode mode = TaskChoiceMode.SurveyAssignment)
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", DialMode = diallingMode, IsUseDb = false, SchedulingScript = "SS1", Assigns = new []{"P1"},
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", TimeZoneId = "1"},
                            new InterviewData() {Tag = "S1.I2", TimeZoneId = "3"}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = mode } },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.DisableCall),
                            new Shift(1, 1, "0.13:00:00", "0.18:00:00"),
                            new Shift(2, 1, "1.13:00:00", "1.18:00:00"),
                            new Shift(3, 1, "2.13:00:00", "2.18:00:00"),
                            new Shift(4, 1, "3.13:00:00", "3.18:00:00"),
                            new Shift(5, 1, "4.13:00:00", "4.18:00:00"),
                            new Shift(6, 1, "5.13:00:00", "5.18:00:00"),
                            new Shift(7, 1, "6.13:00:00", "6.18:00:00")
                            )
                    }
                }
            }.Create();
            return context;
        }

        private TestDataContext CreateSurveyWithCallGroup(DialingMode diallingMode, TaskChoiceMode mode = TaskChoiceMode.SurveyAssignment)
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsCallGroupEnabled = true, DialMode = diallingMode, IsUseDb = false, SchedulingScript = "SS1", Assigns = new []{"P1"},
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", TimeZoneId = "1"},
                            new InterviewData() {Tag = "S1.I2", TimeZoneId = "3"}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", CallGroup="CG1", TaskChoice = mode } },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.DisableCall),
                            new Shift(1, 1, "0.13:00:00", "0.18:00:00"),
                            new Shift(2, 1, "1.13:00:00", "1.18:00:00"),
                            new Shift(3, 1, "2.13:00:00", "2.18:00:00"),
                            new Shift(4, 1, "3.13:00:00", "3.18:00:00"),
                            new Shift(5, 1, "4.13:00:00", "4.18:00:00"),
                            new Shift(6, 1, "5.13:00:00", "5.18:00:00"),
                            new Shift(7, 1, "6.13:00:00", "6.18:00:00")
                        )
                    }
                },
                CallGroups = new[] { new CallGroupData() { Tag = "CG1", ITS = new[] { CallOutcome.FreshSample } } }
            }.Create();
            return context;
        }

        private IEnumerable<CallInfo> CreateContextForPredictiveSurvey(CallsSelectionAlgorithm algorithm)
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
 
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", TimeZoneId = "1", Call = new CallData { Resource = "PG1" }},
                        new InterviewData { Tag="S1.I2", TimeZoneId = "3", Call = new CallData { Resource = "PG1" }},
                        new InterviewData { Tag="S1.I3", TimeZoneId = "1", Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", TimeZoneId = "3", Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I5", TimeZoneId = "1", Call = new CallData { }},
                        new InterviewData { Tag="S1.I6", TimeZoneId = "3", Call = new CallData { }}
                    },
                    Assigns = new[]{"PG1"}
                }},
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1"}
                },
                Persons = new[]{
                    new PersonData { Tag="P1", Memberships="PG1", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"}
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.DisableCall),
                            new Shift(1, 1, "0.13:00:00", "0.18:00:00"),
                            new Shift(2, 1, "1.13:00:00", "1.18:00:00"),
                            new Shift(3, 1, "2.13:00:00", "2.18:00:00"),
                            new Shift(4, 1, "3.13:00:00", "3.18:00:00"),
                            new Shift(5, 1, "4.13:00:00", "4.18:00:00"),
                            new Shift(6, 1, "5.13:00:00", "5.18:00:00"),
                            new Shift(7, 1, "6.13:00:00", "6.18:00:00")
                        )
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var dbShiftTypeID = SurveyManager.GetShiftTypes(survey.Id).Find(x => x.Id == 1).ObjectId;

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id, new[] { context.GetInterview("S1.I1").Id, context.GetInterview("S1.I2").Id }, 1, context.GetResource("PG1").Id, dbShiftTypeID, CallQueueService.DefaultTimeInShift, CallStates.All, false);
            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id, new[] { context.GetInterview("S1.I3").Id, context.GetInterview("S1.I4").Id }, 1, context.GetResource("P1").Id, dbShiftTypeID, CallQueueService.DefaultTimeInShift, CallStates.All, false);
            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id, new[] { context.GetInterview("S1.I5").Id, context.GetInterview("S1.I6").Id}, 1, 0, dbShiftTypeID, CallQueueService.DefaultTimeInShift, CallStates.All, false);

            BackendTools.RunSchedulingProcedure();
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            return dialer.RequestCalls(survey, 6, algorithm, algorithm == CallsSelectionAlgorithm.ByPersonGroup ? context.GetResource("PG1").Id : 0).CallList;
        }
    }
}
