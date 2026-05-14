using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Core.Repositories;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using System.Data;
using System.Data.SqlClient;
using Confirmit.Test.Common.Attributes;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class DeletionFilterTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        private int CountOfRecordsInBvFilterTableForCurrentFilter( int filterId )
        {
            const string query = "SELECT COUNT(*) FROM BvFilters Where SID = @SID";
            var sqlParameter = new SqlParameter( "@SID", filterId );

            return _framework.DbEngine.ExecuteScalar<int>( query, CommandType.Text, sqlParameter );
        }

        private int CountOfRecordsInBvFilterFieldsTableForCurrentFilter( int filterId )
        {
            const string query = "SELECT COUNT(*) FROM BvFilterFields Where FilterSID = @FilterSID";
            var sqlParameter = new SqlParameter( "@FilterSID", filterId );

            return _framework.DbEngine.ExecuteScalar<int>( query, CommandType.Text, sqlParameter );
        }

        [TestMethod, Owner( @"FIRM\AlexanderL" ), Cr(35541) ]
        public void DeletionFilterTest_AllRecordsOfSurveySpecificFilterAreRemovedAfterSurveyDeletion_OnlyNecessaryRecordsAreInDb()
        {
            int surveySid = _backendTools.CreateSurvey("p000112");

            int siteWideFilter = FilterAndPagingTools.CreateSimpleFilter(
                new[] { FilterField.CreateSomeFilterField() } );

            int surveySpecificFilter = FilterAndPagingTools.CreateSimpleFilter( surveySid,
                new[] { FilterField.CreateSurveySpecificFilterField() } );

            SurveyRepository.Delete( surveySid );

            Assert.AreEqual( 1, CountOfRecordsInBvFilterTableForCurrentFilter( siteWideFilter ), "Record for site-wide filter from BvFilter table was deleted" );
            Assert.AreEqual( 1, CountOfRecordsInBvFilterFieldsTableForCurrentFilter( siteWideFilter ), "record for site-wide filter from BvFilterFields table was deleted" );

            Assert.AreEqual( 0, CountOfRecordsInBvFilterTableForCurrentFilter( surveySpecificFilter ), "Record for survey specific filter from BvFilter table was not deleted" );
            Assert.AreEqual( 0, CountOfRecordsInBvFilterFieldsTableForCurrentFilter( surveySpecificFilter ), "record for survey specific filter from BvFilterFields table was not deleted" );
        }
    }
}
