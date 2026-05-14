using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class AddOrUpdateCallTest : BaseMockedIntegrationTest
    {
        public AddOrUpdateCallTest()
        {
            TimezoneService.Activate((int)ActiveTZ.Moscow);

            _script = new TestScript(
                    new Action(Action.Operation.SuspendTheInterview),
                    @"Scheduling2007\Schedule.xml");
            _surveyId = BackendToolsObject.CreateSurvey(_script);
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// Is filled automatically.
        ///</summary>
        public TestContext TestContext { get; set; }

        enum ActiveTZ
        {
            Default = 0,//+0
            Moscow = 16//+3
        }

        enum ShiftType
        {
            AnyValid = -1,
            None = 0,
            Work = 1
        }

        private int _surveyId;
        private TestScript _script;       

        BvCallEntity GetCallForUpdate(DateTime timeInShift, ShiftType shiftType, ActiveTZ tz)
        {
            var interview = BackendTools.NewInterview(_surveyId);
            interview.TimezoneID = (int)tz;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            call.TimeInShift = timeInShift;
            switch (shiftType)
            {
                case ShiftType.AnyValid:
                    call.ShiftID = (int)CallShiftType.AnyValid;
                    break;
                case ShiftType.None:
                    call.ShiftID = (int)CallShiftType.None;
                    break;
                default:
                    call.ShiftID = _script.GetShiftTypeWorkID((int)shiftType);
                    break;
            }

            return call;
        }        

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInDefaultTZ_TimeInShiftOnStartShift_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T10:00:00"), ShiftType.AnyValid, ActiveTZ.Default);

            CallQueueService.UpdateCall(call, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInDefaultTZ_TimeInShiftInShift_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T14:00:00"), ShiftType.AnyValid, ActiveTZ.Default);

            CallQueueService.UpdateCall(call, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInDefaultTZ_TimeInShiftOnEndShift_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T17:59:00"), ShiftType.AnyValid, ActiveTZ.Default);

            CallQueueService.UpdateCall(call, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInMoscowTZ_TimeInShiftOnStartShift_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T07:00:00"), ShiftType.AnyValid, ActiveTZ.Moscow);

            CallQueueService.UpdateCall(call, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInMoscowTZ_TimeInShiftInShift_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T11:00:00"), ShiftType.AnyValid, ActiveTZ.Moscow);

            CallQueueService.UpdateCall(call, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInMoscowTZ_TimeInShiftOnEndShift_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T11:58:00"), ShiftType.AnyValid, ActiveTZ.Moscow);

            CallQueueService.UpdateCall(call, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void UpdateCallInMoscowTZ_TimeInShiftInShiftType_CallUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var call = GetCallForUpdate(DateTime.Parse("2009-02-05T11:00:00"), ShiftType.Work, ActiveTZ.Moscow);

            CallQueueService.UpdateCall(call, 0);
        }
    }
}
