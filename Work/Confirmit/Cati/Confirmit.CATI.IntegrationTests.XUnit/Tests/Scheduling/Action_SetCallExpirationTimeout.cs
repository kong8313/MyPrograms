using System;
using System.Globalization;
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
    public class ActionSetCallExpirationTimeout : BaseMockedIntegrationTest
    {
        private void Test_Base_Success(DateTime callTime, TimeSpan param, int shiftTypeID, bool withCall)
        {
            var script = new TestScript(new[]
                { 
                    new Action(Action.Operation.SetTimeToCall, callTime.ToString("u")),
                    new Action(Action.Operation.SetCallExpirationTimeout, param.TotalMinutes.ToString(CultureInfo.InvariantCulture))
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
            call.TimeToExpire = callTime + param;
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


        internal void Interview_SetCallExpirationTimeout30minutes_Success(bool withCall)
        {
            Test_Base_Success(
                DateTime.Parse("2008-03-17T14:00:00"),
                TimeSpan.FromMinutes(30),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SetCallExpirationTimeout30minutes_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetCallExpirationTimeout30minutes_Success(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_SetCallExpirationTimeout30minutes_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetCallExpirationTimeout30minutes_Success(false);
        }

        internal void Interview_SetExpitationTimeout30WithoutTimeToCall_Failed(bool withCall)
        {
            var script = new TestScript(new[] 
                { 
                    new Action(Action.Operation.SetCallExpirationTimeout, "30")
                },
                @"Scheduling2007\Schedule.xml");
            Test_Base_Failed(script, withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SetExpitationTimeout30WithoutTimeToCall_Failed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetExpitationTimeout30WithoutTimeToCall_Failed(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_SetExpitationTimeout30WithoutTimeToCall_Failed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetExpitationTimeout30WithoutTimeToCall_Failed(false);
        }
    }
}
