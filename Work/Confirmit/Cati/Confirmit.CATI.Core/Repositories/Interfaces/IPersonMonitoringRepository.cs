using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IPersonMonitoringRepository
    {
        long? GetLastId(int interviewerId, long monitoringSessionId);

        void SetLastId(int interviewerId, long monitoringSessionId, long lastSentId);

        List<BvPersonMonitoringEventsEntity> GetEvents(int interviewerId, long monitoringSessionId, long lastSentId);

        BvPersonMonitoringEntity GetRecord(int interviewerId, string supervisorName);
    }
}