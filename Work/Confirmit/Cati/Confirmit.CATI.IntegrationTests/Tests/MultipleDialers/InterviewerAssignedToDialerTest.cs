using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Telephony.Fakes;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PersonTools = Confirmit.CATI.IntegrationTests.Framework.Tools.PersonTools;

namespace Confirmit.CATI.IntegrationTests.Tests.MultipleDialers
{
    [TestClass]
    public class InterviewerAssignedToDialerTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void InterviewerLogin_InterviewerLogsInToCorrectDialer()
        {
            var context = new TestData()
            {
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } },
                Dialers = new[] { new DialerData() { Tag = "D1", Id = 5, TenantId = 222} }
            }.Create();

            var dialer = context.GetDialer("D1");

            var loginParams = dialer.Behavior.Methods.Login.Init();

            var console = context.GetPerson("P1").Console.SetStationId("test400001").Login().LoginToDialer();

            Assert.AreEqual(1, loginParams.Count, "Whrong count of Login calls");
            Assert.AreEqual("222", loginParams[0].TenantId);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void InterviewerLogin_CorrectDialerIdIsWrittenToBvPersonTable()
        {
            var context = new TestData()
            {
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } },
                Dialers = new[] { new DialerData() { Tag = "D1", Id=5} }
            }.Create();

            var dialer = context.GetDialer("D1");
            var person = context.GetPerson("P1");
            var console = person.Console.SetStationId("test400001").Login();

            var task = TaskRepository.GetByPerson(person.Id);
            Assert.AreEqual(dialer.Id, task.DialerId, "Incorrect dialerId in BvPerson table.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void InterviewGoesOn_DialCommandIsProcessedOnCorrectDialer()
        {
            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() {
                        Tag = "S1", DialMode = DialingMode.Preview, AssignsS = "P1",
                        IsUseDb = true,
                        Interviews = new[] {
                            new InterviewData { Tag = "S1.I1", Call = new CallData() } 
                            
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } },
                Dialers = new[] { new DialerData() { Tag = "D1", TenantId = 222 } }
            }.Create();

            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);

            var console = context.GetPerson("P1").Console.Login().LoginToDialer().Start().Wait();

            console.Services.ConsoleService.Dial("123", 0, 0);

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "Whrong count of SendNumberToAgent calls");
            Assert.AreEqual("222", sendNumberToAgentParams[0].TenantId);
        }
    }
}
