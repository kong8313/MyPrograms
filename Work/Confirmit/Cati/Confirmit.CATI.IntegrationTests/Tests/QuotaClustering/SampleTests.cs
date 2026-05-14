using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaClustering
{
    [TestClass]
    public class SampleTests : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSample_QuotaClusteringAreEnabled_CallsAndCountersAreInitialized()
        {
            const string projectId = "p000123";
            const int recordsCount = 4;
            const int BatchId = 1;

            BackendToolsObject.LaunchAllHoursScript();
            int surveyId = BackendToolsObject.CreateSurvey(projectId);

            //there are should be 2*2 cells;
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            ConfirmitTools.CreateRespondentTable(TestingFramework.DbEngine);
            ConfirmitTools.FillRespondentTable(TestingFramework.DbEngine, BatchId, 1, recordsCount, Enumerable.Range(1, recordsCount));

            const int cellId1 = 2;
            const int cellId2 = 3;

            quota.PutInterviewsInCells(
                new[] { 1, 2, 4 },
                new[] { cellId1, cellId2, cellId2 });

            EnableQuotaClustering(surveyId, quota);

            BackendToolsObject.AddSample(projectId,
                BatchId,
                (int)SchedulingMode.Full);

            var actual = BvSvyScheduleAdapter.GetAll().OrderBy(y => y.InterviewID).Select(x => x.CellId).ToArray();
            Trace.TraceInformation("actual.Length = {0}", actual.Length);
            CollectionAssert.AreEqual(new[] { cellId1, cellId2, 0, cellId2 }, actual);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void AddSample_SimpleScheduling_QuotaClusteringAreEnabled_CallsAndCountersAreInitialized()
        {
            const string projectId = "p000123";
            const int recordsCount = 4;
            const int BatchId = 1;

            BackendToolsObject.LaunchAllHoursScript();
            int surveyId = BackendToolsObject.CreateSurvey(projectId);
            //there are should be 2*2 cells;
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            ConfirmitTools.CreateRespondentTable(TestingFramework.DbEngine);
            ConfirmitTools.FillRespondentTable(TestingFramework.DbEngine, BatchId, 1, recordsCount, Enumerable.Range(1, recordsCount));

            const int cellId1 = 2;
            const int cellId2 = 3;

            quota.PutInterviewsInCells(
                new[] { 1, 2, 4 },
                new[] { cellId1, cellId2, cellId2 });

            EnableQuotaClustering(surveyId, quota);

            BackendToolsObject.AddSample(projectId,
                BatchId,
                (int)SchedulingMode.Simple);

            var actual = BvSvyScheduleAdapter.GetAll().OrderBy(y => y.InterviewID).Select(x => x.CellId).ToArray();
            Trace.TraceInformation("actual.Length = {0}", actual.Length);
            CollectionAssert.AreEqual(new[] { cellId1, cellId2, 0, cellId2 }, actual);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSample_QuotaClusteringAreDisabled_CallsAndCountersAreInitialized()
        {
            const string projectId = "p000123";
            const int recordsCount = 4;
            const int BatchId = 1;

            BackendToolsObject.LaunchAllHoursScript();
            int surveyId = BackendToolsObject.CreateSurvey(projectId);

            //there are should be 2*2 cells;
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            ConfirmitTools.CreateRespondentTable(TestingFramework.DbEngine);
            ConfirmitTools.FillRespondentTable(TestingFramework.DbEngine, BatchId, 1, recordsCount, Enumerable.Range(1, recordsCount));

            const int cellId1 = 2;
            const int cellId2 = 3;

            quota.PutInterviewsInCells(
                new[] { 1, 2, 4 },
                new[] { cellId1, cellId2, cellId2 });

            //EnableQuotaClustering(surveyId, quota);

            BackendToolsObject.AddSample(projectId,
                BatchId,
                (int)SchedulingMode.Full);

            var actual = BvSvyScheduleAdapter.GetAll().OrderBy(y => y.InterviewID).Select(x => x.CellId).ToArray();
            Trace.TraceInformation("actual.Length = {0}", actual.Length);
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0 }, actual);
        }

        private static void EnableQuotaClustering(int surveyId, TestQuota quota)
        {
            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();

            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = 5, QuotaName = quota.Name });
            BackendTools.ExecuteAllAsyncOperations();
        }
    }
}
