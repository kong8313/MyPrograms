using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using Confirmit.CATI.Core.DAL.Framework;
using System.Data;
using System.Text;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public class ReplicationIndexService : IReplicationIndexService
    {
        const string clusteredQuotaIndexNameformat = "IX_{0}_{1}";

        public void AddClusteredIndex(string tableName, string columnName)
        {
            string indexName = String.Format(clusteredQuotaIndexNameformat, tableName, columnName);

            var query = $"CREATE CLUSTERED INDEX [{indexName}] ON [dbo].[{tableName}] ( [{columnName}] )";
            new DatabaseEngine().ExecuteNonQuery(query);
        }

        private void DeleteNonClusteredIndex(string tableName, string indexName)
        {
            var query = $"DROP INDEX IF EXISTS [{indexName}] ON [dbo].[{tableName}]";
            new DatabaseEngine().ExecuteNonQuery(query);
        }

        public void ChangeOrderOfIndexColumns(int surveySid, int quotaId, string[] firstIndexColumns)
        {
            using (var transaction = new DatabaseTransactionScope("RecreateQuotaIndex"))
            {
                var tableName = ReplicationSchemaService.GetDestinationTableName(surveySid);
                var indexName = GetQuotaIndexName(quotaId);

                var indexFields = GetIndexFields(tableName, quotaId).ToArray();
                var firstIndexColumnsOrdered = firstIndexColumns.OrderBy(x => x);
                var indexFieldsOrdered = indexFields.Where(x => !x.IsIncluded).Take(firstIndexColumns.Length).Select(x => x.Name).OrderBy(x => x);

                if (firstIndexColumnsOrdered.SequenceEqual(indexFieldsOrdered))
                {
                    return;
                }
                
                DeleteNonClusteredIndex(tableName, indexName);

                CreateNonClusteredIndex(new ReplicationSchemaIndex()
                {
                    TableName = tableName,
                    Name = GetQuotaIndexName(quotaId),
                    IndexedColumnNames = firstIndexColumns.Union(indexFields.Where(x => !x.IsIncluded).Select(x => x.Name)).ToArray(),
                    IncludedColumnNames = indexFields.Where(x => x.IsIncluded).Select(x => x.Name).ToArray()
                });

                if (firstIndexColumns[0] != indexFields[0].Name)
                {
                    CreateNonClusteredIndex(new ReplicationSchemaIndex()
                    {
                        TableName = tableName,
                        Name = GetColumnIndexName(indexFields[0].Name),
                        IndexedColumnNames = new[] { indexFields[0].Name }
                    });

                    DeleteNonClusteredIndex(tableName, GetColumnIndexName(firstIndexColumns[0]));
                }
            
                transaction.Commit();
            }
        }

        public IEnumerable<IndexedColumnInfo> GetIndexFields(string tableName, int quotaId)
        {
            var indexName = GetQuotaIndexName(quotaId);

            var query = $@"
SELECT c.name, CASE WHEN key_ordinal > 0 THEN 'False' ELSE 'True' END as IsIncluded
FROM sys.index_columns ic
INNER JOIN sys.columns c ON c.object_id = ic.object_id and c.column_id = ic.column_id
INNER JOIN sys.indexes i ON ic.index_id = i.index_id AND ic.object_id = i.object_id AND i.name='{indexName}'
WHERE ic.object_id = object_id('[dbo].[{tableName}]')
ORDER BY index_column_id
";
            var data = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text);
            foreach (DataRow row in data.Rows)
            {
                yield return new IndexedColumnInfo((string)row["Name"], Convert.ToBoolean(row["IsIncluded"]));
            }
        }

        public string GetColumnIndexName(string columnName)
        {
            return $"IX_repl_{columnName}";
        }

        public string GetQuotaIndexName(int quotaId)
        {
            return $"IX_Quota{quotaId}";
        }

        public void CreateNonClusteredIndex(ReplicationSchemaIndex rsIndex)
        {
            var keyColumns = new StringBuilder("(");
            foreach (var columnName in rsIndex.IndexedColumnNames)
            {
                keyColumns.Append($"[{columnName}],");
            }

            keyColumns.Append(")");

            var includedColumns = new StringBuilder();
            foreach (var columnName in rsIndex.IncludedColumnNames)
            {
                includedColumns.Append($"[{columnName}],");
            }

            var includedColumnsStr = string.Empty;
            if (includedColumns.Length > 0)
            {
                includedColumnsStr = $"INCLUDE ({includedColumns.ToString().TrimEnd(',')})";
            }

            var query = $"CREATE NONCLUSTERED INDEX [{rsIndex.Name}] ON [dbo].[{rsIndex.TableName}] {keyColumns.ToString().Replace(",)", ")")} {includedColumnsStr}";
            new DatabaseEngine().ExecuteNonQuery(query);
        }

        public string GetNameOfRespondentUpdateTrigger(string tableName)
        {
            return "tr_respondent_update_on_" + tableName;
        }

        public string GetBodyOfRespondentUpdateTrigger(int surveySid)
        {
            return $@"
                IF UPDATE([TelephoneNumber]) 
						OR UPDATE([RespondentName]) 
						OR UPDATE([ExtensionNumber])
						OR UPDATE([TimeZoneId])
						OR UPDATE([DialType])
				BEGIN

				CREATE TABLE #InsertedTable (
					respid INT NULL INDEX IX_respid CLUSTERED,
					[TelephoneNumber] NVARCHAR(255) NULL,
					[RespondentName] NVARCHAR(255) NULL, 
					[ExtensionNumber] NVARCHAR(255) NULL,
					[TimeZoneId] INT NULL,
					[DialType] INT NULL
				);

				CREATE TABLE #BatchTable(
					respid INT NULL INDEX IX_respid CLUSTERED,
					[TelephoneNumber] NVARCHAR(255) NULL,
					[RespondentName] NVARCHAR(255) NULL, 
					[ExtensionNumber] NVARCHAR(255) NULL,
					[TimeZoneId] INT NULL,
					[DialType] INT NULL
				);
				
				INSERT INTO #InsertedTable
				SELECT respid, [TelephoneNumber], [RespondentName], [ExtensionNumber], CAST([TimeZoneId] as INT), CAST([DialType] as INT)
				FROM inserted

                DECLARE @lastProcessedRespId INT = 0
                DECLARE @batchSize INT = 1000
                    
                WHILE(1=1)--batches are needed to not block other queries to BvInterview and BvSvySchedule tables
                BEGIN

                    TRUNCATE TABLE #BatchTable

					INSERT INTO #BatchTable
					SELECT TOP(@batchSize) respid, [TelephoneNumber], [RespondentName], [ExtensionNumber], CAST([TimeZoneId] as INT), CAST([DialType] as INT)
					FROM #InsertedTable WHERE respid > @lastProcessedRespId ORDER BY respid
                    SELECT TOP(1) @lastProcessedRespId = respid FROM #BatchTable ORDER BY respid DESC

                    IF(NOT EXISTS(SELECT 1 FROM #BatchTable))
						BREAK                    

                    IF UPDATE([TimeZoneId])
                    BEGIN
                        -- Set 0 for not existed timezone ids 
                        UPDATE #BatchTable
                            SET [TimeZoneId] = 0
                            FROM #BatchTable it LEFT JOIN BvTimezoneMaster tzm ON it.[TimeZoneId] = tzm.ID
							LEFT JOIN BvTimezone tz ON it.[TimeZoneId] = tz.ID
                            WHERE tzm.ID IS NULL AND [TimeZoneId] IS NOT NULL AND tz.ParentID IS NULL

                        -- Activate timezones if needed with temporary table usage
                        DECLARE @TimeZonesToActivate TABLE (
                            TimeZoneId INT PRIMARY KEY CLUSTERED
                        );
                        
                        INSERT INTO @TimeZonesToActivate (TimeZoneId)
                        SELECT DISTINCT it.[TimeZoneId]
                        FROM #BatchTable it
                        LEFT JOIN BvTimezone tz ON it.[TimeZoneId] = tz.ID
                        LEFT JOIN BvTimezoneMaster tzm ON it.[TimeZoneId] = tzm.ID
                        WHERE tz.ID IS NULL AND tzm.ID IS NOT NULL;

                        -- Call BvSpTimezone_Activate for each timezone to activate
                        DECLARE @TimeZoneId INT;

                        WHILE EXISTS (SELECT 1 FROM @TimeZonesToActivate)
                        BEGIN
                            SELECT TOP 1 @TimeZoneId = TimeZoneId
                            FROM @TimeZonesToActivate
                            ORDER BY TimeZoneId;

                            EXEC BvSpTimezone_Activate @TimeZoneId;

                            DELETE FROM @TimeZonesToActivate
                            WHERE TimeZoneId = @TimeZoneId;
                        END;
                    END

				    UPDATE [BvInterview]
		                SET [TelephoneNumber] = t.[TelephoneNumber],
			                [RespondentName] = t.[RespondentName],
			                [ExtensionNumber] = t.[ExtensionNumber],
			                [TimeZoneId] = CASE WHEN t.[TimeZoneId] = 0 THEN NULL ELSE ISNULL(t.[TimeZoneId], i.[TimeZoneId]) END,
			                [DialTypeId] = ISNULL(t.[DialType], 0)
		                FROM [BvInterview] i INNER JOIN #BatchTable t ON i.[SurveySID] = {surveySid} AND i.[ID] = t.[respid]
		                WHERE ISNULL(i.[TelephoneNumber], '') <> ISNULL(t.[TelephoneNumber], '') OR
			                ISNULL(i.[RespondentName], '') <> ISNULL(t.[RespondentName], '') OR
			                ISNULL(i.[ExtensionNumber], '') <> ISNULL(t.[ExtensionNumber], '') OR
			                ISNULL(i.[TimeZoneId], 0) <> ISNULL(t.[TimeZoneId], 0) OR
			                ISNULL(i.[DialTypeId], 0) <> ISNULL(t.[DialType], 0)                     

                    -- Note: if inserted.[DialType] is null we will not update BvSvySchedule
	                UPDATE [BvSvySchedule]
		                SET [DialTypeId] = t.[DialType]
		                FROM [BvSvySchedule] s INNER JOIN #BatchTable t ON s.[SurveySID] = {surveySid} AND s.[InterviewID] = t.[respid]
		                WHERE s.[DialTypeId] <> t.[DialType]

                    -- Note: if s.[ShiftTypeID] > 0 or = -2147483648 we will not update BvSvySchedule
	                UPDATE [BvSvySchedule]
		                SET [ShiftTypeID] = -t.[TimeZoneId]
		                FROM [BvSvySchedule] s INNER JOIN #BatchTable t ON s.[SurveySID] = {surveySid} AND s.[InterviewID] = t.[respid]
		                WHERE t.[TimeZoneId] IS NOT NULL AND s.[ShiftTypeID] < 1 AND s.[ShiftTypeID] > -2147483648

	                UPDATE [BvAppointment]
		                SET [TZID] = CASE WHEN t.[TimeZoneId] IS NOT NULL AND t.[TimeZoneId] <> 0 THEN t.[TimeZoneId] ELSE a.[TZID] END
		                FROM [BvAppointment] a INNER JOIN #BatchTable t ON a.[SurveySID] = {surveySid} AND a.[InterviewSID] = t.[respid]
		                WHERE ISNULL(a.[TZID], 0) <> ISNULL(t.[TimeZoneId], 0)
                END
                END";
        }
    }

    public class ReplicationSchemaIndex
    {
        public string TableName;
        public string Name;
        public string[] IndexedColumnNames;
        public string[] IncludedColumnNames = {};
    }
    
    public class IndexedColumnInfo
    {
        public string Name { get; set; }

        public bool IsIncluded { get; set; }

        public IndexedColumnInfo(string name, bool isIncluded)
        {
            Name = name;
            IsIncluded = isIncluded;
        }
    }

}
