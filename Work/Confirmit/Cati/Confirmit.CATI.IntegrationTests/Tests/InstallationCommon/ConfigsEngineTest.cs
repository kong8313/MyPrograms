using System.IO;
using System.Reflection;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Tests.InstallationCommon.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationCommon
{
    [TestClass]
    public class ConfigsEngineTest
    {
        private string _configLocation;

        private ILogger _logger;
        private ConfigsEngine _configsEngine;
        private ConfigVerifier _configVerifier;

        [TestInitialize]
        public void TestInitialize()
        {
            _configLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _logger = new TraceLogger();
            _configsEngine = new ConfigsEngine(_logger);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _configVerifier.RestoreConfig();
        }

        private void DefineSupervisorConfigVerifier()
        {
            File.Copy(Path.Combine(_configLocation, "..\\SupervisorNew\\Confirmit.CATI.Supervisor\\web.config"), Path.Combine(_configLocation, "web.config"), true);
            _configVerifier = new ConfigVerifier(Path.Combine(_configLocation, "web.config"));
        }

        private void DefineBackendConfigVerifier()
        {
            _configVerifier = new ConfigVerifier(Path.Combine(_configLocation, "Confirmit.CATI.Backend.exe.config"));
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureSupervisorConfig_AllSettingsAreChanged_ConfigIsOk()
        {
            DefineSupervisorConfigVerifier();
            string defaultRedisHostName = _configVerifier.GetRedisConnectionString();
            const string catiConnectionString = @"Data Source=localhost;Initial Catalog=ConfirmitCATIV15;User ID=sa;Password=firm;Connect Timeout=120;Max Pool Size=4096";
            const string sessionStateMode = "InProc";
            const string redisHostName = "redisHostName";
            const string sessionStateConnectionString = @"Data Source=localhost;User ID=sa;Password=firm;Connect Timeout=120";
            const string sessionStateCookieName = "TestSessionStateCookieName";
            const string confirmitKeepSessionAspxUrl = "http://ConfirmitAuthoringServer/confirm/authoring/KeepSession.aspx";
            const string igResFolderName = "ig_res_18.5.123.4567";
            const string confirmitLogPath = "c:\\test_path";

            _configsEngine.ConfigureSupervisorConfig(_configLocation, catiConnectionString, sessionStateMode,
                redisHostName, "", sessionStateConnectionString, sessionStateCookieName, confirmitKeepSessionAspxUrl, igResFolderName, confirmitLogPath);
            
            Assert.AreEqual(catiConnectionString, _configVerifier.GetCatiConnectionString());
            Assert.AreEqual(sessionStateMode, _configVerifier.GetSessionStateMode());
            Assert.AreEqual(defaultRedisHostName, _configVerifier.GetRedisConnectionString());
            Assert.AreEqual(string.Empty, _configVerifier.GetSessionStateConnectionString());
            Assert.AreEqual(string.Empty, _configVerifier.GetSessionStateCookieName());
            Assert.AreEqual(confirmitKeepSessionAspxUrl, _configVerifier.GetConfirmitKeepSessionAspxUrl());
            Assert.AreEqual("~/" + igResFolderName, _configVerifier.GetIgResFolderName());
            Assert.AreEqual(confirmitLogPath, _configVerifier.GetConfirmitLogPath());
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureSupervisorConfig_SQLServerMode_AllParametersForSQLServerModeAreCorrect()
        {
            DefineSupervisorConfigVerifier();
            string defaultRedisHostName = _configVerifier.GetRedisConnectionString();
            const string sessionStateMode = "SQLServer";
            const string sessionStateConnectionString = @"Data Source=localhost;User ID=sa;Password=firm;Connect Timeout=120";
            const string sessionStateCookieName = "TestSessionStateCookieName";
            const string empty = "";

            _configsEngine.ConfigureSupervisorConfig(_configLocation, empty, sessionStateMode,
                empty, empty, sessionStateConnectionString, sessionStateCookieName, empty, empty, empty);

            Assert.AreEqual(sessionStateMode, _configVerifier.GetSessionStateMode());
            Assert.AreEqual(sessionStateConnectionString, _configVerifier.GetSessionStateConnectionString());
            Assert.AreEqual(sessionStateCookieName, _configVerifier.GetSessionStateCookieName());
            Assert.AreEqual(defaultRedisHostName, _configVerifier.GetRedisConnectionString());
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureSupervisorConfig_RedisMode_AllParametersForRedisModeAreCorrect()
        {
            DefineSupervisorConfigVerifier();
            const string sessionStateMode = "Redis";
            const string redisHostName = "redisHostName1,redisHostName2";
            const string redisPassword = "redisPass";
            const string sessionStateCookieName = "TestSessionStateCookieName";
            const string empty = "";
            string redisConnectionString = $"{redisHostName},password={redisPassword},ssl=False,abortConnect=False";

            _configsEngine.ConfigureSupervisorConfig(_configLocation, empty, sessionStateMode,
                redisHostName, redisPassword, empty, sessionStateCookieName, empty, empty, empty);

            Assert.AreEqual("Custom", _configVerifier.GetSessionStateMode());
            Assert.AreEqual(redisConnectionString, _configVerifier.GetRedisConnectionString());
            Assert.AreEqual(sessionStateCookieName, _configVerifier.GetSessionStateCookieName());
            Assert.AreEqual(empty, _configVerifier.GetSessionStateConnectionString());
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureBackendConfig_WriteLoggingPath_LoggingPathIsCorrect()
        {
            DefineBackendConfigVerifier();

            const string confirmitLogPath = "c:\\test_path";

            _configsEngine.ConfigureBackendConfig(_configLocation, true, confirmitLogPath);

            Assert.AreEqual(confirmitLogPath, _configVerifier.GetConfirmitLogPath());
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureBackendConfig_SslAcceleratorIsUsed_ConfigHasCorrectSSLSettings()
        {
            DefineBackendConfigVerifier();

            const bool isLoadBalancedEnvironment = true;

            _configsEngine.ConfigureBackendConfig(_configLocation, isLoadBalancedEnvironment, "");

            Assert.AreEqual(isLoadBalancedEnvironment, _configVerifier.IsSSLEnabled());
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureBackendConfig_SslAcceleratorIsNotUsed_ConfigHasCorrectSSLSettings()
        {
            DefineBackendConfigVerifier();

            const bool isLoadBalancedEnvironment = false;

            _configsEngine.ConfigureBackendConfig(_configLocation, isLoadBalancedEnvironment, "");

            Assert.AreEqual(isLoadBalancedEnvironment, _configVerifier.IsSSLEnabled());
        }
    }
}

