using System;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIStartedServicesRepository : IStartedServicesRepository 
    {
        private IStartedServicesRepository _inner;

        public StubIStartedServicesRepository()
        {
            _inner = null;
        }

        public IStartedServicesRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddStartedServiceInfoStringStringDelegate(string machineName, string serviceName);
        public AddStartedServiceInfoStringStringDelegate AddStartedServiceInfoStringString;

        void IStartedServicesRepository.AddStartedServiceInfo(string machineName, string serviceName)
        {

            if (AddStartedServiceInfoStringString != null)
            {
                AddStartedServiceInfoStringString(machineName, serviceName);
            } else if (_inner != null)
            {
                ((IStartedServicesRepository)_inner).AddStartedServiceInfo(machineName, serviceName);
            }
        }

        public delegate void RemoveStartedServiceInfoStringStringDelegate(string machineName, string serviceName);
        public RemoveStartedServiceInfoStringStringDelegate RemoveStartedServiceInfoStringString;

        void IStartedServicesRepository.RemoveStartedServiceInfo(string machineName, string serviceName)
        {

            if (RemoveStartedServiceInfoStringString != null)
            {
                RemoveStartedServiceInfoStringString(machineName, serviceName);
            } else if (_inner != null)
            {
                ((IStartedServicesRepository)_inner).RemoveStartedServiceInfo(machineName, serviceName);
            }
        }

    }
}