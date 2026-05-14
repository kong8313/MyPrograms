using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
#pragma warning disable 168

    [TestClass]
    public class CallActivationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private FusionLibTestTools _fusionLibTools;
        private IPersonRepository _personRepository;
        private ICallQueueService _callQueueService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _fusionLibTools = new FusionLibTestTools(_backendTools);

            _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
            _callQueueService = ServiceLocator.Resolve<ICallQueueService>();
            _timezoneId = _timezoneService.GetDefaultCallCenterTimezoneId();
            _now = TimezoneManager.GetCurrentTimeByTzId(_timezoneId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        const int Priority = 5;
        const int NewPriority = Priority * 3;
        private const int NewShiftTypeID = (int) ShiftTypeIDs.Sunday;
        private DateTime _now;
        private int _timezoneId;
        private ITimezoneService _timezoneService;

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_Scheduled_CallsActivated()
        {

            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new [] { calls[1].InterviewID },
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Scheduled,
                false);

            BackendTools.LoginPerson(personSid, "");

            calls[1].Resource = personSid;
            calls[1].Priority = NewPriority;
            calls[1].TimeInShift = TimezoneManager.ConvertToUTC(1, _now);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(calls[0].SurveySID, calls[0].InterviewID));
            TestAssert.AreEqual(calls[1], CallQueueService.GetCallAndNoLock(calls[1].SurveySID, calls[1].InterviewID));

            Assert.AreEqual(calls[1].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(calls[0].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_ScheduledAndCustomFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();
            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.BiggerEqual, "2");

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                filterSid,
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Scheduled,
                false);

            BackendTools.LoginPerson(personSid, "");

            calls[1].Resource = personSid;
            calls[1].Priority = NewPriority;
            calls[1].TimeInShift = TimezoneManager.ConvertToUTC(1, _now);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(calls[0].SurveySID, calls[0].InterviewID));
            TestAssert.AreEqual(calls[1], CallQueueService.GetCallAndNoLock(calls[1].SurveySID, calls[1].InterviewID));

            Assert.AreEqual(calls[1].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(calls[0].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_ScheduledAndDefaultFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled cals
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Scheduled,
                false);

            BackendTools.LoginPerson(personSid, "");

            calls[0].Resource = calls[1].Resource = personSid;
            calls[0].Priority = calls[1].Priority = NewPriority;
            calls[0].TimeInShift = calls[1].TimeInShift = TimezoneManager.ConvertToUTC(1, _now);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(calls[0].SurveySID, calls[0].InterviewID));
            TestAssert.AreEqual(calls[1], CallQueueService.GetCallAndNoLock(calls[1].SurveySID, calls[1].InterviewID));

            Assert.AreEqual(calls[0].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(calls[1].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_ScheduledWithNotPossitivePhase_CallShouldNotBeActiavted()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            //update phase to -1
            bool isCallGet;
            _callQueueService.GetCallWithTryLock(surveySid, interviews[0].ID, out isCallGet);
            calls[0].CallState = -1;
            //update phase to 0
            BvSpSvySch_DeleteAdapter.ExecuteNonQuery(surveySid, interviews[1].ID);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled calls
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Scheduled,
                false);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(calls[0].SurveySID, calls[0].InterviewID));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(calls[1].SurveySID, calls[1].InterviewID));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_Suspended_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new [] { 2 },
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Suspended,
                false);

            var expectedCall = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 2,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            };

            BvCallEntity actualCall = CallQueueService.GetCallAndNoLock(surveySid, 2);

            TestAssert.AreEqual(expectedCall, actualCall);
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 1));

            BackendTools.LoginPerson(personSid, "");

            Assert.AreEqual(actualCall.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.IsNull(TaskService.LookupByPersonSid(personSid, surveySid),
                "Person should get only one call no more");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_SuspendedAndCustomFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            int filterSid = FusionLibTestTools.CreateFilterForTest("TransientState"/*ITS = ID*/, FilterOperator.Equal, "2");

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                filterSid,
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            var expectedCall = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 1,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            };

            BvCallEntity actualCall = CallQueueService.GetCallAndNoLock(surveySid, 1);

            TestAssert.AreEqual(expectedCall, actualCall);
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 2));

            BackendTools.LoginPerson(personSid, "");

            Assert.AreEqual(actualCall.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.IsNull(TaskService.LookupByPersonSid(personSid, surveySid),
                "Person should get only one call no more");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_SuspendedAndDefaultFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0,
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            var expectedCall1 = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 1,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            };

            var expectedCall2 = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 2,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            };

            BvCallEntity actualCall1 = CallQueueService.GetCallAndNoLock(surveySid, 1);
            BvCallEntity actualCall2 = CallQueueService.GetCallAndNoLock(surveySid, 2);

            TestAssert.AreEqual(expectedCall1, actualCall1);
            TestAssert.AreEqual(expectedCall2, actualCall2);

            BackendTools.LoginPerson(personSid, "");

            Assert.AreEqual(actualCall1.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(actualCall2.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_SuspendedNotPossitivePhase_CallShouldNotBeActiavted()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            //update phase to -1
            bool isCallGet;
            _callQueueService.GetCallWithTryLock(surveySid, interviews[0].ID, out isCallGet);
            calls[0].CallState = -1;
            //update phase to 0
            BvSpSvySch_DeleteAdapter.ExecuteNonQuery(surveySid, interviews[1].ID);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new [] { interviews[0].ID, interviews[1].ID },
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Suspended, 
                false);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(calls[0].SurveySID, calls[0].InterviewID));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(calls[1].SurveySID, calls[1].InterviewID));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_ALL_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new List<BvInterviewEntity> { interviews[0] }).ToList();

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new [] { 1, 2 },
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Suspended,
                false);

            calls.Add(new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 2,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            });

            BvCallEntity actualCreatedCall = CallQueueService.GetCallAndNoLock(surveySid, 2);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(surveySid, 1));
            TestAssert.AreEqual(calls[1], actualCreatedCall);

            BackendTools.LoginPerson(personSid, "");

            Assert.AreEqual(actualCreatedCall.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(calls[0].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_ALLAndCustomFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2, 3 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new List<BvInterviewEntity> { interviews[0] }).ToList();
            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.NotEqual, "3");

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                filterSid,
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            calls.Add(new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 2,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            });

            BvCallEntity actualCreatedCall = CallQueueService.GetCallAndNoLock(surveySid, 2);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(surveySid, 1));
            TestAssert.AreEqual(calls[1], actualCreatedCall);
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 3));

            BackendTools.LoginPerson(personSid, "");

            Assert.AreEqual(actualCreatedCall.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(calls[0].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.IsNull(TaskService.LookupByPersonSid(personSid, surveySid),
                "Person should get only one call no more");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_ALLAndDefaultFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new List<BvInterviewEntity> { interviews[1] }).ToList();

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0, //filterSid
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            calls.Add(new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                InterviewID = 1,
                CallState = 2,
                ShiftID = (int)CallShiftType.None
            });

            BvCallEntity actualCreatedCall = CallQueueService.GetCallAndNoLock(surveySid, 1);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(surveySid, 2));
            TestAssert.AreEqual(calls[1], actualCreatedCall);

            BackendTools.LoginPerson(personSid, "");

            Assert.AreEqual(actualCreatedCall.CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
            Assert.AreEqual(calls[0].CallID,
                TaskService.LookupByPersonSid(personSid, surveySid).CallID,
                "Incorrect call was returned for person");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_AllWithNotPossitivePhase_CallShouldNotBeActiavted()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            //update phase to -1
            bool isCallGet;
            _callQueueService.GetCallWithTryLock(surveySid, interviews[0].ID, out isCallGet);
            calls[0].CallState = -1;
            //update phase to 0
            BvSpSvySch_DeleteAdapter.ExecuteNonQuery(surveySid, interviews[1].ID);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new [] { interviews[0].ID, interviews[1].ID },
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.All, 
                false);

            TestAssert.AreEqual(calls[0], CallQueueService.GetCallAndNoLock(calls[0].SurveySID, calls[0].InterviewID));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(calls[1].SurveySID, calls[1].InterviewID));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_ShiftTypeChangedAndScheduled_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.ScriptForShiftType, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            int dbShiftTypeID = SurveyManager.GetShiftTypes(surveySid).Find(x => x.Id == NewShiftTypeID).ObjectId;

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new[] { calls[1].InterviewID },
                Priority,
                personSid,
                dbShiftTypeID,
                new DateTime(2009, 6, 21, 10, 0, 0),
                CallStates.Scheduled, false);

            BvCallEntity activatedCall = CallQueueService.GetCallAndNoLock(surveySid, interviews[1].ID);
            BvCallEntity notActivatedCall = CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID);
            int callShiftTypeID = BackendTools.GetShiftTypeWorkID(NewShiftTypeID);

            Assert.AreEqual(callShiftTypeID, activatedCall.ShiftID, "Shift type is changed for call which shouldont be activated");
            Assert.AreEqual((int)CallShiftType.None, notActivatedCall.ShiftID, "Shift type is changed for call which shouldont be activated");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_ShiftTypeChangedAndScheduledAndCustomFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.ScriptForShiftType, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);
            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.Equal, "1");

            int dbShiftTypeID =
                (int)SurveyManager.GetShiftTypes(surveySid).Find(x => x.Id == NewShiftTypeID).ObjectId;

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                filterSid,
                Priority,
                personSid,
                dbShiftTypeID,
                _timezoneId,
                new DateTime(2009, 6, 21, 10, 0, 0),
                CallStates.Scheduled,
                false);

            BvCallEntity activatedCall = CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID);
            BvCallEntity notActivatedCall = CallQueueService.GetCallAndNoLock(surveySid, interviews[1].ID);
            int callShiftTypeID = BackendTools.GetShiftTypeWorkID(NewShiftTypeID);

            Assert.AreEqual(callShiftTypeID, activatedCall.ShiftID, "Shift type of activated call is incorrect");
            Assert.AreEqual((int)CallShiftType.None, notActivatedCall.ShiftID, "Shift type is changed for call which shouldont be activated");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_ShiftTypeChangedAndScheduledAndDefaultFiltered_CallsActivated()
        {
            int surveySid;
            int personSid;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.ScriptForShiftType, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            int dbShiftTypeID =
                (int)SurveyManager.GetShiftTypes(surveySid).Find(x => x.Id == NewShiftTypeID).ObjectId;

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0, //filterSid
                Priority,
                personSid,
                dbShiftTypeID,
                _timezoneId,
                new DateTime(2009, 6, 21, 10, 0, 0),
                CallStates.Scheduled,
                false);

            BvCallEntity activatedCall1 = CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID);
            BvCallEntity activatedCall2 = CallQueueService.GetCallAndNoLock(surveySid, interviews[1].ID);
            int callShiftTypeID = BackendTools.GetShiftTypeWorkID(NewShiftTypeID);

            Assert.AreEqual(callShiftTypeID, activatedCall1.ShiftID, "Shift type of activated call is incorrect");
            Assert.AreEqual(callShiftTypeID, activatedCall2.ShiftID, "Shift type of activated call is incorrect");
        }

        private class TestResult1
        {
            public int SurveyId { get; set; }
            public int InterviewId { get; set; }
            public int ShiftTypeId { get; set; }
            public AsyncOperationState Result { get; set; }
        };

        private TestResult1 Test_ActivateCallsWhenSiteTimeZoneDifferFrom1(DateTime timeToCall)
        {
            const int siteTimeZone = 3;
            const int respondentTimeZone = 1;
            const int shiftTypeID = 1;

            var script = new TestScript(
                new Action(Action.Operation.SetNewITS, "17"),
                new Shift(1, shiftTypeID, "0.08:00:00", "0.20:00:00"));

            int surveySID = _backendTools.CreateSurvey(script);

            //activate all timezones
            TimezoneManager.AddTimezone(siteTimeZone);

            var interview = new BvInterviewEntity
            {
                ID = 1,
                SurveySID = surveySID,
                TransientState = 16,
                TimezoneID = respondentTimeZone
            };
            BackendTools.CreateInterview(interview);

            var call = new BvCallEntity { InterviewID = interview.ID, SurveySID = surveySID, Priority = 1 };
            BackendTools.CreateCall(call);

            int dbShiftTypeID = SurveyManager.GetShiftTypes(surveySID).Find(x => x.Id == shiftTypeID).ObjectId;
            var person = new BvPersonEntity
            {
                Name = "interviewer1",
                Description = "interviewer1 description",
                CallCenterID = CallCenterTools.DefaultId
            };
            int personSid = _personRepository.Insert(person);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySID,
                new[] { call.InterviewID }, 
                3,
                personSid, 
                dbShiftTypeID, 
                timeToCall,
                CallStates.Scheduled,
                false);

            return new TestResult1
                       {
                           Result = operationResult.State,
                           SurveyId = surveySID,
                           InterviewId = interview.ID,
                           ShiftTypeId = shiftTypeID
                       };
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_TimeToCallIsGreaterThenLeftBoundOfShiftLessThen1Hour_CallsActivated()
        {
            DateTime timeToCall = DateTime.Parse("2009-02-15T08:15:00");
            var result = Test_ActivateCallsWhenSiteTimeZoneDifferFrom1(timeToCall);

            var call = CallQueueService.GetCallAndNoLock(result.SurveyId, result.InterviewId);
            int callShiftTypeID = BackendTools.GetShiftTypeWorkID(result.ShiftTypeId);

            Assert.AreEqual(call.ShiftID, callShiftTypeID);
            Assert.AreEqual(call.TimeInShift, timeToCall);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_TimeToCallIsGreaterThenRightBoundOfShiftLessThen1Hour_ComExceptionIsThrown()
        {
            DateTime timeToCall = DateTime.Parse("2009-02-15T20:15:00");
            var result = Test_ActivateCallsWhenSiteTimeZoneDifferFrom1(timeToCall);

            Assert.AreEqual(AsyncOperationState.Failed, result.Result);
        }

        private class TestResult2
        {
            public int SurveyId { get; set; }
            public int Interview1Id { get; set; }
            public int Interview2Id { get; set; }
            public int RespondentTimezone1Id { get; set; }
            public int RespondentTimezone2Id { get; set; }
            public int ShiftTypeId { get; set; }
            public AsyncOperationState Result { get; set; }
        }

        private TestResult2 Test_ActivateCallsWithDifferentTZWhenSiteTimeZoneDifferFrom1(DateTime timeToCall)
        {
            const int siteTimeZone = 3; //GMT+1
            const int respondentTimeZone1 = 15; //GMT+3
            const int respondentTimeZone2 = 22; //GMT+5
            const int shiftTypeID = 1;

            var script = new TestScript(
                new Action(Action.Operation.SetNewITS, "17"),
                new Shift(1, shiftTypeID, "0.08:00:00", "0.20:00:00"));

            int surveySID = _backendTools.CreateSurvey(script);

            //activate all timezones
            TimezoneManager.AddTimezone(siteTimeZone);
            TimezoneManager.AddTimezone(respondentTimeZone1);
            TimezoneManager.AddTimezone(respondentTimeZone2);

            var interview1 = new BvInterviewEntity
            {
                ID = 1,
                SurveySID = surveySID,
                TransientState = 16,
                TimezoneID = respondentTimeZone1
            };
            BackendTools.CreateInterview(interview1);

            var interview2 = new BvInterviewEntity
            {
                ID = 2,
                SurveySID = surveySID,
                TransientState = 16,
                TimezoneID = respondentTimeZone2
            };
            BackendTools.CreateInterview(interview2);

            var call1 = new BvCallEntity { InterviewID = interview1.ID, SurveySID = surveySID, Priority = 1 };
            BackendTools.CreateCall(call1);

            var call2 = new BvCallEntity { InterviewID = interview2.ID, SurveySID = surveySID, Priority = 1 };
            BackendTools.CreateCall(call2);

            int dbShiftTypeID = SurveyManager.GetShiftTypes(surveySID).Find(x => x.Id == shiftTypeID).ObjectId;
            var person = new BvPersonEntity
            {
                Name = "interviewer1",
                Description = "interviewer1 description",
                CallCenterID = CallCenterTools.DefaultId
            };
            int personSid = _personRepository.Insert(person);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySID,
                new[] {call1.InterviewID, call2.InterviewID},
                3,
                personSid,
                dbShiftTypeID,
                timeToCall,
                CallStates.Scheduled,
                false);

            return new TestResult2
                       {
                           SurveyId = surveySID,
                           Interview1Id = interview1.ID,
                           Interview2Id = interview2.ID,
                           RespondentTimezone1Id = respondentTimeZone1,
                           RespondentTimezone2Id = respondentTimeZone2,
                           ShiftTypeId = shiftTypeID,
                           Result = operationResult.State
                       };
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_CallsWithDifferentTZAndSiteTZdifferFromGMT1TimeInShift_CallsActivated()
        {
            DateTime timeToCall = DateTime.Parse("2009-02-15T08:15:00");
            var result = Test_ActivateCallsWithDifferentTZWhenSiteTimeZoneDifferFrom1(timeToCall);

            var call1 = CallQueueService.GetCallAndNoLock(result.SurveyId, result.Interview1Id);
            var call2 = CallQueueService.GetCallAndNoLock(result.SurveyId, result.Interview2Id);

            DateTime timeToCall1 =
                TimezoneManager.ConvertToUTC(result.RespondentTimezone1Id, timeToCall);

            int callShiftTypeID = BackendTools.GetShiftTypeWorkID(result.ShiftTypeId);

            Assert.AreEqual(call1.ShiftID, callShiftTypeID);
            Assert.AreEqual(call1.TimeInShift, timeToCall1);

            DateTime timeToCall2 =
                TimezoneManager.ConvertToUTC(result.RespondentTimezone2Id, timeToCall);

            Assert.AreEqual(call2.ShiftID, callShiftTypeID);
            Assert.AreEqual(call2.TimeInShift, timeToCall2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_CallsWithDifferentTZAndSiteTZdifferFromGMT1TimeNotInShift_ComExceptionIsThrown()
        {
            DateTime timeToCall = DateTime.Parse("2009-02-15T20:15:00");
            var result = Test_ActivateCallsWithDifferentTZWhenSiteTimeZoneDifferFrom1(timeToCall);

            Assert.AreEqual(AsyncOperationState.Failed, result.Result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_SuspendedInClosedCell_CallShouldNotBeActivated()
        {
            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out var surveySid, out var personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2, 3, 4 }).ToList();

            //there are should be 2 cells
            var quota = TestQuota.Create(_framework.DbEngine,
                surveySid,
                1,
                new[] { "q1" },//quota column
                new[] { 2 }); //count of different answers

            const int cellId1 = 1;
            const int cellId2 = 2;

            quota.PutInterviewsInCells(
                new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID, interviews[3].ID },
                new[] { cellId1, cellId2, cellId1, cellId2 });

            BackendTools.SyncResponseControl(_framework.DbEngine, surveySid);

            quota.CloseCell(cellId1);

            //Sync data
            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled calls
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            var expectedCall = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                CallState = 2,
                ShiftID = (int) CallShiftType.None,
                InterviewID = interviews[1].ID
            };

            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[1].ID));
            expectedCall.InterviewID = interviews[3].ID;
            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[3].ID));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, interviews[2].ID));

            BackendTools.AssertAggregateData(surveySid, interviews.Count, 2/*scheduled calls*/);
            BackendTools.CheckResponseControl(_framework.DbEngine, surveySid);
        }


        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_AllInClosedCell_CallShouldNotBeActivated()
        {
            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out var surveySid, out var personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2, 3, 4, 5, 6 }).ToList();
            FusionLibTestTools.CreateCallsForTest(new List<BvInterviewEntity> { interviews[4], interviews[5] });

            //there are should be 2*3 = 6 cells
            var quota = TestQuota.Create(_framework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },//quota column
                new[] { 2, 3 }); //count of different answers

            const int cellId1 = 1;
            int cellId2 = Randomizer.Next(2, 6);

            quota.PutInterviewsInCells(
                new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID, interviews[3].ID, interviews[4].ID, interviews[5].ID },
                new[] { cellId2, cellId2, cellId1, cellId1, cellId2, cellId2 });

            quota.CloseCell(cellId1);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled calls
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.All,
                false);

            var expectedCall = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                CallState = 2,
                ShiftID = (int) CallShiftType.None,
                InterviewID = interviews[0].ID
            };

            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID));
            expectedCall.InterviewID = interviews[1].ID;
            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[1].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[2].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[3].ID));
            expectedCall.InterviewID = interviews[4].ID;
            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[4].ID));
            expectedCall.InterviewID = interviews[5].ID;
            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[5].ID));

            BackendTools.AssertAggregateData(surveySid, interviews.Count, 4/*scheduled calls*/);
        }


        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_AllInOpenedCell_CallShouldBeActivated()
        {
            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out var surveySid, out var personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2 }).ToList();

            //there are should be 2*3*2 = 12 cells
            var quota = TestQuota.Create(_framework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2", "q3" },//quota column
                new[] { 2, 3, 2 }); //count of different answers

            const int cellId1 = 1;
            const int cellId2 = 10;

            quota.PutInterviewsInCells(
                new[] { interviews[0].ID, interviews[1].ID },
                new[] { cellId1, cellId1 });

            quota.CloseCell(cellId2);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled calls
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.All,
                false);

            var expectedCall = new BvCallEntity
            {
                Resource = personSid,
                Priority = NewPriority,
                TimeInShift = TimezoneManager.ConvertToUTC(1, _now),
                SurveySID = surveySid,
                CallState = 2,
                ShiftID = (int)CallShiftType.None,//None
                InterviewID = interviews[0].ID

            };

            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID));
            expectedCall.InterviewID = interviews[1].ID;
            TestAssert.AreEqual(expectedCall, CallQueueService.GetCallAndNoLock(surveySid, interviews[1].ID));

            BackendTools.AssertAggregateData(surveySid, interviews.Count, 2/*scheduled calls*/);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_ManyQuotasHasManyLongColumns_AllDynamicQueryShouldBePerformedCorrectly()
        {
            string projectId;

            var columnsForQuota1 = new[]
            {
                "A1234567890123456789012345678901A",
                "A1234567890123456789012345678901B",
                "A1234567890123456789012345678901C",
                "A1234567890123456789012345678901D",
                "A1234567890123456789012345678901E"  
            };
            var columnsForQuota2 = new[]
            {
                "A234567890123456789012345678901F",
                "A234567890123456789012345678901G",
                "A234567890123456789012345678901H",
                "A234567890123456789012345678901J",
                "A234567890123456789012345678901K"
            };
            var columnsForQuota3 = new[]
            {
                "A234567890123456789012345678901L",
                "A234567890123456789012345678901M",
                "A234567890123456789012345678901N",
                "A234567890123456789012345678901O",
                "A234567890123456789012345678901P"
            };
            var columnsForQuota4 = new[]
            {
                "A234567890123456789012345678901Q",
                "A234567890123456789012345678901R",
                "A234567890123456789012345678901S",
                "A234567890123456789012345678901T",
                "A234567890123456789012345678901U"
            };

            var answerCountsForQuota1 = new[] { 2, 2, 2, 3, 2 };
            var answerCountsForQuota2 = new[] { 2, 3, 4, 2, 2 };
            var answerCountsForQuota3 = new[] { 2, 2, 2, 2, 2 };
            var answerCountsForQuota4 = new[] { 2, 2, 3, 3, 2 };

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out var surveySid, out var personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1, 2, 3, 4, 5, 6, 7, 8 }).ToList();

            //there are should be 2*3*2 = 12 cells
            var quota1 = TestQuota.Create(_framework.DbEngine,
                surveySid,
                1,
                columnsForQuota1,
                answerCountsForQuota1);

            //there are should be 2*3*2 = 12 cells
            var quota2 = TestQuota.Create(_framework.DbEngine,
                surveySid,
                2,
                columnsForQuota2,
                answerCountsForQuota2);

            //there are should be 2*3*2 = 12 cells
            var quota3 = TestQuota.Create(_framework.DbEngine,
                surveySid,
                3,
                columnsForQuota3,
                answerCountsForQuota3);

            //there are should be 2*3*2 = 12 cells
            var quota4 = TestQuota.Create(_framework.DbEngine,
                surveySid,
                4,
                columnsForQuota4,
                answerCountsForQuota4);

            const int openCellIdForQuota1 = 2;
            const int closeCellIdForQuota1 = 4;
            const int openCellIdForQuota2 = 3;
            const int closeCellIdForQuota2 = 5;
            const int openCellIdForQuota3 = 1;
            const int closeCellIdForQuota3 = 6;
            const int openCellIdForQuota4 = 7;
            const int closeCellIdForQuota4 = 8;

            quota1.PutInterviewsInCells(
                new[] { interviews[0].ID, interviews[1].ID },
                new[] { openCellIdForQuota1, closeCellIdForQuota1 });

            quota2.PutInterviewsInCells(
                new[] { interviews[1].ID, interviews[2].ID, interviews[3].ID },
                new[] { openCellIdForQuota2, openCellIdForQuota2, closeCellIdForQuota2 });

            quota3.PutInterviewsInCells(
                new[] { interviews[1].ID, interviews[4].ID, interviews[5].ID, interviews[2].ID },
                new[] { closeCellIdForQuota3, openCellIdForQuota3, closeCellIdForQuota3, closeCellIdForQuota3 });

            quota4.PutInterviewsInCells(
                new[] { interviews[6].ID, interviews[7].ID },
                new[] { closeCellIdForQuota4, openCellIdForQuota4 });

            BackendTools.SyncResponseControl(_framework.DbEngine, surveySid);

            quota1.CloseCell(closeCellIdForQuota1);
            quota2.CloseCell(closeCellIdForQuota2);
            quota3.CloseCell(closeCellIdForQuota3);
            quota4.CloseCell(closeCellIdForQuota4);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled calls
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            Assert.IsTrue(BackendTools.IsCallExists(surveySid, interviews[0].ID),
                String.Format("Interview with id {0} was not activated", interviews[0].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[1].ID),
                String.Format("Call for Interview with id {0} should not be activated", interviews[1].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[2].ID),
                String.Format("Call for Interview with id {0} should not be activated", interviews[2].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[3].ID),
                String.Format("Call for Interview with id {0} should not be activated", interviews[3].ID));
            Assert.IsTrue(BackendTools.IsCallExists(surveySid, interviews[4].ID),
                String.Format("Interview with id {0} was not activated", interviews[4].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[5].ID),
                String.Format("Call for Interview with id {0} should not be activated", interviews[5].ID));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[6].ID),
                String.Format("Call for Interview with id {0} should not be activated", interviews[6].ID));
            Assert.IsTrue(BackendTools.IsCallExists(surveySid, interviews[7].ID),
                String.Format("Interview with id {0} was not activated", interviews[7].ID));

            BackendTools.AssertAggregateData(surveySid, interviews.Count, 3/*scheduled calls*/);
            BackendTools.CheckResponseControl(_framework.DbEngine, surveySid);
        }

        private void ActivateCall_TestBase(
            IEnumerable<int> activeTzList, 
            TestScript script, 
            int tzId, 
            int shiftTypeID, 
            DateTime timeToCall,
            Func<AsyncOperationResult, bool> resultChecker)
        {
            foreach (var id in activeTzList)
                TimezoneManager.AddTimezone(id);

            int surveySid = _backendTools.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            interview.TimezoneID = tzId;
            BackendTools.CreateInterview(interview);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                new[] {interview.ID},
                1,
                0,
                shiftTypeID,
                timeToCall,
                CallStates.All,
                false);

            if (resultChecker(operationResult) == false)
            {
                return;
            }

            var call = BackendTools.NewCall(interview);
            call.TimeInShift = TimezoneManager.ConvertToUTC(_timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(tzId), timeToCall);

            if (shiftTypeID == (int)CallShiftType.None)
            {
                call.ShiftID = (int)CallShiftType.None;
            }
            else if (shiftTypeID <= 0)
            {
                call.ShiftID = -tzId;
            }
            else
            {
                call.ShiftID = script.GetShiftTypeWorkID(shiftTypeID);
            }
            BackendTools.CheckCall(call);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateBeforeStartShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T07:59:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 0."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateOnStartShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T08:00:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateInShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T12:00:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateBeforeEndShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T19:59:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateOnEndShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T20:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 0."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateAfterEndShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T21:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 0."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithNone_TimeToActivateAfterEndShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                (int)CallShiftType.None,//shiftType to activate
                DateTime.Parse("2010-01-24T21:00:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallInDefTzWithSpecificShift_TimeToActivateInOtherShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-25T12:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 0."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIndefTzWithAnyValid_TimeToActivateOnShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                0,//Interview Tz
                (int)CallShiftType.AnyValid,//shiftType to activate
                DateTime.Parse("2010-01-25T12:00:00"),//time to call
                CheckSuccessfulResult);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateBeforeStartShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T09:59:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 6."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateOnStartShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T10:00:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateInShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T12:00:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateBeforeEndShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T17:59:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateOnEndShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T18:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 6."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateAfterEndShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T19:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 6."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithNone_TimeToActivateAfterEndShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                (int)CallShiftType.None,//shiftType to activate
                DateTime.Parse("2010-01-24T21:00:00"),//time to call
                CheckSuccessfulResult);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithSpecificShift_TimeToActivateInOtherShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-25T12:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 6."));

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn6TzWithAnyValid_TimeToActivateOnShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                6,//Interview Tz
                (int)CallShiftType.AnyValid,//shiftType to activate
                DateTime.Parse("2010-01-25T12:00:00"),//time to call
                CheckSuccessfulResult);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn12TzWithSpecificShift_TimeToActivateInShift_InterviewIsActivated()
        {
            ActivateCall_TestBase(
                new[] { 6, 12 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                12,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-24T12:00:00"),//time to call
                CheckSuccessfulResult);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCallIn12TzWithSpecificShift_TimeToActivateInOtherShift_TimeOutOfShifts()
        {
            ActivateCall_TestBase(
                new[] { 6, 12 },//Active Tzs
                new TestScript(new Action(Action.Operation.SetNewITS, "10"),
                               new Shift(1, 1, new ShiftTimezone(null, "0.08:00:00", "0.20:00:00"),
                                         new ShiftTimezone(6, "0.10:00:00", "0.18:00:00")),
                               new Shift(2, 2, new ShiftTimezone(null, "1.08:00:00", "1.20:00:00"),
                                         new ShiftTimezone(6, "1.10:00:00", "1.18:00:00"))),

                12,//Interview Tz
                1,//shiftType to activate
                DateTime.Parse("2010-01-25T12:00:00"),//time to call
                result => CheckFailedResult(result, "Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: 12."));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCall_Activate2batches_7CallShouldBeActiavtedIn2Batches()
        {
            int surveySid;
            int personSid;

            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.ActivatePortionSize= 4;

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, Enumerable.Range(1, 10).ToArray()).ToList();
            //calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            //activate only first 7 interview
            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                interviews.Take(7).Select(x => x.ID).ToArray(),
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Suspended,
                false);

            var activatedIds = BvSvyScheduleAdapter.GetAll().Select(x => x.ID).OrderBy(y => y).ToArray();

            Assert.AreEqual(7, activatedIds.Length);
            CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), activatedIds);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCall_Activate2batchesFirstBatchWithErrorOnFirstCall_7CallShouldBeActiavtedIn2Batches()
        {
            int surveySid;
            int personSid;

            var originalCallsManagementService = ServiceLocator.Resolve<ICallsManagementService>();

            var callNumber = 0;
            var stubICallsManagementService = new StubICallsManagementService
            {
                ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32
                    = (SurveySID, Mode, BatchID, Priority, PersonSID, ShiftTypeID, TimeToCall, EnableDisabledCalls, DefaultTZID, Its) =>
                    {
                        ++callNumber;

                        if (callNumber == 1)
                        {
                            throw new Exception("error");
                        }

                        originalCallsManagementService.Activate(
                            SurveySID,
                            Mode,
                            BatchID,
                            Priority,
                            PersonSID,
                            ShiftTypeID,
                            TimeToCall,
                            EnableDisabledCalls,
                            DefaultTZID,
                            null);
                    }
            };
            ServiceLocator.RegisterInstance<ICallsManagementService>(stubICallsManagementService);

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, Enumerable.Range(1, 10).ToArray()).ToList();

            //activate only first 7 interview
            var operationResult = new TestCallManagementOperationFactory(4).CreateActivateCallsSelected(
                surveySid,
                interviews.Take(7).Select(x => x.ID).ToArray(),
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Suspended,
                false);

            var activatedIds = BvSvyScheduleAdapter.GetAll().Select(x => x.ID).OrderBy(y => y).ToArray();

            Assert.AreEqual(7, activatedIds.Length);
            CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), activatedIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCall_Activate2batchesFirstBatchIsError_3CallShouldBeActiavtedIn2Batches()
        {
            int surveySid;
            int personSid;

            const int portionSize = 4;
            int retryCount = ServiceLocator.Resolve<IRetryingServiceSettings>().NumberOfRetryAttempts;

            var originalCallsManagementService = ServiceLocator.Resolve<ICallsManagementService>();

            var callNumber = 0;
            var stubICallsManagementService = new StubICallsManagementService
            {
                ActivateNullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfBooleanNullableOfInt32NullableOfInt32
                    = (SurveySID, Mode, BatchID, Priority, PersonSID, ShiftTypeID, TimeToCall, EnableDisabledCalls, DefaultTZID, ITS) =>
                    {
                        if (callNumber++ < retryCount)
                        {
                            throw new Exception("error");
                        }

                        originalCallsManagementService.Activate(
                            SurveySID, 
                            Mode, 
                            BatchID, 
                            Priority, 
                            PersonSID, 
                            ShiftTypeID, 
                            TimeToCall, 
                            EnableDisabledCalls, 
                            DefaultTZID,
                            null);
                    }
            };
            ServiceLocator.RegisterInstance<ICallsManagementService>(stubICallsManagementService);

            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid);
            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, Enumerable.Range(1, 10).ToArray()).ToList();
            //calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            //activate only first 7 interview
            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid,
                interviews.Take(7).Select(x => x.ID).ToArray(),
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _now,
                CallStates.Suspended,
                false,
                portionSize);

            Assert.AreEqual(AsyncOperationState.PartiallyCompleted, operationResult.State);
            Assert.AreEqual(portionSize, operationResult.FailedItemsCount);

            var activatedIds = BvSvyScheduleAdapter.GetAll().Select(x => x.ID).OrderBy(y => y).ToArray();

            Assert.AreEqual(3, activatedIds.Length);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_SeveralSurveysScheduled_CallsActivatedFor1Survey()
        {
            var surveyId1 = _backendTools.CreateSurvey("p0001231");
            var surveyId2 = _backendTools.CreateSurvey("p0001232");

            var personId = PersonTools.CreatePerson("1q");

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1, 1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1, 2);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId2, 1);
            var interview4 = BackendTools.CreateInterviewWithCall(surveyId2, 2);

            var explicitSid = CallQueueService.GetCallAndNoLock(surveyId2, interview3.ID).Resource;

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveyId1,
                0,
                NewPriority,
                personId,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Scheduled,
                false);

            Assert.AreEqual(personId, CallQueueService.GetCallAndNoLock(surveyId1, interview1.ID).Resource);
            Assert.AreEqual(personId, CallQueueService.GetCallAndNoLock(surveyId1, interview2.ID).Resource);
            Assert.AreEqual(explicitSid, CallQueueService.GetCallAndNoLock(surveyId2, interview3.ID).Resource);
            Assert.AreEqual(explicitSid, CallQueueService.GetCallAndNoLock(surveyId2, interview4.ID).Resource);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCalls_SeveralSurveysSuspended_CallsActivatedFor1Survey()
        {
            var surveyId1 = _backendTools.CreateSurvey("p0001231");
            var surveyId2 = _backendTools.CreateSurvey("p0001232");

            var personId = PersonTools.CreatePerson("1q");

            var interview1 = BackendTools.NewInterview(surveyId1);
            interview1.ID = 1;
            BackendTools.CreateInterview(interview1);
            var interview2 = BackendTools.NewInterview(surveyId1);
            interview2.ID = 2;
            BackendTools.CreateInterview(interview2);
            var interview3 = BackendTools.NewInterview(surveyId2);
            interview3.ID = 1;
            BackendTools.CreateInterview(interview3);
            var interview4 = BackendTools.NewInterview(surveyId2);
            interview4.ID = 2;
            BackendTools.CreateInterview(interview4);

            var operationResult = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveyId1,
                0,
                NewPriority,
                0,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            Assert.AreEqual(NewPriority, CallQueueService.GetCallAndNoLock(surveyId1, interview1.ID).Priority);
            Assert.AreEqual(NewPriority, CallQueueService.GetCallAndNoLock(surveyId1, interview2.ID).Priority);
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveyId2, interview3.ID));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveyId2, interview4.ID));
        }

        private bool CheckSuccessfulResult(AsyncOperationResult result)
        {
            Assert.AreEqual(AsyncOperationState.Completed, result.State);

            return true;
        }

        private bool CheckFailedResult(AsyncOperationResult result, params string[] expectedExceptionTexts)
        {
            Assert.AreEqual(AsyncOperationState.Failed, result.State);
            Assert.AreEqual(expectedExceptionTexts.Length, result.Errors.Count(), "Expected {0} errors, but occured {1}", expectedExceptionTexts.Length, result.Errors.Count());
            CollectionAssert.AreEquivalent(expectedExceptionTexts, result.Errors.Select(item => item.Message).ToArray());

            return false;
        }
    }
}
