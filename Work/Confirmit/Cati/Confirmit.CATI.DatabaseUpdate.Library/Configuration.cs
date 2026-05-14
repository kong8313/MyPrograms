using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class Configuration : IConfiguration
    {
        public string SqlServerName { get; private set; }
        public string SqlUserName { get; private set; }
        public string SqlPassword { get; private set; }
        public string DefaultDatabaseName { get; private set; }
        public string DatabaseNamePattern { get; private set; }
        public string ConfirmlogConnectionString { get; private set; }
        public Version ProductVersion { get; private set; }
        public bool IsDBCreation { get; private set; }

        public Configuration(string sqlServerName, string sqlUserName, string sqlPassword, 
            string confirmlogConnectionString, Version productVersion, bool isDBCreation)
        {
            SqlServerName = sqlServerName;
            SqlUserName = sqlUserName;
            SqlPassword = sqlPassword;
            DefaultDatabaseName = "ConfirmitCATIV15";
            DatabaseNamePattern = @"(^ConfirmitCATIV15_\d+$)|(^ConfirmitCATIV15$)";
            ConfirmlogConnectionString = confirmlogConnectionString;
            ProductVersion = productVersion;
            IsDBCreation = isDBCreation;
        }
    }
}