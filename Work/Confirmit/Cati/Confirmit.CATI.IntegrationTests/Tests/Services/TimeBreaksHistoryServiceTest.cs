using System;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Services
{
    [TestClass]
    public class TimeBreaksHistoryServiceTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInterviewerBreaks_NotExistsRecordsForSpecificSurveys_ResultAreEmpty()
        {
            var startTime = DateTime.Parse("2012-02-13 21:03:36.656");
            var finishTime = startTime.AddMonths(1);

            var surveyId1 = BackendToolsObject.CreateSurvey("p00000001");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00000002");
            var surveyId3 = BackendToolsObject.CreateSurvey("p00000003");

            var tbhe = new BvTimeBreaksHistoryEntity()
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = 1,
                StartTime = startTime.AddDays(1),
                SurveyId = surveyId1
            };
            BvTimeBreaksHistoryAdapter.Insert(tbhe);

            var surveys = String.Format("{0},{1}", surveyId2, surveyId3);
            Assert.AreEqual(0, TimeBreaksHistoryService.GetInterviewerBreaks(surveys, startTime, finishTime).Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInterviewerBreaks_OneRecordExistsForSpecificSurveys_ResultisCorect()
        {
            var startTime = DateTime.Parse("2012-02-13 21:03:36.656");
            var finishTime = startTime.AddMonths(1);

            var surveyId1 = BackendToolsObject.CreateSurvey("p00000001");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00000002");
            var surveyId3 = BackendToolsObject.CreateSurvey("p00000003");

            var tbhe = new BvTimeBreaksHistoryEntity()
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = 1,
                StartTime = startTime.AddDays(1),
                SurveyId = surveyId1
            };
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId2;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);

            var surveys = String.Format("{0},{1}", surveyId1, surveyId3);
            Assert.AreEqual(1, TimeBreaksHistoryService.GetInterviewerBreaks(surveys, startTime, finishTime).Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInterviewerBreaks_TwoRecordExistsForSpecificSurvey_ResultIsCorrent()
        {
            var startTime = DateTime.Parse("2012-02-13 21:03:36.656");
            var finishTime = startTime.AddMonths(1);

            var surveyId1 = BackendToolsObject.CreateSurvey("p00000001");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00000002");
            var surveyId3 = BackendToolsObject.CreateSurvey("p00000003");

            var tbhe = new BvTimeBreaksHistoryEntity()
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = 1,
                StartTime = startTime.AddDays(1),
                SurveyId = surveyId1
            };
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId2;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId2;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);

            Assert.AreEqual(2, TimeBreaksHistoryService.GetInterviewerBreaks(surveyId2.ToString(), startTime, finishTime).Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInterviewerBreaks_TwoRecordExistsForSpecificSurveyAndOneIsNotSurveySpecific_ResultIsCorrent()
        {
            var startTime = DateTime.Parse("2012-02-13 21:03:36.656");
            var finishTime = startTime.AddMonths(1);

            var surveyId1 = BackendToolsObject.CreateSurvey("p00000001");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00000002");
            var surveyId3 = BackendToolsObject.CreateSurvey("p00000003");

            
            var tbhe = new BvTimeBreaksHistoryEntity()
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = 1,
                StartTime = startTime.AddDays(1),
                SurveyId = surveyId1
            };
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId2;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId2;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = 0;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);

            Assert.AreEqual(3, TimeBreaksHistoryService.GetInterviewerBreaks(surveyId2.ToString(), startTime, finishTime).Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInterviewerBreaks_RequestAllRecords_ResultIsCorrent()
        {
            var startTime = DateTime.Parse("2012-02-13 21:03:36.656");
            var finishTime = startTime.AddMonths(1);

            var surveyId1 = BackendToolsObject.CreateSurvey("p00000001");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00000002");
            var surveyId3 = BackendToolsObject.CreateSurvey("p00000003");

            new ManagementService().SoftDeleteSurvey("p00000003");

            var tbhe = new BvTimeBreaksHistoryEntity()
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = 1,
                StartTime = startTime.AddDays(1),
                SurveyId = surveyId1
            };
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId2;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId3;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = 0;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);

            Assert.AreEqual(3, TimeBreaksHistoryService.GetInterviewerBreaks(null, startTime, finishTime).Count);
        }
    }
}
