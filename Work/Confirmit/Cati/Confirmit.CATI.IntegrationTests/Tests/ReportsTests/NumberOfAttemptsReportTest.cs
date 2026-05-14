using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class NumberOfAttemptsReportTest
    {
        const string _userName = "testUser_NumberOfAttemptsReportTest";
        private const string _surveyPnumber = "p015366";
        private const int _totalSampleSize = 21;      

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private ISurveyStateService _surveyStateService;


        private int SurveyId { get; set; }
        private int PersonId { get; set; }


        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        private void CreateSurveyPersonAddSample(AgentTaskChoiceMode mode = AgentTaskChoiceMode.Automatic)
        {
            SurveyId = _backendTools.CreateSurvey(_surveyPnumber);
            _surveyStateService.Open(SurveyId);

            PersonId = PersonTools.CreatePerson(_userName, mode);
            BackendTools.AssignCatiPersonToSurvey(SurveyId, PersonId);

            _backendTools.AddSample(_surveyPnumber, 1, (int)SchedulingMode.Simple, 1, 21, new int[] {1,2});
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.NumberOfAttemptsReport)]
        public void NumberOfAttemptsReport_NoAttemps_ZeroAttemptsReturned()
        {
            int totalSampleSize;

            CreateSurveyPersonAddSample();

            var actual = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(SurveyId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, null, out totalSampleSize);

            Assert.AreEqual(_totalSampleSize, totalSampleSize);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.NumberOfAttemptsReport)]
        public void NumberOfAttemptsReport_3AttemptForFirst_1AttemptForSecond_3RecordsReturned()
        {
            int totalSampleSize;

            CreateSurveyPersonAddSample();

            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-3) }, 1);
            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-2) }, 1);
            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-1) }, 1);
            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-1) }, 2);

            var actual = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(SurveyId, DateTime.UtcNow.AddHours(-5), DateTime.UtcNow, null, out totalSampleSize);

            Assert.AreEqual(_totalSampleSize, totalSampleSize);
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(1, actual.First().Attempts);
            Assert.AreEqual(3, actual.Last().Attempts);
            Assert.AreEqual(2, actual.Sum(x=>x.Records));
        }

        [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.NumberOfAttemptsReport)]
        public void NumberOfAttemptsReport_FilterByCallCenter()
        {
            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", IsUseDb = true, Assigns = new[] { "P1" }, CallCenters = new[] { "CC1", "CC2" } }
                },
                Persons = new[] {
                    new PersonData() { Tag = "P1", CallCenter = "CC1" },
                    new PersonData() { Tag = "P2", CallCenter = "CC2" }
                },
                CallCenters = new[] { new CallCenterData() { Tag = "CC1" }, new CallCenterData() { Tag = "CC2" } }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;
            var projectId = context.GetSurvey("S1").Data.ProjectId;
            var personId1 = context.GetPerson("P1").Id;
            var personId2 = context.GetPerson("P2").Id;
            var callCenterId1 = context.GetCallCenter("CC1").Id;
            var callCenterId2 = context.GetCallCenter("CC2").Id;
            var sampleSize = 21;
            _backendTools.AddSample(projectId, 1, (int)SchedulingMode.Simple, 1, sampleSize, new int[] { 1, 2 });

            _backendTools.CreateHistoryRecords(surveyId, personId1, new DateTime[] { DateTime.UtcNow.AddHours(-3) }, 1, callCenterId: callCenterId1);
            _backendTools.CreateHistoryRecords(surveyId, personId1, new DateTime[] { DateTime.UtcNow.AddHours(-2) }, 1, callCenterId: callCenterId1);
            _backendTools.CreateHistoryRecords(surveyId, personId1, new DateTime[] { DateTime.UtcNow.AddHours(-1) }, 1, callCenterId: callCenterId1);
            _backendTools.CreateHistoryRecords(surveyId, personId2, new DateTime[] { DateTime.UtcNow.AddHours(-1) }, 2, callCenterId: callCenterId2);

            var actual1 = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(surveyId, DateTime.UtcNow.AddHours(-5), DateTime.UtcNow, callCenterId1, out var actualSampleSize);

            Assert.AreEqual(sampleSize, actualSampleSize);
            Assert.AreEqual(3, actual1.Count);
            Assert.AreEqual(1, actual1[0].Attempts);
            Assert.AreEqual(2, actual1[1].Attempts);
            Assert.AreEqual(3, actual1[2].Attempts);
            Assert.AreEqual(1, actual1[2].Records);
            Assert.AreEqual(1, actual1.Sum(x => x.Records));

            var actual2 = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(surveyId, DateTime.UtcNow.AddHours(-5), DateTime.UtcNow, callCenterId2, out actualSampleSize);

            Assert.AreEqual(sampleSize, actualSampleSize);
            Assert.AreEqual(1, actual2.Count);
            Assert.AreEqual(1, actual2[0].Attempts);
            Assert.AreEqual(1, actual2[0].Records);
            Assert.AreEqual(1, actual2.Sum(x => x.Records));

            var actual3 = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(surveyId, DateTime.UtcNow.AddHours(-5), DateTime.UtcNow, null, out actualSampleSize);

            Assert.AreEqual(sampleSize, actualSampleSize);
            Assert.AreEqual(3, actual3.Count);
            Assert.AreEqual(1, actual3[0].Attempts);
            Assert.AreEqual(1, actual3[0].Records);
            Assert.AreEqual(2, actual3[1].Attempts);
            Assert.AreEqual(3, actual3[2].Attempts);
            Assert.AreEqual(1, actual3[2].Records);
            Assert.AreEqual(2, actual3.Sum(x => x.Records));
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.NumberOfAttemptsReport)]
        public void NumberOfAttemptsReport_AttemptForExtendedStatuses15And25_Ignored()
        {
            int totalSampleSize;

            CreateSurveyPersonAddSample();

            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-3) }, 1, 100, 5, 15);
            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-2) }, 1, 100, 5, 25);
            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-1) }, 1);
            _backendTools.CreateHistoryRecords(SurveyId, PersonId, new DateTime[] { DateTime.UtcNow.AddHours(-1) }, 2);

            var actual = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(SurveyId, DateTime.UtcNow.AddHours(-5), DateTime.UtcNow, null, out totalSampleSize);

            Assert.AreEqual(_totalSampleSize, totalSampleSize);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual.First().Attempts);
            Assert.AreEqual(2, actual.First().Records);
        }


    }
}
