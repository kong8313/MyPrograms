using System.Data.SqlTypes;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.MonitoringTest
{
    [TestClass]
    public class DeferredRecordCreationTest : BaseMockedIntegrationTest
    {
        private const string Testuser = "testUser";
        private const string Password = "password";
        private TestCati2 _test;
        private BvInterviewEntity _interview;
        private BvCallEntity _call;
        private IPersonDeferredMonitoringRepository _personDeferredMonitoringRepository;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            _test = new TestCati2(false, BackendToolsObject);

            _test.CreateSurveyWithPerson(
                DialingMode.Manual,
                Testuser,
                Password,
                AgentTaskChoiceMode.Automatic);

            _test.CreateInterviewsWithCalls(1);

            _test.Login(Testuser, Password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            _personDeferredMonitoringRepository = ServiceLocator.Resolve<IPersonDeferredMonitoringRepository>();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartInterview_SurveyHasNoVideoRecordingFlag_DeferredRecordIsAlwaysCreatedOnGetState()
        {
            StartInterview();

            var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);
            Assert.IsNotNull(deferredRecord);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartInterview_SurveyHasVideoRecordingFlag_DeferredRecordIsCreatedOnGetState()
        {
            SetRecordVideoFlag();
            StartInterview();

            var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);
            Assert.IsNotNull(deferredRecord);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartInterview_SurveyHasVideoRecordingFlag_DeferredRecordIsEmptyAndContainsProperData()
        {
            SetRecordVideoFlag();
            StartInterview();

            var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);
            Assert.AreEqual(_interview.ID, deferredRecord.InterviewID);
            Assert.AreEqual(_test.PersonSID, deferredRecord.PersonSID);
            Assert.AreEqual(_call.CallID, deferredRecord.CallID);
            Assert.AreEqual(SqlDateTime.MinValue.Value, deferredRecord.ClientTimeUtc);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartInterview_RepeatedGetStateWithoutLogout_ExistingDeferredRecordIsReturned()
        {
            SetRecordVideoFlag();
            StartInterview();

            var oldDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            // simulate repeated log in without logging out
            _test.StateWS.GetState();

            var newDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            Assert.AreEqual(oldDeferredRecord.ID, newDeferredRecord.ID, "Existing record should be returned");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void StartInterview_RepeatedGetStateWithoutLogout_CallIdIsNotChanged()
        {
            SetRecordVideoFlag();
            StartInterview();

            var oldDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            // simulate repeated log in without logging out
            _test.StateWS.GetState();

            var newDeferredRecord = BvPersonDeferredMonitoringPartAdapterEx.GetById(oldDeferredRecord.ID);

            Assert.IsNotNull(oldDeferredRecord);
            Assert.AreEqual(oldDeferredRecord.CallID, newDeferredRecord.CallID, "Call id shoudln't be changed");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Login_RepeatedLoginWithIncompleteInterview_CallIdIsCleanedForOldDeferredRecord()
        {
            SetRecordVideoFlag();
            StartInterview();

            // getting initial deferred record
            var oldDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);
            Assert.AreNotEqual(oldDeferredRecord, "Initial deferred record should be created");

            DoRepeatedLogin();

            oldDeferredRecord = BvPersonDeferredMonitoringPartAdapterEx.GetById(oldDeferredRecord.ID);
            Assert.IsNull(oldDeferredRecord.CallID);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Login_RepeatedLoginWithIncompleteInterview_NewDeferredRecordIsCreatedAfterRelogin()
        {
            SetRecordVideoFlag();
            StartInterview();

            var oldDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            DoRepeatedLogin();

            var newState = _test.StateWS.GetState();
            var newDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            Assert.AreNotEqual(oldDeferredRecord.ID, newState.deferredRecordId, "New deferred record should be created");
            Assert.AreEqual(newDeferredRecord.ID, newState.deferredRecordId, "Deferred record should be the same");
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void OldDeferredRecordExist_SameCallDeliveredToNewInterviewer_NewDeferredRecordIsCreatedOnGetState()
        {
            // Enable monitoring
            SetRecordVideoFlag();
            StartInterview();

            // Emulate call delivered to dialer in the predictive
            var call = CallQueueService.GetCallInfo(_call.CallID);
            call.CallState = -2;
            CallQueueService.UpdateCall(call, 0);

            var oldDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            // Emulate user is logged out by the dialer and so we have call hanged in the -2 state
            _test.DialerHelper.SendEventNotifyAgentState(0, oldDeferredRecord.PersonSID, "4");
            _test.WS.ConfirmLogout();

            // Disabling calls actually works for calls sent to the dialer, calls with stater -2
            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters
            {
                SurveyId = _test.SurveySID,
                BatchParameters = new SelectedBatchParameters(new[] { _call.CallID }),
                EnablingState = false,
                IsFcdOperation = false
            };

            ExecuteAsyncOperation(parameters);

            // Once call is disabled we can enable it now and it will be possible to deliver it to another interviewer
            parameters.EnablingState = true;
            ExecuteAsyncOperation(parameters);

            _test.CreatePerson(
                "User2",
                "User2",
                AgentTaskChoiceMode.Automatic);

            BackendTools.AssignCatiPersonToSurvey(_test.SurveySID, _test.PersonSID);
            _test.Login("User2", "User2", AgentTaskChoiceMode.Automatic, false);

            StartInterview();

            var newState = _test.StateWS.GetState();
            var newDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(_call.CallID);

            Assert.AreNotEqual(oldDeferredRecord.ID, newState.deferredRecordId, "New deferred record should be created");
            Assert.AreNotEqual(oldDeferredRecord.PersonSID, newDeferredRecord.PersonSID, "New deferred record should be created for new person");
            Assert.AreEqual(newDeferredRecord.ID, newState.deferredRecordId, "Deferred record should be the same");
        }

        private void ExecuteAsyncOperation(IAsyncOperationParameters parameters)
        {
            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                "");

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            executor.ExecuteOperationSync(entity);
        }

        private void SetRecordVideoFlag()
        {
            var survey = SurveyRepository.GetById(_test.SurveySID);
            survey.InterviewScreenRecording = true;
            SurveyRepository.Update(survey);
        }

        private void StartInterview()
        {
            _interview = _test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(_interview);

            _call = CallQueueService.GetCallAndNoLock(_interview.SurveySID, _interview.ID);
            Assert.IsNotNull(_call);
        }

        private void DoRepeatedLogin()
        {
            PersonInfo personInfo;
            DiallerInfo dialerInfo;
            CatiConsolePropertiesContainer properties;

            var consoleServiceHelper = new CatiWsHelper(Testuser, Password);
            var consoleDescriptor = new ConsoleDescription();

            consoleServiceHelper.ConsoleService.Login(_test.StationId, consoleDescriptor, out personInfo, out dialerInfo, out properties);
        }
    }
}
