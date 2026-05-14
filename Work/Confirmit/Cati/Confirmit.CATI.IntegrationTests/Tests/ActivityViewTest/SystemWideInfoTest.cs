using System.Collections.Generic;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using System;

namespace Confirmit.CATI.IntegrationTests.Tests.ActivityViewTest
{
    [TestClass]
    public class SystemWideInfoTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private const string PersonName = "ppc";
        private const int BatchId = 1;
        private const int RecordsCountPerSurvey = 3;
        private const int SampleBatchId1 = 12;
        private const int SampleBatchId2 = 13;

        private static string _project1;
        private static string _project2;
        private static DatabaseEngine _confirmitDb1;
        private static DatabaseEngine _confirmitDb2;
        private int _surveyId1;
        private int _surveyId2;
        private static readonly IEnumerable<int> TimeZones1 = Enumerable.Range(1, 3);
        private static readonly IEnumerable<int> TimeZones2 = Enumerable.Range(4, 3);
        private const int StartRespId1 = 1;
        private const int StartRespId2 = 4;

        private ISurveyStateService _surveyStateService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _confirmitDb1 = ConfirmitTools.GetConfirmitSurveyDbOnClass(out _project1);
            _confirmitDb2 = ConfirmitTools.GetConfirmitSurveyDbOnClass(out _project2);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            IntegrationTestingFramework.ClassCleanup();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _backendTools.LaunchAllHoursScript();

            _surveyId1 = _backendTools.CreateSurvey(_project1, _confirmitDb1.ConnectionString);
            _surveyId2 = _backendTools.CreateSurvey(_project2, _confirmitDb2.ConnectionString);

            BvTransferArraysAdapter.Insert(new BvTransferArraysEntity { BatchID = BatchId, ItemID = _surveyId1 });
            BvTransferArraysAdapter.Insert(new BvTransferArraysEntity { BatchID = BatchId, ItemID = _surveyId2 });

            _backendTools.AddSample(
                _project1,
                SampleBatchId1,
                (int)SchedulingMode.Simple, StartRespId1, RecordsCountPerSurvey, TimeZones1);

            _backendTools.AddSample(
                _project2,
                SampleBatchId2,
                (int)SchedulingMode.Simple, StartRespId2, RecordsCountPerSurvey, TimeZones2);

            _surveyStateService.Open(_surveyId2);

            PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveyId2, PersonName, AgentTaskChoiceMode.CampaignAssignment);
            var task = BvTasksAdapter.GetAll().First();
            task.StatusLogout = 1;
            BvTasksAdapter.Update(task);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        private void AssertSistemWideInfo(int callsCount)
        {
            callsCount *= 4;
            BvSpAlert_RecalculateAllAdapter.ExecuteNonQuery(DateTime.UtcNow);

            var systemWideInfoList = BvSpGetSystemWideInfoAdapter.ExecuteEntityList(BatchId, CallCenterTools.DefaultId);

            Assert.AreEqual(1, systemWideInfoList.Count, "SystemWideInfo sp should return only 1 record");
            Assert.AreEqual(0, systemWideInfoList[0].AlertStatusOfCallsCount, "AlertStatusOfCallsCount");
            Assert.AreEqual(0, systemWideInfoList[0].AlertStatusOfLoggedInterviewersCount, "AlertStatusOfLoggedInterviewersCount");
            Assert.AreEqual(0, systemWideInfoList[0].AlertStatusOfOpenSurveysCount, "AlertStatusOfOpenSurveysCount");
            Assert.AreEqual(1, systemWideInfoList[0].LoggedInterviewersCount, "LoggedInterviewersCount");
            Assert.AreEqual(1, systemWideInfoList[0].OpenSurveysCount, "OpenSurveysCount");
            Assert.AreEqual(callsCount, systemWideInfoList[0].CallsCount, "CallsCount");
        }

        

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SystemWideInfo_GetAllStatisticsWithEmptyHistory_Returned()
        {
            AssertSistemWideInfo(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SystemWideInfo_GetAllStatistics_AllRecordsReturned()
        {
            var interview = InterviewRepository.GetById(_surveyId1, 1);
            interview.TransientState = (int)CallOutcome.Completed;

            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsExecuteSchedulingScript = false });

            AssertSistemWideInfo(1);
        }
    }
}
