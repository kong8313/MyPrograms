using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BootstrapperLibrary;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using CustomActionLibrary;
using Microsoft.Web.Administration;
using Resources = CatiInstallation.Properties.Resources;
using System;

namespace CatiInstallation
{
    public class InstallationVerifier
    {
        private readonly ILogger _logger;
        private readonly CatiSetupEngine _catiSetupEngine;
        private readonly IPrereqChecker _prereqChecker;
        private readonly IConfirmitCATIValidator _confirmitCatiValidator;
        private readonly ICertificateEngine _certificateEngine;
        private readonly IIsAliveHtmEngine _isAliveHtmEngine;

        private BackendParameters _backendParameters;
        private SupervisorParameters _supervisorParameters;

        public InstallationVerifier(ILogger logger, IPrereqChecker prereqChecker,
            IConfirmitCATIValidator confirmitCatiValidator, ICertificateEngine certificateEngine)
            : this(logger, prereqChecker, confirmitCatiValidator, certificateEngine, null)
        {
        }

        public InstallationVerifier(ILogger logger, IPrereqChecker prereqChecker, 
            IConfirmitCATIValidator confirmitCatiValidator, ICertificateEngine certificateEngine, IIsAliveHtmEngine isAliveHtmEngine)
        {
            _logger = logger;
            _prereqChecker = prereqChecker;
            _confirmitCatiValidator = confirmitCatiValidator;
            _certificateEngine = certificateEngine;
            _isAliveHtmEngine = isAliveHtmEngine;
            _catiSetupEngine = new CatiSetupEngine(_logger);
        }

        public void VerifyBackendParameters(BackendParameters parameters)
        {
            try
            {
                _backendParameters = parameters;

                string errorMessage = VerifyBackendParametersAvailability();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    throw new MessageException(errorMessage, TraceEventType.Warning);
                }

                CheckPrerequisites();

                VerifyDatabaseSettings();

                VerifySQLPathsAvailability();

                VerifyCertificate();

                VerifyEmail();

                VerifyConfirmAndConfirmlogSqlSettings();

                VerifyConfirmitLinkedServer();

                VerifyIsAlivePage();

                VerifyMsiParameters();

                VerifyDtc();
            }
            catch (Exception ex)
            {
                _logger.WriteLog(true, "En error occured during VerifyBackendParameters: {0}", ex);

                throw;
            }
        }

        private void VerifyIsAlivePage()
        {            
            if (_backendParameters.IsLoadBalancedEnvironment == "True")
            {
                _logger.WriteLog(true, "VerifyIsAlivePage");

                _confirmitCatiValidator.ValidateIsAlivePageUrl(_backendParameters.LoadBalancerIsAlivePageUrl, _isAliveHtmEngine);
                _confirmitCatiValidator.ValidateIntParameter(_backendParameters.LoadBalancerIsAlivePageRenameTimeout, "Cati.LoadBalancer.IsAlivePageRenameTimeout");
            }
        }       

        public void VerifySupervisorParameters(SupervisorParameters parameters)
        {
            _logger.WriteLog(true, "VerifySupervisorParameters");

            _supervisorParameters = parameters;

            string errorMessage = VerifySupervisorParametersAvailability();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new MessageException(errorMessage, TraceEventType.Warning);
            }

            _confirmitCatiValidator.ValidateSqlServerConnection(_supervisorParameters.CatiSqlServerName,
                _supervisorParameters.CatiSqlAdminUserName, _supervisorParameters.CatiSqlAdminPassword);

            VerifySQLSessionStateSettings();

            VerifySupervisorSiteName();
        }

        private void VerifySQLPathsAvailability()
        {
            _logger.WriteLog(true, "VerifySQLPathsAvailability");

            _confirmitCatiValidator.ValidateDataAndLogPathParameters(_backendParameters.CatiDatabasesDataFilePath, _backendParameters.CatiDatabasesLogsFilePath);

            var catiDatabaseEngine = new CatiDatabaseEngine(_logger, _backendParameters.CatiSqlServerName, _backendParameters.CatiSqlUserName, _backendParameters.CatiSqlPassword);

            if (!string.IsNullOrEmpty(_backendParameters.CatiDatabasesDataFilePath))
            {
                _catiSetupEngine.CheckDatabaseCreationAbility(
                    catiDatabaseEngine, _backendParameters.CatiDatabasesDataFilePath, _backendParameters.CatiDatabasesLogsFilePath, _backendParameters.CatiDefaultDbRecoveryModel, Resources.IncorrectPathsWhereCatiDatabaseFilessWillBeStored);
            }
        }

        private void VerifyCertificate()
        {
            _logger.WriteLog(true, "VerifyCertificate");

            if (_backendParameters.IsLoadBalancedEnvironment == "False" && _backendParameters.CertificateType == "Real")
            {                
                string errMessage = _certificateEngine.VerifyCertificateFromFile(_backendParameters.CertificatePath, _backendParameters.CertificatePassword);
                if (!string.IsNullOrEmpty(errMessage))
                {
                    throw new ValidateException(errMessage);
                }
            }
        }

        private void VerifyConfirmitLinkedServer()
        {
            _logger.WriteLog(true, "VerifyConfirmitLinkedServer");

            var confirmConnectionStringBuilder = new SqlConnectionStringBuilder(_backendParameters.ConfirmConnectionString);

            if (confirmConnectionStringBuilder.DataSource.ToLower() == _backendParameters.CatiSqlServerName.ToLower() && !string.IsNullOrEmpty(_backendParameters.ConfirmitLinkedServerName))
            {
                throw new ValidateException(Resources.ConfirmitLinkedServerHasToBeEmpty);
            }

            if (confirmConnectionStringBuilder.DataSource.ToLower() != _backendParameters.CatiSqlServerName.ToLower())
            {
                if (string.IsNullOrEmpty(_backendParameters.ConfirmitLinkedServerName))
                {
                    throw new ValidateException(Resources.ConfirmitLinkedServerHasToBeFilled);
                }

                _confirmitCatiValidator.ValidateConfirmitLinkedServer(
                    _backendParameters.CatiSqlServerName, _backendParameters.CatiSqlAdminUserName, _backendParameters.CatiSqlAdminPassword, _backendParameters.ConfirmitLinkedServerName);
            }
        }

        private void VerifyMsiParameters()
        {
            if (_backendParameters.TypeOfActionWithDatabase == "CreateNewDB")
            {
                return;
            }

            _logger.WriteLog(true, "VerifyMsiParameters");

            var catiDatabaseEngine = new CatiDatabaseEngine(_logger, _backendParameters.CatiSqlServerName, _backendParameters.CatiSqlUserName, _backendParameters.CatiSqlPassword);

            if (!string.IsNullOrEmpty(_backendParameters.MsiInstallLocation))
            {
                string installLocation = catiDatabaseEngine.GetSettingValueFromDefaultCatiDatabase("Setup.InstallLocation");
                if (!string.IsNullOrEmpty(installLocation) && installLocation.TrimEnd('\\') != _backendParameters.MsiInstallLocation.TrimEnd('\\'))
                {
                    throw new ValidateException(string.Format("'Cati.Msi.Parameters.InstallLocation' has a different value to that given in the 'Setup.InstallLocation' parameter of the BvSystemSettings table in the default CATI database.\r\nOctopus value: {0}\r\nDatabase value: {1}", 
                        _backendParameters.MsiInstallLocation, installLocation));
                }
            }            
        }

        /// <summary>
        /// Verify that all prerequisites are installed
        /// </summary>
        private void CheckPrerequisites()
        {
            _logger.WriteLog(true, "CheckPrerequisites");

            _prereqChecker.VerifyIsFramework462Installed();
        }

        /// <summary>
        /// Verify that SQL server, login and password are correct,
        /// if typeOfActionWithDatabase is CreateNewDB - default database doesn't exist
        /// if typeOfActionWithDatabase is UseExistingDB - default database exists
        /// </summary>
        private void VerifyDatabaseSettings()
        {
            _logger.WriteLog(true, "VerifyDatabaseSettings");

            _confirmitCatiValidator.ValidateDatabaseSettings(_backendParameters.CatiSqlServerName, CatiSetupConstants.CatiDefaultDatabaseName, _backendParameters.CatiSqlAdminUserName, _backendParameters.CatiSqlAdminPassword, _backendParameters.TypeOfActionWithDatabase);
            _confirmitCatiValidator.ValidateHasSQLLoginAdministratorPermissions(_backendParameters.CatiSqlServerName, _backendParameters.CatiSqlAdminUserName, _backendParameters.CatiSqlAdminPassword);
            _confirmitCatiValidator.ValidateSqlServerConnection(_backendParameters.CatiSqlServerName, _backendParameters.CatiSqlUserName, _backendParameters.CatiSqlPassword);
        }

        /// <summary>
        /// Verify that SQL server, login and password for confirmlog database are correct
        /// </summary>
        private void VerifyConfirmAndConfirmlogSqlSettings()
        {
            _logger.WriteLog(true, "VerifyConfirmAndConfirmlogSqlSettings");

            var confirmConnectionStringBuilder = new SqlConnectionStringBuilder(_backendParameters.ConfirmConnectionString);

            _confirmitCatiValidator.ValidateSqlServerConnection(confirmConnectionStringBuilder.DataSource, confirmConnectionStringBuilder.UserID, confirmConnectionStringBuilder.Password);
            _confirmitCatiValidator.ValidateDatabaseConnection(confirmConnectionStringBuilder.DataSource, CatiSetupConstants.ConfirmDatabaseName, confirmConnectionStringBuilder.UserID, confirmConnectionStringBuilder.Password);
            _confirmitCatiValidator.ValidateDatabaseConnection(confirmConnectionStringBuilder.DataSource, CatiSetupConstants.ConfirmlogDatabaseName, confirmConnectionStringBuilder.UserID, confirmConnectionStringBuilder.Password);
        }

        /// <summary>
        /// Check that DTC is enabled and work
        /// </summary>
        private void VerifyDtc()
        {
            if (_backendParameters.CatiSqlServerName.ToLowerInvariant() != _backendParameters.ConfirmSqlServerName.ToLowerInvariant())
            {
                _confirmitCatiValidator.VerifyDtc(new SqlConnectionStringBuilder(_backendParameters.CatiConnectionString), _backendParameters.ConfirmitLinkedServerName, CatiSetupConstants.ConfirmlogDatabaseName);
            }
        }

        /// <summary>
        /// Method for determining is the user provided a valid email address
        /// We use regular expressions in this check, as it is a more thorough
        /// way of checking the address provided
        /// </summary>
        private void VerifyEmail()
        {
            _logger.WriteLog(true, "VerifyEmail");

            _confirmitCatiValidator.ValidateOneParameterFilling(_backendParameters.NotificationEmailBCC, Resources.NotificationEmailBCC);
            _confirmitCatiValidator.ValidateEmailAddresses(_backendParameters.NotificationEmailBCC);
        }


        /// <summary>
        /// If session state mode is SQLServer, then verify SQL server, 
        /// login and password for session state are correct
        /// </summary>
        private void VerifySQLSessionStateSettings()
        {
            var ssMode = _supervisorParameters.SessionStateMode.ToLower();
            if (ssMode != "inproc" && ssMode != "sqlserver" && ssMode != "redis")
            {
                throw new ValidateException(Resources.NotSupportedSessionStateMode);
            }

            if (ssMode == "sqlserver")
            {
                const string sessionStateDatabaseName = "ASPState";

                var connectionStringBuilder = new SqlConnectionStringBuilder(_supervisorParameters.SessionStateConnectionString);

                _confirmitCatiValidator.ValidateSqlServerConnection(connectionStringBuilder.DataSource, connectionStringBuilder.UserID, connectionStringBuilder.Password);
                _confirmitCatiValidator.ValidateDatabaseConnection(connectionStringBuilder.DataSource, sessionStateDatabaseName, connectionStringBuilder.UserID, connectionStringBuilder.Password);
            }
        }

        /// <summary>
        /// Check that declared site name exists on IIS server
        /// </summary>
        private void VerifySupervisorSiteName()
        {
            using (var sm = new ServerManager())
            {
                if (sm.Sites.Any(site => site.Name == _supervisorParameters.SupervisorSiteName))
                {
                    return;
                }
            }

            throw new MessageException(Resources.SupervisorSiteNameParameterIsWrong, TraceEventType.Warning);
        }


        /// <summary>
        /// Verify, that all mandatory parameters are defined
        /// </summary>
        private string VerifyBackendParametersAvailability()
        {
            _logger.WriteLog(true, "VerifyBackendParametersAvailability");
            var errInfo = new StringBuilder();

            if (_backendParameters.IsLoadBalancedEnvironment == "False")
            {
                if (_backendParameters.CertificateType == "Test")
                {
                    if (string.IsNullOrEmpty(_backendParameters.TestCertificateName))
                    {
                        errInfo.AppendLine(Resources.NotDefinedTestCertificateName);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_backendParameters.CertificatePath))
                    {
                        errInfo.AppendLine(Resources.NotDefinedCertificatePath);
                    }
                }
            }

            if (string.IsNullOrEmpty(_backendParameters.NotificationEmailBCC))
            {
                errInfo.AppendLine(Resources.NotDefinedNotificationEmailBCC);
            }

            return errInfo.ToString();
        }

        private string VerifySupervisorParametersAvailability()
        {
            var errInfo = new StringBuilder();

            if (string.IsNullOrEmpty(_supervisorParameters.SessionStateMode))
            {
                errInfo.AppendLine(Resources.NotDefinedSessionStateMode);
            }

            if (string.Equals(_supervisorParameters.SessionStateMode, "Redis", StringComparison.OrdinalIgnoreCase) && 
                string.IsNullOrEmpty(_supervisorParameters.RedisHostName))
            {
                errInfo.AppendLine(Resources.NotDefinedRedisHostName);
            }

            return errInfo.ToString();
        }
    }
}
