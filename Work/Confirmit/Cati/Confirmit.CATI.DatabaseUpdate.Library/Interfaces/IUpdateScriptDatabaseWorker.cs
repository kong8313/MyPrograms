namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
{
    public interface IUpdateScriptDatabaseWorker
    {
        UpdateScriptInfo[] GetAppliedUpdateScriptInfos(string databaseName);

        void AddAppliedUpdateScriptInfo(string databaseName, UpdateScriptInfo updateScriptInfo);
    }
}