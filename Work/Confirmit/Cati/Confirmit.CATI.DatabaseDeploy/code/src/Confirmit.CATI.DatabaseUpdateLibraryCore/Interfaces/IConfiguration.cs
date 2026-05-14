namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IConfiguration
    {
        string SqlServerName { get; }

        string SqlUserName { get; }

        string SqlPassword { get; }

        string SqlAdminUserName { get; }

        string SqlAdminPassword { get; }

        string DefaultDatabaseName { get; }

        string DatabaseNamePattern { get; }

        string CatiDatabaseServerDataPath { get; }

        string CatiDatabaseServerLogPath { get; }

        bool IsDbCreation { get; set; }
        string AzureSqlServerEdition { get; set; }
    }
}