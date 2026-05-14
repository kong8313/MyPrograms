using System;
using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using System.Collections.Generic;
using System.Data;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy.Fakes
{
    public class StubIBulkCopyEntitySerializer<T> : IBulkCopyEntitySerializer<T> 
    {
        private IBulkCopyEntitySerializer<T> _inner;

        public StubIBulkCopyEntitySerializer()
        {
            _inner = null;
        }

        public IBulkCopyEntitySerializer<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DataTable SerializeIEnumerableOfTDelegate(IEnumerable<T> entities);
        public SerializeIEnumerableOfTDelegate SerializeIEnumerableOfT;

        DataTable IBulkCopyEntitySerializer<T>.Serialize(IEnumerable<T> entities)
        {


            if (SerializeIEnumerableOfT != null)
            {
                return SerializeIEnumerableOfT(entities);
            } else if (_inner != null)
            {
                return ((IBulkCopyEntitySerializer<T>)_inner).Serialize(entities);
            }

            return default(DataTable);
        }

    }
}