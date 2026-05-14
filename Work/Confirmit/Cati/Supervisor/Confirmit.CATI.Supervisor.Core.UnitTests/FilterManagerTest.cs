using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Core.Filters;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    //TODO: replace with integration tests.
    /*
    /// <summary>
    ///This is a test class for FilterManagerTest and is intended
    ///to contain all FilterManager Unit Tests
    ///</summary>
    [TestClass]
    public class FilterManagerTest
    {
        [TestInitialize]
        public void Init()
        {
            MockManager.Init();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MockManager.ClearAll();
        }
        
        [TestMethod, Owner(@"FIRM\EgorS")]
        public void GetDependentFiltersRecursive_DependentInPreviousLevel_ReturnDependent()
        {
            List<FilterInfoItem> dependent = new List<FilterInfoItem> { new FilterInfoItem { Name = "test", SID = 777 } };

            using (RecordExpectations recorder = new RecordExpectations())
            {
                FilterManager.GetDependentFilters(0);
                recorder.Return(dependent);

                FilterManager.GetDependentFilters(0);
                recorder.Return(new List<FilterInfoItem>()).RepeatAlways();
            }

            List<FilterInfoItem> actual = FilterManager.GetAllParentFilters(10);
            Assert.IsTrue(actual.Select(x => x.SID).Contains(777));
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void GetDependentFiltersRecursive_MultipleDependens_ReturnAllDependents()
        {
            FilterInfoItem test1 = new FilterInfoItem { Name = "test1", SID = 777 };
            FilterInfoItem test2 = new FilterInfoItem { Name = "test2", SID = 888 };
            FilterInfoItem test3 = new FilterInfoItem { Name = "test3", SID = 999 };
            FilterInfoItem test4 = new FilterInfoItem { Name = "test4", SID = 990 };

            List<FilterInfoItem> dependent1 = new List<FilterInfoItem> { test1 };
            List<FilterInfoItem> dependent2 = new List<FilterInfoItem> { test2 };
            List<FilterInfoItem> dependent3 = new List<FilterInfoItem> { test3, test4 };

            using (RecordExpectations recorder = new RecordExpectations())
            {
                FilterManager.GetDependentFilters(0);
                recorder.Return(dependent1);
                FilterManager.GetDependentFilters(0);
                recorder.Return(dependent2);
                FilterManager.GetDependentFilters(0);
                recorder.Return(dependent3);

                FilterManager.GetDependentFilters(0);
                recorder.Return(new List<FilterInfoItem>()).RepeatAlways();
            }

            List<FilterInfoItem> actual = FilterManager.GetAllParentFilters(10);
            Assert.IsTrue(actual.Contains(test1));
            Assert.IsTrue(actual.Contains(test2));
            Assert.IsTrue(actual.Contains(test3));
            Assert.IsTrue(actual.Contains(test4));
        }
        
        [TestMethod, Owner(@"FIRM\EgorS")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetDependentFiltersRecursive_InvalidArgumnt_Exception()
        {
            FilterManager.GetAllParentFilters(0);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteFilter_InvalidArgumnt_Exception()
        {
            FilterManager.DeleteFilter(0);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        [ExpectedException(typeof(FilterIsUsedException))]
        public void DeleteFilter_DeleteInUseFilter_Exception()
        {
            FilterInfoItem test3 = new FilterInfoItem { Name = "test3", SID = 999 };
            FilterInfoItem test4 = new FilterInfoItem { Name = "test4", SID = 990 };
            List<FilterInfoItem> dependent3 = new List<FilterInfoItem> { test3, test4 };

            using (RecordExpectations recorder = new RecordExpectations())
            {
                FilterManager.GetDependentFilters(0);
                recorder.Return(dependent3);
            }

            FilterManager.DeleteFilter(10);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        [VerifyMocks]
        public void DeleteFilterTest_ValidArgument_Success()
        {
            using (RecordExpectations recorder = new RecordExpectations())
            {
                FilterManager.GetDependentFilters(0);
                recorder.Return(new List<FilterInfoItem>());

                FilterRepository.Delete(0);
            }

            FilterManager.DeleteFilter(10);
        }
         
    }
    */
}