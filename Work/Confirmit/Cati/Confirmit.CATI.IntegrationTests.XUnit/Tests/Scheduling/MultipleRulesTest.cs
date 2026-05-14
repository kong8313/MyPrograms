using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class MultipleRulesTest  : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void ThreeRules_SubRuleCriteriaMatchInFirstRule_ExecutionStopsInFirstRule(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int its = 3;
            
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", ITS = (CallOutcome)its}}
                    }
                },
                Scripts =  new []
                {
                    new ScriptData() 
                        { Tag = "SS1", Script = 
                            new TestScript(
                                new [] {
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "31"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "32"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "33"), its, 0, 2, null, false))
                                }, 
                         new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x == null);
            context.GetInterview("S1.I1").Assert.IsTrue(x=>x.TransientState==31);
        }
   
        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void ThreeRules_SubRuleCriteriaMatchInLastRule_ExecutionStopsInLastRule(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int its = 3;
            
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", ITS = (CallOutcome)44}}
                    }
                },
                Scripts =  new []
                {
                    new ScriptData() 
                        { Tag = "SS1", Script = 
                            new TestScript(
                                new [] {
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "31"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "32"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "33"), 44, 0, 2, null, false))
                                }, 
                         new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x == null);
            context.GetInterview("S1.I1").Assert.IsTrue(x=>x.TransientState==33);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void ThreeRules_SecondRuleIsUpdateRule_SubRuleCriteriaMatchInLastRule_UpdateRuleIsNotExecuted_ExecutionStopsInLastRule(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int its = 3;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", ITS = (CallOutcome)44}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() 
                        { Tag = "SS1", Script = 
                            new TestScript(
                                new [] {
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "31"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "32"), 44, 0, 2, null, false))
                                        {SampleUpdate = true},
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "33"), 44, 0, 2, null, false))
                                }, 
                         new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x == null);
            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == 33);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void ThreeRules_LastRuleIsUpdateRule_SubRuleCriteriaDoesNotMatchNormalrules_NoRulesExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            const int its = 3;
            
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", ITS = (CallOutcome)44}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() 
                        { Tag = "SS1", Script = 
                            new TestScript(
                                new [] {
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "31"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "32"), its, 0, 2, null, false)),
                                    new Rule( new SubRule(new Action(Action.Operation.SetNewITS, "33"), 44, 0, 2, null, false))
                                        {SampleUpdate = true}
                                }, 
                         new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x == null);
            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == 44);
        }
    }
}
