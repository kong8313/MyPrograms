using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Supervisor.Reports
{
    public interface IInboundHandlerOperationsProvider
    {
        List<InboundHandlerOperation> GetAll();
    }
}
