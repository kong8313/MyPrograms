using System;
using System.Data;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Reports;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Common;

using System.Threading;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class InterviewerSessionsTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private int _timezoneId;
        private BvBreakTypeEntity _breakType;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(false);
            _backendTools = new BackendTools(_framework);

            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();

            var breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();

            breakTypeRepository.Insert(new BvBreakTypeEntity() { Name = "InterviewerSessionsTest" });
            _breakType = breakTypeRepository.GetAll().First(x => x.Name.Equals("InterviewerSessionsTest"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void InterviewerSessionsTest_SeveralPersonsWithBreaks_ReportIsCorrect()
        {
            const string user1 = "testUser1";
            const string password1 = "password1";
            const string user2 = "testUser2";
            const string password2 = "password2";
            const int callCenterId = 1;

            var test1 = new TestCati2(false, _backendTools);
            test1.CreateSurveyWithPerson(DialingMode.Automatic, user1, password1, AgentTaskChoiceMode.Automatic);

            test1.Login(user1, password1, AgentTaskChoiceMode.Automatic, false);
            test1.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakType.Id);
            var startTime1 = TimezoneManager.GetCurrentTimeByTzId(_timezoneId);
            Thread.Sleep(1 * 1000);
            test1.WS.ContinueWorkAfterBreak(1);

            var test2 = new TestCati2(false, _backendTools);
            test2.CreateSurveyWithPerson(DialingMode.Automatic, user2, password2, AgentTaskChoiceMode.Automatic);

            test2.Login(user2, password2, AgentTaskChoiceMode.Automatic, false);
            test2.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakType.Id);
            var startTime2 = TimezoneManager.GetCurrentTimeByTzId(_timezoneId);
            Thread.Sleep(1 * 1000);

            var args = new PagingArgs(1, 10, "PersonName", true, new SearchParameterCollection());

            int totalCount;
            var parameters = new InterviewerSessionsReportParams
            {
                Persons = new[] { test1.PersonSID, test2.PersonSID },
                PagingArgs = args,
                TimezoneId = _timezoneId,
                CallCenterId = callCenterId,
                CompanyId = BackendInstance.Current.CompanyId,
                EventType = (int)InterviewerBreakReportEvent.Break
            };
            var reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(2, reportRecords.Count, "Count of records");
            Assert.AreEqual(user1, reportRecords[0].PersonName, "user name");
            Assert.AreEqual(_breakType.Name, reportRecords[0].Note);

            Assert.IsTrue(startTime1 >= reportRecords[0].StartTime.Value.CutMilliseconds(), "StartTime is incorrect");
            Assert.IsTrue(1 <= reportRecords[0].Duration, "Duration is incorrect");

            Assert.AreEqual(user2, reportRecords[1].PersonName, "user name");
            Assert.AreEqual(_breakType.Name, reportRecords[1].Note);

            Assert.IsTrue(startTime2 >= reportRecords[1].StartTime.Value.CutMilliseconds(), "StartTime is incorrect");
            Assert.IsNull(reportRecords[1].Duration, "Duration is incorrect");
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void InterviewerSessionsTest_VariousPagingParamsAndPersonIdsFilter_ReportIsCorrect()
        {
            var context = GetDataContextWithTwoPersons();
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            PrepareSessionDatabase();

            var callcenterId = FillDatabase(person1, person2, survey);

            int totalCount;

            var args = new PagingArgs(1, 10, "PersonName", true, new SearchParameterCollection());

            var parameters = new InterviewerSessionsReportParams
            {
                Persons = new[] { person1.Id, person2.Id },
                PagingArgs = args,
                TimezoneId = _timezoneId,
                CallCenterId = callcenterId,
                CompanyId = BackendInstance.Current.CompanyId,
                EventType = (int)InterviewerBreakReportEvent.NotDefined
            };

            var reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(10, reportRecords.Count, "Count of records with two persons on first page");
            Assert.AreEqual(24, totalCount, "Total count with two persons");

            parameters.PagingArgs.PageIndex = 3;
            reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(4, reportRecords.Count, "Count of records for third page");
            Assert.AreEqual(24, totalCount, "Total count with two persons");

            parameters.PagingArgs.PageIndex = 1;
            parameters.Persons = new[] { person1.Id };
            reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.IsTrue(reportRecords.TrueForAll(x => x.PersonName == person1.Data.Name));
            Assert.AreEqual(10, reportRecords.Count, "Count of records for person1 on first page");
            Assert.AreEqual(12, totalCount, "Total count for person1");
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void InterviewerSessionsTest_DifferentEventTypes_ReportIsCorrect()
        {
            var context = GetDataContextWithTwoPersons();
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            PrepareSessionDatabase();

            var callcenterId = FillDatabase(person1, person2, survey);

            int totalCount;

            var args = new PagingArgs(1, 10, "PersonName", true, new SearchParameterCollection());

            var parameters = new InterviewerSessionsReportParams
            {
                Persons = new[] { person1.Id, person2.Id },
                PagingArgs = args,
                TimezoneId = _timezoneId,
                CallCenterId = callcenterId,
                CompanyId = BackendInstance.Current.CompanyId,
                EventType = (int)InterviewerBreakReportEvent.NotDefined
            };

            var reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(10, reportRecords.Count, "Count of records");
            Assert.AreEqual(24, totalCount, "Total count");

            parameters.EventType = (int)InterviewerBreakReportEvent.Break;
            reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.IsTrue(reportRecords.TrueForAll(x => x.Event == (int)InterviewerBreakReportEvent.Break));
            Assert.AreEqual(10, reportRecords.Count, "Count of records");
            Assert.AreEqual(12, totalCount, "Total count");

            parameters.EventType = (int)InterviewerBreakReportEvent.Login;
            reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.IsTrue(reportRecords.TrueForAll(x => x.Event == (int)InterviewerBreakReportEvent.Login));
            Assert.AreEqual(10, reportRecords.Count, "Count of records");
            Assert.AreEqual(12, totalCount, "Total count");
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void InterviewerSessionsTest_OrderByAscAndDesc_ReportIsCorrect()
        {
            var context = GetDataContextWithTwoPersons();
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            PrepareSessionDatabase();

            var callcenterId = FillDatabase(person1, person2, survey);

            int totalCount;

            var args = new PagingArgs(1, 10, "PersonName", true, new SearchParameterCollection());

            var parameters = new InterviewerSessionsReportParams
            {
                Persons = new[] { person1.Id, person2.Id },
                PagingArgs = args,
                TimezoneId = _timezoneId,
                CallCenterId = callcenterId,
                CompanyId = BackendInstance.Current.CompanyId,
                EventType = (int)InterviewerBreakReportEvent.NotDefined
            };

            var reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(10, reportRecords.Count, "Count of records");
            Assert.AreEqual(24, totalCount, "Total count");
            Assert.IsTrue(reportRecords[0].PersonName == person1.Data.Name, "Sort order is wrong. Person1 is not on the top.");

            parameters.PagingArgs.SortOrderAsc = false;
            reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(10, reportRecords.Count, "Count of records");
            Assert.AreEqual(24, totalCount, "Total count");
            Assert.IsTrue(reportRecords[0].PersonName == person2.Data.Name, "Sort order is wrong. Person2 is not on the top.");
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void InterviewerSessionsTest_OnePersonIsLoggedIn_FinishTimeIsNull()
        {
            var context = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1", TaskChoice = TaskChoiceMode.Manual, Name = "personAbc"}
                }
            }.Create();

            var person1 = context.GetPerson("P1");
            PrepareSessionDatabase();

            var callcenterId = 10;

            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
            rep.InsertStartSessionEvent(cp, callcenterId, person1.Id);
            
            int totalCount;

            var args = new PagingArgs(1, 10, "PersonName", true, new SearchParameterCollection());

            var parameters = new InterviewerSessionsReportParams
            {
                Persons = new[] { person1.Id },
                PagingArgs = args,
                TimezoneId = _timezoneId,
                CallCenterId = callcenterId,
                CompanyId = BackendInstance.Current.CompanyId,
                EventType = (int)InterviewerBreakReportEvent.NotDefined
            };

            var reportRecords = ReportManager.GetInterviewerSessions(parameters, out totalCount);

            Assert.AreEqual(1, reportRecords.Count, "Count of records");
            Assert.AreEqual(1, totalCount, "Total count");
            Assert.IsNull(reportRecords[0].FinishTime);
        }

        private static int FillDatabase(PersonController person1, PersonController person2, SurveyController survey)
        {
            var callcenterId = 10;

            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var breakHistoryEntityP1 = new BvTimeBreaksHistoryEntity
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = person1.Id,
                StartTime = DateTime.Now.AddMonths(-1),
                SurveyId = survey.Id
            };
            var breakHistoryEntityP2 = new BvTimeBreaksHistoryEntity
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = person2.Id,
                StartTime = DateTime.Now.AddMonths(-1),
                SurveyId = survey.Id
            };

            for (var i = 0; i < 6; i++)
            {
                BvTimeBreaksHistoryAdapter.Insert(breakHistoryEntityP1);
                BvTimeBreaksHistoryAdapter.Insert(breakHistoryEntityP2);
                var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
                var sessionIdp1 = rep.InsertStartSessionEvent(cp, callcenterId, person1.Id);
                rep.InsertStopSessionEvent(cp, sessionIdp1);
                var sessionIdp2 = rep.InsertStartSessionEvent(cp, callcenterId, person2.Id);
                rep.InsertStopSessionEvent(cp, sessionIdp2);
            }
            return callcenterId;
        }

        private TestDataContext GetDataContextWithTwoPersons()
        {
            return new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData()},
                        },
                        Assigns = new[] {"P1", "P2"}
                    }
                },
                Persons = new[]
                {
                    new PersonData {Tag = "P1", TaskChoice = TaskChoiceMode.Manual, Name = "personAbc"},
                    new PersonData {Tag = "P2", TaskChoice = TaskChoiceMode.Manual, Name = "personDef"}
                }
            }.Create();
        }

        private void PrepareSessionDatabase()
        {
            var dbName = "InterviewerSessionsTest_" + BackendInstance.Current.CompanyId;
            var engine = _framework.CreateDatabaseOnTest(dbName);
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

            _framework.RegistryStub<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>();
        }
    }
}