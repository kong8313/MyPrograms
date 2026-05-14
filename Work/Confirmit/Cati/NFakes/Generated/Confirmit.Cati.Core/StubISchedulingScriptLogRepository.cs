using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubISchedulingScriptLogRepository : ISchedulingScriptLogRepository 
    {
        private ISchedulingScriptLogRepository _inner;

        public StubISchedulingScriptLogRepository()
        {
            _inner = null;
        }

        public ISchedulingScriptLogRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CleanUpExpiredRecordsTimeSpanDelegate(TimeSpan expirationPeriod);
        public CleanUpExpiredRecordsTimeSpanDelegate CleanUpExpiredRecordsTimeSpan;

        void ISchedulingScriptLogRepository.CleanUpExpiredRecords(TimeSpan expirationPeriod)
        {

            if (CleanUpExpiredRecordsTimeSpan != null)
            {
                CleanUpExpiredRecordsTimeSpan(expirationPeriod);
            } else if (_inner != null)
            {
                ((ISchedulingScriptLogRepository)_inner).CleanUpExpiredRecords(expirationPeriod);
            }
        }

        public delegate List<BvSchedulingScriptLogEntity> GetByInterviewIdInt32Int32Delegate(int surveyId, int interviewId);
        public GetByInterviewIdInt32Int32Delegate GetByInterviewIdInt32Int32;

        List<BvSchedulingScriptLogEntity> ISchedulingScriptLogRepository.GetByInterviewId(int surveyId, int interviewId)
        {


            if (GetByInterviewIdInt32Int32 != null)
            {
                return GetByInterviewIdInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptLogRepository)_inner).GetByInterviewId(surveyId, interviewId);
            }

            return default(List<BvSchedulingScriptLogEntity>);
        }

    }
}