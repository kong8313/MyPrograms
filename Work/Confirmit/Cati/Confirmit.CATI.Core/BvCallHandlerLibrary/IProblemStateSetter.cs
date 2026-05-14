using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary
{
    public interface IProblemStateSetter
    {
        void SetProblemState(long agentId, DialerErrorCode result);
    }
}