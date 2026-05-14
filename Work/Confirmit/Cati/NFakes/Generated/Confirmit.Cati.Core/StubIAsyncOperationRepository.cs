using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationRepository : IAsyncOperationRepository 
    {
        private IAsyncOperationRepository _inner;

        public StubIAsyncOperationRepository()
        {
            _inner = null;
        }

        public IAsyncOperationRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvAsyncOperationQueueEntity GetInt32Delegate(int id);
        public GetInt32Delegate GetInt32;

        BvAsyncOperationQueueEntity IAsyncOperationRepository.Get(int id)
        {


            if (GetInt32 != null)
            {
                return GetInt32(id);
            } else if (_inner != null)
            {
                return ((IAsyncOperationRepository)_inner).Get(id);
            }

            return default(BvAsyncOperationQueueEntity);
        }

        public delegate IEnumerable<BvAsyncOperationQueueEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        IEnumerable<BvAsyncOperationQueueEntity> IAsyncOperationRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IAsyncOperationRepository)_inner).GetAll();
            }

            return default(IEnumerable<BvAsyncOperationQueueEntity>);
        }

        public delegate void InsertBvAsyncOperationQueueEntityDelegate(BvAsyncOperationQueueEntity entity);
        public InsertBvAsyncOperationQueueEntityDelegate InsertBvAsyncOperationQueueEntity;

        void IAsyncOperationRepository.Insert(BvAsyncOperationQueueEntity entity)
        {

            if (InsertBvAsyncOperationQueueEntity != null)
            {
                InsertBvAsyncOperationQueueEntity(entity);
            } else if (_inner != null)
            {
                ((IAsyncOperationRepository)_inner).Insert(entity);
            }
        }

        public delegate void UpdateBvAsyncOperationQueueEntityDelegate(BvAsyncOperationQueueEntity entity);
        public UpdateBvAsyncOperationQueueEntityDelegate UpdateBvAsyncOperationQueueEntity;

        void IAsyncOperationRepository.Update(BvAsyncOperationQueueEntity entity)
        {

            if (UpdateBvAsyncOperationQueueEntity != null)
            {
                UpdateBvAsyncOperationQueueEntity(entity);
            } else if (_inner != null)
            {
                ((IAsyncOperationRepository)_inner).Update(entity);
            }
        }

        public delegate void DeleteInt32Delegate(int id);
        public DeleteInt32Delegate DeleteInt32;

        void IAsyncOperationRepository.Delete(int id)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(id);
            } else if (_inner != null)
            {
                ((IAsyncOperationRepository)_inner).Delete(id);
            }
        }

        public delegate void CleanTimeSpanDelegate(TimeSpan expirationPeriod);
        public CleanTimeSpanDelegate CleanTimeSpan;

        void IAsyncOperationRepository.Clean(TimeSpan expirationPeriod)
        {

            if (CleanTimeSpan != null)
            {
                CleanTimeSpan(expirationPeriod);
            } else if (_inner != null)
            {
                ((IAsyncOperationRepository)_inner).Clean(expirationPeriod);
            }
        }

    }
}