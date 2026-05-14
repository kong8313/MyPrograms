using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.OpenEndReview
{
    [TestClass]
    public class OpenReviewTimingsTest : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.OpenEndReviewTimings)]
        public void OpenEndReview_ReviewEnabled_DurationNonZero()
        {
            const int minOpenEndDurationInSec = 65;

            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    Tag="S1", IsOpen = true,DialMode = DialingMode.Automatic, IsUseDb = true, OpenEndReview = true, SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                    },
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var interview = console.StartInterview();

            new DateTimeMocker(TestingFramework).MockOffset(minOpenEndDurationInSec);
            console.StartOpenEndReview();

            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", survey.Id), new SqlParameter("@InterviewId", interview.Id)).Single();

            Assert.IsTrue(history.OpenEndReviewDuration >= minOpenEndDurationInSec);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.OpenEndReviewTimings)]
        public void OpenEndReview_ReviewDisabled_DurationZero()
        {
            const int minOpenEndDurationInSec = 65;
            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    SchedulingScript = AllHoursSchedule.Name,
                    Tag="S1", IsOpen = true,DialMode = DialingMode.Automatic,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                    },
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var interview = console.StartInterview();

            new DateTimeMocker(TestingFramework).MockOffset(minOpenEndDurationInSec);
            console.StartOpenEndReview();

            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", survey.Id), new SqlParameter("@InterviewId", interview.Id)).Single();

            Assert.IsTrue(history.OpenEndReviewDuration == 0);
        }
    }
}
