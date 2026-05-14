using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Runtime.Caching;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.Core.Services
{
    public class SurveyDatabaseService : ISurveyDatabaseService
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;
        private readonly MemoryCache _customFieldsCache = MemoryCache.Default;
        private CacheItemPolicy CreateCachePolicy() => new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1)
        };

        public SurveyDatabaseService(ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public int IncrementCallAttemptCount(int surveyId, int interviewId)
        {
            const string query = @"
                    DECLARE @tmp TABLE (CallAttemptCount INT PRIMARY KEY);

                    UPDATE <Schema>.respondent 
                        SET CallAttemptCount = ISNULL( CallAttemptCount, 0 ) + 1, 
                            TotalAttempts = ISNULL( TotalAttempts, 0 ) + 1
                        OUTPUT INSERTED.CallAttemptCount INTO @tmp(CallAttemptCount)
                        WHERE respID = @RespId

                    SELECT * FROM @tmp";

            var sqlParams = new[] { new SqlParameter("@RespId", interviewId) };

            return _surveyDatabaseEngine.ExecuteScalar<int>(surveyId, query, sqlParams);
        }

        public int GetCallAttemptCount(int surveyId, int interviewId)
        {
            try
            {
                const string query = @"SELECT TRY_CAST(ISNULL(CallAttemptCount, 0) AS INT) FROM <Schema>.respondent WHERE respID = @RespId";
                var sqlParams = new[] { new SqlParameter("@RespId", interviewId) };

                return _surveyDatabaseEngine.ExecuteScalar<int>(surveyId, query, sqlParams);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error to get call attempt count. Exception details: " + ex);
                return 0;
            }
        }

        public void UpdateIts(int surveyId, int interviewId, int its)
        {
            const string query = @"
                    UPDATE <Schema>.response_control 
                        SET ITS = @Its 
                        WHERE respid = @RespId";
            var sqlParams = new[] { new SqlParameter("@Its", its), new SqlParameter("@RespId", interviewId) };

            _surveyDatabaseEngine.ExecuteNonQuery(surveyId, query, sqlParams);
        }

        public void UpdateTimeZoneId(int surveyId, int interviewId, int timeZoneId)
        {
            const string query = @"
                    UPDATE <Schema>.respondent
                        SET TimeZoneId = @TimeZoneId 
                        WHERE respid = @RespId";
            var sqlParams = new[] { new SqlParameter("@TimeZoneId", timeZoneId), new SqlParameter("@RespId", interviewId) };

            _surveyDatabaseEngine.ExecuteNonQuery(surveyId, query, sqlParams);
        }

        private string GetValueFromResponseTable(int surveyId, int tableId, string fieldName, int interviewId)
        {
            var responseTable = $"response{tableId}";

            if (!ColumnExists(surveyId, responseTable, fieldName))
            {
                return null;
            }

            // Dynamically query the actual value from the response table
            var fieldValueQuery = $@"SELECT CAST([{fieldName}] AS NVARCHAR(MAX)) 
                                            FROM <Schema>.[{responseTable}]
                                            WHERE respid = @RespId";

            var sqlParams = new[] { new SqlParameter("@RespId", interviewId) };
            return _surveyDatabaseEngine.ExecuteScalar<string>(surveyId, fieldValueQuery, sqlParams);
        }
        
        private bool ColumnExists(int surveyId, string tableName, string columnName)
        {
            var cacheKey = $"FieldExists_{surveyId}_{tableName}_{columnName}";
            
            var newCacheEntry = new Lazy<bool>(() =>
            {
                const string query = @"
                    SELECT COUNT(1)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = '<Schema>' AND TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";

                var sqlParams = new[]
                {
                    new SqlParameter("@TableName", tableName),
                    new SqlParameter("@ColumnName", columnName)
                };

                var count = _surveyDatabaseEngine.ExecuteScalar<int>(surveyId, query, sqlParams);
                return count > 0;
            }, LazyThreadSafetyMode.PublicationOnly);
            
            var result = (Lazy<bool>)_customFieldsCache.AddOrGetExisting(cacheKey, newCacheEntry, CreateCachePolicy());
            return (result ?? newCacheEntry).Value;
        }

        /// <summary>
        /// Checks if multiple columns exist in a table with a single SQL query.
        /// Returns a dictionary mapping column names to their existence status.
        /// Uses cache for already-checked columns and only queries uncached ones.
        /// </summary>
        private Dictionary<string, bool> ColumnsExistBatch(int surveyId, string tableName, IEnumerable<string> columnNames)
        {
            var result = new Dictionary<string, bool>();
            var uncachedColumns = new List<string>();
            
            // Check cache first
            foreach (var columnName in columnNames)
            {
                var cacheKey = $"FieldExists_{surveyId}_{tableName}_{columnName}";
                var cachedValue = _customFieldsCache.Get(cacheKey) as Lazy<bool>;
                
                if (cachedValue != null)
                {
                    result[columnName] = cachedValue.Value;
                }
                else
                {
                    uncachedColumns.Add(columnName);
                }
            }
            
            // If all columns are cached, return early
            if (!uncachedColumns.Any())
                return result;
            
            // Query uncached columns in a single SQL request
            var columnNamesParam = string.Join(",", uncachedColumns.Select((_, i) => $"@ColumnName{i}"));
            var query = $@"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = '<Schema>' 
                    AND TABLE_NAME = @TableName 
                    AND COLUMN_NAME IN ({columnNamesParam})";
            
            var sqlParams = new List<SqlParameter> { new SqlParameter("@TableName", tableName) };
            for (int i = 0; i < uncachedColumns.Count; i++)
            {
                sqlParams.Add(new SqlParameter($"@ColumnName{i}", uncachedColumns[i]));
            }
            
            var dataTable = _surveyDatabaseEngine.ExecuteQuery(surveyId, query, sqlParams.ToArray());
            var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                existingColumns.Add(row["COLUMN_NAME"].ToString());
            }
            
            // Cache and add results for uncached columns
            foreach (var columnName in uncachedColumns)
            {
                var exists = existingColumns.Contains(columnName);
                result[columnName] = exists;
                
                // Add to cache
                var cacheKey = $"FieldExists_{surveyId}_{tableName}_{columnName}";
                var cacheEntry = new Lazy<bool>(() => exists, LazyThreadSafetyMode.PublicationOnly);
                _customFieldsCache.AddOrGetExisting(cacheKey, cacheEntry, CreateCachePolicy());
            }
            
            return result;
        }

        public List<string> ProcessRespondentFieldsBatch(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields)
        {
            // Initialize result list with 5 null values (for Custom1-Custom5)
            var result = new List<string> { null, null, null, null, null };
            
            if (fields == null || !fields.Any())
                return result;
            
            var validFields = new List<BvHistoryCustomFieldsEntity>();
            
            // Check which fields exist (batch operation - single SQL query for uncached fields)
            var columnNames = fields.Select(f => f.SourceFieldName).ToList();
            var existenceMap = ColumnsExistBatch(surveyId, "respondent", columnNames);
            
            foreach (var field in fields)
            {
                if (existenceMap.TryGetValue(field.SourceFieldName, out var exists) && exists)
                {
                    validFields.Add(field);
                }
            }
            
            if (!validFields.Any())
                return result;
            
            // Build dynamic query for all valid fields
            var selectColumns = string.Join(", ", validFields.Select(f => 
                $"CAST([{f.SourceFieldName}] AS NVARCHAR(MAX)) AS [{f.SourceFieldName}]"));
            
            var query = $@"
                SELECT {selectColumns}
                FROM <Schema>.respondent 
                WHERE respID = @RespId";
            
            var sqlParams = new[] { new SqlParameter("@RespId", interviewId) };
            
            // Execute query and get results as DataTable
            var dataTable = _surveyDatabaseEngine.ExecuteQuery(surveyId, query, sqlParams);
            
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                foreach (var field in validFields)
                {
                    if (row.Table.Columns.Contains(field.SourceFieldName))
                    {
                        var value = row[field.SourceFieldName];
                        if (value != null && value != DBNull.Value)
                        {
                            // Store value at index = fieldId - 1
                            var index = field.Id - 1;
                            if (index >= 0 && index < result.Count)
                            {
                                result[index] = value.ToString();
                            }
                        }
                    }
                }
            }
            
            return result;
        }

        public List<string> ProcessCallHistoryLoopFieldsBatch(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields)
        {
            // Initialize result list with 5 null values (for Custom1-Custom5)
            var result = new List<string> { null, null, null, null, null };
            
            if (fields == null || !fields.Any())
                return result;
            
            // Get the tableId for 'callhistoryinfo' field using cache
            var tableId = GetFieldResponseTableIdCached(surveyId, "callhistoryinfo");
            
            // If callhistoryinfo tableId doesn't exist, return empty result
            if (tableId == null)
                return result;
            
            var responseTable = $"response{tableId.Value}";
            var validFields = new List<BvHistoryCustomFieldsEntity>();
            
            // Check which fields exist in the response table (batch operation - single SQL query for uncached fields)
            var columnNames = fields.Select(f => f.SourceFieldName).ToList();
            var existenceMap = ColumnsExistBatch(surveyId, responseTable, columnNames);
            
            foreach (var field in fields)
            {
                if (existenceMap.TryGetValue(field.SourceFieldName, out var exists) && exists)
                {
                    validFields.Add(field);
                }
            }
            
            if (!validFields.Any())
                return result;
            
            // Build dynamic query for all valid fields
            var selectColumns = string.Join(", ", validFields.Select(f => 
                $"CAST([{f.SourceFieldName}] AS NVARCHAR(MAX)) AS [{f.SourceFieldName}]"));
            
            var query = $@"
                SELECT TOP 1 {selectColumns}
                FROM <Schema>.[{responseTable}]
                WHERE respid = @RespId 
                ORDER BY callhistoryinfo DESC";
            
            var sqlParams = new[] { new SqlParameter("@RespId", interviewId) };
            
            // Execute query and get results as DataTable
            var dataTable = _surveyDatabaseEngine.ExecuteQuery(surveyId, query, sqlParams);
            
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                foreach (var field in validFields)
                {
                    if (row.Table.Columns.Contains(field.SourceFieldName))
                    {
                        var value = row[field.SourceFieldName];
                        if (value != null && value != DBNull.Value)
                        {
                            // Store value at index = fieldId - 1
                            var index = field.Id - 1;
                            if (index >= 0 && index < result.Count)
                            {
                                result[index] = value.ToString();
                            }
                        }
                    }
                }
            }
            
            return result;
        }
        
        public string ProcessResponseField(int surveyId, int interviewId, BvHistoryCustomFieldsEntity field)
        {
            if (field == null)
                return null;
            
            var tableId = GetFieldResponseTableIdCached(surveyId, field.SourceFieldName);
            
            if (tableId == null)
                return null;
            
            return GetValueFromResponseTable(surveyId, tableId.Value, field.SourceFieldName, interviewId);
        }
        
        public List<string> GetCustomFieldValues(int surveySID, int interviewID)
        {
            // Initialize result list with 5 null values (for Custom1-Custom5)
            // Field IDs are always 1-5, so index = fieldId - 1
            var result = new List<string> { null, null, null, null, null };
            
            try
            {
                var customFields = GetActiveCustomFieldsFromCache();
            
                // Group fields by source table for optimized processing
                var respondentFields = customFields.Where(f => f.SourceTable == (int)CallHistoryCustomFieldSourceTable.Respondent).ToList();
                var responseFields = customFields.Where(f => f.SourceTable == (int)CallHistoryCustomFieldSourceTable.Response).ToList();
                var callHistoryLoopFields = customFields.Where(f => f.SourceTable == (int)CallHistoryCustomFieldSourceTable.CallHistoryLoop).ToList();
            
                // Process respondent fields in batch - returns values at correct indices
                if (respondentFields.Any())
                {
                    result = ProcessRespondentFieldsBatch(surveySID, interviewID, respondentFields);
                }
            
                // Process response fields - populate specific indices
                foreach (var field in responseFields)
                {
                    var value = ProcessResponseField(surveySID, interviewID, field);
                    if (value != null)
                    {
                        var index = field.Id - 1;
                        if (index >= 0 && index < result.Count)
                        {
                            result[index] = value;
                        }
                        else
                        {
                            Trace.TraceWarning("Custom field Id {0} is out of range (expected 1-5) for survey {1}, interview {2}",
                                field.Id, surveySID, interviewID);
                        }
                    }
                }
            
                // Process call history loop fields in batch - returns values at correct indices
                if (callHistoryLoopFields.Any())
                {
                    var loopValues = ProcessCallHistoryLoopFieldsBatch(surveySID, interviewID, callHistoryLoopFields);
                    // Only update non-null values from loop processing
                    for (int i = 0; i < loopValues.Count && i < result.Count; i++)
                    {
                        if (loopValues[i] != null)
                        {
                            result[i] = loopValues[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error retrieving custom field values for survey {0}, interview {1}. Exception: {2}",
                    surveySID, interviewID, ex);
            }
        
            return result;
        }
        
        private List<BvHistoryCustomFieldsEntity> GetActiveCustomFieldsFromCache()
        {
            var cacheKey = "ActiveCustomFields";

            var newCacheEntry = new Lazy<List<BvHistoryCustomFieldsEntity>>(() =>
            {
                return BvHistoryCustomFieldsAdapter
                    .GetByCondition("IsActive = 1")
                    .ToList();
            }, LazyThreadSafetyMode.PublicationOnly);

            var result = (Lazy<List<BvHistoryCustomFieldsEntity>>)_customFieldsCache
                .AddOrGetExisting(cacheKey, newCacheEntry, CreateCachePolicy());

            return (result ?? newCacheEntry).Value;
        }
        
        private int? GetFieldResponseTableIdCached(int surveyId, string fieldName)
        {
            var cacheKey = $"ResponseTableId_{surveyId}_{fieldName}";
            
            var newCacheEntry = new Lazy<int?>(() =>
            {
                var query = @"SELECT tableid 
                                 FROM <Schema>.[field]
                                 WHERE fieldname = @FieldName";
                
                var sqlParams = new[] { new SqlParameter("@FieldName", fieldName) };
                return _surveyDatabaseEngine.ExecuteScalar<int?>(surveyId, query, sqlParams);
            }, LazyThreadSafetyMode.PublicationOnly);
            
            var result = (Lazy<int?>)_customFieldsCache.AddOrGetExisting(cacheKey, newCacheEntry, CreateCachePolicy());
            return (result ?? newCacheEntry).Value;
        }
    }
}
