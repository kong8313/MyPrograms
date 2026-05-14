using System;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionRecallAfterANumberOfMinutes : BaseMockedIntegrationTest
    {
        private void Test_Base(DateTime eventTime, int param, DateTime resultTime, int shiftTypeID, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.RecallAfterANumberOfMinutes, param.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview, eventTime);

            BackendTools.CheckInterview(interview);
            call.TimeInShift = resultTime;
            BackendTools.CheckCall(call);
        }

        /* 2.2.1 WithCall
         * Test input:      An Interview and a Call with the RuleID configured 
         *                  for a corresponding rule. The amount of minutes after 
         *                  which a recall should be performed must fall 
         *                  into the current Shift limits
         * Test objective:  The interview should not be altered, 
         *                  and a new Call with the specified TimeToCall and using 
         *                  the ID of the preceding Call should be 
         *                  created (time accuracy check)
         */
        private void PositionInShift_RecallInCurrentShift(bool withCall)
        {
            const int param = 10;

            Test_Base(
                DateTime.Parse("2008-09-29T15:00:00"),
                param,
                DateTime.Parse("2008-09-29T15:10:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInCurrentShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInCurrentShift( true );
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInCurrentShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInCurrentShift( false );
        }

        /* 2.2.2 WithCall
         * Test input:      An Interview and a Call with the RuleID configured for 
         *                  a corresponding rule. The amount of minutes after 
         *                  which a recall should be performed must precisely coincide 
         *                  with the Shift limit.
         * Test objective:  The interview should not be altered, and a new Call with the 
         *                  specified TimeToCall and using the ID of the preceding Call 
         *                  should be created (time accuracy check) 
         */
        private void PositionInShift_RecallInCurrentEndShift(bool withCall)
        {
            const int param = 10;

            Test_Base(
                DateTime.Parse("2008-09-29T16:50:00"),
                param,
                DateTime.Parse("2008-09-30T09:00:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInNextBeginShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInCurrentEndShift(true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInNextBeginShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInCurrentEndShift(false);
        }

        /* 2.2.3 WithCall
         * Test input:  An Interview and a Call with the RuleID configured for a corresponding rule. The amount of minutes after which a recall should be performed must be in between two specified Shifts
         * Test objective: The interview should not be altered, and a new Call with the specified TimeToCall and using the ID of the preceding Call should be created (time accuracy check, the next Shift start time check) 
         */
        private void PositionInShift_RecallInAfterCurrentEndShift( bool withCall )
        {
            const int param = 10;

            Test_Base(
                DateTime.Parse("2008-09-29T08:40:00"),
                param,
                DateTime.Parse("2008-09-29T09:00:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInAfterCurrentEndShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInAfterCurrentEndShift(true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInAfterCurrentEndShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInAfterCurrentEndShift(false);
        }
        /* 2.2.4 WithCall
         * Test input:  An Interview and a Call with the RuleID configured for a corresponding rule. The amount of minutes after which a recall should be performed must precizely coincide with the next Shift start time.
         * Test objective: The interview should not be altered, and a new Call with the specified TimeToCall and using the ID of the preceding Call should be created (time accuracy check, the next Shift start time check) 
         */
        private void PositionInShift_RecallInNextStartShift(bool withCall)
        {
            const int param = 10;

            Test_Base(
                DateTime.Parse("2008-09-29T07:50:00"),
                param,
                DateTime.Parse("2008-09-29T09:00:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInNextStartShiftWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInNextStartShift(true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInNextStartShiftWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInNextStartShift(false);
        }
        /* 2.2.5 WithCall
         * Test input:      An Interview and a Call with the RuleID configured for a corresponding rule. The amount of minutes after which a recall should be performed must fall into the limits of the Shift, which occurs some shifts after the current shift.
         * Test objective:  The interview should not be altered, and a new Call with the specified TimeToCall and using the ID of the preceding Call should be created (time accuracy check) 
         */
        private void PositionInShift_RecallInSomeShiftAfterCurrent(bool withCall)
        {
            const int param = 60 * 24 * 2;

            Test_Base(
                DateTime.Parse("2008-09-28T14:00:00"),
                param,
                DateTime.Parse("2008-09-30T14:00:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInSomeShiftAfterCurrentWithCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInSomeShiftAfterCurrent(true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_RecallInSomeShiftAfterCurrentWithoutCall_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            PositionInShift_RecallInSomeShiftAfterCurrent(false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_GetMatchedShift_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var time = new DateTimeMocker("2019-03-27T16:00:00");

            var context = new TestData()
            {
                Surveys = new[] {new SurveyData() { Tag="S1", SchedulingScript = "SS1",
                    Interviews = new []{new InterviewData(){Tag = "S1.I1"} }}},
                Scripts = new[] {new ScriptData() { Tag="SS1", Script = new TestScript(new Action(Action.Operation.RunCustomScript,
                    @"CallShouldBeCreated(); Scheduling.NewCall.TimeInShift = Scheduling.Shifts.GetMatchingShift(Scheduling.Time, TimezoneID).FinishDate"),
                    new Shift(1, 1, "2.10:00:00", "3.20:00:00"))}}
            }.Create();

            var interview = context.GetInterview("S1.I1");
            var call = context.GetCall("S1.I1");

            ServiceLocator.Resolve<IInterviewRepository>().Update(interview.Model, new SchedulingScriptExecutionOptions());

            call.Assert.AreEqual(DateTime.Parse("2019-03-27T20:00:00"), x => x.TimeInShift, "Wrong time in shift");

        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void PositionInShift_GetMatchedShift_Success2(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var time = new DateTimeMocker("2019-03-27T16:00:00");

            TimezoneManager.AddTimezone(16);

            var context = new TestData()
            {
                Surveys = new[] {new SurveyData() { Tag="S1", SchedulingScript = "SS1",
                    Interviews = new []{new InterviewData(){Tag = "S1.I1", TimeZoneId = "16"} }}},
                Scripts = new[] {new ScriptData() { Tag="SS1", Script = new TestScript(new Action(Action.Operation.RunCustomScript,
                        @"CallShouldBeCreated(); Scheduling.NewCall.TimeInShift = Scheduling.Shifts.GetMatchingShift(Scheduling.Time, TimezoneID).FinishDate"),
                    new Shift(1, 1, "0.09:00:00", "0.13:00:00"),
                    new Shift(2, 2, "0.13:00:00", "0.17:00:00"),
                    new Shift(3, 3, "0.17:00:00", "0.21:00:00"),
                    new Shift(4, 1, "1.09:00:00", "1.13:00:00"),
                    new Shift(5, 2, "1.13:00:00", "1.17:00:00"),
                    new Shift(6, 3, "1.17:00:00", "1.21:00:00"),
                    new Shift(7, 1, "2.09:00:00", "2.13:00:00"),
                    new Shift(8, 2, "2.13:00:00", "2.17:00:00"),
                    new Shift(9, 3, "2.17:00:00", "2.21:00:00"),
                    new Shift(10, 1, "3.09:00:00", "3.13:00:00"),
                    new Shift(11, 2, "3.13:00:00", "3.17:00:00"),
                    new Shift(12, 3, "3.17:00:00", "3.21:00:00"),
                    new Shift(13, 1, "4.09:00:00", "4.13:00:00"),
                    new Shift(14, 2, "4.13:00:00", "4.17:00:00"),
                    new Shift(15, 3, "4.17:00:00", "4.21:00:00"),
                    new Shift(16, 4, "5.09:00:00", "5.21:00:00"),
                    new Shift(17, 5, "6.09:00:00", "6.21:00:00")
                    )}}
            }.Create();

            var interview = context.GetInterview("S1.I1");
            var call = context.GetCall("S1.I1");

            ServiceLocator.Resolve<IInterviewRepository>().Update(interview.Model, new SchedulingScriptExecutionOptions());

            call.Assert.AreEqual(DateTime.Parse("2019-03-27T18:00:00"), x => x.TimeInShift, "Wrong time in shift");

        }
    }
}
