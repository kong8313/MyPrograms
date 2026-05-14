using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Repositories
{
    public class CallDatabaseProvider : ICallProvider
    {
        public BvCallEntity GetCallAndNoLock(int surveySid, int interviewId)
        {
            return CallQueueService.GetCallAndNoLock(surveySid, interviewId);
        }

        public BvCallEntity GetCallAndNoLock(int surveySid, int interviewId, int batchId, bool isSampleUpdateMode)
        {
            return CallQueueService.GetCallAndNoLock(surveySid, interviewId, batchId, isSampleUpdateMode);
        }
    }
}