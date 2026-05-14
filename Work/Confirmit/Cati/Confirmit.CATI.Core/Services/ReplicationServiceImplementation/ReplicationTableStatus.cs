using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    internal enum ReplicationTableStatus
    {
        None = 0,
        Update = 1,
        Reinitialize = 2
    }
}
