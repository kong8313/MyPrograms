using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIPersonMonitoringRepository : IPersonMonitoringRepository 
    {
        private IPersonMonitoringRepository _inner;

        public StubIPersonMonitoringRepository()
        {
            _inner = null;
        }

        public IPersonMonitoringRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate long? GetLastIdInt32Int64Delegate(int interviewerId, long monitoringSessionId);
        public GetLastIdInt32Int64Delegate GetLastIdInt32Int64;

        long? IPersonMonitoringRepository.GetLastId(int interviewerId, long monitoringSessionId)
        {


            if (GetLastIdInt32Int64 != null)
            {
                return GetLastIdInt32Int64(interviewerId, monitoringSessionId);
            } else if (_inner != null)
            {
                return ((IPersonMonitoringRepository)_inner).GetLastId(interviewerId, monitoringSessionId);
            }

            return default(long?);
        }

        public delegate void SetLastIdInt32Int64Int64Delegate(int interviewerId, long monitoringSessionId, long lastSentId);
        public SetLastIdInt32Int64Int64Delegate SetLastIdInt32Int64Int64;

        void IPersonMonitoringRepository.SetLastId(int interviewerId, long monitoringSessionId, long lastSentId)
        {

            if (SetLastIdInt32Int64Int64 != null)
            {
                SetLastIdInt32Int64Int64(interviewerId, monitoringSessionId, lastSentId);
            } else if (_inner != null)
            {
                ((IPersonMonitoringRepository)_inner).SetLastId(interviewerId, monitoringSessionId, lastSentId);
            }
        }

        public delegate List<BvPersonMonitoringEventsEntity> GetEventsInt32Int64Int64Delegate(int interviewerId, long monitoringSessionId, long lastSentId);
        public GetEventsInt32Int64Int64Delegate GetEventsInt32Int64Int64;

        List<BvPersonMonitoringEventsEntity> IPersonMonitoringRepository.GetEvents(int interviewerId, long monitoringSessionId, long lastSentId)
        {


            if (GetEventsInt32Int64Int64 != null)
            {
                return GetEventsInt32Int64Int64(interviewerId, monitoringSessionId, lastSentId);
            } else if (_inner != null)
            {
                return ((IPersonMonitoringRepository)_inner).GetEvents(interviewerId, monitoringSessionId, lastSentId);
            }

            return default(List<BvPersonMonitoringEventsEntity>);
        }

        public delegate BvPersonMonitoringEntity GetRecordInt32StringDelegate(int interviewerId, string supervisorName);
        public GetRecordInt32StringDelegate GetRecordInt32String;

        BvPersonMonitoringEntity IPersonMonitoringRepository.GetRecord(int interviewerId, string supervisorName)
        {


            if (GetRecordInt32String != null)
            {
                return GetRecordInt32String(interviewerId, supervisorName);
            } else if (_inner != null)
            {
                return ((IPersonMonitoringRepository)_inner).GetRecord(interviewerId, supervisorName);
            }

            return default(BvPersonMonitoringEntity);
        }

    }
}