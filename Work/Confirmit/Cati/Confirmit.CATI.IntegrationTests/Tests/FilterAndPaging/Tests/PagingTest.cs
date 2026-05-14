using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using System.Data;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class PagingTest
    {
        

        private DatabaseEngine _databaseEngine;        
        private const int PageSize = 20;

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();

            _databaseEngine = _framework.DbEngine;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PagingTest_AllRecordsArePlacedInFirstPageButNotFillItWhole_FirstPageIsCorrect()
        {
            int count;
            const int totalCount = PageSize / 2;
            FillDb(totalCount);
            
            DataTable dataTable = ExecGetObjectsPageSp(1, out count);

            Assert.AreEqual(totalCount, count, "Total count is incorrect. expected {0}, actial {1}", totalCount, count);
            Assert.AreEqual(totalCount, dataTable.Rows.Count, "Returned records count is incorrect. expected {0}, actial {1}", totalCount, dataTable.Rows.Count);
            Assert.AreEqual(1, dataTable.Rows[0]["SID"], "First record is incorrect. expected SID {0}, actial {1}", 1, dataTable.Rows[0]["SID"]);
            Assert.AreEqual(totalCount, dataTable.Rows[totalCount - 1]["SID"], "Last record is incorrect. expected SID {0}, actial {1}", 1, dataTable.Rows[0]["SID"]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PagingTest_TwoPagesHaveMaximumCountOfRecords_SecondPageIsCorrected()
        {
            int count;
            const int totalCount = PageSize * 2;

            FillDb(totalCount);

            DataTable dataTable = ExecGetObjectsPageSp(2, out count);

            Assert.AreEqual(totalCount, count, "When we try to get 2th page total count is incorrect");
            Assert.AreEqual(PageSize, dataTable.Rows.Count, "When we try to get 2th page count records on page is incorrect");

            Assert.AreEqual(PageSize + 1, dataTable.Rows[0]["SID"], "First record is incorrect. expected SID {0}, actial {1}", 1, dataTable.Rows[0]["SID"]);
            Assert.AreEqual(totalCount, dataTable.Rows[PageSize - 1]["SID"], "Last record is incorrect. expected SID {0}, actial {1}", 1, dataTable.Rows[0]["SID"]);                        
        }   

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PagingTest_LastPageHasOnlyOneRecord_LastPageIsCorrect()
        {
            int count;
            const int totalCount = PageSize * 2 + 1;

            FillDb(totalCount);

            const int pageCount = totalCount / PageSize + 1;
            
            DataTable dataTable = ExecGetObjectsPageSp(pageCount, out count);

            Assert.AreEqual(totalCount, count);
            Assert.AreEqual(1, dataTable.Rows.Count);

            Assert.AreEqual(totalCount, (int)dataTable.Rows[0][0]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PagingTest_ThereAreNoCalls_ThereAreNoPages()
        {
            FillDb(0);

            int count;
            DataTable dataTable = ExecGetObjectsPageSp(1, out count);

            Assert.AreEqual(0, count, "total count is incorrect");

            Assert.AreEqual(0, dataTable.Rows.Count, "records count on page is incorrect");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PagingTest_TryToGetPageWithoutCalls_ThereAreNoPages()
        {
            FillDb(1);

            int count;
            DataTable dataTable = ExecGetObjectsPageSp(2, out count);

            Assert.AreEqual(1, count, "total count is incorrect");

            Assert.AreEqual(0, dataTable.Rows.Count, "records count on page is incorrect");
        }

        private void FillDb(int totalCount)
        {
            string query = @"create table TestTable(
                SID int identity(1, 1),
                GUID uniqueidentifier,
                RepeatableID int null);

                declare @i int
                set @i = 0
                while @i < " + totalCount + @"
                begin
                   insert into TestTable
                   select newid(), @i%10
                   set @i = @i + 1
                end";

            _databaseEngine.ExecuteNonQuery(query, CommandType.Text);
        }

        private DataTable ExecGetObjectsPageSp(int pageIndex, out int count)
        {
            const string query = @"SELECT SID, GUID, RepeatableID FROM TestTable";

            var result = new DataTable();
            result.Load(
                BvSpGetObjectsPageAdapter.ExecuteReader(
                    pageIndex,
                    PageSize,
                    "SID",
                    true,
                    query,
                    "SID",
                    String.Empty,
                    null,
                    out count));

            return result;
        }
    }
}