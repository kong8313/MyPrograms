using Confirmit.CATI.IntegrationTests.Framework.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class FiltersCyclingTest : BaseMockedIntegrationTest
    {
        // Create filters f1, f2, f3
        // insert f3 into f2 as subfilter
        // insert f2 into f1 as subfilter
        // insert f1 into f3 as subfilter
        // Exception should be thrown
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FiltersCyclingTest_InsertFilterIntoItselfSubFilter_ExceptionShouldBeThrown()
        {
            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(
                new[]{ FilterField.CreateSomeFilterField() });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[]{ FilterField.CreateFilterFieldForSubFilter(filterId3) });

            int filterId1 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId2) });

            TestAssert.InvokeMethodAndVerifyExceptionThrown < UserMessageException>(
                    () => FilterAndPagingTools.UpdateFilterFields(filterId3,
                            new[] { FilterField.CreateFilterFieldForSubFilter(filterId1) }));
        }

        // Create filters f1, f2, f3
        // insert f1 into f3 as subfilter
        // insert f2 into f1 as subfilter
        // insert f3 into f2 as subfilter
        // Exception should be thrown
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FiltersCyclingTest_UpdateFilterMakeItPartOfLoop_ExceptionShouldBeThrown()
        {
            int filterId1 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId1) });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.UpdateFilterFields(filterId1,
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId2) });

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => FilterAndPagingTools.UpdateFilterFields(filterId2,
                    new[] { FilterField.CreateFilterFieldForSubFilter(filterId3) }));
        }

        // Create filters f1, f2, f3
        // insert f3 into f2 as subfilter
        // insert f2 into f1 as subfilter
        // insert f3 into f1 as subfilter
        // Last addition should be sucessful
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FiltersCyclingTest_TryToInsertSubFilterTwiceByDifferentWay_AdditionShouldBeSuccessful()
        {
            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId3) });

            int filterId1 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId2),
                                    FilterField.CreateFilterFieldForSubFilter(filterId3)});

            Assert.IsTrue(filterId1 > 0, "Filter was not created");
        }
    }
}
