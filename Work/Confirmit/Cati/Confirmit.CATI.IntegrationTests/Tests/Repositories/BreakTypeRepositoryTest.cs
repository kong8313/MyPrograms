using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.IntegrationTests.Tests.Repositories
{
    [TestClass]
    public class BreakTypeRepositoryTest : BaseMockedIntegrationTest
    {
        private IBreakTypeRepository _breakTypeRepository;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            BvBreakTypeCache.Instance.OnTableChanged();
            _breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();
        }

        private void CompareBreakTypes(BvBreakTypeEntity expectedBreakType, BvBreakTypeEntity actualBreakType, bool compareIds = true)
        {
            Assert.AreEqual(expectedBreakType.Name, actualBreakType.Name);
            Assert.AreEqual(expectedBreakType.Description, actualBreakType.Description);
            Assert.AreEqual(expectedBreakType.IsPaid, actualBreakType.IsPaid);

            if (compareIds)
            {
                Assert.AreEqual(expectedBreakType.Id, actualBreakType.Id);
            }
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Insert_InsertNewBreakType_BreakTypeIsAddedCorrectly()
        {
            var expectedBreakType = new BvBreakTypeEntity
            {
                Name = "Test break",
                Description = "Test description",
                IsPaid = true
            };

            _breakTypeRepository.Insert(expectedBreakType);

            var breakTypes = _breakTypeRepository.GetAll();

            Assert.AreEqual(2, breakTypes.Count, "Insert for breaks work incorrect");
            CompareBreakTypes(expectedBreakType, breakTypes[1], false);
        }
        
        [TestMethod, Owner(@"firm\grigoryk")]
        public void Update_InsertNewBreakTypeAndChangeAllFields_BreakTypeIsUpdatedCorrectly()
        {
            var initialBreakType = _breakTypeRepository.GetAll()[0];

            var expectedBreakType = new BvBreakTypeEntity
            {
                Id = initialBreakType.Id,
                Name = "Test break",
                Description = "Test description",
                IsPaid = false
            };

            _breakTypeRepository.Update(expectedBreakType);

            var actualBreakType = _breakTypeRepository.TryGetById(expectedBreakType.Id);

            CompareBreakTypes(expectedBreakType, actualBreakType);
        }
        
        [TestMethod, Owner(@"firm\grigoryk")]
        public void Delete_InsertTwoBreakTypesAndDeleteThem_BreakTypesAreRemovedCorrectly()
        {
            for (int i = 0; i < 2; i++)
            {
                var testBreakType = new BvBreakTypeEntity
                {
                    Name = "Test break" + i,
                    Description = "Test description" + i,
                    IsPaid = true
                };

                _breakTypeRepository.Insert(testBreakType);
            }

            _breakTypeRepository.Delete(new List<int> { 1, 3 });

            var allBreakTypes = _breakTypeRepository.GetAll();

            Assert.AreEqual(1, allBreakTypes.Count);
            Assert.AreEqual(2, allBreakTypes[0].Id);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        [ExpectedException(typeof(UserMessageException))]
        public void Delete_InsertOneBreakTypeDeleteAllBreakTypes_UserMessageExceptionWasThrown()
        {
            var testBreakType = new BvBreakTypeEntity
            {
                Name = "Test break",
                Description = "Test description",
                IsPaid = true
            };

            _breakTypeRepository.Insert(testBreakType);

            _breakTypeRepository.Delete(new List<int> { 1, 2 });
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void TryGetById_GetBreakTypeWithWrongId_CorrectResultsAreReturned()
        {
            var testBreakType = _breakTypeRepository.TryGetById(2);

            Assert.AreEqual(null, testBreakType);
        }
    }
}