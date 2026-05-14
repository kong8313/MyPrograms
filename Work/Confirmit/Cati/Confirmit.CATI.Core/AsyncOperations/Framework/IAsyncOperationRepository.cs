using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationRepository
    {
        BvAsyncOperationQueueEntity Get(int id);
        IEnumerable<BvAsyncOperationQueueEntity> GetAll();
        void Insert(BvAsyncOperationQueueEntity entity);
        void Update(BvAsyncOperationQueueEntity entity);
        void Delete(int id);
        void Clean(TimeSpan expirationPeriod);
    }
}