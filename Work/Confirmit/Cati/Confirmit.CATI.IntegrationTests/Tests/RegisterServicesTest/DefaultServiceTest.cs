using System;
using System.Diagnostics;
using System.Threading;

using Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.WindowsServiceTools;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RegisterServicesTest
{
    [TestClass]
    public class DefaultServiceTest : BaseRegisterServiceTestClass
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

        [TestMethod, Owner(@"FIRM\SvetlanaT"), CannotWorkInParallel, Bug(48059)]
        public void DefaultServiceNotStarted_AddNewInstanceToDatabaseAndThenStartDefaultInstance_NewInstanceServiceCreatedAndStarted()
        {
            var defaultServiceName = MultimodeInstanceName.GetDefaultServiceName();

            var companyId = framework.GenerateCompanyId();
            RegisterInstanceInTheConfirmlogCompanyTable(companyId, true);


            try
            {
                // Register default instance service.

                var backendInstanceRegistrator = new BackendInstanceRegistrator();
                if (BackendInstanceRegistrator.IsInstanceRegistered(0))
                {
                    backendInstanceRegistrator.UnRegisterServiceForDefaultInstance();
                }

                backendInstanceRegistrator.RegisterServiceForDefaultInstance();

                //
                // Add new instance record in BvBackendInstance table to the default database.
                // Default instance should detect this on sturtup and create NT service.
                //
                string serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);
                string databaseName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);
                string connectionString = framework.GetCatiSqlServerConnectionString(databaseName);
                new DatabaseTools(BackendInstance.Current.MasterConnectionString).CreateNewInstanceDatabase(databaseName,  string.Empty, string.Empty);

                var instanceEntity = new BvBackendInstanceEntity { ServiceName = ServiceLocator.Resolve<ISideBySideManager>().RemoveSideBySideNameFromServiceName(serviceName) };
                BvBackendInstanceAdapter.Insert(instanceEntity);

                // Start default instance.
                WinServiceTools.StartService(defaultServiceName, ServiceLocator.Resolve<ISystemSettings>().Server.ServiceStartTimeout);
                Assert.IsTrue(IsServiceStarted(defaultServiceName), "Default service is not started");

                Assert.IsTrue(WaitUntilInstanceRegistered(companyId), "Instance is not registered");
                Assert.IsTrue(IsServiceStarted(serviceName), "Instance service is not started");
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                WinServiceTools.StopService(MultimodeInstanceName.GetDefaultServiceName(), Settings.Default.ServiceStopTimeout);

                if (companyId != 0)
                {
                    var ws = new InstanceManagementService();
                    ws.UnregisterSchedulingServiceInstance(companyId.ToString());
                }
            }
        }
    }
}
