﻿using System.Linq;
using Confirmit.CATI.Common;
﻿using Confirmit.CATI.Common.ConsoleService.Abstract;
﻿using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Telephony.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    /// <summary>
    /// General CATI tests with no dialler
    /// </summary>
    [TestClass]
    public class GeneralNoDialler
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private ITelephony _telephony;
        private IInterviewRecordingManager _interviewRecordingManager;
        private IAsynchronousTrigger _bvDialersTrigger;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            _telephony = ServiceLocator.Resolve<ITelephony>();
            _interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            _bvDialersTrigger = ServiceLocator.ResolveByName<IAsynchronousTrigger>("BvDialersTrigger");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        /*
        /// <summary>
        /// The test checks that changing person membership while an interview is active 
        /// does not imply errors.
        /// The test is written along with CR 37343 fix:
        /// <see cref="http://fi-osl-tfs:8080/WorkItemTracking/WorkItem.aspx?artifactMoniker=37343"/>
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(37343), Ignore] // Ignored as we have problems with event log service.
        public void InterviewIsStartedInNonTelephonyCompany_SurveyAssignmentPersonMembershipIsChanged_ThereIsNoErrorInEventLog()
        {
            using ( TestCati test = new TestCati(false) )
            {
                string user = "testUser";
                string password = "password";

                DateTime timeJustBefore = DateTime.Now;

                test.CreateSurveyWithPerson(
                    DiallingMode.DIALLING_MODE_MANUAL,
                    user,
                    password,
                    AgentTaskChoiceMode.CampaignAssignment );
                test.CreateInterviewsWithCalls( 1 );
                int anotherGroupId = PersonTools.CreatePersonGroup( "Another group" );

                test.Login( user, password, AgentTaskChoiceMode.CampaignAssignment, false );

                var interview = test.StartInterview_ManualOrPreview( null, 0 );
                Assert.IsNotNull( interview, "Failed to start an interview." );

                //Change the person membership
                var log = new System.Diagnostics.EventLog();
                log.Log = "CATI Confirmit";

                PersonService.SetParentGroups(
                    test.PersonSID,
                    new int[] { anotherGroupId } );

                var entries = log.Entries;

                foreach ( EventLogEntry entry in log.Entries )
                {
                    string message = entry.Message;
                    Assert.IsFalse(
                        message.Contains(
                        "Failed to process update membership in MN dialer." ) &&
                        entry.TimeGenerated.CompareTo( timeJustBefore ) >= 0,
                        "Attempt to update groups in MN dialler while there is no dialler in the system." );
                }

            }
        }
        */
        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(41212)]
        public void StartInterviewInSurveyAssignmentMode_NoCalls_GetStateReturnsCorrectSurveyId()
        {
            var test = new TestCati2(false, _backendTools);
            const string user = "testUser";
            const string password = "password";

            int surveySid = test.CreateSurveyWithPerson(
                DialingMode.Manual,
                user,
                password,
                AgentTaskChoiceMode.CampaignAssignment);

            BvSurveyEntity survey = SurveyRepository.GetById(surveySid);

            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, false);

            test.WS.StartInterview(survey.Name, 0);
            State state = test.WaitState(x => x.interviewState == (int)InterviewState.NO_CALLS);

            Assert.AreEqual(survey.Name, state.surveyId);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(47224)]
        public void CompanyHasNoDialer_OpenendReviewIsOn_HangupIsNotCalledAtInterviewFinish()
        {
            var isHangUpCalled = false;

            var stubITelephony = new StubITelephony
            {
                Inner = _telephony,
                HangupInt32Int64StringInt32Int64 = (id, campaignId, agentId, contactId, callId) =>
                {
                    isHangUpCalled = true;
                    return DialerErrorCode.Success;
                }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubITelephony);

            var test = new TestCati2(false, _backendTools);
            const string user = "testUser";
            const string password = "password";

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                user,
                password,
                AgentTaskChoiceMode.Automatic);

            BvSurveyEntity survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.CreateInterviewsWithCalls(1);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, false);
            test.StartInterview_ManualOrPreview(null, 0);

            test.WS.GetForceOpenendReview(1);

            Assert.IsFalse(isHangUpCalled, "Hangup should not be called");
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(47278)]
        public void CompanyHasNoDialer_AreRecordsExistsCalledInRecordingManager_AreRecordsExistsIsNotCalledInDialerProviderAndListOfFalseReturned()
        {
            new TestCati2(false, _backendTools);

            var interviewIds = new[] { 1, 2, 3 };

            var areRecordsExistsCalled = false;
            var stubITelephony = new StubITelephony
            {
                Inner = _telephony,
                AreRecordsExistsInt32ArrayOfInt32 = (id, ids) =>
                {
                    areRecordsExistsCalled = true;
                    return _telephony.AreRecordsExists(id, ids);
                }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubITelephony);

            var recordsExistanceArray = _interviewRecordingManager.AreRecordsExists(1, interviewIds);

            Assert.IsFalse(areRecordsExistsCalled);

            foreach (var b in recordsExistanceArray)
            {
                Assert.AreEqual(false, b);
            }
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(47278)]
        public void CompanyHasNoDialer_GetAudioRecordsCalledInRecordingManager_GetAudioRecordsIsNotCalledInDialerProviderAndEmptyListOfInterviewsIsReturned()
        {
            new TestCati2(false, _backendTools);

            var getAudioRecordsCalled = false;
            var stubITelephony = new StubITelephony
            {
                Inner = _telephony,
                GetAudioRecordsInt32Int32 = (id, interviewId) =>
                {
                    getAudioRecordsCalled = true;
                    return _telephony.GetAudioRecords(id, interviewId);
                }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubITelephony);

            var audioRecordUrls = ServiceLocator.Resolve<IInterviewRecordingManager>().GetInterviewRecordings(1, 1);

            Assert.IsFalse(getAudioRecordsCalled);
            Assert.AreEqual(0, audioRecordUrls.Count());
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(47548)]
        public void CompanyHasNoDialer_BvDialersSqlDependencyNotificationReceived_NoExceptionThrown()
        {
            new TestCati2(false, _backendTools);

            Stubs.SetNewIAuthoringServiceStub(false);

            _bvDialersTrigger.OnTableChanged(null);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43797)]
        public void CompanyHasDialerButDialerProviderIsNotInitialized_BvDialersSqlDependencyNotificationReceived_UpdateDialersCollectionIsCalled()
        {
            var updateDialersCollectionCalled = false;
            var stubITelephony = new StubITelephony
            {
                UpdateDialersCollection = () =>
                {
                    updateDialersCollectionCalled = true;
                    _telephony.UpdateDialersCollection();
                }
            };

            Stubs.ExtendExistingITelephonyStub(stubITelephony);

            new TestCati2(false, _backendTools);

            Stubs.SetNewIAuthoringServiceStub(false);

            var bvDialersTrigger = ServiceLocator.ResolveByName<IAsynchronousTrigger>("BvDialersTrigger");
            bvDialersTrigger.OnTableChanged(null);

            Assert.IsTrue(updateDialersCollectionCalled);
        }
    }
}