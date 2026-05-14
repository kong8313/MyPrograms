using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Microsoft.SqlServer.Management.Smo;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaBalancing
{
    [TestClass]
    public class IndexRecreatingDuringQuotaBalancingTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IQuotaBalancingService _quotaBalancingService;
        private IQuotaBalancingRepository _quotaBalancingRepository;

        private BackendTools _backendTools;

        const string ProjectId = "p83470903";
        private int _surveyid;
        private QuotaInfo[] _quotaInfos = new QuotaInfo[]{};

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize(false);
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveyid = _backendTools.CreateSurvey(ProjectId);

            _framework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>().GetQuotaInfosInt32 = id => _quotaInfos;

            _quotaBalancingService = ServiceLocator.Resolve<IQuotaBalancingService>();
            _quotaBalancingRepository = ServiceLocator.Resolve<IQuotaBalancingRepository>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        private string GetTableName()
        {
            return ReplicationSchemaService.GetDestinationTableName(_surveyid);
        }

        private bool IsTableContainIndex(string tableName, string indexName)
        {
            var dbEngine = new DatabaseEngine();
            var query = $"SELECT count(*) FROM sys.indexes WHERE object_id = object_id('[dbo].[{tableName}]') AND name = '{indexName}'";
            var cnt = dbEngine.ExecuteScalar<int>(query);
            return cnt > 0;
        }

        private TableInfo[] GetTestDataWithOneResponseTable()
        {
            var c11 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = new[] { 1 } };
            var c12 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 36, Name = "q2", QuotaIds = new[] { 1 } };
            var c13 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 39, Name = "q3", QuotaIds = new[] { 1 } };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c11, c12, c13 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_UpdateReplicationScheme_IndexCreatedCorrectly()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "respid" }, indexColumns);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SetQuotaBalancing_ChangeIndexOrder_IndexRecreatedCorrectly()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q1" }, 10);

            using (new ConnectionScope())
            {
                var tableName = GetTableName();

                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(tableName, 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "respid" }, indexColumns);

                Assert.IsFalse(IsTableContainIndex(tableName, "IX_repl_q1"));
                Assert.IsTrue(IsTableContainIndex(tableName, "IX_repl_q2"));
            }

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q2" }, 10);

            using (new ConnectionScope())
            {
                var tableName = GetTableName();

                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(tableName, 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q2", "q1", "q3", "respid" }, indexColumns);

                Assert.IsTrue(IsTableContainIndex(tableName, "IX_repl_q1"));
                Assert.IsFalse(IsTableContainIndex(tableName, "IX_repl_q2"));
            }

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q1" }, 10);

            using (new ConnectionScope())
            {
                var tableName = GetTableName();

                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(tableName, 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "respid" }, indexColumns);

                Assert.IsFalse(IsTableContainIndex(tableName, "IX_repl_q1"));
                Assert.IsTrue(IsTableContainIndex(tableName, "IX_repl_q2"));
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_MarkQuotaWithAppropriateFilter_IndexIsNotRecreated()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q1" }, 10);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "respid" }, indexColumns);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_MarkQuotaWithNotAppropriateFilter_IndexIsRecreated()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q2" }, 10);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q2", "q1", "q3", "respid" }, indexColumns);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_MarkQuotaWithAppropriateFilterIfChangeFieldOrder_IndexIsRecreated()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q2", "q1" }, 10);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "respid" }, indexColumns);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_ChangeQuota_IndexIsRecreated()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q3" }, 10);

            var column = testData[0].ReplicationColumns[2];
            testData[0].ReplicationColumns = new[] { testData[0].ReplicationColumns[0], testData[0].ReplicationColumns[1] };

            UpdateSurveyReplicationScheme(testData);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "respid" }, indexColumns);
            }

            testData[0].ReplicationColumns = new[] { testData[0].ReplicationColumns[0], testData[0].ReplicationColumns[1], column };

            UpdateSurveyReplicationScheme(testData);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "respid" }, indexColumns);
            }

            column.Name = "q4";
            testData[0].ReplicationColumns = new[] { testData[0].ReplicationColumns[0], testData[0].ReplicationColumns[1], column };

            UpdateSurveyReplicationScheme(testData);

            using (new ConnectionScope())
            {
                var indexColumns = ServiceLocator.Resolve<IReplicationIndexService>().GetIndexFields(GetTableName(), 1).Select(x => x.Name).ToArray();
                CollectionAssert.AreEqual(new[] { "q1", "q2", "q4", "respid" }, indexColumns);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_RemoveQuota_QuotaBalancingDataIsCleaned()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q3" }, 10);

            foreach (var column in testData[0].ReplicationColumns) column.QuotaIds = new int[0];

            UpdateSurveyReplicationScheme(testData);

            Assert.AreEqual(0, _quotaBalancingRepository.GetBalancedQuotasForSurvey(_surveyid).Count);
            Assert.AreEqual(0, _quotaBalancingRepository.GetBalancedFieldsForSurvey(_surveyid).Length);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating__Create2QuotasOneOfThemForBalancingRecretThisQuotas()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();
            testData[0].ReplicationColumns[1].QuotaIds = new[] { 2 };

            UpdateSurveyReplicationScheme(testData);

            SetQuotaBalancing(_surveyid, 2, 10, new[] { "q2" }, 10);

            testData[0].ReplicationColumns[1].QuotaIds = new int[0];
            UpdateSurveyReplicationScheme(testData);

            testData[0].ReplicationColumns[1].QuotaIds = new[] { 2 };
            UpdateSurveyReplicationScheme(testData);
        }

        public void UpdateSurveyReplicationScheme(TableInfo[] testData)
        {
            var columns = testData.SelectMany(table => table.ReplicationColumns).ToArray();
            _quotaInfos = columns.SelectMany(column => column.QuotaIds ?? new int[]{}).Distinct().Select(quotaId => new QuotaInfo()
            {
                Id = quotaId,
                Name = $"quota{quotaId}",
                Table = $"quota_{quotaId}",
                Fields = columns.Where(x => x.QuotaIds != null && x.QuotaIds.Contains(quotaId)).Select(x => x.Name).ToArray()
            }).ToArray();

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);
        }

        private void AssertIndexes()
        {
            var db = new DatabaseEngine();
            var serverConnection = ServerConnectionFactory.Create(db.ConnectionString);
            var server = new Server(serverConnection);
            var database = server.Databases[serverConnection.DatabaseName];
            var tableName = ReplicationSchemaService.GetDestinationTableName(_surveyid);
            var table = database.Tables[tableName];
            var firstColumns = new List<string>(3);
            firstColumns.AddRange(from Index index in table.Indexes select index.IndexedColumns[0].Name);
            CollectionAssert.IsSubsetOf(new[] { "q1", "q2", "q3" }, firstColumns);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void IndexRecreating_IndexesForEachColumnExist()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable();
            UpdateSurveyReplicationScheme(testData);
            AssertIndexes();
            SetQuotaBalancing(_surveyid, 1, 10, new[] { "q2" }, 10);
            AssertIndexes();
        }

        public void SetQuotaBalancing(int surveyId, int quotaId, int promotionPriority, string[] filterFields,int promotionThreshold)
        {
            var configuration = _quotaBalancingService.GetQuotaBalancingConfiguration(surveyId);
            configuration.Quotas.Single(x => x.QuotaId == quotaId).IsEnabled = true;

            foreach (var field in configuration.Fields)
            {
                field.IsEnabled = filterFields.Contains(field.FieldName);
            }

            configuration.PromotionPriority = promotionPriority;
            configuration.PromotionThreshold = promotionThreshold;

            _quotaBalancingService.SetQuotaBalancingConfiguration(surveyId, configuration);
        }
    }
}
