using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Common.Exceptions;
using System.Linq;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class SurveySpecificFilterTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        

        int _surveySid1;
        int _surveySid2;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveySid1 = _backendTools.CreateSurvey("p000112");
            _surveySid2 = _backendTools.CreateSurvey("p000111");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        //Create filters f1, f2, f3
        //make f1 and f3 survey specific for different surveys
        //insert f2 into f1 as subfilter
        //insert f3 into f2 as subfilter
        //Exception should be thrown
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_UpdateSubFilterFromAnotherSurvey_ExceptionShouldBeThrown()
        {
            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[]{ FilterField.CreateSurveySpecificFilterField(),
                       FilterField.CreateFilterFieldForSubFilter(filterId2) });

            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(_surveySid2,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => FilterAndPagingTools.UpdateFilterFields(filterId2,
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId3) }));
        }

        //Create filters f1, f2, f3
        //make f1 and f3 survey specific for different surveys
        //insert f3 into f2 and f2 into f1 as subfilters
        //Exception should be thrown
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_InsertSubSubFilterFromAnotherSurvey_ExceptionShouldBeThrown()
        {
            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId3) });

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => FilterAndPagingTools.CreateSimpleFilter(_surveySid2,
                new[]{ FilterField.CreateSurveySpecificFilterField(),
                                   FilterField.CreateFilterFieldForSubFilter(filterId2) }));
        }

        //Create filters f1, f2, f3, f4
        //make f3 and f4 survey specific for different surveys
        //insert f4 into f2
        //insert f2, f3 into f1 as subfilter
        //Exception should be thrown
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_InsertSubFiltersFromDifferentSurveys_ExceptionShouldBeThrown()
        {
            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            int filterId4 = FilterAndPagingTools.CreateSimpleFilter(_surveySid2,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            int filterId2 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId4) });

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterId3), 
                                    FilterField.CreateFilterFieldForSubFilter(filterId2)}));
        }

        //1. Create a filter F1 and make it specific for Survey1
        //2. Create a filter F2 and make it specific for Survey2
        //3. Create site-wide filter F3
        //4. Include F3 as subfilter in both F1 and F2
        //5. Add variable of Survey1 to F3.
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_UpdateSubFilterMakeItForAnotherSurvey_ExceptionShouldBeThrown()
        {
            int filterId3 = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField(), 
                                    FilterField.CreateFilterFieldForSubFilter(filterId3) });

            FilterAndPagingTools.CreateSimpleFilter(_surveySid2,
                new[] { FilterField.CreateSurveySpecificFilterField(), 
                                    FilterField.CreateFilterFieldForSubFilter(filterId3) });

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => FilterAndPagingTools.UpdateFilterFields(_surveySid1,
                filterId3,
                new[] { FilterField.CreateSurveySpecificFilterField() }));
        }

        //1. Create a filter F0-F5.
        //2. Insert F5 into F4, F4 into F3 etc... F1 into F0
        //3. Update a filter F2 on survey specific
        //4. Check that F2-F0 survey specific F5-F3 NOT
        //5. Update a filter F3 on survey specific
        //6. Check that F3-F0 survey specific F5-F4 NOT
        //7. Update a filter F3 on NOT survey specific
        //8. Check that F2-F0 survey specific and F5-F3 not
        //9. Update F4 on survey specific
        //10.Check that F4-F0 survey specific F5 NOT
        //11.Update F2 on not survey specifc
        //12.Check that F4-F0 survey specific F5 NOT
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_SpecificForParentFiltersShouldBeSurvey_TestCorrcet()
        {
            var filterIds = new int[6];

            filterIds[5] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            filterIds[4] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[5]) });

            filterIds[3] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[4]) });

            filterIds[2] = FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField(),
                        FilterField.CreateFilterFieldForSubFilter(filterIds[3]) });

            filterIds[1] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[2]) });

            filterIds[0] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[1]) });

            Assert.IsFalse( new[]{filterIds[2], filterIds[1], filterIds[0]}.Select(
                x=>FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1), 
              "Some parent filters didn't become survey specific");

            Assert.IsFalse( new[]{filterIds[5], filterIds[4], filterIds[3]}.Select(
                x=>FilterRepository.GetById(x).SurveySID).Any(x => x != 0), 
              "Some filters are survey specific by mistake");

            FilterAndPagingTools.UpdateFilterFields(_surveySid1,
                filterIds[3],
                new[] { FilterField.CreateSurveySpecificFilterField(),
                        FilterField.CreateFilterFieldForSubFilter(filterIds[4])});

            Assert.IsFalse(new[] { filterIds[3], filterIds[2], filterIds[1], filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            Assert.IsFalse(new[] { filterIds[5], filterIds[4] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");

            FilterAndPagingTools.UpdateFilterFields(filterIds[3],
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[4])});

            Assert.IsFalse(new[] { filterIds[2], filterIds[1], filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            Assert.IsFalse(new[] { filterIds[5], filterIds[4], filterIds[4] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");

            FilterAndPagingTools.UpdateFilterFields(_surveySid1,
                filterIds[4],
                new[] { FilterField.CreateSurveySpecificFilterField(),
                        FilterField.CreateFilterFieldForSubFilter(filterIds[5])});

            Assert.IsFalse(new[] { filterIds[4], filterIds[3], filterIds[2], filterIds[1], filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            Assert.IsFalse(new[] { filterIds[5] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");

            FilterAndPagingTools.UpdateFilterFields(filterIds[2],
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[3])});

            Assert.IsFalse(new[] { filterIds[4], filterIds[3], filterIds[2], filterIds[1], filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            Assert.IsFalse(new[] { filterIds[5] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");
        }

        //1. Create a filter F0-F2. F0 as survey specific.
        //2. Insert F0 into F2, F1 into F2
        //3. Check that F2, F0 survey specific F1 NOT
        //4. Update a filter F1 on survey specific
        //5. Check that F2-F0 survey specific
        //6. Update a filter F0 on NOT survey specific
        //7. Check that F2, F1 survey specific
        //8. Update a filter F1 on NOT survey specific
        //7. Check that F2-F0 are not survey specific
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_SpecificForChildFiltersShouldNotBeSurvey_TestCorrcet()
        {
            var filterIds = new int[3];

            filterIds[0] = FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            filterIds[1] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            filterIds[2] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[0]),
                        FilterField.CreateFilterFieldForSubFilter(filterIds[1])});

            Assert.IsFalse(new[] { filterIds[2], filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            Assert.IsFalse(new[] { filterIds[1] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");

            FilterAndPagingTools.UpdateFilterFields(_surveySid1,
                filterIds[1],
                new[] { FilterField.CreateSurveySpecificFilterField()});

            Assert.IsFalse(new[] { filterIds[2], filterIds[1], filterIds[0] }.Select(
                x =>  FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            FilterAndPagingTools.UpdateFilterFields(filterIds[0],
                new[] { FilterField.CreateSomeFilterField() });

            Assert.IsFalse(new[] { filterIds[2], filterIds[1] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some parent filters didn't become survey specific");

            Assert.IsFalse(new[] { filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");

            FilterAndPagingTools.UpdateFilterFields(filterIds[1],
                new[] { FilterField.CreateSomeFilterField() });

            Assert.IsFalse(new[] { filterIds[0], filterIds[1], filterIds[2] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");
        }

        //1. Create a filter F0-F2. F1 as survey specific.
        //2. Insert F0 into F1, F1 into F2
        //3. Delete F1.
        //4. Check that F2, F0 not survey specific
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_DeleteSurveySpecificFilter_AllFiltersAreNotSurveySpecific()
        {
            var filterIds = new int[3];

            filterIds[0] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            filterIds[1] = FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[0]),
                        FilterField.CreateSurveySpecificFilterField() });

            filterIds[2] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[1])});

            FilterRepository.Delete(filterIds[1]);

            Assert.IsFalse(new[] { filterIds[0], filterIds[2] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");
        }

        //1. Create a filter F0-F2. F0 as survey specific.
        //2. Insert F0 into F1, F1 into F2
        //3. Delete F1.
        //4. Check that F2 is not survey specific F0 is specific
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FilterForDifferentSurveysTest_DeleteFilter_ParentFilterIsNotSurveySpecific()
        {
            var filterIds = new int[3];

            filterIds[0] = FilterAndPagingTools.CreateSimpleFilter(_surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            filterIds[1] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[0])});

            filterIds[2] = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(filterIds[1]) });

            FilterRepository.Delete(filterIds[1]);

            Assert.IsFalse(new[] { filterIds[2] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != 0),
              "Some filters are survey specific by mistake");

            Assert.IsFalse(new[] { filterIds[0] }.Select(
                x => FilterRepository.GetById(x).SurveySID).Any(x => x != _surveySid1),
              "Some filters are not survey specific by mistake");
        }
    }
}
