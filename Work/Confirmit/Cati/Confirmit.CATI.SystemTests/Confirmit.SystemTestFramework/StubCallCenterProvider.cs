using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.SystemTestFramework
{
    public class StubICallCenterProvider : ICallCenterProvider
    {
        public int GetCurrentId()
        {
            return 0;
        }

        public BvCallCenterEntity GetCurrent()
        {
            throw new NotImplementedException();
        }
    }
}
