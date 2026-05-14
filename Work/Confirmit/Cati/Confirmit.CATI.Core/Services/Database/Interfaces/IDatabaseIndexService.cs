using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Database.Interfaces
{
    public class IndexInfo
    {
        public string TableName;
        public string IndexName;
        public bool ContainsLob;
        public int FillFactor;
        public long RowCount;
        public double Fragmentation;
        public long PageCount;
    }

    public interface IDatabaseIndexService
    {
        IEnumerable<IndexInfo> GetAllIndexes(string fragmentationDetectMode);

        IndexInfo GetIndex(string tableName, string indexName, string fragmentationDetectMode);
        
        void ReorginizeIndex(string tableName, string indexName);

        bool IsRebuildIndexOnlineSupported();

        void RebuildIndex(string tableName, string indexName, bool containsLob);
        void RebuildIndexOffline(string tableName, string indexName);
        void RebuildIndexOnline(string tableName, string indexName);
    }
}
