using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using InterviewControlData = Confirmit.CATI.Core.ManagementService.InterviewControlData;

using Confirmit.CATI.Supervisor.Core.ITSs;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class ProductivityReportTest : BaseMockedIntegrationTest
    {

        private ISurveyStateService _surveyStateService;

        const string UserName1 = "user1";
        const string UserName2 = "user2";
        const string Password1 = "password1";
        const string Password2 = "password2";
        const string ProjectId1 = "p0123123";
        const string ProjectId2 = "p0123125";

        private const int InterviewDuration = 100;

        private InterviewHistoryData GetDefaultHistoryData(int personId, int interviewId, string projectId)
        {
            return new InterviewHistoryData
            {
                grossDuration = InterviewDuration,
                interviewerID = personId,
                interviewID = interviewId,
                netDuration = InterviewDuration,
                projectID = projectId,
                roleID = 2,
                totalDuration = InterviewDuration,
                time = DateTime.UtcNow,
                status = "59"
            };
        }

        private InterviewHistoryData GetDefaultHistoryData(int personId, int interviewId, string projectId, DateTime firedTime)
        {
            var hdata = GetDefaultHistoryData(personId, interviewId, projectId);
            hdata.time = firedTime;
            return hdata;
        }

        private InterviewControlData GetDefaultInterviewData(int personId, int interviewId, string projectId)
        {
            return new InterviewControlData
            {
                interviewerID = personId,
                interviewID = interviewId,
                lastCallTime = DateTime.Now,
                projectID = projectId,
                roleID = 2,
                totalDuration = 100,
                status = "59"
            };
        }

        private void GenerateWrongHistoryRecord(int projectId, int? personId)
        {
            BvHistoryAdapter.Insert(
                new BvHistoryEntity
                {
                    AppointmentID = null,
                    BatchId = 0,
                    ConfirmitDuration = 0,
                    CallCenterID = 0,
                    Duration = 0,
                    FiredTime = DateTime.UtcNow,
                    InterviewId = null,
                    ITS = null,
                    OpenEndReviewDuration = 0,
                    SurveyId = projectId,
                    PersonSID = personId,
                    RoleID = 2
                });
        }

        private bool CompareBvSpRptProdByInterEx2Entity(
            ProductivityReportRecord expected,
            ProductivityReportRecord actual)
        {
            Assert.AreEqual(expected.PersonCode, actual.PersonCode, "different PersonCode");
            Assert.AreEqual(expected.PersonName, actual.PersonName, "different PersonName");
            Assert.AreEqual(expected.PersonSID, actual.PersonSID, "different PersonSID");
            Assert.AreEqual(expected.StateID, actual.StateID, "different StateID");
            Assert.AreEqual(expected.StateName, actual.StateName, "different StateName");
            Assert.AreEqual(expected.SurveyCode, actual.SurveyCode, "different SurveyCode");
            Assert.AreEqual(expected.SurveyName, actual.SurveyName, "different SurveyName");
            Assert.AreEqual(expected.SurveySID, actual.SurveySID, "different SurveySID");
            Assert.AreEqual(expected.InterviewCount, actual.InterviewCount, "different InterviewCount");
            Assert.AreEqual(expected.InterviewTime, actual.InterviewTime, "different InterviewTime");
            Assert.AreEqual(expected.TotalInterviewCount, actual.TotalInterviewCount, "different TotalInterviewCount");
            Assert.AreEqual(expected.InterviewTimePercentage, actual.InterviewTimePercentage, "different InterviewTimePercentage");
            return true;
        }

        private BvSpSurveyProductivityReportEntity GetDefaultReportData(int personId, string userName, int surveyId, string projectId, Nullable<byte> stateId = 59, string stateName = "Custom29")
        {
            return new BvSpSurveyProductivityReportEntity
            {
                InterviewCount = 1,
                InterviewTime = 100,
                PersonName = userName,
                PersonCode = personId.ToString(CultureInfo.InvariantCulture),
                PersonSID = personId,
                StateID = stateId,
                StateName = stateName,
                SurveySID = surveyId,
                SurveyCode = projectId,
                SurveyName = "",
                TotalInterviewCount = 1,
                InterviewTimePercentage = 100
            };
        }

        private BvInterviewEntity CreateInterviewAndSendControlAndHistoryData(int surveyId, string projectId, int personId)
        {
            return CreateInterviewAndSendControlAndHistoryData(surveyId, projectId, personId, x => { });
        }

        private BvInterviewEntity CreateInterviewAndSendControlAndHistoryData(int surveyId, string projectId, int personId, Action<InterviewHistoryData> historyDataModification)
        {
            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);
            var historyData1 = GetDefaultHistoryData(personId, interview.ID, projectId);

            historyDataModification(historyData1);

            var controlData1 = GetDefaultInterviewData(personId, interview.ID, projectId);
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData1, controlData1);

            return interview;
        }

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

       
        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ThreeHistoryRecords_OnlyOneFitsInSpecifiedShift()
        {
            DateTime now = DateTime.UtcNow;
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);

            BackendToolsObject.CreateHistoryRecords(surveyId1, personId1,  new DateTime[] {now.AddMinutes(-360), now.AddMinutes(-120)}, 1, 200, 20);
            BackendToolsObject.CreateHistoryRecords(surveyId1, personId1, new DateTime[] { now.AddMinutes(-240) }, 3, 100, 5);

            var report = ReportManager.GetProductivityReportData(DateTime.UtcNow.AddMinutes(-360-1), now,
                new[] { surveyId1}, new[] { personId1}, new[] { "13" }, null, now.AddMinutes(-240-10), now.AddMinutes(-240 + 10));

            var reportRecord1 = GetDefaultReportData(personId1, UserName1, surveyId1, ProjectId1, (Nullable<byte>) CallOutcome.Completed, "Completed");

            TestAssert.AreEqual(
                new[] 
                { 
                    new ProductivityReportRecord(reportRecord1)
                },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(47397), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_ReportFor2Surveys_DataIsCorrect()
        {
            new ManagementService();

            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId2);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            var personId2 = PersonTools.CreatePerson(UserName2, Password2, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId2);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId2);
            CreateInterviewAndSendControlAndHistoryData(surveyId2, ProjectId2, personId1);

            CreateInterviewAndSendControlAndHistoryData(surveyId2, ProjectId2, personId1, x => { x.status = "60"; });

            var report = ReportManager.GetProductivityReportData(DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1, surveyId2 },
                new[] { personId1, personId2 },
                new[] { "59" },
                null,
                null,
                null
                );

            var reportRecord1 = GetDefaultReportData(personId1, UserName1, surveyId1, ProjectId1);

            var reportRecord2 = GetDefaultReportData(personId1, UserName1, surveyId2, ProjectId2);
            reportRecord2.TotalInterviewCount = 2;
            reportRecord2.InterviewTimePercentage = 50;

            var reportRecord3 = GetDefaultReportData(personId2, UserName2, surveyId1, ProjectId1);

            TestAssert.AreEqual(
                new[] 
                { 
                    new ProductivityReportRecord(reportRecord1), 
                    new ProductivityReportRecord(reportRecord2), 
                    new ProductivityReportRecord(reportRecord3)
                },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_ReportWithSurveyDataFilter_DataIsCorrect()
        {
            new ManagementService();

            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            var interview = CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            interview = CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");
            
            var report = ReportManager.GetProductivityReportData(DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1 },
                new[] { personId1 },
                new[] { "59" }, 
                "CFInterview.[CallAttemptCount]=1",
                null,
                null
                );

            var reportRecord1 = GetDefaultReportData(personId1, UserName1, surveyId1, ProjectId1);
            reportRecord1.InterviewCount = 2;
            reportRecord1.TotalInterviewCount = 2;
            reportRecord1.InterviewTime *= 2;
     
            TestAssert.AreEqual(
                new[] 
                { 
                    new ProductivityReportRecord(reportRecord1)
                },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }


        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(46921), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_FakeRecordsAreNotCalced_DataIsCorrect()
        {

            var stub = TestingFramework.RegistryStub<IInterviewTimings, StubIInterviewTimings>();

            var test = new TestCati2(true, false, BackendToolsObject);
            test.CreateSurveyWithPerson(DialingMode.Preview, UserName1, Password1, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName1, Password1, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer("1234");

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            test.ReplyOnInterview_Progressive(interview);
            var task = TaskRepository.GetByPerson(test.PersonSID);
            stub.GetInterviewTimingsBvTasksEntityBvSurveyEntity = (t, s) =>
            {
                var timings = new BvInterviewTimings
                {
                    InterviewDurationTime = InterviewDuration,
                    CallCenterID = task.CallCenterID,
                    TimeCallDelivered = task.TimeCallDelivered,
                    WaitingTime = 0,
                    OpenEndReviewDurationTime = 0,
                };
                return timings;
            };

            test.WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails {Its = "59"});

            GenerateWrongHistoryRecord(test.SurveySID, test.PersonSID);

            var report = ReportManager.GetProductivityReportData(
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { test.SurveySID },
                new[] { test.PersonSID },
                new[] { "59" },
                null, null, null
                );

            TestAssert.AreEqual(
                new[] { new ProductivityReportRecord(GetDefaultReportData(test.PersonSID, UserName1, test.SurveySID, test.SurveyName)) },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_TerminateDuringInterview_TimingsAreCalculatedCorrectly()
        {
            int minInterviewingTime = 1;
            
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName1, Password1, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName1, Password1, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer("1234");
            BvInterviewEntity interview = test.StartInterview_Progressive(test.SurveyName, 0);
            Assert.IsNotNull(interview);

            //Sorry cannot avoid Sleep here
            Thread.Sleep(minInterviewingTime*1000);

            Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));

            var report = ReportManager.GetProductivityReportData(
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { test.SurveySID },
                new[] { test.PersonSID },
                new[] { ((int)CallOutcome.InterruptedBySystem).ToString() },
                null, null, null
                );

            Assert.IsTrue(report.Single().InterviewTime >= minInterviewingTime);

            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_AddSeveralSurveysForCATI_ResultIsReturnedOnlyForPassedSurveyID()
        {
            new ManagementService();

            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId2);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            var personId2 = PersonTools.CreatePerson(UserName2, Password2, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId2);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId2);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId2);

            var report = ReportManager.GetProductivityReportData(DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId2 },
                new[] { personId1, personId2 },
                new[] { "59" },
                null, null, null
                );

            TestAssert.AreEqual(
                new ProductivityReportRecord[] { },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }

        private IEnumerable<ProductivityReportRecord> ProductivityReport_PassNullAsParameters_GetCatiData(int personId1, string userName1, int personId2, string userName2, int surveyId1, int surveyId2)
        {
            var catiData1 = GetDefaultReportData(personId1, userName1, surveyId1, ProjectId1);
            catiData1.StateID = 13;
            catiData1.StateName = "Completed";
            var catiData2 = GetDefaultReportData(personId2, userName2, surveyId1, ProjectId1);
            var catiData3 = GetDefaultReportData(personId1, userName1, surveyId2, ProjectId2);
            var catiData4 = GetDefaultReportData(personId2, userName2, surveyId2, ProjectId2);

            return new[]
            {
                new ProductivityReportRecord(catiData1), 
                new ProductivityReportRecord(catiData2), 
                new ProductivityReportRecord(catiData3), 
                new ProductivityReportRecord(catiData4)
            };
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_PassNullAsParametersForCati_AllRecordsForCatiAreReturned()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId2);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            var personId2 = PersonTools.CreatePerson(UserName2, Password2, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId2);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId2);
 
            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1, x => x.status = "Complete");
            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId2);
            CreateInterviewAndSendControlAndHistoryData(surveyId2, ProjectId2, personId1);
            CreateInterviewAndSendControlAndHistoryData(surveyId2, ProjectId2, personId2);

            var report = ReportManager.GetProductivityReportData(DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1, surveyId2 },
                null,
                null,
                null,null,null
                );

            TestAssert.AreEqual(ProductivityReport_PassNullAsParameters_GetCatiData(personId1, UserName1, personId2, UserName2, surveyId1, surveyId2)
                    .OrderBy(x => x.PersonCode).ThenBy(x => x.StateID).ThenBy(x => x.SurveySID),
                report.OrderBy(x => x.PersonCode).ThenBy(x => x.StateID).ThenBy(x => x.SurveySID),
                CompareBvSpRptProdByInterEx2Entity);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_BuildReportForCatiWithEmptyDateRange_ThereAreNoRecords()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);

            var report = ReportManager.GetProductivityReportData(DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1 },
                null,
                null,
                null, null, null
                );

            TestAssert.AreEqual(
                new ProductivityReportRecord[] { },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_BuildReportForCatiWithSeveralStatesGroup_RecordIsCorrect()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);

            StateGroupsManager.AddStateGroup("new state group");

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);

            var report = ReportManager.GetProductivityReportData(
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1 },
                null,
                null,
                null, null, null
                );

            TestAssert.AreEqual(
                new[] { new ProductivityReportRecord(GetDefaultReportData(personId1, UserName1, surveyId1, ProjectId1)) },
                report,
                CompareBvSpRptProdByInterEx2Entity);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyProductivityReport)]
        public void ProductivityReport_BuildReportForDifferentCallCenters_RecordIsCorrect()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);

            StateGroupsManager.AddStateGroup("new state group");
            var callCenter2 = CallCenterTools.Create();
            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            var personId2 = PersonTools.CreatePerson(UserName2, Password2, AgentTaskChoiceMode.Automatic, callCenter2.ID);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId2);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId2);

            var report1 = ReportManager.GetProductivityReportData(
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1 },
                null,
                null,
                null, null, null, CallCenterTools.DefaultId
            );
            var report2 = ReportManager.GetProductivityReportData(
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                new[] { surveyId1 },
                null,
                null,
                null, null, null, callCenter2.ID
            );

            TestAssert.AreEqual(
                new[] { new ProductivityReportRecord(GetDefaultReportData(personId1, UserName1, surveyId1, ProjectId1)) },
                report1,
                CompareBvSpRptProdByInterEx2Entity);
            
            TestAssert.AreEqual(
                new[] { new ProductivityReportRecord(GetDefaultReportData(personId2, UserName2, surveyId1, ProjectId1)) },
                report2,
                CompareBvSpRptProdByInterEx2Entity);
        }

        // TODO : 2 reports below after test code refactoring need to be moved to SurveyOverviewReportTest.cs
        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void SurveyOverviewReport_ForAllPersons_FilteredBySurveyData_ReporIsCorrect()
        {
            new ManagementService();

            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            var personId2 = PersonTools.CreatePerson(UserName2, Password2, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId2);

            CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);

            var interview = CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId2);
            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            interview = CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);
            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId1 }, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null, new[] { 59 }, false, false, 
                "CFinterview.[CallAttemptCount]=1", null, null);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId1, result[0].ProjectId);
            Assert.AreEqual(2, result[0].Completes);
            Assert.AreEqual(2, result[0].DialigsCount);
            Assert.AreEqual(InterviewDuration*2, result[0].LogOnTime);
            Assert.AreEqual(0, result[0].WaitingTime);
            Assert.AreEqual(InterviewDuration, result[0].AverageCompletedInterviewDuration);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void SurveyOverviewReport_ForSpecificPerson_FilteredBySurveyData_ReporIsCorrect()
        {
            new ManagementService();

            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            _surveyStateService.Open(surveyId1);

            var personId1 = PersonTools.CreatePerson(UserName1, Password1, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId1);

            var interview = CreateInterviewAndSendControlAndHistoryData(surveyId1, ProjectId1, personId1);

            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId1 }, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new[] {personId1}, new[] { 59 }, false, false,
                "CFinterview.[CallAttemptCount]=1", null, null);

            Assert.AreEqual(0, result.Count);

            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId1 }, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new[] { personId1 }, new[] { 59 }, false, false, 
                "CFinterview.[CallAttemptCount]=1", null, null);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId1, result[0].ProjectId);
            Assert.AreEqual(1, result[0].Completes);
            Assert.AreEqual(1, result[0].DialigsCount);
            Assert.AreEqual(InterviewDuration * 1, result[0].LogOnTime);
            Assert.AreEqual(0, result[0].WaitingTime);
            Assert.AreEqual(InterviewDuration, result[0].AverageCompletedInterviewDuration);
        }
    }
}
