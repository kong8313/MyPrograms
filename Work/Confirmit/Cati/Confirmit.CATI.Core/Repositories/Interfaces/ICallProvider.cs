using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ICallProvider
    {
        BvCallEntity GetCallAndNoLock(int surveySid, int interviewId);
        BvCallEntity GetCallAndNoLock(int surveySid, int interviewId, int batchId, bool isSampleUpdateMode);        
    }
}