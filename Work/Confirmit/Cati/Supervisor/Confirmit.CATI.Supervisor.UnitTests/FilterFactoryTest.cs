using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Supervisor.Classes.Filters.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class FilterFactoryTest
    {
        private FilterFactory _factoryRepositoryIsNotUsed;
        private FilterFactory _factoryExistingFilter;
        private FilterFactory _factoryWithValidationError;
        private BvFiltersEntity _existingFilter;

        [TestInitialize]
        public void TestInitialize()
        {
            _existingFilter = new BvFiltersEntity
                                  {
                                      SID = 123,
                                      Name = "ExistingName",
                                      Description = "Some description for existing item",
                                      SurveySID = 15,
                                      AndOrOperator = 1,
                                      Hidden = 0
                                  };

            IFilterRepository filterRepositoryStub = new StubIFilterRepository
            {
                GetByIdInt32 = sid =>
                {
                    Assert.Fail("Filter repository shouldn't be called");
                    return null;
                }
            };

            IFilterRepository customFilterRepositoryStub = new StubIFilterRepository
            {
                GetByIdInt32 = sid =>
                    {
                        Assert.AreEqual(_existingFilter.SID, sid, "Wrong existing filter identifier");

                        return _existingFilter;
                    }
            };

            IFilterValidator emptyValidator = new StubIFilterValidator
            {
                ValidateBvFiltersEntity = entity => { }
            };

            IFilterValidator userMessageExceptionValidator = new StubIFilterValidator
            {
                ValidateBvFiltersEntity = entity => { throw new UserMessageException("fff"); }
            };

            _factoryRepositoryIsNotUsed = new FilterFactory(emptyValidator, filterRepositoryStub);
            _factoryExistingFilter = new FilterFactory(emptyValidator, customFilterRepositoryStub);
            _factoryWithValidationError = new FilterFactory(userMessageExceptionValidator, filterRepositoryStub);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Create_FilterIdIsIntMinValue_FilterRepositoryIsNotUsed()
        {
            var result = _factoryRepositoryIsNotUsed.Create(Int32.MinValue, "fds", "desc", "1");
            Assert.AreEqual(0, result.SID);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Create_FilterIdIsExistingFilterId_FilterRepositoryIsCalledWithProperFilterId()
        {
            var result = _factoryExistingFilter.Create(_existingFilter.SID, "fds", "de", "1");
            Assert.AreEqual(_existingFilter.SID, result.SID, "Wrong filter identifier");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Create_ProperDataProvided_ResultingFilterContainsProperData()
        {
            const string name = "name";
            const string description = "description";
            const byte operatorValue = 2;
            var result = _factoryRepositoryIsNotUsed.Create(Int32.MinValue, name, description, operatorValue.ToString());

            Assert.AreEqual(name, result.Name, "Wrong filter name");
            Assert.AreEqual(description, result.Description, "Wrong filter description");
            Assert.AreEqual(operatorValue, result.AndOrOperator, "Wrong filter operator");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof (UserMessageException))]
        public void Create_ValidatorThrowsAnException_ExceptionIsRethrown()
        {
            _factoryWithValidationError.Create(Int32.MinValue, string.Empty, string.Empty, "1");
        }
    }
}
