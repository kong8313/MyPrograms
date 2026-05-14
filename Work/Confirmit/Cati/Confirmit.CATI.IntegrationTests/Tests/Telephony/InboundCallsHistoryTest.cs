using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class InboundCallsHistoryTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\alm")]
        public void CallIsConnactedToInterviewer_OperationIs_ConnectedToAgent()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = callerNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new Framework.Tools.Action(Framework.Tools.Action.Operation.AcceptInboundCall, string.Empty),
                    Shift.Week)} }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            dialer.Behavior.Methods.ConnectInboundCall.Init(DialerMethodBehaviors.SendOutcomeConnected((a) => person.Id));

            //act
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");

            console.WaitInterview();

            var actualOperationType = BvInboundCallsHistoryAdapter.GetAll()
                .Where(x => x.InboundTelNumber == inboundCallNumber)
                .OrderByDescending(x => x.Id).Select(x => (InboundHandlerOperationType)x.OperationType)
                .First();

            // assert
            Assert.IsNotNull(actualOperationType);
            Assert.AreEqual(InboundHandlerOperationType.ConnectedToAgent, actualOperationType);
        }
    }
}