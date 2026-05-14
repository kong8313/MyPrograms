using System;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubICallHistoryRepository : ICallHistoryRepository 
    {
        private ICallHistoryRepository _inner;

        public StubICallHistoryRepository()
        {
            _inner = null;
        }

        public ICallHistoryRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CleanUpExpiredRecordsTimeSpanDelegate(TimeSpan callHistoryCleanPeriod);
        public CleanUpExpiredRecordsTimeSpanDelegate CleanUpExpiredRecordsTimeSpan;

        void ICallHistoryRepository.CleanUpExpiredRecords(TimeSpan callHistoryCleanPeriod)
        {

            if (CleanUpExpiredRecordsTimeSpan != null)
            {
                CleanUpExpiredRecordsTimeSpan(callHistoryCleanPeriod);
            } else if (_inner != null)
            {
                ((ICallHistoryRepository)_inner).CleanUpExpiredRecords(callHistoryCleanPeriod);
            }
        }

    }
}