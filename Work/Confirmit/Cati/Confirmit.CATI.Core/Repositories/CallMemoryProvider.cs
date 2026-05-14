using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class CallMemoryProvider : ICallProvider
    {
        private readonly BvCallEntity m_Call;

        public CallMemoryProvider( BvCallEntity mCall )
        {
            m_Call = mCall;
        }

        public BvCallEntity GetCallAndNoLock(int surveySid, int interviewId)
        {
            return m_Call;
        }

        public BvCallEntity GetCallAndNoLock(int surveySid, int interviewId, int batchId, bool isSampleUpdateMode)
        {
            return m_Call;
        }
    }
}