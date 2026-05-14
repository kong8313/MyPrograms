using System;
using System.Globalization;
using System.Linq;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallHistoryExportTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private const string ProjectId = "p123817";
        private const string ProjectId2 = "p1456817";
        private const string ProjectName = "SpiSurvey";
        private const int InterviewId = 1;

        private int _surveyId;
        private int _surveyId2;
        private int _personId;

        private ISurveyStateService _surveyStateService;
        private ISurveyService _surveyService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _surveyService = ServiceLocator.Resolve<ISurveyService>();

            _backendTools.LaunchAllHoursScript();

            _surveyId = _backendTools.CreateSurvey(ProjectId);
            var survey = SurveyRepository.GetById(_surveyId);
            survey.Description = ProjectName;
            SurveyRepository.Update(survey);
            _surveyStateService.Open(_surveyId);

            _surveyId2 = _backendTools.CreateSurvey(ProjectId2);

            _personId = PersonTools.CreatePerson("u1", "p1", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(_surveyId, _personId);
            BackendTools.AssignCatiPersonToSurvey(_surveyId2, _personId);

            AddRecorToBvHistory(_personId, _surveyId2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        private void AddRecorToBvHistory(int personId, int surveyId)
        {
            BvHistoryAdapter.Insert(new BvHistoryEntity
            {
                FiredTime = DateTime.UtcNow,
                ITS = 1,
                InterviewId = InterviewId,
                SurveyId = surveyId,
                PersonSID = personId,
                RoleID = 2
            });
        }

        private void AddRecorToBvHistory(int personId)
        {
            AddRecorToBvHistory(personId, _surveyId);
        }

        [TestMethod, Owner(@"FROM\AlexanderL")]
        public void CallHistory_InsertRecordToHistoryForTheSameInterviewTwice_TwoRecordsAreExported()
        {
            AddRecorToBvHistory(_personId);
            AddRecorToBvHistory(_personId);

            var actual = _surveyService.GetCallHistoryData(_surveyId.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null);

            Assert.AreEqual(2, actual.Count, "records count");
        }

        [TestMethod, Owner(@"FROM\AlexanderL")]
        public void CallHistory_InsertRecordForDialer_RecordWithDialerNameIsExported()
        {
            AddRecorToBvHistory(0);

            var actual = _surveyService.GetCallHistoryData(_surveyId.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null);

            Assert.AreEqual(1, actual.Count, "records count");
            Assert.AreEqual("Dialer", actual[0].InterviewerName, "InterviewerName");
            Assert.AreEqual(ProjectName, actual[0].Name, "Project name");
        }

        [TestMethod, Owner(@"FROM\AlexanderL")]
        public void CallHistory_InsertRecordForDeleteInterviewer_RecordWithSpecificNameIsExported()
        {
            AddRecorToBvHistory(_personId + 1);

            var actual = _surveyService.GetCallHistoryData(_surveyId.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null);

            Assert.AreEqual(1, actual.Count, "records count");
            Assert.IsNull(actual[0].InterviewerName, "InterviewerName");
            Assert.AreEqual(ProjectName, actual[0].Name, "Project name");
        }

        [TestMethod, Owner(@"FROM\AlexanderL")]
        public void CallHistory_ExportSeveralSurveys_AllRecordsAreReturned()
        {
            AddRecorToBvHistory(_personId);

            var actual = _surveyService.GetCallHistoryData(null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), null);

            Assert.AreEqual(2, actual.Count, "records count");
            CollectionAssert.AreEquivalent(new[] { ProjectId, ProjectId2 }, actual.Select(x => x.ProjectID).ToArray());
        }
    }
}
