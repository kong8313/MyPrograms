using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;
using BootstrapperLibrary;
using BootstrapperLibrary.Interfaces;
using CatiInstallation.Properties;
using Confirmit.CATI.Common.PerformanceCounters;
using Confirmit.CATI.Core.PerformanceCounters;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.Configuration;
using Confirmit.Databases;
using Confirmit.DataServices.RDataAccess;
using CustomActionLibrary;
using Microsoft.Win32;

namespace CatiInstallation
{
    public class InstallationFacade
    {
        private readonly ILogger _logger;
        private readonly CatiSetupEngine _catiSetupEngine;
        private readonly ConfigsEngine _configsEngine;

        public InstallationFacade(ILogger logger)
        {
            _logger = logger;
            _catiSetupEngine = new CatiSetupEngine(_logger);
            _configsEngine = new ConfigsEngine(_logger);
        }
        
        public string GetTypeOfActionWithDatabase(string catiSqlServerName, string saLogin, string saPassword)
        {
            var confirmitCatiValidator = new ConfirmitCATIValidator();
            return confirmitCatiValidator.GetTypeOfActionWithDatabase(catiSqlServerName, CatiSetupConstants.CatiDefaultDatabaseName, saLogin, saPassword);
        }

        public void BackupIsAliveHtmFile(IIsAliveHtmEngine isAliveHtmEngine, string isAlivePageUrl, string isAlivePageRenameTimeout)
        {
            if(isAliveHtmEngine.BackupIsAliveHtmFile(isAlivePageUrl))
            {
                _logger.WriteLog(true, "Wait {0} sec after renaming of IsAlive.htm file", isAlivePageRenameTimeout);
                int renameTimeout = Convert.ToInt32(isAlivePageRenameTimeout) * 1000;
                Thread.Sleep(renameTimeout);

                _logger.WriteLog(true, "Continue installation");
            }
            else
            {
                _logger.WriteLog(true, "Skip renaming because IsAlive.htm file doesn't exist");
            }
        }

        public string GetSchemeAndHostFromConfirmDatabase(IConfirmitCatiEngine confirmitCatiEngine, IDatabaseEngine databaseEngine, string parameterName)
        {
            string urlFromConfirmDatabase = confirmitCatiEngine.GetConfirmParameterValue(CatiSetupConstants.ConfirmDatabaseName, databaseEngine, parameterName);
            return confirmitCatiEngine.GetSchemeAndHostFromUrl(urlFromConfirmDatabase);
        }

        public void StopAllCatiServices(string catiSqlServerName, string sideBySideName, string currentVersion)
        {
            _catiSetupEngine.StopAllCatiServices(catiSqlServerName, sideBySideName, currentVersion);
        }

        public void UpdateDatabases()
        {
            new ConfigurationLoader().LoadConfiguration();

            var sqlServerNames = ConfirmitConfiguration.CatiServers.Select(x => x.Value.SqlServerName).ToList();
            if (!sqlServerNames.Contains(ConfirmitConfiguration.CatiSqlServerName))
                sqlServerNames.Add(ConfirmitConfiguration.CatiSqlServerName);

            var confirmlogConnectionString = DbLib.GetConfirmlogConnectInfo().GetConnectString();
            foreach (var sqlServerName in sqlServerNames)
            {
                var connectionString = DbLib.GetConnectInfo("master", sqlServerName).GetConnectString();
                var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
                var dbUpdateLibraryWorker = new DbUpdateLibraryWorker(
                    _logger,
                    sqlServerName,
                    connectionStringBuilder.UserID,
                    connectionStringBuilder.Password,
                    confirmlogConnectionString,
                    null);

                int errorCode = dbUpdateLibraryWorker.UpdateDatabases(false);
                if (errorCode == 1)
                {
                    throw new DatabaseUpdatePossibilityException(Resources.DatabaseUpdateProcessWasFailed);
                }

                if (errorCode != 0)
                {
                    throw new Exception(Resources.DatabaseUpdateProcessWasFailed);
                }
            }
        }

        public void RemoveCatiServices(string sideBySideName)
        {
            _catiSetupEngine.RemoveAllBackendServices(sideBySideName);
        }

        public void CreateDefaultDatabase(string catiSqlServerName, string saLogin, string saPassword, string confirmitDeployLogin,
            string confirmitDeployPassword, string confirmlogConnectionString, string confirmitLinkedServerName,
            string mdfPath, string ldfPath, string catiDefaultDbRecoveryModel, string productName)
        {
            _logger.WriteLog("Create new database and add assembly");

            var saDatabaseEngine = new CatiDatabaseEngine(_logger, catiSqlServerName, saLogin, saPassword);
            var confirmitDeployDatabaseEngine = new CatiDatabaseEngine(_logger, catiSqlServerName, confirmitDeployLogin, confirmitDeployPassword);
            var dbUpdateWorker = new DbUpdateLibraryWorker(_catiSetupEngine.Logger, catiSqlServerName, saLogin, saPassword,
                confirmlogConnectionString, confirmitLinkedServerName);

            confirmitDeployDatabaseEngine.CreateDatabase(CatiSetupConstants.CatiDefaultDatabaseName, mdfPath, ldfPath, catiDefaultDbRecoveryModel);

            //
            // Enable clr for server if needed
            //
            string commandText = "select value_in_use from sys.configurations where name = 'clr enabled'";
            if (confirmitDeployDatabaseEngine.ExecuteScalar<int>(commandText) == 0)
            {
                commandText = "exec sp_configure 'clr enabled', 1; reconfigure";
                saDatabaseEngine.ExecuteNonQuery(commandText);

                TopMostMessageBox.Show(string.Format(Resources.ClrIntegrationEnabledForSqlServer, catiSqlServerName), productName, MessageBoxIcon.Warning);
            }

            //
            // Disable clr strict security for server if needed
            //
            commandText = "select value_in_use from sys.configurations where name = 'clr strict security'";
            if (confirmitDeployDatabaseEngine.ExecuteScalar<int?>(commandText) == 1)
            {
                commandText = @"
                    EXEC sp_configure 'show advanced options', 1 
                    RECONFIGURE;

                    EXEC sp_configure 'clr strict security', 0;
                    RECONFIGURE;";
                saDatabaseEngine.ExecuteNonQuery(commandText);

                TopMostMessageBox.Show(string.Format(Resources.ClrStrictSecurityDisabledForSqlServer, catiSqlServerName), productName, MessageBoxIcon.Warning);
            }

            confirmitDeployDatabaseEngine.ExecuteGoQuery(CatiSetupConstants.CatiDefaultDatabaseName, Resources.Base_Confirmit_CATI_Database);
            if (dbUpdateWorker.UpdateDatabases(true) != 0)
            {
                throw new Exception(Resources.DatabaseUpdateProcessWasFailed);
            }
        }

        public void InstallTestCertificatesAndConfiguringHttpListenerProgressStatusIfNeeded(string isLoadBalancedEnvironment, string installLocation, string certificateType,
            string testCertificateName, string certificatePath, string certificatePassword, string productName, string overrideCertificate = "True")
        {
            if (Convert.ToBoolean(isLoadBalancedEnvironment) == false)
            {
                try
                {
                    var isListenerRegistered = _catiSetupEngine.IsHttpListenerRegistered();
                    if (isListenerRegistered && Convert.ToBoolean(overrideCertificate) == false)
                    {
                        _logger.WriteLog(true, TraceEventType.Information, $"Skip registration of http listener because listener is already registered and Cati.SSL.OverrideCertificateIfExist was set to false");
                        return;
                    }

                    string certificateThumbprint = _catiSetupEngine.InstallCertificateIfNeeded(
                        installLocation,
                        certificateType,
                        testCertificateName,
                        certificatePath,
                        certificatePassword);

                    _catiSetupEngine.ConfigureHttpListener(certificateThumbprint);
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(TraceEventType.Error, ex.ToString());
                    TopMostMessageBox.Show(
                        string.Format(Resources.ErrorDuringCertificateInstallationYouHaveToInstallItByHand, ex.Message),
                        productName,
                        MessageBoxIcon.Warning);
                }
            }
        }

        public void ConfigureSupervisorConfig(string supervisorLocation, string catiConnectionString,
            string sessionStateMode, string redisHostName, string redisPassword, string sessionStateConnectionString, string sessionStateCookieName, string supervisorVirtualDirectoryName,
            string confirmitKeepSessionAspxUrl, string igResFolderName, string confirmitLogPath)
        {
            _configsEngine.ConfigureSupervisorConfig(
                        supervisorLocation, catiConnectionString, sessionStateMode, redisHostName, redisPassword, sessionStateConnectionString, 
                        sessionStateCookieName, confirmitKeepSessionAspxUrl, igResFolderName, confirmitLogPath);

            string supervisorWebConfigPath = Path.Combine(supervisorLocation, "Web.config");

            if (sessionStateMode != "InProc")
            {
                _configsEngine.EncryptSection(supervisorWebConfigPath, supervisorVirtualDirectoryName, "system.web/sessionState");
            }
            
            _configsEngine.EncryptSection(supervisorWebConfigPath, supervisorVirtualDirectoryName, "Telerik.Reporting");
        }

        public void ConfigureBackendConfig(string installLocation, string isLoadBalancedEnvironment, string confirmitLogPath)
        {
            _configsEngine.ConfigureBackendConfig(installLocation,  Convert.ToBoolean(isLoadBalancedEnvironment), confirmitLogPath);
        }

        public void ConfigureIISApplication(string productName, string supervisorAppPoolName,
            string supervisorSiteName, string supervisorVirtualDirectoryName, string supervisorLocation)
        {
            var iisEngine = new IISEngine(_logger);

            _catiSetupEngine.ConfigureIISApplication(iisEngine, productName, supervisorAppPoolName, supervisorSiteName, supervisorVirtualDirectoryName, supervisorLocation);
        }

        public void SetContentExpiration(string productVersion, string supervisorSiteName, string supervisorVirtualDirectoryName)
        {
            var foldersForContentExpiration = new[] { "images", "Client", "Styles", "ig_res_" + productVersion };

            var iisEngine = new IISEngine(_logger);
            iisEngine.SetMaxAgeContentExpirationForSpecifiedFolders(supervisorSiteName, supervisorVirtualDirectoryName, foldersForContentExpiration, new TimeSpan(30, 0, 0, 0));
        }

        public void ConfigureBackendAndSupervisorSettings(string catiSqlServerName, string confirmitDeployLogin, string confirmitDeployPassword, Dictionary<string, string> settings, bool setDatabaseSettings)
        {
            var databaseEngine = new CatiDatabaseEngine(_logger, catiSqlServerName, confirmitDeployLogin, confirmitDeployPassword);

            databaseEngine.ConfigureBvSystemSetting(CatiSetupConstants.CatiDefaultDatabaseName, settings);

            if (setDatabaseSettings)
            {
                databaseEngine.SetDatabaseSettings(CatiSetupConstants.CatiDefaultDatabaseName);
            }
        }

        public void CreateAndRunDefaultCatiService(string installLocation, string sideBySideName, string isLoadBalancedEnvironment)
        {
            string backendExePath = Path.Combine(installLocation, "Confirmit.CATI.Backend.exe");
            _catiSetupEngine.ExternalInvoker.Invoke("\"" + backendExePath + "\"", "-service", 10000);

            // Register backend performance counters
            var countersContainer = new PerformanceCountersContainer(new PerformanceCounterFactory());

            var categoryCreator = new PerformanceCategoryCreator();

            categoryCreator.Initialize(
                PerformanceCountersContainer.CategoryName,
                "",
                countersContainer.PerformanceCounters,
                PerformanceCounterCategoryType.MultiInstance,
                false);


            _catiSetupEngine.StartAllCatiServicesAndWaitUntilTheyStarted(sideBySideName, isLoadBalancedEnvironment);
        }

        public void StartAllCatiServicesAndWaitUntilTheyStarted(string sideBySideName, string isLoadBalancedEnvironment)
        {
            _catiSetupEngine.StartAllCatiServicesAndWaitUntilTheyStarted(sideBySideName, isLoadBalancedEnvironment);
        }

        public void SetPermissionsForPdbFiles(string installLocation)
        {
            foreach (var fileName in Directory.GetFiles(installLocation, "*.pdb"))
            {
                _catiSetupEngine.AddFileSecurity(fileName, "Everyone", FileSystemRights.FullControl, AccessControlType.Allow);
            }
        }
    }
}
