using System;
using System.Data.SqlClient;
using System.IO;
using ILogger = TeamCityBuildEngine.Interfaces.ILogger;

namespace TeamCityBuildEngine
{
    public class FileCleaner
    {
        private readonly ILogger _logger;

        public FileCleaner(ILogger logger)
        {
            _logger = logger;
        }

        public void CleanOldDatabaseFiles(string sqlInstanceName)
        {
            if (string.IsNullOrWhiteSpace(sqlInstanceName))
            {
                sqlInstanceName = "localhost";
            }

            string dataPath = GetSqlDefaultDataPath(sqlInstanceName);
            _logger.WriteLog($"Default SQL ({sqlInstanceName}) data path: {dataPath}");

            string backupPath = Path.GetFullPath(Path.Combine(dataPath, @"..\cati_integration_tests_backup\"));
            _logger.WriteLog($"Backup path: {backupPath}");

            if (Directory.Exists(dataPath))
            {
                DateTime weekAgoTime = DateTime.Now.AddDays(-7);

                foreach (string filePath in Directory.GetFiles(dataPath, "ConfirmitCATIV15*.*"))
                {
                    if (new FileInfo(filePath).CreationTime < weekAgoTime)
                    {
                        DeleteFileWithLogging(filePath);
                    }
                }

                foreach (string filePath in Directory.GetFiles(dataPath, "survey*.*"))
                {
                    if (new FileInfo(filePath).CreationTime < weekAgoTime)
                    {
                        DeleteFileWithLogging(filePath);
                    }
                }
            }

            if (Directory.Exists(backupPath))
            {
                foreach (string subDirectoryPath in Directory.GetDirectories(backupPath))
                {
                    foreach (string filePath in Directory.GetFiles(subDirectoryPath))
                    {
                        DeleteFileWithLogging(filePath);
                    }

                    if (Directory.GetFiles(subDirectoryPath).Length == 0)
                    {
                        Directory.Delete(subDirectoryPath, false);
                    }
                }
            }
        }

        private void DeleteFileWithLogging(string filePath)
        {
            try
            {
                File.Delete(filePath);
                _logger.WriteLog($"File {filePath} was removed");
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"Can't remove file {filePath}.\r\nException: {ex}");
            }
        }

        private string GetSqlDefaultDataPath(string sqlInstanceName)
        {
            var connectionString = $"Data Source={sqlInstanceName};Initial Catalog=master;User ID=sa;Password=firm";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT SERVERPROPERTY('InstanceDefaultDataPath')", sqlConnection))
            {
                sqlConnection.Open();

                return (string)sqlCommand.ExecuteScalar();
            }
        }
    }
}
