using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary
{
    public class ProblemStateSetter : IProblemStateSetter
    {
        public void SetProblemState(long agentId, DialerErrorCode result)
        {
            BvSpTasks_UpdateProblemStateAdapter.ExecuteNonQuery(
                (int)agentId,
                (int)result);
            EventDetailsScope.Current.AddTiming("BvSpTasks_UpdateProblemStateAdapter");
        }
    }
}