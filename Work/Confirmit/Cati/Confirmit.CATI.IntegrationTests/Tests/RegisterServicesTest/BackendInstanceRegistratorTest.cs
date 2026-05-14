using System.Data.SqlClient;
using System.Globalization;
using System.Threading;

using Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.WindowsServiceTools;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RegisterServicesTest
{
    [TestClass]
    public class BackendInstanceRegistratorTest : BaseRegisterServiceTestClass
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Cleanup();
        }
        
        /// <summary>
        /// Register and start default service
        /// Create database for the new instance
        /// In the default instance table [BvBackendInstance] add record for the new instance
        /// Check that new instance service was created and started 
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT"), CannotWorkInParallel]
        public void ResynchronizeLocalServicesWithDatabase_RecordAddedIntoBvBackendInstance_ServiceCreatedAndStarted()
        {
            int companyId = 0;

            try
            {
                companyId = framework.GenerateCompanyId();
                RegisterInstanceInTheConfirmlogCompanyTable(companyId, true);
                string serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);
                string databaseName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);
                string connectionString = framework.GetCatiSqlServerConnectionString(databaseName);
                new DatabaseTools(BackendInstance.Current.MasterConnectionString).CreateNewInstanceDatabase(databaseName, string.Empty, string.Empty);

                //
                // Add new instance record in BvBackendInstance table to the default database.
                // Default instance should detect this and create/run instance.
                //
                var instanceEntity = new BvBackendInstanceEntity { ServiceName = ServiceLocator.Resolve<ISideBySideManager>().RemoveSideBySideNameFromServiceName(serviceName) };
                BvBackendInstanceAdapter.Insert(instanceEntity);

                BackendInstanceRegistrator.ResynchronizeLocalServicesWithDatabase();

                //
                // Give default instance some time to register and start instance.
                // TODO: improve checking service start to avoid time leak
                //
                Thread.Sleep(2000);

                Assert.IsTrue(BackendInstanceRegistrator.IsInstanceRegistered(companyId));
                Assert.IsTrue(IsServiceStarted(serviceName));
            }
            finally
            {
                if (companyId != 0)
                {
                    var ws = new InstanceManagementService();
                    ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// This test checks that instance services can be successfully synchronized with default database data.
        /// 1. register instance (A)
        /// 2. register instance (B)
        /// 3. in the default instance table [BvBackendInstance] delete record for A and add record for instance (C)
        /// 4. run ResynchronizeWithDatabase
        /// 5. check that service for instance (A) was deleted, (B) stays, and (C) was added
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT"), CannotWorkInParallel]
        public void ResynchronizeLocalServicesWithDatabase_ServiceAndAnotherRecordWasDeleted_ServiceStarted()
        {
            var ws = new InstanceManagementService();

            var companyIdA = framework.GenerateCompanyId();
            RegisterInstanceInTheConfirmlogCompanyTable(companyIdA, true);
            string serviceNameA = MultimodeInstanceName.CompanyIdToServiceName(companyIdA);
            ws.RegisterSchedulingServiceInstance(companyIdA.ToString(CultureInfo.InvariantCulture));
            Thread.Sleep(1000);

            var companyIdB = framework.GenerateCompanyId();
            RegisterInstanceInTheConfirmlogCompanyTable(companyIdB, false);
            string serviceNameB = MultimodeInstanceName.CompanyIdToServiceName(companyIdB);
            ws.RegisterSchedulingServiceInstance(companyIdB.ToString(CultureInfo.InvariantCulture));
            Thread.Sleep(1000);

            var companyIdC = framework.GenerateCompanyId();
            RegisterInstanceInTheConfirmlogCompanyTable(companyIdC, false);
            string serviceNameC = MultimodeInstanceName.CompanyIdToServiceName(companyIdC);
            ws.RegisterSchedulingServiceInstance(companyIdC.ToString(CultureInfo.InvariantCulture));
            Thread.Sleep(1000);

            //
            // Remove instance A record from BvBackendInstance table of default database.
            // ResynchronizeWithDatabase should delete instance A service locally.
            //
            BvBackendInstanceAdapter.DeleteByCondition(
                "[ServiceName] = @ServiceName",
                new SqlParameter("@ServiceName", ServiceLocator.Resolve<ISideBySideManager>().RemoveSideBySideNameFromServiceName(serviceNameA)));

            //
            // Remove instance C service locally.
            // ResynchronizeWithDatabase should create instance C service locally.
            //
            using(var winServiceTools = new WinServiceTools())
            {
                winServiceTools.UnregisterService(serviceNameC);
            }

            // run ResynchronizeWithDatabase
            BackendInstanceRegistrator.ResynchronizeLocalServicesWithDatabase();

            // check that 2 services are registered and started
            try
            {
                Assert.IsFalse(BackendInstanceRegistrator.IsInstanceRegistered(companyIdA));
                Assert.IsTrue(BackendInstanceRegistrator.IsInstanceRegistered(companyIdB));
                Assert.IsTrue(BackendInstanceRegistrator.IsInstanceRegistered(companyIdC));

                Assert.IsTrue(IsServiceStarted(serviceNameB));
                Assert.IsTrue(IsServiceStarted(serviceNameC));
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyIdB.ToString(CultureInfo.InvariantCulture));
                ws.UnregisterSchedulingServiceInstance(companyIdC.ToString(CultureInfo.InvariantCulture));
                //
                // drop database for instanceNameA
                //
                var integrFramework = IntegrationTestingFramework.Instance;
                var databaseHelper = new DatabaseTools(integrFramework.GetCatiSqlServerConnectionString("master"));
                databaseHelper.DropDatabase(MultimodeInstanceName.CompanyIdToDatabaseName(companyIdA));
            }
        }
    }
}
