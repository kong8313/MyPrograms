using System;
using System.Globalization;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionSetNextRule : BaseMockedIntegrationTest
    {
        private void Test_Base(TestScript script, Guid param, int oldITS, int newITS, bool withCall)
        {
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = oldITS;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            call.RuleNumber = param;
            BackendTools.CheckCall(call);

            BackendTools.FireEvent(interview);

            interview.TransientState = newITS;
            BackendTools.CheckInterview(interview);
            call.RuleNumber = Guid.Empty;
            Assert.IsFalse( BackendTools.IsCallExists(interview.SurveySID, interview.ID) );
        }

        internal void TwoReschedulingFor3Rules(bool withCall)
        {
            Guid ruleID = Guid.NewGuid();
            const int oldITS = 16;
            const int errITS = 8;
            const int newITS = 1;

            var script = new TestScript(
                new []
                {
                    new Rule( Guid.NewGuid(), new Action( Action.Operation.SetNextRule, ruleID.ToString())),
                    new Rule( Guid.NewGuid(), new Action( Action.Operation.SetNewITS, errITS.ToString(CultureInfo.InvariantCulture) ) ),
                    new Rule( ruleID, new Action( Action.Operation.SetNewITS, newITS.ToString(CultureInfo.InvariantCulture) ) )
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));
            Test_Base(
                script,
                ruleID,
                oldITS,
                newITS,
                withCall);
        }

        [Theory, Owner("Maxim Lipatov")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithItsEq16AndWithCall_TwoReschedulingForTwoRules_ItsEq1(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TwoReschedulingFor3Rules(true);
        }

        [Theory, Owner("Maxim Lipatov")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithItsEq16_TwoReschedulingForTwoRules_ItsEq1(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TwoReschedulingFor3Rules(false);
        }
    }
}
