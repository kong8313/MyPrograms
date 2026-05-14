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
    public class ActionRecallAfterANumberOfShifts : BaseMockedIntegrationTest
    {
        private void Test_Base(DateTime eventTime, int param, DateTime resultTime, int shiftTypeID, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.RecallAfterANumberOfShifts, param.ToString(CultureInfo.InvariantCulture)),
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
            call.TimeInShift = resultTime;
            BackendTools.CheckCall(call);
        }
        /* 2.3.1 WithCall
          */
        internal void PositionInShift_RecallAfterOneShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-17T14:00:00"),
                1,
                DateTime.Parse("2008-03-18T10:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterOneShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallAfterOneShift(true);
        }
        
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterOneShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallAfterOneShift(false);
        }
        /* 2.3.2 WithCall
         */
        internal void PositionInOutOfShift_RecallAfterOneShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-17T19:00:00"),
                1,
                DateTime.Parse("2008-03-18T10:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInOutOfShift_RecallAfterOneShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInOutOfShift_RecallAfterOneShift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInOutOfShift_RecallAfterOneShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInOutOfShift_RecallAfterOneShift(false);
        }
        /* 2.3.3 WithCall
          */
        internal void PositionInShift_RecallAfter3Shift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-17T14:00:00"),
                3,
                DateTime.Parse("2008-03-20T10:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfter3ShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallAfter3Shift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfter3ShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallAfter3Shift(false);
        }
        /* 2.3.4 WithCall
          */
        internal void PositionInShift_RecallAfterCountOfShiftsPlusOneShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-23T14:00:00"),
                7 * 2 + 1,
                DateTime.Parse("2008-04-07T09:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterCountOfShiftsPlusOneShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallAfterCountOfShiftsPlusOneShift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallAfterCountOfShiftsPlusOneShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallAfterCountOfShiftsPlusOneShift(false);
        }
        /* 2.3.5 WithCall
        */
        internal void PositionInOutOfShift_RecallAfterCountOfShiftsPlusOneShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-23T21:00:00"),
                7 * 2 + 1,
                DateTime.Parse("2008-04-07T09:00:00"),
                1 /*ShiftTypeId(Work day)*/,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInOutOfShift_RecallAfterCountOfShiftsPlusOneShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInOutOfShift_RecallAfterCountOfShiftsPlusOneShift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInOutOfShift_RecallAfterCountOfShiftsPlusOneShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInOutOfShift_RecallAfterCountOfShiftsPlusOneShift(false);
        }
    }
}
