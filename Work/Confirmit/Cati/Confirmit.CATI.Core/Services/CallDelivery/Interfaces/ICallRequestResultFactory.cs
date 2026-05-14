using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.CallDelivery.Interfaces
{
    public interface ICallRequestResultFactory
    {
        CallRequestResult Create(ILookupCallEntity call);
    }
}
