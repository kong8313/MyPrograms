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
    public class ActionRecallOnNextShiftOfSpecifiedType : BaseMockedIntegrationTest
    {
        enum ShiftTypes
        {
            WorkDay = 1,
            Weekend = 2
        }

        private void Test_Base(DateTime eventTime, int param, DateTime resultTime, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.RecallOnNextShiftOfSpecifiedType, param.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            call.ShiftID = script.GetShiftTypeWorkID(param);
            call.TimeInShift = resultTime;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInWorkdayShift_RecallOnNextWeekendShiftWithCall_SuccessAndPositionInNextWeekendShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T14:00:00"),
                (int)ShiftTypes.Weekend,
                DateTime.Parse("2008-03-22T10:00:00"),
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInWeekendShift_RecallOnNextWeekendShiftWithCall_SuccessAndPositionInNextWeekendShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-22T14:00:00"),
                (int)ShiftTypes.Weekend,
                DateTime.Parse("2008-03-23T10:00:00"),
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShift_RecallOnNextWorkdayShiftWithCall_SuccessAndPositionInNextWorkdayShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T23:00:00"),
                (int)ShiftTypes.WorkDay,
                DateTime.Parse("2008-03-18T10:00:00"),
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInWorkdayShift_RecallOnNextWeekendShiftWithoutCall_SuccessAndPositionInNextWeekendShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-18T14:00:00"),
                (int)ShiftTypes.Weekend,
                DateTime.Parse("2008-03-22T10:00:00"),
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInWeekendShift_RecallOnNextWeekendShiftWithoutCall_SuccessAndPositionInNextWeekendShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-22T14:00:00"),
                (int)ShiftTypes.Weekend,
                DateTime.Parse("2008-03-23T10:00:00"),
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShift_RecallOnNextWorkdayShiftWithoutCall_SuccessAndPositionInNextWorkdayShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T23:00:00"),
                (int)ShiftTypes.WorkDay,
                DateTime.Parse("2008-03-18T10:00:00"),
                false);
        }
    }
}
