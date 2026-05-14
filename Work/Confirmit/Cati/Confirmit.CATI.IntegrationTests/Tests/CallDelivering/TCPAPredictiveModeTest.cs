using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class TcpaPredictiveModeTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.TcpaPredictiveMode)]
        public void PersonDialTypeIsAutomatic_RequestCallsByCampaign_OnlyDialTypeAutomaticCallsAreDelivered()
        {
            var context = CreateContextAndRequestCalls(CallsSelectionAlgorithm.ByCampaign);

            context.GetCalls("S1.I1.Cellphone", "S1.I3.Cellphone").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2.Landline", "S1.I4.Landline", "S1.I5.Landline").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.TcpaPredictiveMode)]
        public void PersonDialTypeIsAutomatic_RequestCallsByPersonGroup_OnlyDialTypeAutomaticCallsAreDelivered()
        {
            var context = CreateContextAndRequestCalls(CallsSelectionAlgorithm.ByPersonGroup);

            context.GetCalls("S1.I1.Cellphone", "S1.I3.Cellphone", "S1.I4.Landline").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2.Landline").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.TcpaPredictiveMode)]
        public void PersonDialTypeIsAutomatic_RequestCallsAssignedToAgentsExplicitly_OnlyDialTypeAutomaticCallsAreDelivered()
        {
            var context = CreateContextAndRequestCalls(CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly);

            context.GetCalls("S1.I1.Cellphone", "S1.I2", "S1.I3.Cellphone").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I4.Landline").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.TcpaPredictiveMode)]
        public void PersonDialTypeIsAutomatic_RequestCallsAssignedToCampaignOnly_OneCallsIsDelivered()
        {
            var context = CreateContextAndRequestCalls(CallsSelectionAlgorithm.CallsAssignedToCampaignOnly);

            context.GetCalls("S1.I5.Landline").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
        }

        private TestDataContext CreateContextAndRequestCalls(CallsSelectionAlgorithm algorithm)
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive,
 
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1.Cellphone", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "PG1" }},
                        new InterviewData { Tag="S1.I2.Landline", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "PG1" }},
                        new InterviewData { Tag="S1.I3.Cellphone", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4.Landline", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I5.Landline", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData()}
                    },
                    Assigns = new[]{"PG1"}
                }},
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1"}
                },
                Persons = new[]{
                    new PersonData { Tag="P1", Memberships="PG1", TaskChoice = TaskChoiceMode.SurveyAssignment, DialType = DialType.Landline }
                },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            dialer.RequestCalls(survey, 5, algorithm, algorithm == CallsSelectionAlgorithm.ByPersonGroup ? context.GetResource("PG1").Id : 0);
            return context;
        }
    }
}
