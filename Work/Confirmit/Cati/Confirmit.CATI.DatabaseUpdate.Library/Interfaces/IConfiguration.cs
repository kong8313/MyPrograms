using System;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
{
    public interface IConfiguration
    {
        string SqlServerName { get; }

        string SqlUserName { get; }

        string SqlPassword { get; }

        string DefaultDatabaseName { get; }

        string DatabaseNamePattern { get; }

        string ConfirmlogConnectionString { get; }

        Version ProductVersion { get; }

        bool IsDBCreation { get; }
    }
}