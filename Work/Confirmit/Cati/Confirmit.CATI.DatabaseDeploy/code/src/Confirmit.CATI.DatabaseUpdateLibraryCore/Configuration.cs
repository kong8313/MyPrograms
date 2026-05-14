using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class Configuration : IConfiguration
    {
        public string SqlServerName { get; }
        public string SqlUserName { get; }
        public string SqlPassword { get; }
        public string SqlAdminUserName { get; }
        public string SqlAdminPassword { get; }
        public string DefaultDatabaseName { get; }
        public string DatabaseNamePattern { get; }
        public string CatiDatabaseServerDataPath { get; }
        public string CatiDatabaseServerLogPath { get; }
        public bool IsDbCreation { get; set; }
        public string AzureSqlServerEdition { get; set; }

        public Configuration(string sqlServerName, string sqlUserName, string sqlPassword,
            string sqlAdminUserName, string sqlAdminPassword,
            string catiDatabaseServerDataPath, string catiDatabaseServerLogPath, string azureSqlServerEdition = "")
        {
            SqlServerName = sqlServerName;
            SqlUserName = sqlUserName;
            SqlPassword = sqlPassword;
            SqlAdminUserName = sqlAdminUserName;
            SqlAdminPassword = sqlAdminPassword;
            DefaultDatabaseName = "ConfirmitCATIV15";
            DatabaseNamePattern = @"(^ConfirmitCATIV15_\d+$)|(^ConfirmitCATIV15$)";
            CatiDatabaseServerDataPath = catiDatabaseServerDataPath;
            CatiDatabaseServerLogPath = catiDatabaseServerLogPath;
            IsDbCreation = false;
            AzureSqlServerEdition = azureSqlServerEdition;
        }
    }
}