using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIServiceDiscoveryClientProxy : IServiceDiscoveryClientProxy 
    {
        private IServiceDiscoveryClientProxy _inner;

        public StubIServiceDiscoveryClientProxy()
        {
            _inner = null;
        }

        public IServiceDiscoveryClientProxy Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Uri GetServiceStringDelegate(string serviceId);
        public GetServiceStringDelegate GetServiceString;

        Uri IServiceDiscoveryClientProxy.GetService(string serviceId)
        {


            if (GetServiceString != null)
            {
                return GetServiceString(serviceId);
            } else if (_inner != null)
            {
                return ((IServiceDiscoveryClientProxy)_inner).GetService(serviceId);
            }

            return default(Uri);
        }

    }
}