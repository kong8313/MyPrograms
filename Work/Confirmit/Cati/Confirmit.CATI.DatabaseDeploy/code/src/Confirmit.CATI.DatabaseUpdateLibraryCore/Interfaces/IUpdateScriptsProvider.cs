using System.Collections.Generic;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IUpdateScriptsProvider
    {
        List<UpdateScriptInfo> GetScriptsToValidate(string databaseName);

        List<UpdateScriptInfo> GetScriptsToApply(string databaseName);
    }
}