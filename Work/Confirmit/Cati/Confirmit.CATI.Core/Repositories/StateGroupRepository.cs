using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class StateGroupRepository : IStateGroupRepository
    {
        public static BvStateGroupEntity GetById(int sid)
        {
            var entities = BvStateGroupAdapter.GetByCondition(
                "[ID] = @Sid",
                new SqlParameter("@Sid", sid));

            return entities.FirstOrDefault();
        }

        public static BvStateGroupEntity GetByName(string name)
        {
            return BvStateGroupAdapter.GetByCondition(
                "Name = @Name",
                new SqlParameter("@Name", name)).FirstOrDefault();
        }

        public static List<BvStateGroupEntity> GetAll()
        {
            return BvStateGroupAdapter.GetAll();
        }

        public static int Insert(int copySourceStateGroupSid,BvStateGroupEntity stateGroup)
        {
            if (stateGroup.ID != 0)
            {
                throw ExceptionManager.NewArgumentException("ID");
            }

            stateGroup.ID = SiteService.GetNewSid();

            var evt = GetCreateEvent(stateGroup.ID, stateGroup.Name, copySourceStateGroupSid, copySourceStateGroupSid != 0);

            BvSpStateGroup_InsertAdapter.ExecuteNonQuery(stateGroup.ID, copySourceStateGroupSid, stateGroup.Name);

            BvStateCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishStateUpdated();
            
            evt.Finish();

            return stateGroup.ID;
        }

        public static void Update(BvStateGroupEntity stateGroup)
        {
            if (stateGroup.ID == 0)
            {
                throw ExceptionManager.NewArgumentException("ID");
            }

            BvSpStateGroup_UpdateAdapter.ExecuteNonQuery(
                stateGroup.ID,
                stateGroup.Name);
        }

        public static void Delete(int sid)
        {
            if (sid == 0)
            {
                throw ExceptionManager.NewArgumentException("sid");
            }

            var stateGroup = GetById(sid);

            var evt = new DeleteStateGroupEvent(sid, stateGroup.Name);

            BvSpStateGroup_DeleteAdapter.ExecuteNonQuery(sid);
            BvStateCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishSurveyUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishStateUpdated();
            
            evt.Finish();
        }

        /// <summary>
        /// Gets the default state group entity.
        /// </summary>
        /// <remarks>Default state group should have minimum value in the 'Order' column.</remarks>
        public static BvStateGroupEntity GetDefault()
        {
            var entities = BvStateGroupAdapter.GetByCondition("[Order] = (SELECT MIN([Order]) FROM BvStateGroup)");

            return entities.FirstOrDefault();
        }

        BvStateGroupEntity IStateGroupRepository.GetDefault()
        {
            return GetDefault();
        }

        private static IActivityEvent GetCreateEvent(
            int stateGroupId, 
            string stateGroupName, 
            int baseStateGroupId, 
            bool isDuplicate)
        {
            if (isDuplicate)
            {
                return new DuplicateStateGroupEvent(stateGroupId, stateGroupName, baseStateGroupId);
            }
            else
            {
                return new CreateStateGroupEvent(stateGroupId, stateGroupName);
            }
        }
    }
}
