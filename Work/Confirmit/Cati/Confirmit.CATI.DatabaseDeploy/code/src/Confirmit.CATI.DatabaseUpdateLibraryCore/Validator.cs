using System;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class Validator : IValidator
    {
        private readonly IResources _resources;
        private readonly IDatabaseWorker _databaseWorker;
        private readonly IConfiguration _configuration;

        public Validator(IResources resources, IDatabaseWorker databaseWorker, IConfiguration configuration)
        {
            _resources = resources;
            _databaseWorker = databaseWorker;
            _configuration = configuration;
        }

        public void CheckDatabases(string[] productionDatabases)
        {
            foreach (string databaseForUpdate in productionDatabases)
            {
                if (!_databaseWorker.IsDatabaseExists(databaseForUpdate))
                {
                    throw new Exception($"Database {databaseForUpdate} does not exists or access denied");
                }

                if (_databaseWorker.GetUserAccess(databaseForUpdate) == DatabaseUserAccess.Single)
                {
                    throw new Exception($"Database {databaseForUpdate} has wrong user access (SINGLE_USER).");
                }
            }
        }

        public void CheckUpdateScripts()
        {
            foreach (var updateScriptInfo in _resources.UpdateScriptInfos)
            {
                if (string.IsNullOrEmpty(updateScriptInfo.ScriptText))
                {
                    throw new Exception($"'{updateScriptInfo.Name}' script isn't found");
                }

                if (string.IsNullOrEmpty(updateScriptInfo.Description))
                {
                    throw new Exception($"'{updateScriptInfo.Name}' script hasn't description");
                }
            }
        }
    }
}