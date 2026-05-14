using System;
using System.Security.Principal;
using System.Threading;
using Confirmit.CATI.Core.WcfServices.MessageHeaders;

namespace Confirmit.CATI.Core.Services
{
    public class ThreadIdentityService
    {
        public void SetPrincipalForIncomingWcfRequest()
        {
            var name = SupervisorMessageHeaderInspector.GetIncomingMessageSupervisor();
            if (!string.IsNullOrEmpty(name))
            {
                SetPrincipal(name);
            }
        }

        public void ResetPrincipal()
        {
            SetPrincipal(String.Empty);
        }

        public void SetPrincipal(string name)
        {
            if (name != null)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(name), new string[0]);
            }
        }
    }
}