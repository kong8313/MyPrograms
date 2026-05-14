using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.WindowsServiceTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Logger;

namespace Confirmit.CATI.IntegrationTests.Tests.RegisterServicesTest
{
    [TestClass]
    public class InstanceManagementServiceTest: BaseRegisterServiceTestClass
    {
        private StartedServicesRepository _startedServicesRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
            _startedServicesRepository = new StartedServicesRepository();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Cleanup();
        }
        
        private static void CheckThatDatabaseFilesExistInExpectedDirectory(
            int companyId,
            string expectedDataDirectory,
            string expectedLogDirectory)
        {
            string instanceDbName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);

            string pathWithDataFilename = String.Format(
                @"{0}\{1}.mdf",
                expectedDataDirectory,
                instanceDbName);

            string pathWithLogFilename = String.Format(
                @"{0}\{1}_log.ldf",
                expectedLogDirectory,
                instanceDbName);

            Assert.IsTrue(
                File.Exists(pathWithDataFilename),
                "mdf database file does not exist in expected directory");

            Assert.IsTrue(
                File.Exists(pathWithLogFilename),
                "ldf database file does not exist in expected directory");
        }

        private static Process GetCreatedPrrocess(Process[] existedProcesses, IEnumerable<Process> currentProcesses)
        {
            foreach (Process currentProcess in currentProcesses)
            {
                if (existedProcesses.All(existedProcess => existedProcess.Id != currentProcess.Id))
                {
                    return currentProcess;
                }
            }

            throw new Exception("New created process hasn't found");
        }

        /// <summary>
        /// This test checks that instance service can be registeret and unregistered successfully.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), CannotWorkInParallel]
        public void InstanceManagementServiceTest_RegisterAndUnregisterInstance_Success()
        {
            var companyId = framework.GenerateCompanyId();

            var ws = new InstanceManagementService();

            ws.RegisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

            try
            {
                // check that service is registered and started
                Assert.IsTrue(BackendInstanceRegistrator.IsInstanceRegistered(companyId));

                IsServiceStarted(MultimodeInstanceName.CompanyIdToServiceName(companyId));
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

                Assert.IsFalse(BackendInstanceRegistrator.IsInstanceRegistered(companyId));
            }
        }

        /// <summary>
        /// This test checks if instance service will be restarted after it would be killed.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), CannotWorkInParallel]
        public void InstanceManagementServiceTest_RegisterAndRecoveryServiceProcess_Success()
        {
            // We need wait some time while system will kill process.
            Thread.Sleep(2000);
            Process[] existedProcesses = Process.GetProcessesByName(GeneralConstants.ServiceBinaryName);

            //
            // Register instance
            //
            var companyId = framework.GenerateCompanyId();
            RegisterInstanceInTheConfirmlogCompanyTable(companyId, true);

            var ws = new InstanceManagementService();
            
            ws.RegisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

            try
            {
                Process[] currentProcesses = Process.GetProcessesByName(GeneralConstants.ServiceBinaryName);
                Assert.AreEqual(existedProcesses .Length + 1, currentProcesses.Length); // there should be created 1 process and no more
                Process backendProcess = GetCreatedPrrocess(existedProcesses, currentProcesses);

                //
                // Kill instance process and check that it restarted and available
                //
                backendProcess.Kill();

                //
                // We need wait some time while system will kill process.
                //
                Thread.Sleep(2000);

                Assert.IsTrue(IsServiceStarted(MultimodeInstanceName.CompanyIdToServiceName(companyId)));
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// 1. specify path where instance database should be created (emulate configuration value using mocks)
        /// 2. register instance
        /// 3. check that database files placed in expected directory
        /// 4. unregister instance
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), CannotWorkInParallel]
        public void InstanceManagementServiceTest_RegisterInstanceSetDatabasePath_Success()
        {
            var companyId = framework.GenerateCompanyId();
            string testDataPath = framework.Cfg.TestDBDataPath;
            string testLogPath = framework.Cfg.TestDBLogPath;

            //
            // set path for instance database files
            var settings = ServiceLocator.Resolve<ISystemSettings>();
            settings.SQLServer.SqlServerDataPath = testDataPath;
            settings.SQLServer.SqlServerLogPath = testLogPath;

            //
            // create instance
            var ws = new InstanceManagementService();
            ws.RegisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

            try
            {
                CheckThatDatabaseFilesExistInExpectedDirectory(
                    companyId,
                    testDataPath,
                    testLogPath);
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// 1. emulate that there are no SqlServerDataPath and SqlServerLogPath setting in app.config
        /// 2. register instance
        /// 3. check that database files placed in expected directory
        /// 4. unregister instance
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), CannotWorkInParallel]
        public void InstanceManagementServiceTest_RegisterInstanceDatabasePathSettingDoesNotExistInConfig_Success()
        {
            var companyId = framework.GenerateCompanyId();

            var settings = ServiceLocator.Resolve<ISystemSettings>();
            settings.SQLServer.SqlServerDataPath = "";
            settings.SQLServer.SqlServerLogPath = "";

            //
            // create instance
            var ws = new InstanceManagementService();
            ws.RegisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

            try
            {
                var databaseTools = new DatabaseTools(BackendInstance.Current.MasterConnectionString);

                CheckThatDatabaseFilesExistInExpectedDirectory(
                    companyId,
                    databaseTools.GetSqlServerDefaultDataPath(),
                    databaseTools.GetSqlServerDefaultLogPath());
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// This test checks that instance service state is filled successful
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK"), CannotWorkInParallel]
        public void InstanceManagementServiceTest_RegisterAndUnregisterInstance_CorrectRecordsInBvStartedServicesTable()
        {
            var companyId = framework.GenerateCompanyId();
            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);

            var ws = new InstanceManagementService();

            ws.RegisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

            try
            {
                Assert.IsTrue(_startedServicesRepository.IsServiceStarted(Environment.MachineName, serviceName));
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

                Assert.IsFalse(_startedServicesRepository.IsServiceStarted(Environment.MachineName, serviceName));
            }
        }

        /// <summary>
        /// This test checks that instance service state is filled successful
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK"), CannotWorkInParallel]
        public void InstanceManagementServiceTest_RegisterAndStopInstance_CorrectRecordsInBvStartedServicesTable()
        {
            var companyId = framework.GenerateCompanyId();
            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);

            var ws = new InstanceManagementService();

            ws.RegisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));

            try
            {
                Assert.IsTrue(_startedServicesRepository.IsServiceStarted(Environment.MachineName, serviceName));

                WinServiceTools.StopService(serviceName);

                Assert.IsFalse(_startedServicesRepository.IsServiceStarted(Environment.MachineName, serviceName));
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
                throw;
            }
            finally
            {
                ws.UnregisterSchedulingServiceInstance(companyId.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
