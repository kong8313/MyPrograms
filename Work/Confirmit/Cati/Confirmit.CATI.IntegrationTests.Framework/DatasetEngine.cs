using System;
using System.Data;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Confirmit.CATI.IntegrationTests.Framework
{
    public class DatasetEngine
    {
        public static T ReadDataTableFromXml<T>(
            string schemePath,
            string dataPath,
            string tableName) where T : DataTable, new()
        {
            var table = new T { TableName = tableName };

            table.ReadXmlSchema(
                Path.Combine(
                    IntegrationTestingFramework.Instance.Cfg.TestDataPath,
                    schemePath));

            table.ReadXml(
                Path.Combine(
                    IntegrationTestingFramework.Instance.Cfg.TestDataPath,
                    dataPath));

            return table;
        }

        public static void AreEqual(
            DataTable expectedTable,
            DataTable actualTable)
        {
            AreEqual(expectedTable, actualTable, null);
        }

        private static IEnumerable<DataRow> RowsCollectionToIEnuerable(DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>();
        }

        /// <summary>
        /// Column order doesn't matter
        /// But row order is important
        /// </summary>
        /// <param name="expectedTable">except table</param>
        /// <param name="actualTable">actual table</param>
        /// <param name="orderBy">Order information</param>
        public static void AreEqual(
            DataTable expectedTable,
            DataTable actualTable,
            string orderBy)
        {
            //
            // TODO: Write more intelligent algorithm.
            //

            var expectedWriter = new StringWriter();
            expectedTable.WriteXml(expectedWriter);

            var actualWriter = new StringWriter();
            actualTable.WriteXml(actualWriter);

            Assert.AreEqual(expectedTable.Columns.Count, actualTable.Columns.Count, 
                            String.Format("Count of columns in tables is different: ExpectedTable:\n{0}\nActualTable:\n{1}", expectedWriter, actualWriter));
            Assert.AreEqual(expectedTable.Rows.Count, actualTable.Rows.Count,
                            String.Format("Count of rows in tables is different: ExpectedTable:\n{0}\nActualTable:\n{1}", expectedWriter, actualWriter));

            var expectedRows = (orderBy == null ? RowsCollectionToIEnuerable(expectedTable) : RowsCollectionToIEnuerable(expectedTable).OrderBy(x => x[orderBy].ToString())).ToArray();
            var actualRows = (orderBy == null ? RowsCollectionToIEnuerable(actualTable) : RowsCollectionToIEnuerable(actualTable).OrderBy(x => x[orderBy].ToString())).ToArray();


            for (int indexRow = 0; indexRow < expectedRows.Length; indexRow++)
            {
                DataRow expectedRow = expectedRows[indexRow];
                DataRow actualRow = actualRows[indexRow];

                for (int indexCol = 0; indexCol < expectedTable.Columns.Count; indexCol++)
                {
                    var message = new StringBuilder();
                    string columnName = expectedTable.Columns[indexCol].ColumnName;

                    Assert.AreNotEqual(null, actualTable.Columns[columnName], 
                        String.Format("Column with name {0} exists only in one table. ExpectedTable:\n{1}\nActualTable:\n{2}", columnName, expectedWriter, actualWriter));

                    message.AppendLine("The types of data in tables is different.");
                    message.AppendLine(
                        String.Format(
                            "Column '{0}', row index {1}, expected {2} but actualy {3}.",
                            columnName,
                            indexRow,
                            expectedRow[columnName].GetType(),
                            actualRow[columnName].GetType()
                        )
                    );
                    message.AppendLine("Expected table:");
                    message.AppendLine(expectedWriter.ToString());
                    message.AppendLine("Actual table:");
                    message.AppendLine(actualWriter.ToString());
                    Assert.AreEqual(
                        expectedRow[columnName].GetType(), 
                        actualRow[columnName].GetType(),
                        message.ToString()
                    );

                    message = new StringBuilder();
                    message.AppendLine("The values in tables are different.");
                    message.AppendLine(
                        String.Format(
                            "Column '{0}', row index {1}, expected {2} but actualy {3}.",
                            columnName,
                            indexRow,
                            expectedRow[columnName],
                            actualRow[columnName]
                        )
                    );
                    message.AppendLine("Expected table:");
                    message.AppendLine(expectedWriter.ToString());
                    message.AppendLine("Actual table:");
                    message.AppendLine(actualWriter.ToString());
                    Assert.AreEqual(
                        expectedRow[columnName].ToString(), 
                        actualRow[columnName].ToString(),
                        message.ToString()
                    );
                }
            }
        }
    }
}