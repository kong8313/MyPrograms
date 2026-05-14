using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    /// <summary>
    /// Class contains methods that implement operations with dialers.
    /// </summary>
    public class DialersRepository : IDialersRepository
    {
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public DialersRepository()
        {
            _sqlTableUpdatedPublisher = ServiceLocator.Resolve<ISqlTableUpdatedPublisher>();
        }

        /// <summary>
        /// Gets dialer entity by dialer id.
        /// </summary>
        /// <param name="id">Dialer id.</param>
        /// <returns>Dialer entity.</returns>
        public BvDialersEntity GetById(int id)
        {
            var entities = BvDialersAdapter.GetByCondition(
                "[Id] = @Id",
                new SqlParameter("@Id", id));

            return entities.FirstOrDefault();
        }
        
        public void Update(BvDialersEntity dialerEntity, bool useNotification = true)
        {
            BvDialersAdapter.Update(dialerEntity);
            if (useNotification)
            {
                _sqlTableUpdatedPublisher.PublishDialersUpdated();
            }
        }

        /// <summary>
        /// Gets list of all available dialer entities.
        /// </summary>
        /// <returns>List of all available dialer entities.</returns>
        public List<BvDialersEntity> GetAll()
        {
            return BvDialersAdapter.GetAll();
        }

        public bool IsAnyDialerConfigured()
        {
            return GetAll().Any();
        }

        public int? GetNextAvailableDialer(int surveyId, DialType dialType, int callCenterId = 0)
        {
            var transactionOptions = new DatabaseTransactionOptions("GetNextAvailableDialer");
            using (var transaction = new DatabaseTransactionScope(transactionOptions))
            {
                var dialerIds = string.Empty;
                if (callCenterId != 0)
                {
                    dialerIds = string.Join(",", BvDialerToCallCenterAdapter
                        .GetByCondition("CallCenterId = @CallCenterId", new SqlParameter("@CallCenterId", callCenterId))
                        .Select(x => x.DialerId));
                }
                var dialerId = BvSpGetNextAvailableDialerAdapter.ExecuteScalar<int?>(surveyId, (byte)dialType, dialerIds);
                transaction.Commit();

                return dialerId != -1 ? dialerId : null;
            }
        }

        public BvDialersEntity AddDialer(BvDialersEntity dialer)
        {
            BvDialersAdapter.Insert(dialer);
            _sqlTableUpdatedPublisher.PublishDialersUpdated();
            return GetById(dialer.Id);
        }

        public void Delete(int dialerId)
        {
            BvDialersAdapter.DeleteByCondition(
                "[Id] = @Id",
                new SqlParameter("@Id", dialerId));
            _sqlTableUpdatedPublisher.PublishDialersUpdated();
        }
    }
}
