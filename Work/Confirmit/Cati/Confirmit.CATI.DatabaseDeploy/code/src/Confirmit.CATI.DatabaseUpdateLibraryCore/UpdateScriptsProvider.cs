using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class UpdateScriptsProvider : IUpdateScriptsProvider
    {
        private readonly IResources _resources;
        private readonly IUpdateScriptDatabaseWorker _updateScriptDatabaseWorker;
        private readonly Dictionary<string, List<UpdateScriptInfo>> _scriptsToExecuteCache;

        public UpdateScriptsProvider(IResources resources, IUpdateScriptDatabaseWorker updateScriptDatabaseWorker)
        {
            _resources = resources;
            _updateScriptDatabaseWorker = updateScriptDatabaseWorker;

            _scriptsToExecuteCache = new Dictionary<string, List<UpdateScriptInfo>>();
        }

        public List<UpdateScriptInfo> GetScriptsToApply(string databaseName)
        {
            if (_scriptsToExecuteCache.ContainsKey(databaseName))
            {
                return _scriptsToExecuteCache[databaseName];
            }

            UpdateScriptInfo[] appliedUpdateScriptInfos = _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfos(databaseName);
            var scriptsToExecute = _resources.UpdateScriptInfos.Except(appliedUpdateScriptInfos).ToList();
            _scriptsToExecuteCache.Add(databaseName, scriptsToExecute);
            return scriptsToExecute;
        }

        public List<UpdateScriptInfo> GetScriptsToValidate(string databaseName)
        {
            var scriptsToExecute = GetScriptsToApply(databaseName);

            if (CheckAllLastUpdateScriptsAreUnsafe(scriptsToExecute))
            {
                return scriptsToExecute.Where(x => !x.HasSqlScriptUnsafeType).ToList();
            }

            return scriptsToExecute;
        }

        private bool CheckAllLastUpdateScriptsAreUnsafe(List<UpdateScriptInfo> scriptsToExecute)
        {
            if (scriptsToExecute.All(x => !x.HasSqlScriptUnsafeType))
            {
                return false;
            }

            List<UpdateScriptInfo> scriptsToExecuteCopy = new List<UpdateScriptInfo>(scriptsToExecute);

            scriptsToExecuteCopy.Reverse();
            return scriptsToExecuteCopy.SkipWhile(x => x.HasSqlScriptUnsafeType).All(y => !y.HasSqlScriptUnsafeType);
        }
    }
}