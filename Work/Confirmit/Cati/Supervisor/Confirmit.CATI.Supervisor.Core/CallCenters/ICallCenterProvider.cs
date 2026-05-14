using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public interface ICallCenterProvider
    {
        int GetCurrentId();
        BvCallCenterEntity GetCurrent();
    }
}
