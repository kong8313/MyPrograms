using System;
using System.Collections.Generic;
using System.Globalization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Supervisor.Core.Assignment;
using ConfirmitDialerInterface;
using Confirmit.CATI.Supervisor.Core.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using System.Linq;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class PersonManagerTest : BaseMockedIntegrationTest
    {
        private IAssignmentManager _assignmentManager;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        }
        
        /// <summary>
        /// 1. Create person using PersonTools.CreatePerson method
        /// 2. Get person list using PersonManager.GetPersonsListPage method
        /// 3. Check that method return one person with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_GetPersonsListPage_Successfully()
        {
            const string personName = "TestPerson";
            PersonTools.CreatePerson(personName);

            const string secondPersonName = "TestPerson2";
            const string secondPersonGroup = "TestGroup2";
            const string thirdPersonGroup = "TestGroup3 \"New\"";
            PersonTools.CreatePerson(secondPersonName,
                null,
                AgentTaskChoiceMode.Manual,
                new[]
                {
                    PersonGroupService.RootGroupId,
                    PersonTools.CreatePersonGroup(secondPersonGroup),
                    PersonTools.CreatePersonGroup(thirdPersonGroup)
                },
                CallCenterTools.DefaultId);

            // Get sheduling script list
            var pagingArgs = new PagingArgs(
                1 /*PageIndex*/,
                20 /*PageSize*/,
                "PersonSID" /*SortedColumnKey*/,
                true /*SortIndicatorAsc*/);

            int totalCount;
            var dataList = PersonManager.GetPersonsListPage(PersonGroupService.RootGroupId.ToString(CultureInfo.InvariantCulture), pagingArgs, out totalCount);

            Assert.AreEqual(2, dataList.Count, "GetPersonsListPage return wrong person count: " + dataList.Count);

            Assert.AreEqual(personName, dataList[0].PersonName, "GetPersonsListPage return wrong person name: " + dataList[0].PersonName);
            var groups = JsonConvert.DeserializeObject<string[]>(dataList[0].GroupNamesJson);
            Assert.AreEqual(1, groups.Length);

            Assert.AreEqual(secondPersonName, dataList[1].PersonName, "GetPersonsListPage return wrong person name: " + dataList[1].PersonName);
            groups = JsonConvert.DeserializeObject<string[]>(dataList[1].GroupNamesJson);
            Assert.AreEqual(3, groups.Length);
            Assert.IsTrue(groups.Contains(secondPersonGroup));
            Assert.IsTrue(groups.Contains(thirdPersonGroup));
        }


        /// <summary>
        /// 1. Create group using PersonTools.CreatePersonGroup method
        /// 2. Get group list using PersonManager.GetPersonGroupsLevel method
        /// 3. Check that method return one group with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_GetPersonGroupsLevel_Successfully()
        {
            const string groupName = "TestGroup";

            // Create group
            PersonTools.CreatePersonGroup(groupName, new[] { PersonGroupService.RootGroupId });

            var dataList = PersonManager.GetPersonGroupsLevel(PersonGroupService.RootGroupId, String.Empty);

            Assert.AreEqual(1, dataList.Count, "CreatePersonGroup return wrong person group count: " + dataList.Count);
            Assert.AreEqual(groupName, dataList[0].Name, "CreatePersonGroup return wrong person group name: " + dataList[0].Name);
        }


        /// <summary>
        /// 1. Create person using PersonManager.CreateCatiPerson method        
        /// 2. Get person using PersonRepository.GetById method
        /// 3. Check that method return person with correct name and description
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_CreateCatiPerson_Successfully()
        {
            const string personName = "TestPerson";
            const string personDescription = "TestDescription";
            const string personDisplayName = "TestDisplayName";
            const AgentTaskChoiceMode personMode = AgentTaskChoiceMode.CampaignAssignment;
            const PersonAssignmentListMode personAssignmentListMode = PersonAssignmentListMode.AllCalls;
            var attributes = new []
            {
                "Attribute1",
                "Attribute2",
                "Attribute3",
                "Attribute4",
                "Attribute5"
            };
            
            // Create person
            BvPersonEntity personEntity = PersonService.CreateCatiPerson(CallCenterTools.DefaultId, personName, personDescription, personDisplayName, null, personMode, personAssignmentListMode, attributes: attributes);

            // Get person by id
            BvPersonEntity newPersonEntity = PersonRepository.GetById(personEntity.SID);

            Assert.AreEqual(personName, newPersonEntity.Name, "CreateCatiPerson return person with wrong name: " + personEntity.Name);
            Assert.AreEqual(personDescription, newPersonEntity.Description, "CreateCatiPerson return person with wrong description: " + personEntity.Description);
            Assert.AreEqual((int)personMode, newPersonEntity.ManualSelection, "CreateCatiPerson return person with wrong task choice: " + personEntity.ManualSelection);
            Assert.AreEqual((int)personAssignmentListMode, newPersonEntity.AssignmentsListMode, "CreateCatiPerson return person with wrong assignment list mode: " + personEntity.AssignmentsListMode);
            Assert.AreEqual(personDisplayName, newPersonEntity.FullName, "CreateCatiPerson return person with wrong display name: " + personEntity.FullName);
            Assert.AreEqual(attributes[0], newPersonEntity.Attribute1, "CreateCatiPerson return person with wrong attribute 1: " + personEntity.Attribute1);
            Assert.AreEqual(attributes[1], newPersonEntity.Attribute2, "CreateCatiPerson return person with wrong attribute 2: " + personEntity.Attribute2);
            Assert.AreEqual(attributes[2], newPersonEntity.Attribute3, "CreateCatiPerson return person with wrong attribute 3: " + personEntity.Attribute3);
            Assert.AreEqual(attributes[3], newPersonEntity.Attribute4, "CreateCatiPerson return person with wrong attribute 4: " + personEntity.Attribute4);
            Assert.AreEqual(attributes[4], newPersonEntity.Attribute5, "CreateCatiPerson return person with wrong attribute 5: " + personEntity.Attribute5);
            Assert.AreEqual(CallCenterTools.DefaultId, newPersonEntity.CallCenterID);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CreateCatiPerson_PersonAssignmentListModeIsNotSpecified_PersonIsCreatedWithAssignedOnlyMode()
        {
            const string personName = "TestPerson";
            const string personDescription = "TestDescription";

            // Create person
            BvPersonEntity personEntity = PersonService.CreateCatiPerson(CallCenterTools.DefaultId, personName, personDescription, "", null, AgentTaskChoiceMode.CampaignAssignment);

            // Get person by id
            BvPersonEntity newPersonEntity = PersonRepository.GetById(personEntity.SID);

            Assert.AreEqual((int)PersonAssignmentListMode.AssignedCallsOnly, newPersonEntity.AssignmentsListMode, "Person is created with wrong assignemt list mode: " + personEntity.AssignmentsListMode);
        }

        /// <summary>
        /// 1. Create person using PersonManager.CreateCatiPerson method        
        /// 2. Get person using PersonRepository.GetById method
        /// 3. Check that method return person with correct name and description
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_DeletePerson_Successfully()
        {
            const string personName = "TestPerson";
            const string personDescription = "TestDescription";

            // Create person
            BvPersonEntity personEntity = PersonService.CreateCatiPerson(CallCenterTools.DefaultId, personName, personDescription, "", null, AgentTaskChoiceMode.CampaignAssignment);

            var personRepository = ServiceLocator.Resolve<IPersonRepository>();

            // Delete person
            personRepository.Delete(personEntity.SID);

            // Get person by id
            BvPersonEntity newPersonEntity = personRepository.TryGetById(personEntity.SID);

            Assert.IsNull(newPersonEntity, "DeletePerson method doesn't work");
        }

        /// <summary>
        /// 1. Create person.
        /// 2. Log person in Cati console.
        /// 3. Try to delete person. 
        /// 4. PersonLoggedInException should be thrown.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(PersonLoggedInException))]
        public void DeletePerson_DeleteLoggedInPerson_ExceptionIsThrown()
        {
            var test = new TestCati2(false, false, BackendToolsObject);
            const string user = "testUser";
            const string password = "password";

            test.CreatePerson(user, password, AgentTaskChoiceMode.Automatic);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, false);

            PersonManager.DeletePersons(new List<int> { test.PersonSID });
        }

        /// <summary>
        /// 1. Create person using PersonManager.CreateCatiPerson method        
        /// 2. Call IsPersonNameUsed method for used and unusable person name
        /// 3. Check that method return right values
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_IsPersonNameUsed_Successfully()
        {
            const string personName = "TestPerson";
            const string personDescription = "TestDescription";

            // Create person
            PersonService.CreateCatiPerson(CallCenterTools.DefaultId, personName, personDescription, "", null, AgentTaskChoiceMode.CampaignAssignment);

            // Call IsPersonNameUsed for used and unusable person names
            bool usedPersonName = PersonManager.IsPersonNameUsed(personName);
            bool unusablePersonName = PersonManager.IsPersonNameUsed(personName + "_Temp");

            Assert.IsTrue(usedPersonName, "IsPersonNameUsed return false for used person name");
            Assert.IsFalse(unusablePersonName, "IsPersonNameUsed return true for unusable person name");
        }


        /// <summary>
        /// 1. Create person using PersonTools.CreatePerson method        
        /// 2. Call LookupPersonName method for used and unusable person name
        /// 3. Check that method return personSID for used person and 0 for unusable person
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_LookupPersonName_Successfully()
        {
            const string personName = "TestPerson";

            // Create person
            int personSID = PersonTools.CreatePerson(personName, "", AgentTaskChoiceMode.CampaignAssignment, new[] { PersonGroupService.RootGroupId });

            // Call LookupPersonName for used and unusable person names
            int usedPersonId = PersonManager.LookupPersonName(personName);
            int unusablePersonId = PersonManager.LookupPersonName(personName + "_Temp");

            Assert.AreEqual(personSID, usedPersonId, "LookupPersonName return wrong person id for used person: " + usedPersonId);
            Assert.AreEqual(0, unusablePersonId, "LookupPersonName return wrong person id for unusable person: " + unusablePersonId);
        }


        /// <summary>
        /// 1. Create person using PersonTools.CreatePerson method        
        /// 2. Get persons using PersonRepository.GetAllPersons method
        /// 3. Check that method return one person with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_GetAllPersons_Successfully()
        {
            const string personName = "TestPerson";
            int defaultGroupName = PersonGroupService.RootGroupId;

            // Create person            
            PersonTools.CreatePerson(personName, "", AgentTaskChoiceMode.CampaignAssignment, new[] { defaultGroupName });

            // Get all persons
            var persons = PersonManager.GetAllPersons(defaultGroupName, CallCenterTools.DefaultId);

            Assert.AreEqual(1, persons.Count, "GetAllPersons return wrong person count: " + persons.Count);
            Assert.AreEqual(personName, persons[0].Name, "GetAllPersons return person with wrong name: " + persons[0].Name);
        }


        /// <summary>
        /// 1. Create group using PersonTools.CreatePersonGroup method
        /// 2. Get group list using PersonManager.GetPersonGroupsLevel method
        /// 3. Check that method return one group with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_GetPersonsLevel_Successfully()
        {
            const string personName = "TestPerson";

            // Create person
            PersonTools.CreatePerson(personName, "", AgentTaskChoiceMode.CampaignAssignment, new[] { PersonGroupService.RootGroupId });

            // Get persons
            var persons = PersonManager.GetPersonsLevel(PersonGroupService.RootGroupId, String.Empty);

            Assert.AreEqual(1, persons.Count, "GetPersonsLevel return wrong person count: " + persons.Count);
            Assert.AreEqual(personName, persons[0].Name, "GetPersonsLevel return person with wrong name: " + persons[0].Name);
        }


        /// <summary>
        /// 1. Create person group using PersonTools.CreatePersonGroup method        
        /// 2. Get person groups using PersonManager.GetPersonGroups method
        /// 3. Check that method return two person groups (default and new) with correct ids
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_GetPersonGroups_Successfully()
        {
            const string groupName = "TestGroup";
            int defaultGroupName = PersonGroupService.RootGroupId;

            // Create person group
            int personGroupID = PersonTools.CreatePersonGroup(groupName, new[] { defaultGroupName });

            // Get person groups
            var personGroups = PersonManager.GetPersonGroups(PersonGroupService.RootGroupId);

            Assert.AreEqual(2, personGroups.Count, "GetPersonGroups return wrong person group count: " + personGroups.Count);
            Assert.AreEqual(defaultGroupName, personGroups[0].Id, "GetPersonGroups return person group with wrong default id: " + personGroups[0].Id);
            Assert.AreEqual(personGroupID, personGroups[1].Id, "GetPersonGroups return person group with wrong id: " + personGroups[1].Id);
        }


        /// <summary>
        /// 1. Create person group using PersonTools.CreatePersonGroup method        
        /// 2. Delete person group using PersonManager.DeletePersonGroup method  
        /// 3. Get person groups using PersonManager.GetPersonGroups method
        /// 4. Check that method return one person group (default) with correct id
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_DeletePersonGroup_Successfully()
        {
            const string groupName = "TestGroup";
            int defaultGroupName = PersonGroupService.RootGroupId;

            // Create person group
            int personGroupID = PersonTools.CreatePersonGroup(groupName, new[] { defaultGroupName });

            // Delete person group
            PersonManager.DeletePersonGroup(personGroupID);

            // Get person groups
            var personGroups = PersonManager.GetPersonGroups(PersonGroupService.RootGroupId);

            Assert.AreEqual(1, personGroups.Count, "DeletePersonGroup method didn't delete person group");
            Assert.AreEqual(defaultGroupName, personGroups[0].Id, "DeletePersonGroup method has deleted wrong person group");
        }


        /// <summary>
        /// 1. Create person using PersonTools.CreatePerson method
        /// 2. Create administrative and not administrative groups 
        /// 3. Get persons and person groups using PersonManager.GetCatiPersonAndGroupList method with getAdministrativeGroups param = true
        /// 4. Check that method return one person and 2 person groups 1 not administrative group which we created and default group with correct ids
        /// 5. Get persons and person groups using PersonManager.GetCatiPersonAndGroupList method with getAdministrativeGroups param = false
        /// 6. Check that method return one person and 3 person groups 2 which we created and default group with correct ids
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_GetCatiPersonAndGroupList_Successfully()
        {
            const string personName = "TestPerson";
            int defaultGroupName = PersonGroupService.RootGroupId;

            // Create person            
            int personSID = PersonTools.CreatePerson(personName, "", AgentTaskChoiceMode.CampaignAssignment, new[] { defaultGroupName });

            var group1 = PersonTools.CreatePersonGroup("PG1", false);
            var group2 = PersonTools.CreatePersonGroup("PG2", true);

            // Get persons and person groups
            var expected = new[] { personSID, defaultGroupName, group1, group2 };

            var personsAndGroups = PersonManager.GetAllPersonsAndGroups(includeAdministrativeGroups: true);

            Assert.AreEqual(4, personsAndGroups.Count, "GetCatiPersonAndGroupList return wrong persons and person groups count: " + personsAndGroups.Count);
            CollectionAssert.AreEquivalent(expected, personsAndGroups.Select(x => x.Id).ToList());

            personsAndGroups = PersonManager.GetAllPersonsAndGroups(includeAdministrativeGroups: false);

            Assert.AreEqual(3, personsAndGroups.Count, "GetCatiPersonAndGroupList return wrong persons and person groups count: " + personsAndGroups.Count);

            expected = new[] { personSID, defaultGroupName, group1 };
            CollectionAssert.AreEquivalent(expected, personsAndGroups.Select(x => x.Id).ToList());
        }


        /// <summary>
        /// 1. Create person group using PersonTools.CreatePersonGroup method                
        /// 2. Call IsPersonGroupNameUsed for used and unusable person group names
        /// 3. Check that method return right values
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_IsPersonGroupNameUsed_Successfully()
        {
            const string groupName = "TestGroup";

            // Create person group
            PersonTools.CreatePersonGroup(groupName, new[] { PersonGroupService.RootGroupId });

            // Call IsPersonGroupNameUsed for used and unusable person group names
            bool usedPersonGroupName = PersonManager.IsPersonGroupNameUsed(groupName);
            bool unusablePersonGroupName = PersonManager.IsPersonGroupNameUsed(groupName + "_Temp");

            Assert.IsTrue(usedPersonGroupName, "IsPersonGroupNameUsed return false for used person group name");
            Assert.IsFalse(unusablePersonGroupName, "IsPersonGroupNameUsed return true for unusable person group name");
        }



        /// <summary>        
        /// 1. Create person using PersonTools.CreatePerson method
        /// 2. Change person mode using PersonManager.ChangeTaskChoice method
        /// 3. Get person using PersonRepository.GetById method
        /// 4. Check that person mode was changed successfully
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void PersonManagerTest_ChangeTaskChoice_Successfully()
        {
            const string personName = "TestPerson";
            int defaultGroupName = PersonGroupService.RootGroupId;

            // Create person            
            int personSID = PersonTools.CreatePerson(personName, "", AgentTaskChoiceMode.Manual, new[] { defaultGroupName });

            // Change person mode
            PersonService.ChangeTaskChoice(new[] { personSID }, AgentTaskChoiceMode.Automatic, null, true);

            // Get person
            var person = PersonRepository.GetById(personSID);

            Assert.AreEqual((int)AgentTaskChoiceMode.Automatic, person.ManualSelection, "ChangeTaskChoice didn't change person mode");
        }

        /// <summary>
        /// 1. Create Survey
        /// 2. Create Group g1
        /// 3. Assign Group g1 on survey
        /// 4. Create Group g2
        /// 5. Make g2 Child g1
        /// 6. Make surre g2 not assigned on the survey
        /// </summary>
        [TestMethod, Owner(@"FIRM\EgorS"), Bug(38264)]
        public void PersonManagerTest_CreateCatiPersonGroup_CreateChildPersonsGroup_NotAssigned()
        {
            var surveySID = BackendToolsObject.CreateSurvey("p0000111");

            var catiInterviewersGroupSID = PersonGroupService.RootGroupId;

            var g1GroupSID = PersonManager.CreatePersonGroup(new BvPersonGroupEntity(){Name = "g1"}, new[] { catiInterviewersGroupSID });
            var g1P1Person = PersonService.CreateCatiPerson(CallCenterTools.DefaultId, "g1_p1", "", "", null, AgentTaskChoiceMode.Automatic);
            PersonService.SetParentGroups(g1P1Person.SID, new[] { g1GroupSID });
            BackendTools.AssignCatiPersonToSurvey(surveySID, g1GroupSID);

            var g2GroupSID = PersonManager.CreatePersonGroup(new BvPersonGroupEntity() { Name = "g2" }, new[] { g1GroupSID });
            var g2P1Person = PersonService.CreateCatiPerson(CallCenterTools.DefaultId, "g2_p1", "", "", null, AgentTaskChoiceMode.Automatic);
            PersonService.SetParentGroups(g2P1Person.SID, new[] { g2GroupSID });

            // Needed to call BvSpPerson_GetAssignedSurveyList
            BvUserSurveyPermissionAdapter.Insert(new BvUserSurveyPermissionEntity{SurveySID = surveySID, UserName = "TestUser"});

            var surveysForGroupG1 = _assignmentManager.GetAssignedSurveyList(g1GroupSID, "TestUser");
            var surveysForGroupG2 = _assignmentManager.GetAssignedSurveyList(g2GroupSID, "TestUser");

            Assert.AreEqual(surveysForGroupG1.Count, 1);
            Assert.AreEqual(surveysForGroupG2.Count, 0);
        }
    }
}
