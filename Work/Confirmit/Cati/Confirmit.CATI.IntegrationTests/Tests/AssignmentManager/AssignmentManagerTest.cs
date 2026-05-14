using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Framework.Wrappers;
using Confirmit.CATI.Supervisor.Core.Assignment;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.AssignmentManager
{
    [TestClass]
    public class AssignmentManagerTest : BaseMockedIntegrationTest
    {
        private IAssignmentManager _assignmentManager;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        }

        /// <summary>
        /// 1. Create survey survey1.
        /// 2. Create group group1.
        /// 3. Assign group to survey1.
        /// 4. Create group group2 with parent group1.
        /// 5. Call method GetAssignmentSurveyList2 for group2.
        /// 6. The methos should return empty collection.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetAssignedSurveyListExtended_GetAssignmentForGroupWithoutHierarchy_Success()
        {
            const string surveyName = "p000001";
            const string userName = "grigoryk";
            const string group1Name = "group1";
            const string group2Name = "group2";

            // Create survey
            int surveySid = BackendToolsObject.CreateSurvey(surveyName);

            // Create groups
            int group1 = PersonTools.CreatePersonGroup(group1Name);
            int group2 = PersonTools.CreatePersonGroup(group2Name, new[] { group1 });

            // Assign group1 to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, group1);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);

            var dataList = _assignmentManager.GetPersonAssignments(group2, userName, CallCenterTools.DefaultId);

            Assert.AreEqual(0, dataList.Count, "Should be returned empty collection");
        }

        /// <summary>
        /// 1. Create survey survey1.
        /// 2. Create group group1.
        /// 3. Create person and add it to group1.
        /// 4. Assign group1 to survey1.
        /// 5. Assign person to survey1.
        /// 6. Get survey list using AssignmentManager.GetAssignedSurveyListExtended method for person.
        /// 7. Check that method return 2 records. Check assignment type for that records. Record with parent group name
        /// should have assignment type 0, record with empty parent group name should have 1.
        /// </summary>
        [TestMethod, Owner(@"FIRM\Sergeyc")]
        [Bug(39218)]
        public void GetAssignedSurveyListExtended_CheckAssignmentType_Success()
        {
            const string surveyName = "p000001";
            const string personName = "TestPerson";
            const string userName = "grigoryk";
            const string group1Name = "group1";

            // Create survey
            int surveySid = BackendToolsObject.CreateSurvey(surveyName);

            // Create groups
            int group1 = PersonTools.CreatePersonGroup(group1Name);

            // Create person
            int personSid = PersonTools.CreatePerson(personName, "pass", AgentTaskChoiceMode.Manual, new[] { group1 });

            // Assign groups to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, group1);
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);

            var dataList = _assignmentManager.GetPersonAssignments(personSid, userName, CallCenterTools.DefaultId);

            Assert.AreEqual(2, dataList.Count, "GetAssignedSurveyListExtended returns wrong record count: " + dataList.Count);

            var personAssignment = (from assignment in dataList
                                    where String.IsNullOrEmpty(assignment.ParentGroupName)
                                    select assignment).SingleOrDefault();

            Assert.IsNotNull(personAssignment, "Explicit person assignment not found");
            Assert.AreEqual(1, personAssignment.AssignmentType, "Wrong assignment type for person explicit assignment");

            var groupAssignment = (from assignment in dataList
                                   where assignment.ParentGroupName == group1Name
                                   select assignment).SingleOrDefault();

            Assert.IsNotNull(groupAssignment, "Implicit person assignment by group not found");
            Assert.AreEqual(0, groupAssignment.AssignmentType, "Wrong assignment type for person implicit assignment by group");
        }

        /// <summary>
        /// 1. Create survey survey1.
        /// 2. Create group group1.
        /// 3. Create person and add it to group1.
        /// 4. Assign group1 to survey1.
        /// 5. Assign person to call of survey1.
        /// 6. Start scheduling procedure.
        /// 7. Get list of surveys not assigned to person calling method AssignmentManager.GetNotAssignedSurveysList.
        /// 8. Check that method returns survey1.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Bug(39218)]
        public void GetNotAssignedSurveysList_ExcludeOnlyExplicitAssignmentsForPerson()
        {
            const string surveyName = "p0000001";
            const string personName = "TestPerson";
            const string userName = "grigoryk";
            const string group1Name = "group1";

            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            // Create survey
            int surveySid = FilterAndPagingToolsObject.CreateSurveyWithSample(
                surveyName,
                FilterAndPagingTools.SampleType.SmallSample);

            // Create groups
            int group1 = PersonTools.CreatePersonGroup(group1Name);

            // Create person
            int personSid = PersonTools.CreatePerson(personName, "pass", AgentTaskChoiceMode.Manual, new[] { group1 });

            // Assign groups to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, group1);

            // Assign person to call
            var calls = new BvCallEntity
            {
                InterviewID = 1,
                SurveySID = surveySid,
                CallState = 2,
                ShiftID = (int)CallShiftType.None,
                Priority = 1
            };
            CallQueueService.AddCall(calls, 0, 0);
            CallTools.AssignCalls(surveySid, new[] { 1 }, personSid);

            // Set access
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);

            var dataList = _assignmentManager.GetNotAssignedSurveysList(personSid, userName, false);

            Assert.AreEqual(1, dataList.Count, "Wrong number of returned surveys");
            Assert.AreEqual(surveySid, dataList.Single().Id, "Wrong survey");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Bug(74480)]
        public void GetAssignedInterviewersAndGroupsList_ExplicitInterviewerAssignmentsInDifferentCallCenter_ReturnEmptyCollection()
        {
            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            // Create survey
            int surveySid = BackendToolsObject.CreateSurvey("p123456");

            // Create a person
            var personId = PersonTools.CreatePerson("user1");

            // Create a call and assign it to the person
            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);
            var call = new BvCallEntity
            {
                InterviewID = interview.ID,
                SurveySID = surveySid,
                CallState = 2,
                ShiftID = (int)CallShiftType.None,
                Priority = 1
            };
            CallQueueService.AddCall(call, 0, interview.TransientState);
            call = CallQueueService.GetCallAndNoLock(surveySid, interview.ID);
            CallTools.AssignCalls(surveySid, new[] { call.CallID }, personId);

            // Create a call center
            var callCenter = CallCenterTools.Create();

            // Assign the survey to the second call center
            ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(callCenter.ID, surveySid);

            // Ask for survey assignment in second call center
            var result = CallCenterWrapper.DoCall(callCenter.ID,
                () => _assignmentManager.GetAssignedInterviewersAndGroupsList(surveySid));

            Assert.AreEqual(0, result.Count, "None of assignments should be returned for different call center");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        [Bug(75729)]
        public void GetAssignedInterviewersAndGroupsList_ExplicitInterviewerAssignmentsOnEnabledAndDisabledCalls_ResultIsCorrect()
        {
            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            // Create survey
            int surveySid = BackendToolsObject.CreateSurvey("p123456");

            // Create a person
            var personId = PersonTools.CreatePerson("user1");

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            CallTools.EnableCalls(surveySid, false, new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID });
            CallTools.AssignCalls(surveySid, new[] { interviews[2].ID, interviews[3].ID, interviews[4].ID }, personId);

            var assigments = _assignmentManager.GetAssignedInterviewersAndGroupsList(surveySid);

            Assert.AreEqual(1, assigments.Count);
            Assert.AreEqual(personId, assigments[0].SID);
            Assert.AreEqual(false, assigments[0].IsGroup);
            Assert.AreEqual(3, assigments[0].AssignedCallsCount);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        [Bug(75729)]
        public void DeassignResourceFromSurvey_ExplicitInterviewerAssignmentsOnEnabledAndDisabledCalls_AllAssignmentsAreDeleted()
        {
            // run scheduling
            BackendToolsObject.LaunchAllHoursScript();

            // Create survey
            int surveySid = BackendToolsObject.CreateSurvey("p123456");

            // Create a person
            var personId = PersonTools.CreatePerson("user1");

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            CallTools.EnableCalls(surveySid, false, new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID });
            CallTools.AssignCalls(surveySid, new[] { interviews[2].ID, interviews[3].ID, interviews[4].ID }, personId);

            BackendTools.DeassignCatiPersonFromSurveyCalls(surveySid, personId);

            var assigments = _assignmentManager.GetAssignedInterviewersAndGroupsList(surveySid);

            Assert.AreEqual(0, assigments.Count);
        }

        [TestMethod]
        public void GetPersonAssignments_InterviewerAssignmentToCalls_ReturnCorrectCallsCount()
        {
            // arrange
            const string surveyName = "p000001";
            const string userName = "grigoryk";

            int surveySid = BackendToolsObject.CreateSurvey(surveyName);
            int personId = PersonTools.CreatePerson(userName);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            // act
            CallTools.EnableCalls(surveySid, false, new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID }); // disabled call not included in result
            CallTools.AssignCalls(surveySid, new[] { interviews[2].ID, interviews[3].ID, interviews[4].ID }, personId);
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);
            BackendTools.RunSchedulingProcedure();

            // assert
            var personassigments = _assignmentManager.GetPersonAssignments(personId, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, personassigments.Count);
            Assert.AreEqual(2, personassigments[0].AssignedCallsCount);
            Assert.AreEqual(userName, personassigments[0].ParentGroupName);
            Assert.AreEqual(AssignmentType.ImplicitToSurveyCalls, (AssignmentType)personassigments[0].AssignmentType);
        }

        [TestMethod]
        public void GetPersonAssignments_GroupAssignmentToCalls_ReturnCorrectCallsCount()
        {
            // arrange
            const string surveyName = "p000001";
            const string userName = "grigoryk";
            const string group1Name = "group1";
            int surveySid = BackendToolsObject.CreateSurvey(surveyName);
            int group1 = PersonTools.CreatePersonGroup(group1Name);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            // act
            CallTools.EnableCalls(surveySid, false, new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID }); // disabled call not included in result
            CallTools.AssignCalls(surveySid, new[] { interviews[2].ID, interviews[3].ID, interviews[4].ID }, group1);
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);
            BackendTools.RunSchedulingProcedure();

            // assert
            var assigments = _assignmentManager.GetPersonAssignments(group1, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assigments.Count);
            Assert.AreEqual(2, assigments[0].AssignedCallsCount);
            Assert.AreEqual(group1Name, assigments[0].ParentGroupName);
            Assert.AreEqual(AssignmentType.ImplicitToSurveyCalls, (AssignmentType)assigments[0].AssignmentType);
        }

        [TestMethod]
        public void GetPersonAssignments_ImplicitPersonAssignments_ReturnCorrectCallsCount()
        {
            // arrange
            const string surveyName = "p000001";
            const string userName = "grigoryk";

            int surveySid = BackendToolsObject.CreateSurvey(surveyName);
            int personId = PersonTools.CreatePerson(userName);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            // act
            BackendTools.AssignCatiPersonToSurvey(surveySid, personId);
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);
            BackendTools.RunSchedulingProcedure();

            // assert
            var personassigments = _assignmentManager.GetPersonAssignments(personId, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, personassigments.Count);
            Assert.AreEqual(0, personassigments[0].AssignedCallsCount); // all calls means 0
            Assert.AreEqual(string.Empty, personassigments[0].ParentGroupName);
            Assert.AreEqual(1, personassigments[0].AssignmentType);
        }

        [TestMethod]
        public void GetPersonAssignments_GroupAssignmentToSurvey_ReturnCorrectCallsCount()
        {
            // arrange
            const string surveyName = "p000001";
            const string userName = "grigoryk";
            const string group1Name = "group1";
            int surveySid = BackendToolsObject.CreateSurvey(surveyName);
            int group1 = PersonTools.CreatePersonGroup(group1Name);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            // act
            BackendTools.AssignCatiPersonToSurvey(surveySid, group1);
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);
            BackendTools.RunSchedulingProcedure();

            // assert
            var assigments = _assignmentManager.GetPersonAssignments(group1, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assigments.Count);
            Assert.AreEqual(0, assigments[0].AssignedCallsCount); // all calls means 0
            Assert.AreEqual(string.Empty, assigments[0].ParentGroupName);
            Assert.AreEqual(AssignmentType.Explicit, (AssignmentType)assigments[0].AssignmentType);
        }

        [TestMethod]
        public void GetPersonAssignments_MultipleGroupAssignmentToCalls_ReturnCorrectCallsCount()
        {
            // arrange
            const string surveyName = "p000001";
            const string userName = "grigoryk";
            const string group1Name = "group1";
            const string group2Name = "group2";
            const string personName = "testPerson";
            int surveySid = BackendToolsObject.CreateSurvey(surveyName);
            int group1 = PersonTools.CreatePersonGroup(group1Name);
            int group2 = PersonTools.CreatePersonGroup(group2Name);

            int personSid = PersonTools.CreatePerson(personName, "pass", AgentTaskChoiceMode.Manual, new[] { group1, group2 });

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveySid, 5, out interviews, out calls);

            // act
            CallTools.EnableCalls(surveySid, false, new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID }); // disabled call not included in result
            CallTools.AssignCalls(surveySid, new[] { interviews[2].ID, interviews[3].ID, interviews[4].ID }, new[] { group1, group2 });
            new ManagementService().UpdateSurveyAccessList(userName, surveyName, true);
            BackendTools.RunSchedulingProcedure();

            // assert
            var assigments = _assignmentManager.GetPersonAssignments(personSid, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assigments.Count);
            Assert.AreEqual(2, assigments[0].AssignedCallsCount);
            Assert.AreEqual(string.Join(",", group1Name, group2Name), assigments[0].ParentGroupName);
            Assert.AreEqual(AssignmentType.ImplicitToSurveyCalls, (AssignmentType)assigments[0].AssignmentType);
        }

        [TestMethod]
        public void GetPersonAssignments_ImplicitAssignmentsOnCall_ResultIsCorrectAfterRemovingOfAssignment()
        {
            string userName = "admin";
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData() {Resource = "P1"}},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData() {Resource = "P1"}},
                        }
                    }
                },
                Persons = new[] {new PersonData() { Tag="P1"} }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var callCenter = ServiceLocator.Resolve<ICallCenterRepository>().Default;

            new ManagementService().UpdateSurveyAccessList(userName, survey.Model.Name, true);

            var assigments = _assignmentManager.GetPersonAssignments(person.Id, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assigments.Count, "Wrong count of assignments");

            ServiceLocator.Resolve<IAssignmentService>()
                .DeassignResourcesFromSurveyCalls(survey.Id, new[] {person.Id}, callCenter.ID);

            assigments = _assignmentManager.GetPersonAssignments(person.Id, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(0, assigments.Count, "Wrong count of assignments");
        }
        
        [TestMethod]
        public void GetPersonAssignmentsDoesNotReturnAssignmentsForSoftDeletedSurveys()
        {
            string userName = "admin";
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", Call = new CallData() { Resource = "P1" } },
                            new InterviewData() { Tag = "S1.I2", Call = new CallData() { Resource = "P1" } },
                        },
                        Assigns = new[] { "P1", "PG1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1" , Memberships = "PG1" } },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var callCenter = ServiceLocator.Resolve<ICallCenterRepository>().Default;

            new ManagementService().UpdateSurveyAccessList(userName, survey.Model.Name, true);

            var assigments = _assignmentManager.GetPersonAssignments(person.Id, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(3, assigments.Count, "Wrong count of assignments");
             
            new ManagementService().SoftDeleteSurvey(survey.Model.ProjectId);

            assigments = _assignmentManager.GetPersonAssignments(person.Id, userName, CallCenterTools.DefaultId);
            Assert.AreEqual(0, assigments.Count, "Wrong count of assignments");
        }
    }
}
