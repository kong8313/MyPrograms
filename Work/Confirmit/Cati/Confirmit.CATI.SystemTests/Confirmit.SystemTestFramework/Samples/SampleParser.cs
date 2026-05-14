using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Confirmit.SystemTestFramework.Samples
{
    public class SampleParser
    {
        public DataSet Parser(string text)
        {
            var lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var header = Split(lines[0]);
            var columns = header.Select(s => new DataColumn(s, typeof (string))).ToList();

            var table = new DataTable("respondent");
            table.Columns.AddRange(columns.ToArray());

            var rows = lines.Skip(1);

            foreach (var row in rows)
            {
                var data = Split(row);
                var newRow = table.NewRow();
                var items = new List<object>();

                for (var i = 0; i < header.Length; i++)
                {
                    items.Add(data.Length < header.Length && i >= data.Length ? string.Empty : data[i]); ;
                }

                newRow.ItemArray = items.ToArray();

                table.Rows.Add(newRow);
            }

            var dataSet = new DataSet();

            dataSet.Tables.Add(table);

            return dataSet;
        }

        private string[] Split(string line)
        {
            return line.Split('\t');
        }
    }
}