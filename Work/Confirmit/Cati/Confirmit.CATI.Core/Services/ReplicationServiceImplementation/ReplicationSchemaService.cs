using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public class ReplicationSchemaService : IReplicationSchemaService, IReplicationSchemaInfoService
    {
        private readonly IQuotaBalancingService _quotaBalancingService;
        private readonly IReplicationIndexService _replicationIndexService;
        private readonly IDatabaseObjectService _databaseObjectService;

        public ReplicationSchemaService(
            IQuotaBalancingService quotaBalancingService,
            IReplicationIndexService replicationIndexService,
            IDatabaseObjectService databaseObjectService)
        {
            _quotaBalancingService = quotaBalancingService;
            _replicationIndexService = replicationIndexService;
            _databaseObjectService = databaseObjectService;
        }
        #region Fields

        /// <summary>
        /// Format string for the name of the table with the replicated data. Use 
        /// <see cref="GetDestinationTableName"/> method to get table name by survey SID.
        /// </summary>
        private const string ReplicatedDataTableNameFormat = "BvReplicatedData_{0}";

        /// <summary>
        /// Format string for the name of the table with temp data for transfer changes.
        /// <see cref="GetTransferChangesTableName"/> method to get table name by source replicated name and by survey SID.
        /// </summary>
        private const string TableForTransferChangesNameFormat = "BvReplicationChanges_{0}_{1}";


        /// <summary>
        /// primary key column in the 'respondent' table of CF survey DB.
        /// </summary>
        private static readonly ColumnInfo respIdColumnInfo = new ColumnInfo
        {
            Name = "respid",
            DataType = SqlDataType.Int
        };

        /// <summary>
        /// response id column in the 'response' table of CF survey DB.
        /// </summary>
        private static readonly ColumnInfo responseIdColumnInfo = new ColumnInfo
        {
            Name = "responseid",
            DataType = SqlDataType.Int
        };

        /// <summary>
        /// necessary column for temp table for transfer schanges from CF (look at changetable function)
        /// type of operation (I - insert U - update D - delete)
        /// </summary>
        private static readonly ColumnInfo sysChangeOperation = new ColumnInfo
        {
            Name = "SYS_CHANGE_OPERATION",
            DataType = SqlDataType.NChar,
            MaxLength = 1
        };

        /// <summary>
        /// necessary column for temp table for transfer schanges from CF (look at changetable function)
        /// changed columns
        /// </summary>
        private static readonly ColumnInfo sysChangeColumns = new ColumnInfo
        {
            Name = "SYS_CHANGE_COLUMNS",
            DataType = SqlDataType.VarBinary,
            MaxLength = 4100
        };

        #endregion


        string IReplicationSchemaInfoService.GetDestinationTableName(int surveySid)
        {
            return ReplicationSchemaService.GetDestinationTableName(surveySid);
        }

        /// <summary>
        /// Gets the name of the destination table for the replicated data. Simply name without any brackets and without DB name.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <returns>The name of the table with the replicated data.</returns>
        public static string GetDestinationTableName(int surveySid)
        {
            return String.Format(ReplicatedDataTableNameFormat, surveySid);
        }

        /// <summary>
        /// Gets the name of the temp table for the replicated table. Simply name without any brackets and without DB name.
        /// </summary>
        /// <param name="replicatedTableName">Name of source replicated table</param>
        /// <param name="surveySid">The survey SID.</param>
        /// <returns>The name of the table for transfer changes.</returns>
        public static string GetTransferChangesTableName(string replicatedTableName, int surveySid)
        {
            return String.Format(TableForTransferChangesNameFormat, replicatedTableName, surveySid);
        }

        /// <summary>
        /// Remove duplicated columns from response tables to prevent errors "Column names in each table must be unique" in BvReplicatedData_xxx table
        /// </summary>
        /// <param name="replicatedTables">Array of <see cref="TableInfo"/> objects with list of columns to replicate data</param>
        /// <returns></returns>
        private static TableInfo[] RemoveExtraColumns(TableInfo[] replicatedTables)
        {
            if (replicatedTables == null)
            {
                return null;
            }

            var result = new List<TableInfo>();

            var filteredColumns = replicatedTables.FirstOrDefault(x => x.Name.ToLower() == "respondent")?.ReplicationColumns.Select(x => x.Name).ToArray() ??
                new string[] { };

            foreach (var tableInfo in replicatedTables)
            {
                if (tableInfo.Name.ToLower() == "respondent")
                {
                    result.Add(tableInfo);
                    continue;
                }

                var newReplicationColumns = new List<ReplicationColumnInfo>();

                foreach (var columnInfo in tableInfo.ReplicationColumns)
                {
                    if (filteredColumns.Contains(columnInfo.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        Trace.TraceInformation($"Column '{columnInfo.Name}' for table '{tableInfo.Name}' was removed from replication schema");
                    }
                    else
                    {
                        newReplicationColumns.Add(columnInfo);
                    }
                }

                if (newReplicationColumns.Any())
                {
                    result.Add(new TableInfo
                    {
                        Name = tableInfo.Name,
                        PrimaryKeyColumns = tableInfo.PrimaryKeyColumns,
                        ReplicationColumns = newReplicationColumns.ToArray()
                    });
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Updates the survey replication scheme. Updates <c>BvSurvey</c>, 
        /// <c>BvReplicationTables</c>and <c>BvReplcationColumns</c> tables. Creates 
        /// <c>BvReplicatedData_'SurveySid</c>' table.
        /// </summary>
        /// <param name="surveySid">Survey SID</param>
        /// <param name="tables">Array of <see cref="TableInfo"/> objects with list of
        /// columns to replicate data. null or empty array means that all data should be
        /// removed.</param>
        /// <exception cref="InternalErrorException">Survey not found.</exception>
        /// <exception cref="ArgumentException">There is should be at least one table with
        /// '<c>respid</c>' column as a primary key.</exception>
        public void UpdateSurveyReplicationScheme(int surveySid, TableInfo[] tables)
        {
            tables = RemoveExtraColumns(tables);

            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetSurveyReplicationResourceName(surveySid),
                "ReplicationService.UpdateSurveyReplicationScheme",
                ServiceLocator.Resolve<ISystemSettings>().Replication.ForceReplicationLockTimeout))
            {
                if (!dbLock.TryEnterLock())
                {
                    return;
                }
                using (var dbTransactionScope = new DatabaseTransactionScope("UpdateSurveyReplicationScheme"))
                {
                    tables = tables ?? new TableInfo[0];

                    // Determines if we should create new table & fill data or just remove old.
                    bool needToCreate = tables.Length > 0;

                    UpdateQuotaBalancingConfiguration(surveySid, tables);

                    // Update BvSurvey
                    var survey = SurveyRepository.GetById(surveySid);

                    var primaryKeyNames = tables.SelectMany(x => x.PrimaryKeyColumns).Select(x => x.Name.ToLowerInvariant());

                    if (needToCreate && !primaryKeyNames.Contains(respIdColumnInfo.Name))
                    {
                        throw new ArgumentException(
                            string.Format("There is should be at least one table with '{0}' column as a primary key.", respIdColumnInfo.Name));
                    }

                    var emptyTable = tables.Where(x => x.ReplicationColumns.Length == 0).FirstOrDefault();
                    if (emptyTable != null)
                    {
                        throw new ArgumentException(
                            string.Format("There is should be at least one replication column for table {0}.", emptyTable.Name));
                    }

                    if (needToCreate && survey.DestinationTableName != GetDestinationTableName(surveySid))
                    {
                        survey.DestinationTableName = GetDestinationTableName(surveySid);
                        SurveyRepository.Update(survey);
                    }

                    var oldTables = ReplicationTablesRepository.GetBySurveyId(surveySid);

                    // Clean tables.
                    foreach (var table in ReplicationTablesRepository.GetBySurveyId(surveySid))
                    {
                        ReplicationColumnsRepository.DeleteByTableId(table.ID);
                    }

                    ReplicationTablesRepository.DeleteBySurveyId(surveySid);

                    // Update tables.
                    foreach (TableInfo table in tables)
                    {
                        int tableId = ReplicationTablesRepository.Insert(
                            new BvReplicationTablesEntity
                            {
                                SurveySid = surveySid,
                                TableName = table.Name,
                                PrimaryKey = string.Join(", ", table.PrimaryKeyColumns.Select(x => x.Name).ToArray())
                            });

                        foreach (ReplicationColumnInfo column in table.ReplicationColumns)
                        {
                            ReplicationColumnsRepository.Insert(
                                new BvReplicationColumnsEntity
                                {
                                    ColumnID = column.Id,
                                    ColumnName = column.Name,
                                    TableID = tableId,
                                    ColumnType = (int)column.DataType,
                                    ColumnMaxLength = (column.MaxLength == -1 ? null : (int?)column.MaxLength)
                                });
                        }
                    }

                    // Re-create table for replicated data.
                    var oldTablesNames = oldTables.Select(x => x.TableName).ToArray();
                    RecreateTableForData(surveySid, tables, needToCreate, oldTablesNames);

                    SearchableFieldsRepository.UpdateFieldsAfterReplication(surveySid, oldTables);

                    dbTransactionScope.Commit();
                }
            }
        }

        public void UpdateQuotaBalancingConfiguration(int surveySid, TableInfo[] tables)
        {
            bool needToCreate = tables.Length > 0;

            if (needToCreate)
                _quotaBalancingService.AdjustQuotaBalancingConfiguration(surveySid, tables);
            else
                _quotaBalancingService.ResetQuotaBalancingConfiguration(surveySid);
        }

        public void CreateCopyOfTableWithoutDataAndIndexes(string oldTableName, string newTableName, out string[] indexQueries)
        {
            _databaseObjectService.CopyColumnsAndConstraints(oldTableName, newTableName);

            indexQueries = _databaseObjectService.GetCreateIndexQueries(oldTableName, newTableName).ToArray();

            _databaseObjectService.CopyTriggers(oldTableName, newTableName);
        }

        /// <summary>
        /// Re-creates the table for the replicated data.
        /// </summary>
        /// <param name="surveySid">Survey SID</param>
        /// <param name="tables">The collection of schemes of the tables to create current table scheme on.</param>
        /// <param name="needToCreate">Determines if we should create new table or just remove old one.</param>
        /// <param name="oldTables">List of tables from the previous replication scheme for current survey.</param>
        private void RecreateTableForData(int surveySid, IEnumerable<TableInfo> tables, bool needToCreate, IEnumerable<string> oldTables)
        {
            var db = new DatabaseEngine();
            string query;
            using (var transactionScope = new DatabaseTransactionScope("CreateTableForReplicatedData"))
            {
                // Remove transfer changes tables
                foreach (var table in oldTables)
                {
                    string tableName = GetTransferChangesTableName(table, surveySid);
                    db.DropTable(tableName);
                }

                // Remove replicated table
                db.DropTable(GetDestinationTableName(surveySid));

                // Create new table
                if (needToCreate)
                {
                    var newTableName = GetDestinationTableName(surveySid);
                    var newTable = new StringBuilder($"CREATE TABLE [dbo].[{newTableName}](\r\n");
                    List<string> indexableColumnNames = new List<string>();
                    AddColumn(newTable, respIdColumnInfo.Name, new DataType(respIdColumnInfo.DataType));

                    foreach (TableInfo tableInfo in tables)
                    {
                        var transferTableName = GetTransferChangesTableName(tableInfo.Name, surveySid);
                        var transferTable = new StringBuilder($"CREATE TABLE [dbo].[{transferTableName}](\r\n");

                        // mandatory columns for changetables function:
                        // SYS_CHANGE_OPERATION nchar(1) 
                        // SYS_CHANGE_COLUMNS varbinary(4100)
                        AddColumn(transferTable, sysChangeOperation.Name, new DataType(sysChangeOperation.DataType, sysChangeOperation.MaxLength));
                        AddColumn(transferTable, sysChangeColumns.Name, new DataType(sysChangeColumns.DataType, sysChangeColumns.MaxLength));
                        AddColumn(transferTable, respIdColumnInfo.Name, new DataType(respIdColumnInfo.DataType));

                        foreach (ReplicationColumnInfo column in tableInfo.ReplicationColumns)
                        {
                            AddColumn(newTable, column.Name, GetColumnType(column));
                            if (IsColumnAvailableForSqlIndexing(column))
                                indexableColumnNames.Add(column.Name);
                            AddColumn(transferTable, column.Name, GetColumnType(column));
                        }

                        query = transferTable.ToString().TrimEnd(',', '\r', '\n') + "\r\n)";
                        db.ExecuteNonQuery(query);

                        _replicationIndexService.AddClusteredIndex(transferTableName, respIdColumnInfo.Name);
                    }

                    query = newTable.ToString().TrimEnd(',', '\r', '\n') + "\r\n)";
                    db.ExecuteNonQuery(query);

                    AddTriggerIfNeeded(newTableName, query, surveySid);

                    _replicationIndexService.AddClusteredIndex(newTableName, respIdColumnInfo.Name);

                    var indexes = GetNonClusteredIndexes(tables, surveySid, newTableName, indexableColumnNames);

                    foreach (var index in indexes)
                    {
                        _replicationIndexService.CreateNonClusteredIndex(index);
                    }
                }
            }
        }

        private void AddColumn(StringBuilder creationTableScript, string name, DataType dataType)
        {
            string dataTypeStr = dataType.SqlDataType == SqlDataType.NVarCharMax ? $"[nvarchar](max)" : $"[{dataType.Name}]";
            if (dataType.MaximumLength > 0)
            {
                dataTypeStr += $"({dataType.MaximumLength}),";
            }
            else if (dataType.NumericScale > 0 && dataType.NumericPrecision > 0)
            {
                dataTypeStr += $"({dataType.NumericPrecision}, {dataType.NumericScale}),";
            }
            else if (dataType.NumericScale > 0)
            {
                dataTypeStr += $"({dataType.NumericScale}),";
            }
            else if (dataType.NumericPrecision > 0)
            {
                dataTypeStr += $"({dataType.NumericPrecision}),";
            }
            else
            {
                dataTypeStr += ",";
            }

            creationTableScript.AppendLine($"\t[{name}] {dataTypeStr}");
        }

        private void AddTriggerIfNeeded(string tableName, string tableCreationScript, int surveySid)
        {
            if (!tableCreationScript.Contains("[TelephoneNumber]") ||
                !tableCreationScript.Contains("[RespondentName]") ||
                !tableCreationScript.Contains("[ExtensionNumber]") ||
                !tableCreationScript.Contains("[TimeZoneId]") ||
                !tableCreationScript.Contains("[DialType]"))
            {
                return;
            }

            var query = $@"
CREATE TRIGGER [dbo].[{_replicationIndexService.GetNameOfRespondentUpdateTrigger(tableName)}]
ON [dbo].[{tableName}] AFTER INSERT, UPDATE 
AS
BEGIN
    {_replicationIndexService.GetBodyOfRespondentUpdateTrigger(surveySid)}
END;";
            new DatabaseEngine().ExecuteNonQuery(query);
        }
        
        public static bool IsColumnAvailableForSqlIndexing(ReplicationColumnInfo column)
        {
            switch (column.DataType)
            {
                case SqlDataType.NText:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.VarBinaryMax:
                    return false;
                case SqlDataType.NChar:
                case SqlDataType.NVarChar:
                case SqlDataType.Char:
                case SqlDataType.VarChar:
                    return column.MaxLength < 500;
                default:
                    return true;
            }
        }

        private List<ReplicationSchemaIndex> GetNonClusteredIndexes(IEnumerable<TableInfo> tables, int surveySid, string tableName, List<string> indexableColumnNames)
        {
            var quotaBalancingConfiguration = _quotaBalancingService.GetQuotaBalancingConfiguration(surveySid);

            var replicationColumns = tables.SelectMany(x => x.ReplicationColumns);
            var quotaIds = replicationColumns.SelectMany(x => x.QuotaIds ?? new int[0]).Distinct();

            var indexes = new List<ReplicationSchemaIndex>();

            foreach (int quotaId in quotaIds)
            {
                IEnumerable<string> indexedColumnNames = replicationColumns
                    .Where(x => x.QuotaIds != null && x.QuotaIds.Contains(quotaId))
                    .Select(x => x.Name)
                    .Distinct();

                var quotaBalancing = quotaBalancingConfiguration.Quotas.SingleOrDefault(x => x.IsEnabled && x.QuotaId == quotaId);

                if (quotaBalancing != null)
                {
                    var quotaFilterQuestionNames = quotaBalancingConfiguration.Fields.Where(
                            field => field.IsEnabled && quotaBalancing.QuotaFieldIds.Contains(field.FieldId))
                        .Select(x => x.FieldName);

                    indexedColumnNames = quotaFilterQuestionNames.Union(indexedColumnNames);
                }

                indexes.Add(new ReplicationSchemaIndex
                {
                    TableName = tableName,
                    Name = _replicationIndexService.GetQuotaIndexName(quotaId),
                    IndexedColumnNames = indexedColumnNames.ToArray(),
                    IncludedColumnNames = new[] { respIdColumnInfo.Name }
                });
            }

            foreach (var column in indexableColumnNames)
            {
                if (indexes.Any(x => x.IndexedColumnNames.FirstOrDefault() == column))
                    continue;

                indexes.Add(new ReplicationSchemaIndex
                {
                    TableName = tableName,
                    Name = _replicationIndexService.GetColumnIndexName(column),
                    IndexedColumnNames = new[] { column, respIdColumnInfo.Name }
                });
            }

            return indexes;
        }
        
        /// <summary>
        /// Gets the SQL DataType of the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>SQL DataType of the column</returns>
        private static DataType GetColumnType(ColumnInfo column)
        {
            DataType type;
            switch (column.DataType)
            {
                case SqlDataType.Binary:
                case SqlDataType.Char:
                case SqlDataType.NChar:
                case SqlDataType.NVarChar:
                case SqlDataType.VarChar:
                case SqlDataType.VarBinary:
                    type = new DataType(column.DataType, column.MaxLength);
                    break;
                case SqlDataType.Numeric:
                case SqlDataType.Decimal:
                    type = new DataType(column.DataType, column.NumericPrecision, column.NumericScale);
                    break;
                default:
                    type = new DataType(column.DataType);
                    break;
            }

            return type;
        }

        public static string GetSelectForReplicatedDataTable(int batchId, int respId, BvReplicationTablesEntity[] orderedTables)
        {
            var select = new StringBuilder();
            var from = new StringBuilder();
            string prevTableName = null;

            select.AppendFormat("[{0}].[{1}]", orderedTables[0].TableName, respIdColumnInfo.Name);
            from.AppendFormat("<Schema>.[{0}]", orderedTables[0].TableName);

            //we should 
            foreach (var table in orderedTables)
            {
                if (prevTableName != null)
                {
                    from.Append($" LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY {respIdColumnInfo.Name} ORDER BY {responseIdColumnInfo.Name}) AS [{table.TableName}_row_number] FROM <Schema>.[{table.TableName}]) [{table.TableName}] ON [{table.TableName}].[{respIdColumnInfo.Name}] = [{prevTableName}].[{respIdColumnInfo.Name}] AND [{table.TableName}_row_number]=1");
                }

                var columns = ReplicationColumnsRepository.GetByTableId(table.ID);
                foreach (var column in columns)
                {
                    select.AppendFormat(", [{0}].[{1}] as [{1}]", table.TableName, column.ColumnName);
                }

                prevTableName = table.TableName;
            }

            var where = "WHERE 1=1";
            if (batchId != 0)
            {
                where += $" AND {orderedTables[0].TableName}.BatchID = {batchId}";
            }

            if (respId != 0)
            {
                where += $" AND {orderedTables[0].TableName}.RespId = {respId}";
            }

            return $"SELECT {select} FROM {from} {where}";
        }

        public bool IsReplicationSchemaChanged(int surveySid, TableInfo[] newTablesInfo)
        {
            var oldTablesInfo = GetOldTablesInfo(surveySid);

            SortTablesInfo(newTablesInfo);
            SortTablesInfo(oldTablesInfo);

            return !oldTablesInfo.SequenceEqual(newTablesInfo);
        }

        private TableInfo[] GetOldTablesInfo(int surveySid)
        {
            var oldTables = new List<TableInfo>();
            foreach (var table in ReplicationTablesRepository.GetBySurveyId(surveySid))
            {
                oldTables.Add(GetTableInfo(table));
            }

            return oldTables.ToArray();
        }

        private void SortTablesInfo(TableInfo[] tables)
        {
            tables = tables.OrderBy(x => x.Name).ToArray();
            foreach (var table in tables)
            {
                table.ReplicationColumns = table.ReplicationColumns.OrderBy(x => x.Name).ToArray();
            }
        }

        private TableInfo GetTableInfo(BvReplicationTablesEntity entity)
        {
            var table = new TableInfo();
            table.Name = entity.TableName;

            var columns = ReplicationColumnsRepository.GetByTableId(entity.ID);
            var columnsInfo = new List<ReplicationColumnInfo>();
            foreach (var column in columns)
            {
                columnsInfo.Add(GetColumnInfo(column));
            }
            table.ReplicationColumns = columnsInfo.ToArray();

            return table;
        }

        private ReplicationColumnInfo GetColumnInfo(BvReplicationColumnsEntity entity)
        {
            var column = new ReplicationColumnInfo();
            column.DataType = (SqlDataType)entity.ColumnType;
            column.Id = entity.ColumnID;
            column.Name = entity.ColumnName;
            column.MaxLength = entity.ColumnMaxLength == null ? -1 : entity.ColumnMaxLength.Value;

            return column;
        }
    }
}