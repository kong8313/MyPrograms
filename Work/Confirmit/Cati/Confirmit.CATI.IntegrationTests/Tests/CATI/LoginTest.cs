using System;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class LoginTest
    {
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        


        [TestMethod, Owner(@"FIRM\EgorS")]
        [ExpectedException(typeof(UserMessageException))]
        public void LoginWithOldConsole_ExceptionThrows()
        {
            new TestCati2(false, false, _backendTools);

            var id = PersonTools.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            string stationId = string.Empty;

            var consoleDescription = new ConsoleDescription { ConsoleVersion = "18.5.0.0", OperatingSystemVersion = "6.2.0" };
            ServiceLocator.Resolve<ISetupSettings>().InterviewerConsoleVersion = "18.5.0.1";

            var authorizerStub = new StubIConsoleWsRequestsAuthoriser
            {
                AuthoriseRequestBvPersonEntityOut =
                    (out BvPersonEntity interviewer) => interviewer = new BvPersonEntity() { SID = id, CallCenterID = 1 }
            };
            ServiceLocator.RegisterInstance<IConsoleWsRequestsAuthoriser>(authorizerStub);

            // Register original version validator so it will perform validation while we call Login
            ServiceLocator.Register<IConsoleVersionValidator, ConsoleVersionValidator>();

            var consoleService = new ConsoleService();
            consoleService.Login(
                stationId,
                consoleDescription,
                out personInfo,
                out diallerInfo,
                out outProperties);

        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void LoginWithUpToDateConsole_LoginPass()
        {
            new TestCati2(false, false, _backendTools);

            var id = PersonTools.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            string stationId = string.Empty;

            var consoleDescription = new ConsoleDescription { ConsoleVersion = "18.5.0.0", OperatingSystemVersion = "6.2.0" };

            ServiceLocator.Resolve<ISetupSettings>().InterviewerConsoleVersion = "18.5.0.0";

            var authorizerStub = new StubIConsoleWsRequestsAuthoriser
            {
                AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBoolean =
                    (out BvPersonEntity interviewer, out BvTasksEntity task, bool exist) =>
                    {
                        task = null;
                        interviewer = new BvPersonEntity {SID = id, CallCenterID = 1};
                    }
            };
            ServiceLocator.RegisterInstance<IConsoleWsRequestsAuthoriser>(authorizerStub);

            // Register original version validator so it will perform validation while we call Login
            ServiceLocator.Register<IConsoleVersionValidator, ConsoleVersionValidator>();

            var consoleService = new ConsoleService();
            consoleService.Login(
                stationId,
                consoleDescription,
                out personInfo,
                out diallerInfo,
                out outProperties);

        }

        // *** Провека login-а из CATIConsole для существующей персоны и неправильным паролем
        // Подготовка:
        // Создает новый инстанс, создается персона с CATI-ролью, паролем. 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS  с правильным логином и неправильным паролем
        // Проверяем что Login кинул FaultException
        [TestMethod, Owner(@"FIRM\MaximL")]
        [ExpectedException(typeof(InvalidInterviewerCredentialsException))]
        public void PersonExistsWithPassword_LoginWithInvalidPassword_ThrowException()
        {
            new TestCati2(false, false, _backendTools);

            PersonTools.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            var stationId = string.Empty;

            var consoleDescriptor = new ConsoleDescription();

            var consoleService = new ConsoleService();
            consoleService.Login(
                stationId,
                consoleDescriptor,
                out personInfo,
                out diallerInfo,
                out outProperties);

        }

        // *** Провека login-а из CATIConsole для не существующей персоны
        // Подготовка:
        // Создает новый инстанс, 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS с несушествующим логином(именем) персоны
        // Проверяем что Login кинул SoapException
        [TestMethod, Owner(@"FIRM\MaximL")]
        [ExpectedException(typeof(InvalidInterviewerCredentialsException))]
        public void PersonDoesNotExists_Login_ThrowException()
        {
            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            string stationId = string.Empty;

            var consoleService = new ConsoleService();
            var consoleDescriptor = new ConsoleDescription();

            consoleService.Login(stationId, consoleDescriptor, out personInfo, out diallerInfo, out outProperties);
        }

        // *** Провека login-а из CATIConsole для существующей персоны и правильным паролем, когда дайлер не доступен
        // Подготовка:
        // Создает новый инстанс, создается персона с CATI-ролью, паролем и режимом X(survey assignment/auto/manual. 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS с правильным логином и паролем, проверяет возращаемые данные( PersonMode, connectedToDialer, catiConsoleProperties )
        // Проверяем что Login вернул sessionID
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonExistsWithPasswordAndMnDialerDoesNotAvailable_LoginWithValidPassword_LoginSuccessConnectedToDialerIsFalse()
        {
            var test = new TestCati2(false, false, _backendTools);

            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            test.CheckState(test.GetExpectedStateLoginToDialer(LoginState.NOT_LOGGED_IN));
        }

        // *** Провека login-а из CATIConsole для существующей персоны и правильным паролем, когда дайлер доступен
        // Подготовка:
        // Создает новый инстанс, создается персона с CATI-ролью, паролем и режимом X(survey assignment/auto/manual. 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS с правильным логином и паролем, проверяет возращаемые данные( PersonMode, connectedToDialer, catiConsoleProperties )
        // Проверяем что Login вернул sessionID
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonExistsWithPasswordAndMnDialerAvailable_LoginWithValidPassword_LoginSuccessConnectedToDialerIsTrue()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);

            test.CheckState(test.GetExpectedStateLoginToDialer(LoginState.NOT_LOGGED_IN));
        }

        // Слудующте тесты определяются только различными режимами персоны( manual, auto, survey assignment ) и 
        // параметрами в вызове LoginToDialer

        /// <summary>
        /// Initializes login process. Method creates user with specified mode
        /// and logs in CATI Console WS and Dialer.
        /// </summary>
        /// <param name="surveyMode">Survey mode.</param>
        /// <param name="personMode">Person assignment mode.</param>
        private void LoginToDialer_Base(DialingMode surveyMode, AgentTaskChoiceMode personMode)
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(surveyMode, UserName, Password, personMode);

            test.Login(UserName, Password, personMode, true);

            test.LoginToDialer(ExtensionNumber);
        }

        // *** Проверка LoginToDialer
        // Подготовка:
        // Создает новый инстанс, создается персона с CATI-ролью, паролем и режимом auto, создается сарвей( dialing режимом preview(возможно этот тест требует пробублировать для всех режимов)? )
        // Открывает сарвей, персона назначается на сарвей 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS с правильным логином и паролем, проверяет возращаемые данные( PersonMode, connectedToDialer, catiConsoleProperties )
        // Проверяем что Login вернул sessionID и GetState
        // Вызывает LoginToDialer c соответсвующим номером
        // Проверяем что Login вернул sessionID
        // Вызывается GetState, проверяет правильность результата
        // Если LoginToDialer приводит к асинхронному обращению в дайлуру, то можно проверить промежуточное состаяние( interviewerLoginToDialerState == LOGGING_IN )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyInPreviewModeAndPersonModeIsAutoMnDialerAvailable_LoginToDialer_LoginSuccess()
        {
            LoginToDialer_Base(DialingMode.Preview, AgentTaskChoiceMode.Automatic);
        }

        // *** Проверка LoginToDialer
        // Подготовка:
        // Создает новый инстанс, создается персона с CATI-ролью, паролем и режимом SurveyAssignment, создается сарвей( dialing режимом preview(возможно этот тест требует пробублировать для всех режимов)? )
        // Открывает сарвей, персона назначается на сарвей 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS с правильным логином и паролем, проверяет возращаемые данные( PersonMode, connectedToDialer, catiConsoleProperties )
        // проверяет, что в BvTasks была создана соответсвующая запись
        // вызывается метод GetOpenedSurveys, проверят правильность результат
        // Вызывает LoginToDialer c соответсвующим номером сарвея
        // проверяет, что в BvTasks изменилась запись
        // Вызывается GetState, проверяет правильность результата
        // Если LoginToDialer приводит к асинхронному обращению в дайлуру, то можно проверить промежуточное состаяние, временно заблокировав обработку запроса дайлером( interviewerLoginToDialerState == LOGGING_IN )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyInPreviewAndPersonModeIsSurveyAssignmentMnDialerAvailable_LoginToDialer_LoginSuccess()
        {
            LoginToDialer_Base(DialingMode.Preview, AgentTaskChoiceMode.CampaignAssignment);
        }

        // *** Проверка LoginToDialer
        // Подготовка:
        // Создает новый инстанс, создается персона с CATI-ролью, паролем и режимом Manual, создается сарвей( dialing режимом preview(возможно этот тест требуется продублировать для всех режимов)? )
        // Открывает сарвей, персона назначается на сарвей 
        // Тест:
        // Вызывает метод Login через CATIConsoleWS с правильным логином и паролем, проверяет возвращаемые данные( PersonMode, connectedToDialer, catiConsoleProperties )
        // проверяет, что в BvTasks была создана соответствующая запись
        // вызывается метод GetOpenedSurveys, проверяет правильность результата
        // Вызывает LoginToDialer c соответсвующим номером сарвея
        // проверяет, что в BvTasks изменилась запись
        // Вызывается GetState, проверяет правильность результата
        // Если LoginToDialer приводит к асинхронному обращению к дайлеру, то можно проверить промежуточное состояние, временно заблокировав обработку запроса дайлером( interviewerLoginToDialerState == LOGGING_IN )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyIsPreviewPersonModeIsManualMnDialerAvailable_LoginToDialer_LoginSuccess()
        {
            LoginToDialer_Base(DialingMode.Preview, AgentTaskChoiceMode.Manual);
        }

        //The test checks that BvTasks contains null value of TimeCallDelivered
        //after interviewer login and after interviewer login to dialer.
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(38867)]
        public void PersonLoggedIn_PersonLoggedInToDialer_TimeCallDeliveredIsNull()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            var entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered = entity.TimeCallDelivered;
            Assert.IsNull(
                timeCallDelivered,
                "Interviewer has just logged in and no interview is started yet but timeCallDelivered is not null.");

            test.LoginToDialer(ExtensionNumber);
            entity = test.GetBvTasksEntityForThePerson();
            timeCallDelivered = entity.TimeCallDelivered;
            Assert.IsNull(
                timeCallDelivered,
                "Interviewer has just logged in to dialler and no interview is started yet but timeCallDelivered is not null.");
        }

        //The test checks that BvTasks value of TimeCallDelivered is not null and does not change 
        //after interviewer relogins CATI console while interviewing.
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(38867)]
        public void PersonProcessesAnInterview_PersonReloggedInDuringInterviewing_TimeCallDeliveredNotNullAndIsNotChanged()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            test.ReplyOnInterview_Progressive(interview);
            var entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered1 = entity.TimeCallDelivered;
            Assert.IsNotNull(timeCallDelivered1, "An interview is started but timeCallDelivered is null.");

            State state = test.StateWS.GetState();

            //Emulate CATI console reopen
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true, state);
            entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered2 = entity.TimeCallDelivered;

            Assert.IsNotNull(timeCallDelivered2, "Interview is in progress, but timeCallDelivered became null after CATI console restart.");
            test.CheckValueInBvTask("TimeCallDelivered", timeCallDelivered1);
        }

        /// <summary>
        /// The test checks that if an user tries to login to a dialer with user mode not supported by current dialer type then:
        /// 1. Login fails
        /// 2. There is no record for the failed person in BvTasks table.
        /// 
        /// Ex. Automatic user can not login to PRO-T-S.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(41313)]
        public void DialerTypeDoesNotSupportAutomaticUsers_AutomaticUserTriesToLogin_LoginFailsAndThereIsNoRecordInBvTasksForTheUser()
        {
            var test = new TestCati2(true, false, true, _backendTools);

            var telephonyStub = new StubITelephony
            {
                IsPersonModeSupportedAgentTaskChoiceModeNullableOfInt32 = (mode, id) => false
            };
            ServiceLocator.RegisterInstance<ITelephony>(telephonyStub);

            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            try
            {
                test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            }
            catch (UserMessageException ex)
            {
                Assert.AreSame(
                    ex.MessageKey,
                    "Error_PersonModeIsNotSupportedByDialer",
                    "Some incorrect exception was thrown while trying to login a person. 'Error_PersonModeIsNotSupportedByDialer' exception must be thrown.");
            }

            Assert.IsFalse(test.IsThereARecordInBvTasksForThePerson(), "BvTasks contains a record for the person which failed to login.");
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void LoginInMultiDialerConfiguration_OneConnectedAndOneActiveDialers_ErrorConnectingToInactiveDialer()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1"
                }},

                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },

                Dialers = new[]
                {
                    new DialerData { Id = 1, Tag = "D1", IsActive = true, IsConnected = true},
                    new DialerData { Id = 2, Tag = "D2", IsActive = false, IsConnected = true},
                }
            }.Create();

            TestAssert.InvokeMethodAndVerifyExceptionThrown<LoginToInactiveDialerException>(
                () => context.GetPerson("P1").Console.SetStationId("g100000").Login("S1").LoginToDialer(),
                exception =>
                {
                    Assert.AreEqual(
                        "ConsoleLoginToDialerProcessor: Person tries to login to dialer but the dialer is not activated. /// personId=35, dialerId=2",
                        exception.Message, $"Exception was thrown when person login and connect to dialer: {exception.Message}");
                }
            );
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void LoginInMultiDialerConfiguration_OneConnectedAndOneActiveDialers_SuccessConnectingToActiveDialer()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1"
                }},

                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },

                Dialers = new[]
                {
                    new DialerData { Id = 1, Tag = "D1", IsActive = true, IsConnected = true},
                    new DialerData { Id = 2, Tag = "D2", IsActive = false, IsConnected = true},
                }
            }.Create();

            context.GetPerson("P1").Console.SetStationId("g000000").Login("S1").LoginToDialer();
        }
    }
}