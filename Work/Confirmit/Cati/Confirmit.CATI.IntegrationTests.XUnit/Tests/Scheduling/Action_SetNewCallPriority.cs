using System.Globalization;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSetNewCallPriority : BaseMockedIntegrationTest
    {
        private void Test_Base(int its, short priority, short result, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.SetNewCallPriority, result.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = its;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            call.Priority = priority;
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            call.Priority = result;
            BackendTools.CheckCall(call);
        }

        internal void Test_SetPriority20(bool withCall)
        {
            Test_Base(6, 10, 20, withCall);
        }
        internal void Test_SetPriority10(bool withCall)
        {
            Test_Base(6, 10, 10, withCall);

        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCallWithPriority10_SetPriority20_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_SetPriority20(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Interview_SetPriority20_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_SetPriority20(false);
        }
        
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCallWithPriority10_SetPriority10_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_SetPriority10(true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Interview_SetPriority10_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_SetPriority10(false);
        }
    }
}
