using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.IntegrationTests.Tests.CATIConsoleService
{
    [TestClass]
    public class StationIdTests : BaseMockedIntegrationTest
    {
        private CatiWsHelper _serviceHelper;

        private const string UserName = "APerson";
        private const string UserPassword = "password";

        private PersonInfo _personInfo;
        private DiallerInfo _diallerInfo;

        private CatiConsolePropertiesContainer _outCatiConsoleProperties;

        /// <summary>
        /// Prepare data for test
        /// 1. Add survey, launch 'all hours' script, open survey
        /// 2. Create interview with required parameters
        /// </summary>
        private void PrepareDataForTest()
        {
            PersonTools.CreatePerson(UserName, UserPassword, AgentTaskChoiceMode.Manual);
            _serviceHelper = new CatiWsHelper(UserName, UserPassword);
        }

        /// <summary>        
        /// 1. Create interviewer        
        /// 2. Login interviewer into Console with StationId
        /// 3. Check StationId field in the bvTask table
        /// 4. Check StationExtensionNumber field in the bvPerson table
        /// 5. Logout person
        /// 5. Check StationExtensionNumber field in the bvPerson table has to be empty     
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void PersonLoginToConsole_ValidStationId_ValidDataInBvTaskAndInBvPerson_Success()
        {
            PrepareDataForTest();

            const string stationId = "Name12345L";

            var consoleDescriptor = new ConsoleDescription(); 
            
            _serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,                
                out _outCatiConsoleProperties);

            BvTasksEntity task = TaskRepository.GetByPerson(_personInfo.PersonId);

            Assert.AreEqual("Name12345L", task.StationId);
            Assert.AreEqual("12345", task.StationExtensionNumber);
            Assert.AreEqual(true, task.IsDialerAgentLocal);

            TaskService.RemoveTaskAndLogoutPerson(_personInfo.PersonId);

            task = TaskRepository.GetByPerson(_personInfo.PersonId);
            Assert.IsNull(task);
        }

        /// <summary>
        /// 1. Log in person with first station id.
        /// 2. Log in the same person with different station id.
        /// 3. Exception should be thrown.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Cr(43795)]
        public void PersonLoginToConsole_TwoPersonsAreLoggingWithSameInterviewerFromDifferentStations_ExceptionIsThrown()
        {
            PrepareDataForTest();
            const string stationId1 = "Name123";
            const string stationId2 = "Noname321";

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                stationId1,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _outCatiConsoleProperties);

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserAlreadyLoggedInException>(
                () => _serviceHelper.ConsoleService.Login(
                    stationId2,
                    consoleDescriptor,
                    out _personInfo,
                    out _diallerInfo,
                    out _outCatiConsoleProperties),
                exception => 
                {
                    Assert.AreEqual("User is already logged in from another station.", exception.Message);
                    Assert.AreEqual(stationId1, exception.FirstStationId);
                    Assert.AreEqual(stationId2, exception.SecondStationId);
                }
            );
        }

        /// <summary>
        /// 1. Log in person with first station id.
        /// 2. Log in the same person with the same station id.
        /// 3. Success.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Cr(43795)]
        public void PersonLoginToConsole_TwoPersonsAreLoggingWithSameInterviewerFromTheSameStations_Success()
        {
            PrepareDataForTest();
            const string stationId = "Name123";

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _outCatiConsoleProperties);

            _serviceHelper.ConsoleService.Login(
                    stationId,
                    consoleDescriptor, 
                    out _personInfo,
                    out _diallerInfo,
                    out _outCatiConsoleProperties);
        }

        /// <summary>
        /// 1. Log in person with first station id.
        /// 2. Log in the same person with the same station id.
        /// 3. Station ids are compared case-insensitive.
        /// 4. Success.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Cr(43795)]
        public void PersonLoginToConsole_TwoPersonsAreLoggingWithSameInterviewerFromTheSameStationsCaseInsensitive_Success()
        {
            PrepareDataForTest();
            const string stationId1 = "Name123";
            const string stationId2 = "nAMe123";

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                stationId1,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _outCatiConsoleProperties);

            _serviceHelper.ConsoleService.Login(
                    stationId2,
                    consoleDescriptor,
                    out _personInfo,
                    out _diallerInfo,
                    out _outCatiConsoleProperties);
        }
    }
}
