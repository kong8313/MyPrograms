using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class PersonDialTypeUpdateTests : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public void Init()
        {
            TestInitialize();
            ServiceLocator.Resolve<IServiceRegistrator>();
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void UpdateSampleTypeId_PersonSid_PersonUpdated()
        {
            var context = CreateContext();

            var person = context.GetPerson("P1");
            var qualifier = person.Id.ToString();

            BvSpPerson_UpdateBatchedAdapter.ExecuteNonQuery(qualifier, (byte)DialType.Cellphone);
            PersonRepository.RefreshCache();

            var updatedPerson = PersonRepository.GetById(person.Id);

            Assert.AreEqual((byte)DialType.Cellphone, updatedPerson.DialTypeId);
        }


        [TestMethod, Owner(@"FIRM\DenisM")]
        public void UpdateSampleTypeId_PersonGroupSid_TwoPersonsUpdated()
        {
            var context = CreateContext();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");

            var qualifier = context.PersonGroups.First().Id.ToString();

            BvSpPerson_UpdateBatchedAdapter.ExecuteNonQuery(qualifier, (byte)DialType.Cellphone);
            PersonRepository.RefreshCache();

            var p1 = PersonRepository.GetById(person1.Id);
            var p2 = PersonRepository.GetById(person2.Id);

            Assert.AreEqual((byte)DialType.Cellphone, p1.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, p2.DialTypeId);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void UpdateSampleTypeId_PersonAndGroupSids_ThreePersonsUpdated()
        {
            var context = CreateContext();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var person3 = context.GetPerson("P3");

            var qualifier = string.Format("{0},{1}",
                context.PersonGroups.First().Id,
                person3.Id);

            BvSpPerson_UpdateBatchedAdapter.ExecuteNonQuery(qualifier, (byte)DialType.Cellphone);
            PersonRepository.RefreshCache();

            var p1 = PersonRepository.GetById(person1.Id);
            var p2 = PersonRepository.GetById(person2.Id);
            var p3 = PersonRepository.GetById(person3.Id);

            Assert.AreEqual((byte)DialType.Cellphone, p1.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, p2.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, p3.DialTypeId);
        }

        private static TestDataContext CreateContext()
        {
            var context = new TestData()
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1", Memberships = "PG1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData {Tag = "P2", Memberships = "PG1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData {Tag = "P3", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                PersonGroups = new[]
                {
                    new PersonGroupData() {Tag = "PG1"}
                }
            }.Create();
            return context;
        }
    }
}
