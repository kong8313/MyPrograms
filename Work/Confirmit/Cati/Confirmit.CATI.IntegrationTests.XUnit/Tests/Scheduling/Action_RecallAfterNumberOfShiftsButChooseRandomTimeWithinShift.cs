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
    public class ActionRecallAfterNumberOfShiftsButChooseRandomTimeWithinShift : BaseMockedIntegrationTest
    {
        private void Test_Base(DateTime eventTime, int param, DateTime resultStartTime, DateTime resultEndTime, int shiftTypeID, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.RecallAfterNumberOfShiftsButChooseRandomTimeWithinShift, param.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            call.ShiftID = script.GetShiftTypeWorkID(shiftTypeID);

            BackendTools.CheckCall(call, resultStartTime, resultEndTime);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterOneShiftWithCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T14:00:00"),
                1,
                DateTime.Parse("2008-03-18T10:00:00"),
                DateTime.Parse("2008-03-18T18:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShift_RecallAfterOneShiftWithCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T19:00:00"),
                1,
                DateTime.Parse("2008-03-18T10:00:00"),
                DateTime.Parse("2008-03-18T18:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterThreeShiftWithCall_SuccessAndPositionInExclusionShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T14:00:00"),
                3,
                DateTime.Parse("2008-03-20T10:00:00"),
                DateTime.Parse("2008-03-20T18:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfter15ShiftWithCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-23T14:00:00"),
                7 * 2 + 1,
                DateTime.Parse("2008-04-07T09:00:00"),
                DateTime.Parse("2008-04-07T17:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShift_RecallAfter15ShiftWithCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-23T21:00:00"),
                7 * 2 + 1,
                DateTime.Parse("2008-04-07T09:00:00"),
                DateTime.Parse("2008-04-07T17:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterOneShiftWithoutCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T14:00:00"),
                1,
                DateTime.Parse("2008-03-18T10:00:00"),
                DateTime.Parse("2008-03-18T18:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShift_RecallAfterOneShiftWithoutCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T19:00:00"),
                1,
                DateTime.Parse("2008-03-18T10:00:00"),
                DateTime.Parse("2008-03-18T18:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterThreeShiftWithoutCall_SuccessAndPositionInExclusionShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-17T14:00:00"),
                3,
                DateTime.Parse("2008-03-20T10:00:00"),
                DateTime.Parse("2008-03-20T18:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfter15ShiftWithoutCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-23T14:00:00"),
                7 * 2 + 1,
                DateTime.Parse("2008-04-07T09:00:00"),
                DateTime.Parse("2008-04-07T17:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionOutOfShift_RecallAfter15ShiftWithoutCall_SuccessAndPositionInShift(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(
                DateTime.Parse("2008-03-23T21:00:00"),
                7 * 2 + 1,
                DateTime.Parse("2008-04-07T09:00:00"),
                DateTime.Parse("2008-04-07T17:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                false);
        }
    }
}
