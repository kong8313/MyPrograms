using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;

using ConfirmitDialerInterface;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.Replication
{
    public static class ReplicationTools
    {
        internal static int AddSurvey()
        {
            return SurveyRepository.Insert(
                new BvSurveyEntity { Name = BackendTools.GenerateSurveyName(), Description = "", DialMode = (byte)DialingMode.Manual, });
        }

        internal static void CheckReplicationSchemeIsEmpty(int surveySid, string destinationTableName, TableInfo[] testData)
        {
            // Check BvReplicationTables
            var replicationTables = ReplicationTablesRepository.GetBySurveyId(surveySid);
            Assert.AreEqual(0, replicationTables.Count);

            // Check BvReplicationColumns
            var tableIds = replicationTables.Select(x => x.ID);
            var replicationColumns = BvReplicationColumnsAdapter.GetAll().Where(x => tableIds.Contains(x.TableID));
            Assert.AreEqual(0, replicationColumns.Count());

            // Check BvReplicatedData
            var serverConnection = ServerConnectionFactory.Create(BackendInstance.Current.ConnectionString);
            var server = new Server(serverConnection);
            var db = server.Databases[IntegrationTestingFramework.Instance.DbEngine.DatabaseName];
            var isTableExists = db.Tables.Contains(destinationTableName);
            Assert.IsFalse(isTableExists);

            // Check BvReplicationChanges
            var transferChangesTables = testData.Select(x => ReplicationSchemaService.GetTransferChangesTableName(x.Name, surveySid));
            Assert.IsTrue(transferChangesTables.All(x => !db.Tables.Contains(x)));
        }

        internal static void CheckDbByTestData(int surveySid, IEnumerable<TableInfo> testData)
        {
            testData = RemoveResponseNamesEqualToSystemRespondentFields(testData.ToArray());

            // Check BvSurvey
            var survey = SurveyRepository.GetById(surveySid);
            Assert.IsNotNull(survey);

            string destinationTableName = survey.DestinationTableName;
            Assert.IsFalse(String.IsNullOrEmpty(destinationTableName));

            // Check BvReplicationTables
            var replicationTables = ReplicationTablesRepository.GetBySurveyId(surveySid);
            Assert.AreEqual(testData.Count(), replicationTables.Count);

            var tableNames = replicationTables.Select(x => x.TableName);
            Assert.AreEqual(0, tableNames.Except(testData.Select(x => x.Name)).Count());

            var tablePrimaryKeys = replicationTables.Select(x => x.PrimaryKey);
            Assert.AreEqual(0, tablePrimaryKeys.Except(testData.SelectMany(x => x.PrimaryKeyColumns).Select(x => x.Name)).Count());

            // Check BvReplicationColumns
            var tableIds = replicationTables.Select(x => x.ID);

            var replicationColumns = BvReplicationColumnsAdapter.GetAll().Where(x => tableIds.Contains(x.TableID));
            Assert.AreEqual(testData.SelectMany(x => x.ReplicationColumns).Count(), replicationColumns.Count());

            var columnNames = replicationColumns.Select(x => x.ColumnName);
            Assert.AreEqual(0, columnNames.Except(testData.SelectMany(x => x.ReplicationColumns).Select(x => x.Name)).Count());

            var columnIds = replicationColumns.Select(x => x.ColumnID);
            Assert.AreEqual(0, columnIds.Except(testData.SelectMany(x => x.ReplicationColumns).Select(x => x.Id)).Count());

            // Check BvReplicatedData
            var serverConnection = ServerConnectionFactory.Create(BackendInstance.Current.ConnectionString);
            var server = new Server(serverConnection);
            var db = server.Databases[IntegrationTestingFramework.Instance.DbEngine.DatabaseName];
            var isTableExists = db.Tables.Contains(destinationTableName);
            Assert.IsTrue(isTableExists);

            var table = db.Tables[destinationTableName];
            foreach (var column in testData.SelectMany(x => x.ReplicationColumns))
            {
                Assert.IsNotNull(table.Columns[column.Name]);
                Assert.AreEqual(column.DataType, table.Columns[column.Name].DataType.SqlDataType);
            }

            //Check index on respid
            var columnName = "respid";
            Assert.IsTrue(
                table.Indexes.Cast<Index>().Count(x => x.IndexedColumns.Contains(columnName)) > 0,
                "No indexes for primary key {0}", columnName);

            var expectedQuotas = testData.SelectMany(x => x.ReplicationColumns).SelectMany(x => x.QuotaIds ?? new int[0])
                .Distinct()
                .OrderBy(x => x);

            foreach (var quota in expectedQuotas)
            {
                var quotaId = quota;
                var columns = testData.SelectMany(x => x.ReplicationColumns).Where(
                    x => x.QuotaIds != null && x.QuotaIds.Contains(quotaId)).Select(q => q.Name);

                Assert.AreEqual(
                    1,
                    table.Indexes.Cast<Index>().Where(
                        x =>
                        x.IndexedColumns
                            .Cast<IndexedColumn>()
                            .Where(c => !c.IsIncluded)
                            .Count() == columns.Count()
                        &&
                        x.IndexedColumns
                            .Cast<IndexedColumn>()
                            .Where(c => !c.IsIncluded)
                            .Select(c => c.Name)
                            .Except(columns)
                            .Count() == 0)
                        .Count(),
                    "Incorrect count of indexes for quota {0}", quotaId);
            }
        }

        internal static TableInfo[] GetTestData()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Numeric, Id = 100, Name = "Var1", QuotaIds = null, NumericPrecision = 2, NumericScale = 1 };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 101, Name = "Var2", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.VarChar, Id = 102, Name = "Var3", QuotaIds = null, MaxLength = 10 };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "fanta" };

            var t1 = new TableInfo { Name = "table1", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "table2", ReplicationColumns = new[] { c3 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        internal static TableInfo[] GetTestData2()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Numeric, Id = 1001, Name = "Var11", QuotaIds = null, NumericPrecision = 8, NumericScale = 2 };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 1011, Name = "Var21", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.VarChar, Id = 1021, Name = "Var31", QuotaIds = null, MaxLength = 10 };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 2001, Name = "Var41", QuotaIds = null };
            var c5 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 2011, Name = "Var51", QuotaIds = null };
            var c6 = new ReplicationColumnInfo { DataType = SqlDataType.VarChar, Id = 2021, Name = "Var61", QuotaIds = null, MaxLength = 10 };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "fanta" };
            var p3 = new ColumnInfo { DataType = SqlDataType.Int, Name = "sprite" };

            var t1 = new TableInfo { Name = "table11", ReplicationColumns = new[] { c1 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "table21", ReplicationColumns = new[] { c2, c3, c4 }, PrimaryKeyColumns = new[] { p2 } };
            var t3 = new TableInfo { Name = "table31", ReplicationColumns = new[] { c5, c6 }, PrimaryKeyColumns = new[] { p3 } };

            return new[] { t1, t2, t3 };
        }

        internal static TableInfo[] GetTestDataForSurveyInterview(int[] columnNumbers)
        {
            var allClms = new ReplicationColumnInfo[5];
            allClms[0] = new ReplicationColumnInfo { DataType = SqlDataType.NVarChar, Id = 1001, Name = "q1", QuotaIds = null, MaxLength = 10 };
            allClms[1] = new ReplicationColumnInfo { DataType = SqlDataType.NVarChar, Id = 1011, Name = "q2", QuotaIds = null, MaxLength = 10 };
            allClms[2] = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 1021, Name = "q3", QuotaIds = null, MaxLength = 10 };
            allClms[3] = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 1031, Name = "q4", QuotaIds = null, MaxLength = 10 };
            allClms[4] = new ReplicationColumnInfo { DataType = SqlDataType.NVarChar, Id = 1041, Name = "q5", QuotaIds = null, MaxLength = 10 };
            var callCnt = new ReplicationColumnInfo { DataType = SqlDataType.Int, Name = "CallAttemptCount", Id = 32, QuotaIds = new int[0] };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };

            var addClmn = new ReplicationColumnInfo[columnNumbers.Length];
            for (int i = 0; i < columnNumbers.Length; i++)
            {
                addClmn[i] = allClms[columnNumbers[i]];
            }
            var t1 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { callCnt }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "response_control", ReplicationColumns = addClmn, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        internal static TableInfo[] GetTestDataSomeResponseNamesEqualToSystemRespondentFields()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "key", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 33, Name = "TelephoneNumber", QuotaIds = null };
            var c5 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 34, Name = "RespondentName", QuotaIds = null };
            var c6 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 35, Name = "ExtensionNumber", QuotaIds = null };
            var c7 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 36, Name = "DialType", QuotaIds = null };
            var c8 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 37, Name = "TimeZoneId", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c1, c2, c3, c4, c5, c6, c7, c8 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3, c4, c5, c6, c7, c8 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        internal static TableInfo[] GetTestDataAllResponseNamesEqualToSystemRespondentFields()
        {
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 33, Name = "TelephoneNumber", QuotaIds = null };
            var c5 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 34, Name = "RespondentName", QuotaIds = null };
            var c6 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 35, Name = "ExtensionNumber", QuotaIds = null };
            var c7 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 36, Name = "DialType", QuotaIds = null };
            var c8 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 37, Name = "TimeZoneId", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c3, c4, c5, c6, c7, c8 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3, c4, c5, c6, c7, c8 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        internal static TableInfo[] RemoveResponseNamesEqualToSystemRespondentFields(TableInfo[] replicatedTables)
        {
            var result = new List<TableInfo>();

            var filteredColumns = replicatedTables.FirstOrDefault(x => x.Name.ToLower() == "respondent")?.ReplicationColumns.Select(x => x.Name) ??
                new string[] { };

            foreach (var tableInfo in replicatedTables)
            {
                if (tableInfo.Name.ToLower() == "respondent")
                {
                    result.Add(tableInfo);
                    continue;
                }

                var item = new TableInfo
                {
                    Name = tableInfo.Name,
                    PrimaryKeyColumns = tableInfo.PrimaryKeyColumns,
                    ReplicationColumns = tableInfo.ReplicationColumns.Where(columnInfo =>
                        !filteredColumns.Contains(columnInfo.Name, StringComparer.OrdinalIgnoreCase)).ToArray()
                };

                if (item.ReplicationColumns.Length > 0)
                {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }

        internal static DatabaseEngine GetConfirmitSurveyDb(out string projectId)
        {
            var framework = IntegrationTestingFramework.Instance;
            projectId = BackendTools.GenerateSurveyName();
            var cfSurveyDbName = "survey_" + projectId;
            var confirmitSurveyDb = new DatabaseEngine(framework.GetConfirmitSqlServerConnectionString(cfSurveyDbName));

            new DatabaseTools(framework.ConfirmitSqlServerMasterConnectionString).
                CreateEmptyDatabase(cfSurveyDbName);

            ConfirmitTools.CreateQuotaTables(confirmitSurveyDb);
            framework.RegisterDbToDeleteOnTestCleaup(confirmitSurveyDb.DatabaseName);
            return confirmitSurveyDb;
        }
    }
}
