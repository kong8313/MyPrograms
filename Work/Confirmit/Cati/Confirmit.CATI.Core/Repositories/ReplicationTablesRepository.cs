using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.Core.Repositories
{
    public static class ReplicationTablesRepository
    {
        /// <summary>
        /// Gets the list of ReplicationTables entities by survey SID.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <returns>The list of ReplicationTables entities.</returns>
        public static List<BvReplicationTablesEntity> GetBySurveyId(int surveySid)
        {
            return BvReplicationTablesAdapter.GetByCondition(
                "[SurveySid] = @SurveySid",
                new SqlParameter("@SurveySid", surveySid));
        }

        /// <summary>
        /// Gets the ReplicationTables entity by survey SID and table name.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The ReplicationTables entity.</returns>
        public static BvReplicationTablesEntity GetBySurveyIdAndTableName(int surveySid, string tableName)
        {
            return BvReplicationTablesAdapter.GetByCondition(
                "[SurveySid] = @SurveySid AND [TableName] = @TableName",
                new SqlParameter("@SurveySid", surveySid),
                new SqlParameter("@TableName", tableName))
                .FirstOrDefault();
        }

        /// <summary>
        /// Deletes the ReplicationTables entry by survey SID.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        public static void DeleteBySurveyId(int surveySid)
        {
            BvReplicationTablesAdapter.DeleteByCondition("[SurveySid] = @SurveySid", new SqlParameter("@SurveySid", surveySid));
        }

        /// <summary>
        /// Inserts the specified entity into the ReplicationTables table.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>ID of inserted entity.</returns>
        public static int Insert(BvReplicationTablesEntity entity)
        {
            BvReplicationTablesAdapter.Insert(entity);

            return GetBySurveyIdAndTableName(entity.SurveySid, entity.TableName).ID;
        }
    }
}