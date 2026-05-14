using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Repositories
{
    /// <summary>
    /// Provides methods to work with BvSearchableFields table
    /// </summary>
    public static class SearchableFieldsRepository
    {
        /// <summary>
        /// Check that this column name is one from "InterviewID", "RespondentName", "TelephoneNumber", "ITSName"
        /// </summary>
        /// <param name="columnName">Column name for check</param>with 
        /// <returns></returns>
        public static bool CheckPredefinedColumnName(string columnName)
        {
            var existingColumns = new[] { "InterviewID", "RespondentName", "TelephoneNumber", "ITSName" };

            return existingColumns.Contains(columnName);
        }

        /// <summary>
        /// Gets all available fields for given survey ID.
        /// </summary>
        /// <param name="surveyId">The survey SID.</param>
        private static List<BvSearchableFieldsEntity> GetBySurveyId(int surveyId)
        {
            return BvSearchableFieldsAdapter.GetByCondition("[SurveyId] = @SurveyId", new SqlParameter("@SurveyId", surveyId));
        }

        /// <summary>
        /// Gets all fields available for interviewers for given survey ID.
        /// </summary>
        /// <param name="surveyId">The survey SID.</param>
        public static List<BvSearchableFieldsEntity> GetSearchableFieldsForRole(int surveyId)
        {
            return BvSearchableFieldsAdapter.GetByCondition("[SurveyId] = @SurveyId",
                new SqlParameter("@SurveyId", surveyId));
        }

        /// <summary>
        /// Deletes records from BvReplicationColumns by table id.
        /// </summary>
        /// <param name="surveyId">The survey SID.</param>
        public static void DeleteFieldsForRoleBySurveyId(int surveyId)
        {
            BvSearchableFieldsAdapter.DeleteByCondition("[SurveyId] = @SurveyId", new SqlParameter("@SurveyId", surveyId));
        }

        /// <summary>
        /// Deletes records from BvReplicationColumns by table id.
        /// </summary>
        /// <param name="surveyId">The survey SID.</param>
        private static void DeleteBySurveyId(int surveyId)
        {
            BvSearchableFieldsAdapter.DeleteByCondition("[SurveyId] = @SurveyId", new SqlParameter("@SurveyId", surveyId));
        }

        /// <summary>
        /// Adds new record into BvSearchableFields
        /// </summary>
        public static void AddFieldForRole(int surveyId, int tableId, int columnId)
        {
            var replicatedColumns = ReplicationColumnsRepository.GetBySurveyId(surveyId);

            if (replicatedColumns.Exists(x => x.TableID == tableId && x.ColumnID == columnId) == false)
            {
                throw new UserMessageException("Survey schema has been changed by supervisor.");
            }

            var entity = new BvSearchableFieldsEntity
            {
                SurveyId = surveyId,
                TableId = tableId,
                ColumnId = columnId
            };

            Insert(entity);
        }

        private static void Add(BvSearchableFieldsEntity searchableFieldsEntity)
        {
            var replicatedColumns = ReplicationColumnsRepository.GetBySurveyId(searchableFieldsEntity.SurveyId);

            if (replicatedColumns.Exists(x => x.TableID == searchableFieldsEntity.TableId && x.ColumnID == searchableFieldsEntity.ColumnId) == false)
            {
                throw new UserMessageException("Survey schema has been changed from authoring.");
            }

            Insert(searchableFieldsEntity);
        }

        /// <summary>
        /// Inserts new entity into BvSearchableFields table
        /// </summary>
        /// <param name="entity">BvSearchableFieldsEntity</param>
        private static void Insert(BvSearchableFieldsEntity entity)
        {
            BvSearchableFieldsAdapter.Insert(entity);
        }

        /// <summary>
        /// Updates available fields
        /// </summary>
        /// <remarks>
        /// After replication TableId-s in bvReplicatedColumns are changed therefore corresponding fields in 
        /// bvSearchableFields table must be changed. Old identifier of table we can get using TableName field
        /// </remarks>
        /// <param name="surveyId">Survey identifier</param>
        /// <param name="oldTables">List of tables from the previous replication scheme for current survey.</param>
        internal static void UpdateFieldsAfterReplication(int surveyId, List<BvReplicationTablesEntity> oldTables)
        {
            var newTables = ReplicationTablesRepository.GetBySurveyId(surveyId);
            var replicatedColumns = ReplicationColumnsRepository.GetBySurveyId(surveyId);
            var availableColumns = GetBySurveyId(surveyId);

            IEnumerable<BvSearchableFieldsEntity> list = GetUpdatedSearchableColumns(replicatedColumns, newTables, oldTables, availableColumns);

            DeleteBySurveyId(surveyId);

            foreach (var column in list)
            {
                Add(column);
            }
        }

        internal static IEnumerable<BvSearchableFieldsEntity> GetUpdatedSearchableColumns(
            List<BvReplicationColumnsEntity> replicatedColumns,
            List<BvReplicationTablesEntity> newTables,
            List<BvReplicationTablesEntity> oldTables,
            List<BvSearchableFieldsEntity> searchableFieldsColumns)
        {
            var list = new List<BvSearchableFieldsEntity>();
            foreach (var replicationColumn in replicatedColumns)
            {
                BvReplicationColumnsEntity column = replicationColumn;
                string newTableName = null;
                int oldTableId = 0;

                if (newTables.Exists(x => x.ID == column.TableID))
                {
                    newTableName = newTables.First(x => x.ID == column.TableID).TableName;
                }

                if (oldTables.Exists(x => x.TableName == newTableName))
                {
                    oldTableId = oldTables.First(x => x.TableName == newTableName).ID;
                }

                var searchableColumns = searchableFieldsColumns.Where(x => x.TableId == oldTableId && x.ColumnId == column.ColumnID);

                foreach (var searchableColumn in searchableColumns)
                {
                    list.Add(new BvSearchableFieldsEntity
                               {
                                   TableId = column.TableID,
                                   ColumnId = column.ColumnID,
                                   SurveyId = searchableColumn.SurveyId
                               });
                }
            }

            return list;
        }

        /// <summary>
        /// Returns names of all available columns for specified survey
        /// </summary>        
        public static IEnumerable<string> GetSearchableColumnsNames(int surveyId, List<BvSearchableFieldsEntity> bvSearchableFieldsEntities)
        {
            var replicatedColumns = ReplicationColumnsRepository.GetBySurveyId(surveyId);

            return replicatedColumns.Where(x => bvSearchableFieldsEntities.Exists(y => y.TableId == x.TableID && y.ColumnId == x.ColumnID)).Select(x=>x.ColumnName);
        }
    }
}