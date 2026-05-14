using System;
using System.Reflection;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.UnitTests.FakeClasses
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

        public FakeConfiguration()
        {
            SqlServerName = "localhost";
            SqlUserName = "sa";
            SqlPassword = "firm";
            DefaultDatabaseName = "ConfirmitCATIV15";
            DatabaseNamePattern = @"(ConfirmitCATIV15_\d+$)|(ConfirmitCATIV15$)";
            ConfirmlogConnectionString = @"Data Source=localhost;Initial Catalog=Confirmlog;User ID=sa;Password=firm;Connect Timeout=120";
            ProductVersion = Assembly.GetExecutingAssembly().GetName().Version;
            IsDBCreation = false;
            ConfirmitLinkedServerIsLoopBack = false;
        }
    }
}