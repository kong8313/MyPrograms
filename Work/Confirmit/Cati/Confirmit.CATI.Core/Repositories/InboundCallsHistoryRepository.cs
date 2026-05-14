using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class InboundCallsHistoryRepository : IInboundCallsHistoryRepository
    {
        public void Insert(BvInboundCallsHistoryEntity entity)
        {
            BvInboundCallsHistoryAdapter.Insert(entity);
        }

        public BvInboundCallsHistoryEntity GetById(int id)
        {
            var entities = BvInboundCallsHistoryAdapter.GetByCondition(
                "[Id] = @Id",
                new SqlParameter("@Id", id));

            return entities.FirstOrDefault();
        }
    }
}
