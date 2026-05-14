using System;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Batch.Interfaces;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch.Interfaces.Fakes
{
    public class StubIBatchUploader : IBatchUploader 
    {
        private IBatchUploader _inner;

        public StubIBatchUploader()
        {
            _inner = null;
        }

        public IBatchUploader Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void UploadFromDatabaseStringArrayOfSqlParameterDelegate(string sqlQuery, SqlParameter[] parametrs);
        public UploadFromDatabaseStringArrayOfSqlParameterDelegate UploadFromDatabaseStringArrayOfSqlParameter;

        void IBatchUploader.UploadFromDatabase(string sqlQuery, SqlParameter[] parametrs)
        {

            if (UploadFromDatabaseStringArrayOfSqlParameter != null)
            {
                UploadFromDatabaseStringArrayOfSqlParameter(sqlQuery, parametrs);
            } else if (_inner != null)
            {
                ((IBatchUploader)_inner).UploadFromDatabase(sqlQuery, parametrs);
            }
        }

        public delegate void UploadFromMemoryIEnumerableOfInt32Delegate(IEnumerable<int> items);
        public UploadFromMemoryIEnumerableOfInt32Delegate UploadFromMemoryIEnumerableOfInt32;

        void IBatchUploader.UploadFromMemory(IEnumerable<int> items)
        {

            if (UploadFromMemoryIEnumerableOfInt32 != null)
            {
                UploadFromMemoryIEnumerableOfInt32(items);
            } else if (_inner != null)
            {
                ((IBatchUploader)_inner).UploadFromMemory(items);
            }
        }

    }
}