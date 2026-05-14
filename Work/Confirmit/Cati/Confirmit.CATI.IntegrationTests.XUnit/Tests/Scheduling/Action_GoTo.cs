using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionGoTo : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\DmitryS")]
        [ClassData(typeof(TestDataGenerator))]
        public void GoToJumpsToSubRule_PriorityIncremented(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var secondSubRuleId = Guid.NewGuid();
            var incrementValue = 10;

            var script = new TestScript(new[] {
                            new SubRule(new [] {new Action(Action.Operation.GoTo, secondSubRuleId.ToString())}),
                            new SubRule(secondSubRuleId, new [] {new Action(Action.Operation.IncrementPriority, incrementValue.ToString()) })
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Priority += incrementValue;

            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\DmitryS")]
        [ClassData(typeof(TestDataGenerator))]
        public void GoToInLoop_PriorityNotIncremented_ErrorOutcome(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var firstSubRuleId = Guid.NewGuid();
            var secondSubRuleId = Guid.NewGuid();
            var incrementValue = 10;

            var script = new TestScript(new[] {
                            new SubRule(firstSubRuleId, new [] {new Action(Action.Operation.GoTo, secondSubRuleId.ToString())}),
                            new SubRule(secondSubRuleId, new [] {
                                new Action(Action.Operation.IncrementPriority, incrementValue.ToString()),
                                new Action(Action.Operation.GoTo, firstSubRuleId.ToString())
                            })
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = (int)CallOutcome.Error;
            BackendTools.CheckInterview(interview);
        }
    }
}