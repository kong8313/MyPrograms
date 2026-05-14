using System;
using System.Data;
using System.Data.SqlClient;

namespace CatiEncoder
{
    public class DatabaseEngine
    {
        public string SqlConnectionString { get; private set; }

        public DatabaseEngine(string sqlConnectionString)
        {
            SqlConnectionString = sqlConnectionString;
            ValidateConnection();
        }

        private void ValidateConnection()
        {
            try
            {
                using (var cn = new SqlConnection(SqlConnectionString))
                {
                    cn.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Connection '{0}' is wrong.", SqlConnectionString), ex);
            }
        }

        /// <summary>
        /// Run execute scalar function
        /// </summary>
        /// <typeparam name="T">Type of query result</typeparam>
        /// <param name="commandText">Command for execution</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string commandText)
        {
            using (var cn = new SqlConnection(SqlConnectionString))
            using (var cmd = new SqlCommand(commandText, cn))
            {
                cn.Open();

                cmd.CommandType = CommandType.Text;
                var result = cmd.ExecuteScalar();                
                return result.GetType().Name == "DBNull" ? default(T) : (T)result;
            }
        }

        /// <summary>
        /// Read some data as table from database
        /// </summary>
        /// <param name="commandText">Command for execution</param>
        /// <returns></returns>
        public void ExecuteNonQuery(string commandText)
        {
            using (var cn = new SqlConnection(SqlConnectionString))
            using (var cmd = new SqlCommand(commandText, cn))
            {
                cn.Open();

                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
