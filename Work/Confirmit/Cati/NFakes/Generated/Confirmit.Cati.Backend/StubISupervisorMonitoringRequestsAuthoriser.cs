using System;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService;
using Confirmit.CATI.Common.Monitoring;

namespace Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Fakes
{
    public class StubISupervisorMonitoringRequestsAuthoriser : ISupervisorMonitoringRequestsAuthoriser 
    {
        private ISupervisorMonitoringRequestsAuthoriser _inner;

        public StubISupervisorMonitoringRequestsAuthoriser()
        {
            _inner = null;
        }

        public ISupervisorMonitoringRequestsAuthoriser Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate MonitoringIdentityInfo AuthoriseRequestBooleanDelegate(bool validateLaunchFileCreationTime);
        public AuthoriseRequestBooleanDelegate AuthoriseRequestBoolean;

        MonitoringIdentityInfo ISupervisorMonitoringRequestsAuthoriser.AuthoriseRequest(bool validateLaunchFileCreationTime)
        {


            if (AuthoriseRequestBoolean != null)
            {
                return AuthoriseRequestBoolean(validateLaunchFileCreationTime);
            } else if (_inner != null)
            {
                return ((ISupervisorMonitoringRequestsAuthoriser)_inner).AuthoriseRequest(validateLaunchFileCreationTime);
            }

            return default(MonitoringIdentityInfo);
        }

    }
}