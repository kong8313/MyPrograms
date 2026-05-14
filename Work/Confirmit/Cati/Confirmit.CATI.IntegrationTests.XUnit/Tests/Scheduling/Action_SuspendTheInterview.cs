using System;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSuspendTheInterview : BaseMockedIntegrationTest
    {
        private void Test_Base(bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.SuspendTheInterview),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);
            if (withCall)
                BackendTools.CreateCall(BackendTools.NewCall(interview));

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(interview.SurveySID, interview.ID));
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_SuspendTheInterview_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(true);
        }
        
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_SuspendTheInterview_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(false);
        }
    }
}
