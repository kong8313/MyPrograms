using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistTestAutomaticTaskChoice
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            BackendTools.ResetInterviewId();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        private void CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser(string blackListpatter, string interviewTelNumber, DialingMode surveyDialingMode)
        {
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            ServiceLocator.Resolve<ITelephoneBlacklistRepository>().Insert(new BvTelephoneBlacklistEntity { DisplayPattern = blackListpatter });

            var test = new TestCati2(true, false, _backendTools);

            var surveyId = test.CreateSurveyWithPerson(surveyDialingMode, user, password, AgentTaskChoiceMode.Automatic);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            var interview = BackendTools.NewInterview(surveyId);
            interview.TelephoneNumber = interviewTelNumber;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(extensionNumber);

            Assert.IsTrue(test.WS.StartInterview("", 0));

            Assert.AreEqual((int)CallOutcome.Blacklist, (int)InterviewRepository.GetById(survey.SID, interview.ID).TransientState);

            test.CheckState(new State(
                                null, null, 0, null, null,
                                (int)InterviewState.WAITING,
                                (int)CallOutcome.Blacklist,
                                (int)LoginState.LOGGED_IN,
                                (int)LoginState.LOGGED_IN,
                                (int)DialerErrorCode.Success,
                                0,
                                false));

            Assert.IsNull(TaskService.LookupByPersonSid(test.PersonSID, survey.SID));

            Assert.AreEqual(0, BvSvyScheduleAdapter.GetAll().Count(x => x.CallState != 0));
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForAutomaticUserAutomaticSurvey_NumberFromBlacklist_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser("12345", "(1) 23-45", DialingMode.Automatic);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForAutomaticUserManualSurvey_NumberFromBlacklist_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser("12345", "(1) 23-45", DialingMode.Manual);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForAutomaticUserPreviewSurvey_NumberFromBlacklist_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser("12345", "123 45", DialingMode.Preview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void StartInterviewForAutomaticUserAutomaticSurvey_NumberFromBlacklistPattern_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser("123*", "1234 5", DialingMode.Automatic);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void StartInterviewForAutomaticUserManualSurvey_NumberFromBlacklistPattern_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser("123*", "12 345", DialingMode.Manual);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void StartInterviewForAutomaticUserPreviewSurvey_NumberFromBlacklistPattern_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForAutomaticUser("123*", "1234+5", DialingMode.Preview);
        }
    }
}
