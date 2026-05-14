using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.IntegrationTests.Tests.CATIConsoleService
{
    [TestClass]
    public class AuthenticationKeyTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private string _stationId = string.Empty;
        private readonly string _personName = "user";
        private readonly string _password = "password";
        private int _personSid;
        private CatiWsHelper _serviceHelper;
        private PersonInfo _personInfo;
        private DiallerInfo _diallerInfo;
        private CatiConsolePropertiesContainer _catiConsoleProperties;

        public TestContext TestContext
        {
            get; set;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();

            _personSid = PersonTools.CreatePerson(_personName, _password, AgentTaskChoiceMode.Manual);
            _serviceHelper = new CatiWsHelper(_personName, _password);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GenerateAuthenticationKey_GenerateNewKeyForLoggedInUser_NewKeyGenerated()
        {
            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            Guid authenticationKey = _serviceHelper.ConsoleService.GenerateAuthenticationKey();

            Assert.AreNotEqual(_personInfo.AuthenticationKey, authenticationKey);
        }
        
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GenerateAuthenticationKey_GenerateNewKeyForLoggedInUser_NewKeyIsSavedInDatabase()
        {
            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            Guid authenticationKey = _serviceHelper.ConsoleService.GenerateAuthenticationKey();

            var task = TaskRepository.GetByPerson(_personSid);
            Assert.AreEqual(authenticationKey, task.AuthenticationKey);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GenerateAuthenticationKey_GenerateNewKeyForLoggedInUser_SessionStartTimeIsUpdated()
        {
            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            var task = TaskRepository.GetByPerson(_personSid);
            DateTime sessionTime = task.StartSessionTime;

            _serviceHelper.ConsoleService.GenerateAuthenticationKey();

            task = TaskRepository.GetByPerson(_personSid);

            Assert.IsTrue(task.StartSessionTime > sessionTime, "Updated session time should be greater than previous session time");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Login_RepeatedLoginWithoutLogout_AuthenticationKeyIsRenewed()
        {
            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            var oldAuthenticationKey = _personInfo.AuthenticationKey;

            EmulateStateServiceSessionExpiration();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            Assert.AreNotEqual(oldAuthenticationKey, _personInfo.AuthenticationKey,
                string.Format("Authentication key {0} wasn't renewed", oldAuthenticationKey));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Login_RepeatedLoginWithoutLogout_SessionStartTimeIsUpdated()
        {
            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            var task = TaskRepository.GetByPerson(_personSid);
            var oldSessionStart = task.StartSessionTime;

            EmulateStateServiceSessionExpiration();

            _serviceHelper.ConsoleService.Login(
                _stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _catiConsoleProperties);

            task = TaskRepository.GetByPerson(_personSid);
            Assert.IsTrue(oldSessionStart < task.StartSessionTime, 
                string.Format(
                    "State service session wasn't renewed. Old session start time: {0:HH:mm:ss.ffff}, new {1:HH:mm:ss.ffff}",
                    oldSessionStart, task.StartSessionTime));
        }

        private void EmulateStateServiceSessionExpiration()
        {
            var task = TaskRepository.GetByPerson(_personSid);
            task.StartSessionTime = DateTime.UtcNow.AddMinutes(-30);
            TaskRepository.Update(task);
        }
    }
}
