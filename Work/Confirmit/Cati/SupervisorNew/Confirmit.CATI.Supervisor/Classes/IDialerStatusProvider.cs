using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Classes
{
    public interface IDialerStatusProvider
    {
        DialerStatus GetDialerStatus(int dialerId, bool isActivated);
        DialerStatus GetDialerActualStatus(int dialerId, bool isActivated, bool withReconnection, int expectedStatus);

    }
}