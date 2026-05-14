using BootstrapperLibrary;
using BootstrapperLibrary.Interfaces;
using CatiInstallation;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Setup.UnitTests.FakeClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Setup.UnitTests
{
    [TestClass]
    public class InstallationVerifierTests
    {
        private ILogger _logger;
        private IPrereqChecker _prereqChecker;
        private IConfirmitCATIValidator _confirmitCATIValidator;
        private ICertificateEngine _certificateEngine;
        private InstallationVerifier _installationVerifier;
        private BackendParameters _backendParameters;
        private IIsAliveHtmEngine _isAliveHtmEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new TraceLogger();
            _prereqChecker = new FakePrereqChecker();
            _confirmitCATIValidator = new FakeConfirmitCATIValidator();
            _certificateEngine = new FakeCertificateEngine();
            _isAliveHtmEngine = new FakeIsAliveHtmEngine();
            _installationVerifier = new InstallationVerifier(_logger, _prereqChecker, _confirmitCATIValidator, _certificateEngine, _isAliveHtmEngine);

            _backendParameters = new BackendParameters
            {
                CatiSqlServerName = "sqlserver",
                CatiSqlAdminUserName = "sa",
                CatiSqlAdminPassword = "firm",
                CatiSqlUserName = "ConfDep",
                CatiSqlPassword = "DepConf",
                CatiConnectionString = "Data Source=sqlserver;Initial Catalog=ConfirmitCATIV15;User ID=sa;Password=firm;Connect Timeout=120;Max Pool Size=4096",
                TypeOfActionWithDatabase = "UseExistingDB",
                CatiDefaultDbRecoveryModel = "full",
                ConfirmSqlServerName = "sqlserver",
                ConfirmUserName = "ConfDep",
                ConfirmPassword = "DepConf",
                ConfirmConnectionString = "Data Source=sqlserver;Initial Catalog=confirm;User ID=ConfDep;Password=DepConf;Connect Timeout=120",
                ConfirmlogConnectionString = "Data Source=sqlserver;Initial Catalog=confirmlog;User ID=ConfDep;Password=DepConf;Connect Timeout=120",
                IsLoadBalancedEnvironment = "True",
                LoadBalancerIsAlivePageUrl = "/IsAlive.htm",
                LoadBalancerIsAlivePageRenameTimeout = "180",
                CertificateType = "Test",
                CertificatePassword = "localhost",
                CertificatePath = "",
                CatiDatabasesDataFilePath = "",
                CatiDatabasesLogsFilePath = "",
                NotificationEmailBCC = "qwer@firmsw.no",
                ConfirmitLinkedServerName = ""
            };
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void VerifyBackendParameters_AllParametersAreFine_NoException()
        {
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        [ExpectedException(typeof(ValidateException))]
        public void VerifyBackendParameters_WrongSqlAdminUserName_ValidateExceptionOccured()
        {
            ((FakeConfirmitCATIValidator)_confirmitCATIValidator).IsDatabaseConnectionOk = false;
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        [ExpectedException(typeof(ValidateException))]
        public void VerifyBackendParameters_WrongEmailAddress_ValidateExceptionOccured()
        {
            ((FakeConfirmitCATIValidator)_confirmitCATIValidator).UseRealEmailAddressValidation = true;
            _backendParameters.NotificationEmailBCC = "wrong_email";
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]        
        public void VerifyBackendParameters_WrongConfirmitLinkedServerButSqlServersAreTheSame_NoException()
        {
            ((FakeConfirmitCATIValidator)_confirmitCATIValidator).IsConfirmitLinkedServerOk = false;
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        [ExpectedException(typeof(ValidateException))]
        public void VerifyBackendParameters_WrongConfirmitLinkedServerAndSqlServersAreDifferent_ValidateExceptionOccured()
        {
            ((FakeConfirmitCATIValidator)_confirmitCATIValidator).IsConfirmitLinkedServerOk = false;
            _backendParameters.CatiSqlServerName = "cati_sql_server";
            _backendParameters.ConfirmitLinkedServerName = "linked_server";
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        [ExpectedException(typeof(ValidateException))]
        public void VerifyBackendParameters_EmptyConfirmitLinkedServerAndSqlServersAreDifferent_ValidateExceptionOccured()
        {
            ((FakeConfirmitCATIValidator)_confirmitCATIValidator).IsConfirmitLinkedServerOk = false;
            _backendParameters.CatiSqlServerName = "cati_sql_server";
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        [ExpectedException(typeof(MessageException))]
        public void VerifyBackendParameters_NoLoadBalancerButNonDistuptiveModeSelected_ValidateExceptionOccured()
        {
            _backendParameters.IsLoadBalancedEnvironment = "False";
            _installationVerifier.VerifyBackendParameters(_backendParameters);
        }
    }
}