using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using System.Linq;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Telephony.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.SurveyTest
{
    [TestClass]
    public class SurveyTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ISurveyService _surveyService;
        private ICallCenterRepository _callCenterRepository;
        private ICallCenterService _callCenterService;
        private ITelephony _telephony;
        private ISystemSettings _systemSettings;

        private string _projectId;
        private string _cfSurveyDbName;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveyService = ServiceLocator.Resolve<ISurveyService>();
            _callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            _callCenterService = ServiceLocator.Resolve<ICallCenterService>();
            _telephony = ServiceLocator.Resolve<ITelephony>();
            ServiceLocator.Resolve<IDialerAvailabilityManager>();
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();

            _projectId = BackendTools.GenerateSurveyName();
            _cfSurveyDbName = "survey_" + _projectId;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        private void TryToOpenSurveyFailStartCampaignAndCheckErrorMessage(int surveySid, string surveyName)
        {
            var expectedExceptionThrown = false;
            var errors = new List<DialerStartCampaignResult>();
            var result = new DialerStartCampaignResult()
            {
                ErrorCode = DialerErrorCode.NotAvailable,
                DialerId = 1,
                DialerName = "Dialer name"
            };
            errors.Add(result);
            var errorsDescription = string.Format(
                " [id: {0}, name: {1}]", result.DialerId, result.DialerName);
            var expectedErrorMessage =
                string.Format(
                    "Warning: Survey '{0}' unavailable on dialers: {1}",
                    surveyName,
                    errorsDescription);

            var stubTelephony = new StubITelephony
            {
                StartCampaignInt64StringDialingModeStringString = (id, name, mode, type, xml) => errors
            };
            Stubs.ExtendExistingITelephonyStub(_telephony, stubTelephony);

            try
            {
                var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
                surveyStateService.Open(surveySid);
            }
            catch (UserMessageException e)
            {
                Assert.IsTrue(
                    string.Equals(e.Message, expectedErrorMessage),
                    "invalid error message for StartCampaign fail");

                expectedExceptionThrown = true;
            }

            Assert.IsTrue(expectedExceptionThrown, "expected exception has not been thrown");
        }

        /// <summary>
        /// 1. add survey
        /// 2. try to open survey and StartCampaign returns error - check err message
        /// 3. check that survey is not opened
        /// </summary>
        [TestMethod]
        [Owner(@"FIRM\AlexeyN")]
        [Cr(37988)]
        public void SurveyTest_OpenSurvey_StartCampaignReturnsError_CheckErrorMessage()
        {
            const string projectId = "p0001234";
            var surveySid = _backendTools.CreateSurvey(projectId);
            _telephony.InitializeDialers();

            SurveyService.SetDialingMode(surveySid, DialingMode.Automatic);

            var stubIDialerCollection = new StubIDialerCollection
            {
                IsDialerInitializedInt32 = id => true
            };

            ServiceLocator.RegisterInstance<IDialerCollection>(stubIDialerCollection);

            var dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            dialerSettings.DialerType = "PROTS";

            TryToOpenSurveyFailStartCampaignAndCheckErrorMessage(surveySid, projectId);

            // make sure that survey was not opened
            var surveyEntity = SurveyRepository.GetById(surveySid);
            Assert.IsTrue((SurveyState)surveyEntity.State == SurveyState.Close, "It is expected that the survey is closed");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ShutdownSurvey_SeveralCallsWithInProgressAndSentToDialerState_CallStateMovedTo2()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true/*TODO:false*/, IsOpen = true,
                        Interviews = new []{
                            new InterviewData(){ Tag="S1.I1", Call = new CallData(){CallState=2}},
                            new InterviewData(){ Tag="S1.I2", Call = new CallData(){CallState=1}},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData(){CallState=-1}},
                            new InterviewData(){ Tag="S1.I4", Call = new CallData(){CallState=-2}},
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            ServiceLocator.Resolve<ISurveyStateService>().ShutdownSurvey(survey.Model.SID);

            context.GetCalls("S1.I1", "S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);

        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SurveyServiceTest_AddSurvey_SupervisorNameIsNotGiven_SurveyIsNotAssignedToAnyCallCenter()
        {
            var callCenter = new BvCallCenterEntity { Name = "MyCallCenterNameForTest", LocalTimezoneId = 1 };
            _callCenterRepository.Insert(callCenter);

            _surveyService.CreateSurvey(_projectId, "My survey for tests", _framework.GetCatiSqlServerConnectionString(_cfSurveyDbName), string.Empty, string.Empty);

            var surveyId = SurveyRepository.GetByName(_projectId).SID;
            var surveyAssignments = _callCenterService.GetAssignmentsBySurvey(surveyId);
            Assert.IsFalse(surveyAssignments.Any(), "There shouldn't be any assignment for survey");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SurveyTest_AddSurvey_SupervisorNameIsGiven_SurveyIsAssignedToSupersCallCenter()
        {
            var callCenter = new BvCallCenterEntity { Name = "MyCallCenterNameForTest", LocalTimezoneId = 1 };
            _callCenterRepository.Insert(callCenter);

            const string superName = "super";
            _callCenterService.AssignSupervisors(callCenter.ID, superName);

            _surveyService.CreateSurvey(_projectId, "My survey for tests", _framework.GetCatiSqlServerConnectionString(_cfSurveyDbName), superName, string.Empty);

            var surveyId = SurveyRepository.GetByName(_projectId).SID;
            var surveyAssignments = _callCenterService.GetAssignmentsBySurvey(surveyId).ToArray();
            Assert.AreEqual(1, surveyAssignments.Count(), "Single assignment should be created");
            Assert.AreEqual(callCenter.ID, surveyAssignments.ElementAt(0).CallCenterId, "Wrong call center");
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void SurveyTest_AddSurvey_DefaultCallDeliveryModeIsRandom_CallDeliveryModeForSurveyIsRandom()
        {
            const string superName = "super";

            _systemSettings.Surveys.DefaultCallDeliveryMode = (int)CallDeliveryMode.Random;

            _surveyService.CreateSurvey(_projectId, "My survey for tests", _framework.GetCatiSqlServerConnectionString(_cfSurveyDbName), superName, string.Empty);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.IsTrue(survey.IsRandomCallDeliveryEnabled);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void SurveyTest_AddSurvey_DefaultCallDeliveryModeWasNotChanged_CallDeliveryModeForSurveyIsOrderById()
        {
            const string superName = "super";

            _surveyService.CreateSurvey(_projectId, "My survey for tests", _framework.GetCatiSqlServerConnectionString(_cfSurveyDbName), superName, string.Empty);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.IsFalse(survey.IsRandomCallDeliveryEnabled);
        }
    }
}
