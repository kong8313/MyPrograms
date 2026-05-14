using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class BreakTypeRepository : IBreakTypeRepository
    {
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public BreakTypeRepository(ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
        }

        public List<BvBreakTypeEntity> GetAll()
        {
            return BvBreakTypeCache.Instance.GetAll();
        }

        public BvBreakTypeEntity TryGetById(int id)
        {
            return BvBreakTypeCache.Instance.GetById(id);
        }

        public void Insert(BvBreakTypeEntity entity)
        {
            var evt = new AddBreakTypeEvent(entity);

            var id = BvBreakTypeAdapter.InsertWithReturnIdentityValue(entity);

            BvBreakTypeCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishBreakTypeUpdated();
            
            evt.ObjectId = id;
            evt.Finish();
        }

        public void Update(BvBreakTypeEntity entity)
        {
            var evt = new UpdateBreakTypeEvent(entity);

            BvBreakTypeAdapter.Update(entity);

            BvBreakTypeCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishBreakTypeUpdated();
            
            evt.Finish();
        }

        public void Delete(List<int> ids)
        {
            var allBreakTypes = BvBreakTypeAdapter.GetAll();

            if (allBreakTypes.All(x => ids.Contains(x.Id)))
            {
                throw new UserMessageException("Can't remove all breaks");
            }

            foreach (var deletingBreakType in allBreakTypes.Where(x => ids.Contains(x.Id)))
            {
                var evt = new DeleteBreakTypeEvent(deletingBreakType);

                BvBreakTypeAdapter.DeleteByCondition($"[Id] = @ID", new SqlParameter("@ID", deletingBreakType.Id));

                evt.Finish();
            }

            BvBreakTypeCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishBreakTypeUpdated();
        }
    }
}
