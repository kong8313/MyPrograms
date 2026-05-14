namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IUpdateScriptDatabaseWorker
    {
        UpdateScriptInfo[] GetAppliedUpdateScriptInfos(string databaseName);

        void AddAppliedUpdateScriptInfo(string databaseName, UpdateScriptInfo updateScriptInfo);
    }
}