using System;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public class DatabaseNotAvailableException : Exception
    {
        public DatabaseNotAvailableException(string database, SqlException e)
            : base(string.Format("Database {0} is not available", database), e)
        {

        }
    }
}