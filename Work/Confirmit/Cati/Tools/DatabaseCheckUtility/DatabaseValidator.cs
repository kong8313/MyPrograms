using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCheckUtility
{
    class DatabaseValidator
    {
        private readonly DatabaseInfoProvider _databaseInfoProvider;
        private readonly string[] _validateScripts;

        public DatabaseValidator(DatabaseInfoProvider databaseInfoProvider, string[] validateScripts)
        {
            _databaseInfoProvider = databaseInfoProvider;
            _validateScripts = validateScripts;
        }

        public void CheckDatabase(string dbName, DatabaseErrorAgregator erorrAgregator)
        {
            try
            {
                var connectionString = _databaseInfoProvider.GetDatabaseConnectionString(dbName);

                using (var connection = new SqlConnection(connectionString))
                {
                    var result = new List<string>();
                    connection.InfoMessage += (sender, args) => OnInfoMessage(dbName, erorrAgregator, args);
                    connection.FireInfoMessageEventOnUserErrors = true;
                    connection.Open();

                    foreach (var script in _validateScripts)
                    {
                        using (var command = new SqlCommand(script, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                do
                                {
                                    if (reader.FieldCount <= 0)
                                    {
                                        continue;
                                    }

                                    var table = new DataTable();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        table.Columns.Add(new DataColumn()
                                                          {
                                                              ColumnName = reader.GetName(i)
                                                          });
                                    }

                                    var rowData = new object[table.Columns.Count];

                                    while (reader.Read())
                                    {
                                        reader.GetValues(rowData);
                                        table.Rows.Add(rowData.ToArray());
                                    }

                                    OnDataTable(table);
                                } while (reader.NextResult());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Database state isn't valid.");
            }
        }

        private void OnDataTable(DataTable table)
        {
            var message = DataTableToTextConverter.FormatDataTable(table);
            Trace.TraceInformation(message);
        }

        private static void OnInfoMessage(string dbName, DatabaseErrorAgregator erorrAgregator, SqlInfoMessageEventArgs sqlInfoMessageEventArgs)
        {
            foreach (SqlError error in sqlInfoMessageEventArgs.Errors)
            {
                var message = String.Format("S:{0} N:{1} M:{2}", error.Class, error.Number, error.Message);
                if (error.Class < 11)
                {
                    Trace.TraceInformation(message);
                }
                else
                {
                    Trace.TraceError(message);
                    erorrAgregator.OnError(dbName, message);
                }
            }
        }
    }
}