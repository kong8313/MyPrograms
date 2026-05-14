using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistTestSurveyAssignmentTaskChoice
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

        

        private void CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser(string blackListPattern, string interviewTelNumber, DialingMode surveyDialingMode)
        {
            const string user = "testUser";
            const string password = "password";

            ServiceLocator.Resolve<ITelephoneBlacklistRepository>().Insert(new BvTelephoneBlacklistEntity { DisplayPattern = blackListPattern });

            var test = new TestCati2(true, false, _backendTools);

            var surveyId = test.CreateSurveyWithPerson(surveyDialingMode, user, password, AgentTaskChoiceMode.CampaignAssignment);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            var interview = BackendTools.NewInterview(surveyId);
            interview.TelephoneNumber = interviewTelNumber;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true);

            BackendTools.RunSchedulingProcedure();

            Assert.IsTrue(test.WS.StartInterview(survey.Name, 0));

            Assert.AreEqual((int)CallOutcome.Blacklist, InterviewRepository.GetById(survey.SID, interview.ID).TransientState);

            test.CheckState(new State(
                                survey.Name, null, 0, null, null,
                                (int)InterviewState.WAITING,
                                (int)CallOutcome.Blacklist,
                                (int)LoginState.LOGGED_IN,
                                (int)LoginState.NOT_LOGGED_IN,
                                (int)DialerErrorCode.Success,
                                0,
                                false));

            Assert.IsNull(TaskService.LookupByPersonSid(test.PersonSID, survey.SID));

            Assert.AreEqual(0, BvSvyScheduleAdapter.GetAll().Count(x => x.CallState != 0));
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForSurveyAssignedUserAutomaticSurvey_NumberFromBlacklist_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser("12345", "12345", DialingMode.Automatic);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForSurveyAssignedUserManualSurvey_NumberFromBlacklist_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser("12345", "12345", DialingMode.Manual);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForSurveyAssignedUserPreviewSurvey_NumberFromBlacklist_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser("12345", "12345", DialingMode.Preview);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForSurveyAssignedUserAutomaticSurvey_NumberFromBlacklistPattern_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser("123*", "12345", DialingMode.Automatic);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForSurveyAssignedUserManualSurvey_NumberFromBlacklistPattern_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser("123*", "12345", DialingMode.Manual);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void StartInterviewForSurveyAssignedUserPreviewSurvey_NumberFromBlacklistPattern_WaitingState()
        {
            CheckStateIsWaitingAfterStartBadInterviewForSurveyAssignedUser("123*", "12345", DialingMode.Preview);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void OnDialerRequestCallsForPredictiveSurvey_CallWithNumberFromBlacklist_CallIsNotPassedToDialer()
        {
            const string badTelephoneNumber = "12345";
            const string goodTelephoneNumber = "67890";
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            ServiceLocator.Resolve<ITelephoneBlacklistRepository>().Insert(new BvTelephoneBlacklistEntity { DisplayPattern = badTelephoneNumber });

            var test = new TestCati2(true, false, _backendTools);

            var surveyId = test.CreateSurveyWithPerson(DialingMode.Predictive, user, password, AgentTaskChoiceMode.CampaignAssignment);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            // create 2 interviews with calls
            var badInterview = BackendTools.NewInterview(surveyId);
            badInterview.TelephoneNumber = badTelephoneNumber;
            BackendTools.CreateInterview(badInterview);
            var badCall = BackendTools.NewCall(badInterview);
            BackendTools.CreateCall(badCall);
            var goodInterview = BackendTools.NewInterview(surveyId);
            goodInterview.TelephoneNumber = goodTelephoneNumber;
            BackendTools.CreateInterview(goodInterview);
            var goodCall = BackendTools.NewCall(goodInterview);
            BackendTools.CreateCall(goodCall);

            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(extensionNumber, false, null);

            var stubTelephony = new StubITelephony
            {
                SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32Boolean =
                    (id, requestId, campaignId, mode, list, timeout, recording) => DialerErrorCode.Success
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubTelephony);

            BackendTools.RunSchedulingProcedure();

            test.StartInterview_Predictive(2);

            Assert.AreEqual((int)CallOutcome.Blacklist, InterviewRepository.GetById(survey.SID, badInterview.ID).TransientState);
            Assert.IsFalse(BackendTools.IsCallExists(survey.SID, badInterview.ID));

            Assert.AreNotEqual((int)CallOutcome.Blacklist, InterviewRepository.GetById(survey.SID, goodInterview.ID).TransientState);
            Assert.IsTrue(BackendTools.IsCallExists(survey.SID, goodInterview.ID));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void OnDialerRequestCallsForPredictiveSurvey_CallWithNumberFromBlacklistWithStartWithPattern_CallIsNotPassedToDialer()
        {
            const string blackListPattern = "123*";
            const string badTelephoneNumber = "12345";
            const string goodTelephoneNumber = "67890";
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            ServiceLocator.Resolve<ITelephoneBlacklistRepository>().Insert(new BvTelephoneBlacklistEntity { DisplayPattern = blackListPattern });

            var test = new TestCati2(true, false, _backendTools);

            var surveyId = test.CreateSurveyWithPerson(DialingMode.Predictive, user, password, AgentTaskChoiceMode.CampaignAssignment);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            // create 2 interviews with calls
            var badInterview = BackendTools.NewInterview(surveyId);
            badInterview.TelephoneNumber = badTelephoneNumber;
            BackendTools.CreateInterview(badInterview);
            var badCall = BackendTools.NewCall(badInterview);
            BackendTools.CreateCall(badCall);
            var goodInterview = BackendTools.NewInterview(surveyId);
            goodInterview.TelephoneNumber = goodTelephoneNumber;
            BackendTools.CreateInterview(goodInterview);
            var goodCall = BackendTools.NewCall(goodInterview);
            BackendTools.CreateCall(goodCall);

            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(extensionNumber, false, null);

            var stubTelephony = new StubITelephony
            {
                SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32Boolean =
                    (id, requestId, campaignId, mode, list, timeout, recording) => DialerErrorCode.Success
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubTelephony);

            BackendTools.RunSchedulingProcedure();

            test.StartInterview_Predictive(2);

            Assert.AreEqual((int)CallOutcome.Blacklist, InterviewRepository.GetById(survey.SID, badInterview.ID).TransientState);
            Assert.IsFalse(BackendTools.IsCallExists(survey.SID, badInterview.ID));

            Assert.AreNotEqual((int)CallOutcome.Blacklist, InterviewRepository.GetById(survey.SID, goodInterview.ID).TransientState);
            Assert.IsTrue(BackendTools.IsCallExists(survey.SID, goodInterview.ID));
        }
    }
}
