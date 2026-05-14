using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.PersonGroups;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class PersonServiceTest
    {
        private const string Description = "description";
        private const string Password = "password";
        private const string PersonName = "person";
        private int _surveySID;
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private IPersonService _personService;
        private IPersonGroupRepository _personGroupRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
            _personService = ServiceLocator.Resolve<IPersonService>();
            _personGroupRepository = ServiceLocator.Resolve<IPersonGroupRepository>();
            
            _surveySID = _backendTools.CreateSurvey("p0000123");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Bug(51359), Owner("SvetlanaT")]
        public void CreateOrUpdatePerson_ChoiceMode_AutomaticSurveyWasSet()
        {
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, 0, PersonName, Description, "", Password, AgentTaskChoiceMode.Choice, PersonAssignmentListMode.AssignedCallsOnly, 
                TaskChoicePermissions.SurveyAssignment, new List<int>(), _surveySID, 0, null, DialType.Landline, AgentType.LiveAgent);

            var actualAutomaticSurveyId = PersonRepository.GetByName(PersonName).AutomaticSurveyID;

            Assert.AreEqual(_surveySID, actualAutomaticSurveyId);
        }

        [TestMethod, Bug(51359), Owner("SvetlanaT")]
        public void CreateOrUpdatePerson_ChoiceModeAndNullAutomaticSurvey_AutomaticSurveyWasCleared()
        {
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, 0, PersonName, Description, "", Password, AgentTaskChoiceMode.Choice, PersonAssignmentListMode.AssignedCallsOnly, 
                TaskChoicePermissions.SurveyAssignment, new List<int>(), _surveySID, 0, null, DialType.Landline, AgentType.LiveAgent);
            var personId = PersonRepository.GetByName(PersonName).SID;
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, personId, PersonName, Description, "", Password, AgentTaskChoiceMode.Choice, PersonAssignmentListMode.AssignedCallsOnly, 
                TaskChoicePermissions.SurveyAssignment, new List<int>(), null, 0, null, DialType.Landline, AgentType.LiveAgent);

            var actualAutomaticSurveyId = PersonRepository.GetByName(PersonName).AutomaticSurveyID;

            Assert.AreEqual(null, actualAutomaticSurveyId);
        }

        [TestMethod, Bug(51359), Owner("SvetlanaT")]
        public void CreateOrUpdatePerson_SurveyAssignmentMode_AutomaticSurveyWasSet()
        {
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, 0, PersonName, Description, "", Password, AgentTaskChoiceMode.CampaignAssignment, PersonAssignmentListMode.AssignedCallsOnly,
                null, new List<int>(), _surveySID, 0, null, DialType.Landline, AgentType.LiveAgent);

            var actualAutomaticSurveyId = PersonRepository.GetByName(PersonName).AutomaticSurveyID;

            Assert.AreEqual(_surveySID, actualAutomaticSurveyId);
        }

        [TestMethod, Bug(51359), Owner("SvetlanaT")]
        public void CreateOrUpdatePerson_SurveyAssignmentModeAndNullAutomaticSurvey_AutomaticSurveyWasCleared()
        {
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, 0, PersonName, Description, "", Password, AgentTaskChoiceMode.CampaignAssignment, PersonAssignmentListMode.AssignedCallsOnly, 
                TaskChoicePermissions.SurveyAssignment, new List<int>(), _surveySID, 0, null, DialType.Landline, AgentType.LiveAgent);
            var personId = PersonRepository.GetByName(PersonName).SID;
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, personId, PersonName, Description, "", Password, AgentTaskChoiceMode.CampaignAssignment, PersonAssignmentListMode.AssignedCallsOnly, 
                TaskChoicePermissions.SurveyAssignment, new List<int>(), null, 0, null, DialType.Landline, AgentType.LiveAgent);

            var actualAutomaticSurveyId = PersonRepository.GetByName(PersonName).AutomaticSurveyID;

            Assert.AreEqual(null, actualAutomaticSurveyId);
        }

        [TestMethod, Owner("DenisM")]
        public void CreateOrUpdatePerson_DialType_ManualDialTypeWasSet()
        {
            _personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, 0, PersonName, Description, "", Password, AgentTaskChoiceMode.Choice, PersonAssignmentListMode.AssignedCallsOnly,
                TaskChoicePermissions.SurveyAssignment, new List<int>(), _surveySID, 0, null, DialType.Cellphone, AgentType.LiveAgent);

            var actualDialType = PersonRepository.GetByName(PersonName).DialTypeId;

            Assert.AreEqual((byte)DialType.Cellphone, actualDialType);
        }
        
        [TestMethod, Owner("EgorK")]
        public void CreateOrUpdatePerson_SetParentGroups_AdministrativeGroupsAreNotFilledInBvPersonRel()
        {
            var group1Id = _personGroupRepository.Insert(new BvPersonGroupEntity() {
                Name = "PG1",
                Description = "PG1",
                IsAdministrative = false
            });
            var group2Id = _personGroupRepository.Insert(new BvPersonGroupEntity() {
                Name = "PG2",
                Description = "PG2",
                IsAdministrative = true
            });
            
            var personId =_personService.CreateOrUpdatePerson(
                CallCenterTools.DefaultId, 0, PersonName, Description, "", Password, AgentTaskChoiceMode.Choice, PersonAssignmentListMode.AssignedCallsOnly,
                TaskChoicePermissions.SurveyAssignment, new List<int>() { group1Id, group2Id }, _surveySID, 0, null, DialType.Cellphone, AgentType.LiveAgent);
            
            var groups = BvSpGetUserGroupsAdapter.ExecuteEntityList(personId);
            
            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(group1Id, groups[0].GroupSID);
        }
    }
}
