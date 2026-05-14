using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.PowerShellApi
{
    public class ApiSql
    {
        private readonly Func<IConnectionProvider> _connectionProviderBuilder;
        private readonly string _connectionString;

        private const int ExecutionTimeout = 10000;

        public ApiSql(Func<IConnectionProvider> connectionProviderBuilder, string connectionString)
        {
            _connectionProviderBuilder = connectionProviderBuilder;
            _connectionString = connectionString;
        }

        public bool IsExist()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = "master";
            var masterConnectionString = connectionStringBuilder.ToString();

            using (var connection = new SqlConnection(masterConnectionString))
            using( var command = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = @DbName", connection))
            {
                command.Parameters.Add(new SqlParameter("@DbName", databaseName));
                connection.Open();
                return (int) command.ExecuteScalar() == 1;
            }
        }

        public void Attach()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = "master";
            var masterConnectionString = connectionStringBuilder.ToString();

            var query = @"
EXEC confirm_admin.DBO.usp_ActivateSurvey @DbName
declare @deadline DATETIME = DATEADD(MINUTE, 5, GETDATE())

IF NOT EXISTS( SELECT 1 FROM sys.databases WHERE Name = @DbName )
BEGIN 
    RAISERROR('Database %s haven''t been started to attach', 16, 1, @DbName )
END 
ELSE 
BEGIN
    WHILE NOT EXISTS( SELECT 1 FROM sys.databases WHERE Name = @DbName AND State = 0 )
    BEGIN 
	    IF(@deadline < GETDATE())
	    BEGIN
		    RAISERROR('Database %s haven''t been attached to online state', 16, 1, @DbName )
		    BREAK
	    END
	    WAITFOR DELAY '00:00:00.100'
    END
END";

            using (var connection = new SqlConnection(masterConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add(new SqlParameter("@DbName", databaseName));
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using(var connectionProvider = _connectionProviderBuilder())
            using (var sqlCommand = new SqlCommand(query, connectionProvider.Connection))
            {
                sqlCommand.Transaction = connectionProvider.Transaction;
                sqlCommand.CommandTimeout = 0;

                sqlCommand.ExecuteNonQuery();
            }
        }

        public DataRow ExecuteRow(string query)
        {
            var res = ExecuteRowList(query).Single();
            return res;
        }

        public object ExecuteScalar(string query)
        {
            return ExecuteRow(query)[0];
        }


        public IEnumerable<object> ExecuteScalarList(string query)
        {
            return ExecuteRowList(query).Select(x => x[0]);
        }

        public IEnumerable<DataRow> ExecuteRowList(string query)
        {
            DataTable table = new DataTable();

            using (var connectionProvider = _connectionProviderBuilder())
            using (var sqlCommand = new SqlCommand(query, connectionProvider.Connection))
            {
                sqlCommand.Transaction = connectionProvider.Transaction;
                sqlCommand.CommandTimeout = 0;

                using (var reader = sqlCommand.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table.Rows.Cast<DataRow>();
        }

        public int CopyDataToNewTable(ApiSql from, string toTableName, string query)
        {
            using (var toConnectionProvider = _connectionProviderBuilder())
            using (var fromConnectionProvider = from._connectionProviderBuilder())
            {
                return new RemoteDataCopier().CopyDataToNewTable(
                    fromConnectionProvider,
                    toConnectionProvider,
                    toTableName,
                    query, 
                    null,
                    ExecutionTimeout);
            }
        }

        public int CopyDataToExistTable(ApiSql from, string toTableName, string query)
        {
            using (var toConnectionProvider = _connectionProviderBuilder())
            using (var fromConnectionProvider = from._connectionProviderBuilder())
            {
                return new RemoteDataCopier().CopyDataToExistTable(
                    fromConnectionProvider,
                    toConnectionProvider,
                    toTableName,
                    query,
                    null,
                    ExecutionTimeout);
            }
        }
    }
}