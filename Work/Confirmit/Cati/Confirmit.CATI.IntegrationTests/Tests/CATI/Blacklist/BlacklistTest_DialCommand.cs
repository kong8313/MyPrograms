using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistTestDialCommand
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

        

        [TestMethod, Owner(@"FIRM\SvetlanaT"), Bug(45713)]
        public void DialCommandInPreviewSurvey_NumberFromBlacklist_NoCheckForBlacklist()
        {
            const string badTelephoneNumber = "12345";
            const string goodTelephoneNumber = "67890";
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            ServiceLocator.Resolve<ITelephoneBlacklistRepository>().Insert(new BvTelephoneBlacklistEntity { TelephoneNumber = badTelephoneNumber });

            var test = new TestCati2(true, false, _backendTools);

            var surveyId = test.CreateSurveyWithPerson(DialingMode.Preview, user, password, AgentTaskChoiceMode.Automatic);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            var interview = BackendTools.NewInterview(surveyId);
            interview.TelephoneNumber = goodTelephoneNumber;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(extensionNumber);

            test.WS.StartInterview(survey.Name, 0);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(45713)]
        public void DialCommandInPreviewSurvey_NumberFromBlacklistPattern_NoCheckForBlacklist()
        {
            const string goodTelephoneNumber = "67890";
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            ServiceLocator.Resolve<ITelephoneBlacklistRepository>().Insert(new BvTelephoneBlacklistEntity { DisplayPattern = "123*" });

            var test = new TestCati2(true, false, _backendTools);

            var surveyId = test.CreateSurveyWithPerson(DialingMode.Preview, user, password, AgentTaskChoiceMode.Automatic);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            var interview = BackendTools.NewInterview(surveyId);
            interview.TelephoneNumber = goodTelephoneNumber;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(extensionNumber);

            test.WS.StartInterview(survey.Name, 0);
        }
    }
}
