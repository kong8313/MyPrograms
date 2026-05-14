using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class StateRepository : IStateRepository
    {
        public static BvStateEntity GetById(
            int stateGroupSid,
            int sid)
        {
            var entities = BvStateAdapter.GetByCondition(
                "[StateID] = @Sid AND " +
                "[StateGroupID] = @StateGroupID",
                new SqlParameter("@Sid", sid),
                new SqlParameter("@StateGroupID", stateGroupSid));

            return entities.FirstOrDefault();
        }
        //TODO: Need to remove GetByItsAndStateGroupId method, becasue it is duplicate of GetById method
        public static BvStateEntity GetByItsAndStateGroupId(
            int its,
            int stateGroupId)
        {
            return BvStateCache.Instance.GetByStateIDStateGroupID(its, stateGroupId);
        }

        BvStateEntity IStateRepository.GetByItsAndStateGroupId(
            int its,
            int stateGroupId)
        {
            return GetByItsAndStateGroupId(its, stateGroupId);
        }

        public static List<BvStateEntity> GetAll(int stateGroupSid)
        {
            var entities = BvStateAdapter.GetByCondition(
                "[StateGroupID] = @StateGroupID",
                new SqlParameter("@StateGroupID", stateGroupSid));

            return entities;
        }

        public static void Update(BvStateEntity state)
        {
            if (state.StateID == 0)
            {
                throw ExceptionManager.NewArgumentException("StateID");
            }

            BvStateGroupEntity group = StateGroupRepository.GetById(state.StateGroupID);

            var evt = new EditStateEvent(group.ID, group.Name, state.StateID, state.Name, state.Priority, state.DA != 0);

            BvStateAdapter.Update(state);
            
            BvStateCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishStateUpdated();

            evt.Finish();
        }
    }
}