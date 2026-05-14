using System;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class HighPriorityCallsFiltering
    {

        private const int _nCallsPerGroup = 20;
        private const int argLow = 1;
        private const int argHigh = 300;

        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private int _timezoneId;
        private BackendTools _backendTools;
        private ISurveyStateService _surveyStateService;


        private const string SurveyPnumber = "p015365";
        private const string PersonName1 = "p1";
        private const string PersonName2 = "p2";
        private const string GroupName1 = "g1";
        private const string GroupName2 = "g2";

        private int SurveyId { get; set; }
        private int PersonId1 { get; set; }
        private int PersonId2 { get; set; }
        private int Group1 { get; set; }
        private int Group2 { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void NoPersonsLoggedIn_NoCallsReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample();

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            Assert.AreEqual(0, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_CallsForSurveyAndPersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample();
            BackendTools.LoginPerson(PersonId1, "ST001");
            
            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs( argLow,  argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual((1+1)*_nCallsPerGroup, totalCount);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_NoValidshifts_NoCallsReturned()
        {
            int totalCount;

            var time = new DateTime(2017, 02, 23, 1, 8, 0);
            new DateTimeMocker(_framework).MockDate(time);

            CreateSurvey2Persons2GroupsAddSample();
            CreateTestSchScript();
            BackendTools.LoginPerson(PersonId1, "ST001");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            Assert.AreEqual(0, actualRecordSet.Rows.Count);
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_NoValidShiftTypes_NoCallsReturned()
        {
            int totalCount;

            var time = new DateTime(2017, 02, 23, 1, 8, 0);
            new DateTimeMocker(_framework).MockDate(time);

            CreateSurvey2Persons2GroupsAddSample();
            CreateTestSchScript();
            var dbShiftTypeId = SurveyManager.GetShiftTypes(SurveyId).Find(x => x.Id == 1).ObjectId;
            BackendTools.LoginPerson(PersonId1, "ST001");
            var result = CallTools.ChangeCallsShiftType(SurveyId, 0, CallStates.Scheduled, dbShiftTypeId);
            Assert.IsTrue(result.Errors.Count == 0,  result.Errors.Count != 0 ? result.Errors[0].Message : String.Empty);
            BackendTools.RunSchedulingProcedure();

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            Assert.AreEqual(0, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_ValidShiftTypes_CallsReturned()
        {
            int totalCount;

            var time = new DateTime(2017, 02, 23, 2, 8, 0);
            new DateTimeMocker(_framework).MockDate(time);

            CreateSurvey2Persons2GroupsAddSample();
            CreateTestSchScript();
            var dbShiftTypeId = SurveyManager.GetShiftTypes(SurveyId).Find(x => x.Id == 1).ObjectId;
            BackendTools.LoginPerson(PersonId1, "ST001");
            var result = CallTools.ChangeCallsShiftType(SurveyId, 0, CallStates.Scheduled, dbShiftTypeId);
            Assert.IsTrue(result.Errors.Count == 0, result.Errors.Count != 0 ? result.Errors[0].Message : String.Empty);
            BackendTools.RunSchedulingProcedure();

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            Assert.AreEqual(40, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePersonInAutomaticMode_CallsForSurveyAndPersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode.Automatic);
            BackendTools.LoginPerson(PersonId1, "ST001");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual((1+1) * _nCallsPerGroup, totalCount);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePersonInAutomaticMode_ThereAreCallsWithDifferentShiftTypes_CallsForSurveyAndPersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode.Automatic);

            var dbShiftTypeId = SurveyManager.GetShiftTypes(SurveyId).Find(x => x.Id == 1).ObjectId;
            var result = CallTools.ChangeCallsShiftType(SurveyId, new int[] {1, 2, 3, 4, 5}, CallStates.Scheduled, dbShiftTypeId);
            Assert.IsTrue(result.Errors.Count == 0, result.Errors.Count != 0 ? result.Errors[0].Message : String.Empty);
            result = CallTools.ChangeCallsShiftType(SurveyId, new int[] { 6, 7, 8, 9, 10 }, CallStates.Scheduled, (int) CallShiftType.None);
            Assert.IsTrue(result.Errors.Count == 0, result.Errors.Count != 0 ? result.Errors[0].Message : String.Empty);

            BackendTools.LoginPerson(PersonId1, "ST001");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual((1 + 1) * _nCallsPerGroup, totalCount);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginTwoPersons_CallsForSurveyAnd2PersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample();
            BackendTools.LoginPerson(PersonId1, "ST001");
            BackendTools.LoginPerson(PersonId2, "ST002");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(3, groups.Count());
            Assert.AreEqual((2+1+1) * _nCallsPerGroup, totalCount);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_PersonIsMemberOfGroup_CallsForSurveyGroupAndPersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample();
            PersonService.SetParentGroups(PersonId1, new int[] {Group1});
            BackendTools.LoginPerson(PersonId1, "ST001");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);
            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(3, groups.Count());
            Assert.AreEqual((1+1+1) * _nCallsPerGroup, totalCount);
        }


        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_PersonIsMemberOfGroup_AssignedToSurveyViaGroup_CallsForSurveyGroupAndPersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample();
            BackendTools.DeassignCatiPersonFromSurvey(SurveyId, PersonId1);
            PersonService.SetParentGroups(PersonId1, new int[] { Group1 });
            BackendTools.AssignCatiPersonToSurvey(SurveyId, Group1);
            BackendTools.LoginPerson(PersonId1, "ST001");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);
            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(3, groups.Count());
            Assert.AreEqual((1+1+1) * _nCallsPerGroup, totalCount);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginTwoPersons_PersonsAreMembersOfGroup_CallsForSurvey2GroupsAnd2PersonReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample();
            PersonService.SetParentGroups(PersonId1, new int[] {Group1});
            PersonService.SetParentGroups(PersonId2, new int[] {Group2});

            BackendTools.LoginPerson(PersonId1, "ST001");
            BackendTools.LoginPerson(PersonId2, "ST002");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);
            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(5, groups.Count());
            Assert.AreEqual((2+1+1+1+1) * _nCallsPerGroup, totalCount); //2+1+1+1+1 - 2 users logged in to survey, then 1 for 2 groups and 2 users
        }
        
        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_FilterByPerson_OnlyPersonCallsReturned()
        {
            int totalCount = 0;


            CreateSurvey2Persons2GroupsAddSample();
            BackendTools.LoginPerson(PersonId1, "ST001");
            var searchArgs = SearchTools.SearchBy("Resource", SearchColumnType.Text, SearchOperator.Like, PersonName1);

            var actualRecordSet = CallHelper.GetCallsPage(SurveyId, null, _timezoneId, CallStates.HighPriority, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(1, groups.Count());
            Assert.AreEqual(1 * _nCallsPerGroup, totalCount);
            Assert.AreEqual(PersonName1, groups.First());
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginOnePerson_FilterByTelNumber_OneCallReturned()
        {
            int totalCount = 0;

            CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode.Automatic);
            BackendTools.LoginPerson(PersonId1, "ST001");
            var searchArgs = SearchTools.SearchBy("TelephoneNumber", SearchColumnType.Text, SearchOperator.Like, "21");

            var actualRecordSet = CallHelper.GetCallsPage(SurveyId, null, _timezoneId, CallStates.HighPriority, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
            Assert.AreEqual("21", actualRecordSet.Rows[0]["TelephoneNumber"]);
        }


        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginTwoPersons_BothMembersOfOneGroup_TwiceTheNumberCallsPerGroupReturned()
        {
            int totalCount = 0;

            CreateSurvey2Persons2GroupsAddSample();
            PersonService.SetParentGroups(PersonId1, new int[] {Group1});
            PersonService.SetParentGroups(PersonId2, new int[] {Group1});

            BackendTools.LoginPerson(PersonId1, "ST001");
            BackendTools.LoginPerson(PersonId2, "ST002");


            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(1, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(4, groups.Count());
            Assert.AreEqual((2+2+1+1) * _nCallsPerGroup, totalCount);

        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginTwoPersons_Group1HasLowPriorityCalls_LowPriorityCallsReturned()
        {
            int totalCount = 0;

            CreateSurvey2Persons2GroupsAddSample();
            PersonService.SetParentGroups(PersonId1, new int[] { Group1 });
            PersonService.SetParentGroups(PersonId2, new int[] { Group2 });

            BackendTools.LoginPerson(PersonId1, "ST001");
            BackendTools.LoginPerson(PersonId2, "ST002");
            CallTools.ChangeCallsPriority(SurveyId, 0, CallStates.Scheduled, 100); // for all calls
            CallTools.ChangeCallsPriority(SurveyId, 0, 5,
                new SearchParameterCollection() 
                    { new SearchParameter{ColumnName = "Resource", ColumnType = SearchColumnType.Text, Operator = SearchOperator.Equal, Value = "g1"}});


            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(1, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(5, groups.Count());
            Assert.AreEqual((2+1+1+1+1) * _nCallsPerGroup, totalCount);

            var groupRecords = actualRecordSet.AsEnumerable().Where(x =>x.Field<string>("Resource") == "g1");
            Assert.AreEqual(_nCallsPerGroup, groupRecords.Count());
            Assert.AreEqual(5, groupRecords.First()["Priority"] );
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void LoginPersonToDifferentSurvey_NoCallsForCurrentSurveyReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode.CampaignAssignment);
            var otherSurveyId = _backendTools.CreateSurvey(SurveyPnumber + "1");
            _surveyStateService.Open(otherSurveyId);
            BackendTools.LoginPerson(PersonId1, "ST001");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(PersonId1, otherSurveyId);

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(1, argHigh, "ID", true), out totalCount);

            Assert.AreEqual(0, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void Login2PersonToDifferentSurveys_OnlyCallsForCurrentSurveyReturned()
        {
            int totalCount;
            string otherSurveypNumber = SurveyPnumber + "1";

            CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode.CampaignAssignment);
            var otherSurveyId = _backendTools.CreateSurvey(otherSurveypNumber);
            _surveyStateService.Open(otherSurveyId);
            _backendTools.AddSample(otherSurveypNumber, 2, (int) SchedulingMode.Simple, 1, 50, null);
            BackendTools.LoginPerson(PersonId1, "ST001");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(PersonId1, otherSurveyId);
            BackendTools.LoginPerson(PersonId2, "ST002");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(PersonId2, SurveyId);

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(1, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual((1+1) * _nCallsPerGroup, totalCount);
        }
     
        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.HighPriorityCallsFiltering)]
        public void Login2Person_OneWithAutomatic_OtherSurveyAssignment_NoDuplicateCallsReturned()
        {
            int totalCount;

            CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode.CampaignAssignment);
            BackendTools.LoginPerson(PersonId1, "ST001");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(PersonId1, SurveyId);
            PersonTools.UpdatePersonMode(PersonId2, AgentTaskChoiceMode.Automatic);
            BackendTools.LoginPerson(PersonId2, "ST002");

            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.HighPriority, new RangingArgs(argLow, argHigh, "ID", true), out totalCount);

            var groups = actualRecordSet.AsEnumerable().Select(x => x.Field<string>("Resource")).Distinct();
            Assert.AreEqual(3, groups.Count());
            Assert.AreEqual((2+1+1) * _nCallsPerGroup, totalCount);
        }


        private void CreateSurvey2Persons2GroupsAddSample(AgentTaskChoiceMode mode = AgentTaskChoiceMode.Manual)
        {
            SurveyId = _backendTools.CreateSurvey(SurveyPnumber);
            _surveyStateService.Open(SurveyId);

            PersonId1 = PersonTools.CreatePerson(PersonName1, mode);
            PersonId2 = PersonTools.CreatePerson(PersonName2, mode);

            BackendTools.AssignCatiPersonToSurvey(SurveyId,PersonId1);
            BackendTools.AssignCatiPersonToSurvey(SurveyId, PersonId2);

            Group1 = PersonTools.CreateGroupAsRootChild(GroupName1);
            Group2 = PersonTools.CreateGroupAsRootChild(GroupName2);

            var allGroups = new int[] {SurveyId, PersonId1, PersonId2, Group1, Group2};
            _backendTools.AddSample(SurveyPnumber, 1, (int) SchedulingMode.Simple, 1, allGroups.Length*50, null, allGroups);
        }


        private void  CreateTestSchScript()
        {
            var testDay = (int) ServiceLocator.Resolve<ITimeService>().GetUtcNow().DayOfWeek;

            var script = new TestScript(
                new Action(Action.Operation.RecallAfterANumberOfMinutes, "60"),
                new Shift(1, 1, new ShiftTimezone(null, (new TimeSpan(testDay, 2, 0, 0)).ToString(),
                        (new TimeSpan(testDay, 3, 0, 0)).ToString())));
            _backendTools.LaunchScript(SurveyId, script);
        }
    }
}
