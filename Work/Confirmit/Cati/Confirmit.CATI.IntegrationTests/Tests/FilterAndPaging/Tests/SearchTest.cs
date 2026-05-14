using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class SearchTest
    {
        #region Fields

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private const string SearchSampleDataPath = @"FilterWithPaging\SearchTestSample.xml";

        #endregion

        #region Initialize and Cleanup methods

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();

            SearchTools.CreateSampleTable(SearchSampleDataPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SearchTools.DeleteSampleTable();

            _framework.TestCleanup();
        }

        #endregion

        #region Properties

        
        
        #endregion

        #region Tests

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_CheckReturnedTotalCount_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnInt",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Greater,
                    Value = 10
                }
            );

            BvSpGetObjectsRangeAdapter.ExecuteReader(
                1,
                100,
                "ColumnInt",
                true,
                "select * from SearchTestSample",
                "ColumnInt", 
                SearchManager.GetSqlCondition(search),
                null, //count is taken from Query which is passed not from separate query
                out totalCount
            );

            Assert.AreEqual(2, totalCount);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_CheckIntegerValue_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnInt",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Greater,
                    Value = 10
                }
            );

            var result = new
                Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckIntegerValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_CheckIntegerValue_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_CheckDateValue_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnDate",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Equal,
                    Value = new DateTime(2009, 11, 3, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            var result = 
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDateValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_CheckDateValue_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_CheckDecimalValue_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnDecimal",
                    ColumnType = SearchColumnType.Decimal,
                    Operator = SearchOperator.Equal,
                    Value = -67865.22
                }
            );

            var result =
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDecimalValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_CheckDecimalValue_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_SimpleLikeCondition_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnText",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "Text"
                }
            );

            var result =
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_SimpleLikeCondition_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_SimpleLikeCondition_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_LikeConditionWithAstrisk_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnText",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "*2*k"
                }
            );

            var result =
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_LikeConditionWithAstrisk_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_LikeConditionWithAstrisk_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_StringCaseInsensitiveComparision_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnText",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Equal,
                    Value = "BOHGF"
                }
            );

            var result =
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_StringCaseInsensitiveComparision_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_StringCaseInsensitiveComparision_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_SeveralConditions_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnDate",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Greater,
                    Value = new DateTime(2009, 12, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            );
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnInt",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Less,
                    Value = 0
                }
            );

            var result =
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_SeveralConditions_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_SeveralConditions_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BvSpGetObjectsRange_TimeSpanEqualCondition_Success()
        {
            int totalCount;
            SearchParameterCollection search = new SearchParameterCollection();
            search.Add(
                new SearchParameter()
                {
                    ColumnName = "ColumnInt",
                    ColumnType = SearchColumnType.TimeSpan,
                    Operator = SearchOperator.Equal,
                    Value = new TimeSpan(0, 1, 40)
                }
            );

            var result = 
                new Confirmit.CATI.IntegrationTests.TestsData.FilterWithPaging.BvSpGetObjectsRange_TimeSpanEqualCondition_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"FilterWithPaging\BvSpGetObjectsRange_TimeSpanEqualCondition_Success",
                new string[] { "SpecialTempRowNumberForPaging" }
            );
        }

        #endregion
    }
}
