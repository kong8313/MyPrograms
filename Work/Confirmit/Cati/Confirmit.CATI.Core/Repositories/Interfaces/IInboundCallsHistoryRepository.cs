using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IInboundCallsHistoryRepository
    {
        void Insert(BvInboundCallsHistoryEntity entity);

        BvInboundCallsHistoryEntity GetById(int id);
    }
}
