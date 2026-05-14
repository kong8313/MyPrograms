using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace StartStopTest
{
    static class SQLTask
    {
        public static void RunSqlTask()
        {

            string connectionString = "server=CO-OSL-DEVB38;database=ConfirmitCATIV15;uid=sa;password=firm";
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = CreateCommand(60))  //sec
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

        }


        public static SqlCommand CreateCommand(int commandExecutionTimeout)
        {
            var command = new SqlCommand();

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "BvSpAlert_RecalculateAll";

            command.CommandTimeout = commandExecutionTimeout;

            var returnValue = new SqlParameter("@ReturnValue", 0);
            returnValue.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnValue);

            return command;
        }

    }
}
