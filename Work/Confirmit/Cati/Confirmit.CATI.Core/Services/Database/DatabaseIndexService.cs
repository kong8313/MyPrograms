using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database
{
    public class DatabaseIndexService : IDatabaseIndexService
    {
        private readonly IDatabaseServerPropertiesProvider _databaseServerPropertiesProvider;

        public DatabaseIndexService(IDatabaseServerPropertiesProvider databaseServerPropertiesProvider)
        {
            _databaseServerPropertiesProvider = databaseServerPropertiesProvider;
        }

        public IEnumerable<IndexInfo> GetAllIndexes(string fragmentationDetectMode)
        {
            var dbEngine = new DatabaseEngine();

            string query = String.Format(@"
SELECT
  obj.name AS [TableName]
, ind.name AS [IndexName]
, SUM( CASE WHEN info.DATA_TYPE in ('TEXT', 'NTEXT','IMAGE' ,'FILESTREAM') THEN 1 ELSE 0 END ) as Lob
, MAX(ind.fill_factor) AS [FillFactor]
, MAX(part.rows) AS [RowCount]
, MAX(stat.avg_fragmentation_in_percent) AS [Fragmentation]
, SUM(stat.page_count) AS [PageCount]
FROM sys.dm_db_index_physical_stats(DB_ID(),NULL,NULL,NULL,'{0}') stat
INNER JOIN sys.objects obj
ON stat.object_id = obj.object_id
INNER JOIN sys.indexes ind
ON obj.object_id = ind.object_id AND stat.index_id = ind.index_id
INNER JOIN sys.partitions part
ON obj.object_id = part.object_id AND stat.index_id = part.index_id
INNER JOIN information_schema.columns info
ON obj.object_id = OBJECT_ID(info.TABLE_NAME)
WHERE ind.index_id > 0 --AND ind.name = 'PK_BvVersionHistory_Id'
GROUP BY obj.name, ind.name", fragmentationDetectMode ?? "LIMITED");

            using( var reader = dbEngine.ExecuteReaderInNewConnection(query, CommandType.Text) )
            {
                foreach (var indexInfo in ReadIndexInfos(reader))
                {
                    yield return indexInfo;
                }
            }
        }

        private static IEnumerable<IndexInfo> ReadIndexInfos(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return new IndexInfo
                {
                    TableName = GetString(reader["TableName"]),
                    IndexName =  GetString(reader["IndexName"]),
                    ContainsLob = GetInt32(reader["Lob"]) > 0,
                    FillFactor = GetInt32(reader["FillFactor"]),
                    RowCount = GetInt64(reader["RowCount"]),
                    Fragmentation = GetDouble(reader["Fragmentation"]),
                    PageCount = GetInt64(reader["PageCount"])
                };
            }
        }

        public IndexInfo GetIndex(string tableName, string indexName, string fragmentationDetectMode)
        {
            var dbEngine = new DatabaseEngine();

            string query = String.Format(@"
;WITH data as 
(
	SELECT  object_id as TableObjectId,
			fill_factor as [FillFactor],
			index_id as IndexId
		from sys.indexes 
		WHERE name = @IndexName AND object_id = OBJECT_ID( @TableName, 'U')
)
SELECT
  @TableName AS [TableName]
, @IndexName AS [IndexName]
, SUM( CASE WHEN info.DATA_TYPE in ('TEXT', 'NTEXT','IMAGE' ,'FILESTREAM') THEN 1 ELSE 0 END ) as Lob
, MAX([FillFactor]) AS [FillFactor]
, MAX(part.rows) AS [RowCount]
, MAX(stat.avg_fragmentation_in_percent) AS [Fragmentation]
, SUM(stat.page_count) AS [PageCount]
FROM data
INNER JOIN information_schema.columns info
ON data.TableObjectId = OBJECT_ID(info.TABLE_NAME)
CROSS APPLY sys.dm_db_index_physical_stats(DB_ID(),TableObjectId,IndexId,NULL,'{0}') stat
INNER JOIN sys.partitions part
ON TableObjectId = part.object_id AND stat.index_id = part.index_id", fragmentationDetectMode ?? "LIMITED");

            using (var reader = dbEngine.ExecuteReaderInNewConnection(query, CommandType.Text, 
                                        new SqlParameter("TableName", tableName), 
                                        new SqlParameter("IndexName", indexName)))
            {
                return ReadIndexInfos(reader).Single();
            }
        }

        public void ReorginizeIndex(string tableName, string indexName)
        {
            string query = String.Format( @"ALTER INDEX [{0}] ON [{1}] REORGANIZE", indexName, tableName);

            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }

        public bool IsRebuildIndexOnlineSupported()
        {
            return _databaseServerPropertiesProvider.GetEngineEdition() == EngineEdition.Enterprise &&
                   _databaseServerPropertiesProvider.GetProductVersion().Major >= (int)SqlServerMajorVersion.Sql2012;
        }

        public void RebuildIndex(string tableName, string indexName, bool containsLob)
        {
            if (IsRebuildIndexOnlineSupported() && !containsLob)
            {
                RebuildIndexOnline(tableName, indexName);
            }
            else
            {
                RebuildIndexOffline(tableName, indexName);
            }
        }

        public void RebuildIndexOffline(string tableName, string indexName)
        {
            string query = String.Format(@"ALTER INDEX [{0}] ON [{1}] REBUILD WITH ( ONLINE = OFF)", indexName, tableName);

            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }

        public void RebuildIndexOnline(string tableName, string indexName)
        {
            string query = String.Format(@"ALTER INDEX [{0}] ON [{1}] REBUILD WITH ( ONLINE = ON)", indexName, tableName);

            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }

        private static string GetString(object value)
        {
            return Convert.IsDBNull(value) ? null : Convert.ToString(value);
        }

        private static int GetInt32(object value)
        {
            return Convert.IsDBNull(value) ? 0 : Convert.ToInt32(value);
        }

        private static long GetInt64(object value)
        {
            return Convert.IsDBNull(value) ? 0 : Convert.ToInt64(value);
        }

        private static double GetDouble(object value)
        {
            return Convert.IsDBNull(value) ? 0 : Convert.ToDouble(value);
        }

    }
}
