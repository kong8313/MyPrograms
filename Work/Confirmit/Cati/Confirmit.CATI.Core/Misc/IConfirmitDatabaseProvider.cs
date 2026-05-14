namespace Confirmit.CATI.Core.Misc
{
    public interface IConfirmitDatabaseProvider
    {
        string GetSurveyDatabaseName(string projectId);

        string GetSqlServerName(string projectId, bool updateLastConnectionTime = true);

        string GetSchemaName(string projectId);
    }
}
