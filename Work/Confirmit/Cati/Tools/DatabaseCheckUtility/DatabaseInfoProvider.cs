using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;

namespace DatabaseCheckUtility
{
    public class DatabaseInfoProvider
    {
        private readonly IConnectionStrings _connectionStrings;

        public DatabaseInfoProvider(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public string[] GetDatabaseNames()
        {
            var query = "SELECT Name FROM sys.databases where name like 'ConfirmitCATIV15%'";

            var dbEngine = new DatabaseEngine(_connectionStrings.DefaultInstanceConnectionString);
            
            return dbEngine.ExecuteScalarList<string>(query, CommandType.Text).ToArray();
        }

        public string GetDatabaseConnectionString(string dbName)
        {
            var scb = new SqlConnectionStringBuilder(_connectionStrings.DefaultInstanceConnectionString)
            {
                InitialCatalog = dbName
            };
            return scb.ToString();
        }
    }
}