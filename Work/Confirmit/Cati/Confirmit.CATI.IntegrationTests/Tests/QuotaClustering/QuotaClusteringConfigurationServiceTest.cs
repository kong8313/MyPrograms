using System.Linq;
using System.Reflection.Emit;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Misc.CP.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaClustering
{
    [TestClass]
    public class QuotaClusteringConfigurationServiceTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Configure_EnableQuotaClustering_CallsAndCountersAreInitialized()
        {

            var quotaId = 1;
            var surveyId = BackendToolsObject.CreateSurvey("p000001");
            var userName = "SuperPuperMegaDron";

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

            var supervisorProvider = TestingFramework.RegistryStub<ISupervisorNameProvider, StubISupervisorNameProvider>();
            supervisorProvider.NameGet = () => userName;

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview4 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview5 = BackendTools.CreateInterviewWithCall(surveyId);

            quota.PutInterviewsInCells(
                new[] { interview1.ID, interview2.ID, interview3.ID, interview4.ID },
                new[] { 1, 2, 1, 2 });

            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();

            service.Configure(surveyId, new QuotaClusteringConfiguration(){LiveThreshod = 5, QuotaName = quota.Name});
            BackendTools.ExecuteAllAsyncOperations();
            
            Assert.AreEqual(1, CallQueueService.GetCallAndNoLock(surveyId, interview1.ID).CellId);
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveyId, interview2.ID).CellId);
            Assert.AreEqual(1, CallQueueService.GetCallAndNoLock(surveyId, interview3.ID).CellId);
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveyId, interview4.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview5.ID).CellId);

            var operation =
                BvAsyncOperationQueueAdapter.GetAll()
                    .Single(x => x.Type == (byte) OperationTypes.ConfigureClusteredQuota);

            Assert.AreEqual(userName, operation.CreatedBySupervisorName);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Configure_EnableQuotaClusteringWithActiveInterviews_LiveCountersAndCountersAreInitialized()
        {
            ICallQueueService callQueueService = ServiceLocator.Resolve<ICallQueueService>();
            
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

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview4 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview5 = BackendTools.CreateInterviewWithCall(surveyId);

            quota.PutInterviewsInCells(
                new[] { interview1.ID, interview2.ID, interview3.ID, interview4.ID },
                new[] { 1, 2, 1, 2 });

            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();
            
            bool isLocked;

            callQueueService.GetCallWithTryLock(surveyId, interview1.ID, out isLocked);
            Assert.IsTrue(isLocked);
            callQueueService.GetCallWithTryLock(surveyId, interview2.ID, out isLocked);
            Assert.IsTrue(isLocked);
            callQueueService.GetCallWithTryLock(surveyId, interview3.ID, out isLocked);
            Assert.IsTrue(isLocked);
            
            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = 1, QuotaName = quota.Name });
            BackendTools.ExecuteAllAsyncOperations();

            var cells = BvClusteredQuotaCellAdapter.GetAll().ToArray();
            
            Assert.AreEqual(2, cells.Length);

            Assert.AreEqual(2, cells.Single(x => x.CellId == 1).LiveCount);
            Assert.AreEqual(1, cells.Single(x => x.CellId == 2).LiveCount);
            

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Configure_DisableQuotaClustering_CallsAndCountersAreInitialized()
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

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview4 = BackendTools.CreateInterviewWithCall(surveyId);
            var interview5 = BackendTools.CreateInterviewWithCall(surveyId);

            quota.PutInterviewsInCells(
                new[] { interview1.ID, interview2.ID, interview3.ID, interview4.ID },
                new[] { 1, 2, 1, 2 });

            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();

            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = 5, QuotaName = quota.Name });
            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = 0, QuotaName = null });
            BackendTools.ExecuteAllAsyncOperations();

            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview1.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview2.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview3.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview4.ID).CellId);
            Assert.AreEqual(0, CallQueueService.GetCallAndNoLock(surveyId, interview5.ID).CellId);
        }
    }
}
