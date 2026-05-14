using System.Collections.Generic;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
{
    public interface IUpdateScriptsProvider
    {
        List<UpdateScriptInfo> GetScriptsToValidate(string databaseName);

        List<UpdateScriptInfo> GetScriptsToApply(string databaseName);
    }
}