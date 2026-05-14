using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Core.DAL.Framework;
using System.Data.SqlClient;
using System.Data;

using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.DAL.Handmade.Adapter.Query
{
    public class GetMaxAndMinFilledCellsAdapter
    {
        private const string QueryTableNameFormat = "select tablename from <Schema>.quotas where quotaid = @quotaid";
        private const string QueryIsColumnExistsFormat = @"exec sp_executesql N'
SELECT COUNT(*) FROM sys.columns WHERE name = @columnName AND object_id = OBJECT_ID(@tableName)',
N'@tableName NVARCHAR(MAX), @columnName NVARCHAR(MAX)', @tableName = @tableName, @columnName = @columnName";

        private const string QueryGetMaxAndMinFilledCellsFormat =
@"exec sp_executesql N'
WITH priorityWeight(priority, weight) AS
(
    SELECT 0, 0.66f
    UNION 
    SELECT 1, 0.66f
    UNION 
    SELECT 2, 1.0f
    UNION 
    SELECT 3, 1.5f
),
cellData AS 
(   
    SELECT quotaId cellid,
           {1} as priority,
           counter as counter,
           limit * p.weight as limit
    FROM <Schema>.{0} q INNER JOIN priorityWeight p 
        ON {1} = p.priority
),
cellInfo AS
(
   SELECT	cellid, 
            priority,
			limit, 
			CASE WHEN limit = 0 OR limit <= counter THEN 100.00 
				ELSE (CAST(counter AS FLOAT)/CAST(limit AS FLOAT))*100.00 
			END filling
   FROM cellData
),
cellDisbalance AS
(
SELECT cellid, MAX(filling) OVER() - filling disbalance, limit, priority
FROM cellInfo
)
select top(@count) cellid, priority, disbalance, CAST( limit * @threshold / 100.00 AS FLOAT) CallsCountNeededToComplete
from cellDisbalance
where disbalance >= @threshold and priority <> 0
order by priority DESC, checksum(NewID())
', N'@count INT, @threshold INT', @count = @count, @threshold = @threshold
";

        private static string GetTableName(string surveyDbConnectionString, int quotaId, string schema)
        {
            return new DatabaseEngine(surveyDbConnectionString).ExecuteScalarInNewConnection<String>(
                QueryTableNameFormat.Replace("<Schema>", schema), 
                CommandType.Text, 
                new SqlParameter("@quotaid", quotaId));
        }

        private static bool IsColumnExists(string surveyDbConnectionString, string tableName, string columnName, string schema)
        {
            return new DatabaseEngine(surveyDbConnectionString).ExecuteScalarInNewConnection<int>(
                QueryIsColumnExistsFormat,
                CommandType.Text,
                new SqlParameter("@tableName", schema + "." + tableName),
                new SqlParameter("@columnName", columnName)) > 0;
        }

        public static IEnumerable<PromotedCellEntity> ExecuteEntityList(int quotaId, string quotaName, SurveyConnectionInfo surveyConnectionInfo, int count, float processedCallsCountPerCompletedCall, int threshold)
        {
            var surveyDbConnectionString = surveyConnectionInfo.ConnectionString;
            var schema = surveyConnectionInfo.SchemaName;
            string tableName = GetTableName(surveyDbConnectionString, quotaId, schema);

            bool isPriorityColumnExists = IsColumnExists(surveyDbConnectionString, tableName, "balancing_priority", schema);

            return
                new DatabaseEngine(surveyDbConnectionString).ExecuteDataTableInNewConnection<DataTable>(
                    string.Format(
                        QueryGetMaxAndMinFilledCellsFormat.Replace("<Schema>", schema), 
                        tableName,
                        isPriorityColumnExists ? "q.balancing_priority" : "2"),
                    CommandType.Text,
                    new SqlParameter("@count", count),
                    new SqlParameter("@threshold", threshold)).Select().Select(
                        x => new PromotedCellEntity 
                        { 
                            QuotaId = quotaId,
                            QuotaName = quotaName,
                            CellId = (int)x["cellid"],
                            Priority = (int)x["priority"],
                            CallsCountNeededToPromote = (double)x["CallsCountNeededToComplete"] * processedCallsCountPerCompletedCall,
                        });
        }
    }
}
