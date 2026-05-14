using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public interface IReplicationIndexService
    {
        string GetColumnIndexName(string columnName);
        string GetQuotaIndexName(int quotaId);
        void CreateNonClusteredIndex(ReplicationSchemaIndex index);
        void ChangeOrderOfIndexColumns(int surveySid, int quotaId, string[] firstIndexColumns);
        IEnumerable<IndexedColumnInfo> GetIndexFields(string tableName, int quotaId);
        void AddClusteredIndex(string tableName, string columnName);
        string GetNameOfRespondentUpdateTrigger(string tableName);
        string GetBodyOfRespondentUpdateTrigger(int surveySid);
    }
}
