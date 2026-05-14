using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ManagementService;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.IntegrationTests.Tests.ActivityViewTest
{
    [TestClass]
    public class SurveyActivityViewTests : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;
        private IPersonRepository _personRepository;

        private RespondentTools _respondentTools;

        private const string Project1 = "p0001231";
        private int _surveyId1;
        private int _interviewerId = 0;

        public override void OnPostTestInitialize()
        {
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();

            BackendToolsObject.LaunchAllHoursScript();

            _respondentTools = new RespondentTools(TestingFramework);

            _surveyId1 = BackendToolsObject.CreateSurvey(Project1);
            _surveyStateService.Open(_surveyId1);

            var person = new BvPersonEntity
            {
                Name = "interviewer1",
                CallCenterID = CallCenterTools.DefaultId
            };

            _interviewerId = _personRepository.Insert(person);
        }

        private InterviewControlData GetDefaultControlData()
        {
            return new InterviewControlData
            {
                interviewerID = _interviewerId,
                interviewID = 1,
                lastCallTime = DateTime.Now,
                projectID = Project1,
                roleID = 2,
                totalDuration = 3
            };
        }

        private InterviewHistoryData GetDefaultHistoryData()
        {
            return new InterviewHistoryData
            {
                grossDuration = 1,
                interviewerID = _interviewerId,
                interviewID = 1,
                netDuration = 2,
                projectID = Project1,
                roleID = 2,
                totalDuration = 3,
                time = DateTime.Now,
            };
        }

        private List<BvSpGetSurveyActivityWithAlertsEntity> GetSurveyAggregateData(IEnumerable<int> surveyIds, params CallOutcome[] itses)
        {
            return GetSurveyAggregateData(surveyIds, false, itses);
        }

        private List<BvSpGetSurveyActivityWithAlertsEntity> GetSurveyAggregateData(IEnumerable<int> surveyIds, bool onlyCatiInterviews, params CallOutcome[] itses)
        {
            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();
            BvSpAggregateSurveyProcessDeltaAdapter.ExecuteNonQuery();
            BvSpSampleStatusSummaryProcessDeltaAdapter.ExecuteNonQuery();

            var now = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(now - now.TimeOfDay, "13");

            return GetSurveyAggregateData(now, surveyIds, onlyCatiInterviews, itses);
        }

        private List<BvSpGetSurveyActivityWithAlertsEntity> GetSurveyAggregateData(DateTime? now, IEnumerable<int> surveyIds, bool onlyCatiInterviews, params CallOutcome[] itses)
        {
            BvSpAlert_RecalculateAllAdapter.ExecuteNonQuery(now);

            Func<int, int?> getItsAtIndex = index =>
            {
                if (index >= itses.Length) return (int?)null;
                return itses.Skip(index).Cast<int>().FirstOrDefault();
            };

            using (TransferBatch batch = TransferBatch.Create())
            {
                batch.Insert(surveyIds);

                return BvSpGetSurveyActivityWithAlertsAdapter.ExecuteEntityList(batch.Value, false,
                    getItsAtIndex(0),
                    getItsAtIndex(1),
                    getItsAtIndex(2),
                    getItsAtIndex(3),
                    getItsAtIndex(4),
                    onlyCatiInterviews);
            }
        }

        private void AddSample(int firstInterviewId, int interviewsCount)
        {
            BackendToolsObject.AddSample(Project1, 1, 2, firstInterviewId, interviewsCount, null);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRecordLifeTime_AddAndRemoveSurvey_RecordExistsIfSurveyExists()
        {
            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(1, aggregateDataRecords.Count, "Aggregated data is not returned for existence survey");

            BackendTools.DeleteSurvey(Project1);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords.Count, "Aggregated data is returned for deleted survey");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_RequestPartOfExistenceRecords_NecessaryRecordIsReturn()
        {
            const string project2 = "p0001232";

            var surveyId2 = BackendToolsObject.CreateSurvey(project2);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { surveyId2 });

            Assert.AreEqual(1, aggregateDataRecords.Count, "Aggregated data should be returned only for one survey");
            Assert.AreEqual(surveyId2, aggregateDataRecords[0].SurveySID, "Aggregated data was returned for other survey");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_AddAndRemoveInterview_SuspendedCallsCountIsCorrect()
        {
            const int firstInterviewId = 1;
            const int interviewsCount = 1;

            AddSample(firstInterviewId, interviewsCount);

            using (var batch = TransferBatch.Create())
            {
                batch.Insert(new[]{firstInterviewId});

                CallQueueService.DeleteCalls(_surveyId1, batch.Value);
            }

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(1, aggregateDataRecords[0].SuspendedCallsCount, "SuspendedCallsCount should be 1 if one interview has been created");

            _respondentTools.DeleteRespondentsAsync(Project1, new[]{firstInterviewId});

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].SuspendedCallsCount, "SuspendedCallsCount should be 0 if interview has been deleted");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_AddAndRemoveCallFromInterview_SuspendedAndScheduledCallsCountAreCorrect()
        {
            const int firstInterviewId = 1;
            const int interviewsCount = 1;

            AddSample(firstInterviewId, interviewsCount);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].SuspendedCallsCount, "Only scheduled call should exists");
            Assert.AreEqual(1, aggregateDataRecords[0].ScheduledCallsCount, "Only scheduled call should exists");

            CallQueueService.DeleteCall(_surveyId1, firstInterviewId);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(1, aggregateDataRecords[0].SuspendedCallsCount, "Only suspended call should exists");
            Assert.AreEqual(0, aggregateDataRecords[0].ScheduledCallsCount, "Only suspended call should exists");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_AddCallAndSetThreshold_ScheduledCallsCountAlertIsCorrect()
        {
            BvSpThresholds_insertAdapter.ExecuteNonQuery(0, 6, 1, 2);
            BackendTools.CreateInterviewWithCall(_surveyId1);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(1, aggregateDataRecords[0].AlertStatusOfScheduledCallsCount, "AlertStatusOfScheduledCallsCount");

            BackendTools.CreateInterviewWithCall(_surveyId1);
            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(2, aggregateDataRecords[0].AlertStatusOfScheduledCallsCount, "AlertStatusOfScheduledCallsCount");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_SetAppointmentAndGetAppointmentCall_NextAppointmentTimeIsCorrect()
        {
            DateTime firstDate = DateTime.UtcNow.AddHours(2).CutMilliseconds();
            DateTime secondDate = DateTime.UtcNow.AddHours(3).CutMilliseconds();
            DateTime thirdDate = DateTime.UtcNow.AddHours(1).CutMilliseconds();

            var interview1 = BackendTools.CreateInterviewWithCall(_surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(_surveyId1);

            var interview3 = BackendTools.NewInterview(_surveyId1);
            BackendTools.CreateInterview(interview3);

            BackendTools.AddAppointmentAndLinkItWithCall(interview1.ID, _surveyId1, firstDate);
            BackendTools.AddAppointmentAndLinkItWithCall(interview2.ID, _surveyId1, secondDate);
            BackendTools.AddAppointment(interview3.ID, _surveyId1, thirdDate);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(firstDate, aggregateDataRecords[0].NextAppointmentTime, "NextAppointmentTime is incorrect");

            var personId = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveyId1, "Spi", AgentTaskChoiceMode.Automatic);
            BackendTools.RunSchedulingProcedure(DateTime.UtcNow.AddHours(10));
            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(DateTime.UtcNow.AddHours(10)));
            TaskService.LookupByPersonSid(personId, 0);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(secondDate, aggregateDataRecords[0].NextAppointmentTime, "NextAppointmentTime is incorrect");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_SetAppointmentAndSetThreshold_NextAppointmentTimeAlertIsCorrect()
        {
            BvSpThresholds_insertAdapter.ExecuteNonQuery(0, 3, 50, 100);

            DateTime nowDate = DateTime.UtcNow.CutMilliseconds();
            DateTime firstDate = nowDate.AddSeconds(50);
            DateTime secondDate = nowDate.AddSeconds(120);
            DateTime thirdDate = nowDate.AddSeconds(180);
            nowDate = nowDate.AddSeconds(200);

            var interview1 = BackendTools.CreateInterviewWithCall(_surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(_surveyId1);
            var interview3 = BackendTools.CreateInterviewWithCall(_surveyId1);

            BackendTools.AddAppointmentAndLinkItWithCall(interview1.ID, _surveyId1, firstDate);
            BackendTools.AddAppointmentAndLinkItWithCall(interview2.ID, _surveyId1, secondDate);
            BackendTools.AddAppointmentAndLinkItWithCall(interview3.ID, _surveyId1, thirdDate);

            var aggregateDataRecords = GetSurveyAggregateData(nowDate, new[] { _surveyId1 }, false);
            Assert.AreEqual(2, aggregateDataRecords[0].AlertStatusOfNextAppointmentTime, "AlertStatusOfNextAppointmentTime");
            BvAppointmentAdapter.DeleteByCondition(
                "InterviewSID = @InterviewSID", new SqlParameter("@InterviewSID", interview1.ID));

            aggregateDataRecords = GetSurveyAggregateData(nowDate, new[] { _surveyId1 }, false);
            Assert.AreEqual(1, aggregateDataRecords[0].AlertStatusOfNextAppointmentTime, "AlertStatusOfNextAppointmentTime");
            BvAppointmentAdapter.DeleteByCondition(
                "InterviewSID = @InterviewSID", new SqlParameter("@InterviewSID", interview2.ID));

            aggregateDataRecords = GetSurveyAggregateData(nowDate, new[] { _surveyId1 }, false);
            Assert.AreEqual(0, aggregateDataRecords[0].AlertStatusOfNextAppointmentTime, "AlertStatusOfNextAppointmentTime");
            BvAppointmentAdapter.DeleteByCondition(
                "InterviewSID = @InterviewSID", new SqlParameter("@InterviewSID", interview3.ID));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_LoginUser_CountOfLoggedInUsersIsCorrect()
        {
            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].InterviewersLoggedCount, "There are no logged in interviewers");

            var personId = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveyId1, "Spi", AgentTaskChoiceMode.CampaignAssignment);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(1, aggregateDataRecords[0].InterviewersLoggedCount, "There is only 1 logged in interviewer");

            TaskService.RemoveTaskAndLogoutPerson(personId);
            _personRepository.Delete(personId);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].InterviewersLoggedCount, "There are no logged in interviewers");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_AssignAndDeassignUser_CountOfAssignedUsersIsCorrect()
        {
            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].AssignedInterviewersCount, "There are no assigned interviewers");

            var personId = PersonTools.CreatePerson("Spi");
            BackendTools.AssignCatiPersonToSurvey(_surveyId1, personId);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(1, aggregateDataRecords[0].AssignedInterviewersCount, "There is only 1 assigned interviewer");

            PersonTools.RemovePerson(personId);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].InterviewersLoggedCount, "There are no assigned interviewers");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_Assign2UserAndSetThresholds_AlertOfAssignedUsersIsCorrect()
        {
            BvSpThresholds_insertAdapter.ExecuteNonQuery(0, 9, 1, 2);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, aggregateDataRecords[0].AlertStatusOfAssignedInterviewersCount, "AlertStatusOfAssignedInterviewersCount");

            var personId1 = PersonTools.CreatePerson("Spi1");
            BackendTools.AssignCatiPersonToSurvey(_surveyId1, personId1);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(1, aggregateDataRecords[0].AlertStatusOfAssignedInterviewersCount, "AlertStatusOfAssignedInterviewersCount");

            var personId2 = PersonTools.CreatePerson("Spi2");
            BackendTools.AssignCatiPersonToSurvey(_surveyId1, personId2);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(2, aggregateDataRecords[0].AlertStatusOfAssignedInterviewersCount, "AlertStatusOfAssignedInterviewersCount");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_AddSampleAndPassSomeinterviews_TotalSampleSizeIsCorrect()
        {
            const int firstInterviewId = 1;
            const int interviewsCount = 3;

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].TotalSampleSize, "There is no loaded samples");

            AddSample(firstInterviewId, interviewsCount);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(3, aggregateDataRecords[0].TotalSampleSize, "There are 3 interviews with fresh sample state");

            var interview = InterviewRepository.GetById(_surveyId1, 1);
            interview.TransientState = (int)CallOutcome.Completed;
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions(){ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, opType = OperationType.MovedAndReschedule});

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(2, aggregateDataRecords[0].TotalSampleSize, "There are 2 interviews with fresh sample state");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_PassSomeInterview_MinutesSpentWorkingOnSurveyIsCorrect()
        {
            int firstInterviewId = 1;
            const int interviewsCount = 3;

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(0, aggregateDataRecords[0].MinutesSpentWorkingOnSurvey, "There was no some activity for survey MinutesSpentWorkingOnSurvey should be 0");

            AddSample(firstInterviewId, interviewsCount);

            var historyData = GetDefaultHistoryData();

            historyData.totalDuration = 3;
            historyData.interviewID = firstInterviewId++;
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());
            historyData.totalDuration = 4;
            historyData.interviewID = firstInterviewId++;
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());
            historyData.totalDuration = 5;
            historyData.interviewID = firstInterviewId;
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });

            Assert.AreEqual(3+4+5, aggregateDataRecords[0].MinutesSpentWorkingOnSurvey, "MinutesSpentWorkingOnSurvey is incorrect");

            new DatabaseEngine().ExecuteNonQuery("UPDATE BvSurveyListAlertsViewConfiguration SET IdlePeriodMaxCountOfChecks = 0, IdlePeriodMaxSeconds = 0", CommandType.Text);

            BvSpAggregateSurveyProcessDeltaAdapter.ExecuteNonQuery();

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(3+4+5, aggregateDataRecords[0].MinutesSpentWorkingOnSurvey, "MinutesSpentWorkingOnSurvey is incorrect");
        }

        private void CheckAggregateDataForInterviewWithSpecificITS(Action<InterviewHistoryData> changeIts,
            Func<BvSpGetSurveyActivityWithAlertsEntity, int?> fieldAccessor)
        {
            const int firstInterviewId = 1;
            const int interviewsCount = 2;

            AddSample(firstInterviewId, interviewsCount);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, fieldAccessor(aggregateDataRecords[0]), "There is no processed interviews");

            var historyData = GetDefaultHistoryData();
            changeIts(historyData);
            historyData.interviewID = firstInterviewId;
            historyData.time = DateTime.Parse("09:55:11 10.10.2009");

            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            historyData.time = DateTime.Parse("09:55:09 10.10.2009");

            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            historyData.interviewID = firstInterviewId + 1;
            historyData.time = DateTime.Parse("10:00:00 10.10.2009");

            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            aggregateDataRecords = GetSurveyAggregateData(DateTime.Parse("10:10:10 10.10.2009"), new[] { _surveyId1 }, false);

            Assert.AreEqual(2*4, fieldAccessor(aggregateDataRecords[0]), "StrikeRate should be 2");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_PassSomeInterview_StrikeRateIsCorrect()
        {
            CheckAggregateDataForInterviewWithSpecificITS(
                historyData => historyData.status = "13", aggregateDataRecord => aggregateDataRecord.StrikeRate);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_PassSomeInterview_CallsCountIsCorrect()
        {
            CheckAggregateDataForInterviewWithSpecificITS(
                historyData => { }, aggregateDataRecord => aggregateDataRecord.CountCalls);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_PassSomeInterview_AverageDurationIsCorrect()
        {
            int firstInterviewId = 1;
            const int interviewsCount = 2;

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, aggregateDataRecords[0].AvgDuration, "Average duration is incorrect");

            AddSample(firstInterviewId, interviewsCount);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, aggregateDataRecords[0].AvgDuration, "Average duration is incorrect");

            var historyData = GetDefaultHistoryData();
            historyData.totalDuration = 5;
            historyData.interviewID = firstInterviewId;
            historyData.time = DateTime.Parse("09:55:11 10.10.2009");
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            historyData.totalDuration = 7;
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            historyData.totalDuration = 111;
            historyData.time = DateTime.Parse("09:55:09 10.10.2009");
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            historyData.totalDuration = 24;
            historyData.interviewID = ++firstInterviewId;
            historyData.time = DateTime.Parse("10:09:09 10.10.2009");
            BackendToolsObject.SaveInterviewHistoryAndControlData(historyData, GetDefaultControlData());

            aggregateDataRecords = GetSurveyAggregateData(DateTime.Parse("10:10:10 10.10.2009"), new[] { _surveyId1 }, false);
            Assert.AreEqual(12, aggregateDataRecords[0].AvgDuration, "Average duration is incorrect");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_Login2Interviewers_InterviewersLoggedCountPrevIsCorrect()
        {
            PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveyId1, "Spi1", AgentTaskChoiceMode.CampaignAssignment);
            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, aggregateDataRecords[0].InterviewersLoggedCountPrev, "InterviewersLoggedCountPrev is incorrect");

            PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveyId1, "Spi2", AgentTaskChoiceMode.CampaignAssignment);
            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(1, aggregateDataRecords[0].InterviewersLoggedCountPrev, "InterviewersLoggedCountPrev is incorrect");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_Add2ScheduledCalls_ScheduledCallsCountPrevIsCorrect()
        {
            BackendTools.CreateInterviewWithCall(_surveyId1);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, aggregateDataRecords[0].ScheduledCallsCountPrev, "ScheduledCallsCountPrev is incorrect");

            BackendTools.CreateInterviewWithCall(_surveyId1);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(1, aggregateDataRecords[0].ScheduledCallsCountPrev, "ScheduledCallsCountPrev is incorrect");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivityRequestRecords_Add2Interview_SuspendedCallsCountPrevIsCorrect()
        {
            var interview = BackendTools.NewInterview(_surveyId1);
            BackendTools.CreateInterview(interview);

            var aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(0, aggregateDataRecords[0].SuspendedCallsCountPrev, "SuspendedCallsCountPrev is incorrect");

            interview = BackendTools.NewInterview(_surveyId1);
            BackendTools.CreateInterview(interview);

            aggregateDataRecords = GetSurveyAggregateData(new[] { _surveyId1 });
            Assert.AreEqual(1, aggregateDataRecords[0].SuspendedCallsCountPrev, "SuspendedCallsCountPrev is incorrect");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyActivityRequestRecords_SetupAllCustomIts_CustomITSesCountersAndAllertsAreCorrect()
        {
            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData(2) {Tag = "S1.I1", ITS = CallOutcome.Busy},
                            new InterviewData(4) {Tag = "S1.I2", ITS = CallOutcome.Appointment},
                            new InterviewData(6) {Tag = "S1.I3", ITS = CallOutcome.FreshSample},
                            new InterviewData(8) {Tag = "S1.I4", ITS = CallOutcome.NoReply},
                            new InterviewData(10) {Tag = "S1.I5", ITS = CallOutcome.Completed},
                            new InterviewData(1) {Tag = "S1.I6", ITS = CallOutcome.TelephonyFailure},
                        }
                    },
                    new SurveyData() { Tag = "S2", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData(3) {Tag = "S2.I1", ITS = CallOutcome.Busy},
                            new InterviewData(5) {Tag = "S2.I2", ITS = CallOutcome.Appointment},
                            new InterviewData(7) {Tag = "S2.I3", ITS = CallOutcome.FreshSample},
                            new InterviewData(9) {Tag = "S2.I4", ITS = CallOutcome.NoReply},
                            new InterviewData(11) {Tag = "S2.I5", ITS = CallOutcome.Completed},
                            new InterviewData(1) {Tag = "S2.I6", ITS = CallOutcome.TelephonyFailure},
                        }
                    },
                },
                Alerts = new[] {
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Busy, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Appointment, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.FreshSample, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.NoReply, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Completed, Amber = 5, Red = 10 },
                }
            }.Create();

            var s1 = context.GetSurvey("S1");
            var s2 = context.GetSurvey("S2");

            var aggregateDataRecords = GetSurveyAggregateData(new[] { s1.Id, s2.Id }, 
                CallOutcome.Completed, 
                CallOutcome.NoReply,
                CallOutcome.FreshSample,
                CallOutcome.Appointment,
                CallOutcome.Busy).OrderBy(x => x.SurveySID).ToArray();

            Assert.AreEqual(s1.Id, aggregateDataRecords[0].SurveySID, "Wrong surveyId");
            Assert.AreEqual(10, aggregateDataRecords[0].CustomITS1_Cnt, "Wrong CustomITS1_Cnt");
            Assert.AreEqual(2, aggregateDataRecords[0].CustomITS1_Alert, "Wrong CustomITS1_Alert");
            Assert.AreEqual(8, aggregateDataRecords[0].CustomITS2_Cnt, "Wrong CustomITS2_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[0].CustomITS2_Alert, "Wrong CustomITS2_Alert");
            Assert.AreEqual(6, aggregateDataRecords[0].CustomITS3_Cnt, "Wrong CustomITS3_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[0].CustomITS3_Alert, "Wrong CustomITS3_Alert");
            Assert.AreEqual(4, aggregateDataRecords[0].CustomITS4_Cnt, "Wrong CustomITS4_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS4_Alert, "Wrong CustomITS4_Alert");
            Assert.AreEqual(2, aggregateDataRecords[0].CustomITS5_Cnt, "Wrong CustomITS5_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS5_Alert, "Wrong CustomITS5_Alert");

            Assert.AreEqual(s2.Id, aggregateDataRecords[1].SurveySID, "Wrong surveyId");
            Assert.AreEqual(11, aggregateDataRecords[1].CustomITS1_Cnt, "Wrong CustomITS1_Cnt");
            Assert.AreEqual(2, aggregateDataRecords[1].CustomITS1_Alert, "Wrong CustomITS1_Alert");
            Assert.AreEqual(9, aggregateDataRecords[1].CustomITS2_Cnt, "Wrong CustomITS2_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[1].CustomITS2_Alert, "Wrong CustomITS2_Alert");
            Assert.AreEqual(7, aggregateDataRecords[1].CustomITS3_Cnt, "Wrong CustomITS3_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[1].CustomITS3_Alert, "Wrong CustomITS3_Alert");
            Assert.AreEqual(5, aggregateDataRecords[1].CustomITS4_Cnt, "Wrong CustomITS4_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[1].CustomITS4_Alert, "Wrong CustomITS4_Alert");
            Assert.AreEqual(3, aggregateDataRecords[1].CustomITS5_Cnt, "Wrong CustomITS5_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[1].CustomITS5_Alert, "Wrong CustomITS5_Alert");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyActivityRequestRecords_SetupTwoCustomIts_CustomITSesCountersAndAllertsAreCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData(2) {Tag = "S1.I1", ITS = CallOutcome.Busy},
                            new InterviewData(4) {Tag = "S1.I2", ITS = CallOutcome.Appointment},
                            new InterviewData(6) {Tag = "S1.I3", ITS = CallOutcome.FreshSample},
                            new InterviewData(8) {Tag = "S1.I4", ITS = CallOutcome.NoReply},
                            new InterviewData(10) {Tag = "S1.I5", ITS = CallOutcome.Completed},
                            new InterviewData(1) {Tag = "S1.I6", ITS = CallOutcome.TelephonyFailure},
                        }
                    },
                    new SurveyData() { Tag = "S2", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData(3) {Tag = "S2.I1", ITS = CallOutcome.Busy},
                            new InterviewData(5) {Tag = "S2.I2", ITS = CallOutcome.Appointment},
                            new InterviewData(7) {Tag = "S2.I3", ITS = CallOutcome.FreshSample},
                            new InterviewData(9) {Tag = "S2.I4", ITS = CallOutcome.NoReply},
                            new InterviewData(11) {Tag = "S2.I5", ITS = CallOutcome.Completed},
                            new InterviewData(1) {Tag = "S2.I6", ITS = CallOutcome.TelephonyFailure},
                        }
                    },
                },
                Alerts = new[] {
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Busy, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Appointment, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.FreshSample, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.NoReply, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Completed, Amber = 5, Red = 10 },
                }
            }.Create();

            var s1 = context.GetSurvey("S1");
            var s2 = context.GetSurvey("S2");

            var aggregateDataRecords = GetSurveyAggregateData(new[] { s1.Id, s2.Id },
                CallOutcome.NoReply,
                CallOutcome.Appointment).OrderBy(x => x.SurveySID).ToArray();

            Assert.AreEqual(s1.Id, aggregateDataRecords[0].SurveySID, "Wrong surveyId");
            Assert.AreEqual(8, aggregateDataRecords[0].CustomITS1_Cnt, "Wrong CustomITS1_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[0].CustomITS1_Alert, "Wrong CustomITS1_Alert");
            Assert.AreEqual(4, aggregateDataRecords[0].CustomITS2_Cnt, "Wrong CustomITS2_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS2_Alert, "Wrong CustomITS2_Alert");
            Assert.AreEqual(null, aggregateDataRecords[0].CustomITS3_Cnt, "Wrong CustomITS3_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS3_Alert, "Wrong CustomITS3_Alert");
            Assert.AreEqual(null, aggregateDataRecords[0].CustomITS4_Cnt, "Wrong CustomITS4_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS4_Alert, "Wrong CustomITS4_Alert");
            Assert.AreEqual(null, aggregateDataRecords[0].CustomITS5_Cnt, "Wrong CustomITS5_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS5_Alert, "Wrong CustomITS5_Alert");

            Assert.AreEqual(s2.Id, aggregateDataRecords[1].SurveySID, "Wrong surveyId");
            Assert.AreEqual(9, aggregateDataRecords[1].CustomITS1_Cnt, "Wrong CustomITS1_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[1].CustomITS1_Alert, "Wrong CustomITS1_Alert");
            Assert.AreEqual(5, aggregateDataRecords[1].CustomITS2_Cnt, "Wrong CustomITS2_Cnt");
            Assert.AreEqual(1, aggregateDataRecords[1].CustomITS2_Alert, "Wrong CustomITS2_Alert");
            Assert.AreEqual(null, aggregateDataRecords[1].CustomITS3_Cnt, "Wrong CustomITS3_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[1].CustomITS3_Alert, "Wrong CustomITS3_Alert");
            Assert.AreEqual(null, aggregateDataRecords[1].CustomITS4_Cnt, "Wrong CustomITS4_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[1].CustomITS4_Alert, "Wrong CustomITS4_Alert");
            Assert.AreEqual(null, aggregateDataRecords[1].CustomITS5_Cnt, "Wrong CustomITS5_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[1].CustomITS5_Alert, "Wrong CustomITS5_Alert");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyActivityRequestRecords_ConfigureInterviewHistoryInLastHour_StrikeRateAndCallCountAndDurationAndAlertsAreCorrected()
        {
            new DateTimeMocker(TestingFramework).MockDate("2017-11-01T10:50:00");
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData{Tag = "S1.I1", ITS = CallOutcome.Busy, 
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2017-10-01T09:00:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 100},
                                    new InterviewHisotryData(){Time = "2017-11-01T09:00:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 100},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:00:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 40},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:30:00", Person = "P1", ITS = CallOutcome.Completed, Duration = 40},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:45:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 100},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:50:00", Person = "P1", ITS = CallOutcome.Completed, Duration = 100},
                                }},
                        }
                    },
                    new SurveyData() { Tag = "S2", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData{Tag = "S2.I1", ITS = CallOutcome.Busy,
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2017-10-01T09:00:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 120},
                                    new InterviewHisotryData(){Time = "2017-11-01T09:00:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 120},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:00:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 60},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:05:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 60},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:30:00", Person = "P1", ITS = CallOutcome.Completed, Duration = 60},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:45:00", Person = "P1", ITS = CallOutcome.Busy, Duration = 120},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:47:00", Person = "P1", ITS = CallOutcome.Completed, Duration = 120},
                                    new InterviewHisotryData(){Time = "2017-11-01T10:50:00", Person = "P1", ITS = CallOutcome.Completed, Duration = 120},
                                }},
                        }
                    },
                },
                Alerts = new[] { 
                    new AlertData { Amber = 5, Red = 9, Type = BvThresholdType.CountCallsAlert}, 
                    new AlertData { Amber = 3, Red = 7, Type = BvThresholdType.StrikeRateAlert}, 
                },
                Persons = new[] { new PersonData() { Tag = "P1"} }
            }.Create();

            var s1 = context.GetSurvey("S1");
            var s2 = context.GetSurvey("S2");

            var aggregateDataRecords = GetSurveyAggregateData(new[] { s1.Id, s2.Id });

            Assert.AreEqual(s1.Id, aggregateDataRecords[0].SurveySID, "Wrong surveyId");
            Assert.AreEqual(100, aggregateDataRecords[0].AvgDuration, "Wrong AvgDuration");
            Assert.AreEqual(70, aggregateDataRecords[0].AvgDuration1h, "Wrong AvgDuration1h");
            Assert.AreEqual(8, aggregateDataRecords[0].CountCalls, "Wrong CountCalls");
            Assert.AreEqual(4, aggregateDataRecords[0].CountCalls1h, "Wrong CountCalls1h");
            Assert.AreEqual(4, aggregateDataRecords[0].StrikeRate, "Wrong StrikeRate");
            Assert.AreEqual(2, aggregateDataRecords[0].StrikeRate1h, "Wrong StrikeRate1h");
            Assert.AreEqual(480, aggregateDataRecords[0].MinutesSpentWorkingOnSurvey, "Wrong MinutesSpentWorkingOnSurvey");
            Assert.AreEqual(380, aggregateDataRecords[0].MinutesSpentWorkingOnSurveyInDay, "Wrong MinutesSpentWorkingOnSurveyInDay");

            Assert.AreEqual(1, aggregateDataRecords[0].AlertStatusOfStrikeRate, "Wrong AlertStatusOfStrikeRate");
            Assert.AreEqual(0, aggregateDataRecords[0].AlertStatusOfStrikeRate1h, "Wrong AlertStatusOfStrikeRate1h");
            Assert.AreEqual(1, aggregateDataRecords[0].AlertStatusOfCountCalls, "Wrong AlertStatusOfCountCalls");
            Assert.AreEqual(0, aggregateDataRecords[0].AlertStatusOfCountCalls1h, "Wrong AlertStatusOfCountCalls1h");

            Assert.AreEqual(s2.Id, aggregateDataRecords[1].SurveySID, "Wrong surveyId");
            Assert.AreEqual(120, aggregateDataRecords[1].AvgDuration, "Wrong AvgDuration");
            Assert.AreEqual(90, aggregateDataRecords[1].AvgDuration1h, "Wrong AvgDuration1h");
            Assert.AreEqual(12, aggregateDataRecords[1].CountCalls, "Wrong CountCalls");
            Assert.AreEqual(6, aggregateDataRecords[1].CountCalls1h, "Wrong CountCalls1h");
            Assert.AreEqual(8, aggregateDataRecords[1].StrikeRate, "Wrong StrikeRate");
            Assert.AreEqual(3, aggregateDataRecords[1].StrikeRate1h, "Wrong StrikeRate1h");
            Assert.AreEqual(780, aggregateDataRecords[1].MinutesSpentWorkingOnSurvey, "Wrong MinutesSpentWorkingOnSurvey");
            Assert.AreEqual(660, aggregateDataRecords[1].MinutesSpentWorkingOnSurveyInDay, "Wrong MinutesSpentWorkingOnSurveyInDay");

            Assert.AreEqual(2, aggregateDataRecords[1].AlertStatusOfStrikeRate, "Wrong AlertStatusOfStrikeRate");
            Assert.AreEqual(1, aggregateDataRecords[1].AlertStatusOfStrikeRate1h, "Wrong AlertStatusOfStrikeRate1h");
            Assert.AreEqual(2, aggregateDataRecords[1].AlertStatusOfCountCalls, "Wrong AlertStatusOfCountCalls");
            Assert.AreEqual(1, aggregateDataRecords[1].AlertStatusOfCountCalls1h, "Wrong AlertStatusOfCountCalls1h");
        }


        [TestMethod, Owner(@"FIRM\Egork")]
        public void SurveyActivityRequestRecords_InterviewsWIthDifferentLastChannelIdCreated_AggregateOnlyCatiData_CorrectCntAmountsReturned()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", IsOpen = true,
                        Interviews = new[] {
                            new InterviewData(2) {Tag = "S1.I1", ITS = CallOutcome.Busy, LastChannelId="1"},
                            new InterviewData(4) {Tag = "S1.I2", ITS = CallOutcome.Appointment, LastChannelId="1"},
                            new InterviewData(6) {Tag = "S1.I3", ITS = CallOutcome.FreshSample, LastChannelId="1"},
                            new InterviewData(8) {Tag = "S1.I4", ITS = CallOutcome.NoReply, LastChannelId="1"},
                            new InterviewData(10) {Tag = "S1.I5", ITS = CallOutcome.Completed, LastChannelId="1"},
                            new InterviewData(1) {Tag = "S1.I6", ITS = CallOutcome.TelephonyFailure,  LastChannelId="1"},
                        }
                    }
                },
                Alerts = new[] {
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Busy, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Appointment, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.FreshSample, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.NoReply, Amber = 5, Red = 10 },
                    new ExtendedStatusAlertData() { ITS = CallOutcome.Completed, Amber = 5, Red = 10 },
                }
            }.Create();

            var s1 = context.GetSurvey("S1");


            var aggregateDataRecords = GetSurveyAggregateData(new[] { s1.Id},
                true,
                CallOutcome.Completed,
                CallOutcome.NoReply,
                CallOutcome.FreshSample,
                CallOutcome.Appointment,
                CallOutcome.Busy).OrderBy(x => x.SurveySID).ToArray();

            Assert.AreEqual(s1.Id, aggregateDataRecords[0].SurveySID, "Wrong surveyId");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS1_Cnt, "Wrong CustomITS1_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS2_Cnt, "Wrong CustomITS2_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS3_Cnt, "Wrong CustomITS3_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS4_Cnt, "Wrong CustomITS4_Cnt");
            Assert.AreEqual(0, aggregateDataRecords[0].CustomITS5_Cnt, "Wrong CustomITS5_Cnt");

            aggregateDataRecords = GetSurveyAggregateData(new[] { s1.Id },
               false,
               CallOutcome.Completed,
               CallOutcome.NoReply,
               CallOutcome.FreshSample,
               CallOutcome.Appointment,
               CallOutcome.Busy).OrderBy(x => x.SurveySID).ToArray();

            Assert.AreEqual(s1.Id, aggregateDataRecords[0].SurveySID, "Wrong surveyId");
            Assert.AreEqual(10, aggregateDataRecords[0].CustomITS1_Cnt, "Wrong CustomITS1_Cnt");
            Assert.AreEqual(8, aggregateDataRecords[0].CustomITS2_Cnt, "Wrong CustomITS2_Cnt");
            Assert.AreEqual(6, aggregateDataRecords[0].CustomITS3_Cnt, "Wrong CustomITS3_Cnt");
            Assert.AreEqual(4, aggregateDataRecords[0].CustomITS4_Cnt, "Wrong CustomITS4_Cnt");
            Assert.AreEqual(2, aggregateDataRecords[0].CustomITS5_Cnt, "Wrong CustomITS5_Cnt");
        }
    }
}
