using System.Data.SqlClient;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.Interfaces;
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
    public class ScriptXetRespondentValueFunctionTest: BaseMockedIntegrationTest
    {
        private const int InitIts = (int)CallOutcome.FreshSample;
        private const int NewIts = (int)CallOutcome.Completed;
        
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void GetRespondentValue_ReadCallAttemptCount_FilterAreFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="CallAttemptCount=0"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "GetRespondentValue('CallAttemptCount') == 1"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            interview.Assert.IsTrue(x => x.TransientState == InitIts);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void GetRespondentValue_ReadCallAttemtCount_FilterAreTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="CallAttemptCount=1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "GetRespondentValue('CallAttemptCount') == 1"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            interview.Assert.IsTrue(x => x.TransientState == NewIts);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SetRespondentValue_WriteCallAttemtCount_CallAttemptCountIsUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="CallAttemptCount=1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){SetRespondentValue('CallAttemptCount', 10);}"
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            
            var cat = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(
                survey.Id,
                @"SELECT CallAttemptCount FROM <Schema>.respondent WHERE respid = @respId",
            new SqlParameter("@respId", interview.Id));

            TestAssert.AreEqual(10, cat);
        }
    }
}
