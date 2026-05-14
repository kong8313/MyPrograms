using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.TimezoneTest
{
    [TestClass]
    public class TimezoneChangingTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        /// <summary>
        /// 1. Do all preparations and start interview.
        /// 2. Check that GetState() returns time zone according site setting.
        /// 3. Change interview time zone.
        /// 4. Check that GetState() returns new time zone.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetState_TimezoneIsChangedDuringInterview_GetStateReturnsNewTimezone()
        {
            var test = new TestCati2(true, false, _backendTools);
            const string user = "testUser1";
            const string password = "password";

            test.CreateSurveyWithPerson(DialingMode.Manual, user, password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);

            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.StartInterview_ManualOrPreview(test.SurveyName, 0);

            var state = test.StateWS.GetState();
            Assert.IsNotNull(state.respondentTimezone, "Respondent timezone is not returned. Seems interview didn't start");
            int defaultTimezone = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            Assert.AreEqual(defaultTimezone, state.respondentTimezone.Id, "Expected site timezone for interview");

            int newTimezone = defaultTimezone + 1;
            TimezoneManager.AddTimezone(newTimezone);

            // updating interview timezone
            interviews[0].TimezoneID = newTimezone;
            InterviewRepository.UpdateOnly(interviews[0]);

            state = test.StateWS.GetState();
            Assert.IsNotNull(state.respondentTimezone, "Respondent timezone is not returned. Seems interview didn't start");
            Assert.AreEqual(newTimezone, state.respondentTimezone.Id, "New timezone is not returned");
        }
    }
}
