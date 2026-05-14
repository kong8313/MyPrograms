using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class RemoteConnectionProvider : ITransactedConnectionProvider
    {
        public RemoteConnectionProvider(string remoteConnectionString)
        {
            Connection = new SqlConnection(remoteConnectionString);
            Connection.Open();
        }

        public SqlTransaction BeginTransaction(string transactionName)
        {
            if (Transaction != null) throw new Exception("Nested transactions aren't supported");

            Transaction = Connection.BeginTransaction(transactionName);

            return Transaction;
        }

        public SqlConnection Connection { get; private set; }
        public SqlTransaction Transaction { get; private set; }

        public void Dispose()
        {
            var connection = Connection;
            Connection = null;

            if (connection != null)
            {
                try
                {
                    connection.Dispose();
                }
                catch (Exception e)
                {
                    Trace.TraceError("Unexpected error occurred inside Dispose method of RemoteConnectionProvider class while calling SqlConnection.Dispose\r\nException:\r\n{0}", e);
                }
            }
        }
    }
}
