namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IConfirmitCatiEngine
    {
        string GetConfirmParameterValue(string confirmDatabaseName, IDatabaseEngine databaseEngine, string parameterName);

        string GetSchemeAndHostFromUrl(string url);
    }
}