using Confirmit.CATI.IntegrationTests.Framework.ControllerExtensions;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistLaunchSurveyTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\KirillV")]
        public void LaunchSurvey_BlacklistEnabledInitially_NoCallsFilteredAfterRelaunch()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        IsSupportBlackList = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", TelephoneNumber = "100", Data = "q1=", Call = new CallData()},
                            new InterviewData {Tag = "S1.I2", TelephoneNumber = "200", Data = "q1=", Call = new CallData()}
                        }
                    }
                },
                TelephoneBlacklist = new[] {"100"}
            }.Create();

            var testSurveyController = context.GetSurvey("S1");

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int) CallOutcome.FreshSample);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int) CallOutcome.FreshSample);

            testSurveyController.Data.IsSupportBlackList = true;

            testSurveyController.Launch();

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int) CallOutcome.FreshSample);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int) CallOutcome.FreshSample);

            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
            context.GetCall("S1.I2").Assert.IsTrue(x => x != null);

            Assert.AreEqual(0, context.GetInterview("S1.I2").GetCallHistory().Count);
            Assert.AreEqual(0, context.GetInterview("S1.I1").GetCallHistory().Count);
        }
    }
}