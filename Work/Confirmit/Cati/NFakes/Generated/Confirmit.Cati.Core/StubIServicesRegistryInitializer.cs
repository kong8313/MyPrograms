using System;
using Confirmit.CATI.Core.ServiceRegistration;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.ServiceRegistration.Fakes
{
    public class StubIServicesRegistryInitializer : IServicesRegistryInitializer 
    {
        private IServicesRegistryInitializer _inner;

        public StubIServicesRegistryInitializer()
        {
            _inner = null;
        }

        public IServicesRegistryInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<IServiceLocatorRegistry> GetRegistriesDelegate();
        public GetRegistriesDelegate GetRegistries;

        IEnumerable<IServiceLocatorRegistry> IServicesRegistryInitializer.GetRegistries()
        {


            if (GetRegistries != null)
            {
                return GetRegistries();
            } else if (_inner != null)
            {
                return ((IServicesRegistryInitializer)_inner).GetRegistries();
            }

            return default(IEnumerable<IServiceLocatorRegistry>);
        }

        public delegate void RegisterRegistriesIEnumerableOfIServiceLocatorRegistryDelegate(IEnumerable<IServiceLocatorRegistry> registries);
        public RegisterRegistriesIEnumerableOfIServiceLocatorRegistryDelegate RegisterRegistriesIEnumerableOfIServiceLocatorRegistry;

        void IServicesRegistryInitializer.RegisterRegistries(IEnumerable<IServiceLocatorRegistry> registries)
        {

            if (RegisterRegistriesIEnumerableOfIServiceLocatorRegistry != null)
            {
                RegisterRegistriesIEnumerableOfIServiceLocatorRegistry(registries);
            } else if (_inner != null)
            {
                ((IServicesRegistryInitializer)_inner).RegisterRegistries(registries);
            }
        }

    }
}