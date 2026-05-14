namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public interface IReplicationSchemaService
    {
        void UpdateSurveyReplicationScheme(int surveySid, TableInfo[] tables);
        bool IsReplicationSchemaChanged(int surveySid, TableInfo[] newTablesInfo);
        void UpdateQuotaBalancingConfiguration(int surveySid, TableInfo[] tables);
    }

    public interface IReplicationSchemaInfoService
    {
        string GetDestinationTableName(int surveySid);

        void CreateCopyOfTableWithoutDataAndIndexes(string oldTableName, string newTableName, out string[] indexQueries);
    }
}