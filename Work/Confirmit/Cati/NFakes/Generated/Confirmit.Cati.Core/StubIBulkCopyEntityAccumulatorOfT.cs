using System;
using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy.Fakes
{
    public class StubIBulkCopyEntityAccumulator<T> : IBulkCopyEntityAccumulator<T> 
    {
        private IBulkCopyEntityAccumulator<T> _inner;

        public StubIBulkCopyEntityAccumulator()
        {
            _inner = null;
        }

        public IBulkCopyEntityAccumulator<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddEntityTDelegate(T entity);
        public AddEntityTDelegate AddEntityT;

        void IBulkCopyEntityAccumulator<T>.AddEntity(T entity)
        {

            if (AddEntityT != null)
            {
                AddEntityT(entity);
            } else if (_inner != null)
            {
                ((IBulkCopyEntityAccumulator<T>)_inner).AddEntity(entity);
            }
        }

        public delegate IEnumerable<T> GetAccumulatedEntitiesAndCleanAccumulatorDelegate();
        public GetAccumulatedEntitiesAndCleanAccumulatorDelegate GetAccumulatedEntitiesAndCleanAccumulator;

        IEnumerable<T> IBulkCopyEntityAccumulator<T>.GetAccumulatedEntitiesAndCleanAccumulator()
        {


            if (GetAccumulatedEntitiesAndCleanAccumulator != null)
            {
                return GetAccumulatedEntitiesAndCleanAccumulator();
            } else if (_inner != null)
            {
                return ((IBulkCopyEntityAccumulator<T>)_inner).GetAccumulatedEntitiesAndCleanAccumulator();
            }

            return default(IEnumerable<T>);
        }

    }
}