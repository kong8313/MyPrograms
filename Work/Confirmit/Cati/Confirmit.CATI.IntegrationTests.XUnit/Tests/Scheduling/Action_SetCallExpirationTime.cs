using System;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSetCallExpirationTime : BaseMockedIntegrationTest
    {
        private void Test_Base_Success(DateTime callTime, DateTime expireTime, int shiftTypeID, bool withCall)
        {
            var script = new TestScript(new[]
                { 
                    new Action(Action.Operation.SetTimeToCall, callTime.ToString("u")),
                    new Action(Action.Operation.SetCallExpirationTime, expireTime.ToString("u"))
                },
                @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);

            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            call.TimeInShift = callTime;
            call.ShiftID = script.GetShiftTypeWorkID(shiftTypeID);
            call.TimeToExpire = expireTime;
            BackendTools.CheckCall(call);
        }

        internal void Test_Base_Failed(TestScript script, bool withCall)
        {
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);

            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            interview.TransientState = (int)CallOutcome.Error; // expected ITS

            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(interview.SurveySID, interview.ID));
        }

        internal void Interview_SetExpitationTimeAfter30minutes_Success(bool withCall)
        {
            Test_Base_Success(
                DateTime.Parse("2008-03-17T14:00:00"),
                DateTime.Parse("2008-03-17T14:30:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SetExpitationTimeAfter30minutes_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetExpitationTimeAfter30minutes_Success(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_SetExpitationTimeAfter30minutes_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetExpitationTimeAfter30minutes_Success(false);
        }

        internal void Interview_SetExpitationTimeBefore30minutes_Failed(bool withCall)
        {
            var script = new TestScript(new[] 
                { 
                    new Action(Action.Operation.SetTimeToCall, "2008-03-17 14:00:00Z"),
                    new Action(Action.Operation.SetCallExpirationTime, "2008-03-17T13:30:00")
                },
                @"Scheduling2007\Schedule.xml");

            Test_Base_Failed(script, withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SetExpitationTimeBefore30minutes_Failed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetExpitationTimeBefore30minutes_Failed(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_SetExpitationTimeBefore30minutes_Failed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetExpitationTimeBefore30minutes_Failed(false);
        }
    }
}
