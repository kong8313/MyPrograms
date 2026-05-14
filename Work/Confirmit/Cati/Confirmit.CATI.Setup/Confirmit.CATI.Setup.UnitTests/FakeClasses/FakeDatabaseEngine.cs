using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    public class FakeDatabaseEngine : IDatabaseEngine
    {

        public T ExecuteDataTable<T>(string databaseName, string query) where T : System.Data.DataTable, new()
        {
            throw new System.NotImplementedException();
        }

        public void ValidateConnection(string serverName)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteNonQuery(string databaseName, string commandText, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public T ExecuteScalar<T>(string databaseName, string commandText, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public T ExecuteScalar<T>(string commandText, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public string Login
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Password
        {
            get { throw new System.NotImplementedException(); }
        }

        public string ServerName
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}