using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class InboundCallSummaryReportTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\Denism")]
        public void OneHandledAndOneDroppedBySystem_IsClaculatedSuccessfully()
        {
            var time = DateTime.Parse("2015-03-13T08:00:00");
            var timeMocker = new DateTimeMocker(TestingFramework);
            timeMocker.MockDate(time);

            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var inboundCallId = Guid.NewGuid().ToString();

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "SA",
                        AssignsS = "P1",
                        SchedulingScript = "SS1",
                        DialMode = DialingMode.Predictive,
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData {Tag = "SA.I2", TelephoneNumber = callerNumber, Call = new CallData()}
                        },
                        InboundTelephoneNumbers = new[]
                        {
                            new InboundTelephoneNumberData {Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData
                    {
                        Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new[]
                            {
                                new Action(Action.Operation.AcceptInboundCall, string.Empty),
                                new Action(Action.Operation.IncrementPriority, "10")
                            }),
                            new Shift(1, 1, "0.00:00:00", "6.00:00:00"))
                    }
                },
                Persons = new[] {new PersonData {Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment}},
                Dialers = new[] {new DialerData {Tag = "D1"}}
            }.Create();

            var survey = context.GetSurvey("SA");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");
            dialer.SetNotificationReply(ReplyType.Sync);

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.Behavior.Methods.ConnectInboundCall.Init(DialerMethodBehaviors.SendOutcomeConnected((a) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(7));
                return person.Id;
            }));
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, inboundCallId);

            var interview = console.StartInterview();

            Assert.AreEqual("SA.I2", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));
            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 100, Status = "Complete" });

            // fill calls dropped by system
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            systemSettings.Toggle.EnableInbound = false;
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, inboundCallId);
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, inboundCallId);

            var result = BvSpReportInboundCallsAdapter.ExecuteEntityList(survey.Id, "13", time.AddHours(-1),
                time.AddHours(1));

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(7, result[0].HourInDay);
            Assert.AreEqual(8, result[1].HourInDay);
            Assert.AreEqual((decimal) 66.67, result[1].AbandonRate);
            Assert.AreEqual(0, result[1].AbandonedByResp);
            Assert.AreEqual(30, result[1].AvgCallDurationForConnected);
            Assert.AreEqual(0, result[1].AvgWaitTimeForAbandons);
            Assert.AreEqual(7, result[1].AvgWaitTimeForConnection);
            Assert.AreEqual(1, result[1].CompletesCount);
            Assert.AreEqual(1, result[1].DistinctAgents);
            Assert.AreEqual(3, result[1].TotalCalls);
            Assert.AreEqual(1, result[1].HandledCalls);
            Assert.AreEqual(2, result[1].DroppedBySystem);

        }

        [TestMethod]
        public void OneCallDroppedByRespondent_IsCalculatedSuccessfuly()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var inboundCallId = Guid.NewGuid().ToString();

            var time = DateTime.Parse("2015-03-13T08:00:00");
            var timeMocker = new DateTimeMocker(TestingFramework);
            timeMocker.MockDate(time);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        DialMode = DialingMode.Automatic,
                        Assigns = new []{"P1"},
                        SchedulingScript = "SS1",
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData(), TelephoneNumber = callerNumber},
                        },
                        InboundTelephoneNumbers = new[]
                        {
                            new InboundTelephoneNumberData {Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[]
                {
                    new ScriptData
                    {
                        Tag = "SS1",
                        Script =
                            new TestScript(new Action(Action.Operation.AcceptInboundCall, string.Empty),
                                new Shift(1, 1, "0.00:00:00", "6.00:00:00"))
                    }
                },
                Dialers = new[] {new DialerData {Tag = "D1"}},

            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, inboundCallId);
            timeMocker.AddTime(TimeSpan.FromSeconds(12));
            dialer.SendNotifyDropInboundCall(inboundCallId);

            var result = BvSpReportInboundCallsAdapter.ExecuteEntityList(survey.Id, "13", time.AddHours(-1),
                time.AddHours(1));

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(7, result[0].HourInDay);
            Assert.AreEqual(8, result[1].HourInDay);
            Assert.AreEqual(100, result[1].AbandonRate);
            Assert.AreEqual(1, result[1].AbandonedByResp);
            Assert.AreEqual(0, result[1].AvgCallDurationForConnected);
            Assert.AreEqual(12, result[1].AvgWaitTimeForAbandons);
            Assert.AreEqual(0, result[1].AvgWaitTimeForConnection);
            Assert.AreEqual(0, result[1].CompletesCount);
            Assert.AreEqual(0, result[1].DistinctAgents);
            Assert.AreEqual(1, result[1].TotalCalls);
            Assert.AreEqual(0, result[1].HandledCalls);
            Assert.AreEqual(0, result[1].DroppedBySystem);
        }

    }
}
