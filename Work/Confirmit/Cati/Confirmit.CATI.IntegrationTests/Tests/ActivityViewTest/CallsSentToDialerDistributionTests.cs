using System;
using System.Data;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.ActivityViewTest
{
    [TestClass]
    public class CallsSentToDialerDistributionTests : BaseMockedIntegrationTest
    {
        private const string groupName = "groupName";
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";
        private const int defaultTimeZoneId = 1;

        private int totalCount;
        BvInterviewEntity[] interviews;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            BackendTools.ResetInterviewId();
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void CallsSentToDialerDistribution_SentToCampaignOneTime_CorrectDataAreReturn()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            var surveySid = test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            var groupName = "groupName";
            int groupId = PersonTools.CreatePersonGroup(groupName);
            PersonService.SetParentGroups(test.PersonSID, new[] { PersonGroupService.RootGroupId, groupId });

            interviews = test.CreateInterviewsWithCalls(4);

            BackendTools.AssignResourceToInterview(surveySid, interviews[0].ID, test.PersonSID); //ExplicitAssignment
            BackendTools.AssignResourceToInterview(surveySid, interviews[1].ID, groupId); //Assigned to the concrete group

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { "" });
            BackendTools.RunSchedulingProcedure();

            PredictiveTools.GetCallsForPredictive(surveySid, groupId, CallsSelectionAlgorithm.ByCampaign, 4);

            var table = ServiceLocator.Resolve<ISurveyCallDistributionService>().GetCallsSentToDialerDistribution(surveySid, null, defaultTimeZoneId, out totalCount);

            Assert.AreEqual(3, table.Rows.Count);

            CheckTable(table, 3, 2, new[] { new object[] { "*Survey Assignment*", 2 },
                                            new object[] { groupName, 1 },
                                            new object[] { UserName, 1 }});
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void GetCallsDispositionCodes_GetDataFromBvCallHistoryAndBvCallHistoryExTables_CorrectDataAreReturn()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData(){ Tag="S1", IsUseDb = true, IsOpen = true, AssignsS = "P1", DialMode = DialingMode.Predictive, SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new []{
                        new InterviewData(){Tag="S1.I1", DialMode = "2", Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){Tag="S1.I2", DialMode = "2", Call = new CallData(){Resource = "P1"}}
                    }
                } },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            var callsRequest = console.LoginAndStart(10, CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly, person.Id);
            dialer.DialerHelper.SendEventNotifyOutcome(callsRequest.CampaignId, person.Id, (int)callsRequest.CallList[0].callId, CallOutcome.ReturnedDiallerExpired);
            dialer.DialerHelper.SendEventNotifyOutcome(callsRequest.CampaignId, person.Id, (int)callsRequest.CallList[1].callId, CallOutcome.ReturnedNotDialled);

            // Copy some rows from BvCallHistoryEx to BvCallHistory to check that BvSpGetExtendedCallHistory gets data from both tables
            BackendTools.CopyCallHistoryExToCallHistory(2);
            
            var table = ServiceLocator.Resolve<ISurveyCallDistributionService>().GetCallsDispositionCodes(survey.Id, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), out var totalCount);
            Assert.AreEqual(2, totalCount);
            Assert.AreEqual(2, table.Rows.Count);
            CheckTable(table, 2, 4, new[] { 
                new object[] { (int)CallOutcome.ReturnedNotDialled, "Returned not dialled", 1, "50.00" },
                new object[] { (int)CallOutcome.ReturnedDiallerExpired, "Returned dialler expired", 1, "50.00" }});
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void CallsSentToDialerDistribution_SentToCampaignTwoTimes_CorrectDataAreReturn()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            var surveySid = test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            int groupId = PersonTools.CreatePersonGroup(groupName);
            PersonService.SetParentGroups(test.PersonSID, new[] { PersonGroupService.RootGroupId, groupId });

            interviews = test.CreateInterviewsWithCalls(20);

            BackendTools.AssignResourceToInterview(surveySid, interviews[0].ID, test.PersonSID); //ExplicitAssignment
            BackendTools.AssignResourceToInterview(surveySid, interviews[1].ID, groupId); //Assigned to the concrete group

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { "" });
            BackendTools.RunSchedulingProcedure();

            PredictiveTools.GetCallsForPredictive(surveySid, groupId, CallsSelectionAlgorithm.ByCampaign, 9);

            PredictiveTools.GetCallsForPredictive(surveySid, groupId, CallsSelectionAlgorithm.ByCampaign, 9);

            var table = ServiceLocator.Resolve<ISurveyCallDistributionService>().GetCallsSentToDialerDistribution(surveySid, null, defaultTimeZoneId, out totalCount);

            CheckTable(table, 3, 3, new[] { new object[] { "*Survey Assignment*", 8, 9 },
                                            new object[] { groupName, 1 },
                                            new object[] { UserName, 1 }});
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void CallsSentToDialerDistribution_SentToCampaignTwoTimesRequestDataOfSecondTime_CorrectDataAreReturn()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            var surveySid = test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            int groupId = PersonTools.CreatePersonGroup(groupName);
            PersonService.SetParentGroups(test.PersonSID, new[] { PersonGroupService.RootGroupId, groupId });

            interviews = test.CreateInterviewsWithCalls(20);

            BackendTools.AssignResourceToInterview(surveySid, interviews[0].ID, test.PersonSID); //ExplicitAssignment
            BackendTools.AssignResourceToInterview(surveySid, interviews[1].ID, groupId); //Assigned to the concrete group

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { "" });
            BackendTools.RunSchedulingProcedure();

            var currentTimeMinusOneMinute = DateTime.UtcNow.AddMinutes(-1);
            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(currentTimeMinusOneMinute));

            PredictiveTools.GetCallsForPredictive(surveySid, groupId, CallsSelectionAlgorithm.ByCampaign, 9);

            ServiceLocator.RegisterInstance<ITimeService>(new TimeService());

            var timeOfSecondRequest = DateTime.UtcNow;

            PredictiveTools.GetCallsForPredictive(surveySid, groupId, CallsSelectionAlgorithm.ByCampaign, 9);

            var table = ServiceLocator.Resolve<ISurveyCallDistributionService>().GetCallsSentToDialerDistribution(surveySid, timeOfSecondRequest, defaultTimeZoneId, out totalCount);

            CheckTable(table, 1, 2, new[] { new object[] { "*Survey Assignment*", 9 } });
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void CallsSentToDialerDistribution_Retrieve20Requests_CorrectDataAreReturned()
        {
            int totalCount;
            var date = new DateTime(2017, 06, 12, 12, 0, 0);

            var context = CreateContextAndGenerateDataInBvCallsSentToDialerTable(date);


            var table = ServiceLocator.Resolve<ISurveyCallDistributionService>().GetCallsSentToDialerDistribution(context.GetSurvey("S1").Id,
                date, defaultTimeZoneId, out totalCount);

            //need to test header and probably change from 19 columns to 20
            CheckTable(table, 2, 21, new[] { new object[] { "PG1", 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10 },
                                             new object[] { "PG2", 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10 }
                                             });

            for (int column = 1; column < table.Columns.Count; column++)
            {
                Assert.AreEqual(DateTime.Parse(table.Columns[column].ColumnName).ToString("mm:ss"), String.Format("{0:00}:00", column - 1));
            }
        }

        private static void CheckTable(DataTable table, int rowsCount, int columnsCount, object[][] expectedData)
        {
            Assert.AreEqual(rowsCount, table.Rows.Count, "Incorrect rows count");
            Assert.AreEqual(columnsCount, table.Columns.Count, "Incorrect columns count");

            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < expectedData[rowIndex].Length; columnIndex++)
                {
                    Assert.AreEqual(expectedData[rowIndex][columnIndex], table.Rows[rowIndex][columnIndex], "Incorrect count for Row: " + rowIndex + ", Column: " + columnIndex);
                }
            }
        }

        private TestDataContext CreateContextAndGenerateDataInBvCallsSentToDialerTable(DateTime date)
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, SchedulingScript = AllHoursSchedule.Name,
                    Assigns = new[]{"P1", "P2"},
                }},
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1", Name = "PG1"},
                    new PersonGroupData(){Tag="PG2", Name = "PG2"}
                },
                Persons = new[]{
                    new PersonData { Tag="P1", Memberships="PG1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag="P2", Memberships="PG2", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            BackendToolsObject.AddSample(survey.Model.Name, 2, (int)SchedulingMode.Simple, 1, 400, null,
                new int[] { context.GetPersonGroup("PG1").Id, context.GetPersonGroup("PG2").Id });

            BackendTools.RunSchedulingProcedure();
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var dialer = context.GetDialer("D1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            console1.Login();
            console1.LoginToDialer();

            var console2 = new AutomaticConsoleController(context, person2, survey);
            console2.Login();
            console2.LoginToDialer();

            GenerateDialerDistributionData(date, survey, dialer);
            return context;
        }

        private void GenerateDialerDistributionData(DateTime date, SurveyController survey, DialerController dialer)
        {
            var dateMocker = new DateTimeMocker(TestingFramework);

            dateMocker.MockDate(date);

            for (int i = 0; i < 20; i++)
            {
                dateMocker.MockDate(date.AddMinutes(i));
                dialer.RequestCalls(survey, 20, CallsSelectionAlgorithm.ByCampaign);
            }
        }

    }
}
