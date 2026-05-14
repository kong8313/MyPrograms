using System;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.PersonLogin;

namespace Confirmit.CATI.Core.PersonLogin.Fakes
{
    public class StubILicenseService : ILicenseService 
    {
        private ILicenseService _inner;

        public StubILicenseService()
        {
            _inner = null;
        }

        public ILicenseService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckLicenseAgentTypeDelegate(AgentType agentType);
        public CheckLicenseAgentTypeDelegate CheckLicenseAgentType;

        void ILicenseService.CheckLicense(AgentType agentType)
        {

            if (CheckLicenseAgentType != null)
            {
                CheckLicenseAgentType(agentType);
            } else if (_inner != null)
            {
                ((ILicenseService)_inner).CheckLicense(agentType);
            }
        }

    }
}