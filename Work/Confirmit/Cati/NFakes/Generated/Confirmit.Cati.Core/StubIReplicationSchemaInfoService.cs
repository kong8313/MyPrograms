using System;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes
{
    public class StubIReplicationSchemaInfoService : IReplicationSchemaInfoService 
    {
        private IReplicationSchemaInfoService _inner;

        public StubIReplicationSchemaInfoService()
        {
            _inner = null;
        }

        public IReplicationSchemaInfoService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetDestinationTableNameInt32Delegate(int surveySid);
        public GetDestinationTableNameInt32Delegate GetDestinationTableNameInt32;

        string IReplicationSchemaInfoService.GetDestinationTableName(int surveySid)
        {


            if (GetDestinationTableNameInt32 != null)
            {
                return GetDestinationTableNameInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IReplicationSchemaInfoService)_inner).GetDestinationTableName(surveySid);
            }

            return default(string);
        }

        public delegate void CreateCopyOfTableWithoutDataAndIndexesStringStringArrayOfStringOutDelegate(string oldTableName, string newTableName, out string[] indexQueries);
        public CreateCopyOfTableWithoutDataAndIndexesStringStringArrayOfStringOutDelegate CreateCopyOfTableWithoutDataAndIndexesStringStringArrayOfStringOut;

        void IReplicationSchemaInfoService.CreateCopyOfTableWithoutDataAndIndexes(string oldTableName, string newTableName, out string[] indexQueries)
        {
            indexQueries = default(string[]);

            if (CreateCopyOfTableWithoutDataAndIndexesStringStringArrayOfStringOut != null)
            {
                CreateCopyOfTableWithoutDataAndIndexesStringStringArrayOfStringOut(oldTableName, newTableName, out indexQueries);
            } else if (_inner != null)
            {
                ((IReplicationSchemaInfoService)_inner).CreateCopyOfTableWithoutDataAndIndexes(oldTableName, newTableName, out indexQueries);
            }
        }

    }
}