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
    public class ActionDecrementPriority : BaseMockedIntegrationTest
    {
        private void Test_Base(int its, short priority, short param, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.DecrementPriority, param.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = its;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            if (withCall)
            {
                call.Priority = priority;
                BackendTools.CreateCall(call);
            }

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);

            call.Priority -= param;
            if (call.Priority < 1)
                call.Priority = 1;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithPriorityMax_DecrementOnOne_PriorityDecremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 0x7FFF, 1, true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithoutCall_DecrementOnOne_PriorityNotChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 0x7FFF, 1, false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithPriority10_DecrementOnTwo_PriorityDecremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 10, 2, true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithPriorityOne_DecrementOnOne_PriorityNotChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(6, 1, 1, true);
        }
    }
}
