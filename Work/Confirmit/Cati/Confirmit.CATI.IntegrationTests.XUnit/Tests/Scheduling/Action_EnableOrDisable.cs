using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionEnableOrDisable : BaseMockedIntegrationTest
    {
        private void TestBase(bool initState, bool withCall, Action.Operation operation, bool endState)
        {
            var script = new TestScript(
                    new Action(operation),
                    new Shift(1,1,"0.00:00:00", "1.00:00:00"));
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            if (!initState)
            {
                call.CallState = (int)CallState.DisabledByUser;
            }
            if (withCall)
            {
                BackendTools.CreateCall(call);
            }

            BackendTools.FireEvent(interview);

            call.CallState = endState ? (int)CallState.Scheduled : (int)CallState.DisabledByUser;

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void EnableCall_CallIsNotExists_CallIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TestBase(false, false, Action.Operation.EnableCall, true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void EnableCall_EnabledCallIsExists_EnabledCallIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TestBase(true, true, Action.Operation.EnableCall, true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void EnableCall_DisabledCallIsExists_EnabledCallIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TestBase(false, true, Action.Operation.EnableCall, true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void DisableCall_CallIsNotExists_DisabledCallIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TestBase(false, false, Action.Operation.DisableCall, false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void DisableCall_CallIsExists_DisabledCallIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TestBase(true, true, Action.Operation.DisableCall, false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void DisableCall_DisabledCallIsExists_DisabledCallIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TestBase(false, true, Action.Operation.DisableCall, false);
        }
    }
}
