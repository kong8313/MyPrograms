using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces
{
    public interface IDatabaseEngine
    {
        string DatabaseName { get; }

        string ConnectionString { get; }

        void ExecuteNonQuery(
            string cmdText, 
            CommandType cmdType, 
            params SqlParameter[] parameters);
        
        T ExecuteScalar<T>(
            string cmdText, 
            CommandType cmdType, 
            params SqlParameter[] parameters);

        T ExecuteScalar<T>(
            SqlCommand command, 
            params SqlParameter[] parameters);

        List<T> ExecuteScalarList<T>(
            SqlCommand command, 
            params SqlParameter[] parameters);

        List<T> ExecuteScalarList<T>(
            string cmdText, 
            CommandType cmdType, 
            params SqlParameter[] parameters);

        List<T> ExecuteScalarListWithSpecificTimeOut<T>(
            string cmdText,
            CommandType cmdType,
            int connectionTimeout,
            params SqlParameter[] parameters);

        T ExecuteDataTableInNewConnection<T>(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters)
            where T : DataTable, new();

        T ExecuteDataTable<T>(
            string cmdText, 
            CommandType cmdType, 
            params SqlParameter[] parameters)
            where T : DataTable, new();

        T ExecuteDataTableWithReturn<T>(
            string procedureName,
            out int result,
            params SqlParameter[] parameters) 
            where T : DataTable, new();

        T ExecuteDataTable<T>(
            SqlCommand command,
            params SqlParameter[] parameters)
            where T : DataTable, new();

        T ExecuteDataTable<T>(SqlCommand command)
            where T : DataTable, new();

        IDataReader ExecuteReaderInNewConnection(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters);

        void ExecuteBatch(
            string batchText, bool useInfinityExecutionTimeout = false);
    }
}
