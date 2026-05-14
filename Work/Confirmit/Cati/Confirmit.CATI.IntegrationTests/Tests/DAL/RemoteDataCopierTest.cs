using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DAL
{
    [TestClass]
    public class RemoteDataCopierTest
    {
        IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IRemoteDataCopier _remoteDataCopier;

        private DatabaseTools _databaseTools;
        private DatabaseEngine _remoteDatabaseEngine;
        private DatabaseEngine _localDatabaseEngine;

        private const string RemoteDatabaseName = "TestRemoteDatabase";
        private const string LocalDatabaseName = "TestLocalDatabase";
        private string RemoteTableName = "TestRemoteTable_" + Guid.NewGuid();
        private string LocalTableName = "TestLocalTable_" + Guid.NewGuid();

        private DataTable _remoteData;

        [TestInitialize]
        public void TestInitialize()
        {
            _remoteDataCopier = new RemoteDataCopier();

            _databaseTools = new DatabaseTools(_framework.GetCatiSqlServerConnectionString("master"));
            CreateTestDatabases();

            _remoteDatabaseEngine = new DatabaseEngine(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName));
            _localDatabaseEngine = new DatabaseEngine(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName));
            CreateTestTable(_localDatabaseEngine, LocalTableName, true);
            CreateTestTable(_remoteDatabaseEngine, RemoteTableName, true);

            _remoteData = _remoteDatabaseEngine.ExecuteDataTable<DataTable>($"SELECT * FROM [{RemoteTableName}]", CommandType.Text);
        }

        private void CreateTestDatabases()
        {
            _databaseTools.DropDatabase(RemoteDatabaseName);
            _databaseTools.CreateDatabase(RemoteDatabaseName, string.Empty);

            _databaseTools.DropDatabase(LocalDatabaseName);
            _databaseTools.CreateDatabase(LocalDatabaseName, string.Empty);
        }

        private void CreateTestTable(DatabaseEngine databaseEngine, string tableName, bool fillWithTestData = false)
        {
            string createTableQuery = $"CREATE TABLE[dbo].[{tableName}] (" + @"
                [Id] [int] NULL,
                [Col1] [bigint] NULL,
                [Col2] [nvarchar](max) NULL,
                [Col3] [nvarchar](255) NULL,
                [Col4] [bit] NULL,
	            [Col5] [tinyint] NULL,
	            [Col6] [datetime] NULL,
	            [Col7] [smallint] NULL,
	            [Col8] [real] NULL,	
	            [Col9] [varchar] (64) NULL,
	            [Col10] [uniqueidentifier] NULL,
	            [Col11] [varbinary](max) NULL)";
            databaseEngine.ExecuteNonQuery(createTableQuery);

            if (!fillWithTestData)
            {
                return;
            }

            string fillDataQuery = $"INSERT INTO[dbo].[{tableName}]" + @"
                VALUES
                    ('1','2','3', '<xml test=""1""/>', '0', '5', '11.12.2006 00:12:12', '1', '0.1', 'r', '{865BCC28-FB49-4432-94A7-BC51A966EBA2}', 0x0000000001000000),
                    ('6', '7', '8', '<xml test=""2""/>', '1', '9', '11.12.2007 00:12:12', '2', '0.2', 'l', '{865BCC28-FB49-4432-94A7-BC51A966EBA2}', 0x0000000002000000),
	                ('10', '11', '12', '<xml test=""3""/>', '0', '13', '11.12.2008 00:12:12', '3', '0.3', 'p', '{865BCC28-FB49-4432-94A7-BC51A966EBA2}', 0x0000000003000000)";
            databaseEngine.ExecuteNonQuery(fillDataQuery);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _databaseTools.DropDatabase(RemoteDatabaseName);
            _databaseTools.DropDatabase(LocalDatabaseName);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToExistLocalTable_CreateTableWithTestDataInBothDatabases_CopyDataFromOneTableToAnother_DataIsCopiedCorrectly()
        {
            var localTableName = LocalTableName + "_test";
            CreateTestTable(_localDatabaseEngine, localTableName);

            using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
            {
                _remoteDataCopier.CopyDataToExistTable(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName), connectionScope, localTableName, $"SELECT * FROM [{RemoteTableName}]");
            }

            DataTable localData = _localDatabaseEngine.ExecuteDataTable<DataTable>($"SELECT * FROM [{localTableName}]", CommandType.Text);

            CompareTwoTables(localData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToExistLocalTable_CreateTableWithTestDataInBothDatabases_CopyDataFromOneTableToAnother_UseMethodWithTwoSqlConnectionParams_DataIsCopiedCorrectly()
        {
            var localTableName = LocalTableName + "_test";
            CreateTestTable(_localDatabaseEngine, localTableName);

            using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
            {
                using (var remoteConnectionProvider = new RemoteConnectionProvider(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName)))
                {
                    _remoteDataCopier.CopyDataToExistTable(remoteConnectionProvider, connectionScope, localTableName, $"SELECT * FROM [{RemoteTableName}]");
                }
            }

            DataTable localData = _localDatabaseEngine.ExecuteDataTable<DataTable>($"SELECT * FROM [{localTableName}]", CommandType.Text);

            CompareTwoTables(localData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToNewLocalTable_CreateTableWithTestDataInOneDatabase_CopyToAnotherDatabase_TableIsCreatedAndDataIsCopiedCorrectly()
        {
            string localTableName = "#TestTempTableName";
            DataTable localData;
            using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
            {
                _remoteDataCopier.CopyDataToNewTable(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName), connectionScope, localTableName, $"SELECT * FROM [{RemoteTableName}]");

                localData = _localDatabaseEngine.ExecuteDataTable<DataTable>($"SELECT * FROM [{localTableName}]", CommandType.Text);
            }

            CompareTwoTables(localData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToNewLocalTable_DoCopyingInsideDatabaseTransaction_TableIsCreatedAndDataIsCopiedCorrectly()
        {
            string localTableName = "#TestTempTableName";
            DataTable localData;
            BackendInstance.Current.ConnectionString = _framework.GetCatiSqlServerConnectionString(LocalDatabaseName);

            using (var transactionScope = new DatabaseTransactionScope("TestTransaction"))
            using (var connectionScope = new ConnectionScope(BackendInstance.Current.ConnectionString))
            {
                _remoteDataCopier.CopyDataToNewTable(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName), connectionScope, localTableName, $"SELECT * FROM [{RemoteTableName}]");

                localData = _localDatabaseEngine.ExecuteDataTable<DataTable>($"SELECT * FROM [{localTableName}]", CommandType.Text);

                transactionScope.Commit();
            }

            CompareTwoTables(localData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToNewRemoteTable_CreateTableWithTestDataInOneDatabase_CopyToAnotherDatabase_TableIsCreatedAndDataIsCopiedCorrectly()
        {
            string remoteTableName = "#TestTempTableName";
            DataTable remoteData;

            string remoteConnectionString = _framework.GetCatiSqlServerConnectionString(RemoteDatabaseName);
            using (var remoteConnectionProvider = new RemoteConnectionProvider(remoteConnectionString))
            {
                using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
                {
                    _remoteDataCopier.CopyDataToNewTable(connectionScope.ConnectionString, remoteConnectionProvider, remoteTableName, $"SELECT * FROM [{LocalTableName}]");
                }

                using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [{remoteTableName}]", remoteConnectionProvider.Connection))
                {
                    remoteData = _remoteDatabaseEngine.ExecuteDataTable<DataTable>(sqlCommand);
                }
            }

            CompareTwoTables(remoteData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToNewRemoteTable_CreateTableWithTestDataInOneDatabase_CopyToAnotherDatabase_UseMethodWithTwoSqlConnectionParameters_TableIsCreatedAndDataIsCopiedCorrectly()
        {
            string remoteTableName = "#TestTempTableName";
            DataTable remoteData;

            string remoteConnectionString = _framework.GetCatiSqlServerConnectionString(RemoteDatabaseName);
            using (var remoteConnectionProvider = new RemoteConnectionProvider(remoteConnectionString))
            {
                using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
                {
                    _remoteDataCopier.CopyDataToNewTable(connectionScope, remoteConnectionProvider, remoteTableName, $"SELECT * FROM [{LocalTableName}]");
                }

                using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [{remoteTableName}]", remoteConnectionProvider.Connection))
                {
                    remoteData = _remoteDatabaseEngine.ExecuteDataTable<DataTable>(sqlCommand);
                }
            }

            CompareTwoTables(remoteData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToNewRemoteTable_CreateTempTableWithTestDataInOneDatabase_CopyToAnotherDatabase_TableIsCreatedAndDataIsCopiedCorrectly()
        {
            string remoteTableName = "#TestTempRemoteTableName";
            string localTableName = "#TestTempTableName";
            DataTable remoteData;

            using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
            {
                CreateTestTable(_localDatabaseEngine, localTableName, true);
                _remoteData = _localDatabaseEngine.ExecuteDataTable<DataTable>($"SELECT * FROM [{localTableName}]", CommandType.Text);

                using (var remoteConnectionProvider = new RemoteConnectionProvider(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName)))
                {
                    _remoteDataCopier.CopyDataToNewTable(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName), remoteConnectionProvider, remoteTableName, $"SELECT * FROM [{localTableName}]");

                    using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [{remoteTableName}]", remoteConnectionProvider.Connection))
                    {
                        remoteData = _remoteDatabaseEngine.ExecuteDataTable<DataTable>(sqlCommand);
                    }
                }
            }

            CompareTwoTables(remoteData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void CopyDataToNewRemoteTable_DoCopyingInsideDatabaseTransaction_TableIsCreatedAndDataIsCopiedCorrectly()
        {
            string remoteTableName = "#TestTempTableName";
            DataTable remoteData;

            string remoteConnectionString = _framework.GetCatiSqlServerConnectionString(RemoteDatabaseName);
            using (var remoteConnectionProvider = new RemoteConnectionProvider(remoteConnectionString))
            {
                using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
                using (var transactionScope = new DatabaseTransactionScope("TestTransaction"))
                {
                    _remoteDataCopier.CopyDataToNewTable(connectionScope.ConnectionString, remoteConnectionProvider, remoteTableName, $"SELECT * FROM [{LocalTableName}]");

                    transactionScope.Commit();
                }

                using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [{remoteTableName}]", remoteConnectionProvider.Connection))
                {
                    remoteData = _remoteDatabaseEngine.ExecuteDataTable<DataTable>(sqlCommand);
                }
            }

            CompareTwoTables(remoteData);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeNewTempTable_SelectContainsColumnWithoutName_ArgumentExceptionOccured()
        {
            string localTableName = "#TestTempTableName";
            using (var connectionScope = new ConnectionScope(_framework.GetCatiSqlServerConnectionString(LocalDatabaseName)))
            {
                _remoteDataCopier.CopyDataToNewTable(_framework.GetCatiSqlServerConnectionString(RemoteDatabaseName), connectionScope, localTableName, $"SELECT *, 1 FROM [{RemoteTableName}]");
            }
        }

        private void CompareTwoTables(DataTable localData)
        {            
            Assert.AreEqual(_remoteData.Rows.Count, localData.Rows.Count, "Wrong count of rows");
            for (int i = 0; i < localData.Rows.Count; i++)
            {
                DataRow remoteRow = _remoteData.Rows[i];
                DataRow localRow = localData.Rows[i];
                
                Assert.AreEqual(remoteRow.ItemArray.Count(), localRow.ItemArray.Count(), "Wrong count of columns");

                for (int j = 0; j < localRow.ItemArray.Count(); j++)
                {
                    if (remoteRow.ItemArray[j].GetType() == typeof(byte[]))
                    {
                        CompareTwoByteArray((byte[])remoteRow.ItemArray[j], (byte[])localRow.ItemArray[j]);
                    }
                    else
                    {
                        Assert.AreEqual(remoteRow.ItemArray[j], localRow.ItemArray[j], "Wrong data in the copied table");
                    }
                }
            }
        }

        private void CompareTwoByteArray(byte[] remote, byte[] local)
        {
            Assert.AreEqual(remote.Length, local.Length, "Wrong data in the copied table");
            for (int i = 0; i < local.Length; i++)
            {
                Assert.AreEqual(remote[i], local[i], "Wrong data in the copied table");
            }
        }
    }
}
