using System;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Database.Interfaces.Fakes
{
    public class StubIDatabaseIndexService : IDatabaseIndexService 
    {
        private IDatabaseIndexService _inner;

        public StubIDatabaseIndexService()
        {
            _inner = null;
        }

        public IDatabaseIndexService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<IndexInfo> GetAllIndexesStringDelegate(string fragmentationDetectMode);
        public GetAllIndexesStringDelegate GetAllIndexesString;

        IEnumerable<IndexInfo> IDatabaseIndexService.GetAllIndexes(string fragmentationDetectMode)
        {


            if (GetAllIndexesString != null)
            {
                return GetAllIndexesString(fragmentationDetectMode);
            } else if (_inner != null)
            {
                return ((IDatabaseIndexService)_inner).GetAllIndexes(fragmentationDetectMode);
            }

            return default(IEnumerable<IndexInfo>);
        }

        public delegate IndexInfo GetIndexStringStringStringDelegate(string tableName, string indexName, string fragmentationDetectMode);
        public GetIndexStringStringStringDelegate GetIndexStringStringString;

        IndexInfo IDatabaseIndexService.GetIndex(string tableName, string indexName, string fragmentationDetectMode)
        {


            if (GetIndexStringStringString != null)
            {
                return GetIndexStringStringString(tableName, indexName, fragmentationDetectMode);
            } else if (_inner != null)
            {
                return ((IDatabaseIndexService)_inner).GetIndex(tableName, indexName, fragmentationDetectMode);
            }

            return default(IndexInfo);
        }

        public delegate void ReorginizeIndexStringStringDelegate(string tableName, string indexName);
        public ReorginizeIndexStringStringDelegate ReorginizeIndexStringString;

        void IDatabaseIndexService.ReorginizeIndex(string tableName, string indexName)
        {

            if (ReorginizeIndexStringString != null)
            {
                ReorginizeIndexStringString(tableName, indexName);
            } else if (_inner != null)
            {
                ((IDatabaseIndexService)_inner).ReorginizeIndex(tableName, indexName);
            }
        }

        public delegate bool IsRebuildIndexOnlineSupportedDelegate();
        public IsRebuildIndexOnlineSupportedDelegate IsRebuildIndexOnlineSupported;

        bool IDatabaseIndexService.IsRebuildIndexOnlineSupported()
        {


            if (IsRebuildIndexOnlineSupported != null)
            {
                return IsRebuildIndexOnlineSupported();
            } else if (_inner != null)
            {
                return ((IDatabaseIndexService)_inner).IsRebuildIndexOnlineSupported();
            }

            return default(bool);
        }

        public delegate void RebuildIndexStringStringBooleanDelegate(string tableName, string indexName, bool containsLob);
        public RebuildIndexStringStringBooleanDelegate RebuildIndexStringStringBoolean;

        void IDatabaseIndexService.RebuildIndex(string tableName, string indexName, bool containsLob)
        {

            if (RebuildIndexStringStringBoolean != null)
            {
                RebuildIndexStringStringBoolean(tableName, indexName, containsLob);
            } else if (_inner != null)
            {
                ((IDatabaseIndexService)_inner).RebuildIndex(tableName, indexName, containsLob);
            }
        }

        public delegate void RebuildIndexOfflineStringStringDelegate(string tableName, string indexName);
        public RebuildIndexOfflineStringStringDelegate RebuildIndexOfflineStringString;

        void IDatabaseIndexService.RebuildIndexOffline(string tableName, string indexName)
        {

            if (RebuildIndexOfflineStringString != null)
            {
                RebuildIndexOfflineStringString(tableName, indexName);
            } else if (_inner != null)
            {
                ((IDatabaseIndexService)_inner).RebuildIndexOffline(tableName, indexName);
            }
        }

        public delegate void RebuildIndexOnlineStringStringDelegate(string tableName, string indexName);
        public RebuildIndexOnlineStringStringDelegate RebuildIndexOnlineStringString;

        void IDatabaseIndexService.RebuildIndexOnline(string tableName, string indexName)
        {

            if (RebuildIndexOnlineStringString != null)
            {
                RebuildIndexOnlineStringString(tableName, indexName);
            } else if (_inner != null)
            {
                ((IDatabaseIndexService)_inner).RebuildIndexOnline(tableName, indexName);
            }
        }

    }
}