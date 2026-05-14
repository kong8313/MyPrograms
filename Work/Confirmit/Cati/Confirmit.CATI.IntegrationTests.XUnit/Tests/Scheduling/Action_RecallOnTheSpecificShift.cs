using System;
using System.Globalization;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionRecallOnTheSpecificShift : BaseMockedIntegrationTest
    {
        private void Test_Base(DateTime eventTime, int param, int paramShiftID, DateTime resultTime, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.RecallOnTheSpecificShift, param.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);

            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            call.ShiftID = script.GetShiftTypeWorkID(paramShiftID);
            call.TimeInShift = resultTime;
            BackendTools.CheckCall(call);
        }

        internal void PositionOutOfShiftWithoutDaylightSavingTime_RecallOnThe3Shift_SuccessAndPositionInShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-18T23:00:00"),
                1, 1,
                DateTime.Parse("2008-03-24T10:00:00"),
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShiftWithoutDaylightSavingTime_RecallOnThe3ShiftWithCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionOutOfShiftWithoutDaylightSavingTime_RecallOnThe3Shift_SuccessAndPositionInShift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShiftWithoutDaylightSavingTime_RecallOnThe3ShiftWithoutCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionOutOfShiftWithoutDaylightSavingTime_RecallOnThe3Shift_SuccessAndPositionInShift(false);
        }

        internal void PositionOutOfShiftWithDaylightSavingTime_RecallOnThe3Shift_SuccessAndPositionInShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-06-16T05:00:00"),
                1, 1,
                DateTime.Parse("2008-06-16T09:00:00"),
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShiftWithDaylightSavingTime_RecallOnThe3ShiftWithCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionOutOfShiftWithDaylightSavingTime_RecallOnThe3Shift_SuccessAndPositionInShift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShiftWithDaylightSavingTime_RecallOnThe3ShiftWithoutCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionOutOfShiftWithDaylightSavingTime_RecallOnThe3Shift_SuccessAndPositionInShift(false);
        }
    }
}
