using System.Collections.Generic;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Fakes;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.Fakes;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PersonTools = Confirmit.CATI.IntegrationTests.Framework.Tools.PersonTools;

namespace Confirmit.CATI.IntegrationTests.Tests.MultipleDialers
{
    [TestClass]
    public class DialersEnabledAndDisabledTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        

        private ISurveyStateService _surveyStateService;

        private IDialerStateTools _dialerStateTools;
        private IDialerAvailabilityManager _dialerAvailabilityManager;
        private IDialersRepository _dialersRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            
            Stubs.SetNewIAuthoringServiceStub(true);
            
            _backendTools = new BackendTools(_framework);

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _dialerAvailabilityManager = ServiceLocator.Resolve<IDialerAvailabilityManager>();
            _dialerStateTools = ServiceLocator.Resolve<IDialerStateTools>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void DisablingAndEnablingDialerCorrectlyUpdatesTheDialerState()
        {
            var context = new TestData
            {
                Dialers = new[] {new DialerData {Tag = "D1", ReplyType = ReplyType.Sync, Id = 1}},
            }.Create();
            
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);

            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
            
            _dialerAvailabilityManager.DisableDialer(1);

            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
            
            _dialerAvailabilityManager.EnableDialer(1);

            ServiceLocator.ResolveByName<IAsynchronousTrigger>("BvDialersTrigger").OnTableChanged(null);
            
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void OneDialerIsDisabled_CallEnableDialer_StartCampaignCalledOnTheDialerForAllOpenedSurveys()
        {
            var campaignIds = new List<long>();
            var stubIDialerApi = new StubIDialerAPI
            {
                LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString =
                    (id, campaignId, agentId, name, type, extension, userId, predictive, local, attributes) =>
                            (int)DialerErrorCode.Success,
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) =>
                    {
                        campaignIds.Add(campaignId);
                        return (int)DialerErrorCode.Success;
                    }
            };

            PrepareTest(stubIDialerApi);
            _framework.BackendInitialize(true, null, 3);

            const DialingMode dialingMode = DialingMode.Preview;
            var survey1Sid = _backendTools.CreateSurvey("p10000001");
            var campaign1Id = ProjectIdConverter.ProjectIdToCampaignId("p10000001");
            var survey2Sid = _backendTools.CreateSurvey("p10000002");
            var campaign2Id = ProjectIdConverter.ProjectIdToCampaignId("p10000002");
            var survey3Sid = _backendTools.CreateSurvey("p10000003");
            var campaign3Id = ProjectIdConverter.ProjectIdToCampaignId("p10000003");
            SurveyService.SetDialingMode(survey1Sid, dialingMode);
            SurveyService.SetDialingMode(survey2Sid, dialingMode);
            SurveyService.SetDialingMode(survey3Sid, dialingMode);

            _surveyStateService.Open(survey1Sid);
            _surveyStateService.Open(survey2Sid);

            const int dialerId = 3; // In fact it can be any correct dialerId.

            _dialerAvailabilityManager.DisableDialer(dialerId);

            FakeDialerNotification();

            try
            {
                _dialerAvailabilityManager.EnableDialer(dialerId);
                Assert.Fail("Exception expected");
            }
            catch
            {
                // Exception expected
            }

            Assert.IsTrue(campaignIds.Contains(campaign1Id));
            Assert.IsTrue(campaignIds.Contains(campaign2Id));
            Assert.IsFalse(campaignIds.Contains(campaign3Id));
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void OneDialerBecomesUnavailable_ResultDialerIsNotAvailableErrorCodeIsSetForAllLoggedInToTheDialerPersons()
        {
            var stubIDialerApi = new StubIDialerAPI
            {
                LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString =
                    (id, campaignId, agentId, name, type, extension, userId, predictive, local, attributes) =>
                            (int)DialerErrorCode.Success,
                IsPersonModeSupportedAgentTaskChoiceMode = mode => true
            };

            PrepareTest(stubIDialerApi);
            _framework.BackendInitialize(true, null, 3);

            PersonTools.CreatePerson("user1", "password1", AgentTaskChoiceMode.Automatic);
            PersonTools.CreatePerson("user2", "password2", AgentTaskChoiceMode.Automatic);
            PersonTools.CreatePerson("user3", "password3", AgentTaskChoiceMode.Automatic);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            bool isPredictive;

            var consoleDescriptor = new ConsoleDescription();

            // Person not logged in to dialer
            var catiWsHelper = new CatiWsHelper("user1", "password1");
            catiWsHelper.ConsoleService.Login("test001", consoleDescriptor, out personInfo, out diallerInfo, out outProperties);

            // Person logged in to the dialer which becomes unavailable
            catiWsHelper = new CatiWsHelper("user2", "password2");
            catiWsHelper.ConsoleService.Login("test100001", consoleDescriptor, out personInfo, out diallerInfo, out outProperties);
            catiWsHelper.ConsoleService.LoginToDialer(string.Empty, string.Empty, out isPredictive);

            // Person logged in to another dialer
            catiWsHelper = new CatiWsHelper("user3", "password3");
            catiWsHelper.ConsoleService.Login("test200001", consoleDescriptor, out personInfo, out diallerInfo, out outProperties);
            catiWsHelper.ConsoleService.LoginToDialer(string.Empty, string.Empty, out isPredictive);

            const int unavailableDialerId = 2; // In fact it can be any correct dialerId.
            _dialerAvailabilityManager.DisableDialer(unavailableDialerId);

            foreach (var task in BvTasksAdapter.GetAll())
            {
                if (task.DialerId == unavailableDialerId)
                {
                    Assert.AreEqual((int)DialerErrorCode.NotAvailable, task.ProblemId);
                }
                else
                {
                    Assert.AreEqual(0, task.ProblemId);
                }
            }
        }

        private void PrepareTest(IDialerAPI dialer)
        {
            Stubs.SetNewIAuthoringServiceStub(true);
            Stubs.SetNewIDialerApiStub(dialer);
            Stubs.ExtendExistingIMnTciToolsStub(new StubIDialerRecordingAPI());
        }

        private void FakeDialerNotification()
        {
            var stubIDialerStateTools = new StubIDialerStateTools
            {
                Inner = _dialerStateTools,
            };

            ServiceLocator.RegisterInstance<IDialerStateTools>(stubIDialerStateTools);
        }
    }
}

