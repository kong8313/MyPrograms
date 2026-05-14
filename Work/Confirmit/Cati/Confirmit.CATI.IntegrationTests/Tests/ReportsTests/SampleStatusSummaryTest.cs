using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class SampleStatusSummaryTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetSampleStatusSummaryData_AllPeople_ReportIsCorrect()
        {
            var expected = new[]
                           {
                               new SampleStatusSummaryRecord
                               {
                                   Calls = 0, Count = 2, Index = 0, Person = "ALL_PERSONS",
                                   SampleSize = 5, StateID = 2, StateName = "Busy", SurveyName = " (p123456)"
                               },
                               new SampleStatusSummaryRecord
                               {
                                   Calls = 0, Count = 2, Index = 1, Person = "ALL_PERSONS",
                                   SampleSize = 5, StateID = 13, StateName = "Completed", SurveyName = " (p123456)"
                               },
                               new SampleStatusSummaryRecord
                               {
                                   Calls = 0, Count = 1, Index = 2, Person = "ALL_PERSONS",
                                   SampleSize = 5, StateID = 16, StateName = "Fresh sample", SurveyName = " (p123456)"
                               }
                           };

            var surveyId = BackendToolsObject.CreateSurvey("p123456");
            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 5, out interviews, out calls);
            CallTools.MoveCalls(surveyId, new[] {interviews[0].ID, interviews[1].ID}, (int)CallOutcome.Completed);
            CallTools.MoveCalls(surveyId, new[] { interviews[2].ID, interviews[3].ID }, (int)CallOutcome.Busy);

            var itsIds = string.Join(",", (int)CallOutcome.Completed, (int)CallOutcome.FreshSample, (int)CallOutcome.Busy);
            var result = ReportManager.GetSssData(surveyId, null, itsIds, true);

            TestAssert.AreEqual(expected, result, AssertSssRecords);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetSampleStatusSummaryData_TwoPeople_ReportIsCorrect()
        {
            var expected = new[]
                           {
                               new SampleStatusSummaryRecord
                               {
                                   Calls = 0, Count = 1, Index = 0, Person = "user1",
                                   SampleSize = 3, StateID = 2, StateName = "Busy", SurveyName = " (p123456)"
                               },
                               new SampleStatusSummaryRecord
                               {
                                   Calls = 0, Count = 1, Index = 1, Person = "user1",
                                   SampleSize = 3, StateID = 13, StateName = "Completed", SurveyName = " (p123456)"
                               },
                               new SampleStatusSummaryRecord
                               {
                                   Calls = 0, Count = 1, Index = 0, Person = "user2",
                                   SampleSize = 3, StateID = 13, StateName = "Completed", SurveyName = " (p123456)"
                               }
                           };

            var surveyId = BackendToolsObject.CreateSurvey("p123456");
            var person1Id = PersonTools.CreatePerson("user1");
            var person2Id = PersonTools.CreatePerson("user2");
            BackendTools.CreateInterview(new BvInterviewEntity
                                         {
                                             ID = 1,
                                             LastCallPersonSID = person1Id,
                                             SurveySID = surveyId,
                                             TransientState = (int) CallOutcome.Completed
                                         });
            BackendTools.CreateInterview(new BvInterviewEntity
                                         {
                                             ID = 2,
                                             LastCallPersonSID = person1Id,
                                             SurveySID = surveyId,
                                             TransientState = (int) CallOutcome.Busy
                                         });
            BackendTools.CreateInterview(new BvInterviewEntity
                                         {
                                             ID = 3,
                                             LastCallPersonSID = person2Id,
                                             SurveySID = surveyId,
                                             TransientState = (int) CallOutcome.Completed
                                         });



            var itsIds = string.Join(",", (int)CallOutcome.Completed, (int)CallOutcome.Busy);
            var result = ReportManager.GetSssData(surveyId, new[]{person1Id, person2Id}, itsIds, true);

            TestAssert.AreEqual(expected, result, AssertSssRecords);
        }

        private bool AssertSssRecords(SampleStatusSummaryRecord expected, SampleStatusSummaryRecord actual)
        {
            Assert.AreEqual(expected.Calls, actual.Calls, "Calls");
            Assert.AreEqual(expected.Count, actual.Count, "Count");
            Assert.AreEqual(expected.Index, actual.Index, "Index");
            Assert.AreEqual(expected.Person, actual.Person, "Person");
            Assert.AreEqual(expected.SampleSize, actual.SampleSize, "SampleSize");
            Assert.AreEqual(expected.StateID, actual.StateID, "StateID");
            Assert.AreEqual(expected.StateName, actual.StateName, "StateName");
            Assert.AreEqual(expected.SurveyName, actual.SurveyName, "SurveyName");

            return true;
        }
    }
}
