using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Framework.Fakes;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Confirmit.CATI.IntegrationTests.Tests.RoutineMaintenance
{
    [TestClass]
    public class RoutineMaintenanceTest : BaseMockedIntegrationTest
    {
        public override void OnPostTestInitialize()
        {
            ServiceLocator.Resolve<ISideBySideManager>();

            BvBackendInstanceAdapter.Insert(
                new BvBackendInstanceEntity
                {
                    ServiceName = "Confirmit.CATI.Backend$" + BackendInstance.Current.CompanyId
                });
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void RoutineMaintenanceThread_TestCleanup_Success()
        {
            int operationsCount = 0;
            var awaiter = TestingFramework.RegistryStub<IAsyncOperationAwaiter, StubIAsyncOperationAwaiter>();
            awaiter.AwaitBvAsyncOperationQueueEntity = e =>
            {
                var operations = BvAsyncOperationQueueAdapter.GetAll().Where(
                    op => op.Title.Equals("ExecuteRoutineMaintenance")
                    ).ToList();

                operationsCount = operations.Count();

                BackendToolsObject.ExecuteRoutineMaintenance();

                var a = new AsyncOperationAwaiter();
                return a.Await(e);
            };

            ServiceLocator.Resolve<RoutineMaintenanceThread>().RunRoutineMaintenanceForCompanies();

            var asyncOperations = BvAsyncOperationQueueAdapter.GetAll()
                .Where(asyncOperation => asyncOperation.Title.Equals("ExecuteRoutineMaintenance"))
                .ToList();

            asyncOperations.ForEach(o => Assert.AreEqual(AsyncOperationState.Completed, (AsyncOperationState)o.State));

            Assert.AreEqual(1, operationsCount);
        }
    }
}
