using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class FilterValidatorTests
    {
        private FilterValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            _validator = new FilterValidator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(UserMessageException))]
        public void Validate_EmptyFilterName_ExceptionIsThrown()
        {
            var filter = new BvFiltersEntity();
            
            _validator.Validate(filter);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_CorrectObject_Success()
        {
            var filter = new BvFiltersEntity { Name = "sss" };

            _validator.Validate(filter);
        }
    }
}
