using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class PersonMonitoringRepository : IPersonMonitoringRepository
    {
        public long? GetLastId(int interviewerId, long monitoringSessionId)
        {
            var entity = BvSpPersonMonitoring_GetLastIDAdapter.ExecuteEntityList(interviewerId, monitoringSessionId).FirstOrDefault();

            if ((entity == null) || !entity.LastSentID.HasValue)
            {
                return null;
            }

            return entity.LastSentID.Value;
        }

        public List<BvPersonMonitoringEventsEntity> GetEvents(int interviewerId, long monitoringSessionId, long lastSentId)
        {
            return BvPersonMonitoringEventsAdapter.ReadList(
                    BvSpPersonMonitoring_GetNewEventsAdapter.ExecuteReader(interviewerId, monitoringSessionId, lastSentId));
        }

        public BvPersonMonitoringEntity GetRecord(int interviewerId, string supervisorName)
        {
            return BvPersonMonitoringAdapter.GetByCondition(
                "PersonSID = @PersonSID AND SupervisorName = @SupervisorName",
                new[] { new SqlParameter("@PersonSID", interviewerId) , new SqlParameter("@SupervisorName", supervisorName) }).SingleOrDefault();            
        }

        public void SetLastId(int interviewerId, long monitoringSessionId, long lastSentId)
        {
            BvSpPersonMonitoring_SetLastIDAdapter.ExecuteNonQuery(interviewerId, monitoringSessionId, lastSentId);
        }
    }
}