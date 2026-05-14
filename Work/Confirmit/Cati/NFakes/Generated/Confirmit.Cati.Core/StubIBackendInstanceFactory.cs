using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIBackendInstanceFactory : IBackendInstanceFactory 
    {
        private IBackendInstanceFactory _inner;

        public StubIBackendInstanceFactory()
        {
            _inner = null;
        }

        public IBackendInstanceFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BackendInstance CreateInt32HostTypeDelegate(int companyId, HostType hostType);
        public CreateInt32HostTypeDelegate CreateInt32HostType;

        BackendInstance IBackendInstanceFactory.Create(int companyId, HostType hostType)
        {


            if (CreateInt32HostType != null)
            {
                return CreateInt32HostType(companyId, hostType);
            } else if (_inner != null)
            {
                return ((IBackendInstanceFactory)_inner).Create(companyId, hostType);
            }

            return default(BackendInstance);
        }

    }
}