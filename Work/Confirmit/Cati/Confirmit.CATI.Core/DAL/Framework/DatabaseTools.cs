using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class DatabaseTools
    {
        private const int Timeout = 1000*60;
        private const string BackupDirectory = "cati_integration_tests_backup";

        private readonly string _connectionString;

        public DatabaseTools(string connectionString)
        {
            _connectionString = BuildMasterConnectionString(connectionString);
        }

        private string BuildMasterConnectionString(string connectionString)
        {
            return BuildNewConnectionString(connectionString, "master");
        }

        private string BuildNewConnectionString(string connectionString, string database)
        {
            var cb = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = database, Pooling = false };
            return cb.ToString();
        }

        public bool IsConnectionStringValid(out SqlException excetpionThrown)
        {
            excetpionThrown = null;

            try
            {
                using (var cn = new SqlConnection(_connectionString))
                {
                    cn.Open();
                    return true;
                }
            }
            catch (SqlException e)
            {
                excetpionThrown = e;
                return false;
            }
        }

        private const int DeadlockErrorCode = 1205;
        private static readonly int[] TimeoutAndConnectionErrorCodes = { -2, 121, 64, 258 };

        private static bool IsTimeoutOrConnectionError(SqlException ex)
        {
            foreach (SqlError error in ex.Errors)
            {
                if (Array.Exists(TimeoutAndConnectionErrorCodes, code => code == error.Number))
                {
                    return true;
                }
            }
            return false;
        }

        // TODO: Extract deadlock related stuff in to the separate class
        public bool IsNotDeadlock(SqlException ex)
        {
            return ex.Number != DeadlockErrorCode;
        }

        public static bool IsDeadlock(SqlException ex)
        {
            return ex.Number == DeadlockErrorCode;
        }

        public T RetryOnDeadlock<T>(Func<T> f, [CallerMemberName] string caller = "")
        {
            for (int attempt = 0; attempt < 5; ++attempt)
            {
                try
                {
                    return f();
                }
                catch (SqlException ex)
                {
                    Trace.TraceError("Caller: {0}\r\nSqlException:\r\n{1}\r\n\r\n", caller, ex);

                    if (IsNotDeadlock(ex))
                    {
                        throw;
                    }

                    SleepAfterDeadlock();
                }
            }

            throw new Exception("All attempts to retry are failed with deadlock");
        }

        public void RetryOnDeadlock(Action a, [CallerMemberName] string caller = "")
        {
            for (int attempt = 0; attempt < 5; ++attempt)
            {
                try
                {
                    a();

                    return;
                }
                catch (SqlException ex)
                {
                    Trace.TraceError("Caller: {0}\r\nSqlException:\r\n{1}\r\n\r\n", caller, ex);

                    if (IsNotDeadlock(ex))
                    {
                        throw;
                    }

                    SleepAfterDeadlock();
                }
            }

            throw new Exception("All attempts to retry are failed with deadlock");
        }

        private void SleepAfterDeadlock()
        {
            var rnd = new Random();
            var delay = 2000 + rnd.Next(100, 10000);

            Trace.TraceInformation("Waiting {0} milliseconds after deadlock\r\n", delay);

            Thread.Sleep(delay);
        }

        public bool IsDatabaseExists(string dbName)
        {
            return RetryOnDeadlock(() =>
            {
                var dbEngine = new DatabaseEngine(_connectionString);

                var isExists = dbEngine.ExecuteScalarWithSpecificTimeOut<int>(
                    string.Format(
                        "IF EXISTS(SELECT NULL FROM sys.databases WHERE [Name] = '{0}') SELECT 1 ELSE SELECT 0",
                        dbName),
                        CommandType.Text,
                        Timeout);

                return isExists == 1;
            });
        }

        public string GetDatabaseDirectory(string dbName)
        {
            return RetryOnDeadlock(() =>
            {
                var query = string.Format(
                        @"
                        SELECT 
                            physical_name 
                        FROM 
                           sys.master_files 
                        WHERE 
                           database_id = DB_ID('{0}') AND (data_space_id = 1) AND type_desc = 'ROWS'
                        ", dbName);
                var dbEngine = new DatabaseEngine(_connectionString);

                var physicalName = dbEngine.ExecuteScalarWithSpecificTimeOut<string>(
                    query,
                    CommandType.Text,
                    Timeout);

                return Path.GetDirectoryName(physicalName);
            });
        }

        public void DropDatabase(string dbName)
        {
            if (!IsDatabaseExists(dbName))
            {
                return;
            }

            RetryOnDeadlock(() =>
            {
                SqlConnection.ClearAllPools();

                using (var connectionScope = new ConnectionScope(_connectionString))
                {
                    var dbEngine = new DatabaseEngine(_connectionString);

                    dbEngine.ExecuteNonQueryWithSpecificTimeOut(
                        string.Format(@"ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbName),
                        CommandType.Text,
                        Timeout);

                    dbEngine.ExecuteNonQueryWithSpecificTimeOut(
                        string.Format(@"DROP DATABASE [{0}]", dbName),
                        CommandType.Text,
                        Timeout);
                }
            });
        }

        private string GetTempBackupPath()
        {
            var masterDatabasePath = GetDatabaseDirectory("master");

            return Path.Combine(masterDatabasePath, "TempDefaultDatabase.bak");
        }

        public void CreateNewInstanceDatabase(
           string newDatabaseName,
           string sqlServerDataPath,
           string sqlServerLogPath)
        {
            var sqlConnection = new SqlConnection(BackendInstance.Current.DefaultInstanceConnectionString);

            string backupFilePath = GetTempBackupPath();

            CreateBackupFileFromDatabase(sqlConnection.Database, backupFilePath);

            CreateDatabaseFromBackupFile(newDatabaseName, backupFilePath, sqlServerDataPath, sqlServerLogPath);
        }

        /// <summary>
        /// Creates database from backup file.
        /// </summary>
        /// <param name="newDatabaseName">Restored database name</param>
        /// <param name="backupFilePath">Backup file location path</param>
        /// <param name="sqlServerDataPath">Path where new DB data files should be stored</param>
        /// <param name="sqlServerLogPath">Path where new DB log files should be stored</param>
        private void CreateDatabaseFromBackupFile(
           string newDatabaseName,
           string backupFilePath,
           string sqlServerDataPath,
           string sqlServerLogPath)
        {

            if (string.IsNullOrEmpty(sqlServerDataPath) ^ string.IsNullOrEmpty(sqlServerLogPath))
            {
                throw new ArgumentException("Specify both paths for storing new DB files, or leave settings SqlServerDataPath and SqlServerLogPath blank");
            }

            DropDatabase(newDatabaseName);

            var relocationFiles = GetRelocationFiles(newDatabaseName, backupFilePath, sqlServerDataPath, sqlServerLogPath);

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("RESTORE DATABASE {0} ", newDatabaseName);
            queryBuilder.AppendFormat("FROM DISK = N'{0}' ", backupFilePath);
            queryBuilder.Append("WITH NEW_BROKER");

            foreach (var file in relocationFiles)
            {
                queryBuilder.AppendFormat(", MOVE N'{0}' TO N'{1}' ", file.Key, file.Value);
            }

            var dbEngine = new DatabaseEngine(_connectionString);

            dbEngine.ExecuteNonQuery(queryBuilder.ToString(), CommandType.Text);
        }

        private IEnumerable<KeyValuePair<string, string>> GetRelocationFiles(string newDatabaseName,
                                                                             string backupFilePath,
                                                                             string sqlServerDataPath,
                                                                             string sqlServerLogPath)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrEmpty(sqlServerDataPath) || string.IsNullOrEmpty(sqlServerLogPath))
            {
                sqlServerDataPath = GetSqlServerDefaultDataPath();
                sqlServerLogPath = GetSqlServerDefaultLogPath();
            }

            var query = $"RESTORE FILELISTONLY FROM  DISK = N'{backupFilePath}' WITH  NOUNLOAD";
            var dataTable =  new DatabaseEngine(_connectionString).ExecuteDataTable<DataTable>(query, CommandType.Text);
            foreach (DataRow row in dataTable.Rows)
            {
                var logicalName = row["LogicalName"].ToString();
                var extention = Path.GetExtension(row["PhysicalName"].ToString()).ToLower();

                string newPhysicalName;
                if (extention == ".mdf")
                {
                    newPhysicalName = Path.Combine(
                        sqlServerDataPath,
                        Path.ChangeExtension(newDatabaseName, extention));
                }
                else
                {
                    newPhysicalName = Path.Combine(
                        sqlServerLogPath,
                        Path.ChangeExtension(newDatabaseName + "_log", extention));
                }

                result.Add(new KeyValuePair<string, string>(logicalName, newPhysicalName));
            }

            return result;
        }

        /// <summary>
        /// Creates backup file from the database.
        /// </summary>
        /// <param name="databaseName">Database name to backup</param>
        /// <param name="backupFilePath">File path for the new backup file location</param>
        private void CreateBackupFileFromDatabase(
           string databaseName,
           string backupFilePath)
        {
            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);

            new DatabaseEngine(_connectionString).ExecuteNonQuery($"BACKUP DATABASE {databaseName} TO DISK = N'{backupFilePath}' WITH NOFORMAT, INIT, NOSKIP, REWIND, NOUNLOAD, STATS = 10, CHECKSUM, CONTINUE_AFTER_ERROR");
        }

        private string GetDatabaseState(string databaseName)
        {
            using (var connectionScope = new ConnectionScope(_connectionString))
            {
                var dbEngine = new DatabaseEngine(_connectionString);

                var query = string.Format(@"SELECT state_desc FROM sys.databases WHERE [Name] = '{0}'", databaseName);

                var states = dbEngine.ExecuteScalarList<string>(new SqlCommand(query, connectionScope.Connection));

                if (states.Count == 0)
                {
                    throw new Exception(string.Format("Database {0} does not exists", databaseName));
                }

                if (states.Count > 1)
                {
                    throw new Exception(string.Format("Many ({0}) databases with same name {1} found", states.Count, databaseName));
                }

                return states.First();
            }
        }

        /// <summary>
        /// Detaches database and deletes its files from the disk.
        /// </summary>
        public void DetachDatabase(string databaseName)
        {
            Trace.TraceInformation("Detaching database {0}", databaseName);

            var state = GetDatabaseState(databaseName);

            Trace.TraceInformation("Database {0} state is {1}", databaseName, state);

            if (state == "OFFLINE")
            {
                // Database is already offline, so no problem, it is not an error, let's just return
                return;
            }

            RetryOnDeadlock(() =>
            {
                // Clearing pools SIGNIFIACLLY speeds up detaching database
                SqlConnection.ClearAllPools();

                using (var connectionScope = new ConnectionScope(_connectionString))
                {
                    var dbEngine = new DatabaseEngine(_connectionString);

                    dbEngine.ExecuteNonQueryWithSpecificTimeOut(
                        string.Format(@"ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE", databaseName),
                        CommandType.Text,
                        Timeout);
                }
            });
        }

        /// <summary>
        /// Copies stored database files to the proper place,
        /// attaches database.
        /// </summary>
        /// <param name="databaseName">Attached database name</param>
        public void AttachDatabase(string databaseName)
        {
            Trace.TraceInformation("Attaching database {0}", databaseName);

            var state = GetDatabaseState(databaseName);

            Trace.TraceInformation("Database {0} state is {1}", databaseName, state);

            if (state != "OFFLINE")
            {
                throw new Exception(string.Format("Cannot attach database {0}, database state is {1}", databaseName, state));
            }

            RetryOnDeadlock(() =>
            {
                // Clearing pools SIGNIFIACLLY speeds up attaching/detaching database
                SqlConnection.ClearAllPools();

                using (var connectionScope = new ConnectionScope(_connectionString))
                {
                    var dbEngine = new DatabaseEngine(_connectionString);

                    dbEngine.ExecuteNonQueryWithSpecificTimeOut(
                        string.Format(@"ALTER DATABASE [{0}] SET ONLINE WITH ROLLBACK IMMEDIATE", databaseName),
                        CommandType.Text,
                        Timeout);
                }

                Trace.TraceInformation("Database {0} is attached and  online, waiting for availability", databaseName);

                WaitWhileDatabaseIsNotAvailable(databaseName);
            });
        }

        private void WaitWhileDatabaseIsNotAvailable(string database)
        {
            var cb = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = database };
            var cs = cb.ToString();
            var dbEngine = new DatabaseEngine(cs);

            SqlException exception = null;
            for (int attempt = 0; attempt < 1000 * 60; ++attempt)
            {
                try
                {
                    dbEngine.ExecuteScalarWithSpecificTimeOut<int>(
                        "SELECT 1",
                        CommandType.Text,
                        10);

                    Trace.TraceInformation(
                        "Database {0} is available on attempt {1}",
                        database,
                        attempt);

                    return;
                }
                catch (SqlException e)
                {
                    exception = e;
                    Thread.Sleep(1);
                }
            }

            throw new Exception(
                "Database {0} has not become available after 1 minute",
                exception);
        }

        public void BackupDatabaseFiles(string databaseName, string subfolder)
        {
            BackupDatabaseFile(GetRowsFileName(databaseName), subfolder);
            BackupDatabaseFile(GetLogFileName(databaseName), subfolder);
        }

        public bool IsDatabaseFilesBackupAvailable(string databaseName, string subfolder)
        {
            return 
                IsDatabaseFileBackupAvailable(GetRowsFileName(databaseName), subfolder) &&
                IsDatabaseFileBackupAvailable(GetLogFileName(databaseName), subfolder);
        }

        public void RestoreDatabaseFiles(string databaseName, string subfolder)
        {
            RestoreDatabaseFile(GetRowsFileName(databaseName), subfolder, databaseName);
            RestoreDatabaseFile(GetLogFileName(databaseName), subfolder, databaseName);
        }

        public void DeleteDatabaseFiles(string databaseName)
        {
            Trace.TraceInformation("Deleting files for the database {0}", databaseName);

            var rowsFile = GetRowsFileName(databaseName);
            var logsFile = GetLogFileName(databaseName);

            DeleteFileIfExists(rowsFile);
            DeleteFileIfExists(logsFile);
        }

        private void DeleteFileIfExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            else
            {
                Trace.TraceWarning("Cannot delete file {0}, file does not exists", fileName);
            }
        }

        public string GetRowsFileFolder(string databaseName)
        {
            return Path.GetDirectoryName(GetRowsFileName(databaseName));
        }

        public string GetLogFileFolder(string databaseName)
        {
            return Path.GetDirectoryName(GetLogFileName(databaseName));
        }

        public string GetSqlServerDefaultDataPath()
        {
            var query = "SELECT SERVERPROPERTY('InstanceDefaultDataPath')";

            var dbEngine = new DatabaseEngine(_connectionString);

            var path = dbEngine.ExecuteScalarWithSpecificTimeOut<string>(
                query,
                CommandType.Text,
                Timeout);

            return path;
        }

        public string GetSqlServerDefaultLogPath()
        {
            var query = "SELECT SERVERPROPERTY('InstanceDefaultLogPath')";

            var dbEngine = new DatabaseEngine(_connectionString);

            var path = dbEngine.ExecuteScalarWithSpecificTimeOut<string>(
                query,
                CommandType.Text,
                Timeout);

            return path;
        }

        public string GetRowsFileName(string databaseName)
        {
            var result = Path.GetDirectoryName(GetSqlServerDefaultDataPath()) + "\\" + databaseName + ".mdf";

            return result;
        }

        public string GetLogFileName(string databaseName)
        {
            var result = Path.GetDirectoryName(GetSqlServerDefaultLogPath()) + "\\" + databaseName + "_log.ldf";

            return result;
        }

        private void BackupDatabaseFile(string path, string subfolder)
        {
            var destinationDirectory = GetBackupDirectory(path, subfolder);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            var destination = GetBackupFilePath(path, subfolder);

            File.Copy(path, destination, true);
        }

        private bool IsDatabaseFileBackupAvailable(string path, string subfolder)
        {
            var destination = GetBackupFilePath(path, subfolder);

            return File.Exists(destination);
        }

        private void RestoreDatabaseFile(string destinationFilePath, string subfolder, string database)
        {
            bool restored = false;

            Exception e = null;

            var backupedFilePath = GetBackupFilePath(destinationFilePath, subfolder);

            for (int attempt = 0; attempt < 5; ++attempt)
            {
                try
                {
                    File.Copy(backupedFilePath, destinationFilePath, true);
                    restored = true;

                    if (attempt > 0)
                    {
                        Trace.TraceWarning(
                            "Database {0} file {1} is restored on {2} attempt",
                            database,
                            destinationFilePath,
                            attempt);
                    }

                    break;
                }
                catch (Exception ex)
                {
                    e = ex;

                    var state = GetDatabaseState(database);
                    if (state != "OFFLINE")
                    {
                        throw new Exception(
                            string.Format(
                                "Cannot restore database {0} files, database state is {1}",
                                database,
                                state));
                    }

                    Thread.Sleep(1);
                }
            }

            if (!restored)
            {
                var state = GetDatabaseState(database);
                throw new Exception(
                    string.Format(
                        "Failed to restore database file '{0}'.\r\nDatabase state: {1}\r\nLast exception message: '{2}'\r\n\r\nLast Exception:\r\n{3}", 
                        destinationFilePath,
                        state,
                        e.Message
                        , e));
            }
        }

        private string GetBackupDirectory(string path, string subfolder)
        {
            var destination = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(path),
                "..",
                BackupDirectory,
                subfolder));

            return destination;
        }

        private string GetBackupFilePath(string path, string subfolder)
        {
            var destination = Path.Combine(
                GetBackupDirectory(path, subfolder),
                Path.GetFileName(path));

            return destination;
        }

        public void CreateEmptyDatabase(string databaseName)
        {
            var dbEngine = new DatabaseEngine(_connectionString);

            var query = $"CREATE DATABASE [{databaseName}]";

            dbEngine.ExecuteNonQueryWithSpecificTimeOut(
                query,
                CommandType.Text,
                Timeout);

            RetryOnDeadlock(() =>
            {
                query = string.Format(
                        @"
                        ALTER DATABASE [{0}] SET RECOVERY SIMPLE;
                        ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION ON;
                        ALTER DATABASE[{0}] SET READ_COMMITTED_SNAPSHOT ON;
                        ", databaseName);

                dbEngine.ExecuteNonQueryWithSpecificTimeOut(
                    query,
                    CommandType.Text,
                    Timeout);
            });
        }

        public static void BulkRemove(string deleteQuery, int batchSize = 5000)
        {
            var dbEngine = new DatabaseEngine();

            if (!deleteQuery.Contains("DELETE") || deleteQuery.ToUpperInvariant().Contains("SELECT"))
            {
                throw new ArgumentException("Invalid query: it must contain the 'DELETE' statement (in uppercase) and must not contain the 'SELECT' statement.");
            }

            if (!deleteQuery.ToUpperInvariant().Contains("DELETE TOP"))
            {
                deleteQuery = deleteQuery.Replace("DELETE", $"DELETE TOP ({batchSize})");
            }

            deleteQuery += "\r\n SELECT @@ROWCOUNT";
            
            int removedRows;
            do
            {
                removedRows = dbEngine.ExecuteScalar<int>(deleteQuery);
                Thread.Sleep(100);
            } while (removedRows > 0);
        }

        /// <summary>
        /// TODO: Move to IBulkCopy interface
        /// Adds data to the database using bulk.
        /// </summary>
        public static void BulkAdd<TTable, TEntity>(TTable table,
            Action<TTable, TEntity> saveInTableAction,
            IEnumerable<TEntity> items,
            int batchSize,
            int timeout)
            where TTable : DataTable
        {
            using (var connection = new ConnectionScope())
            {
                SqlTransaction sqlTransaction =
                    DatabaseTransactionScope.Current != null ? DatabaseTransactionScope.Current.Transaction : null;

                var bulk = new SqlBulkCopy(connection.Connection, SqlBulkCopyOptions.FireTriggers, sqlTransaction)
                {
                    BatchSize = batchSize,
                    BulkCopyTimeout = timeout,
                    DestinationTableName = table.TableName
                };

                foreach (var item in items)
                {
                    saveInTableAction(table, item);
                }
                
                bulk.WriteToServer(table);
            }
        }

        public bool IsServiceBrokerEnabled(string databaseName)
        {
            var dbEngine = new DatabaseEngine(_connectionString);

            return dbEngine.ExecuteScalar<bool>(
                "select is_broker_enabled from sys.databases where name = @databaseName", CommandType.Text, new SqlParameter("@databaseName", databaseName));
        }

        public void CreateDatabase(string databaseName, string script)
        {
            CreateEmptyDatabase(databaseName);

            var newDatabaseConnectionString = BuildNewConnectionString(_connectionString, databaseName);

            new DatabaseEngine(newDatabaseConnectionString).ExecuteBatch(script);
        }

        private static int CalculateRetryDelay(int baseDelay, int attemptNumber)
        {
            return baseDelay * (int)Math.Pow(2, attemptNumber - 1); // Exponential backoff
        }
        public static void RetryWithDelay(int attempts, int delayMilliseconds, Action retryAction, string description)
        {
            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                try
                {
                    retryAction();
                    return;
                }
                catch (SqlException ex) when (IsDeadlock(ex) || IsTimeoutOrConnectionError(ex))
                {
                    var errorType = IsDeadlock(ex) ? "Deadlock" : "Timeout/Connection Error";
                    var message = $"Attempt {attempt} of '{description}' has failed due to a {errorType}. Exception: {ex}. Retrying in {delayMilliseconds}ms...";

                    if (attempt < attempts)
                    {
                        Trace.TraceWarning(message);
                        var delay = CalculateRetryDelay(delayMilliseconds, attempt);
                        Thread.Sleep(delay);
                    }
                    else
                    {
                        Trace.TraceError($"All retry attempts  of '{description}' failed: {ex}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Non-retryable error of '{description}': {ex}");
                    throw;
                }
            }
        }
    }
}
