using System;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class EditCallsAsyncOperationTest : BaseMockedIntegrationTest
    {
        private DateTime? _timeToCall;
        private DateTime? _timeToExpire;
        private int? _callState;
        private int? _callPriority;
        private int? _shiftType;
        private int? _extendedStatus;
        private byte? _dialingMode;

        private readonly DateTime _defaultTimeToCall = new DateTime(2098, 1, 2, 3, 4, 5);
        private readonly DateTime _defaultTimeToExpire = new DateTime(2098, 2, 3, 4, 5, 6);
        private const int DefaultCallState = 3;
        private const int DefaultCallPriority = 10;
        private const int DefaultShiftType = 0;
        private const CallOutcome DefaultExtendedStatus = CallOutcome.Busy;
        private const string DefaultDialingMode = "2";
        private const string SpecificTimezoneId = "2";
        private int _defaultFcdBehaviorType;
        private int _localTimezoneId;

        private IAsyncOperationExecutor _asyncOperationExecutor;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            systemSettings.AsyncOperation.ActivatePortionSize = 2;
            systemSettings.FCD.BehaviorType = (int)CallState.DisabledByFCD;

            _defaultFcdBehaviorType = systemSettings.FCD.BehaviorType;

            _asyncOperationExecutor = ServiceLocator.Resolve<IAsyncOperationExecutor>();
            _localTimezoneId = ServiceLocator.Resolve<ICallCenterRepository>().Default.LocalTimezoneId;
        }

        private int GetCallHistoryRowsCount()
        {
            return new DatabaseEngine().ExecuteScalar<int>("SELECT COUNT(*) FROM BvCallHistoryEx", CommandType.Text);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_NoParametersToChange_NothingHasBeenDone()
        {
            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);
            
            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(0, GetCallHistoryRowsCount());
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeAllParameters_AllParametersAreChanged()
        {
            _timeToCall = new DateTime(2099, 10, 9, 8, 7, 6);
            _timeToExpire = new DateTime(2099, 5, 4, 3, 2, 1);
            _callState = 2;
            _callPriority = 200;
            _shiftType = int.MinValue;
            _extendedStatus = 16;
            _dialingMode = 5;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode, 
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift,  _timeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _timeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == _callState);
            changedCalls.Assert.IsTrue(x => x.Priority == _callPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == _shiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == _dialingMode);
            changedInterviews.Assert.IsTrue(x => x.TransientState == _extendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeTimeToCalls_OnlyTimeToCallsAreChanged()
        {
            _timeToCall = new DateTime(2099, 10, 9, 8, 7, 6);

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _timeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
            changedCalls.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            changedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeTimeToExpires_OnlyTimeToExpiresAreChanged()
        {
            _timeToExpire = new DateTime(2099, 5, 4, 3, 2, 1);

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _timeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
            changedCalls.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            changedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeCallStates_OnlyCallStatesAreChanged()
        {
            _callState = 2;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == _callState);
            changedCalls.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            changedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeCallPriorities_OnlyCallPrioritiesAreChanged()
        {
            _callPriority = 200;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
            changedCalls.Assert.IsTrue(x => x.Priority == _callPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            changedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeShiftTypes_OnlyShiftTypesAreChanged()
        {
            _shiftType = int.MinValue;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
            changedCalls.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == _shiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            changedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeExtendedStatuses_OnlyExtendedStatusesAreChanged()
        {
            _extendedStatus = 16;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
            changedCalls.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            changedInterviews.Assert.IsTrue(x => x.TransientState == _extendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeDialingModes_OnlyDialingModesAreChanged()
        {
            _dialingMode = 5;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            changedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
            changedCalls.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            changedCalls.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, _defaultTimeToCall));
            notChangedCall.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, _defaultTimeToExpire));
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);

            var changedInterviews = context.GetInterviews("S1.I3", "S1.I4", "S1.I5");
            changedInterviews.Assert.IsTrue(x => x.DialingMode == _dialingMode);
            changedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);

            var notChangedInterviews = context.GetInterviews("S1.I6");
            notChangedInterviews.Assert.IsTrue(x => x.DialingMode == byte.Parse(DefaultDialingMode));
            notChangedInterviews.Assert.IsTrue(x => x.TransientState == (int)DefaultExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_TryToEnabledCallStateForCallWithClosedFcdCell_FcdBehaviorAlgorithmValueIsUsed()
        {
            _callState = 2;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var callsWithClosedFcdCell = context.GetCalls("S1.I1", "S1.I2");
            callsWithClosedFcdCell.Assert.IsTrue(x => x.CallState == _defaultFcdBehaviorType);

            var changedCall = context.GetCalls("S1.I3");
            changedCall.Assert.IsTrue(x => x.CallState == _callState);

            var notChangedCalls = context.GetCalls("S1.I4", "S1.I5", "S1.I6");
            notChangedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_TryToDisableCallStateForCallWithClosedFcdCell_CallStateIsDisabled()
        {
            _callState = 3;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            new DatabaseEngine().ExecuteNonQuery($"UPDATE BvSvySchedule SET CallState = 2 WHERE SurveySID = {survey.Id} AND(ID = 1 OR ID = 2)", CommandType.Text);
            
            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var callsWithClosedFcdCell = context.GetCalls("S1.I1", "S1.I2", "S1.I3");
            callsWithClosedFcdCell.Assert.IsTrue(x => x.CallState == _callState);

            var notChangedCalls = context.GetCalls("S1.I4", "S1.I5", "S1.I6");
            notChangedCalls.Assert.IsTrue(x => x.CallState == DefaultCallState);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_TryToChangeCallStateForInProgressInterview_SuchCallsWereNotChangedAtAll()
        {
            int interviewInProgressCAllState = (int)CallState.InterviewInProgress;
            _callState = 2;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            new DatabaseEngine().ExecuteNonQuery($"UPDATE BvSvySchedule SET CallState = {interviewInProgressCAllState} WHERE SurveySID = {survey.Id} AND(ID = 3 OR ID = 5)", CommandType.Text);

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(1, GetCallHistoryRowsCount());

            var changedCall = context.GetCalls("S1.I4");
            changedCall.Assert.IsTrue(x => x.CallState == _callState);

            var callsInProgress = context.GetCalls("S1.I3", "S1.I5");
            callsInProgress.Assert.IsTrue(x => x.CallState == interviewInProgressCAllState);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => x.CallState == DefaultCallState);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeShiftTypeToAnyValid_ChangeTypeIsMinusTimezoneIdFromInterview()
        {
            _shiftType = -1;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => x.ShiftID == -int.Parse(SpecificTimezoneId));

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeShiftTypeToPositiveValue_ShiftTypeIsCorrectIdFromBvShiftZones()
        {
            _shiftType = 3;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            new DatabaseEngine().ExecuteNonQuery("INSERT INTO BvShiftZones values (2, 3)", CommandType.Text);

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => x.ShiftID == _shiftType);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => x.ShiftID == DefaultShiftType);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeShiftTypeToNotExistedOne_NoChangesHasBeenDone()
        {
            _shiftType = 3;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, _callPriority, _shiftType, _extendedStatus, _dialingMode,
                new SelectedBatchParameters(context.GetInterviews("S1.I3", "S1.I4", "S1.I5").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(0, GetCallHistoryRowsCount());
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeCallPrioritiesForFilteredCalls_CallPrioritiesAreChanged()
        {
            _callPriority = 200;

            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");

            var batchParameters = new FilteredBatchParameters(survey.Id, 0, _localTimezoneId, CallStates.All,
                new SearchParameterCollection
                {
                    new SearchParameter
                    {
                        ColumnName = "InterviewID",
                        ColumnType = SearchColumnType.Number,
                        Operator = SearchOperator.LessThanOrEqual,
                        Value = 5
                    }});

            var entity = CallManager.EditCalls(
                survey.Id, _timeToCall, _timeToExpire, _callState, 
                _callPriority, _shiftType, _extendedStatus, _dialingMode, batchParameters);

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditFilteredCalls, nameof(EditFilteredCallsEvent), survey.Id);
            Assert.AreEqual(5, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5");
            changedCalls.Assert.IsTrue(x => x.Priority == _callPriority);

            var notChangedCall = context.GetCalls("S1.I6");
            notChangedCall.Assert.IsTrue(x => x.Priority == DefaultCallPriority);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeTimeToCallAndTimeToExpireForCallsWithDifferentTimezones_ChangedTimesAreCorrect()
        {
            var timeToCall = new DateTime(2099, 8, 8, 10, 0, 0);
            var timeToExpire = new DateTime(2099, 9, 9, 10, 0, 0);

            var context = CreateSurveyWithCallsInDifferentTimezones();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, timeToCall, timeToExpire, null, null, null, null, null,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));
            
            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var callWithExistedTimezone = context.GetCall("S1.I1");
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeInShift, TimezoneManager.ConvertToUTC(3, timeToCall)));
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeToExpire, TimezoneManager.ConvertToUTC(3, timeToExpire)));

            callWithExistedTimezone = context.GetCall("S1.I2");
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeInShift, TimezoneManager.ConvertToUTC(8, timeToCall)));
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeToExpire, TimezoneManager.ConvertToUTC(8, timeToExpire)));

            var callWithNotExistedTimezone = context.GetCall("S1.I3");
            Assert.IsTrue(AreDateTimesEqual(callWithNotExistedTimezone.Model.TimeInShift, TimezoneManager.ConvertToUTC(19, timeToCall)));
            Assert.IsTrue(AreDateTimesEqual(callWithNotExistedTimezone.Model.TimeToExpire, TimezoneManager.ConvertToUTC(19, timeToExpire)));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeTimeToCallForCallsWithDifferentTimezones_ChangedTimesAreCorrect()
        {
            var timeToCall = new DateTime(2099, 8, 8, 10, 0, 0);

            var context = CreateSurveyWithCallsInDifferentTimezones();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, timeToCall, null, null, null, null, null, null,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);
            
            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var callWithExistedTimezone = context.GetCall("S1.I1");
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeInShift, TimezoneManager.ConvertToUTC(3, timeToCall)));
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeToExpire, _defaultTimeToExpire));

            callWithExistedTimezone = context.GetCall("S1.I2");
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeInShift, TimezoneManager.ConvertToUTC(8, timeToCall)));
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeToExpire, _defaultTimeToExpire));

            var callWithNotExistedTimezone = context.GetCall("S1.I3");
            Assert.IsTrue(AreDateTimesEqual(callWithNotExistedTimezone.Model.TimeInShift, TimezoneManager.ConvertToUTC(19, timeToCall)));
            Assert.IsTrue(AreDateTimesEqual(callWithNotExistedTimezone.Model.TimeToExpire, _defaultTimeToExpire));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_ChangeTimeToExpireForCallsWithDifferentTimezones_ChangedTimesAreCorrect()
        {
            var timeToExpire = new DateTime(2099, 9, 9, 10, 0, 0);

            var context = CreateSurveyWithCallsInDifferentTimezones();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, null, timeToExpire, null, null, null, null, null,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var callWithExistedTimezone = context.GetCall("S1.I1");
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeInShift, _defaultTimeToCall));
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeToExpire, TimezoneManager.ConvertToUTC(3, timeToExpire)));

            callWithExistedTimezone = context.GetCall("S1.I2");
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeInShift, _defaultTimeToCall));
            Assert.IsTrue(AreDateTimesEqual(callWithExistedTimezone.Model.TimeToExpire, TimezoneManager.ConvertToUTC(8, timeToExpire)));

            var callWithNotExistedTimezone = context.GetCall("S1.I3");
            Assert.IsTrue(AreDateTimesEqual(callWithNotExistedTimezone.Model.TimeInShift, _defaultTimeToCall));
            Assert.IsTrue(AreDateTimesEqual(callWithNotExistedTimezone.Model.TimeToExpire, TimezoneManager.ConvertToUTC(19, timeToExpire)));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void EditCalls_SetTimeToCallToNowAndTimeToExpireToNeverForCallsWithDifferentTimezones_NowAndNewerTimesAreTheSameForAllCalls()
        {
            var timeToCall = new DateTime(1899, 12, 30, 0, 0, 0);
            var timeToExpire = new DateTime(9999, 1, 1, 0, 0, 0);

            var context = CreateSurveyWithCallsInDifferentTimezones();
            var survey = context.GetSurvey("S1");

            var entity = CallManager.EditCalls(
                survey.Id, timeToCall, timeToExpire, null, null, null, null, null,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            _asyncOperationExecutor.ExecuteOperationSync(entity);

            TestAssert.ManagementActivityEventExists(ManagementEvent.EditSelectedCalls, nameof(EditSelectedCallsEvent), survey.Id);
            Assert.AreEqual(3, GetCallHistoryRowsCount());

            var changedCalls = context.GetCalls("S1.I1", "S1.I2", "S1.I3");
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeInShift, timeToCall));
            changedCalls.Assert.IsTrue(x => AreDateTimesEqual(x.TimeToExpire, timeToExpire));
        }

        private TestDataContext CreateSurveyWithCallsInDifferentTimezones()
        {
            TimezoneManager.AddTimezone(3);
            TimezoneManager.AddTimezone(8);
            TimezoneManager.AddTimezone(19);

            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            var defaultCallCenter = callCenterRepository.Default;
            defaultCallCenter.LocalTimezoneId = 19;
            callCenterRepository.Update(defaultCallCenter);

            return new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", 
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = CreateDefaultCallData(), TimeZoneId = "3" },
                            new InterviewData { Tag = "S1.I2", Call = CreateDefaultCallData(), TimeZoneId = "8" },
                            new InterviewData { Tag = "S1.I3", Call = CreateDefaultCallData() }
                        }
                    }
                }
            }.Create();
        }

        private TestDataContext CreateSurveyWithQuota()
        {
            return new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData { Name = "q1", Precodes = new[] { "1", "2" } }
                        },
                        Quotas = new[]
                        {
                            new QuotaData
                            {
                                Id = 1, Name = "quota", Fields = new[] { "q1" },
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values = "q1=1", Counter = 1, Limit = 1 },
                                    new CellData { Id = 2, Values = "q1=2", Counter = 0, Limit = 1 },
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Data = "q1=1", Call = CreateDefaultCallData(), DialMode = DefaultDialingMode, ITS = DefaultExtendedStatus },
                            new InterviewData { Tag = "S1.I2", Data = "q1=1", Call = CreateDefaultCallData(), DialMode = DefaultDialingMode, ITS = DefaultExtendedStatus },
                            new InterviewData { Tag = "S1.I3", Data = "q1=2", Call = CreateDefaultCallData(), DialMode = DefaultDialingMode, ITS = DefaultExtendedStatus, TimeZoneId = SpecificTimezoneId },
                            new InterviewData { Tag = "S1.I4", Data = "q1=2", Call = CreateDefaultCallData(), DialMode = DefaultDialingMode, ITS = DefaultExtendedStatus, TimeZoneId = SpecificTimezoneId },
                            new InterviewData { Tag = "S1.I5", Data = "q1=2", Call = CreateDefaultCallData(), DialMode = DefaultDialingMode, ITS = DefaultExtendedStatus, TimeZoneId = SpecificTimezoneId },
                            new InterviewData { Tag = "S1.I6", Data = "q1=2", Call = CreateDefaultCallData(), DialMode = DefaultDialingMode, ITS = DefaultExtendedStatus },
                        }
                    }
                }
            }.Create();
        }

        private CallData CreateDefaultCallData()
        {
            return new CallData
            {
                TimeInShift = _defaultTimeToCall,
                TimeToExpire = _defaultTimeToExpire,
                CallState = DefaultCallState,
                Priority = DefaultCallPriority,
                ShiftType = DefaultShiftType
            };
        }

        private bool AreDateTimesEqual(DateTime? dt1, DateTime? dt2)
        {
            if (dt1.HasValue && dt2.HasValue)
            {
                return dt1.Value.Year == dt2.Value.Year && dt1.Value.Month == dt2.Value.Month && dt1.Value.Day == dt2.Value.Day &&
                       dt1.Value.Hour == dt2.Value.Hour && dt1.Value.Minute == dt2.Value.Minute && dt1.Value.Second == dt2.Value.Second;
            }

            return !dt1.HasValue && !dt2.HasValue;
        }
    }
}