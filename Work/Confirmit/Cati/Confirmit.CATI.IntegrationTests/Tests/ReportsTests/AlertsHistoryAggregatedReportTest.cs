using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchParameter = Confirmit.CATI.Core.Paging.SearchParameter;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class AlertsHistoryAggregatedReportTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            _surveyStateService.Open(_surveyId);

            _personId = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveyId, PersonName, AgentTaskChoiceMode.Automatic);

            BvSpThresholds_insertAdapter.ExecuteNonQuery(0, AnswerSubmissionAlertTypeId, DefaultAmberAnswerSubmissionThreshold, DefaultRedAnswerSubmissionThreshold);
            BvSpThresholds_insertAdapter.ExecuteNonQuery(0, QuickAnswerSubmissionAlertTypeId, DefaultAmberQuickAnswerSubmissionThreshold, DefaultRedQuickAnswerSubmissionThreshold);

            _ws = new CatiWsHelper(PersonName, "p1");

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            _ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);
        }

        private const string ProjectId = "p01293438";
        private const string PersonName = "u1";

        private const int AnswerSubmissionAlertTypeId = 1;
        private const int QuickAnswerSubmissionAlertTypeId = 17;

        private const int DefaultAmberAnswerSubmissionThreshold = 8;
        private const int DefaultRedAnswerSubmissionThreshold = 10;
        private const int DefaultAmberQuickAnswerSubmissionThreshold = 4;
        private const int DefaultRedQuickAnswerSubmissionThreshold = 2;

        private const int AmberAnswerSubmissionValue = 9;
        private const int RedAnswerSubmissionValue = 11;
        private const int AmberQuickAnswerSubmissionValue = 3;
        private const int RedQuickAnswerSubmissionValue = 1;

        private int _surveyId;
        private int _personId;

        private CatiWsHelper _ws;

        private static void AnswerOnQuestion(string projectId, int personId, string questionid, DateTime datetime)
        {
            BvSpTask_UpdateActiveQuestionAdapter.ExecuteNonQuery(projectId, personId, questionid, datetime);
        }

        private static void InternalFinishQuestion(string projectId, int personId, DateTime finishTime)
        {
            BvSpTask_UpdateActiveQuestionAdapter.ExecuteNonQuery(projectId, personId, "Internal_finished", finishTime);
        }

        private void AssertAggregatedAlertsCount(DateTime startDate, DateTime endDate,
            int personId,
            IEnumerable<int> surveyIds,
            int quickAnswerSubmisssionAmberAlertsCount,
            int quickAnswerSubmisssionRedAlertsCount,
            int answerSubmisssionAmberAlertsCount,
            int answerSubmisssionRedAlertsCount)
        {
            var result = BvSpAlertsHistoryAggregatedReportAdapter.ExecuteEntityList(
                personId.ToString(CultureInfo.InvariantCulture), String.Join(",", surveyIds.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()), startDate, endDate, null);

            Assert.AreEqual(1, result.Count, "Report should contains 1 record");
            Assert.AreEqual(answerSubmisssionAmberAlertsCount, result[0].AnswerSubmissionAmberCounts, "AnswerSubmissionAmberCounts");
            Assert.AreEqual(answerSubmisssionRedAlertsCount, result[0].AnswerSubmissionRedCounts, "AnswerSubmissionRedCounts");
            Assert.AreEqual(quickAnswerSubmisssionAmberAlertsCount, result[0].QuickAnswerSubmissionAmberCounts, "QuickAnswerSubmissionAmberCounts");
            Assert.AreEqual(quickAnswerSubmisssionRedAlertsCount, result[0].QuickAnswerSubmissionRedCounts, "QuickAnswerSubmissionRedCounts");

            Assert.AreEqual(personId, result[0].PersonId, "PersonId");
            Assert.AreEqual(PersonRepository.GetById(personId).Name, result[0].PersonName, "PersonName");
        }

        //TODO: rename 
        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void AlertsHistory_RecordsAreStoredMoreThanMonth_RecordsAreCleaned()
        {
            var alertHistory = new BvAnswerSubmissionAlertHistoryEntity
            {
                SubmissionTime = DateTime.UtcNow.AddDays(-30).AddMinutes(-1)
            };
            BvAnswerSubmissionAlertHistoryAdapter.Insert(alertHistory);

            var action = new CleanAnswerSubmissionAlertHistoryTableAction(
                ServiceLocator.Resolve<IAnswerSubmissionAlertHistoryTableCleanupSettings>());
            action.Execute(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(0, BvAnswerSubmissionAlertHistoryAdapter.GetAll().Count, "All records are expired and should be deleted");

            alertHistory.SubmissionTime = DateTime.UtcNow.AddDays(-30).AddMinutes(1);
            BvAnswerSubmissionAlertHistoryAdapter.Insert(alertHistory);

            action.Execute(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(1, BvAnswerSubmissionAlertHistoryAdapter.GetAll().Count);
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void AlertsHistory_TerminateInterview_AlertIsStored()
        {
            BackendTools.CreateInterviewWithCall(_surveyId);
            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;
            var endTime = startTime.AddDays(1);

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);
            Thread.Sleep(DefaultRedAnswerSubmissionThreshold*1000);

            TaskService.TerminateTask(
                _personId,
                new DatabaseTransactionOptions("Terminate", DeadlockPriority.Normal));

            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { _surveyId }, 0, 0, 0, 1);
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void AlertsHistoryAggregatedReport_2Interviews2QuestionsPerInterview_1AlertPerAlertTypes()
        {
            BackendTools.CreateInterviewWithCall(_surveyId);
            BackendTools.CreateInterviewWithCall(_surveyId);

            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);

            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;
            var endTime = startTime.AddDays(1);

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);

            var nextTime = startTime.AddSeconds(RedQuickAnswerSubmissionValue);
            AnswerOnQuestion(ProjectId, _personId, "q2", nextTime);
            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { _surveyId }, 0, 1, 0, 0);

            nextTime = nextTime.AddSeconds(AmberQuickAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);
            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { _surveyId }, 1, 1, 0, 0);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);

            task = TaskRepository.GetByPerson(_personId);
            AnswerOnQuestion(ProjectId, _personId, "q1", task.TimeStateChanged.Value);
            nextTime = task.TimeStateChanged.Value.AddSeconds(AmberAnswerSubmissionValue);

            AnswerOnQuestion(ProjectId, _personId, "q2", nextTime);
            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { _surveyId }, 1, 1, 1, 0);

            nextTime = nextTime.AddSeconds(RedAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);
            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { _surveyId }, 1, 1, 1, 1);
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void AlertsHistoryAggregatedReport_2Surveys2Persons_AlertsAreCorrect()
        {
            const string projectId2 = "p01278383";
            var surveyId2 = BackendToolsObject.CreateSurvey(projectId2);
            _surveyStateService.Open(surveyId2);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, _personId);

            var personId2 = PersonTools.CreateAssignAndLoginPersonOnSurvey(surveyId2, "u2", AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.CreateInterviewWithCall(_surveyId);
            var call = CallQueueService.GetCallAndNoLock(_surveyId, interview1.ID);
            call.Priority = 1000;
            CallQueueService.UpdateCall(call, 0);
            BackendTools.CreateInterviewWithCall(surveyId2);
            BackendTools.CreateInterviewWithCall(surveyId2);

            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;
            var endTime = startTime.AddDays(1);

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);
            var nextTime = startTime.AddSeconds(AmberQuickAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);
            task = TaskRepository.GetByPerson(_personId);
            AnswerOnQuestion(projectId2, _personId, "q1", task.TimeStateChanged.Value);
            nextTime = task.TimeStateChanged.Value.AddSeconds(RedQuickAnswerSubmissionValue);
            InternalFinishQuestion(projectId2, _personId, nextTime);

            var ws2 = new CatiWsHelper("u2", "p1");
            ws2.ConsoleService.StartInterview("", 0);
            var task2 = TaskRepository.GetByPerson(personId2);
            AnswerOnQuestion(projectId2, personId2, "q1", task2.TimeStateChanged.Value);

            nextTime = task2.TimeStateChanged.Value.AddSeconds(AmberAnswerSubmissionValue);
            InternalFinishQuestion(projectId2, personId2, nextTime);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);
            ws2.ConsoleService.WrapUp(task2.InterviewID, 1);

            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { _surveyId, surveyId2 }, 1, 1, 0, 0);
            AssertAggregatedAlertsCount(startTime, endTime, _personId, new[] { surveyId2 }, 0, 1, 0, 0);
            AssertAggregatedAlertsCount(startTime, endTime, personId2, new[] { _surveyId, surveyId2 }, 0, 0, 1, 0);
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void AlertsHistoryAggregatedReport_FilterByInterviewState_RecordsAreCorrect()
        {
            BackendTools.CreateInterviewWithCall(_surveyId);
            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;
            var endTime = startTime.AddDays(1);

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);
            var nextTime = startTime.AddSeconds(AmberQuickAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);

            var result = BvSpAlertsHistoryAggregatedReportAdapter.ExecuteEntityList(
                _personId.ToString(CultureInfo.InvariantCulture), _surveyId.ToString(CultureInfo.InvariantCulture), startTime, endTime, (int)InterviewState.INTERVIEWING);

            Assert.AreEqual(1, result.Count, "Count of records filterred by interviewing interview state");

            result = BvSpAlertsHistoryAggregatedReportAdapter.ExecuteEntityList(
                _personId.ToString(CultureInfo.InvariantCulture), _surveyId.ToString(CultureInfo.InvariantCulture), startTime, endTime, (int)InterviewState.OPENEND_REVIEW);

            Assert.AreEqual(0, result.Count, "Count of records filterred by openend review interview state");
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void BvSpAlertsHistoryReport_ApplyFilters_RecordsAreCorrect()
        {
            BackendTools.CreateInterviewWithCall(_surveyId);
            BackendTools.CreateInterviewWithCall(_surveyId);

            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);
            var nextTime = startTime.AddSeconds(AmberQuickAnswerSubmissionValue);
            AnswerOnQuestion(ProjectId, _personId, "q2", nextTime);
            nextTime = nextTime.AddSeconds(RedQuickAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);
            task = TaskRepository.GetByPerson(_personId);
            AnswerOnQuestion(ProjectId, _personId, "q1", task.TimeStateChanged.Value);
            nextTime = task.TimeStateChanged.Value.AddSeconds(RedAnswerSubmissionValue);
            AnswerOnQuestion(ProjectId, _personId, "q2", nextTime);
            nextTime = nextTime.AddSeconds(AmberAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);

            var actual = BvSpAlertsHistoryReportAdapter.ExecuteEntityList(
                _personId.ToString(CultureInfo.InvariantCulture), _surveyId.ToString(CultureInfo.InvariantCulture), "", 1, 100, "PersonId", true);
            Assert.AreEqual(4, actual.Count);

            actual = BvSpAlertsHistoryReportAdapter.ExecuteEntityList(
                null, _surveyId.ToString(CultureInfo.InvariantCulture), "", 1, 100, "PersonId", true);
            Assert.AreEqual(4, actual.Count);

            var pagingArgs = new PagingArgs
            {
                PageIndex = 1,
                PageSize = 10,
                SortField = "PersonId",
                SearchParameters = new SearchParameterCollection
                {
                    new SearchParameter
                        {
                            ColumnName = "QuestionId",
                            ColumnType = SearchColumnType.Text,
                            Operator = SearchOperator.Equal,
                            Value = "q2"
                        }
                }
            };
            int totalCounts;
            int timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            actual = ReportManager.GetAlertsHistory(new[] { _surveyId }, new[] { _personId }, pagingArgs, timezoneId, out totalCounts);
            Assert.AreEqual(2, actual.Count);

            pagingArgs.PageSize = 1;
            actual = ReportManager.GetAlertsHistory(new[] { _surveyId }, new[] { _personId }, pagingArgs, timezoneId, out totalCounts);
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void BvSpAlertsHistoryReport_1Interview_RecordIsCorrect()
        {
            BackendTools.CreateInterviewWithCall(_surveyId);
            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);
            var nextTime = startTime.AddSeconds(AmberQuickAnswerSubmissionValue);
            InternalFinishQuestion(ProjectId, _personId, nextTime);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);
            
            var actual = BvSpAlertsHistoryReportAdapter.ExecuteEntityList(
                _personId.ToString(CultureInfo.InvariantCulture), _surveyId.ToString(CultureInfo.InvariantCulture), "", 1, 100, "PersonId", true);

            Assert.AreEqual(1, actual.Count, "Count of records");
            Assert.AreEqual(_personId, actual[0].PersonId, "PersonId");
            Assert.AreEqual(PersonName, actual[0].PersonName, "PersonName");
            Assert.AreEqual(_surveyId, actual[0].SurveyId, "SurveyId");
            Assert.AreEqual(false, actual[0].Alert, "Alert");
            Assert.AreEqual((int)InterviewerSubmissionAlert.QuickAnswer, actual[0].AlertType, "AlertType");
            Assert.AreEqual(AmberQuickAnswerSubmissionValue, actual[0].AnswerDuration, "AnswerDuration");
            Assert.AreEqual(task.InterviewID, actual[0].InterviewId, "InterviewId");
            Assert.AreEqual((byte)InterviewState.INTERVIEWING, actual[0].InterviewState, "InterviewState");
            Assert.AreEqual(ProjectId, actual[0].ProjectId, "ProjectId");
            Assert.AreEqual("q1", actual[0].QuestionId, "QuestionId");
            Assert.AreEqual(startTime, actual[0].SubmissionTime, "SubmissionTime");
            Assert.AreEqual("", actual[0].SurveyName, "SurveyName");
        }

        [TestMethod, Owner(@"FIRM\AlexandeL")]
        public void BvSpAlertsHistoryReport_2InterviewIncorrectTimeOrder_RecordIsCorrect()
        {
            BackendTools.CreateInterviewWithCall(_surveyId);
            BackendTools.RunSchedulingProcedure();

            _ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(_personId);
            var startTime = task.TimeStateChanged.Value;

            AnswerOnQuestion(ProjectId, _personId, "q1", startTime);
            var nextTime = startTime.AddSeconds(5);

            AnswerOnQuestion(ProjectId, _personId, "q3", nextTime.AddSeconds(1));
            AnswerOnQuestion(ProjectId, _personId, "q2", nextTime);
            nextTime = nextTime.AddSeconds(1).AddSeconds(5);

            task = TaskRepository.GetByPerson(_personId);
            Assert.AreEqual("q3", task.State, "Current question in task");

            InternalFinishQuestion(ProjectId, _personId, nextTime);

            _ws.ConsoleService.WrapUp(task.InterviewID, 1);

            var actual = BvSpAlertsHistoryReportAdapter.ExecuteEntityList(
                _personId.ToString(CultureInfo.InvariantCulture), _surveyId.ToString(CultureInfo.InvariantCulture), "", 1, 100, "PersonId", true);

            Assert.AreEqual(1, actual.Count(), "Records count");
            Assert.AreEqual("q2", actual[0].QuestionId, "Question");
            Assert.AreEqual(1, actual[0].AnswerDuration, "AnswerDuration");
        }
    }
}
