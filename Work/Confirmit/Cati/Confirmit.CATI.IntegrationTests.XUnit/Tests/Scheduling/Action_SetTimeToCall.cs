using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSetTimeToCall : BaseMockedIntegrationTest
    {
        private void Test_Base(DateTime param, int shiftTypeID, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.SetTimeToCall, param.ToString("u")),
                    @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);

            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            call.ShiftID = script.GetShiftTypeWorkID(shiftTypeID);
            call.TimeInShift = param;
            BackendTools.CheckCall(call);
        }

        internal void Interview_SetTimeToCallInShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-18 12:00:00"),
                1,
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SetTimeToCallInShift_TimeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetTimeToCallInShift(true);
        }
        
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Interview_SetTimeToCallInShift_TimeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetTimeToCallInShift(false);
        }

        internal void Interview_SetTimeToCallOutOfShift(bool withCall)
        {
            Test_Base(
                DateTime.Parse("2008-03-19 00:00:00"),
                (int)CallShiftType.None, 
                withCall);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SetTimeToCallOutOfShift_TimeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetTimeToCallOutOfShift(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Interview_SetTimeToCallOutOfShift_TimeSet(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_SetTimeToCallOutOfShift(false);
        }
    }
}
