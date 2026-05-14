using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerLoginLogoutManager
    {
        DialerErrorCode Logout(int dialerId, long campaignId, bool isPredictive, int agentId);
    }
}