using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Misc.ConfirmitClientKey;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class SupervisorConfirmitClientKeyProvider : IConfirmitClientKeyProvider
    {
        public string Get()
        {
            return ((SupervisorPrincipal) HttpContext.Current.User).ClientKey;
        }
    }
}