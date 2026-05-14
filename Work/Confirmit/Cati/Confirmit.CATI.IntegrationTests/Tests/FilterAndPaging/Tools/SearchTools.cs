using System;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.IntegrationTests.Framework;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools
{
    internal static class SearchTools
    {

        private const string m_TestTableName = "SearchTestSample";


        /// <summary>
        /// Creates sample table used for search functionality tests.
        /// </summary>
        public static void CreateSampleTable(string samplePath)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            var columns = new KeyValuePair<string, DataType>[]
            {
                new KeyValuePair<string, DataType>("ColumnInt", DataType.Int),
                new KeyValuePair<string, DataType>("ColumnText", DataType.NVarChar(4000)),
                new KeyValuePair<string, DataType>("ColumnDate", DataType.DateTime),
                new KeyValuePair<string, DataType>("ColumnDecimal", DataType.Float)
            };
            framework.DbEngine.CreateTable(m_TestTableName, columns);

            using (SqlConnection connection = new SqlConnection(framework.DbEngine.ConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.InsertCommand = new SqlCommand(
                    "insert into " + m_TestTableName+ " (ColumnInt, ColumnText, ColumnDate, ColumnDecimal) values (@ColumnInt, @ColumnText, @ColumnDate, @ColumnDecimal)",
                    connection
                );
                adapter.InsertCommand.Parameters.Add("@ColumnInt", SqlDbType.Int, 4, "ColumnInt");
                adapter.InsertCommand.Parameters.Add("@ColumnText", SqlDbType.NVarChar, 4000, "ColumnText");
                adapter.InsertCommand.Parameters.Add("@ColumnDate", SqlDbType.DateTime, 8, "ColumnDate");
                adapter.InsertCommand.Parameters.Add("@ColumnDecimal", SqlDbType.Float, 10, "ColumnDecimal");

                DataTable table = new DataTable();
                table.ReadXml(Path.Combine(framework.Cfg.TestDataPath, samplePath));

                adapter.Update(table);
            }
        }

        /// <summary>
        /// Inserts into given column of sample table given values.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="values">Values to insert.</param>
        public static void InsertIntoSampleTable(string columnName, SqlDbType type, int size, object[] values)
        {
            using (SqlConnection connection = new SqlConnection(IntegrationTestingFramework.Instance.DbEngine.ConnectionString))
            {
                connection.Open();

                string query = "insert into " + m_TestTableName + " (" + columnName + ") values (@Value)";
                SqlCommand command = new SqlCommand(query, connection);
                SqlParameter param = new SqlParameter("@Value", type, size, columnName);
                command.Parameters.Add(param);

                foreach (object val in values)
                {
                    command.Parameters[0].Value = val;
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Deletes sample table used for search functionality tests.
        /// </summary>
        public static void DeleteSampleTable()
        {
            IntegrationTestingFramework.Instance.DbEngine.DropTable(m_TestTableName);
        }

        public static PagingArgs SearchBy(string columnName, SearchColumnType type, SearchOperator op, string value)
        {
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = columnName,
                    ColumnType = type,
                    Operator = op,
                    Value = value
                }
            };

            return (new PagingArgs(1, 100, "InterviewID", true, search));
        }

        public static PagingArgs SearchByDateColumn(string columnName, SearchOperator op, DateTime value)
        {
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = columnName,
                    ColumnType = SearchColumnType.DateTime,
                    Operator = op,
                    Value = value
                }
            };

            return (new PagingArgs(1, 100, "InterviewID", true, search));
        }

    }
}
