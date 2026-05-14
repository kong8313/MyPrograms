using System;
using System.Data;
using System.Text.RegularExpressions;

using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.SurveyArchive
{
    [TestClass]
    public class SurveyArchiveServiceTests : BaseMockedIntegrationTest
    {
        private const string ProjectId = "p0000001";
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private readonly DatabaseEngine _confirmitSurveyDb;

        public SurveyArchiveServiceTests()
        {
            _confirmitSurveyDb = new DatabaseEngine(_framework.GetConfirmitSqlServerConnectionString(_framework.TestSurveyDatabaseName));
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);
            base.TestCleanup();
        }

        private void FillSurveyData()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            var formData = new[] 
            { 
                new FormData { Name = "q1" },
                new FormData { Name = "q2" },
                new FormData { Name = "key" }
            };

            var sdb = new SurveyDatabaseBuilder(_confirmitSurveyDb, formData);

            const int batchId = 1;
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "0", InterviewerId = "1", TelephoneNumber = "5550", ExtensionNumber = "0", LastChannelId = "1", TimeZoneId = "0", RespondentName = "0", DialMode = "1", Data = "q1=2,q2=3,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "1", InterviewerId = "2", TelephoneNumber = "5551", ExtensionNumber = "1", LastChannelId = "1", TimeZoneId = "1", RespondentName = "1", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "2", InterviewerId = "3", TelephoneNumber = "5552", ExtensionNumber = "2", LastChannelId = "1", TimeZoneId = "2", RespondentName = "2", DialMode = "1", Data = "q1=6,q2=9,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "3", InterviewerId = "4", TelephoneNumber = "5553", ExtensionNumber = "3", LastChannelId = "1", TimeZoneId = "3", RespondentName = "3", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "4", InterviewerId = "5", TelephoneNumber = "5554", ExtensionNumber = "4", LastChannelId = "1", TimeZoneId = "4", RespondentName = "4", DialMode = "1", Data = "q1=10,q2=15,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "5", InterviewerId = "6", TelephoneNumber = "5555", ExtensionNumber = "5", LastChannelId = "1", TimeZoneId = "5", RespondentName = "5", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "6", InterviewerId = "7", TelephoneNumber = "5556", ExtensionNumber = "6", LastChannelId = "1", TimeZoneId = "6", RespondentName = "6", DialMode = "1", Data = "q1=14,q2=21,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "7", InterviewerId = "8", TelephoneNumber = "5557", ExtensionNumber = "7", LastChannelId = "1", TimeZoneId = "0", RespondentName = "7", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "8", InterviewerId = "9", TelephoneNumber = "5558", ExtensionNumber = "8", LastChannelId = "1", TimeZoneId = "1", RespondentName = "8", DialMode = "1", Data = "q1=18,q2=27,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "9", InterviewerId = "10", TelephoneNumber = "5559", ExtensionNumber = "9", LastChannelId = "1", TimeZoneId = "2", RespondentName = "9", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "10", InterviewerId = "11", TelephoneNumber = "55510", ExtensionNumber = "10", LastChannelId = "1", TimeZoneId = "3", RespondentName = "10", DialMode = "1", Data = "q1=22,q2=33,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "11", InterviewerId = "12", TelephoneNumber = "55511", ExtensionNumber = "11", LastChannelId = "1", TimeZoneId = "4", RespondentName = "11", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "12", InterviewerId = "13", TelephoneNumber = "55512", ExtensionNumber = "12", LastChannelId = "1", TimeZoneId = "5", RespondentName = "12", DialMode = "1", Data = "q1=26,q2=39,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "13", InterviewerId = "14", TelephoneNumber = "55513", ExtensionNumber = "13", LastChannelId = "1", TimeZoneId = "6", RespondentName = "13", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "14", InterviewerId = "15", TelephoneNumber = "55514", ExtensionNumber = "14", LastChannelId = "1", TimeZoneId = "0", RespondentName = "14", DialMode = "1", Data = "q1=30,q2=45,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "15", InterviewerId = "16", TelephoneNumber = "55515", ExtensionNumber = "15", LastChannelId = "1", TimeZoneId = "1", RespondentName = "15", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "16", InterviewerId = "17", TelephoneNumber = "55516", ExtensionNumber = "16", LastChannelId = "1", TimeZoneId = "2", RespondentName = "16", DialMode = "1", Data = "q1=34,q2=51,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "17", InterviewerId = "18", TelephoneNumber = "55517", ExtensionNumber = "17", LastChannelId = "1", TimeZoneId = "3", RespondentName = "17", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "18", InterviewerId = "19", TelephoneNumber = "55518", ExtensionNumber = "18", LastChannelId = "1", TimeZoneId = "4", RespondentName = "18", DialMode = "1", Data = "q1=38,q2=57,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "19", InterviewerId = "20", TelephoneNumber = "55519", ExtensionNumber = "19", LastChannelId = "1", TimeZoneId = "5", RespondentName = "19", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "20", InterviewerId = "21", TelephoneNumber = "55520", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "6", RespondentName = "20", DialMode = "1", Data = "q1=42,q2=63,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "21", InterviewerId = "22", TelephoneNumber = "55521", ExtensionNumber = "21", LastChannelId = "1", TimeZoneId = "0", RespondentName = "21", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "22", InterviewerId = "23", TelephoneNumber = "55522", ExtensionNumber = "22", LastChannelId = "1", TimeZoneId = "1", RespondentName = "22", DialMode = "1", Data = "q1=46,q2=69,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "23", InterviewerId = "24", TelephoneNumber = "55523", ExtensionNumber = "23", LastChannelId = "1", TimeZoneId = "2", RespondentName = "23", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "24", InterviewerId = "25", TelephoneNumber = "55524", ExtensionNumber = "24", LastChannelId = "1", TimeZoneId = "3", RespondentName = "24", DialMode = "1", Data = "q1=50,q2=75,key=9" });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Archive_SurveyWithSpecificSchedulingScript_Successed()
        {
            //reset new id;
            Action._newID = 1;
            var surveyId = BackendToolsObject.CreateSurvey(new TestScript(
                new Action(Action.Operation.IncrementPriority, "1"), new Shift(1, 1, "1.00:00:00", "2.00:00:00")));

            var survey = SurveyRepository.GetById(surveyId);
            var archive = ServiceLocator.Resolve<ISurveyArchiveService>().Archive(survey);

            archive = Regex.Replace(
                archive, @"[0-9abcdef]{8}\-[0-9abcdef]{4}\-[0-9abcdef]{4}\-[0-9abcdef]{4}\-[0-9abcdef]{12}", "{Guid}");

            archive = archive.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "{SchemaUrl}");
            archive = archive.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "{SchemaUrl}");

            BackendTools.WriteAllText(@"SurveyArchive\Archive_SurveyWithSpecificSchedulingScript_Successed.xml", archive);

            var expected = BackendTools.ReadAllText(@"SurveyArchive\Archive_SurveyWithSpecificSchedulingScript_Successed.xml");

            Assert.AreEqual(expected, archive);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Restore_SurveyWithSpecificSchedulingScript_Successed()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            // Create survey
            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScript_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var survey = SurveyRepository.GetById(surveyId);
            Assert.AreEqual(49, survey.ScheduleID);
            Assert.AreEqual(StateGroupRepository.GetDefault().ID, survey.StateGroupID);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreNotEqual(0, replDataCount);
            Assert.AreNotEqual(0, intrDataCount);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Restore_SurveyWithSpecificSchedulingScriptAndStateGroupIsDeletedInDb_SuccessedAnSetdDefaultStateGroup()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();
            // Create survey
            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var survey = SurveyRepository.GetById(surveyId);
            Assert.AreEqual(49, survey.ScheduleID);
            Assert.AreEqual(StateGroupRepository.GetDefault().ID, survey.StateGroupID);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreNotEqual(0, replDataCount);
            Assert.AreNotEqual(0, intrDataCount);
        }        

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(60954)]
        public void Restore_SurveyWithSpecificSchedulingScriptAndStateGroupWhichAreadyExist_SuccessedStateGroupInDbDidntChange()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();
            // Create survey
            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            const int existingStateGroupId = 127;

            _framework.DbEngine.ExecuteNonQuery(string.Format("INSERT INTO BvStateGroup ([ID], [Name], [Order], [Deleted]) VALUES ({0}, 'Existing group', 100, 0)", existingStateGroupId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var survey = SurveyRepository.GetById(surveyId);
            Assert.AreEqual(49, survey.ScheduleID);
            Assert.AreEqual(existingStateGroupId, survey.StateGroupID);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreNotEqual(0, replDataCount);
            Assert.AreNotEqual(0, intrDataCount);

            //reset survey
            survey.ScheduleID = BackendTools.GetDefaultScheduleID();
            survey.StateGroupID = StateGroupRepository.GetDefault().ID;
            SurveyRepository.Update(survey);
            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            survey = SurveyRepository.GetById(surveyId);
            Assert.AreEqual(49, survey.ScheduleID);
            Assert.AreEqual(existingStateGroupId, survey.StateGroupID);
            Assert.AreEqual(0, StateRepository.GetAll(127).Count);

            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvStateGroup WHERE ID={0}", existingStateGroupId), CommandType.Text);

            replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreNotEqual(0, replDataCount);
            Assert.AreNotEqual(0, intrDataCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(59563)]
        public void Restore_SurveyWithNullLastChannelID_Successed()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            // Create survey
            var surveyDb = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();

            surveyDb.ExecuteNonQuery("UPDATE [respondent] SET [LastChannelId]=NULL", CommandType.Text);

            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreEqual(25, replDataCount);
            Assert.AreEqual(25, intrDataCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(59563)]
        public void Restore_SurveyWithNullBatchID_Successed()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();
            // Create survey
            var surveyDb = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();

            surveyDb.ExecuteNonQuery("UPDATE TOP(2) [respondent] SET [BatchId]=NULL", CommandType.Text);

            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreEqual(23, replDataCount);
            Assert.AreEqual(23, intrDataCount);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Restore_SurveyWithNotNullSampleTypeColumn_Successed()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            var surveyDb = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            surveyDb.ExecuteNonQuery("UPDATE [dbo].[respondent] SET DialType = 1", CommandType.Text);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var interDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview WHERE DialTypeId = 1", CommandType.Text);
            var replDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreEqual(25, replDataCount);
            Assert.AreEqual(25, interDataCount);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Restore_SurveyWithNullSampleTypeColumn_Successed()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            var surveyDb = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            surveyDb.ExecuteNonQuery("UPDATE [dbo].[respondent] SET DialType = NULL", CommandType.Text);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreEqual(25, replDataCount);
            Assert.AreEqual(25, intrDataCount);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Restore_2RowsWithTheSameRespId_RestoredOnlyTheFirstRow()
        {
            FillSurveyData();

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            var surveyId = FilterAndPagingToolsObject.CreateSurveyWithSample(ProjectId, FilterAndPagingTools.SampleType.SmallSample);

            var firstRowIts = 10;
            var secondRowIts = 11;
            var surveyDb = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            surveyDb.ExecuteNonQuery($@"INSERT INTO [dbo].[response_control] ([respid], [interviewerid], [companyid], [language], [iterationid], [its], [rowguid], [last_touched])
            VALUES (1, 1, 1, 9, 1, {firstRowIts}, '{Guid.NewGuid()}', '{DateTime.Now}'), (1, 2, 1, 9, 1,  {secondRowIts}, '{Guid.NewGuid()}', '{DateTime.Now}')", CommandType.Text);

            _framework.DbEngine.ExecuteNonQuery("DELETE FROM BvInterview", CommandType.Text);
            _framework.DbEngine.ExecuteNonQuery(string.Format("DELETE FROM BvReplicatedData_{0}", surveyId), CommandType.Text);

            var archive = BackendTools.ReadAllText(@"SurveyArchive\Restore_SurveyWithSpecificSchedulingScriptAndStateGroup_Successed.xml");
            ServiceLocator.Resolve<ISurveyArchiveService>().Restore(surveyId, archive, default);

            var replDataCount = _framework.DbEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM BvInterview", CommandType.Text);
            var intrDataCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM BvReplicatedData_{0}", surveyId), CommandType.Text);
            Assert.AreEqual(25, replDataCount);
            Assert.AreEqual(25, intrDataCount);

            var interview = InterviewRepository.GetById(surveyId, 1);
            Assert.AreEqual(firstRowIts, interview.TransientState);
        }
    }
}
