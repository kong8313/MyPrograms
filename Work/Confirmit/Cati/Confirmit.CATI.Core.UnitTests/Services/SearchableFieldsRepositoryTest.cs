using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class SearchableFieldsRepositoryTest
    {
        [TestMethod, Owner ("SvetlanaT")]
        public void UpdateFieldsAfterReplication_ChangeField_FieldIsUpdated()
        {
            var replicationColums = new List<BvReplicationColumnsEntity>
                {
                    new BvReplicationColumnsEntity { ColumnID = 1, TableID = 4 },
                };
            var newTables = new List<BvReplicationTablesEntity>
                {
                    new BvReplicationTablesEntity { ID = 4, TableName = "table1" },
                };
            var oldTables = new List<BvReplicationTablesEntity>
                {
                    new BvReplicationTablesEntity { ID = 1, TableName = "table1" },
                };
            var searchableColumns = new List<BvSearchableFieldsEntity>
                {
                    new BvSearchableFieldsEntity { ColumnId = 1, TableId = 1 },
                };
            IEnumerable<BvSearchableFieldsEntity> expected = new List<BvSearchableFieldsEntity>
            {
                    new BvSearchableFieldsEntity { ColumnId = 1, TableId = 4 },
                };
            IEnumerable<BvSearchableFieldsEntity> result = SearchableFieldsRepository.GetUpdatedSearchableColumns(
                replicationColums, newTables, oldTables, searchableColumns);

            Assert.IsTrue(expected.SequenceEqual(result, new BvSearchableFieldsEntityEqualityComparer())); 
        }

        [TestMethod, Owner("SvetlanaT")]
        public void UpdateFieldsAfterReplication_InterviewerFieldWasRemovedFromReplicated_RemovedFromSearchable()
        {
            var replicationColums = new List<BvReplicationColumnsEntity>
                {
                    new BvReplicationColumnsEntity { ColumnID = 1, TableID = 4 }
                };
            var newTables = new List<BvReplicationTablesEntity>
                {
                    new BvReplicationTablesEntity { ID = 4, TableName = "table1" },
                };
            var oldTables = new List<BvReplicationTablesEntity>
                {
                    new BvReplicationTablesEntity { ID = 1, TableName = "table1" },
                };
            var searchableColumns = new List<BvSearchableFieldsEntity>
                {
                    new BvSearchableFieldsEntity { ColumnId = 1, TableId = 1 },
                    new BvSearchableFieldsEntity { ColumnId = 2, TableId = 1 },
                };
            IEnumerable<BvSearchableFieldsEntity> expected = new List<BvSearchableFieldsEntity>
            {
                    new BvSearchableFieldsEntity { ColumnId = 1, TableId = 4 },
                };
            IEnumerable<BvSearchableFieldsEntity> result = SearchableFieldsRepository.GetUpdatedSearchableColumns(
                replicationColums, newTables, oldTables, searchableColumns);

            Assert.IsTrue(expected.SequenceEqual(result, new BvSearchableFieldsEntityEqualityComparer()));
        }

        [TestMethod, Owner("SvetlanaT")]
        public void UpdateFieldsAfterReplication_TwoFieldsFromDifferentTables_BothUpdated()
        {
            var replicationColums = new List<BvReplicationColumnsEntity>
                {
                    new BvReplicationColumnsEntity { ColumnID = 1, TableID = 4 },
                    new BvReplicationColumnsEntity { ColumnID = 2, TableID = 5 }
                };
            var newTables = new List<BvReplicationTablesEntity>
                {
                    new BvReplicationTablesEntity { ID = 4, TableName = "table1" },
                    new BvReplicationTablesEntity { ID = 5, TableName = "table2" }
                };
            var oldTables = new List<BvReplicationTablesEntity>
                {
                    new BvReplicationTablesEntity { ID = 1, TableName = "table1" },
                    new BvReplicationTablesEntity { ID = 2, TableName = "table2" },
                };
            var searchableColumns = new List<BvSearchableFieldsEntity>
                {
                    new BvSearchableFieldsEntity { ColumnId = 1, TableId = 1 },
                    new BvSearchableFieldsEntity { ColumnId = 2, TableId = 2 },
                };
            IEnumerable<BvSearchableFieldsEntity> expected = new List<BvSearchableFieldsEntity>
            {
                    new BvSearchableFieldsEntity { ColumnId = 1, TableId = 4 },
                    new BvSearchableFieldsEntity { ColumnId = 2, TableId = 5 },
                };
            IEnumerable<BvSearchableFieldsEntity> result = SearchableFieldsRepository.GetUpdatedSearchableColumns(
                replicationColums, newTables, oldTables, searchableColumns);

            Assert.IsTrue(expected.SequenceEqual(result, new BvSearchableFieldsEntityEqualityComparer()));
        }

    }

    internal class BvSearchableFieldsEntityEqualityComparer : IEqualityComparer<BvSearchableFieldsEntity>
    {
        public bool Equals(BvSearchableFieldsEntity x, BvSearchableFieldsEntity y)
        {
            return x.ColumnId == y.ColumnId && x.TableId == y.TableId;
        }

        public int GetHashCode(BvSearchableFieldsEntity obj)
        {
            return obj.ColumnId.GetHashCode() ^ obj.TableId.GetHashCode();
        }
    }
}
