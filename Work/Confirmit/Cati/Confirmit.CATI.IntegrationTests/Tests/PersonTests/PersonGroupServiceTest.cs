using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Core.PersonGroups;
using Confirmit.CATI.Supervisor.Core.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class PersonGroupServiceTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void DeletePersonGroup_CallAssignmentsAreDeleted_PersonRelDeleted()
        {
            var context = new TestData {
                Surveys = new[] {
                    new SurveyData {
                        Tag = "S1",
                        Interviews = new[] { new InterviewData { Tag = "S1.I1", Call = new CallData() { Resource = "PG1,PG2" } } }
                    }
                },
                PersonGroups = new[] {
                    new PersonGroupData { Tag = "PG1", Name = "PersonGroup1" },
                    new PersonGroupData { Tag = "PG2", Name = "PersonGroup2" }
                },
                Persons = new PersonData[] {
                    new PersonData() { Tag = "P1", Memberships = "PG1,PG2" }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var group1 = context.GetResource("PG1");
            var group2 = context.GetResource("PG2");
            var personId = context.GetPerson("P1").Id;

            var groups = BvSpGetUserGroupsAdapter.ExecuteEntityList(personId);
            Assert.AreEqual(3, groups.Count);
            Assert.IsTrue(groups.Any(x => x.GroupSID == group1.Id));
            Assert.IsTrue(groups.Any(x => x.GroupSID == group2.Id));

            BvSpMembership_DeleteAdapter.ExecuteNonQuery(group1.Id, 0);
            PersonGroupRepository.Delete(group1.Id);

            context.GetCall("S1.I1").Assert.IsTrue(c => c.Resource == 0);
            
            groups = BvSpGetUserGroupsAdapter.ExecuteEntityList(personId);
            Assert.AreEqual(2, groups.Count);
            Assert.IsFalse(groups.Any(x => x.GroupSID == group1.Id));
            Assert.IsTrue(groups.Any(x => x.GroupSID == group2.Id));
        }
        
        [TestMethod]
        public void UpdatePersonGroupToBeAdministrative_CallAssignmentsAreDeleted_PersonGroupsAndSurveyRelationshipsAreCorrect()
        {
            var context = new TestData {
                Surveys = new[] {
                    new SurveyData {
                        Tag = "S1",
                        Interviews = new[] {
                            new InterviewData { Tag = "S1.I1", Call = new CallData() { Resource = "PG1,PG2" } },
                            new InterviewData { Tag = "S1.I2" }
                        },
                        Assigns = new string[]{"PG1"}
                    }
                },
                PersonGroups = new[] {
                    new PersonGroupData { Tag = "PG1", Name = "PersonGroup1" },
                    new PersonGroupData { Tag = "PG2", Name = "PersonGroup2" },
                    new PersonGroupData { Tag = "PG3", Name = "PersonGroup3" }
                },
                Persons = new PersonData[] {
                    new PersonData() { Tag = "P1", Memberships = "PG1,PG2" }
                },
                Supervisors = new [] {
                    new SupervisorData() {
                        Tag = "SV1",
                        Name = "Admin",
                        Surveys = new []{"S1"}
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var group1 = context.GetResource("PG1");
            var group2 = context.GetResource("PG2");
            var group3 = context.GetResource("PG3");

            var rootGroup = PersonManager.GetCatiRootID();

            PersonGroupService.SetParentGroups(group1.Id, new[] { rootGroup, group3.Id });
            
            var personId = context.GetPerson("P1").Id;
            var supervisorName = context.GetSupervisor("SV1").Name;
            
            var userGroups = BvSpGetUserGroupsAdapter.ExecuteEntityList(personId);
            Assert.AreEqual(3, userGroups.Count);
            Assert.IsTrue(userGroups.Any(x => x.GroupSID == group1.Id));
            Assert.IsTrue(userGroups.Any(x => x.GroupSID == group2.Id));
            
            var assignedSurveys = BvSpPerson_GetAssignedSurveyListAdapter.ExecuteEntityList(personId, supervisorName, 1);
            Assert.IsNotNull(assignedSurveys.SingleOrDefault(x => x.SID == survey.Id));
            
            //make group administrative
            var personGroupRepo = ServiceLocator.Resolve<IPersonGroupRepository>();
            var group = personGroupRepo.GetById(group1.Id);
            group.IsAdministrative = true;
            personGroupRepo.Update(group);

            context.GetCall("S1.I1").Assert.IsTrue(c => c.Resource == 0);
            
            userGroups = BvSpGetUserGroupsAdapter.ExecuteEntityList(personId);
            Assert.AreEqual(2, userGroups.Count);
            Assert.IsFalse(userGroups.Any(x => x.GroupSID == group1.Id));
            Assert.IsTrue(userGroups.Any(x => x.GroupSID == group2.Id));
            
            assignedSurveys = BvSpPerson_GetAssignedSurveyListAdapter.ExecuteEntityList(personId, supervisorName, 1);
            Assert.IsNotNull(assignedSurveys.SingleOrDefault(x => x.SID == survey.Id));
            
            //make group not administrative again 
            personGroupRepo = ServiceLocator.Resolve<IPersonGroupRepository>();
            group.IsAdministrative = false;
            personGroupRepo.Update(group);
            
            userGroups = BvSpGetUserGroupsAdapter.ExecuteEntityList(personId);
            Assert.AreEqual(3, userGroups.Count);
            Assert.IsTrue(userGroups.Any(x => x.GroupSID == group1.Id));
            Assert.IsTrue(userGroups.Any(x => x.GroupSID == group2.Id));
            
            assignedSurveys = BvSpPerson_GetAssignedSurveyListAdapter.ExecuteEntityList(personId, supervisorName, 1);
            Assert.IsNotNull(assignedSurveys.SingleOrDefault(x => x.SID == survey.Id));
        }
    }
}
