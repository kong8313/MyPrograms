using System.Collections.Generic;
using System.Data;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Services
{
    public class DatabaseObjectService  : IDatabaseObjectService
    {
        private readonly string _connectionString;

        public DatabaseObjectService(IConnectionStrings connectionStrings, ICompanyInfo companyInfo)
        {
            _connectionString = connectionStrings.GetConnectionStringForSpecificCompany(companyInfo.CompanyId);
        }

        public void CopyColumnsAndConstraints(string sourceTableName, string destinationTableName)
        {
            var dbEngine = new DatabaseEngine(_connectionString);

            string query = $"SELECT TOP (0) * INTO [{destinationTableName}] FROM [{sourceTableName}]";
            dbEngine.ExecuteNonQuery(query);

            query = $@"
SELECT
    REPLACE(dc.name, '{sourceTableName}', '{destinationTableName}') as ConstraintName, [definition] as CheckClause, c.name as ColumnName
FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE OBJECT_NAME(parent_object_id) = '{sourceTableName}' ";

            var table = dbEngine.ExecuteDataTable<DataTable>(query, CommandType.Text);
            foreach (DataRow row in table.Rows)
            {
                query = $"ALTER TABLE [{destinationTableName}] ADD CONSTRAINT [{row["ConstraintName"]}] DEFAULT {row["CheckClause"]} FOR {row["ColumnName"]}";
                dbEngine.ExecuteNonQuery(query);
            }
        }

        public void CopyTriggers(string sourceTableName, string destinationTableName)
        {
            var dbEngine = new DatabaseEngine(_connectionString);

            var query = $@"
SELECT Defininion = REPLACE(object_definition( t.object_id ), '{sourceTableName}', '{destinationTableName}')
FROM sys.triggers t
WHERE OBJECT_NAME(t.parent_id) =  '{sourceTableName}'";

            var triggerQueries = dbEngine.ExecuteScalarList<string>(query, CommandType.Text);
            foreach (string triggerQuesy in triggerQueries)
            {
                dbEngine.ExecuteNonQuery(triggerQuesy);
            }
        }

        public List<string> GetCreateIndexQueries(string sourceTableName, string destinationTableName)
        {
            var dbEngine = new DatabaseEngine(_connectionString);

            var result = new List<string>();

            var query = $@"
SELECT index_id as IndexId, name as IndexName, is_unique as IsUnique, is_unique_constraint as IsUniqueConstraint, filter_definition as FilterDefinition, type 
FROM sys.indexes 
WHERE object_id = object_id('[dbo].[{sourceTableName}]')";

            var table = dbEngine.ExecuteDataTable<DataTable>(query, CommandType.Text);
            foreach (DataRow row in table.Rows)
            {
                var unique = row["IsUnique"].ToString() == "1" ? "UNIQUE" : string.Empty;

                var type = row["type"].ToString() == "1" ? "CLUSTERED" : "NONCLUSTERED";

                var indexName = row["IndexName"].ToString();

                query = $@"
DECLARE @KeyColumns nvarchar(max)
SET @KeyColumns = ''

SELECT @KeyColumns = @KeyColumns + '[' + c.name + '] ' + CASE WHEN is_descending_key = 1 THEN 'DESC' ELSE 'ASC' END + ',' 
FROM sys.index_columns ic
INNER JOIN sys.columns c ON c.object_id = ic.object_id and c.column_id = ic.column_id
WHERE index_id = {row["IndexId"]} and ic.object_id = object_id('[dbo].[{sourceTableName}]') and key_ordinal > 0
ORDER BY index_column_id

SELECT @KeyColumns
";
                var keyColumns = dbEngine.ExecuteScalar<string>(query);
                if (keyColumns.Length > 0)
                {
                    keyColumns = $"({keyColumns.TrimEnd(',')})";
                }

                query = $@"
DECLARE @IncludedColumns nvarchar(max)
SET @IncludedColumns = ''

select @IncludedColumns = @IncludedColumns + '[' + c.name + '],'
FROM sys.index_columns ic
INNER JOIN sys.columns c ON c.object_id = ic.object_id and c.column_id = ic.column_id
WHERE index_id = {row["IndexId"]} and ic.object_id = object_id('[dbo].[{sourceTableName}]') and key_ordinal = 0
ORDER BY index_column_id

SELECT @IncludedColumns
";
                var includedColumns = dbEngine.ExecuteScalar<string>(query);
                if (includedColumns.Length > 0)
                {
                    includedColumns = $"INCLUDE ({includedColumns.TrimEnd(',')})";
                }

                var filterDefinition = !string.IsNullOrEmpty(row["FilterDefinition"].ToString()) ? $"WHERE {row["FilterDefinition"]}" : string.Empty;

                query = $"CREATE {unique} {type} INDEX [{indexName}] ON [dbo].[{destinationTableName}] {keyColumns} {includedColumns} {filterDefinition}";
                result.Add(query);
            }


            return result;
        }
    }
}
