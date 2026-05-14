using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IDeferredMonitoringRepository
    {
        /// <summary>
        /// Gets paged list of deferred monitoring records.
        /// </summary>
        /// <param name="personLogin">CATI interviewer login name.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="timezoneId">Timezone ID of dates in search conditions.</param>
        /// <param name="totalCount">Returns total count of records in database.</param>
        /// <returns>List of BvSpGetDeferredMonitoringListPageEntity entities.</returns>
        List<BvSpGetDeferredMonitoringListPageEntity> GetPages(string personLogin, PagingArgs pagingArgs, int timezoneId, out int totalCount);

        BvPersonDeferredMonitoringEntity TryGetById(long deferredRecordId);

        List<BvPersonDeferredMonitoringEntity> TryGetByInterviewId(long surveySid, long interviewId);

        List<BvPersonDeferredMonitoringEntity> GetAllSavedRecords();
        
        void UpdateRecord(BvPersonDeferredMonitoringEntity entity);
    }
}