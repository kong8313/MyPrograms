using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.Core.Repositories
{
    public static class ReplicationColumnsRepository
    {
        /// <summary>
        /// Gets all replication columns for given survey ID.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        public static List<BvReplicationColumnsEntity> GetBySurveyId(int surveySid)
        {
            var result = new List<BvReplicationColumnsEntity>();
            var replicationTableIds = ReplicationTablesRepository.GetBySurveyId(surveySid).Select(x => x.ID);

            foreach (int tableId in replicationTableIds)
            {
                result.AddRange(GetByTableId(tableId));
            }

            return result;
        }


        /// <summary>
        /// Gets all replication columns for given table.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        public static List<BvReplicationColumnsEntity> GetByTableId(int tableId)
        {
            return BvReplicationColumnsAdapter.GetByCondition("[TableID] = @TableID", new SqlParameter("@TableID", tableId));
        }

        /// <summary>
        /// Deletes records from BvReplicationColumns by table id.
        /// </summary>
        /// <param name="tableId">The table id.</param>
        public static void DeleteByTableId(int tableId)
        {
            BvReplicationColumnsAdapter.DeleteByCondition("[TableID] = @TableID", new SqlParameter("@TableID", tableId));
        }

        /// <summary>
        /// Inserts the specified entity into BvReplicationColumns.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public static void Insert(BvReplicationColumnsEntity entity)
        {
            BvReplicationColumnsAdapter.Insert(entity);
        }

        /// <summary>
        /// Gets replication column with given name of given survey ID.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="columnName">Column name.</param>
        /// <returns>BvReplicationColumnsEntity if exists; otherwise null.</returns>
        public static BvReplicationColumnsEntity GetBySurveyIdName(int surveyId, string columnName)
        {
            return GetBySurveyId(surveyId).Where(c => c.ColumnName == columnName).FirstOrDefault();
        }
    }
}