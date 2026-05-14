using System.Linq;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;

namespace Confirmit.CATI.DatabaseUpdateLibrary
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
            if (!productionDatabases.Any())
            {
                throw new ValidateException(string.Format("No databases for update with DatabaseNamePattern {0}", _configuration.DatabaseNamePattern));
            }

            if (!_databaseWorker.IsDatabaseExists(_configuration.DefaultDatabaseName))
            {
                throw new ValidateException(string.Format("Default database {0} does not exists or access denied", _configuration.DefaultDatabaseName));
            }

            foreach (string databaseForUpdate in productionDatabases)
            {
                if (!_databaseWorker.IsDatabaseExists(databaseForUpdate))
                {
                    throw new ValidateException(string.Format("Database {0} does not exists or access denied", databaseForUpdate));
                }

                if (_databaseWorker.GetUserAccess(databaseForUpdate) == DatabaseUserAccess.Single)
                {
                    throw new ValidateException(string.Format("Database {0} has wrong user access (SINGLE_USER).", databaseForUpdate));
                }
            }
        }

        public void CheckUpdateScripts()
        {
            int cnt = 0;
            foreach (UpdateScriptInfo updateScriptInfo in _resources.UpdateScriptInfos)
            {
                if (string.IsNullOrEmpty(updateScriptInfo.ScriptText))
                {
                    throw new ValidateException(string.Format("'{0}' script isn't found", updateScriptInfo.Name));
                }

                if (string.IsNullOrEmpty(updateScriptInfo.Description))
                {
                    throw new ValidateException(string.Format("'{0}' script hasn't description", updateScriptInfo.Name));
                }

                if (cnt > 94 && !updateScriptInfo.Description.Contains("CATI"))
                {
                    throw new ValidateException(string.Format("'{0}' script description hasn't information about jira issue", updateScriptInfo.Name));
                }
                cnt++;
            }
        }
    }
}