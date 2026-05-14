using System.Globalization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.Filters;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class FilterCyclicReferenceValidatorTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(UserMessageException))]
        public void Validate_CyclicReference_ExceptionIsThrown()
        {
            int filterId1 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId1) });

            var filter1 = FilterRepository.GetById(filterId1);

            new FilterCyclicReferenceValidator().Validate(filter1, new[]
                                                                       {
                                                                           new BvFilterFieldsEntity
                                                                               {
                                                                                   Column = "column",
                                                                                   IsNeedCast = false,
                                                                                   Sign = (int) FilterOperator.Subfilter,
                                                                                   Type = (int) VariableTypes.Subfilter,
                                                                                   Value = filterId2.ToString(CultureInfo.InvariantCulture)
                                                                               }
                                                                       });
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_NoReferences_Success()
        {
            int filterId1 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            var filter1 = FilterRepository.GetById(filterId1);

            new FilterCyclicReferenceValidator().Validate(filter1, new[]
                                                                       {
                                                                           new BvFilterFieldsEntity
                                                                               {
                                                                                   Column = "column",
                                                                                   IsNeedCast = false,
                                                                                   Sign = (int) FilterOperator.Subfilter,
                                                                                   Type = (int) VariableTypes.Subfilter,
                                                                                   Value = filterId2.ToString(CultureInfo.InvariantCulture)
                                                                               }
                                                                       });
        }
    }
}
