using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationRepository : IAsyncOperationRepository
    {
        private readonly ITimeService _timeService;

        public AsyncOperationRepository(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public BvAsyncOperationQueueEntity Get(int id)
        {
            return BvAsyncOperationQueueAdapter.GetByCondition(
                "[id] = @id\r\n",
                new SqlParameter("@id", id)).FirstOrDefault();
        }

        public void Insert(BvAsyncOperationQueueEntity entity)
        {
            using (var connection = new ConnectionScope())
            {
                BvAsyncOperationQueueAdapter.Insert(entity);

                var engine = new DatabaseEngine();
                entity.Id = engine.ExecuteScalar<int>("SELECT CAST( @@IDENTITY AS INT)", CommandType.Text);
            }
        }

        public void Update(BvAsyncOperationQueueEntity entity)
        {
            BvAsyncOperationQueueAdapter.Update(entity);
        }

        public void Delete(int id)
        {
            BvAsyncOperationQueueAdapter.DeleteByCondition(
                "[id] = @id\r\n",
                new SqlParameter("@id", id));
        }

        public void Clean(TimeSpan expirationPeriod)
        {
            var epxirationDate = _timeService.GetUtcNow() - expirationPeriod;

            BvSpAsyncOperationQueue_CleanupAdapter.ExecuteNonQuery((int)AsyncOperationState.Completed, epxirationDate);
            BvSpAsyncOperationQueue_CleanupAdapter.ExecuteNonQuery((int)AsyncOperationState.PartiallyCompleted, epxirationDate);
            BvSpAsyncOperationQueue_CleanupAdapter.ExecuteNonQuery((int)AsyncOperationState.Aborted, epxirationDate);
            BvSpAsyncOperationQueue_CleanupAdapter.ExecuteNonQuery((int)AsyncOperationState.Failed, epxirationDate);
            BvSpAsyncOperationQueue_CleanupAdapter.ExecuteNonQuery((int)AsyncOperationState.Hanged, epxirationDate);
        }

        public IEnumerable<BvAsyncOperationQueueEntity> GetAll()
        {
            return BvAsyncOperationQueueAdapter.GetAll();
        }

        public static List<BvSpAsyncOperations_ListPageEntity> GetPage(int? callCenterId,
                                                                      PagingArgs pagingArgs,
                                                                      int timezoneId,
                                                                      string userName, out int totalCount)
        {
            return BvSpAsyncOperations_ListPageAdapter.ExecuteEntityList(callCenterId,
                                                                         pagingArgs.PageIndex,
                                                                         pagingArgs.PageSize,
                                                                         pagingArgs.SortField,
                                                                         pagingArgs.SortOrderAsc ? 1 : 0,
                                                                         userName,
                                                                         SearchManager.GetSqlCondition(pagingArgs.SearchParameters, timezoneId),
                                                                         out totalCount);            
        }
    }
}