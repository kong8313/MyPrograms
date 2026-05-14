using System.Data;
using System.Data.SqlClient;

namespace BuildServerRegistrator
{
    public class ConfirmQueryExecutor
    {
        private readonly string _connectionString;

        public ConfirmQueryExecutor(ConfigParameters configParameters)
        {
            var stringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = configParameters.SqlServerName,
                InitialCatalog = "confirm",
                UserID = configParameters.SqlLoginName,
                Password = configParameters.SqlPassword
            };

            _connectionString = stringBuilder.ToString();
        }

        public T ExecuteScalar<T>(string cmdText, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                command.CommandType = CommandType.Text;

                command.Parameters.AddRange(parameters);
                return (T)command.ExecuteScalar();
            }
        }

        public void ExecuteNonQuery(string cmdText, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                command.CommandType = CommandType.Text;

                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }

        public T ExecuteDataTable<T>(string cmdText, params SqlParameter[] parameters) where T : DataTable, new()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                command.Parameters.AddRange(parameters);

                using (var reader = command.ExecuteReader())
                {
                    var dataTable = new T();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }
    }
}
