using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;

using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class FilterCopyingServiceTest
    {
        

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private IUserSurveyPermissionRepository _permissionRepository;

        private int _surveySid1;
        private int _surveySid2;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            _permissionRepository = ServiceLocator.Resolve<IUserSurveyPermissionRepository>();

            _surveySid1 = _backendTools.CreateSurvey("p000112");
            _permissionRepository.Insert("administrator", "p000112");
            _surveySid2 = _backendTools.CreateSurvey("p000111");
            _permissionRepository.Insert("administrator", "p000111");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetListOfSurveysToCopyFiltersFromDoesNotReturnCurrentSurvey()
        {
            FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] {FilterField.CreateSurveySpecificFilterField()});

            var surveys = new FilterCopyingService().GetListOfSurveysToCopyFiltersFrom(_surveySid1, "administrator");

            Assert.IsFalse(surveys.Any());
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetListOfSurveysToCopyFiltersFromReturnsOnlySurveySpecificFiltersCount()
        {
            FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] {FilterField.CreateSurveySpecificFilterField()});

            var surveys = new FilterCopyingService().GetListOfSurveysToCopyFiltersFrom(_surveySid2, "administrator");

            Assert.AreEqual(1, surveys.Count);
            Assert.AreEqual(1, surveys[0].FiltersCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetListOfSurveysToCopyFiltersFromDoesNotReturnSoftDeletedSurveys()
        {
            FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            new ManagementService().SoftDeleteSurvey("p000112");

            var surveys = new FilterCopyingService().GetListOfSurveysToCopyFiltersFrom(_surveySid2, "administrator");

            Assert.IsFalse(surveys.Any());
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetListOfSurveysToCopyFiltersFromDoesNotReturnUnpermittedSurveys()
        {
            FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            var surveys = new FilterCopyingService().GetListOfSurveysToCopyFiltersFrom(_surveySid2, "administrator2");

            Assert.IsFalse(surveys.Any());
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void MoveSurveySpecificFiltersToSurveyMovesSurveySpecificAndLeavesSiteWideFilters()
        {
            int siteWideFilter = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            new FilterCopyingService().MoveSurveySpecificFiltersToSurvey(_surveySid1, _surveySid2);

            var surveySpecificFiltersForSurvey1 = FilterRepository.GetFiltersList(false, _surveySid1);
            var surveySpecificAndSiteWideFiltersForSurvey1 = FilterRepository.GetFiltersList(true, _surveySid1);
            var surveySpecificFiltersForSurvey2 = FilterRepository.GetFiltersList(false, _surveySid2);

            Assert.AreEqual(0, surveySpecificFiltersForSurvey1.Count);
            Assert.AreEqual(1, surveySpecificAndSiteWideFiltersForSurvey1.Count);
            Assert.AreEqual(siteWideFilter, surveySpecificAndSiteWideFiltersForSurvey1[0].SID);
            Assert.AreEqual(1, surveySpecificFiltersForSurvey2.Count);
            Assert.AreEqual(surveySpecificFilter, surveySpecificFiltersForSurvey2[0].SID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void MoveSurveySpecificFiltersToSurveyLeavesOldFilterNames()
        {
            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            var oldName = FilterRepository.GetById(surveySpecificFilter).Name;
            new FilterCopyingService().MoveSurveySpecificFiltersToSurvey(_surveySid1, _surveySid2);

            var surveySpecificFiltersForSurvey2 = FilterRepository.GetFiltersList(false, _surveySid2);

            Assert.AreEqual(oldName, surveySpecificFiltersForSurvey2[0].Name);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CopySurveySpecificFiltersToSurveyCopiesSurveySpecificAndLeavesSiteWideFilters()
        {
            FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() });

            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            new FilterCopyingService().CopySurveySpecificFiltersToSurvey(_surveySid1, _surveySid2);

            var surveySpecificFiltersForSurvey1 = FilterRepository.GetFiltersList(false, _surveySid1);
            var surveySpecificAndSiteWideFiltersForSurvey1 = FilterRepository.GetFiltersList(true, _surveySid1);
            var surveySpecificFiltersForSurvey2 = FilterRepository.GetFiltersList(false, _surveySid2);
            var surveySpecificAndSiteWideFiltersForSurvey2 = FilterRepository.GetFiltersList(true, _surveySid2);

            Assert.AreEqual(1, surveySpecificFiltersForSurvey1.Count);
            Assert.AreEqual(surveySpecificFilter, surveySpecificFiltersForSurvey1[0].SID);

            Assert.AreEqual(2, surveySpecificAndSiteWideFiltersForSurvey1.Count);
            
            Assert.AreEqual(1, surveySpecificFiltersForSurvey2.Count);

            Assert.AreEqual(2, surveySpecificAndSiteWideFiltersForSurvey2.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CopySurveySpecificFiltersToSurveyChangesLinksToSurveySpecificSubfilters()
        {
            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateFilterFieldForSubFilter(surveySpecificFilter) });

            new FilterCopyingService().CopySurveySpecificFiltersToSurvey(_surveySid1, _surveySid2);

            var surveySpecificFiltersForSurvey2 = FilterRepository.GetFiltersList(false, _surveySid2);

            Assert.AreEqual(2, surveySpecificFiltersForSurvey2.Count);

            string newSubfilterId = surveySpecificFiltersForSurvey2
                .Select(x => FilterService.GetFields(x.SID)[0])
                .Where(x => x.Sign == (int) FilterOperator.Subfilter)
                .Select(x => x.Value).First();

            Assert.AreNotEqual(
                surveySpecificFilter.ToString(CultureInfo.InvariantCulture),
                newSubfilterId);

            Assert.IsTrue(surveySpecificFiltersForSurvey2.Select(x => x.SID.ToString(CultureInfo.InvariantCulture)).Contains(newSubfilterId));
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CopySurveySpecificFiltersToSurveyAppendsProjectIdToFilterNames()
        {
            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            var oldName = FilterRepository.GetById(surveySpecificFilter).Name;
            new FilterCopyingService().CopySurveySpecificFiltersToSurvey(_surveySid1, _surveySid2);

            var surveySpecificFiltersForSurvey2 = FilterRepository.GetFiltersList(false, _surveySid2);

            Assert.AreEqual(oldName + "_" + "p000111", surveySpecificFiltersForSurvey2[0].Name);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CopySurveySpecificFiltersThrowsExceptionIfNewFilterNameIsTooLong()
        {
            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter(
                _surveySid1,
                new[] { FilterField.CreateSurveySpecificFilterField() });

            var entity = FilterRepository.GetById(surveySpecificFilter);

            entity.Name = new string(Enumerable.Repeat('f', 251).ToArray());
            FilterRepository.Update(entity);

            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () =>
                new FilterCopyingService().CopySurveySpecificFiltersToSurvey(_surveySid1, _surveySid2));

        }
    }
}