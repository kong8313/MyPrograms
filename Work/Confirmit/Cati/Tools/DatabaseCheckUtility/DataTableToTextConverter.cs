using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCheckUtility
{
    public class DataTableToTextConverter
    {
        public static string FormatDataTable(DataTable table)
        {
            var sb = new StringBuilder();
            sb.AppendLine();

            var orderColumns = table.Columns.Cast<DataColumn>().OrderBy(x => x.Ordinal).Select(x => x.ColumnName).ToArray();
            var column2Len = new Dictionary<string, int>();

            foreach (var columnName in orderColumns)
            {
                var len = table.Rows.Cast<DataRow>().Max(x => GetFormatValue(x[columnName]).Length);
                len = Math.Max(len, columnName.Length) + 1;
                column2Len[columnName] = len;
                sb.Append(columnName.PadRight(len));
            }

            foreach (DataRow row in table.Rows)
            {
                sb.AppendLine();
                foreach (var columnName in orderColumns)
                {
                    var value = GetFormatValue(row[columnName]);
                    sb.Append(value.PadRight(column2Len[columnName]));
                }
            }

            return sb.ToString();
        }

        private static string GetFormatValue(object value)
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
