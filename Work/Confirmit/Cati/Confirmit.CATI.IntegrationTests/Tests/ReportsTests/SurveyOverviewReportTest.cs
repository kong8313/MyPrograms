using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class SurveyOverviewReportTest : BaseMockedIntegrationTest
    {
        private const string ProjectId = "p0000001";
        private const string UserName = "username1";
        private const string Password = "password1";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\MaximL"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ForAllPerson_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = false;
            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, 10);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, 10);

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");


            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, null, null, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].Completes);
            Assert.AreEqual(4, result[0].DialigsCount);
            Assert.AreEqual(630 + 40, result[0].LogOnTime);
            Assert.AreEqual(30, result[0].WaitingTime);
            Assert.AreEqual(0, result[0].AverageCompletedInterviewDuration);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ForAllPerson_IncludingBreakTime_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = false;
            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, 10);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, 10);

            CreateBreakTimeHistoryRecord(personId1, DateTime.Parse("10:35:00 10.10.2009"), surveyId, 50);
            CreateBreakTimeHistoryRecord(personId2, DateTime.Parse("11:35:00 10.10.2009"), surveyId, 20);
            
            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");


            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, null, null, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].Completes);
            Assert.AreEqual(4, result[0].DialigsCount);
            Assert.AreEqual(630 + 40 + 50, result[0].LogOnTime);
            Assert.AreEqual(30, result[0].WaitingTime);
            Assert.AreEqual(0, result[0].AverageCompletedInterviewDuration);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ForSpecificPersonAndCompletedItses_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = false;
            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Busy, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, 10);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Busy, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, 10);

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");


            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(1, result[0].Completes);
            Assert.AreEqual(2, result[0].DialigsCount);
            Assert.AreEqual(210 + 20, result[0].LogOnTime);
            Assert.AreEqual(10, result[0].WaitingTime);
            Assert.AreEqual(100, result[0].AverageCompletedInterviewDuration);
        }
        
          [TestMethod, Owner(@"FIRM\MaximL"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ForSpecificPerson_IncludingBreakTime_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = false;
            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Busy, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, 10);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Busy, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, 10);

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");

            CreateBreakTimeHistoryRecord(personId1, DateTime.Parse("10:35:00 10.10.2009"), surveyId, 50);
            CreateBreakTimeHistoryRecord(personId2, DateTime.Parse("10:35:00 10.10.2009"), surveyId, 20);


            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(1, result[0].Completes);
            Assert.AreEqual(2, result[0].DialigsCount);
            Assert.AreEqual(210 + 20 + 50, result[0].LogOnTime);
            Assert.AreEqual(10, result[0].WaitingTime);
            Assert.AreEqual(100, result[0].AverageCompletedInterviewDuration);
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ThreeHistoryRecords_OnlyOneFitsInSpecifiedShift()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = false;
            DateTime now = DateTime.UtcNow;
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            CreateHistoryRecord(personId1, now.AddMinutes(-360), surveyId1, 1, (CallOutcome)13, 5, 100, 10);
            CreateHistoryRecord(personId1, now.AddMinutes(-240), surveyId1, 2, (CallOutcome)13, 10, 200, 10);
            CreateHistoryRecord(personId1, now.AddMinutes(-120), surveyId1, 3, (CallOutcome)13, 15, 300, 10);

            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId1 }, DateTime.UtcNow.AddMinutes(-360 - 1), now,
                 new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, now.AddMinutes(-240 - 10), now.AddMinutes(-240 + 10));

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(1, result[0].Completes);
            Assert.AreEqual(1, result[0].DialigsCount);
            Assert.AreEqual(200 + 10 + 10, result[0].LogOnTime);
            Assert.AreEqual(10, result[0].WaitingTime);
            Assert.AreEqual(200, result[0].AverageCompletedInterviewDuration);
        }

        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ForAllPerson_IncludeOpenEndReviewTimeInInterviewDuration_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = true;

            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, 10);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, 10);

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");

            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, null, null, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].Completes);
            Assert.AreEqual(4, result[0].DialigsCount);
            Assert.AreEqual(630, result[0].LogOnTime);
            Assert.AreEqual(30, result[0].WaitingTime);
            Assert.AreEqual(0, result[0].AverageCompletedInterviewDuration);
        }

        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_ForAllPerson_FilterByCallCenter()
        {
            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", IsUseDb = true, Assigns = new[] { "P1" }, CallCenters = new[] { "CC1", "CC2" } }
                },
                Persons = new[] {
                    new PersonData() { Tag = "P1", CallCenter = "CC1" },
                    new PersonData() { Tag = "P2", CallCenter = "CC2" }
                },
                CallCenters = new[] { new CallCenterData() { Tag = "CC1" }, new CallCenterData() { Tag = "CC2" } }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;
            var projectId = context.GetSurvey("S1").Data.ProjectId;
            var personId1 = context.GetPerson("P1").Id;
            var personId2 = context.GetPerson("P2").Id;
            var callCenterId1 = context.GetCallCenter("CC1").Id;
            var callCenterId2 = context.GetCallCenter("CC2").Id;

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, callCenterId:callCenterId1);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Completed, 5, 100, callCenterId:callCenterId1);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, callCenterId:callCenterId1);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, callCenterId:callCenterId2);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Completed, 10, 200, callCenterId:callCenterId2);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, callCenterId:callCenterId2);

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");

            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, null, new[] { 13 }, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(projectId, result[0].ProjectId);
            Assert.AreEqual(4, result[0].Completes);
            Assert.AreEqual(4, result[0].DialigsCount);
            Assert.AreEqual(400 + 200 + 10 + 20, result[0].LogOnTime);
            Assert.AreEqual(10 + 20, result[0].WaitingTime);
            Assert.AreEqual(150, result[0].AverageCompletedInterviewDuration);

            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, null, new[] { 13 }, false, false, null, null, null, callCenterId1);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(projectId, result[0].ProjectId);
            Assert.AreEqual(2, result[0].Completes);
            Assert.AreEqual(2, result[0].DialigsCount);
            Assert.AreEqual(200 + 10, result[0].LogOnTime);
            Assert.AreEqual(10, result[0].WaitingTime);
            Assert.AreEqual(100, result[0].AverageCompletedInterviewDuration);

            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, null, new[] { 13 }, false, false, null, null, null, callCenterId2);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(projectId, result[0].ProjectId);
            Assert.AreEqual(2, result[0].Completes);
            Assert.AreEqual(2, result[0].DialigsCount);
            Assert.AreEqual(400 + 20, result[0].LogOnTime);
            Assert.AreEqual(20, result[0].WaitingTime);
            Assert.AreEqual(200, result[0].AverageCompletedInterviewDuration);
        }


        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_IncludeOpenEndReviewTimeInInterviewDuration_ForSpecificPersonAndCompletedItses_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = true;
            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            CreateHistoryRecord(personId1, DateTime.Parse("10:30:00 10.10.2009"), surveyId, 1, CallOutcome.Completed, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("10:45:00 10.10.2009"), surveyId, 2, CallOutcome.Busy, 5, 100, 10);
            CreateHistoryRecord(personId1, DateTime.Parse("11:45:00 10.10.2009"), surveyId, 3, CallOutcome.Completed, 5, 100, 10);

            CreateHistoryRecord(personId2, DateTime.Parse("09:15:00 10.10.2009"), surveyId, 4, CallOutcome.Completed, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:15:00 10.10.2009"), surveyId, 5, CallOutcome.Busy, 10, 200, 10);
            CreateHistoryRecord(personId2, DateTime.Parse("10:20:00 10.10.2009"), surveyId, 6, CallOutcome.Completed, 10, 200, 10);

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");


            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId }, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(1, result[0].Completes);
            Assert.AreEqual(2, result[0].DialigsCount);
            Assert.AreEqual(210, result[0].LogOnTime);
            Assert.AreEqual(10, result[0].WaitingTime);
            Assert.AreEqual(100, result[0].AverageCompletedInterviewDuration);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_HideEmpty_DontHideEmpty_ResultAreCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = true;
            var surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson("p1");

            var startDate = DateTime.Parse("10:00:00 10.10.2009");
            var endDate = DateTime.Parse("11:00:00 10.10.2009");

            //BvSpSurveyOverviewReport
            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, null, null, CallCenterTools.DefaultId);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].DialigsCount);
            
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].DialigsCount);
            
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, true, null, null, null, CallCenterTools.DefaultId);
            Assert.AreEqual(0, result.Count);
            
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, true, null, null, null);
            Assert.AreEqual(0, result.Count);
            
            //BvSpSurveyOverviewReportForAllPersons
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, null, new[] { (int)CallOutcome.Completed }, false, false, null, null, null, CallCenterTools.DefaultId);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].DialigsCount);
            
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, null, new[] { (int)CallOutcome.Completed }, false, false, null, null, null, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(0, result[0].DialigsCount);
            
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, null, new[] { (int)CallOutcome.Completed }, false, true, null, null, null, CallCenterTools.DefaultId);
            Assert.AreEqual(0, result.Count);
            
            result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId}, startDate, endDate, null, new[] { (int)CallOutcome.Completed }, false, true, null, null, null);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.SurveyOverviewReport)]
        public void GetSurveyOverviewReportData_IncludeOpenEndReviewTimeInInterviewDuration_ThreeHistoryRecords_OnlyOneFitsInSpecifiedShift()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = true;
            DateTime now = DateTime.UtcNow;
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId);
            var personId1 = PersonTools.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            CreateHistoryRecord(personId1, now.AddMinutes(-360), surveyId1, 1, (CallOutcome)13, 5, 100, 10);
            CreateHistoryRecord(personId1, now.AddMinutes(-240), surveyId1, 2, (CallOutcome)13, 10, 200, 10);
            CreateHistoryRecord(personId1, now.AddMinutes(-120), surveyId1, 3, (CallOutcome)13, 15, 300, 10);

            var result = ReportManager.GetSurveyOverviewReportData(new[] { surveyId1 }, DateTime.UtcNow.AddMinutes(-360 - 1), now,
                 new[] { personId1 }, new[] { (int)CallOutcome.Completed }, false, false, null, now.AddMinutes(-240 - 10), now.AddMinutes(-240 + 10));

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ProjectId, result[0].ProjectId);
            Assert.AreEqual(1, result[0].Completes);
            Assert.AreEqual(1, result[0].DialigsCount);
            Assert.AreEqual(200 + 10, result[0].LogOnTime);
            Assert.AreEqual(10, result[0].WaitingTime);
            Assert.AreEqual(200, result[0].AverageCompletedInterviewDuration);
        }

        private void CreateHistoryRecord(int personId, DateTime time, int surveyId, int IID, CallOutcome callOutcome, int waitingTime, int interviewwingTime, int openEndReviewDuration = 0, int? callCenterId = null)
        {
            var history = new BvHistoryEntity()
            {
                CallCenterID = callCenterId ?? CallCenterTools.DefaultId,
                FiredTime = time,
                ConfirmitDuration = interviewwingTime,
                Duration = interviewwingTime,
                PersonSID = personId,
                SurveyId = surveyId,
                InterviewId = IID,
                ITS = (byte)callOutcome,
                WaitingTime = waitingTime,
                RoleID = 2,
                OpenEndReviewDuration = openEndReviewDuration
            };
            BvHistoryAdapter.Insert(history);
        }

        private void CreateBreakTimeHistoryRecord(int personId, DateTime startTime, int surveyId, int duration )
        {
            var history = new BvTimeBreaksHistoryEntity() {
                InterviewerId = personId,
                StartTime = startTime,
                Duration = duration,
                SurveyId = surveyId,
                BreakTypeId = 1,
                CallCenterId = 1
            };
            
            BvTimeBreaksHistoryAdapter.Insert(history);
        }
    }
}
