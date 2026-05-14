using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class LoginLogoutEventsTests : BaseMockedIntegrationTest
    {

        const string UserName = "testUser";
        const string Password = "password";
        const string ExtensionNumber = "101010";

        [TestMethod]
        public void Login_PersonManualMode_LoginEventAreWrited()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                        },
                        Assigns = new[] {"P1"}
                    }
                },
                Persons = new[] {new PersonData() {Tag = "P1", TaskChoice = TaskChoiceMode.Manual}}
            }.Create();

            var person = context.GetPerson("P1");
            var console = new ManualModeConsoleController(context, person);

            var callCenterId = 0;
            var personId = 0;
            var sessionId = 123456789;

            var stub =
                TestingFramework.RegistryStub<IPersonSessionHistoryRepository, StubIPersonSessionHistoryRepository>();

            stub.InsertStartSessionEventIConnectionProviderInt32Int32 = (cp, ccId, pId) =>
            {
                callCenterId = ccId;
                personId = pId;
                return sessionId;
            };

            console.Login();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual(1, callCenterId, "IPersonSessionHistoryRepository.InsertStartSessionEvent method was called with wrong callcelnterId");
            Assert.AreEqual(person.Id, personId, "IPersonSessionHistoryRepository.InsertStartSessionEvent method was called with wrong personId");
            Assert.AreEqual(sessionId, task.SessionId, "Task contains wrong session Id");
        }

        [TestMethod]
        public void Logout_PersonManualMode_LoginEventAreWrited()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                        },
                        Assigns = new[] {"P1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } }
            }.Create();

            var person = context.GetPerson("P1");
            var console = new ManualModeConsoleController(context, person);

            var expectedSessionId = 123456789;
            int actualSessionId = 0;

            var stub =
                TestingFramework.RegistryStub<IPersonSessionHistoryRepository, StubIPersonSessionHistoryRepository>();

            stub.InsertStartSessionEventIConnectionProviderInt32Int32 = (cp, ccId, pId) => expectedSessionId;
            stub.InsertStopSessionEventIConnectionProviderInt32 = (cp, sId) => { actualSessionId = sId; };

            console.Login();
            console.Logout();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual(expectedSessionId, actualSessionId, "IPersonSessionHistoryRepository.InsertStopSessionEvent method was called with wrong personId");
        }

        [TestMethod]
        public void PersonSessionHistoryRepository_InsertStartSessionEvent_Success()
        {
            var engine = PrepareTest();

            var callcenterId = 10;
            var personId = 12;

            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
            var sessionId = ServiceLocator.Resolve<IPersonSessionHistoryRepository>().InsertStartSessionEvent(cp, callcenterId, personId);
            using (var reader = engine.ExecuteReaderInNewConnection("SELECT * FROM CatiInterviewerSessionHistory", CommandType.Text))
            {
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(sessionId, (int)reader["SessionId"]);
                Assert.AreEqual(BackendInstance.Current.CompanyId, (int) reader["CompanyId"]);
                Assert.AreEqual(callcenterId, (int)reader["CallCenterId"]);
                Assert.AreEqual(personId, (int)reader["InterviewerId"]);
                Assert.AreNotEqual(DBNull.Value, reader["LoginTime"]);
                Assert.AreEqual(DBNull.Value, reader["LogoutTime"]);
                Assert.IsFalse(reader.Read());
            }
        }

        [TestMethod]
        public void PersonSessionHistoryRepository_InsertStopSessionEvent_Success()
        {
            var engine = PrepareTest();

            var callcenterId = 10;
            var personId = 12;

            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
            var sessionId = rep.InsertStartSessionEvent(cp, callcenterId, personId);
            rep.InsertStopSessionEvent(cp, sessionId);

            using (var reader = engine.ExecuteReaderInNewConnection("SELECT * FROM CatiInterviewerSessionHistory", CommandType.Text))
            {
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(sessionId, (int)reader["SessionId"]);
                Assert.AreEqual(BackendInstance.Current.CompanyId, (int)reader["CompanyId"]);
                Assert.AreEqual(callcenterId, (int)reader["CallCenterId"]);
                Assert.AreEqual(personId, (int)reader["InterviewerId"]);
                Assert.AreNotEqual(DBNull.Value, reader["LoginTime"]);
                Assert.AreNotEqual(DBNull.Value, reader["LogoutTime"]);
                Assert.IsFalse(reader.Read());
            }
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void PersonSessionHistoryRepository_SelectEvents_Success()
        {
            PrepareTest();

            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.Manual, Name = "Test" } }
            }.Create();

            var callCenter = CallCenterTools.Create();

            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var companyId = BackendInstance.Current.CompanyId;
            var person = context.GetPerson("P1");
            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
            var sessionId = rep.InsertStartSessionEvent(cp, callCenter.ID, person.Id);

            var evts = rep.GetSessionEvents(callCenter.ID, companyId, null, null).ToList();
            Assert.IsTrue(evts.Any());

            var personSessionHistoryEntity = evts.First();
            Assert.AreEqual(person.Id, personSessionHistoryEntity.InterviewerId);
            Assert.AreEqual(callCenter.ID, personSessionHistoryEntity.CallCenterId);
            Assert.AreEqual(companyId, personSessionHistoryEntity.CompanyId);
            Assert.IsNotNull(personSessionHistoryEntity.LoginTime);
            Assert.IsNull(personSessionHistoryEntity.LogoutTime);

            rep.InsertStopSessionEvent(cp, sessionId);
            evts = rep.GetSessionEvents(callCenter.ID, companyId, null, null).ToList();
            personSessionHistoryEntity = evts.First();
            Assert.IsNotNull(personSessionHistoryEntity.LogoutTime);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void PersonSessionHistoryRepository_SelectEventsWithStartEndCondition_Success()
        {
            var engine = PrepareTest();

            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.Manual, Name = "Test" } }
            }.Create();
            var callCenter = CallCenterTools.Create();
            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var companyId = BackendInstance.Current.CompanyId;
            var person = context.GetPerson("P1");
            var startTime = DateTime.UtcNow;

            InsertSessionEvent(engine, companyId, callCenter.ID, person.Id, startTime, startTime.AddSeconds(5));

            var endTime = startTime.AddSeconds(5);            

            var evts = rep.GetSessionEvents(callCenter.ID, companyId, startTime.AddHours(-2), startTime.AddHours(-1)).ToList();
            Assert.IsFalse(evts.Any());

            evts = rep.GetSessionEvents(callCenter.ID, companyId, endTime.AddHours(1), endTime.AddHours(2)).ToList();
            Assert.IsFalse(evts.Any());

            evts = rep.GetSessionEvents(callCenter.ID, companyId, startTime, endTime).ToList();
            Assert.AreEqual(1, evts.Count);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void PersonSessionHistoryRepository_SelectEventsWithConditionOnCallCenter_ReturnsEventsForCorrectCenters()
        {
            PrepareTest();

            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.Manual, Name = "Test" } }
            }.Create(); 

            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var person = context.GetPerson("P1");
            var callCenter1 = CallCenterTools.Create();
            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();

            var sessionId1 = rep.InsertStartSessionEvent(cp, callCenter1.ID, person.Id);
            rep.InsertStopSessionEvent(cp, sessionId1);

            var callCenter2 = CallCenterTools.Create();
            var sessionId2 = rep.InsertStartSessionEvent(cp, callCenter2.ID, person.Id);
            rep.InsertStopSessionEvent(cp, sessionId2);

            var companyId = BackendInstance.Current.CompanyId;

            var evts = rep.GetSessionEvents(callCenter1.ID, companyId, null, null).ToList();
            Assert.AreEqual(1, evts.Count);
            Assert.AreEqual(callCenter1.ID, evts.First().CallCenterId);

            evts = rep.GetSessionEvents(callCenter2.ID, companyId, null, null).ToList();
            Assert.AreEqual(1, evts.Count);
            Assert.AreEqual(callCenter2.ID, evts.First().CallCenterId);

            evts = rep.GetSessionEvents(null, companyId, null, null).ToList();
            Assert.AreEqual(2, evts.Count);
            Assert.AreEqual(1, evts.Count(x => x.CallCenterId == callCenter1.ID));
            Assert.AreEqual(1, evts.Count(x => x.CallCenterId == callCenter2.ID));
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void UnexpectedLogoutNotificationFromDialer_NotificationIgnored()
        {
            string tenantId = String.Empty;

            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, null);
            test.StartInterview_Predictive(1);

            test.DialerHelper.SendEventNotifyAgentState(0, test.PersonSID, "4");

            Assert.AreEqual((byte)LoginState.LOGGED_IN, TaskRepository.GetByPerson(test.PersonSID).StatusLogout);
        }


        private void InsertSessionEvent(DatabaseEngine engine, int companyId, int callCenterId, int interviewerId, DateTime startTime, DateTime endTime)
        {
            engine.ExecuteNonQuery(
                @"INSERT INTO CatiInterviewerSessionHistory(CompanyId, CallCenterId, InterviewerId, LoginTime, LogoutTime) 
                    VALUES( @CompanyId, @CallCenterId, @InterviewerId, @StartTime, @EndTime)", CommandType.Text,
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@CallCenterId", callCenterId),
                new SqlParameter("@InterviewerId", interviewerId),
                new SqlParameter("@StartTime", startTime),
                new SqlParameter("@EndTime", endTime));
        }

        private DatabaseEngine PrepareTest()
        {
            var dbName = "PersonSessionHistoryRepository_InsertStartSessionEvent_Success-" + Guid.NewGuid();
            var engine = TestingFramework.CreateDatabaseOnTest(dbName);
            engine.ExecuteNonQuery(
                @"	CREATE TABLE [dbo].[CatiInterviewerSessionHistory]
	(
		[SessionId] INT IDENTITY(1,1),
		[CompanyId] INT NOT NULL,
		[CallCenterId] INT NOT NULL,
		[InterviewerId] INT NOT NULL,
		[LoginTime] DATETIME NOT NULL,
		[LogoutTime] DATETIME,
		[DurationRangeType] AS CASE WHEN DATEDIFF(HOUR , LoginTime, LogoutTime) >= 40 THEN 1 ELSE 0 END PERSISTED,
		CONSTRAINT [PK_CatiInterviewerSessionHistory] PRIMARY KEY CLUSTERED([SessionId] ASC)
	)", CommandType.Text);

            BackendInstance.Current.ConfirmlogConnectionString = engine.ConnectionString;

            TestingFramework.RegistryStub<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>();
            return engine;
        }
    }
}
