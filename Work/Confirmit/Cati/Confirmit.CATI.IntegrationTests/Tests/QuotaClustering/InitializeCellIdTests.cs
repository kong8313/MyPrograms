using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaClustering
{
    [TestClass]
    public class InitializeCellIdTests : BaseMockedIntegrationTest
    {

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddCall_AddNewCallsWithDifferentCells_CellIdAreInitialized()
        {
            var quotaId = 1;
            var surveyId = BackendToolsObject.CreateSurvey("p000001");

            var quota = TestQuota.Create(TestingFramework.DbEngine,
               surveyId,
               quotaId,
               new[] { "q1" },
               new[] { 2 },
               new[] { 3, 6 },
               new[] { 10, 10 });

            var quotaInfoService = TestingFramework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>();
            quotaInfoService.GetQuotaTableBvSurveyEntityString = (x, y) => quota.TableName;
            quotaInfoService.GetQuotaFieldsInt32String = (x, y) => quota.FieldNames;

            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();

            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = 5, QuotaName = quota.Name });
            BackendTools.ExecuteAllAsyncOperations();
            
            var interview1 = CreateInterview(surveyId);
            var interview2 = CreateInterview(surveyId);
            var interview3 = CreateInterview(surveyId);
            var interview4 = CreateInterview(surveyId);
            var interview5 = CreateInterview(surveyId);
            
            quota.PutInterviewsInCells(
                new[] { interview1.ID, interview2.ID, interview3.ID, interview4.ID },
                new[] { 1, 2, 1, 2 });

            AddCall(interview1);
            AddCall(interview2);
            AddCall(interview3);
            AddCall(interview4);
            AddCall(interview5);
            

            Assert.AreEqual(1, CallQueueService.GetCallAndNoLock(surveyId, interview1.ID).CellId);
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveyId, interview2.ID).CellId);
            Assert.AreEqual(1, CallQueueService.GetCallAndNoLock(surveyId, interview3.ID).CellId);
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveyId, interview4.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview5.ID).CellId);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Activate_ActivateCallsWithDifferentCells_CellIdAreInitialized()
        {
            var quotaId = 1;
            var surveyId = BackendToolsObject.CreateSurvey("p000001");

            var quota = TestQuota.Create(TestingFramework.DbEngine,
               surveyId,
               quotaId,
               new[] { "q1" },
               new[] { 2 },
               new[] { 3, 6 },
               new[] { 10, 10 });

            var quotaInfoService = TestingFramework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>();
            quotaInfoService.GetQuotaTableBvSurveyEntityString = (x, y) => quota.TableName;
            quotaInfoService.GetQuotaFieldsInt32String = (x, y) => quota.FieldNames;

            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();

            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = 5, QuotaName = quota.Name });
            BackendTools.ExecuteAllAsyncOperations();

            var interview1 = CreateInterview(surveyId);
            var interview2 = CreateInterview(surveyId);
            var interview3 = CreateInterview(surveyId);
            var interview4 = CreateInterview(surveyId);
            var interview5 = CreateInterview(surveyId);

            quota.PutInterviewsInCells(
                new[] { interview1.ID, interview2.ID, interview3.ID, interview4.ID },
                new[] { 1, 2, 1, 2 });

            CallTools.ActivateCalls(surveyId, 1, CallStates.All, 0, (int)CallShiftType.None, null, true,
                new[] {interview1.ID, interview2.ID, interview3.ID, interview4.ID, interview5.ID});

            Assert.AreEqual(1, CallQueueService.GetCallAndNoLock(surveyId, interview1.ID).CellId);
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveyId, interview2.ID).CellId);
            Assert.AreEqual(1, CallQueueService.GetCallAndNoLock(surveyId, interview3.ID).CellId);
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveyId, interview4.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview5.ID).CellId);
        }

        private void AddCall(BvInterviewEntity interview)
        {
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);
        }

        private static BvInterviewEntity CreateInterview(int surveyId)
        {
            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);
            return interview;
        }
    }
}
