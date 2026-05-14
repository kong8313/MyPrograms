using System;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.ManagementService.Fakes
{
    public class StubIInstanceManagementService : IInstanceManagementService 
    {
        private IInstanceManagementService _inner;

        public StubIInstanceManagementService()
        {
            _inner = null;
        }

        public IInstanceManagementService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string RegisterSchedulingServiceInstanceStringDelegate(string instanceName);
        public RegisterSchedulingServiceInstanceStringDelegate RegisterSchedulingServiceInstanceString;

        string IInstanceManagementService.RegisterSchedulingServiceInstance(string instanceName)
        {


            if (RegisterSchedulingServiceInstanceString != null)
            {
                return RegisterSchedulingServiceInstanceString(instanceName);
            } else if (_inner != null)
            {
                return ((IInstanceManagementService)_inner).RegisterSchedulingServiceInstance(instanceName);
            }

            return default(string);
        }

        public delegate void UnregisterSchedulingServiceInstanceStringDelegate(string instanceName);
        public UnregisterSchedulingServiceInstanceStringDelegate UnregisterSchedulingServiceInstanceString;

        void IInstanceManagementService.UnregisterSchedulingServiceInstance(string instanceName)
        {

            if (UnregisterSchedulingServiceInstanceString != null)
            {
                UnregisterSchedulingServiceInstanceString(instanceName);
            } else if (_inner != null)
            {
                ((IInstanceManagementService)_inner).UnregisterSchedulingServiceInstance(instanceName);
            }
        }

    }
}