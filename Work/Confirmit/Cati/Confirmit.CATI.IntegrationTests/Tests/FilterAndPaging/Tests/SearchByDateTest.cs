using System;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class SearchByDateTest
    {
        #region Fields

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private const string SearchSampleDataPath = @"SearchingByDate\SearchByDateSample.xml";

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

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SearchByDateInMoskowTZ_EqualOperator_Success()
        {
            int totalCount;
            var search = new SearchParameterCollection
                             {
                                 new SearchParameter
                                     {
                                         ColumnName = "ColumnDate",
                                         ColumnType = SearchColumnType.DateTime,
                                         Operator = SearchOperator.Equal,
                                         Value = new DateTime(2009, 3, 11, 0, 0, 0)
                                     }
                             };

            TimezoneManager.AddTimezone(16);
            var result =
                new TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDateValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search, 16), // TimezoneID=16 - GMT+3
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"SearchingByDate\SearchByDateInMoskowTZ_EqualOperator_Success",
                new [] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SearchByDateInMoskowTZ_LessOperator_Success()
        {
            int totalCount;
            var search = new SearchParameterCollection
                             {
                                 new SearchParameter
                                     {
                                         ColumnName = "ColumnDate",
                                         ColumnType = SearchColumnType.DateTime,
                                         Operator = SearchOperator.Less,
                                         Value = new DateTime(2009, 3, 12, 0, 0, 0)
                                     }
                             };

            TimezoneManager.AddTimezone(16);
            var result =
                new TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDateValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search, 16), // TimezoneID=16 - GMT+3
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"SearchingByDate\SearchByDateInMoskowTZ_LessOperator_Success",
                new [] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SearchByDateInMoskowTZ_LessOrEqualOperator_Success()
        {
            int totalCount;
            var search = new SearchParameterCollection
                             {
                                 new SearchParameter
                                     {
                                         ColumnName = "ColumnDate",
                                         ColumnType = SearchColumnType.DateTime,
                                         Operator = SearchOperator.LessThanOrEqual,
                                         Value = new DateTime(2009, 3, 12, 0, 0, 0)
                                     }
                             };

            TimezoneManager.AddTimezone(16);
            var result =
                new TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDateValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search, 16), // TimezoneID=16 - GMT+3
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"SearchingByDate\SearchByDateInMoskowTZ_LessOrEqualOperator_Success",
                new [] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SearchByDateInMoskowTZ_GreaterOperator_Success()
        {
            int totalCount;
            var search = new SearchParameterCollection
                             {
                                 new SearchParameter
                                     {
                                         ColumnName = "ColumnDate",
                                         ColumnType = SearchColumnType.DateTime,
                                         Operator = SearchOperator.Greater,
                                         Value = new DateTime(2009, 3, 12, 0, 0, 0)
                                     }
                             };

            TimezoneManager.AddTimezone(16);
            var result =
                new TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDateValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search, 16), // TimezoneID=16 - GMT+3
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"SearchingByDate\SearchByDateInMoskowTZ_GreaterOperator_Success",
                new [] { "SpecialTempRowNumberForPaging" }
            );
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SearchByDateInMoskowTZ_GreaterOrEqualOperator_Success()
        {
            int totalCount;
            var search = new SearchParameterCollection
                             {
                                 new SearchParameter
                                     {
                                         ColumnName = "ColumnDate",
                                         ColumnType = SearchColumnType.DateTime,
                                         Operator = SearchOperator.GreaterThanOrEqual,
                                         Value = new DateTime(2009, 3, 12, 0, 0, 0)
                                     }
                             };

            TimezoneManager.AddTimezone(16);
            var result =
                new TestsData.FilterWithPaging.BvSpGetObjectsRange_CheckDateValue_Success.NewDataSet.tableDataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnInt",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(search, 16), // TimezoneID=16 - GMT+3
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            FilterAndPagingTools.Compare(
                result,
                @"SearchingByDate\SearchByDateInMoskowTZ_GreaterOrEqualOperator_Success",
                new[] { "SpecialTempRowNumberForPaging" }
            );
        }

        /// <summary>
        /// 1. Set local (site) timezone to (GMT+03:00) Kuwait, Riyadh
        /// 2. Take today in local (site) timezone.
        /// 3. Construct 2 dates: begin of local today and end of local today.
        /// 4. Convert dates to UTC and store into sample table.
        /// 5. Construct search condition with Today predefined condition and call BvSpGetObjectsRange procedure.
        /// 6. Check that procedure returns 2 records and they are equal to created dates.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SearchByPredefinedDate_Today_Success()
        {
            // activating (GMT+03:00) Kuwait, Riyadh timezone
            int timezoneId = 15;
            TimezoneManager.AddTimezone(timezoneId);

            DateTime firstDate;
            DateTime secondDate;
            FillSampleForTodayTest(timezoneId, out firstDate, out secondDate);

            var searchParams = new SearchParameterCollection();
            searchParams.Add(
                new SearchParameter
                {
                    ColumnName = "ColumnDate",
                    ColumnType = SearchColumnType.PredefinedDatePeriod,
                    Operator = SearchOperator.Equal,
                    Value = SearchPredefinedDate.Today
                }
            );

            int totalCount;
            var result = new DataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnDate",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(searchParams, timezoneId),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            Assert.AreEqual(2, result.Rows.Count, "Test returned wrong number of rows");
            Assert.AreEqual(firstDate, result.Rows[0]["ColumnDate"]);
            Assert.AreEqual(secondDate, result.Rows[1]["ColumnDate"]);
        }

        /// <summary>
        /// 1. Set local (site) timezone to (GMT+03:00) Kuwait, Riyadh
        /// 2. Take today in local (site) timezone.
        /// 3. Construct 2 dates: begin of current week (week starts on Monday) and local now.
        /// 4. Convert dates to UTC and store into sample table.
        /// 5. Construct search condition with ThisWeek predefined condition and call BvSpGetObjectsRange procedure.
        /// 6. Check that procedure returns 2 records and they are equal to created dates.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SearchByPredefinedDate_ThisWeek_Success()
        {
            // activating (GMT+03:00) Kuwait, Riyadh timezone
            int timezoneId = 15;
            TimezoneManager.AddTimezone(timezoneId);

            DateTime firstDate;
            DateTime secondDate;
            FillSampleForThisWeekTest(timezoneId, out firstDate, out secondDate);

            var searchParams = new SearchParameterCollection();
            searchParams.Add(
                new SearchParameter
                {
                    ColumnName = "ColumnDate",
                    ColumnType = SearchColumnType.PredefinedDatePeriod,
                    Operator = SearchOperator.Equal,
                    Value = SearchPredefinedDate.ThisWeek
                }
            );

            int totalCount;
            var result = new DataTable();
            result.Load(
                BvSpGetObjectsRangeAdapter.ExecuteReader(
                    1,
                    100,
                    "ColumnDate",
                    true,
                    "select * from SearchTestSample",
                    "ColumnInt",
                    SearchManager.GetSqlCondition(searchParams, timezoneId),
                    null, //count is taken from Query which is passed not from separate query
                    out totalCount
                )
            );

            Assert.AreEqual(2, result.Rows.Count, "Test returned wrong number of rows");
            Assert.AreEqual(firstDate, result.Rows[0]["ColumnDate"]);
            Assert.AreEqual(secondDate, result.Rows[1]["ColumnDate"]);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills sample table with data needed for testing Today predifined date.
        /// We fill dates according given timezone.
        /// </summary>
        /// <param name="timezoneId">Local timezone we are working with.</param>
        private void FillSampleForTodayTest(int timezoneId, out DateTime todayStart, out DateTime todayEnd)
        {
            DateTime utcNow = DateTime.UtcNow;
            // Local now in our selected site timezone
            DateTime localNow = TimezoneManager.ConvertToTzLocalTime(timezoneId, utcNow);

            DateTime localTodayStart = new DateTime(localNow.Year, localNow.Month, localNow.Day, 0, 0, 0);
            DateTime localTodayEnd = new DateTime(localNow.Year, localNow.Month, localNow.Day, 23, 59, 59);

            // converting dates to utc and store into sample table
            todayStart = TimezoneManager.ConvertToUTC(timezoneId, localTodayStart);
            todayEnd = TimezoneManager.ConvertToUTC(timezoneId, localTodayEnd);
            SearchTools.InsertIntoSampleTable("ColumnDate", SqlDbType.DateTime, 8,
                new object[] 
                {
                    todayStart,
                    todayEnd
                }
            );
        }

        /// <summary>
        /// Fills sample table with data needed for testing ThisWeek predifined date.
        /// We fill dates according given timezone.
        /// </summary>
        /// <param name="timezoneId">Local timezone we are working with.</param>
        private void FillSampleForThisWeekTest(int timezoneId, out DateTime weekStart, out DateTime endDate)
        {
            DateTime utcNow = DateTime.UtcNow;
            // Local now in our selected site timezone
            DateTime localNow = TimezoneManager.ConvertToTzLocalTime(timezoneId, utcNow);

            // calculating first day of current week. We consider that a week starts on Monday
            DateTime weekStartDate = localNow.AddDays(localNow.DayOfWeek != DayOfWeek.Sunday ? DayOfWeek.Monday - localNow.DayOfWeek : -6);
            weekStartDate = new DateTime(weekStartDate.Year, weekStartDate.Month, weekStartDate.Day, 0, 0, 0);

            weekStart = TimezoneManager.ConvertToUTC(timezoneId, weekStartDate);
            endDate = TimezoneManager.ConvertToUTC(timezoneId, localNow);
            SearchTools.InsertIntoSampleTable("ColumnDate", SqlDbType.DateTime, 8,
                new object[] 
                {
                    weekStart,
                    endDate
                }
            );
        }

        #endregion
    }
}
