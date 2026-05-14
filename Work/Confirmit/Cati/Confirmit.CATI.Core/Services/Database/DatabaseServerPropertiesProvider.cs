using System;
using System.Data;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database
{
    public class DatabaseServerPropertiesProvider : IDatabaseServerPropertiesProvider
    {
        public EngineEdition GetEngineEdition()
        {
            string query = String.Format(@"SELECT SERVERPROPERTY('EngineEdition')");

            return (EngineEdition)new DatabaseEngine().ExecuteScalar<int>(query, CommandType.Text);
        }

        public Version GetProductVersion()
        {
            string query = String.Format(@"SELECT SERVERPROPERTY('ProductVersion')");

            var version = new DatabaseEngine().ExecuteScalar<string>(query, CommandType.Text);

            return Version.Parse(version);
        }
    }

    
}
