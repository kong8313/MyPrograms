using System;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Common.ServiceLocation.Fakes
{
    public class StubIServiceLocatorRegistry : IServiceLocatorRegistry 
    {
        private IServiceLocatorRegistry _inner;

        public StubIServiceLocatorRegistry()
        {
            _inner = null;
        }

        public IServiceLocatorRegistry Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void RegisterTypesIServiceRegistratorDelegate(IServiceRegistrator serviceRegistrator);
        public RegisterTypesIServiceRegistratorDelegate RegisterTypesIServiceRegistrator;

        void IServiceLocatorRegistry.RegisterTypes(IServiceRegistrator serviceRegistrator)
        {

            if (RegisterTypesIServiceRegistrator != null)
            {
                RegisterTypesIServiceRegistrator(serviceRegistrator);
            } else if (_inner != null)
            {
                ((IServiceLocatorRegistry)_inner).RegisterTypes(serviceRegistrator);
            }
        }

    }
}