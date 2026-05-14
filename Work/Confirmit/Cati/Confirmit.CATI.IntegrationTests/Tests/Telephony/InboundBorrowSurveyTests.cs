using System;
using System.Collections.Generic;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class InboundBorrowSurveyTests : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void NewInboundCallForSurveyA_InterviewerWorksInSurveyB_InterviewsIsBorrowedToSurveyAAndReturnedBackNormallyInTheEnd()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new []{ 
                    new SurveyData{ Tag="SA", AssignsS = "P1", SchedulingScript = "SS1", DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="SA.I1", TelephoneNumber = callerNumber, Call = new CallData()}
                        },
                        InboundTelephoneNumbers = new []
                        {
                        new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                    }
                    },
                    new SurveyData{ Tag="SB", AssignsS = "P1", DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="SB.I1", Call = new CallData()},
                            new InterviewData(){Tag="SB.I2", Call = new CallData()},
                            new InterviewData(){Tag="SB.I3", Call = new CallData()}
                        }
                    },
                },
                Scripts = new[]{ new ScriptData{ Tag = "SS1",
                    Script = new TestScript(
                        new SubRule(
                            new Action(Action.Operation.AcceptInboundCall, string.Empty),
                            new Action(Action.Operation.AssignResource, "{PG1}")),
                        Shift.Week)}
                },
                
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1"} },
                PersonGroups = new[] {new PersonGroupData() {Tag="PG1", InboundBehavior = InboundGroupBehavior.DeliverCallsFromOtherSurvey} },
                Dialers = new[] { new DialerData { Tag="D1", ReplyType = ReplyType.Sync } }
            }.Create();

            var surveyB = context.GetSurvey("SB");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var setCompaingParams = dialer.Behavior.Methods.SetCampaign.Init();

            var console = new PredictiveConsoleController(context, person, surveyB, dialer);
            var calls = console.LoginAndStart();

            var interview = console.WaitInterview(calls, calls.CallList[0]);
            Assert.AreEqual("SB.I1", interview.Tag);
            console.FinishInterview(interview);

            dialer.Behavior.Methods.ConnectInboundCall.Init(DialerMethodBehaviors.SendOutcomeConnected((a) => person.Id));
            
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");

            interview = console.WaitInterview();
            
            Assert.AreEqual("SA.I1", interview.Tag);
            Assert.AreEqual(0, setCompaingParams.Count);

            console.FinishInterview(interview);

            Assert.AreEqual(1, setCompaingParams.Count);
            Assert.AreEqual(surveyB.Model.CampaignId, setCompaingParams[0].CampaignId);
            Assert.AreEqual(person.Id, setCompaingParams[0].AgentId);

            interview = console.WaitInterview(calls, calls.CallList[1]);

            Assert.AreEqual("SB.I2", interview.Tag);
        }

        [TestMethod]
        public void NewInboundCallForSurveyAWithLinkedToSurveyC_InterviewerWorksInSurveyB_InterviewsIsBorrowedToSurveyAAndReturnedBackToSurveyB()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[]{ 
                    new SurveyData{ Tag="SA", AssignsS = "P1", SchedulingScript = "SS1", DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="SA.I1", TelephoneNumber = callerNumber, Call = new CallData()}
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    },
                    new SurveyData{ Tag="SB", AssignsS = "P1", DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="SB.I1", Call = new CallData()},
                            new InterviewData(){Tag="SB.I2", Call = new CallData()},
                            new InterviewData(){Tag="SB.I3", Call = new CallData()}
                        }
                    },
                    new SurveyData{ Tag="SC", AssignsS = "P1", DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="SC.I1", Call = new CallData()},
                        }
                    },
                },
                Scripts = new[]{ new ScriptData{ Tag = "SS1",
                    Script = new TestScript(
                        new SubRule( 
                            new Action(Action.Operation.AcceptInboundCall, string.Empty),
                            new Action(Action.Operation.AssignResource, "{P1}")),
                        Shift.Week)}
                },

                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1", ReplyType = ReplyType.Sync } }
            }.Create();

            var surveyB = context.GetSurvey("SB");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var setCompaingParams = dialer.Behavior.Methods.SetCampaign.Init();
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();

            var console = new PredictiveConsoleController(context, person, surveyB, dialer);
            var calls = console.LoginAndStart();

            var interview = console.WaitInterview(calls, calls.CallList[0]);
            Assert.AreEqual("SB.I1", interview.Tag);
            console.FinishInterview(interview);

            dialer.Behavior.Methods.ConnectInboundCall.Init(DialerMethodBehaviors.SendOutcomeConnected((a) => person.Id));

            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");

            interview = console.WaitInterview();

            Assert.AreEqual("SA.I1", interview.Tag);
            Assert.AreEqual(0, setCompaingParams.Count);

            console.SetLinkedInterview(context.GetInterview("SC.I1"));

            Assert.AreEqual(0, setNextInterviewParams.Count);

            console.FinishInterview(interview);

            interview = console.WaitInterview();

            Assert.AreEqual("SC.I1", interview.Tag);
            Assert.AreEqual(0, setCompaingParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(interview.Survey.Model.CampaignId, setNextInterviewParams[0].NextCampaingId);
            Assert.AreEqual(interview.Id, setNextInterviewParams[0].NextInterviewId);

            console.FinishInterview(interview);

            Assert.AreEqual(1, setCompaingParams.Count);
            Assert.AreEqual(surveyB.Model.CampaignId, setCompaingParams[0].CampaignId);
            Assert.AreEqual(person.Id, setCompaingParams[0].AgentId);

            interview = console.WaitInterview(calls, calls.CallList[1]);

            Assert.AreEqual("SB.I2", interview.Tag);
        }
    }
}
