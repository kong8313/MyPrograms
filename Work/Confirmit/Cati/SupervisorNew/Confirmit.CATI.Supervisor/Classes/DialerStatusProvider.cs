using Confirmit.CATI.Common;
using Confirmit.CATI.Core.SupervisorService;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class DialerStatusProvider : IDialerStatusProvider
    {
        private readonly ISupervisorServiceClient _supervisorServiceClient;

        public DialerStatusProvider(ISupervisorServiceClient supervisorServiceClient)
        {
            _supervisorServiceClient = supervisorServiceClient;
        }

        public DialerStatus GetDialerStatus(int dialerId, bool isActivated)
        {
            var isConnected = _supervisorServiceClient.IsDialerOperational(dialerId);

            if (!isConnected)
            {
                return DialerStatus.DisconnectedAndDeactivated;
            }

            return isActivated
                ? DialerStatus.ConnectedAndActivated
                : DialerStatus.ConnectedAndDeactivated;
        }

        public DialerStatus GetDialerActualStatus(int dialerId, bool isActivated, bool withReconnection, int expectedStatus)
        {
            var status = GetDialerStatus(dialerId, isActivated);

            if (status == DialerStatus.DisconnectedAndDeactivated && withReconnection)
            {
                switch ((DialerStatus)expectedStatus)
                {
                    case DialerStatus.ConnectedAndDeactivated:
                        return DialerStatus.DisconnectedTryingToConnect;
                    case DialerStatus.ConnectedAndActivated:
                        return DialerStatus.DisconnectedTryingToConnectAndActivate;
                }
            }
            return status;
        }

    }
}