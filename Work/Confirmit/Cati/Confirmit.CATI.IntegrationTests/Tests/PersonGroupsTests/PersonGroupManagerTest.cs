using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Core.PersonGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonGroupsTests
{
    [TestClass]
    public class PersonGroupManagerTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void GetPersonsInGroups_ForCallCenterId_GroupsWithPersonsIdentifiersMappingAreReturned()
        {
            // arrange
            var context = new TestData
            {
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "PersonGroup1" },
                    new PersonGroupData { Tag = "PG2", Name = "PersonGroup2" },
                    new PersonGroupData { Tag = "PG3", Name = "PersonGroup3Subgroup1", Memberships = "PG1" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", CallCenter = "CC1", Memberships = "PG1" }, 
                    new PersonData { Tag = "P2", CallCenter = "CC1", Memberships = "PG2" },
                    new PersonData { Tag = "P3", CallCenter = "CC2", Memberships = "PG2" },
                    new PersonData { Tag = "P4", CallCenter = "CC1", Memberships = "PG3" }
                },
                CallCenters = new[] { new CallCenterData { Tag = "CC1" }, new CallCenterData { Tag = "CC2" } }
            }.Create();

            var callCenterId = context.GetCallCenter("CC1").Id;
            var group1Id = context.GetPersonGroup("PG1").Id;
            var group2Id = context.GetPersonGroup("PG2").Id;
            var subGroupId = context.GetPersonGroup("PG3").Id;

            // act
            var manager = ServiceLocator.Resolve<PersonGroupManager>();
            var result = manager.GetPersonsInGroups(callCenterId);

            // assert
            var expected = new Dictionary<int, List<int>>();
            expected[PersonGroupService.RootGroupId] = new List<int>();
            expected[group1Id] = new List<int> { context.GetPerson("P1").Id };
            expected[group2Id] = new List<int> { context.GetPerson("P2").Id };
            expected[subGroupId] = new List<int> { context.GetPerson("P4").Id };

            CollectionAssert.AreEquivalent(expected.Keys, result.Keys);
            CollectionAssert.AreEquivalent(expected.Values.SelectMany(x => x).ToList(), result.Values.SelectMany(x => x).ToList());
        }
    }
}