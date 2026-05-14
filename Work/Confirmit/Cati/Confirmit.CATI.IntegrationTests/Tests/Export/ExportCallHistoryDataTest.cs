using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Export
{
    [TestClass]
    public class ExportCallHistoryDataTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ExportCallHistoryData_GetPersonSessionData_DataCorrect()
        {
            var callCenters = new[]
            {
                new CallCenterData { Tag = "CC1" },
                new CallCenterData { Tag = "CC2" }
            };

            var context = PrepareContext(callCenters);

            PrepareSessionDatabase();
            var dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();
            var rep = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();

            var callCenter = context.GetCallCenter("CC1");
            var callCenter2 = context.GetCallCenter("CC2");

            var person1 = context.GetPerson("P1");

            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
            var session = rep.InsertStartSessionEvent(cp, callCenter.Id, person1.Id);
            var firedTime = rep.GetSessionEvents(null, BackendInstance.Current.CompanyId, null, null).First().LoginTime;
            var dateForm = firedTime.AddHours(-1);
            var dateTo = firedTime.AddHours(1);

            var personSessionHistoryData = dataProvider.GetPersonSessionHistoryData(null, dateForm, dateTo).ToList();
            Assert.IsTrue(personSessionHistoryData.Count() == 1);
            Assert.IsTrue(personSessionHistoryData.Count(x => x.ProjectID.ToLowerInvariant().Equals("login")) == 1);

            var record = personSessionHistoryData.Single();

            Assert.IsNull(record.WaitingTime);
            Assert.IsNull(record.ExtendedStatus);
            Assert.IsNull(record.InterviewID);
            Assert.IsTrue(record.ProjectID.ToLowerInvariant().Equals("login"));
            Assert.AreEqual(callCenter.Model.Name, record.CallCenterName);
            Assert.AreEqual(null, record.Duration);
            Assert.AreEqual(firedTime, record.FiredTime);
            Assert.AreEqual(person1.Id, record.InterviewerID);
            Assert.AreEqual(person1.Data.Name, record.InterviewerName);
            Assert.AreEqual(string.Empty, record.Name);
            Assert.AreEqual(string.Empty, record.TelephoneNumber);

            rep.InsertStopSessionEvent(cp, session);

            personSessionHistoryData = dataProvider.GetPersonSessionHistoryData(null, dateForm, dateTo).ToList();
            Assert.IsTrue(personSessionHistoryData.Count() == 2);
            Assert.IsTrue(personSessionHistoryData.Count(x => x.ProjectID.ToLowerInvariant().Equals("login")) == 1);
            Assert.IsTrue(personSessionHistoryData.Count(x => x.ProjectID.ToLowerInvariant().Equals("logout")) == 1);

            personSessionHistoryData = dataProvider.GetPersonSessionHistoryData(callCenter.Id, dateForm, dateTo).ToList();
            Assert.IsTrue(personSessionHistoryData.Count() == 2);
            Assert.IsTrue(personSessionHistoryData.Count(x => x.ProjectID.ToLowerInvariant().Equals("login")) == 1);
            Assert.IsTrue(personSessionHistoryData.Count(x => x.ProjectID.ToLowerInvariant().Equals("logout")) == 1);

            personSessionHistoryData = dataProvider.GetPersonSessionHistoryData(callCenter2.Id, dateForm, dateTo).ToList();
            Assert.IsFalse(personSessionHistoryData.Any());
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ExportCallHistoryData_GetPersonBreakData_DataCorrect()
        {
            var callCenters = new [] { new CallCenterData{Tag="CC1"}};
            var context = PrepareContext(callCenters);

            var dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();
            var breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();
            var breakTypes = breakTypeRepository.GetAll();
            var availableBreak = breakTypes.First();
            var notExistingBreakId = breakTypes.Max(x => x.Id) + 1000;

            var now = DateTime.Now;
            var firedTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var dateForm = firedTime.AddHours(-1);
            var dateTo = firedTime.AddHours(1);
            var survey = context.GetSurvey("S1");
            var person1 = context.GetPerson("P1");
            var callCenter = context.GetCallCenter("CC1");
            
            const int duration = 10;

            BvTimeBreaksHistoryAdapter.Insert(new BvTimeBreaksHistoryEntity
            {
                CallCenterId = callCenter.Id,
                Duration = duration,
                InterviewerId = person1.Id,
                StartTime = firedTime,
                SurveyId = survey.Id,
                BreakTypeId = availableBreak.Id
            });
            BvTimeBreaksHistoryAdapter.Insert(new BvTimeBreaksHistoryEntity
            {
                CallCenterId = callCenter.Id,
                Duration = duration,
                InterviewerId = person1.Id,
                StartTime = firedTime.AddHours(0.5),
                SurveyId = survey.Id,
                BreakTypeId = notExistingBreakId
            });

            var breaksData = dataProvider.GetInterviewerBreaksData(null, dateForm, dateTo).ToList();
            var record = breaksData[0];

            Assert.IsNull(record.WaitingTime);
            Assert.IsNull(record.ExtendedStatus);
            Assert.IsNull(record.InterviewID);
            Assert.AreEqual(
                $"BREAK {availableBreak.Name} {(!availableBreak.IsPaid ? "Unpaid" : "Paid")} {survey.Model.Name}",
                record.ProjectID);
            Assert.AreEqual(callCenter.Model.Name, record.CallCenterName);
            Assert.AreEqual(duration, record.Duration);
            Assert.AreEqual(firedTime, record.FiredTime);
            Assert.AreEqual(person1.Id, record.InterviewerID);
            Assert.AreEqual(person1.Data.Name, record.InterviewerName);
            Assert.AreEqual(survey.Model.Description, record.Name);
            Assert.AreEqual(string.Empty, record.TelephoneNumber);

            Assert.AreEqual(
                $"BREAK DELETED BREAKTYPE {survey.Model.Name}",
                breaksData[1].ProjectID);

            breaksData = dataProvider.GetInterviewerBreaksData(null, null, null).ToList();
            Assert.IsTrue(breaksData.Count == 2);

            breaksData = dataProvider.GetInterviewerBreaksData(null, dateForm, null).ToList();
            Assert.IsTrue(breaksData.Count == 2);

            breaksData = dataProvider.GetInterviewerBreaksData(null, null, dateTo).ToList();
            Assert.IsTrue(breaksData.Count == 2);

            breaksData = dataProvider.GetInterviewerBreaksData(null, dateTo, null).ToList();
            Assert.IsTrue(breaksData.Count == 0);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ExportCallHistoryData_GetCallHistoryDataWithReplicatedVariables_VariablesExistInReport()
        {
            var callCenters = new[] { new CallCenterData { Tag = "CC1" } };
            PrepareContext(callCenters, true);
            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();
            var dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();

            var columnNames = new[] { "q1", "q2" };

            var result = dataProvider.GetCallHistoryData(null, null, null, columnNames);
            Assert.IsTrue(result.Count() == 1);

            var item = result.First();
            Assert.AreEqual("2", item.ReplicatedVariables[0]);
            Assert.AreEqual("1", item.ReplicatedVariables[1]);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ExportCallHistoryData_PassFakeSurveySIDThatContainsRealSurveySID_NoRecordsReturned()
        {
            var callCenters = new[] { new CallCenterData { Tag = "CC1" } };
            var context = PrepareContext(callCenters);

            var dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();

            var survey = context.Surveys.First();
            var surveySid = survey.Id;
            var fakeSurveySid = string.Format("11{0}", surveySid);

            var result = dataProvider.GetCallHistoryData(fakeSurveySid, null, null, new string[] { });
            Assert.AreEqual(0, result.Count(), "Should return empty result as we pass fake SurveySID");
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ExportCallHistoryData_PassMultipleSurveySIDs_DataForOneSurveySIDExists()
        {
            var callCenters = new[] { new CallCenterData { Tag = "CC1" } };
            var context = PrepareContext(callCenters);

            var dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();

            var survey = context.Surveys.First();

            var surveySid1 = survey.Id;
            var surveySid2 = survey.Id + 1;
            var surveySid3 = survey.Id + 2;
            var surveySids = string.Format("{0},{1},{2}", surveySid2, surveySid1, surveySid3);

            var result = dataProvider.GetCallHistoryData(surveySids, null, null, new string[] { });
            Assert.AreEqual(1, result.Count());
        }

        private TestDataContext PrepareContext(CallCenterData[] callCenters, bool isUseDb = false)
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData() {Name = "q2", Precodes = new[] {"1", "2"}}
                        },
                        Tag = "S1",
                        IsUseDb = isUseDb,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Data = "q1=2,q2=1",
                                Call = new CallData(),
                                CallHistory = new[]
                                {
                                    new CallHistoryData {Tag = "S1.C1", Person = "P1"}
                                }
                            },
                        },

                        Assigns = new[] {"P1"},
                    }
                },
                Persons = new[]
                {
                    new PersonData
                    {
                        Tag = "P1",
                        TaskChoice = TaskChoiceMode.Manual,
                        Name = "personAbc",
                        CallCenter = callCenters.First().Tag
                    }
                },
                CallCenters = callCenters
            }.Create();
            return context;
        }

        private void PrepareSessionDatabase()
        {
            var dbName = "ExportCallHistoryDataTest_" + BackendInstance.Current.CompanyId;
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

