using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    /// <summary>
    /// Repesents repository for BvPersonDeferredMonitoringEntity entity.
    /// </summary>
    public class DeferredMonitoringRepository : IDeferredMonitoringRepository
    {
        /// <summary>
        /// Gets paged list of deferred monitoring records.
        /// </summary>
        /// <param name="personLogin">CATI interviewer login name.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="timezoneId">Timezone ID of dates in search conditions.</param>
        /// <param name="totalCount">Returns total count of records in database.</param>
        /// <returns>List of BvSpGetDeferredMonitoringListPageEntity entities.</returns>
        public List<BvSpGetDeferredMonitoringListPageEntity> GetPages(string personLogin, PagingArgs pagingArgs, int timezoneId, out int totalCount)
        {
            return BvSpGetDeferredMonitoringListPageAdapter.ExecuteEntityList(
                pagingArgs.PageIndex,
                pagingArgs.PageSize,
                pagingArgs.SortField,
                pagingArgs.SortOrderAsc,
                personLogin,
                SearchManager.GetSqlCondition(pagingArgs.SearchParameters, timezoneId),
                out totalCount
            );
        }

        public BvPersonDeferredMonitoringEntity TryGetById(long deferredRecordId)
        {
            return BvPersonDeferredMonitoringAdapter.GetByCondition(
                "ID = @ID",
                new[] { new SqlParameter("@ID", deferredRecordId) }).SingleOrDefault();
        }

        public List<BvPersonDeferredMonitoringEntity> TryGetByInterviewId(long surveySid, long interviewId)
        {
            return BvPersonDeferredMonitoringAdapter.GetByCondition(
                "InterviewID = @InterviewID AND SurveySID = @SurveySID",
                new[] { new SqlParameter("@InterviewID", interviewId), new SqlParameter("@SurveySID", surveySid) }).ToList();
        }
        
        public List<BvPersonDeferredMonitoringEntity> GetAllSavedRecords()
        {
            return BvPersonDeferredMonitoringAdapter.GetByCondition("IsRetained = 1").ToList();
        }
        
        public void UpdateRecord(BvPersonDeferredMonitoringEntity entity)
        {
            BvPersonDeferredMonitoringAdapter.Update(entity);
        }
    }
}
