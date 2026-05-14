namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IResources
    {
        UpdateScriptInfo[] UpdateScriptInfos { get; }

        string BaseCreationScript { get; }
        string NewCompanyUpdateScript { get; }
    }
}