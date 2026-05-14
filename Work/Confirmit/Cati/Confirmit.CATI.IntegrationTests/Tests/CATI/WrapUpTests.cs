using System;
using System.Threading;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class WrapUpTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\MaximL")]
        public void CallSecondWrapUp_FirstWrapUpIsLockedOnBvSvySchedule_OnlyOneCompleteCallAreSentToDialer()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        SchedulingScript = AllHoursSchedule.Name,
                        Tag = "S1", IsOpen = true, DialMode = DialingMode.Automatic,
                        Interviews = new[]
                        {
                            new InterviewData()
                                {Tag = "S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData() {Resource = "P1"}},
                            new InterviewData()
                                {Tag = "S1.I2", ITS = CallOutcome.FreshSample, Call = new CallData() {Resource = "P1"}},
                            new InterviewData()
                                {Tag = "S1.I3", ITS = CallOutcome.FreshSample, Call = new CallData() {Resource = "P1"}},
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[]
                {
                    new DialerData {Tag = "D1"}
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);

            console.Login();
            console.LoginToDialer();

            var interview = console.StartInterview();
            var details = new CompletedInterviewDetails() { Its = ((int)CallOutcome.Completed).ToString() };

            Thread firstWrapUpThread;
            Thread secondWrapUpThread;

            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            bool hasFirstExceptionBeenThrown = false;
            bool hasSecondExceptionBeenThrown = false;

            using (var connection = new SqlConnection(BackendInstance.Current.ConnectionString))
            {
                connection.Open();
                var command =
                    new SqlCommand("BEGIN TRAN; UPDATE BvSvySchedule WITH (TABLOCK, XLOCK) SET CallState = CallState",
                        connection);
                command.ExecuteNonQuery();

                //Run first wrapup
                firstWrapUpThread = new Thread(() =>
                    {
                        try
                        {
                            console.NextInterview(interview, details, 1);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.ToString());
                            hasFirstExceptionBeenThrown = true;
                        }
                    });

                firstWrapUpThread.Start();

                //wait until first wrapup enter into task lock and call CompleteCall
                TestAssert.WaitCondition(() => completeCallParams.Count > 0, "Complete call wasn't called.");

                Assert.AreEqual(1, completeCallParams.Count, "Wrong count of completeCall call.");

                //Run second wrapup
                secondWrapUpThread = new Thread(() =>
                {
                    try
                    {
                        console.NextInterview(interview, details, 2);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                        hasSecondExceptionBeenThrown = true;
                    }
                });
                secondWrapUpThread.Start();

                //wait until second wrapup is locked on entering into task lock

                Func<bool> condition = () => new DatabaseEngine().ExecuteScalar<int>(
                                                 @"select COUNT(*) from sys.dm_tran_locks 
                            WHERE resource_description like '%TaskLocker_%' and request_status = 'WAIT'") > 0;
                TestAssert.WaitCondition(condition, "Second wrapup didn't try to take task lock");

                //unlock second wrapup, which should be locked on request next call
                command = new SqlCommand("ROLLBACK TRAN", connection);
                command.ExecuteNonQuery();
            }

            Assert.IsTrue(firstWrapUpThread.Join(10000));
            Assert.IsTrue(secondWrapUpThread.Join(10000));

            Assert.IsFalse(hasFirstExceptionBeenThrown, "exception during first interview moving has been thrown");
            Assert.IsFalse(hasSecondExceptionBeenThrown, "exception during second interview moving has been thrown");

            Assert.AreEqual(1, completeCallParams.Count, "Wrong complete calls count");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CallWrapUp_DropByrespondentNotificationIsSentSimultaneously_DropByrespondentNotificationShouldWaitWhileWrapUpIsFinished()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        SchedulingScript = AllHoursSchedule.Name,
                        Tag = "S1", IsOpen = true, DialMode = DialingMode.Predictive,
                        Interviews = new[]
                        {
                            new InterviewData()
                                {Tag = "S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData() {Resource = "P1"}},
                            new InterviewData()
                                {Tag = "S1.I2", ITS = CallOutcome.FreshSample, Call = new CallData() {Resource = "P1"}}
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", ReplyType = ReplyType.Sync}
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var predictive = context.GetDialer("D1").Predictive("S1");

            var console = context.GetPerson(("P1")).Console.Login("S1").LoginToDialer();

            var callId = context.GetCall("S1.I1").Model.CallID;
            var calls = predictive.RequestCalls(survey, 10);
            predictive.Connect(console, "S1.I1");

            console.Wait().Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");
            bool isNotifyDropCallByRespondentFinished = true;
            predictive.Dialer.Behavior.Methods.CompleteCall.Init(
                (controller, args) =>
                {
                    var task = Task.Factory.StartNew(() => predictive.SendEventNotifyDropCallByRespondent(args.CampaignId, long.Parse(args.AgentId), callId));
                    isNotifyDropCallByRespondentFinished = task.Wait(TimeSpan.FromSeconds(1));
                    return 0;
                });

            console.WrapUp().Wait(InterviewState.WAITING);

            Assert.IsFalse(isNotifyDropCallByRespondentFinished);
        }
    }
}
