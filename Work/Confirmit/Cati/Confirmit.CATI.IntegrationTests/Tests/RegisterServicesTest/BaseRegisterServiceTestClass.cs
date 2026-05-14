using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceProcess;
using System.Threading;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RegisterServicesTest
{
    public class BaseRegisterServiceTestClass
    {
        protected readonly IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;

        protected void Initialize()
        {
            framework.TestInitialize();

            Environment.SetEnvironmentVariable(GeneralConstants.TestCatiConnectionString, BackendInstance.Current.ConnectionString, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(GeneralConstants.TestConfirmConnectionString, BackendInstance.Current.ConnectionString, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(GeneralConstants.TestConfirmlogConnectionString, BackendInstance.Current.ConfirmlogConnectionString, EnvironmentVariableTarget.Machine);

            CreateAddonCustomerTable();
            CreateDeletedCatiDatabasesTable();
        }

        protected void Cleanup()
        {
            framework.TestCleanup();

            Environment.SetEnvironmentVariable(GeneralConstants.TestCatiConnectionString, null, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(GeneralConstants.TestConfirmConnectionString, null, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(GeneralConstants.TestConfirmlogConnectionString, null, EnvironmentVariableTarget.Machine);
        }

        /// <summary>
        /// Returns true if service is started and false otherwise.
        /// </summary>
        /// <param name="serviceName">Service name to check</param>
        /// <returns></returns>
        internal static bool IsServiceStarted(string serviceName)
        {
            using (var service = new ServiceController(serviceName))
            {
                try
                {
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                    return true;
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    return false;
                }
            }
        }

        protected void RegisterInstanceInTheConfirmlogCompanyTable(int companyId, bool cleanTable)
        {
            var databaseEngine = new DatabaseEngine(BackendInstance.Current.ConfirmlogConnectionString);

            if (cleanTable)
            {
                const string sql1 = @"DELETE FROM dbo.[company]";

                databaseEngine.ExecuteNonQuery(sql1, CommandType.Text);
            }

            string sql2 =
                string.Format(
                    @"INSERT dbo.[company] ([companyid], [Name], [CatiCompanyIdentifier]) VALUES ({0}, 'Company{1}', 'Company{2}Alias')",
                    companyId, companyId, companyId);

            databaseEngine.ExecuteNonQuery(sql2, CommandType.Text);
        }

        /// <summary>
        /// Create "addon_customer" table
        /// </summary>
        private void CreateAddonCustomerTable()
        {
            var databaseEngine = new DatabaseEngine(BackendInstance.Current.ConnectionString);

            var columns = new[]
            {
                new KeyValuePair<string, DataType>("addon_companyid", DataType.Int),
                new KeyValuePair<string, DataType>("addon_id", DataType.Int)
            };

            databaseEngine.CreateTable("addon_customer", columns);
        }

        /// <summary>
        /// Create "DeletedCatiDatabases" table
        /// </summary>
        private void CreateDeletedCatiDatabasesTable()
        {
            var databaseEngine = new DatabaseEngine(BackendInstance.Current.ConnectionString);

            var columns = new[]
            {
                new KeyValuePair<string, DataType>("CompanyId", DataType.Int),
                new KeyValuePair<string, DataType>("DeletedDate", DataType.DateTime)
            };

            databaseEngine.CreateTable("DeletedCatiDatabases", columns);
        }

        public bool WaitUntilInstanceRegistered(int companyId, int waitTimeInSec = 60)
        {
            int i = 0;
            while (i < waitTimeInSec * 1000)
            {
                if(BackendInstanceRegistrator.IsInstanceRegistered(companyId))
                {
                    return true;
                }

                i += 100;
                Thread.Sleep(100);
            }

            return false;
        }
    }
}
