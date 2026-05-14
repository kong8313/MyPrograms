using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.SystemTestFramework
{
    public class DataProvider
    {
        public string GetDataFromDb(string pid, string query)
        {
            var table = GetTableFromDb(pid, query);
            return FormatDataTable(table);
        }

        public DataTable GetTableFromDb(string pid, string query)
        {
            var sid = SurveyRepository.GetByName(pid).SID;

            var surveyConnectInfo = ServiceLocator.Resolve<ISurveyConnectionStringProvider>().GetConnectionInfo(sid);

            return new DatabaseEngine(surveyConnectInfo.ConnectionString).ExecuteDataTable<DataTable>(query, CommandType.Text);
        }

        private string FormatDataTable(DataTable table)
        {
            if (table.Rows.Count == 0)
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            result.AppendLine();

            var orderColumns = table.Columns.Cast<DataColumn>().OrderBy(x => x.Ordinal).Select(x => x.ColumnName).ToArray();
            var column2Len = new Dictionary<string, int>();

            foreach (var columnName in orderColumns)
            {
                var len = table.Rows.Cast<DataRow>().Max(x => GetFormatValue(x[columnName]).Length);
                len = Math.Max(len, columnName.Length) + 1;
                column2Len[columnName] = len;
                result.Append(columnName.PadRight(len));
            }

            foreach (DataRow row in table.Rows)
            {
                result.AppendLine();
                foreach (var columnName in orderColumns)
                {
                    var value = GetFormatValue(row[columnName]);
                    result.Append(value.PadRight(column2Len[columnName]));
                }
            }

            return result.ToString();
        }

        private string GetFormatValue(object value)
        {
            if (value is DBNull)
                return "NULL";
            if (value is DateTime)
            {
                var result = ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss.ffff", CultureInfo.InvariantCulture);

                return result;
            }

            return value.ToString();
        }
    }
}
