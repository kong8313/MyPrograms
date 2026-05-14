using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses
{    
    public class FakeConfiguration : IConfiguration
    {
        public string SqlServerName { get; set; }
        public string SqlUserName { get; set; }
        public string SqlPassword { get; set; }
        public string DefaultDatabaseName { get; set; }
        public string DatabaseNamePattern { get; set; }
        public string ConfirmlogConnectionString { get; set; }
        public Version ProductVersion { get; set; }
        public bool IsDBCreation { get; set; }
        public string ConfirmitLinkedServerName { get; set; }
        public bool ConfirmitLinkedServerIsLoopBack { get; set; }

        public FakeConfiguration(string sqlServerName, string sqlUserName, string sqlPassword, string confirmlogConnectionString, Version productVersion, bool isDBCreation)
            : this(sqlServerName, sqlUserName, sqlPassword, "ConfirmitCATIV15", @"(ConfirmitCATIV15_\d+$)|(ConfirmitCATIV15$)", 
                   confirmlogConnectionString, productVersion, isDBCreation, string.Empty)
        {
            
        }

        public FakeConfiguration(
            string sqlServerName, string sqlUserName, string sqlPassword, string defaultDatabaseName, string databaseNamePattern, 
            string confirmlogConnectionString, Version productVersion, bool isDBCreation, string confirmitLinkedServerName)
        {
            SqlServerName = sqlServerName;
            SqlUserName = sqlUserName;
            SqlPassword = sqlPassword;
            DefaultDatabaseName = defaultDatabaseName;
            DatabaseNamePattern = databaseNamePattern;
            ConfirmlogConnectionString = confirmlogConnectionString;
            ProductVersion = productVersion;
            IsDBCreation = isDBCreation;
            ConfirmitLinkedServerName = confirmitLinkedServerName;
            ConfirmitLinkedServerIsLoopBack = false;
        }
    }
}