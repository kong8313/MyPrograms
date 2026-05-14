using System;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes
{
    public class StubIReplicationIndexService : IReplicationIndexService 
    {
        private IReplicationIndexService _inner;

        public StubIReplicationIndexService()
        {
            _inner = null;
        }

        public IReplicationIndexService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetColumnIndexNameStringDelegate(string columnName);
        public GetColumnIndexNameStringDelegate GetColumnIndexNameString;

        string IReplicationIndexService.GetColumnIndexName(string columnName)
        {


            if (GetColumnIndexNameString != null)
            {
                return GetColumnIndexNameString(columnName);
            } else if (_inner != null)
            {
                return ((IReplicationIndexService)_inner).GetColumnIndexName(columnName);
            }

            return default(string);
        }

        public delegate string GetQuotaIndexNameInt32Delegate(int quotaId);
        public GetQuotaIndexNameInt32Delegate GetQuotaIndexNameInt32;

        string IReplicationIndexService.GetQuotaIndexName(int quotaId)
        {


            if (GetQuotaIndexNameInt32 != null)
            {
                return GetQuotaIndexNameInt32(quotaId);
            } else if (_inner != null)
            {
                return ((IReplicationIndexService)_inner).GetQuotaIndexName(quotaId);
            }

            return default(string);
        }

        public delegate void CreateNonClusteredIndexReplicationSchemaIndexDelegate(ReplicationSchemaIndex index);
        public CreateNonClusteredIndexReplicationSchemaIndexDelegate CreateNonClusteredIndexReplicationSchemaIndex;

        void IReplicationIndexService.CreateNonClusteredIndex(ReplicationSchemaIndex index)
        {

            if (CreateNonClusteredIndexReplicationSchemaIndex != null)
            {
                CreateNonClusteredIndexReplicationSchemaIndex(index);
            } else if (_inner != null)
            {
                ((IReplicationIndexService)_inner).CreateNonClusteredIndex(index);
            }
        }

        public delegate void ChangeOrderOfIndexColumnsInt32Int32ArrayOfStringDelegate(int surveySid, int quotaId, string[] firstIndexColumns);
        public ChangeOrderOfIndexColumnsInt32Int32ArrayOfStringDelegate ChangeOrderOfIndexColumnsInt32Int32ArrayOfString;

        void IReplicationIndexService.ChangeOrderOfIndexColumns(int surveySid, int quotaId, string[] firstIndexColumns)
        {

            if (ChangeOrderOfIndexColumnsInt32Int32ArrayOfString != null)
            {
                ChangeOrderOfIndexColumnsInt32Int32ArrayOfString(surveySid, quotaId, firstIndexColumns);
            } else if (_inner != null)
            {
                ((IReplicationIndexService)_inner).ChangeOrderOfIndexColumns(surveySid, quotaId, firstIndexColumns);
            }
        }

        public delegate IEnumerable<IndexedColumnInfo> GetIndexFieldsStringInt32Delegate(string tableName, int quotaId);
        public GetIndexFieldsStringInt32Delegate GetIndexFieldsStringInt32;

        IEnumerable<IndexedColumnInfo> IReplicationIndexService.GetIndexFields(string tableName, int quotaId)
        {


            if (GetIndexFieldsStringInt32 != null)
            {
                return GetIndexFieldsStringInt32(tableName, quotaId);
            } else if (_inner != null)
            {
                return ((IReplicationIndexService)_inner).GetIndexFields(tableName, quotaId);
            }

            return default(IEnumerable<IndexedColumnInfo>);
        }

        public delegate void AddClusteredIndexStringStringDelegate(string tableName, string columnName);
        public AddClusteredIndexStringStringDelegate AddClusteredIndexStringString;

        void IReplicationIndexService.AddClusteredIndex(string tableName, string columnName)
        {

            if (AddClusteredIndexStringString != null)
            {
                AddClusteredIndexStringString(tableName, columnName);
            } else if (_inner != null)
            {
                ((IReplicationIndexService)_inner).AddClusteredIndex(tableName, columnName);
            }
        }

        public delegate string GetNameOfRespondentUpdateTriggerStringDelegate(string tableName);
        public GetNameOfRespondentUpdateTriggerStringDelegate GetNameOfRespondentUpdateTriggerString;

        string IReplicationIndexService.GetNameOfRespondentUpdateTrigger(string tableName)
        {


            if (GetNameOfRespondentUpdateTriggerString != null)
            {
                return GetNameOfRespondentUpdateTriggerString(tableName);
            } else if (_inner != null)
            {
                return ((IReplicationIndexService)_inner).GetNameOfRespondentUpdateTrigger(tableName);
            }

            return default(string);
        }

        public delegate string GetBodyOfRespondentUpdateTriggerInt32Delegate(int surveySid);
        public GetBodyOfRespondentUpdateTriggerInt32Delegate GetBodyOfRespondentUpdateTriggerInt32;

        string IReplicationIndexService.GetBodyOfRespondentUpdateTrigger(int surveySid)
        {


            if (GetBodyOfRespondentUpdateTriggerInt32 != null)
            {
                return GetBodyOfRespondentUpdateTriggerInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IReplicationIndexService)_inner).GetBodyOfRespondentUpdateTrigger(surveySid);
            }

            return default(string);
        }

    }
}