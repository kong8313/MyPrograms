using System.Collections.Generic;
using Confirmit.Logging;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public static class DialerErrorSeverityProvider
    {
        private static readonly HashSet<DialerErrorCode> _warningCodes = new HashSet<DialerErrorCode>() {
            DialerErrorCode.Restarted, DialerErrorCode.UnknownAgent, DialerErrorCode.WrongAgentState,
            DialerErrorCode.AgentAlreadyLoggedIn, DialerErrorCode.UnknownSupervisor, DialerErrorCode.UnknownCampaign, DialerErrorCode.InvalidDialingMode,
            DialerErrorCode.InvalidExtension, DialerErrorCode.InvalidPhoneNumber, DialerErrorCode.PhoneNumberAlreadyInUse, DialerErrorCode.ResourceAlreadyInUse,
            DialerErrorCode.ResourceNotFound, DialerErrorCode.AgentIsNotLoggedin, DialerErrorCode.AgentAlreadyBeingMonitored, DialerErrorCode.MonitoringIsAlreadyStarted
        };

        public static bool IsWarning(DialerErrorCode code)
        {
            return _warningCodes.Contains(code);
        }
    }
}