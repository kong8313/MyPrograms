using System;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Core.Telephony.Connection;

namespace Confirmit.CATI.Core.Telephony.Connection.Fakes
{
    public class StubIDialerConnectionStateProvider : IDialerConnectionStateProvider 
    {
        private IDialerConnectionStateProvider _inner;

        public StubIDialerConnectionStateProvider()
        {
            _inner = null;
        }

        public IDialerConnectionStateProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerConnectionState GetCurrentConnectionStateForBackgroundHealthCheckStringInt32IDialerAPIDelegate(string tenantId, int dialerId, IDialerAPI dialerApi);
        public GetCurrentConnectionStateForBackgroundHealthCheckStringInt32IDialerAPIDelegate GetCurrentConnectionStateForBackgroundHealthCheckStringInt32IDialerAPI;

        DialerConnectionState IDialerConnectionStateProvider.GetCurrentConnectionStateForBackgroundHealthCheck(string tenantId, int dialerId, IDialerAPI dialerApi)
        {


            if (GetCurrentConnectionStateForBackgroundHealthCheckStringInt32IDialerAPI != null)
            {
                return GetCurrentConnectionStateForBackgroundHealthCheckStringInt32IDialerAPI(tenantId, dialerId, dialerApi);
            } else if (_inner != null)
            {
                return ((IDialerConnectionStateProvider)_inner).GetCurrentConnectionStateForBackgroundHealthCheck(tenantId, dialerId, dialerApi);
            }

            return default(DialerConnectionState);
        }

        public delegate DialerConnectionState GetCurrentConnectionStateWhenActivatingDialerStringInt32IDialerAPIDelegate(string tenantId, int dialerId, IDialerAPI dialerApi);
        public GetCurrentConnectionStateWhenActivatingDialerStringInt32IDialerAPIDelegate GetCurrentConnectionStateWhenActivatingDialerStringInt32IDialerAPI;

        DialerConnectionState IDialerConnectionStateProvider.GetCurrentConnectionStateWhenActivatingDialer(string tenantId, int dialerId, IDialerAPI dialerApi)
        {


            if (GetCurrentConnectionStateWhenActivatingDialerStringInt32IDialerAPI != null)
            {
                return GetCurrentConnectionStateWhenActivatingDialerStringInt32IDialerAPI(tenantId, dialerId, dialerApi);
            } else if (_inner != null)
            {
                return ((IDialerConnectionStateProvider)_inner).GetCurrentConnectionStateWhenActivatingDialer(tenantId, dialerId, dialerApi);
            }

            return default(DialerConnectionState);
        }

    }
}